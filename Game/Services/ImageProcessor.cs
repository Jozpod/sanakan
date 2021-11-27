using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DiscordBot.Services.PocketWaifu;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.Common.Models;
using Sanakan.DAL.Models;
using Sanakan.Extensions;
using Sanakan.Game.Services.Abstractions;
using Sanakan.Services.PocketWaifu;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Utilities;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace Sanakan.Game.Services
{
    internal class ImageProcessor : IImageProcessor
    {
        private readonly IOptionsMonitor<ImagingConfiguration> _options;
        private readonly IResourceManager _resourceManager;
        private readonly FontCollection _fontCollection;
        private readonly FontFamily _digital;
        private readonly FontFamily _latoBold;
        private readonly FontFamily _latoLight;
        private readonly FontFamily _latoRegular;
        private readonly Point _origin = new (0, 0);
        private readonly IFileSystem _fileSystem;
        private readonly HttpClient _httpClient;

        public ImageProcessor(
            IOptionsMonitor<ImagingConfiguration> options,
            IResourceManager resourceManager,
            IFileSystem fileSystem,
            IHttpClientFactory _httpClientFactory)
        {
            _options = options;
            _resourceManager = resourceManager;
            _fileSystem = fileSystem;
            _httpClient = _httpClientFactory.CreateClient(nameof(IImageProcessor));
            _fontCollection = new FontCollection();

            _digital = LoadFontFromStream(Resources.DigitalFont);
            _latoBold = LoadFontFromStream(Resources.LatoBoldFont);
            _latoLight = LoadFontFromStream(Resources.LatoLightfont);
            _latoRegular = LoadFontFromStream(Resources.LatoRegularfont);
        }

        private FontFamily LoadFontFromStream(string resourceName)
        {
            var fontStream = _resourceManager.GetResourceStream(resourceName);
            return _fontCollection.Install(fontStream);
        }

        private async Task<Stream?> GetImageFromUrlAsync(string url, bool fixExt = false)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStreamAsync();
                }

                if (!fixExt)
                {
                    return null;
                }

                var splited = url.Split(".");
                var exts = new[] { "png", "jpeg", "gif", "jpg" };

                foreach (var ext in exts)
                {
                    splited[splited.Length - 1] = ext;
                    response = await _httpClient.GetAsync(string.Join(".", splited));

                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    return await response.Content.ReadAsStreamAsync();
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }

        private Font GetFontSize(FontFamily fontFamily, float size, string text, float maxWidth)
        {
            var font = new Font(fontFamily, size);
            var measured = TextMeasurer.Measure(text, new RendererOptions(font));

            while (measured.Width > maxWidth)
            {
                if (--size < 1) break;
                font = new Font(fontFamily, size);
                measured = TextMeasurer.Measure(text, new RendererOptions(font));
            }

            return font;
        }

        private void CheckProfileImageSize(Image<Rgba32> image, Size size, bool strech)
        {
            if (image.Width > size.Width || image.Height > size.Height)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = size
                }));

                return;
            }

            if (!strech)
                return;

            if (image.Width < size.Width || image.Height < size.Height)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Stretch,
                    Size = size
                }));
            }
        }

        public async Task SaveImageFromUrlAsync(string url, string path)
            => await SaveImageFromUrlAsync(url, path, Size.Empty);

        public async Task SaveImageFromUrlAsync(string imageUrl, string filePath, Size size, bool strech = false)
        {
            using var stream = await GetImageFromUrlAsync(imageUrl, true);
            using var image = Image.Load(stream);

            if (size.Height > 0 || size.Width > 0)
            {
                CheckProfileImageSize(image, size, strech);
            }

            image.SaveToPath(filePath, _fileSystem);
        }

        public async Task<Image<Rgba32>> GetUserProfileAsync(
            UserInfo shindenUser,
            User botUser,
            string avatarUrl,
            long topPos,
            string nickname,
            Discord.Color color)
        {
            if (color == Discord.Color.Default)
            {
                color = Discord.Color.DarkerGrey;
            }

            string rangName = shindenUser?.Rank ?? "";
            string colorRank = color.RawValue.ToString("X6");

            var nickFont = GetFontSize(_latoBold, 28, nickname, 290);
            var rangFont = new Font(_latoRegular, 16);
            var levelFont = new Font(_latoBold, 40);

            var template = Image.Load(Paths.ProfileBodyPicture);
            var profilePic = new Image<Rgba32>(template.Width, template.Height);

            if (!_fileSystem.Exists(botUser.BackgroundProfileUri))
            {
                botUser.BackgroundProfileUri = Paths.DefaultBackgroundPicture;
            }

            using var userBg = Image.Load(botUser.BackgroundProfileUri);
            profilePic.Mutate(x => x.DrawImage(userBg, _origin, 1));
            profilePic.Mutate(x => x.DrawImage(template, _origin, 1));

            template.Dispose();

            using var avatar = Image.Load(await GetImageFromUrlAsync(avatarUrl));
            using var avBack = new Image<Rgba32>(82, 82);
            avBack.Mutate(x => x.BackgroundColor(Rgba32.FromHex(colorRank)));
            avBack.Mutate(x => x.Round(42));

            profilePic.Mutate(x => x.DrawImage(avBack, new Point(20, 115), 1));

            avatar.Mutate(x => x.Resize(new Size(80, 80)));
            avatar.Mutate(x => x.Round(42));

            profilePic.Mutate(x => x.DrawImage(avatar, new Point(21, 116), 1));

            var defFontColor = Colors.MediumGrey;
            var posColor = Colors.RubberDuckyYellow;

            if (topPos == 2)
            {
                posColor = Colors.SilverSand;
            }
            else if (topPos == 3)
            {
                posColor = Colors.BrandyPunch;
            }
            else if (topPos > 3)
            {
                posColor = defFontColor;
            }

            profilePic.Mutate(x => x.DrawText(nickname, nickFont, Colors.GreyChateau, new Point(132, 150 + (int)((30 - nickFont.Size) / 2))));
            profilePic.Mutate(x => x.DrawText(rangName, rangFont, defFontColor, new Point(132, 180)));

            var mLevel = TextMeasurer.Measure($"{botUser.Level}", new RendererOptions(levelFont));
            profilePic.Mutate(x => x.DrawText($"{botUser.Level}", levelFont, defFontColor, new Point((int)(125 - mLevel.Width) / 2, 206)));

            var mTopPos = TextMeasurer.Measure($"{topPos}", new RendererOptions(levelFont));
            profilePic.Mutate(x => x.DrawText($"{topPos}", levelFont, posColor, new Point((int)(125 - mTopPos.Width) / 2, 284)));

            var mScOwn = TextMeasurer.Measure($"{botUser.ScCount}", new RendererOptions(rangFont));
            profilePic.Mutate(x => x.DrawText($"{botUser.ScCount}", rangFont, defFontColor, new Point((int)(125 - mScOwn.Width) / 2, 365)));

            var mTcOwn = TextMeasurer.Measure($"{botUser.TcCount}", new RendererOptions(rangFont));
            profilePic.Mutate(x => x.DrawText($"{botUser.TcCount}", rangFont, defFontColor, new Point((int)(125 - mTcOwn.Width) / 2, 405)));

            var mMsg = TextMeasurer.Measure($"{botUser.MessagesCount}", new RendererOptions(rangFont));
            profilePic.Mutate(x => x.DrawText($"{botUser.MessagesCount}", rangFont, defFontColor, new Point((int)(125 - mMsg.Width) / 2, 445)));

            if (botUser.GameDeck.FavouriteWaifuId != 0 && botUser.ShowWaifuInProfile)
            {
                var tChar = botUser.GameDeck.Cards
                    .OrderBy(x => x.Rarity)
                    .FirstOrDefault(x => x.CharacterId == botUser.GameDeck.FavouriteWaifuId);

                if (tChar != null)
                {
                    using var cardImage = await GetWaifuInProfileCardAsync(tChar);
                    cardImage.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(105, 0)
                    }));
                    profilePic.Mutate(x => x.DrawImage(cardImage, new Point(10, 350), 1));
                }
            }

            var prevLvlExp = ExperienceUtils.CalculateExpForLevel(botUser.Level);
            var nextLvlExp = ExperienceUtils.CalculateExpForLevel(botUser.Level + 1);
            var expOnLvl = botUser.ExperienceCount - prevLvlExp;
            var lvlExp = nextLvlExp - prevLvlExp;

            if (expOnLvl < 0)
            {
                expOnLvl = 0;
            }
            if (lvlExp < 0)
            {
                lvlExp = expOnLvl + 1;
            }

            int progressBarLength = (int)(305f * ((double)expOnLvl / (double)lvlExp));
            if (progressBarLength > 0)
            {
                using var progressBar = new Image<Rgba32>(progressBarLength, 19);
                progressBar.Mutate(x => x.BackgroundColor(Colors.BattleshipGrey));
                profilePic.Mutate(x => x.DrawImage(progressBar, new Point(135, 201), 1));
            }

            var expText = $"EXP: {expOnLvl} / {lvlExp}";
            var mExp = TextMeasurer.Measure(expText, new RendererOptions(rangFont));
            profilePic.Mutate(x => x.DrawText(expText, rangFont, Colors.White, new Point(135 + ((int)(305 - mExp.Width) / 2), 204)));

            using var inside = GetProfileInside(shindenUser, botUser);
            profilePic.Mutate(x => x.DrawImage(inside, new Point(125, 228), 1));

            return profilePic;
        }

        private Image<Rgba32> GetProfileInside(UserInfo shindenUser, User botUser)
        {
            var image = new Image<Rgba32>(325, 272);

            if (!_fileSystem.Exists(botUser.StatsReplacementProfileUri))
            {
                if ((botUser.ProfileType == ProfileType.Image 
                    || botUser.ProfileType == ProfileType.StatisticsWithImage))
                {
                    botUser.ProfileType = ProfileType.Statistics;
                }
            }

            switch (botUser.ProfileType)
            {
                case ProfileType.Statistics:
                case ProfileType.StatisticsWithImage:
                    if (shindenUser != null)
                    {
                        
                        if (shindenUser.WatchedStatus != null)
                        {
                            using var stats = GetRWStats(
                                shindenUser,
                                Paths.StatsAnimePicture,
                                false);
                            image.Mutate(x => x.DrawImage(stats, new Point(0, 2), 1));
                        }
                        if (shindenUser.ReadedStatus != null)
                        {
                            using var stats = GetRWStats(
                                shindenUser,
                                Paths.StatsMangaPicture,
                                true); 
                            image.Mutate(x => x.DrawImage(stats, new Point(0, 142), 1));
                        }

                        if (botUser.ProfileType == ProfileType.StatisticsWithImage)
                        {
                            goto case ProfileType.Image;
                        }
                    }
                    break;

                case ProfileType.Cards:
                    {
                        using var cardsBg = GetCardsProfileImage(botUser).Result;
                        image.Mutate(x => x.DrawImage(cardsBg, _origin, 1));
                    }
                    break;

                case ProfileType.Image:
                    {
                        using var userBg = Image.Load(botUser.StatsReplacementProfileUri);
                        image.Mutate(x => x.DrawImage(userBg, _origin, 1));
                    }
                    break;
            }

            return image;
        }

        private async Task<Image<Rgba32>> GetCardsProfileImage(User botUser)
        {
            var profilePic = new Image<Rgba32>(325, 272);
            var gameDeck = botUser.GameDeck;
            var cards = gameDeck.Cards;

            if (gameDeck.FavouriteWaifuId.HasValue)
            {
                var favouriteCharacter = gameDeck.Cards
                    .OrderBy(x => x.Rarity)
                    .FirstOrDefault(x => x.CharacterId == botUser.GameDeck.FavouriteWaifuId);

                if (favouriteCharacter != null)
                {
                    using var cardImage = await GetWaifuInProfileCardAsync(favouriteCharacter);
                    
                    cardImage.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(0, 260)
                    }));
                    profilePic.Mutate(x => x.DrawImage(cardImage, new Point(10, 6), 1));
                }
            }

            var sss = 0;
            var ss = 0;
            var s = 0;
            var a = 0;
            var b = 0;
            var c = 0;
            var d = 0;
            var e = 0;

            foreach (var card in cards)
            {
                switch(card.Rarity)
                {
                    case Rarity.SSS:
                        sss++;
                        break;
                    case Rarity.SS:
                        ss++;
                        break;
                    case Rarity.S:
                        s++;
                        break;
                    case Rarity.A:
                        a++;
                        break;
                    case Rarity.B:
                        b++;
                        break;
                    case Rarity.C:
                        c++;
                        break;
                    case Rarity.D:
                        d++;
                        break;
                    case Rarity.E:
                        e++;
                        break;
                }
            }

            int jumpY = 18;
            int row2X = 45;
            int startY = 12;
            int startX = 205;
            var font1 = GetFontSize(_latoBold, 18, $"SUM", 100);
            var font2 = GetFontSize(_latoLight, 18, "10000", 130);

            var defaultXPosition = startX + row2X;
            var textColor = Colors.Dawn;

            var rarityMap = new[]
            {
                ("SSS", sss.ToString(), jumpY, defaultXPosition),
                ("SS", ss.ToString(), jumpY, defaultXPosition),
                ("S", s.ToString(), jumpY, defaultXPosition),
                ("A", a.ToString(), jumpY, defaultXPosition),
                ("B", b.ToString(), jumpY, defaultXPosition),
                ("C", c.ToString(), jumpY, defaultXPosition),
                ("D", d.ToString(), jumpY, defaultXPosition),
                ("E", e.ToString(), jumpY, defaultXPosition),
                ("SUM", cards.Count.ToString(), 4 * jumpY, defaultXPosition),
                ("CT", gameDeck.CTCount.ToString(), jumpY, defaultXPosition),
                ("K", gameDeck.Karma.ToString("F"), jumpY, startX + 15),
            };

            foreach (var (text, count, yOffset, xPosition) in rarityMap)
            {
                profilePic.Mutate(x => x.DrawText(text, font1, textColor, new Point(startX, startY)));
                profilePic.Mutate(x => x.DrawText(count, font2, textColor, new Point(xPosition, startY)));
                startY += yOffset;
            }    

            return profilePic;
        }

        private async Task<Image<Rgba32>> GetSiteStatisticUserBadge(string avatarUrl, string name, string color)
        {
            var font = GetFontSize(_latoBold, 32, name, 360);

            var badge = new Image<Rgba32>(450, 65);
            badge.Mutate(x => x.DrawText(name, font, Colors.Dawn, new Point(72, 6 + (int)((58 - font.Size) / 2))));

            using var border = new Image<Rgba32>(3, 57);
            border.Mutate(x => x.BackgroundColor(Rgba32.FromHex(color)));
            badge.Mutate(x => x.DrawImage(border, new Point(63, 5), 1));

            using var stream = await GetImageFromUrlAsync(avatarUrl);

            if (stream == null)
            {
                return badge;
            }

            using var avatar = Image.Load(stream);
                
            avatar.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Crop,
                Size = new Size(57, 57)
            }));
            badge.Mutate(x => x.DrawImage(avatar, new Point(6, 5), 1));

            return badge;
        }

        private Image<Rgba32> GetRWStats(UserInfo userInfo, string path, bool isManga)
        {
            int startPointX = 7;
            int startPointY = 3;
            var baseImg = Image.Load(path);

            var status = userInfo.ReadedStatus;
            var meanScore = isManga ? userInfo.MeanMangaScore : userInfo.MeanAnimeScore;

            if (status.Total.HasValue && status.Total > 0)
            {
                using var bar = GetStatusBar(
                    status.Total.Value,
                    status.InProgress.Value,
                    status.Completed.Value,
                    status.Skip.Value,
                    status.Hold.Value,
                    status.Dropped.Value,
                    status.Plan.Value);
                
                bar.Mutate(x => x.Round(5));
                baseImg.Mutate(x => x.DrawImage(bar, new Point(startPointX, startPointY), 1));
            }

            startPointY += 24;
            startPointX += 110;
            int ySecondStart = startPointY;
            int fontSizeAndInterline = 10 + 6;
            var font = new Font(_latoBold, 13);
            int xSecondRow = startPointX + 200;
            var fontColor = Colors.SmokeyGrey;

            ulong?[] rowArr = { 
                status?.InProgress,
                status?.Completed,
                status?.Skip,
                status?.Hold,
                status?.Dropped,
                status?.Plan
            };

            for (var i = 0; i < rowArr.Length; i++)
            {
                baseImg.Mutate(x => x.DrawText($"{rowArr[i]}", font, fontColor, new Point(startPointX, startPointY)));
                startPointY += fontSizeAndInterline;
            }

            var gOptions = new TextGraphicsOptions { HorizontalAlignment = HorizontalAlignment.Right };

            baseImg.Mutate(x => x.DrawText(gOptions, $"{meanScore.ScoreCount.Value.ToString("0.0")}", font, fontColor, new Point(xSecondRow, ySecondStart)));
            ySecondStart += fontSizeAndInterline;

            baseImg.Mutate(x => x.DrawText(gOptions, $"{status?.Total}", font, fontColor, new Point(xSecondRow, ySecondStart)));
            ySecondStart += fontSizeAndInterline;

            baseImg.Mutate(x => x.DrawText(gOptions, $"{meanScore.ScoreCount.Value}", font, fontColor, new Point(xSecondRow, ySecondStart)));
            ySecondStart += fontSizeAndInterline;

            var listTime = new List<string>();

            if (userInfo.ReadTime != null)
            {
                var time = userInfo.ReadTime;

                if (time.Years != 0)
                {
                    listTime.Add($"{time.Years} lat");
                }
                if (time.Months != 0)
                {
                    listTime.Add($"{time.Months} mies.");
                }
                if (time.Days != 0)
                {
                    listTime.Add($"{time.Days} dni");
                }
                if (time.Hours != 0)
                {
                    listTime.Add($"{time.Hours} h");
                }
                if (time.Minutes != 0)
                {
                    listTime.Add($"{time.Minutes} m");
                }
            }

            ySecondStart += fontSizeAndInterline;

            if (listTime.Count > 2)
            {
                string fs = listTime.First();
                listTime.Remove(fs);
                string sc = listTime.First();
                listTime.Remove(sc);

                baseImg.Mutate(x => x.DrawText(gOptions, $"{fs} {sc}", font, fontColor, new Point(xSecondRow, ySecondStart)));

                ySecondStart += fontSizeAndInterline;
            }

            var text = string.Join<string>(" ", listTime);
            baseImg.Mutate(x => x.DrawText(gOptions, text, font, fontColor, new Point(xSecondRow, ySecondStart)));

            return baseImg;
        }

        private Image<Rgba32> GetStatusBar(ulong all, ulong green, ulong blue, ulong purple, ulong yellow, ulong red, ulong grey)
        {
            int offset = 0;
            int length = 311;
            int fixedLength = 0;

            var arrLength = new int[6];
            var arrProcent = new double[6];
            double[] arrValues = { green, blue, purple, yellow, red, grey };

            for (int i = 0; i < arrValues.Length; i++)
            {
                if (arrValues[i] != 0)
                {
                    arrProcent[i] = arrValues[i] / all;
                    arrLength[i] = (int)((length * arrProcent[i]) + 0.5);
                    fixedLength += arrLength[i];
                }
            }

            if (fixedLength > length)
            {
                var res = arrLength.OrderByDescending(x => x).FirstOrDefault();
                arrLength[arrLength.ToList().IndexOf(res)] -= fixedLength - length;
            }

            var bar = new Image<Rgba32>(length, 17);
            for (var i = 0; i < arrValues.Length; i++)
            {
                if (arrValues[i] != 0)
                {
                    using var thisBar = new Image<Rgba32>(arrLength[i] < 1 ? 1 : arrLength[i], 17);
                    thisBar.Mutate(x => x.BackgroundColor(Colors.StatusBarColors[i]));
                    bar.Mutate(x => x.DrawImage(thisBar, new Point(offset, 0), 1));
                    offset += arrLength[i];
                }
            }

            return bar;
        }

        private Image<Rgba32>? GetLastRWListCover(Stream imageStream)
        {
            if (imageStream == null)
            {
                return null;
            }

            var cover = Image.Load(imageStream);
            cover.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(20, 50)
            }));

            return cover;
        }

        private async Task<Image<Rgba32>> GetLastRWList(List<LastWatchedRead>? lastRead, List<LastWatchedRead>? lastWatch)
        {
            var titleFont = new Font(_latoBold, 10);
            var nameFont = new Font(_latoBold, 16);
            var fColor = Colors.LemonGrass;
            int startY = 25;

            var image = new Image<Rgba32>(175, 248);
            image.Mutate(x => x.DrawText("Ostatnio obejrzane:", nameFont, fColor, new Point(0, 5)));

            if (lastWatch != null)
            {
                int max = -1;
                foreach (var last in lastWatch)
                {
                    if (++max >= 3)
                    {
                        break;
                    }

                    using var stream = await GetImageFromUrlAsync(last.AnimeCoverUrl, true);
                    using var cover = GetLastRWListCover(stream);
                    if (cover != null)
                    {
                        image.Mutate(x => x.DrawImage(cover, new Point(0, startY + (35 * max)), 1));
                    }

                    image.Mutate(x => x.DrawText($"{last.Title.ElipseTrimToLength(29)}", titleFont, fColor, new Point(25, startY + (35 * max))));
                    image.Mutate(x => x.DrawText($"{last.EpisodeNo} / {last.EpisodesCount}", titleFont, fColor, new Point(25, startY + 11 + (35 * max))));
                }
            }

            startY += 128;
            image.Mutate(x => x.DrawText("Ostatnio przeczytane:", nameFont, fColor, new Point(0, 133)));

            if (lastRead != null)
            {
                int max = -1;
                foreach (var last in lastRead)
                {
                    if (++max >= 3)
                    {
                        break;
                    }
                    
                    using var stream = await GetImageFromUrlAsync(last.MangaCoverUrl, true);
                    using var cover = GetLastRWListCover(stream);

                    if (cover != null)
                    {
                        image.Mutate(x => x.DrawImage(cover, new Point(0, startY + (35 * max)), 1));
                    }

                    image.Mutate(x => x.DrawText($"{last.Title.ElipseTrimToLength(29)}", titleFont, fColor, new Point(25, startY + (35 * max))));
                    image.Mutate(x => x.DrawText($"{last.ChapterNo} / {last.ChaptersCount}", titleFont, fColor, new Point(25, startY + 11 + (35 * max))));
                }
            }

            return image;
        }

        public async Task<Image<Rgba32>> GetSiteStatisticAsync(
            UserInfo shindenInfo,
            Discord.Color color,
            List<LastWatchedRead>? lastRead = null,
            List<LastWatchedRead>? lastWatch = null)
        {
            if (color == Discord.Color.Default)
            {
                color = Discord.Color.DarkerGrey;
            }

            var baseImg = new Image<Rgba32>(500, 320);
            baseImg.Mutate(x => x.BackgroundColor(Colors.Onyx));

            using var template = Image.Load(Paths.SiteStatisticsPicture);
            baseImg.Mutate(x => x.DrawImage(template, _origin, 1));

            var avatarUrl = UrlHelpers.GetUserAvatarURL(shindenInfo.AvatarId, shindenInfo.Id.Value);
            var hexColor = color.RawValue.ToString("X6");

            using var avatar = await GetSiteStatisticUserBadge(
                avatarUrl,
                shindenInfo.Name,
                hexColor);
            baseImg.Mutate(x => x.DrawImage(avatar, _origin, 1));

            using var image = new Image<Rgba32>(325, 248);
            if (shindenInfo?.MeanAnimeScore != null)
            {
                using var stats = GetRWStats(
                    shindenInfo,
                    Paths.StatsAnimePicture,
                    false);
                    
                image.Mutate(x => x.DrawImage(stats, _origin, 1));
            }
            if (shindenInfo?.MeanMangaScore != null)
            {
                using var stats = GetRWStats(
                    shindenInfo,
                    Paths.StatsMangaPicture,
                    true);
                    
                image.Mutate(x => x.DrawImage(stats, new Point(0, 128), 1));
                    
            }

            baseImg.Mutate(x => x.DrawImage(image, new Point(5, 71), 1));
            using var rwListImage = await GetLastRWList(lastRead, lastWatch);
            baseImg.Mutate(x => x.DrawImage(rwListImage, new Point(330, 69), 1));

            return baseImg;
        }

        public async Task<Image<Rgba32>> GetLevelUpBadgeAsync(
            string name,
            ulong userLevel,
            string avatarUrl,
            Discord.Color color)
        {
            if (color == Discord.Color.Default)
            {
                color = Discord.Color.DarkerGrey;
            }

            var msgText1 = "POZIOM";
            var msgText2 = "Awansuje na:";

            var textFont = new Font(_latoRegular, 16);
            var nickNameFont = new Font(_latoBold, 22);
            var lvlFont = new Font(_latoBold, 36);

            var msgText1Length = TextMeasurer.Measure(msgText1, new RendererOptions(textFont));
            var msgText2Length = TextMeasurer.Measure(msgText2, new RendererOptions(textFont));
            var nameLength = TextMeasurer.Measure(name, new RendererOptions(nickNameFont));
            var lvlLength = TextMeasurer.Measure($"{userLevel}", new RendererOptions(lvlFont));

            var textLength = lvlLength.Width + msgText1Length.Width > nameLength.Width ? lvlLength.Width + msgText1Length.Width : nameLength.Width;
            var estimatedLength = 106 + (int)(textLength > msgText2Length.Width ? textLength : msgText2Length.Width);

            var nickNameColor = Rgba32.FromHex(color.RawValue.ToString("X6"));
            var baseImg = new Image<Rgba32>((int)estimatedLength, 100);

            baseImg.Mutate(x => x.BackgroundColor(Colors.Onyx));
            baseImg.Mutate(x => x.DrawText(msgText1, textFont, Rgba32.Gray, new Point(98 + (int)lvlLength.Width, 75)));
            baseImg.Mutate(x => x.DrawText(name, nickNameFont, nickNameColor, new Point(98, 10)));
            baseImg.Mutate(x => x.DrawText(msgText2, textFont, Rgba32.Gray, new Point(98, 33)));
            baseImg.Mutate(x => x.DrawText($"{userLevel}", lvlFont, Rgba32.Gray, new Point(96, 61)));

            using var colorRec = new Image<Rgba32>(82, 82);
            
            colorRec.Mutate(x => x.BackgroundColor(nickNameColor));
            baseImg.Mutate(x => x.DrawImage(colorRec, new Point(9, 9), 1));

            using var stream = await GetImageFromUrlAsync(avatarUrl);

            if (stream == null)
            {
                return baseImg;
            }

            using var avatar = Image.Load(stream);
                    
            avatar.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Crop,
                Size = new Size(80, 80)
            }));
            baseImg.Mutate(x => x.DrawImage(avatar, new Point(10, 10), 1));
    
            return baseImg;
        }

        public Image<Rgba32> GetFColorsView(IEnumerable<(string, uint)> colours)
        {
            var message = new Font(_latoRegular, 16);
            var firstColumnMaxLength = TextMeasurer.Measure("A", new RendererOptions(message));
            var secondColumnMaxLength = TextMeasurer.Measure("A", new RendererOptions(message));

            var inFirstColumn = (colours.Count() + 1) / 2;

            var index = 1;
            foreach (var colour in colours)
            {
                var nLen = TextMeasurer.Measure(colour.Item1, new RendererOptions(message));

                if (index < inFirstColumn + 1)
                {
                    if (firstColumnMaxLength.Width < nLen.Width)
                        firstColumnMaxLength = nLen;
                }
                else
                {
                    if (secondColumnMaxLength.Width < nLen.Width)
                        secondColumnMaxLength = nLen;
                }
                index++;
            }

            int posY = 5;
            int posX = 0;
            int realWidth = (int)(firstColumnMaxLength.Width + secondColumnMaxLength.Width + 20);
            int realHeight = (int)(firstColumnMaxLength.Height + 2) * (inFirstColumn + 1);

            var imgBase = new Image<Rgba32>(realWidth, realHeight);
            imgBase.Mutate(x => x.BackgroundColor(Colors.Onyx));
            imgBase.Mutate(x => x.DrawText("Lista:", message, Colors.Black, _origin));

            index = 1;
            foreach(var colour in colours)
            {
                if (inFirstColumn + 1 == index)
                {
                    posY = 5;
                    posX = (int)firstColumnMaxLength.Width + 10;
                }

                posY += (int)firstColumnMaxLength.Height + 2;
                var structColour = Rgba32.FromHex(colour.Item2.ToString("X6"));
                imgBase.Mutate(x => x.DrawText(colour.Item1, message, structColour, new Point(posX, posY)));
                index++;
            }

            return imgBase;
        }

        private async Task<Image<Rgba32>> GetCharacterPictureAsync(string characterUrl, bool ultimate)
        {
            var characterImg = Image.Load(Paths.PWEmptyPicture);
            if (ultimate)
            {
                characterImg = new Image<Rgba32>(_options.CurrentValue.CharacterImageWidth, _options.CurrentValue.CharacterImageHeight);
            }

            using var stream = await GetImageFromUrlAsync(characterUrl ?? "http://cdn.shinden.eu/cdn1/other/placeholders/title/225x350.jpg", true);
            
            if (stream == null)
            {
                return characterImg;
            }

            using var image = Image.Load(stream);
                
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(characterImg.Width, 0)
            }));

            int startY = 0;
            if (characterImg.Height > image.Height)
            {
                startY = (characterImg.Height / 2) - (image.Height / 2);
            }

            characterImg.Mutate(x => x.DrawImage(image, new Point(0, startY), 1));

            return characterImg;
        }

        private Image<Rgba32> GenerateBorder(Card card)
        {
            var borderStr = string.Format(Paths.PWPicture, card.Rarity);
            var dereStr = string.Format(Paths.PWPicture, card.Dere);

            if (card.FromFigure)
            {
                borderStr = string.Format(Paths.PWCGPicture, card.Quality);
                dereStr = dereStr = string.Format(Paths.PWCGDerePicture, card.Quality, card.Dere);
            }

            var img = Image.Load(borderStr);

            using var dere = Image.Load(dereStr);
            img.Mutate(x => x.DrawImage(dere, _origin, 1));

            return img;
        }

        private async Task<Image<Rgba32>> LoadCustomBorderAsync(Card card)
        {
            if (card.CustomBorderUrl == null)
            {
                return GenerateBorder(card);
            }

            using var stream = await GetImageFromUrlAsync(card.CustomBorderUrl.ToString());

            if (stream == null)
            {
                return GenerateBorder(card);
            }

            return Image.Load(stream);
        }

        private void ApplyAlphaStats(Image<Rgba32> image, Card card)
        {
            var adFont = new Font(_latoBold, 36);
            var hpFont = new Font(_latoBold, 32);

            var healthPoints = card.GetHealthWithPenalty().ToString();
            var defencePoints = card.GetDefenceWithBonus().ToString();
            var attackPoints = card.GetAttackWithBonus().ToString();

            var options = _options.CurrentValue;
            using var hpImg = new Image<Rgba32>(options.StatsImageWidth, options.StatsImageHeight);
            
            hpImg.Mutate(x => x.DrawText(healthPoints, hpFont, Colors.Pine, new Point(1)));
            hpImg.Mutate(x => x.Rotate(-18));
            image.Mutate(x => x.DrawImage(hpImg, new Point(320, 528), 1));
            

            image.Mutate(x => x.DrawText(attackPoints, adFont, Colors.PlumPurple, new Point(43, 603)));
            image.Mutate(x => x.DrawText(defencePoints, adFont, Colors.DeepSeaBlue, new Point(337, 603)));
        }

        private void ApplyBetaStats(Image<Rgba32> image, Card card)
        {
            var adFont = new Font(_latoBold, 36);
            var hpFont = new Font(_latoBold, 29);

            var healthPoints = card.GetHealthWithPenalty().ToString();
            var defencePoints = card.GetDefenceWithBonus().ToString();
            var attackPoints = card.GetAttackWithBonus().ToString();

            var options = _options.CurrentValue;
            using var baseImage = new Image<Rgba32>(options.StatsImageWidth, options.StatsImageHeight);

            using var hpImg = baseImage.Clone();

            hpImg.Mutate(x => x.DrawText(healthPoints, hpFont, Colors.DartmouthGreen, new Point(1)));
            hpImg.Mutate(x => x.Rotate(18));

            image.Mutate(x => x.DrawImage(hpImg, new Point(342, 338), 1));


            using var defImg = baseImage.Clone();

            defImg.Mutate(x => x.DrawText(defencePoints, adFont, Colors.ToryBlue, new Point(1)));
            defImg.Mutate(x => x.Rotate(19));

            image.Mutate(x => x.DrawImage(defImg, new Point(28, 175), 1));


            using var atkImg = baseImage.Clone();

            atkImg.Mutate(x => x.DrawText(attackPoints, adFont, Colors.DarkBurgundy, new Point(1)));
            atkImg.Mutate(x => x.Rotate(-16));

            image.Mutate(x => x.DrawImage(atkImg, new Point(50, 502), 1));
            
        }

        private void ApplyGammaStats(Image<Rgba32> image, Card card)
        {
            var aphFont = new Font(_latoBold, 37);

            var healthPoints = card.GetHealthWithPenalty().ToString();
            var defencePoints = card.GetDefenceWithBonus().ToString();
            var attackPoints = card.GetAttackWithBonus().ToString();

            // TODO: center numbers
            image.Mutate(x => x.DrawText(attackPoints, aphFont, Colors.Flirt, new Point(196, 495)));
            image.Mutate(x => x.DrawText(defencePoints, aphFont, Colors.DarkGreenBlue, new Point(282, 545)));
            image.Mutate(x => x.DrawText(healthPoints, aphFont, Colors.MediumSpringGreen, new Point(90, 545)));
        }

        private void ApplyDeltaStats(Image<Rgba32> image, Card card)
        {
            var hpFont = new Font(_latoBold, 34);
            var adFont = new Font(_latoBold, 26);

            var healthPoints = card.GetHealthWithPenalty().ToString();
            var def = card.GetDefenceWithBonus().ToString();
            var atk = card.GetAttackWithBonus().ToString();

            var options = _options.CurrentValue;
            using var hpImg = new Image<Rgba32>(options.StatsImageWidth, options.StatsImageHeight);

            hpImg.Mutate(x => x.DrawText(healthPoints, hpFont, Colors.Pine, new Point(1)));
            hpImg.Mutate(x => x.Rotate(-27));

            image.Mutate(x => x.DrawImage(hpImg, new Point(333, 490), 1));

            // TODO: center numbers
            image.Mutate(x => x.DrawText(atk, adFont, Colors.MetallicCopper, new Point(62, 600)));
            image.Mutate(x => x.DrawText(def, adFont, Colors.DeepSeaBlue, new Point(352, 600)));
        }

        private void ApplyEpsilonStats(Image<Rgba32> image, Card card)
        {
            var aphFont = new Font(_latoBold, 28);

            var healthPoints = card.GetHealthWithPenalty().ToString();
            int def = card.GetDefenceWithBonus();
            int atk = card.GetAttackWithBonus();

            var textGraphicsOptions = new TextGraphicsOptions() { ApplyKerning = true, DpiX = 80 }; // TODO: rotate hp
            image.Mutate(x => x.DrawText(textGraphicsOptions, healthPoints, aphFont, Colors.BrightLightGreen, new Point(59, 365)));
            image.Mutate(x => x.DrawText(textGraphicsOptions, $"{atk}", aphFont, Colors.DeepOrange, new Point(64, 432)));
            image.Mutate(x => x.DrawText(textGraphicsOptions, $"{def}", aphFont, Colors.Azure, new Point(55, 485)));
        }

        private void ApplyZetaStats(Image<Rgba32> image, Card card)
        {
            var aphFont = new Font(_digital, 28);

            var healthPoints = card.GetHealthWithPenalty().ToString("D5");
            int def = card.GetDefenceWithBonus();
            int atk = card.GetAttackWithBonus();

            var textGraphicsOptions = new TextGraphicsOptions() { ApplyKerning = true, DpiX = 80 };
            image.Mutate(x => x.DrawText(textGraphicsOptions, atk.ToString("D4"), aphFont, Colors.DeepOrange, new Point(342, 538)));
            image.Mutate(x => x.DrawText(textGraphicsOptions, def.ToString("D4"), aphFont, Colors.Azure, new Point(342, 565)));
            image.Mutate(x => x.DrawText(textGraphicsOptions, healthPoints, aphFont, Colors.BrightLightGreen, new Point(328, 593)));
        }

        private void ApplyLambdaStats(Image<Rgba32> image, Card card)
        {
            var aphFont = new Font(_latoBold, 28);

            int hp = card.GetHealthWithPenalty();
            int def = card.GetDefenceWithBonus();
            int atk = card.GetAttackWithBonus();

            var options = _options.CurrentValue;
            using var hpImg = new Image<Rgba32>(options.StatsImageWidth, options.StatsImageHeight);
            hpImg.Mutate(x => x.DrawText($"{hp}", aphFont, Colors.LightGreenishBlue, new Point(1)));
            hpImg.Mutate(x => x.Rotate(-19));
            image.Mutate(x => x.DrawImage(hpImg, new Point(57, 555), 1));


            using var atkImg = new Image<Rgba32>(options.StatsImageWidth, options.StatsImageHeight);
            atkImg.Mutate(x => x.DrawText($"{atk}", aphFont, Colors.BlossomPink, new Point(1)));
            atkImg.Mutate(x => x.Rotate(34));
            image.Mutate(x => x.DrawImage(atkImg, new Point(80, 485), 1));
            

            image.Mutate(x => x.DrawText($"{def}", aphFont, Colors.BlueDiamond, new Point(326, 576)));
        }

        private void ApplyUltimateStats(Image<Rgba32> image, Card card)
        {
            var path = string.Format(Paths.PWCGStatsPicture, card.Quality);
            if (_fileSystem.Exists(path))
            {
                using var stats = Image.Load(path);
                image.Mutate(x => x.DrawImage(stats, _origin, 1));
            }

            switch (card.Quality)
            {
                case Quality.Alpha:ApplyAlphaStats(image, card);
                    break;
                case Quality.Beta: ApplyBetaStats(image, card);
                    break;
                case Quality.Gamma: ApplyGammaStats(image, card);
                    break;
                case Quality.Delta: ApplyDeltaStats(image, card);
                    break;
                case Quality.Epsilon: ApplyEpsilonStats(image, card);
                    break;
                case Quality.Zeta: ApplyZetaStats(image, card);
                    break;
                case Quality.Lambda: ApplyLambdaStats(image, card);
                    break;

                default:
                    break;
            }
        }

        private bool AllowStatsOnNoStatsImage(Card card)
        {
            switch (card.Quality)
            {
                case Quality.Zeta:
                    if (card.CustomBorderUrl != null)
                        return false;
                    return true;

                default:
                    return false;
            }
        }

        private void ApplyStats(Image<Rgba32> image, Card card, bool applyNegativeStats = false)
        {
            int health = card.GetHealthWithPenalty();
            int defence = card.GetDefenceWithBonus();
            int attack = card.GetAttackWithBonus();
            var position = _origin;
            var opacity = 1;

            using var shield = Image.Load(Paths.ShieldPicture);
            image.Mutate(x => x.DrawImage(shield, position, opacity));

            using var heart = Image.Load(Paths.HeartPicture);
            image.Mutate(x => x.DrawImage(heart, position, opacity));

            using var fire = Image.Load(Paths.FirePicture);
            image.Mutate(x => x.DrawImage(fire, position, opacity));

            var starType = card.GetCardStarType();
            var starCnt = card.GetCardStarCount();

            var starX = 239 - (18 * starCnt);
            for (int i = 0; i < starCnt; i++)
            {
                using var starStyle = Image.Load(string.Format(Paths.PWStarPicture, starType, card.StarStyle));
                image.Mutate(x => x.DrawImage(starStyle, new Point(starX, 30), 1));

                starX += 36;
            }

            int startXDef = 390;
            if (defence < 10) startXDef += 15;
            if (defence > 99) startXDef -= 15;

            int startXAtk = 390;
            if (attack < 10) startXAtk += 15;
            if (attack > 99) startXAtk -= 15;

            int startXHp = 380;
            if (health < 10) startXHp += 15;
            if (health > 99) startXHp -= 15;

            var numFont = new Font(_latoBold, 54);
            image.Mutate(x => x.DrawText($"{health}", numFont, Colors.Black, new Point(startXHp, 190)));
            image.Mutate(x => x.DrawText($"{attack}", numFont, Colors.Black, new Point(startXAtk, 320)));
            image.Mutate(x => x.DrawText($"{defence}", numFont, Colors.Black, new Point(startXDef, 440)));

            if (applyNegativeStats)
            {
                using var neg = Image.Load(Paths.NegativeStatsPicture);
                image.Mutate(x => x.DrawImage(neg, _origin, 1));
            }
        }

        private void ApplyBorderBack(Image<Rgba32> image, Card card)
        {
            var isFromFigureOriginalBorder = card.CustomBorderUrl == null && card.FromFigure;
            var backBorderStr = string.Format(Paths.BackBorderPicture, card.Quality);

            if (isFromFigureOriginalBorder && _fileSystem.Exists(backBorderStr))
            {
                using var back = Image.Load(backBorderStr);
                image.Mutate(x => x.DrawImage(back, _origin, 1));
            }
        }

        private async Task<Image<Rgba32>> GetWaifuCardNoStatsAsync(Card card)
        {
            var image = new Image<Rgba32>(_options.CurrentValue.CharacterImageWidth, _options.CurrentValue.CharacterImageHeight);

            ApplyBorderBack(image, card);

            using var chara = await GetCharacterPictureAsync(card.GetImage(), card.FromFigure);
            var mov = card.FromFigure ? 0 : 13;
            image.Mutate(x => x.DrawImage(chara, new Point(mov, mov), 1));

            using var bord = GenerateBorder(card);
            image.Mutate(x => x.DrawImage(bord, _origin, 1));
            

            if (AllowStatsOnNoStatsImage(card))
            {
                ApplyUltimateStats(image, card);
            }

            return image;
        }

        public async Task<Image<Rgba32>> GetWaifuInProfileCardAsync(Card card)
        {
            var image = new Image<Rgba32>(_options.CurrentValue.CharacterImageWidth, _options.CurrentValue.CharacterImageHeight);

            ApplyBorderBack(image, card);

            using var chara = await GetCharacterPictureAsync(card.GetImage(), card.FromFigure);
            var mov = card.FromFigure ? 0 : 13;
            image.Mutate(x => x.DrawImage(chara, new Point(mov, mov), 1));

            using var bord = await LoadCustomBorderAsync(card);
            image.Mutate(x => x.DrawImage(bord, _origin, 1));
            

            if (AllowStatsOnNoStatsImage(card))
            {
                ApplyUltimateStats(image, card);
            }

            return image;
        }

        public Image<Rgba32> GetDuelCardImage(
            DuelInfo duelInfo,
            DuelImage? duelImage,
            Image<Rgba32> winImage,
            Image<Rgba32> lossImage)
        {
            int Xiw = 76;
            int Yt = 780;
            int Yi = 131;
            int Xil = 876;

            if (duelInfo.Side == WinnerSide.Right)
            {
                Xiw = 876;
                Xil = 76;
            }

            Image<Rgba32> image;

            if(duelImage == null)
            {
                image = Image.Load(DuelImage.DefaultUri((int)duelInfo.Side));
            }
            else
            {
                var imagePath = string.Format(Paths.DuelPicture, duelImage.Name, duelInfo.Side);
                if (_fileSystem.Exists(imagePath))
                {
                    image = Image.Load(imagePath);
                }
                else
                {
                    imagePath = DuelImage.DefaultUri((int)duelInfo.Side);
                    image = Image.Load(imagePath);
                }
            }

            var nameFont = new Font(_latoBold, 34);

            winImage.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(450, 0)
            }));

            lossImage.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(450, 0)
            }));

            if (duelInfo.Side != WinnerSide.Draw)
            {
                lossImage.Mutate(x => x.Grayscale());
            }

            image.Mutate(x => x.DrawImage(winImage, new Point(Xiw, Yi), 1));
            image.Mutate(x => x.DrawImage(lossImage, new Point(Xil, Yi), 1));

            var winnerColor = Rgba32.FromHex(duelImage != null ? duelImage.Color : DuelImage.DefaultColor());
            var loserColor = Rgba32.FromHex(duelImage != null ? duelImage.Color : DuelImage.DefaultColor());

            var options = new TextGraphicsOptions() { HorizontalAlignment = HorizontalAlignment.Center, WrapTextWidth = winImage.Width };
            image.Mutate(x => x.DrawText(options, duelInfo.Winner.Name, nameFont, winnerColor, new Point(Xiw, Yt)));
            image.Mutate(x => x.DrawText(options, duelInfo.Loser.Name, nameFont, loserColor, new Point(Xil, Yt)));

            return image;
        }

        public Image<Rgba32> GetCatchThatWaifuImage(Image<Rgba32> card, string pokeImg, int xPos, int yPos)
        {
            var image = Image.Load(pokeImg);
            image.Mutate(x => x.DrawImage(card, new Point(xPos, yPos), 1));
            return image;
        }

        public async Task<Image<Rgba32>> GetWaifuCardAsync(string? url, Card card)
        {
            if (url == null)
            {
                return await GetWaifuCardImageAsync(card);
            }

            return Image.Load(url);
        }

        public async Task<Image<Rgba32>> GetWaifuCardImageAsync(Card card)
        {
            var image = await GetWaifuCardNoStatsAsync(card);

            if (card.FromFigure)
            {
                ApplyUltimateStats(image, card);
            }
            else
            {
                ApplyStats(image, card, !card.HasImage());
            }

            return image;
        }
    }
}