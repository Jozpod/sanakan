using Microsoft.EntityFrameworkCore;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Models.Enums;
using Shinden.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Fake
{
    /// <summary>
    /// Implements simple shinden webscraper and lookup database.
    /// </summary>
    internal class FakeShindenClient : IShindenClient
    {
        private readonly WebScrapedDbContext _dbContext;
        private readonly ShindenWebScraper _shindenWebScraper;

        public FakeShindenClient(
            WebScrapedDbContext dbContext,
            ShindenWebScraper shindenWebScraper)
        {
            _dbContext = dbContext;
            _shindenWebScraper = shindenWebScraper;
        }

        public Task<ShindenResult<Modification>> AddToFavouritesAsync(ulong userId, FavouriteType favouriteType, ulong favouriteId)
        {
            return Task.FromResult(new ShindenResult<Modification>
            {
                Value = new Modification
                {
                    Updated = string.Empty,
                }
            });
        }

        public Task<ShindenResult<TitleStatusAfterChange>> ChangeTitleStatusAsync(ulong userId, ListType status, ulong titleId)
        {
            return Task.FromResult(new ShindenResult<TitleStatusAfterChange>
            {
                Value = new TitleStatusAfterChange
                {
                    
                }
            });
        }

        public async Task<ShindenResult<List<ulong>>> GetAllCharactersAsync()
        {
            return new ShindenResult<List<ulong>>
            {
                Value = await _dbContext.Characters.Select(pr => pr.Id).ToListAsync(),
            };
        }

        public async Task<ShindenResult<IEnumerable<ulong>>> GetAllCharactersFromAnimeAsync()
        {
            return new ShindenResult<IEnumerable<ulong>>
            {
                Value = await _dbContext.Characters
                    .Where(pr => pr.Illustrations.Any(npr => npr.Type == Models.IllustrationType.Anime))
                    .Select(pr => pr.Id)
                    .ToListAsync(),
            };
        }

        public async Task<ShindenResult<List<ulong>>> GetAllCharactersFromMangaAsync()
        {
            return new ShindenResult<List<ulong>>
            {
                Value = await _dbContext.Characters
                    .Where(pr => pr.Illustrations.Any(npr => npr.Type == Models.IllustrationType.Manga))
                    .Select(pr => pr.Id)
                    .ToListAsync(),
            };
        }

        public async Task<ShindenResult<List<CharacterSearchResult>>> SearchCharacterAsync(string name)
        {
            var characters = await _shindenWebScraper.GetCharactersAsync(name);

            return new ShindenResult<List<CharacterSearchResult>>
            {
                Value = characters.Select(pr => new CharacterSearchResult
                {
                    Id = pr.Id,
                    FirstName = pr.Name,
                }).ToList(),
            };
        }

        public async Task<ShindenResult<List<UserSearchResult>>> SearchUserAsync(string nick)
        {
            var users = await _shindenWebScraper.GetUsersAsync(nick);

            return new ShindenResult<List<UserSearchResult>>
            {
                Value = users.Select(pr => new UserSearchResult
                {
                    Id = pr.Id,
                    Name = pr.Username,
                }).ToList(),
            };
        }

        public async Task<ShindenResult<AnimeMangaInfo>> GetAnimeMangaInfoAsync(ulong titleId)
        {
            return new ShindenResult<AnimeMangaInfo>
            {
                Value = new AnimeMangaInfo
                {
                    Title = new TitleEntry
                    {
                        TitleId = titleId,
                        Description = new AnimeMangaInfoDescription
                        {
                            
                        }
                    }
                },
            };
        }

        public async Task<ShindenResult<CharacterInfo>> GetCharacterInfoAsync(ulong characterId)
        {
            var character = await _dbContext.Characters.FindAsync(characterId);

            return new ShindenResult<CharacterInfo>
            {
                Value = new CharacterInfo()
                {
                    CharacterId = characterId,
                    FirstName = character?.Name ?? $"Test character {characterId}"
                }
            };
        }

        public async Task<ShindenResult<TitleCharacters>> GetCharactersAsync(ulong titleId)
        {
            var characterIds = await _dbContext.Characters
                .Where(pr => pr.Illustrations.Any(npr => npr.Type == Models.IllustrationType.Manga))
                .Select(pr => pr.Id)
                .ToListAsync();
            
            var titleCharacters = new TitleCharacters();

            titleCharacters.Relations = characterIds.Select(pr => new StaffInfoRelation
            {
                CharacterId = pr
            }).ToList();

            return new ShindenResult<TitleCharacters>
            {
                Value = titleCharacters,
            };
        }

        public Task<ShindenResult<TitleEpisodes>> GetEpisodesAsync(ulong episodeId)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<EpisodesRange>> GetEpisodesRangeAsync(ulong episodeId)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<List<FavCharacter>>> GetFavouriteCharactersAsync(ulong userId)
        {
            return Task.FromResult(new ShindenResult<List<FavCharacter>>
            {
                Value = new List<FavCharacter>(),
            });
        }

        public async Task<ShindenResult<IllustrationInfo>> GetIllustrationInfoAsync(ulong titleId)
        {
            return new ShindenResult<IllustrationInfo>
            {
                Value = new IllustrationInfo
                {

                }
            };
        }

        public Task<ShindenResult<List<LastWatchedRead>>> GetLastReadAsync(ulong userId, uint limit = 5)
        {
            return Task.FromResult(new ShindenResult<List<LastWatchedRead>>
            {
                Value = new List<LastWatchedRead>(),
            });
        }

        public Task<ShindenResult<List<LastWatchedRead>>> GetLastWatchedAsync(ulong userId, uint limit = 5)
        {
            return Task.FromResult(new ShindenResult<List<LastWatchedRead>>
            {
                Value = new List<LastWatchedRead>(),
            });
        }

        public Task<ShindenResult<List<NewEpisode>>> GetNewEpisodesAsync()
        {
            return Task.FromResult(new ShindenResult<List<NewEpisode>>
            {
                Value = new List<NewEpisode>(),
            });
        }

        public Task<ShindenResult<TitleRecommendation>> GetRecommendationsAsync(ulong titleId)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<TitleRelations>> GetRelationsAsync(ulong titleId)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<TitleReviews>> GetReviewsAsync(ulong reviewId)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<StaffInfo>> GetStaffInfoAsync(ulong staffId)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<UserInfo>> GetUserInfoAsync(ulong userId)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<IncreaseWatched>> IncreaseNumberOfWatchedEpisodesAsync(ulong userId, ulong titleId)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<LogInResult>> LoginAsync(string username, string password)
        {
            return Task.FromResult(new ShindenResult<LogInResult>
            {
                Value = new LogInResult
                {

                }
            });
        }

        public Task<ShindenResult<List<QuickSearchResult>>> QuickSearchAsync(string search, QuickSearchType type)
        {
            return Task.FromResult(new ShindenResult<List<QuickSearchResult>>
            {
                Value = new List<QuickSearchResult>(),
            });
        }

        public Task<ShindenResult<List<QuickSearchResult>>> QuickSearchAsync(string search)
        {
            return Task.FromResult(new ShindenResult<List<QuickSearchResult>>
            {
                Value = new List<QuickSearchResult>(),
            });
        }

        public Task<ShindenResult<Status>> RateAnimeAsync(ulong titleId, AnimeRateType type, uint value)
        {
            return Task.FromResult(new ShindenResult<Status>
            {
                Value = new Status
                {

                }
            });
        }

        public Task<ShindenResult<Status>> RateMangaAsync(ulong titleId, MangaRateType type, uint value)
        {
            return Task.FromResult(new ShindenResult<Status>
            {
                Value = new Status
                {

                }
            });
        }

        public Task<ShindenResult<Modification>> RemoveFromFavouritesAsync(ulong userId, FavouriteType favouriteType, ulong favouriteId)
        {
            return Task.FromResult(new ShindenResult<Modification>
            {
                Value = new Modification
                {

                }
            });
        }

        public Task<ShindenResult<TitleStatusAfterChange>> RemoveTitleFromListAsync(ulong userId, ulong titleId)
        {
            return Task.FromResult(new ShindenResult<TitleStatusAfterChange>
            {
                Value = new TitleStatusAfterChange
                {

                }
            });
        }

        public Task<ShindenResult<List<StaffSearchResult>>> SearchStaffAsync(string name)
        {
            throw new NotImplementedException();
        }
    }
}
