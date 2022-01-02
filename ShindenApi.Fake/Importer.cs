using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sanakan.ShindenApi.Fake.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task RunAsync()
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

            var pagesLeft = 20;

            while(pagesLeft > 0)
            {
                var animeDetails = await _shindenWebScraper.GetAnimeDetailsAsync(page: scrapeStats.Page);

                var illustrations = new List<Illustration>();

                foreach (var animeDetail in animeDetails)
                {
                    if (await _dbContext.Illustrations.AnyAsync(pr => pr.Id == animeDetail.Id))
                    {
                        _logger.LogWarning($"Skipping {animeDetail.Name}");
                        continue;
                    }

                    var illustration = new Illustration
                    {
                        Id = animeDetail.Id,
                        Name = animeDetail.Name,
                        Type = IllustrationType.Anime
                    };

                    _dbContext.Illustrations.Add(illustration);
                    await _dbContext.SaveChangesAsync();

                    var details = await _shindenWebScraper.GetAnimeDetailAsync(animeDetail.Id);

                    if(details == null)
                    {
                        _logger.LogError($"Could not retrieve information for: {animeDetail.Name}");
                        continue;
                    }

                    foreach (var character in details.Characters)
                    {
                        var characterEntity = await _dbContext.Characters.FindAsync(character.Id);

                        if (characterEntity == null)
                        {
                            characterEntity = new Character
                            {
                                Id = character.Id,
                                ImageId = character.ImageId,
                                Name = character.Name,
                                Biography = character.Biography,
                            };

                            _dbContext.Characters.Add(characterEntity);
                        }

                        illustration.Characters.Add(characterEntity);
                    }

                    await _dbContext.SaveChangesAsync();
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