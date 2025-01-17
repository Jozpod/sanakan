﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Models.Enums;
using Shinden.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Fake
{
    /// <summary>
    /// Implements simple shinden webscraper and lookup database.
    /// </summary>
    internal class FakeShindenClient : IShindenClient, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IServiceScope _serviceScope;
        private readonly WebScrapedDbContext _dbContext;
        private readonly ShindenWebScraper _shindenWebScraper;

        public FakeShindenClient(
            ILogger<FakeShindenClient> logger,
            IServiceScopeFactory serviceScopeFactory,
            ShindenWebScraper shindenWebScraper)
        {
            _logger = logger;
            _serviceScope = serviceScopeFactory.CreateScope();
            _dbContext = _serviceScope.ServiceProvider.GetRequiredService<WebScrapedDbContext>();
            _shindenWebScraper = shindenWebScraper;

            var created = _dbContext.Database.EnsureCreated();

            if (created)
            {
                _logger.LogError("No database found. The api");
            }
        }

        public void Dispose()
        {
            _serviceScope.Dispose();
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
                Value = new TitleStatusAfterChange(),
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
            var animeDetails = await _shindenWebScraper.GetAnimeDetailAsync(titleId);
            var mangaDetails = await _shindenWebScraper.GetMangaDetailAsync(titleId);
            var title = string.Empty;
            ulong? imageId = null;
            IllustrationType type = IllustrationType.Anime;

            if (animeDetails != null)
            {
                title = animeDetails.Name;
                type = IllustrationType.Anime;
                imageId = animeDetails.ImageId;
            }

            if (mangaDetails != null)
            {
                title = mangaDetails.Name;
                type = IllustrationType.Manga;
                imageId = mangaDetails.ImageId;
            }

            return new ShindenResult<AnimeMangaInfo>
            {
                Value = new AnimeMangaInfo
                {
                    Title = new TitleEntry
                    {
                        TitleId = titleId,
                        Title = title,
                        CoverId = imageId ?? 0,
                        Type = type,
                        Manga = new MangaInfo(),
                        Anime = new AnimeInfo(),
                        Description = new AnimeMangaInfoDescription(),
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
                    PictureId = character?.ImageId,
                    Biography = new CharacterBio
                    {
                        Biography = character?.Biography,
                    },
                    FirstName = character?.Name ?? $"Test character {characterId}"
                }
            };
        }

        public async Task<ShindenResult<TitleCharacters>> GetCharactersAsync(ulong titleId)
        {
            var characterIds = await _dbContext.Characters
                .Where(pr => pr.Illustrations.Any(npr => npr.Id == titleId))
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
            var animeDetails = await _shindenWebScraper.GetAnimeDetailAsync(titleId);
            var mangaDetails = await _shindenWebScraper.GetMangaDetailAsync(titleId);
            var title = string.Empty;
            IllustrationInfoTitle? entry = null;

            ulong? imageId;
            if (animeDetails != null)
            {
                title = animeDetails.Name;
                imageId = animeDetails.ImageId;
                entry = new IllustrationInfoTitleAnime
                {
                    CoverId = imageId,
                    Title = title,
                };
            }

            if (mangaDetails != null)
            {
                title = mangaDetails.Name;
                imageId = mangaDetails.ImageId;
                entry = new IllustrationInfoTitleManga
                {
                    CoverId = imageId,
                    Title = title,
                };
            }

            return new ShindenResult<IllustrationInfo>
            {
                Value = new IllustrationInfo
                {
                    Entry = entry!,
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

        public async Task<ShindenResult<UserInfo>> GetUserInfoAsync(ulong userId)
        {
            var user = await _shindenWebScraper.GetUserAsync(userId);

            if(user == null)
            {
                return new ShindenResult<UserInfo>
                {
                    Value = null,
                };
            }

            return new ShindenResult<UserInfo>
            {
                Value = new UserInfo
                {
                    Id = user.Id,
                    Name = user.Username,
                },
            };
        }

        public Task<ShindenResult<IncreaseWatched>> IncreaseNumberOfWatchedEpisodesAsync(ulong userId, ulong titleId)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<LogInResult>> LoginAsync(string username, string password)
        {
            return Task.FromResult(new ShindenResult<LogInResult>
            {
                Value = new LogInResult()
            });
        }

        public async Task<ShindenResult<List<QuickSearchResult>>> QuickSearchAsync(string search, QuickSearchType type)
        {
            List<QuickSearchResult> results;

            if (type == QuickSearchType.Anime)
            {
                var details = await _shindenWebScraper.GetAnimeDetailsAsync(search: search);

                results = details.Select(pr => new QuickSearchResult
                {
                    TitleId = pr.Id,
                    Title = pr.Name,
                }).ToList();
            }
            else
            {
                var details = await _shindenWebScraper.GetMangaDetailsAsync(search: search);

                results = details.Select(pr => new QuickSearchResult
                {
                    TitleId = pr.Id,
                    Title = pr.Name,
                }).ToList();
            }

            return new ShindenResult<List<QuickSearchResult>>
            {
                Value = results,
            };
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
                Value = new Status(),
            });
        }

        public Task<ShindenResult<Status>> RateMangaAsync(ulong titleId, MangaRateType type, uint value)
        {
            return Task.FromResult(new ShindenResult<Status>
            {
                Value = new Status(),
            });
        }

        public Task<ShindenResult<Modification>> RemoveFromFavouritesAsync(ulong userId, FavouriteType favouriteType, ulong favouriteId)
        {
            return Task.FromResult(new ShindenResult<Modification>
            {
                Value = new Modification(),
            });
        }

        public Task<ShindenResult<TitleStatusAfterChange>> RemoveTitleFromListAsync(ulong userId, ulong titleId)
        {
            return Task.FromResult(new ShindenResult<TitleStatusAfterChange>
            {
                Value = new TitleStatusAfterChange(),
            });
        }

        public Task<ShindenResult<List<StaffSearchResult>>> SearchStaffAsync(string name)
        {
            throw new NotImplementedException();
        }
    }
}
