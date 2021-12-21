using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sanakan.ShindenApi.Fake.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Fake
{
    public class ShindenWebScraper
    {
        private readonly ILogger _logger;
        private readonly WebScrapedDbContext _dbContext;
        private readonly HttpClient _httpClient;

        public ShindenWebScraper(
            ILogger<ShindenWebScraper> logger,
            WebScrapedDbContext dbContext,
            HttpClient httpClient)
        {
            _logger = logger;
            _dbContext = dbContext;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://shinden.pl/");
            _httpClient.DefaultRequestHeaders.UserAgent
                .ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36");
        }

        public IllustrationType ParseType(string illustrationType)
        {
            switch (illustrationType.ToLower())
            {
                case "anime":
                    return IllustrationType.Anime;
                case "manga":
                    return IllustrationType.Manga;
                case "novel":
                    return IllustrationType.Novel;
                default:
                    return IllustrationType.Unknown;
            }
        }

        public async Task Test()
        {
            await _dbContext.Database.EnsureDeletedAsync();
            await _dbContext.Database.EnsureCreatedAsync();

            var scrapeStats = _dbContext.ScrapeStats.FirstOrDefault();
            var idInLinkPattern = new Regex(@"\/(\d+)\-", RegexOptions.Compiled);

            if (scrapeStats == null)
            {
                scrapeStats = new Models.ScrapeStats
                {
                    Page = 0,
                    ModifiedOn = DateTime.UtcNow,
                };
                _dbContext.ScrapeStats.Add(scrapeStats);
                await _dbContext.SaveChangesAsync();
            }

            var pagesLeft = 10;

            while(pagesLeft > 0)
            {
                var query = $"/series?sort_by=ranking-rate&sort_order=desc&page={scrapeStats.Page}";
                _logger.LogInformation($"Processing {query}");

                var seriesDocument = await GetDocument(query);

                var serieNodes = seriesDocument
                    .SelectNodes("/html/body/div[2]/div/section[2]/section/article/ul");

                var illustrations = new List<Illustration>();

                foreach (var serieNode in serieNodes)
                {
                    var linkNode = serieNode.SelectSingleNode("./li[2]/h3/a");
                    var serieLink = linkNode.Attributes.FirstOrDefault(pr => pr.Name == "href").Value;
                    var serieName = linkNode.InnerText;
                    var match = idInLinkPattern.Match(serieLink);
                    var serieidStr = match.Groups[1].Value;
                    var serieId = int.Parse(serieidStr);

                    if (await _dbContext.Illustrations.AnyAsync(pr => pr.Id == serieId))
                    {
                        _logger.LogInformation($"Skipping {serieName}");
                        continue;
                    }

                    var illustration = new Illustration
                    {
                        Id = serieId,
                        Name = serieName,
                        Type = IllustrationType.Unknown
                    };

                    _dbContext.Illustrations.Add(illustration);
                    await _dbContext.SaveChangesAsync();

                    var serieDocument = await GetDocument(serieLink);
                    var characterNodes = serieDocument
                        .SelectNodes("/html/body/div[2]/div/article/div/section[4]/section/div");

                    var titleNode = serieDocument.SelectSingleNode("/html/body/div[4]/div/h1");
                    var illustrationType = titleNode.InnerText.Split(":")[0];
                    illustration.Type = ParseType(illustrationType);
                    await _dbContext.SaveChangesAsync();

                    foreach (var characterNode in characterNodes)
                    {
                        var characterLinkNode = serieNode.SelectSingleNode("./span[1]/div/h3/a");
                        var imageIdStr = characterLinkNode.GetDataAttribute("over-image").Value;
                        var imageId = int.Parse(imageIdStr);
                        var characterName = characterLinkNode.InnerText;
                        var characterLink = characterLinkNode.Attributes.FirstOrDefault(pr => pr.Name == "href").Value;
                        match = idInLinkPattern.Match(characterLink);
                        var characterId = int.Parse(match.Groups[1].Value);

                        var character = new Character
                        {
                            Id = characterId,
                            ImageId = imageId,
                            Name = characterName,
                        };

                        illustration.Characters.Add(character);
                    }

                    await _dbContext.SaveChangesAsync();
                }

                scrapeStats.Page++;
                scrapeStats.ModifiedOn = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
                pagesLeft--;
            }
        }

        public async Task<HtmlNode> GetDocument(string requireUri)
        {
            var response = await _httpClient.GetAsync(requireUri);
            var stream = await response.Content.ReadAsStreamAsync();
            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(stream);
            return htmlDocument.DocumentNode;
        }
    }
}
