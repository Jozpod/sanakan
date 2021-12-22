using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Sanakan.ShindenApi.Fake.Models;
using Sanakan.ShindenApi.Fake.Models.WebScraper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Fake
{
    public class ShindenWebScraper
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public ShindenWebScraper(
            ILogger<ShindenWebScraper> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient(nameof(ShindenWebScraper));
            _httpClient.BaseAddress = new Uri("https://shinden.pl/");
            _httpClient.DefaultRequestHeaders.UserAgent
                .ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36");
        }

        public IllustrationType ParseType(string illustrationType)
        {
            switch (illustrationType.ToLowerInvariant())
            {
                case "anime":
                    return IllustrationType.Anime;
                case "manga":
                    return IllustrationType.Manga;
                case "novel":
                    return IllustrationType.Novel;
                case "webtoon":
                    return IllustrationType.WebToon;
                case "webnovel":
                    return IllustrationType.WebNovel;
                default:
                    return IllustrationType.Unknown;
            }
        }

        public async Task<UserDetail?> GetUserAsync(ulong userId)
        {
            var query = $"/user/{userId}";
            var document = await _httpClient.GetDocumentAsync(query);

            if(document == null)
            {
                return null;
            }

            var usernameNode = document.SelectSingleNode("//li/strong");
            var username = usernameNode.InnerText;
            var result = new UserDetail(userId, username);

            return result;
        }

        public async Task<IEnumerable<UserDetail>> GetUsersAsync(string name)
        {
            var query = $"/users?type=contains&search={name}";
            var document = await _httpClient.GetDocumentAsync(query);

            var info = document.SelectSingleNode("/html/body/div[4]/div/section[2]/div");

            if (info != null)
            {
                return Enumerable.Empty<UserDetail>();
            }

            var userNodes = document.SelectNodes("//li[contains(@class, 'user-list-item')]");
            var results = new List<UserDetail>();

            foreach (var userNode in userNodes)
            {
                var linkNode = userNode.SelectSingleNode("./div/h3/a");
                var link = linkNode.Attributes.FirstOrDefault(pr => pr.Name == "href").Value;
                var username = linkNode.InnerText;
                var id = link.GetIdFromLink();
                results.Add(new UserDetail(id!.Value, username));
            }

            return results;
        }

        public async Task<CharacterDetail?> GetCharacterAsync(ulong characterId)
        {
            var query = $"/character/{characterId}";
            var document = await _httpClient.GetDocumentAsync(query);

            if (document == null)
            {
                return null;
            }

            var (type, name) = ParsePageTitle(document);
            var imageId = GetSideImageId(document);
            var result = new CharacterDetail(characterId, name, imageId);

            return result;
        }

        public async Task<IEnumerable<CharacterDetail>> GetCharactersAsync(string name)
        {
            var query = $"/character?type=contains&search={name}";
            var document = await _httpClient.GetDocumentAsync(query);

            var characterNodes = document
               .SelectNodes("//li[@class = 'data-view-list']");
            var results = new List<CharacterDetail>();

            foreach (var characterNode in characterNodes)
            {
                var linkNode = characterNode.SelectSingleNode("./div/div[2]/h3/a");
                var link = linkNode.Attributes.FirstOrDefault(pr => pr.Name == "href").Value;
                var characterName = linkNode.InnerText;
                var id = link.GetIdFromLink();
                var characterImageNode = characterNode.SelectSingleNode("./div/div/img");
                var imageLink = characterImageNode.Attributes.FirstOrDefault(pr => pr.Name == "src").Value;
                var imageId = imageLink.GetIdFromLink();
                results.Add(new CharacterDetail(id!.Value, characterName, imageId));
            }
            
            return results;
        }

        public async Task<MangaDetail?> GetMangaDetailAsync(ulong mangaId)
        {
            var query = $"/manga/{mangaId}";

            var document = await _httpClient.GetDocumentAsync(query);

            if (document == null)
            {
                return null;
            }

            var (type, name) = ParsePageTitle(document);
            var characterNodes = document.SelectNodes("//section[contains(@class, 'ch-st-list')]/div");
            var imageId = GetSideImageId(document);
            var characters = new List<CharacterDetail>();

            foreach (var characterNode in characterNodes)
            {
                var character = ParseCharacter(characterNode); ;
                characters.Add(character);
            }

            var result = new MangaDetail(mangaId, name, characters, imageId);

            return result;
        }

        public async Task<IEnumerable<BasicMangaDetail>> GetMangaDetailsAsync(int page)
        {
            var query = $"/manga?sort_by=ranking-rate&sort_order=desc&page={page}";
            var document = await _httpClient.GetDocumentAsync(query);

            var serieNodes = document
                .SelectNodes("//section[contains(@class, 'title-table')]/article/ul");

            var results = new List<BasicMangaDetail>();

            foreach (var serieNode in serieNodes)
            {
                var linkNode = serieNode.SelectSingleNode("./li[2]/h3/a");
                var link = linkNode.Attributes.FirstOrDefault(pr => pr.Name == "href").Value;
                var name = linkNode.InnerText;
                var id = link.GetIdFromLink();
                results.Add(new(id!.Value, name));
            }

            return results;
        }

        public async Task<AnimeDetail?> GetAnimeDetailAsync(ulong animeId)
        {
            var query = $"/series/{animeId}";
            var document = await _httpClient.GetDocumentAsync(query);

            if (document == null)
            {
                query = $"/titles/{animeId}";
                document = await _httpClient.GetDocumentAsync(query);

                if (document == null)
                {
                    return null;
                }

                if (document.InnerText.Contains("Trwa ładowanie"))
                {
                    return null;
                }
            }

            var (type, name) = ParsePageTitle(document);
            var characterNodes = document.SelectNodes("//section[contains(@class, 'ch-st-list')]/div");
            var imageId = GetSideImageId(document);

            var characters = new List<CharacterDetail>();

            foreach (var characterNode in characterNodes)
            {
                var character = ParseCharacter(characterNode);
                characters.Add(character);
            }

            var result = new AnimeDetail(animeId, name, characters, imageId);

            return result;
        }

        public async Task<IEnumerable<BasicAnimeDetail>> GetAnimeDetailsAsync(int page)
        {
            var query = $"/series?sort_by=ranking-rate&sort_order=desc&page={page}";
            var seriesDocument = await _httpClient.GetDocumentAsync(query);

            var serieNodes = seriesDocument
                .SelectNodes("//section[contains(@class, 'title-table')]/article/ul");

            var results = new List<BasicAnimeDetail>();

            foreach (var serieNode in serieNodes)
            {
                var linkNode = serieNode.SelectSingleNode("./li[2]/h3/a");
                var serieLink = linkNode.Attributes.FirstOrDefault(pr => pr.Name == "href").Value;
                var serieName = linkNode.InnerText;
                var serieId = serieLink.GetIdFromLink();

                results.Add(new(serieId!.Value, serieName));
            }

            return results;
        }

        private CharacterDetail ParseCharacter(HtmlNode htmlNode)
        {
            var characterImageNode = htmlNode.SelectSingleNode("//a[@class = 'img']");
            var characterLinkNode = htmlNode.SelectSingleNode("./span[1]/div/h3/a");

            if (characterLinkNode == null)
            {
                throw new Exception("Invalid node");
            }

            var imageIdStr = characterImageNode.GetDataAttribute("over-image").Value;
            var characterImageId = ulong.Parse(imageIdStr);
            var characterName = characterLinkNode.InnerText;
            var characterLink = characterLinkNode.Attributes.FirstOrDefault(pr => pr.Name == "href").Value;
            var characterId = characterLink.GetIdFromLink();
            return new(characterId!.Value, characterName, characterImageId);
        }

        private (string, string) ParsePageTitle(HtmlNode htmlNode)
        {
            var titleNode = htmlNode.SelectSingleNode("//h1[contains(@class, 'page-title')]");
            var split = titleNode.InnerText.Split(":");
            var type = split[0];
            var name = split[1].Trim();

            return (type, name);
        }

        private ulong? GetSideImageId(HtmlNode htmlNode)
        {
            var imageNode = htmlNode.SelectSingleNode("//img[@class = 'info-aside-img']");
            var imageLink = imageNode.Attributes.FirstOrDefault(pr => pr.Name == "src").Value;

            if (imageLink.Contains("placeholders"))
            {
                return null;
            }

            var imageId = imageLink.GetIdFromLink();
            return imageId;
        }
    }
}
