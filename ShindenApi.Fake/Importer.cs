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
    public class Importer
    {
        private readonly ILogger _logger;
        private readonly WebScrapedDbContext _dbContext;
        private readonly ShindenWebScraper _shindenWebScraper;

        public Importer(
            ILogger<Importer> logger,
            WebScrapedDbContext dbContext,
            ShindenWebScraper shindenWebScraper)
        {
            _logger = logger;
            _dbContext = dbContext;
            _shindenWebScraper = shindenWebScraper;
        }

        public async Task Test()
        {
            await _dbContext.Database.EnsureDeletedAsync();
            await _dbContext.Database.EnsureCreatedAsync();

            var scrapeStats = _dbContext.ScrapeStats.FirstOrDefault();

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
                var animeDetails = await _shindenWebScraper.GetAnimeDetailsAsync(scrapeStats.Page);

                //var serieNodes = seriesDocument
                //    .SelectNodes("/html/body/div[2]/div/section[2]/section/article/ul");

                var illustrations = new List<Illustration>();

                foreach (var animeDetail in animeDetails)
                {

                    if (await _dbContext.Illustrations.AnyAsync(pr => pr.Id == animeDetail.Id))
                    {
                        _logger.LogInformation($"Skipping {animeDetail.Name}");
                        continue;
                    }

                    var illustration = new Illustration
                    {
                        Id = animeDetail.Id,
                        Name = animeDetail.Name,
                        Type = IllustrationType.Unknown
                    };

                    _dbContext.Illustrations.Add(illustration);
                    await _dbContext.SaveChangesAsync();

                    //var serieDocument = await _shindenWebScraper.GetAnimeDetailAsync(animeDetail.Id);
                    //var characterNodes = serieDocument
                    //    .SelectNodes("/html/body/div[2]/div/article/div/section[4]/section/div");

                    //var titleNode = serieDocument.SelectSingleNode("//h1[contains(@class, 'page-title')]");
                    //var illustrationType = titleNode.InnerText.Split(":")[0];
                    //illustration.Type = ParseType(illustrationType);
                    //await _dbContext.SaveChangesAsync();

                    //foreach (var characterNode in characterNodes)
                    //{

                    //    var character = await _dbContext.Characters.FindAsync(characterId);

                    //    if(character == null)
                    //    {
                    //        character = new Character
                    //        {
                    //            Id = characterId,
                    //            ImageId = imageId,
                    //            Name = characterName,
                    //        };

                    //        _dbContext.Characters.Add(character);
                    //    }

                    //    illustration.Characters.Add(character);
                    //}

                    //await _dbContext.SaveChangesAsync();
                }

                scrapeStats.Page++;
                scrapeStats.ModifiedOn = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
                pagesLeft--;
            }

            _logger.LogInformation($"Finished importing....");
        }

    
    }
}
