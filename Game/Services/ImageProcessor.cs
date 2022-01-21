using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.Common.Models;
using Sanakan.DAL.Models;
using Sanakan.Extensions;
using Sanakan.Game.Extensions;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Utilities;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly Point _origin = new(0, 0);
        private readonly IFileSystem _fileSystem;
        private readonly IImageResolver _imageResolver;
        private readonly IDictionary<int, Font> _fontCache;
        private readonly IEnumerable<string> _extensions = new[] { "png", "jpeg", "gif", "jpg" };

        public ImageProcessor(
            IOptionsMonitor<ImagingConfiguration> options,
            IResourceManager resourceManager,
            IFileSystem fileSystem,
            IImageResolver imageResolver)
        {
            _options = options;
            _resourceManager = resourceManager;
            _fileSystem = fileSystem;
            _imageResolver = imageResolver;
            _fontCollection = new FontCollection();

            _digital = LoadFontFromStream(Resources.DigitalFont);
            _latoBold = LoadFontFromStream(Resources.LatoBoldFont);
            _latoLight = LoadFontFromStream(Resources.LatoLightfont);
            _latoRegular = LoadFontFromStream(Resources.LatoRegularfont);
            _fontCache = new Dictionary<int, Font>(10);
        }

        public Font GetOrCreateFont(FontFamily fontFamily, float size)
        {
            var key = fontFamily.GetHashCode() + (int)size;
            if(!_fontCache.TryGetValue(key, out var font))
            {
                font = new Font(fontFamily, size);
                _fontCache[key] = font;
            }

            return font;
        }

        public async Task SaveImageFromUrlAsync(string url, string path)
            => await SaveImageFromUrlAsync(url, path, Size.Empty);

        public async Task<Image> GetWaifuInProfileCardAsync(Card card)
        {
            var image = new Image<Rgba32>(_options.CurrentValue.CharacterImageWidth, _options.CurrentValue.CharacterImageHeight);

            await ApplyBorderBack(image, card);

            var imageUrl = card.GetImage()!;
            using var characterImage = await GetCharacterPictureAsync(imageUrl, card.FromFigure);
            var mov = card.FromFigure ? 0 : 13;
            image.Mutate(x => x.DrawImage(characterImage, new Point(mov, mov), 1));

            using var borderImage = await LoadCustomBorderAsync(card);
            image.Mutate(x => x.DrawImage(borderImage, _origin, 1));

            if (AllowStatsOnNoStatsImage(card))
            {
                await ApplyUltimateStatsAsync(image, card);
            }

            return image;
        }

        public async Task<Image> GetDuelCardImageAsync(
            DuelInfo duelInfo,
            DuelImage? duelImage,
            Image winImage,
            Image lossImage)
        {
            var xiw = 76;
            var yt = 780;
            var yi = 131;
            var xil = 876;

            if (duelInfo.Side == WinnerSide.Right)
            {
                xiw = 876;
                xil = 76;
            }

            Image image;

            if (duelImage == null)
            {
                image = await LoadImageAsync(DuelImage.DefaultUri((int)duelInfo.Side));
            }
            else
            {
                var imagePath = string.Format(Paths.DuelPicture, duelImage.Name, duelInfo.Side);
                if (_fileSystem.Exists(imagePath))
                {
                    image = await LoadImageAsync(imagePath);
                }
                else
                {
                    imagePath = DuelImage.DefaultUri((int)duelInfo.Side);
                    image = await LoadImageAsync(imagePath);
                }
            }

            var nameFont = GetOrCreateFont(_latoBold, 34);

            winImage.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(450, 0),
            }));

            lossImage.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(450, 0),
            }));

            if (duelInfo.Side != WinnerSide.Draw)
            {
                lossImage.Mutate(x => x.Grayscale());
            }

            image.Mutate(x => x.DrawImage(winImage, new Point(xiw, yi), 1));
            image.Mutate(x => x.DrawImage(lossImage, new Point(xil, yi), 1));

            var winnerColor = Rgba32.ParseHex(duelImage != null ? duelImage.Color : DuelImage.DefaultColor());
            var loserColor = Rgba32.ParseHex(duelImage != null ? duelImage.Color : DuelImage.DefaultColor());

            var textOptions = new TextOptions(nameFont)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                WrappingLength = winImage.Width,
            };

            textOptions.Origin = new Point(xiw, yt);
            image.Mutate(x => x.DrawText(textOptions, duelInfo.Winner.Name, winnerColor));
            textOptions.Origin = new Point(xil, yt);
            image.Mutate(x => x.DrawText(textOptions, duelInfo.Loser.Name, loserColor));

            return image;
        }

        public async Task<Image> GetCatchThatWaifuImageAsync(Image card, string pokeImg, int xPos, int yPos)
        {
            var image = await LoadImageAsync(pokeImg);
            image.Mutate(x => x.DrawImage(card, new Point(xPos, yPos), 1));
            return image;
        }

        public async Task<Image> GetWaifuCardImageAsync(Card card)
        {
            var image = await GetWaifuCardNoStatsAsync(card);

            if (card.FromFigure)
            {
                await ApplyUltimateStatsAsync(image, card);
            }
            else
            {
                await ApplyStatsAsync(image, card, !card.HasImage());
            }

            return image;
        }

        public async Task SaveImageFromUrlAsync(
            string imageUrl,
            string filePath,
            Size size,
            bool stretch = false)
        {
            using var stream = await GetImageFromUrlAsync(new Uri(imageUrl), true);
            using var image = await Image.LoadAsync(stream);

            if (size.Height > 0 || size.Width > 0)
            {
                CheckProfileImageSize(image, size, stretch);
            }

            image.SaveToPath(filePath, _fileSystem);
        }

        public async Task<Image> GetUserProfileAsync(
            UserInfo? shindenUser,
            User databaseUser,
            string avatarUrl,
            long topPos,
            string nickname,
            Discord.Color color)
        {
            if (color == Discord.Color.Default)
            {
                color = Discord.Color.DarkerGrey;
            }

            var rangName = shindenUser?.Rank ?? string.Empty;
            var colorRank = color.RawValue.ToString("X6");

            var nickFont = GetFontSize(_latoBold, 28, nickname, 290);
            var rangFont = GetOrCreateFont(_latoRegular, 16);
            var levelFont = GetOrCreateFont(_latoBold, 40);

            var template = await LoadImageAsync(Paths.ProfileBodyPicture);
            var profilePic = new Image<Rgba32>(template.Width, template.Height);

            if (!_fileSystem.Exists(databaseUser.BackgroundProfileUri))
            {
                databaseUser.BackgroundProfileUri = Paths.DefaultBackgroundPicture;
            }

            using var userBackground = await LoadImageAsync(databaseUser.BackgroundProfileUri);
            profilePic.Mutate(x => x.DrawImage(userBackground, _origin, 1));
            profilePic.Mutate(x => x.DrawImage(template, _origin, 1));

            template.Dispose();

            var imageStream = await GetImageFromUrlAsync(new Uri(avatarUrl));
            using var avatar = await Image.LoadAsync(imageStream);

            imageStream.Close();
            using var avBack = new Image<Rgba32>(82, 82);
            avBack.Mutate(x => x.BackgroundColor(Rgba32.ParseHex(colorRank)));
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

            var levelTextOptions = new TextOptions(levelFont);
            var mLevel = TextMeasurer.Measure($"{databaseUser.Level}", levelTextOptions);
            profilePic.Mutate(x => x.DrawText($"{databaseUser.Level}", levelFont, defFontColor, new Point((int)(125 - mLevel.Width) / 2, 206)));

            var mTopPos = TextMeasurer.Measure($"{topPos}", levelTextOptions);
            profilePic.Mutate(x => x.DrawText($"{topPos}", levelFont, posColor, new Point((int)(125 - mTopPos.Width) / 2, 284)));

            var rangTextOptions = new TextOptions(rangFont);
            var mScOwn = TextMeasurer.Measure($"{databaseUser.ScCount}", rangTextOptions);
            rangTextOptions.Origin = new Point((int)(125 - mScOwn.Width) / 2, 365);
            profilePic.Mutate(x => x.DrawText(rangTextOptions, $"{databaseUser.ScCount}", defFontColor));

            var mTcOwn = TextMeasurer.Measure($"{databaseUser.TcCount}", rangTextOptions);
            rangTextOptions.Origin = new Point((int)(125 - mTcOwn.Width) / 2, 405);
            profilePic.Mutate(x => x.DrawText(rangTextOptions, $"{databaseUser.TcCount}", defFontColor));

            var mMsg = TextMeasurer.Measure($"{databaseUser.MessagesCount}", rangTextOptions);
            rangTextOptions.Origin = new Point((int)(125 - mMsg.Width) / 2, 445);
            profilePic.Mutate(x => x.DrawText(rangTextOptions, $"{databaseUser.MessagesCount}", defFontColor));

            var favouriteWaifuId = databaseUser.GameDeck.FavouriteWaifuId;

            if (favouriteWaifuId.HasValue
                && databaseUser.ShowWaifuInProfile)
            {
                var favouriteCard = databaseUser.GameDeck.Cards
                    .OrderBy(x => x.Rarity)
                    .FirstOrDefault(x => x.CharacterId == favouriteWaifuId);

                if (favouriteCard != null)
                {
                    using var cardImage = await GetWaifuInProfileCardAsync(favouriteCard);
                    cardImage.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(105, 0),
                    }));
                    profilePic.Mutate(x => x.DrawImage(cardImage, new Point(10, 350), 1));
                }
            }

            var prevLvlExp = ExperienceUtils.CalculateExpForLevel(databaseUser.Level);
            var nextLvlExp = ExperienceUtils.CalculateExpForLevel(databaseUser.Level + 1);
            var expOnLvl = databaseUser.ExperienceCount - prevLvlExp;
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
            var mExp = TextMeasurer.Measure(expText, rangTextOptions);
            rangTextOptions.Origin = new Point(135 + ((int)(305 - mExp.Width) / 2), 204);
            profilePic.Mutate(x => x.DrawText(rangTextOptions, expText, Colors.White));

            using var inside = await GetProfileInside(shindenUser, databaseUser);
            profilePic.Mutate(x => x.DrawImage(inside, new Point(125, 228), 1));

            return profilePic;
        }

        public async Task<Image> GetSiteStatisticAsync(
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

            using var template = await LoadImageAsync(Paths.SiteStatisticsPicture);
            baseImg.Mutate(x => x.DrawImage(template, _origin, 1));

            var avatarUrl = UrlHelpers.GetUserAvatarURL(shindenInfo.AvatarId, shindenInfo.Id!.Value);
            var hexColor = color.RawValue.ToString("X6");

            using var avatar = await GetSiteStatisticUserBadge(
                avatarUrl.ToString(),
                shindenInfo.Name,
                hexColor);
            baseImg.Mutate(x => x.DrawImage(avatar, _origin, 1));

            using var image = new Image<Rgba32>(325, 248);
            if (shindenInfo?.MeanAnimeScore != null)
            {
                using var stats = await GetRWStatsAsync(
                    shindenInfo,
                    Paths.StatsAnimePicture,
                    false);

                image.Mutate(x => x.DrawImage(stats, _origin, 1));
            }

            if (shindenInfo?.MeanMangaScore != null)
            {
                using var stats = await GetRWStatsAsync(
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

        public async Task<Image> GetLevelUpBadgeAsync(
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

            var textFont = GetOrCreateFont(_latoRegular, 16);
            var nickNameFont = GetOrCreateFont(_latoBold, 22);
            var lvlFont = GetOrCreateFont(_latoBold, 36);

            var messageTextOptions = new TextOptions(textFont);
            var msgText1Length = TextMeasurer.Measure(msgText1, messageTextOptions);
            var msgText2Length = TextMeasurer.Measure(msgText2, messageTextOptions);

            var nicknameTextOptions = new TextOptions(nickNameFont);
            var nameLength = TextMeasurer.Measure(name, nicknameTextOptions);

            var levelTextOptions = new TextOptions(lvlFont);
            var lvlLength = TextMeasurer.Measure(userLevel.ToString(), levelTextOptions);

            var textLength = lvlLength.Width + msgText1Length.Width > nameLength.Width ? lvlLength.Width + msgText1Length.Width : nameLength.Width;
            var estimatedLength = 106 + (int)(textLength > msgText2Length.Width ? textLength : msgText2Length.Width);

            var nickNameColor = Rgba32.ParseHex(color.RawValue.ToString("X6"));
            var baseImg = new Image<Rgba32>((int)estimatedLength, 100);

            baseImg.Mutate(x => x.BackgroundColor(Colors.Onyx));

            messageTextOptions.Origin = new Point(98 + (int)lvlLength.Width, 75);
            baseImg.Mutate(x => x.DrawText(messageTextOptions, msgText1, Colors.Gray));

            nicknameTextOptions.Origin = new Point(98, 10);
            baseImg.Mutate(x => x.DrawText(nicknameTextOptions, name, nickNameColor));

            messageTextOptions.Origin = new Point(98, 33);
            baseImg.Mutate(x => x.DrawText(messageTextOptions, msgText2, Colors.Gray));

            levelTextOptions.Origin = new Point(96, 61);
            baseImg.Mutate(x => x.DrawText(levelTextOptions, userLevel.ToString(), Colors.Gray));

            using var colorRec = new Image<Rgba32>(82, 82);

            colorRec.Mutate(x => x.BackgroundColor(nickNameColor));
            baseImg.Mutate(x => x.DrawImage(colorRec, new Point(9, 9), 1));

            using var stream = await GetImageFromUrlAsync(new Uri(avatarUrl));

            if (stream == null)
            {
                return baseImg;
            }

            using var avatar = await Image.LoadAsync(stream);

            avatar.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Crop,
                Size = new Size(80, 80),
            }));
            baseImg.Mutate(x => x.DrawImage(avatar, new Point(10, 10), 1));

            return baseImg;
        }

        public Image GetFColorsView(IEnumerable<(string, uint)> colours)
        {
            var font = GetOrCreateFont(_latoRegular, 16);
            var textOptions = new TextOptions(font);
            var columnMaxLength = TextMeasurer.Measure("A", textOptions);

            var inFirstColumn = (colours.Count() + 1) / 2;

            var index = 1;
            foreach (var colour in colours)
            {
                var nLen = TextMeasurer.Measure(colour.Item1, textOptions);

                if (index < inFirstColumn + 1)
                {
                    if (columnMaxLength.Width < nLen.Width)
                    {
                        columnMaxLength = nLen;
                    }
                }
                else
                {
                    if (columnMaxLength.Width < nLen.Width)
                    {
                        columnMaxLength = nLen;
                    }
                }

                index++;
            }

            var posY = 5;
            var posX = 0;
            var realWidth = (int)(columnMaxLength.Width + columnMaxLength.Width + 20);
            var realHeight = (int)(columnMaxLength.Height + 2) * (inFirstColumn + 1);

            var imgBase = new Image<Rgba32>(realWidth, realHeight);
            imgBase.Mutate(x => x.BackgroundColor(Colors.Onyx));
            imgBase.Mutate(x => x.DrawText("Lista:", font, Colors.Black, _origin));

            index = 1;
            foreach (var colour in colours)
            {
                if (inFirstColumn + 1 == index)
                {
                    posY = 5;
                    posX = (int)columnMaxLength.Width + 10;
                }

                posY += (int)columnMaxLength.Height + 2;
                var structColour = Rgba32.ParseHex(colour.Item2.ToString("X6"));
                imgBase.Mutate(x => x.DrawText(colour.Item1, font, structColour, new Point(posX, posY)));
                index++;
            }

            return imgBase;
        }

        private async Task<Image> GetProfileInside(UserInfo? shindenUser, User databaseUser)
        {
            var config = _options.CurrentValue;
            var image = new Image<Rgba32>(config.ProfileImageWidth, config.ProfileImageHeight);

            if (!_fileSystem.Exists(databaseUser.StatsReplacementProfileUri!))
            {
                if (databaseUser.ProfileType == ProfileType.Image
                    || databaseUser.ProfileType == ProfileType.StatisticsWithImage)
                {
                    databaseUser.ProfileType = ProfileType.Statistics;
                }
            }

            switch (databaseUser.ProfileType)
            {
                case ProfileType.Statistics:
                case ProfileType.StatisticsWithImage:
                    if (shindenUser != null)
                    {
                        if (shindenUser.WatchedStatus != null)
                        {
                            using var stats = await GetRWStatsAsync(
                                shindenUser,
                                Paths.StatsAnimePicture,
                                false);
                            image.Mutate(x => x.DrawImage(stats, new Point(0, 2), 1));
                        }

                        if (shindenUser.ReadedStatus != null)
                        {
                            using var stats = await GetRWStatsAsync(
                                shindenUser,
                                Paths.StatsMangaPicture,
                                true);
                            image.Mutate(x => x.DrawImage(stats, new Point(0, 142), 1));
                        }

                        if (databaseUser.ProfileType == ProfileType.StatisticsWithImage)
                        {
                            goto case ProfileType.Image;
                        }
                    }

                    break;

                case ProfileType.Cards:
                    {
                        using var cardsBg = GetCardsProfileImage(databaseUser).Result;
                        image.Mutate(x => x.DrawImage(cardsBg, _origin, 1));
                    }

                    break;

                case ProfileType.Image:
                    {
                        using var userBackground = await LoadImageAsync(databaseUser.StatsReplacementProfileUri!);
                        image.Mutate(x => x.DrawImage(userBackground, _origin, 1));
                    }

                    break;
            }

            return image;
        }

        private async Task<Image<Rgba32>> GetCardsProfileImage(User databaseUser)
        {
            var config = _options.CurrentValue;
            var profilePic = new Image<Rgba32>(config.ProfileImageWidth, config.ProfileImageHeight);
            var gameDeck = databaseUser.GameDeck;
            var cards = gameDeck.Cards;

            if (gameDeck.FavouriteWaifuId.HasValue)
            {
                var favouriteCharacter = gameDeck.Cards
                    .OrderBy(x => x.Rarity)
                    .FirstOrDefault(x => x.CharacterId == gameDeck.FavouriteWaifuId);

                if (favouriteCharacter != null)
                {
                    using var cardImage = await GetWaifuInProfileCardAsync(favouriteCharacter);

                    cardImage.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(0, 260),
                    }));
                    profilePic.Mutate(x => x.DrawImage(cardImage, new Point(10, 6), 1));
                }
            }

            var cardRarityStats = cards.GetRarityStats();

            var jumpY = 18;
            var row2X = 45;
            var startY = 12;
            var startX = 205;
            var font1 = GetFontSize(_latoBold, 18, "SUM", 100);
            var font2 = GetFontSize(_latoLight, 18, "10000", 130);

            var defaultXPosition = startX + row2X;
            var textColor = Colors.Dawn;

            var rarityMap = new[]
            {
                ("SSS", cardRarityStats.SSS.ToString(), jumpY, defaultXPosition),
                ("SS", cardRarityStats.SS.ToString(), jumpY, defaultXPosition),
                ("S", cardRarityStats.S.ToString(), jumpY, defaultXPosition),
                ("A", cardRarityStats.A.ToString(), jumpY, defaultXPosition),
                ("B", cardRarityStats.B.ToString(), jumpY, defaultXPosition),
                ("C", cardRarityStats.C.ToString(), jumpY, defaultXPosition),
                ("D", cardRarityStats.D.ToString(), jumpY, defaultXPosition),
                ("E", cardRarityStats.E.ToString(), jumpY, defaultXPosition),
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
            var point = new Point(72, 6 + (int)((58 - font.Size) / 2));
            badge.Mutate(x => x.DrawText(name, font, Colors.Dawn, point));

            using var border = new Image<Rgba32>(3, 57);
            border.Mutate(x => x.BackgroundColor(Rgba32.ParseHex(color)));
            badge.Mutate(x => x.DrawImage(border, new Point(63, 5), 1));

            using var stream = await GetImageFromUrlAsync(new Uri(avatarUrl));

            if (stream == null)
            {
                return badge;
            }

            using var avatar = await Image.LoadAsync(stream);

            avatar.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Crop,
                Size = new Size(57, 57),
            }));
            badge.Mutate(x => x.DrawImage(avatar, new Point(6, 5), 1));

            return badge;
        }

        private async Task<Image> GetRWStatsAsync(UserInfo userInfo, string path, bool isManga)
        {
            var startPointX = 7;
            var startPointY = 3;
            var baseImg = await LoadImageAsync(path);

            var status = userInfo.ReadedStatus;
            var meanScore = isManga ? userInfo.MeanMangaScore : userInfo.MeanAnimeScore;

            if (status.Total.HasValue && status.Total > 0)
            {
                using var statusBar = GetStatusBar(
                    status.Total.Value,
                    status.InProgress!.Value,
                    status.Completed!.Value,
                    status.Skip!.Value,
                    status.Hold!.Value,
                    status.Dropped!.Value,
                    status.Plan!.Value);

                statusBar.Mutate(x => x.Round(5));
                baseImg.Mutate(x => x.DrawImage(statusBar, new Point(startPointX, startPointY), 1));
            }

            startPointY += 24;
            startPointX += 110;
            int ySecondStart = startPointY;
            int fontSizeAndInterline = 10 + 6;
            var font = GetOrCreateFont(_latoBold, 13);
            int xSecondRow = startPointX + 200;
            var fontColor = Colors.SmokeyGrey;

            var rowArr = new[]
            {
                status?.InProgress,
                status?.Completed,
                status?.Skip,
                status?.Hold,
                status?.Dropped,
                status?.Plan,
            };

            for (var i = 0; i < rowArr.Length; i++)
            {
                baseImg.Mutate(x => x.DrawText($"{rowArr[i]}", font, fontColor, new Point(startPointX, startPointY)));
                startPointY += fontSizeAndInterline;
            }

            var textOptions = new TextOptions(font)
            {
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            var point = new Point(xSecondRow, ySecondStart);
            textOptions.Origin = point;
            var scoreCount = meanScore.ScoreCount!.Value;

            baseImg.Mutate(x => x.DrawText(textOptions, scoreCount.ToString("0.0"), fontColor));
            ySecondStart += fontSizeAndInterline;

            point.X = xSecondRow;
            point.Y = ySecondStart;
            baseImg.Mutate(x => x.DrawText(textOptions, status?.Total?.ToString(), fontColor));
            ySecondStart += fontSizeAndInterline;

            point.X = xSecondRow;
            point.Y = ySecondStart;
            baseImg.Mutate(x => x.DrawText(textOptions, scoreCount.ToString(), fontColor));
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
                var fs = listTime.First();
                listTime.Remove(fs);
                var sc = listTime.First();
                listTime.Remove(sc);

                textOptions.Origin = new Point(xSecondRow, ySecondStart);
                baseImg.Mutate(x => x.DrawText(textOptions, $"{fs} {sc}", fontColor));

                ySecondStart += fontSizeAndInterline;
            }

            var text = string.Join<string>(" ", listTime);
            textOptions.Origin = new Point(xSecondRow, ySecondStart);
            baseImg.Mutate(x => x.DrawText(textOptions, text, fontColor));

            return baseImg;
        }

        private Image GetStatusBar(
            ulong all,
            ulong green,
            ulong blue,
            ulong purple,
            ulong yellow,
            ulong red,
            ulong grey)
        {
            var offset = 0;
            var length = 311;
            var fixedLength = 0;

            var arrLength = new int[6];
            var arrProcent = new double[6];
            var arrValues = new double[] { green, blue, purple, yellow, red, grey };

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

            var height = 17;
            var bar = new Image<Rgba32>(length, height);
            for (var i = 0; i < arrValues.Length - 1; i++)
            {
                if (arrValues[i] != 0)
                {
                    var width = arrLength[i] < 1 ? 1 : arrLength[i];
                    using var thisBar = new Image<Rgba32>(width, height);
                    thisBar.Mutate(x => x.BackgroundColor(Colors.StatusBarColors[i]));
                    bar.Mutate(x => x.DrawImage(thisBar, new Point(offset, 0), 1));
                    offset += arrLength[i];
                }
            }

            return bar;
        }

        private async Task<Image?> GetLastRWListCoverAsync(Stream imageStream)
        {
            if (imageStream == null)
            {
                return null;
            }

            var cover = await Image.LoadAsync(imageStream);
            cover.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(20, 50),
            }));

            return cover;
        }

        private async Task<Image<Rgba32>> GetLastRWList(
            IEnumerable<LastWatchedRead>? lastRead,
            IEnumerable<LastWatchedRead>? lastWatch)
        {
            lastRead ??= Enumerable.Empty<LastWatchedRead>();
            lastWatch ??= Enumerable.Empty<LastWatchedRead>();

            var titleFont = GetOrCreateFont(_latoBold, 10);
            var nameFont = GetOrCreateFont(_latoBold, 16);
            var fColor = Colors.LemonGrass;
            int startY = 25;

            var image = new Image<Rgba32>(175, 248);
            image.Mutate(x => x.DrawText("Ostatnio obejrzane:", nameFont, fColor, new Point(0, 5)));

            var yOffset = 0;
            foreach (var last in lastWatch.Take(4))
            {
                using var stream = await GetImageFromUrlAsync(last.AnimeCoverUrl, true);
                using var cover = await GetLastRWListCoverAsync(stream!);
                if (cover != null)
                {
                    image.Mutate(x => x.DrawImage(cover, new Point(0, startY + (35 * yOffset)), 1));
                }

                var title = last.Title.ElipseTrimToLength(29);
                var epsiodes = $"{last.EpisodeNo} / {last.EpisodesCount}";
                var point = new Point(25, startY + (35 * yOffset));

                image.Mutate(x => x.DrawText(title, titleFont, fColor, point));

                point.Y += 11;
                image.Mutate(x => x.DrawText(epsiodes, titleFont, fColor, point));
                yOffset++;
            }

            startY += 128;
            image.Mutate(x => x.DrawText("Ostatnio przeczytane:", nameFont, fColor, new Point(0, 133)));

            yOffset = 0;
            foreach (var last in lastRead.Take(4))
            {
                using var stream = await GetImageFromUrlAsync(last.MangaCoverUrl, true);
                using var cover = await GetLastRWListCoverAsync(stream!);

                if (cover != null)
                {
                    image.Mutate(x => x.DrawImage(cover, new Point(0, startY + (35 * yOffset)), 1));
                }

                var title = last.Title.ElipseTrimToLength(29);
                var chapters = $"{last.ChapterNo} / {last.ChaptersCount}";
                var point = new Point(25, startY + (35 * yOffset));

                image.Mutate(x => x.DrawText(title, titleFont, fColor, point));

                point.Y += 11;
                image.Mutate(x => x.DrawText(chapters, titleFont, fColor, point));
                yOffset++;
            }

            return image;
        }

        private async Task<Image> GetCharacterPictureAsync(string characterUrl, bool ultimate)
        {
            var characterImg = await LoadImageAsync(Paths.PWEmptyPicture);

            if (ultimate)
            {
                characterImg = new Image<Rgba32>(_options.CurrentValue.CharacterImageWidth, _options.CurrentValue.CharacterImageHeight);
            }

            var url = characterUrl == null ? ShindenApi.Constants.PlaceholderImageUrl : new Uri(characterUrl);
            var stream = await GetImageFromUrlAsync(url, true);

            if (stream == null)
            {
                return characterImg;
            }

            using var image = await Image.LoadAsync(stream);

            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(characterImg.Width, 0),
            }));

            int startY = 0;
            if (characterImg.Height > image.Height)
            {
                startY = (characterImg.Height / 2) - (image.Height / 2);
            }

            characterImg.Mutate(x => x.DrawImage(image, new Point(0, startY), 1));

            return characterImg;
        }

        private bool HasCustomBorderString(Card card)
        {
            switch (card.Quality)
            {
                case Quality.Gamma: return true;
                default: return false;
            }
        }

        private string GetCustomBorderString(Card card)
        {
            var border = "Border.png";

            switch (card.Quality)
            {
                case Quality.Gamma:
                    {
                        var totalPower = card.GetHealthWithPenalty();
                        totalPower += card.GetDefenceWithBonus();
                        totalPower += card.GetAttackWithBonus();

                        if (totalPower > 5000)
                        {
                            border = "Border_2";
                            break;
                        }

                        if (totalPower > 2000)
                        {
                            border = "Border_1.png";
                            break;
                        }

                        border = "Border_0.png";
                    }

                    break;
            }

            return string.Format(Paths.PWCGFolderFile, card.Quality, border);
        }

        private async Task<Image> GenerateBorderAsync(Card card)
        {
            var borderStr = string.Format(Paths.PWPicture, card.Rarity);
            var dereStr = string.Format(Paths.PWPicture, card.Dere);

            if (card.FromFigure)
            {
                borderStr = string.Format(Paths.PWCGPicture, card.Quality);
                dereStr = dereStr = string.Format(Paths.PWCGDerePicture, card.Quality, card.Dere);
            }

            if (HasCustomBorderString(card))
            {
                borderStr = GetCustomBorderString(card);
            }

            var image = await LoadImageAsync(borderStr);

            using var dereImage = await LoadImageAsync(borderStr);
            image.Mutate(x => x.DrawImage(dereImage, _origin, 1));

            return image;
        }

        private async Task<Image> LoadCustomBorderAsync(Card card)
        {
            if (card.CustomBorderUrl == null)
            {
                return await GenerateBorderAsync(card);
            }

            using var stream = await GetImageFromUrlAsync(card.CustomBorderUrl);

            if (stream == null)
            {
                return await GenerateBorderAsync(card);
            }

            return await Image.LoadAsync(stream);
        }

        private void ApplyAlphaStats(Image image, Card card)
        {
            var adFont = GetOrCreateFont(_latoBold, 36);
            var hpFont = GetOrCreateFont(_latoBold, 32);

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

        private void ApplyBetaStats(Image image, Card card)
        {
            var adFont = GetOrCreateFont(_latoBold, 36);
            var hpFont = GetOrCreateFont(_latoBold, 29);

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

        private void ApplyGammaStats(Image image, Card card)
        {
            var aphFont = GetOrCreateFont(_latoBold, 26);

            var healthPoints = card.GetHealthWithPenalty().ToString();
            var defencePoints = card.GetDefenceWithBonus().ToString();
            var attackPoints = card.GetAttackWithBonus().ToString();

            var textOptions = new TextOptions(aphFont);

            // TODO: center numbers
            textOptions.Origin = new Point(115, 593);
            image.Mutate(x => x.DrawText(textOptions, attackPoints, Colors.BrickRed));

            textOptions.Origin = new Point(155, 565);
            image.Mutate(x => x.DrawText(textOptions, defencePoints, Colors.Mariner));

            textOptions.Origin = new Point(300, 593);
            image.Mutate(x => x.DrawText(textOptions, healthPoints, Colors.LaPalma));
        }

        private void ApplyDeltaStats(Image image, Card card)
        {
            var hpFont = GetOrCreateFont(_latoBold, 34);
            var adFont = GetOrCreateFont(_latoBold, 26);

            var healthPoints = card.GetHealthWithPenalty().ToString();
            var defencePoints = card.GetDefenceWithBonus().ToString();
            var attackPoints = card.GetAttackWithBonus().ToString();

            var textOptions = new TextOptions(hpFont);
            textOptions.Origin = new Point(1);

            var options = _options.CurrentValue;
            using var healthPointsImage = new Image<Rgba32>(options.StatsImageWidth, options.StatsImageHeight);

            healthPointsImage.Mutate(x => x.DrawText(textOptions, healthPoints, Colors.Pine));
            healthPointsImage.Mutate(x => x.Rotate(-27));

            image.Mutate(x => x.DrawImage(healthPointsImage, new Point(333, 490), 1));

            // TODO: center numbers
            textOptions = new TextOptions(adFont);
            textOptions.Origin = new Point(62, 600);
            image.Mutate(x => x.DrawText(textOptions, attackPoints, Colors.MetallicCopper));

            textOptions.Origin = new Point(352, 600);
            image.Mutate(x => x.DrawText(textOptions, defencePoints, Colors.DeepSeaBlue));
        }

        private void ApplyEpsilonStats(Image image, Card card)
        {
            var aphFont = GetOrCreateFont(_latoBold, 28);

            var healthPoints = card.GetHealthWithPenalty().ToString();
            var defencePoints = card.GetDefenceWithBonus().ToString();
            var attackPoints = card.GetAttackWithBonus().ToString();

            var textOptions = new TextOptions(aphFont)
            {
                KerningMode = KerningMode.Normal,
                Dpi = 80,
            }; // TODO: rotate hp.
            var point = new Point(59, 365);
            textOptions.Origin = point;

            image.Mutate(x => x.DrawText(textOptions, healthPoints, Colors.BrightLightGreen));

            point.X += 5;
            point.Y += 67;
            image.Mutate(x => x.DrawText(textOptions, attackPoints, Colors.DeepOrange));

            point.X -= 9;
            point.Y += 53;
            image.Mutate(x => x.DrawText(textOptions, defencePoints, Colors.Azure));
        }

        private void ApplyZetaStats(Image image, Card card)
        {
            var aphFont = GetOrCreateFont(_digital, 28);

            var healthPoints = card.GetHealthWithPenalty().ToString("D5");
            var defencePoints = card.GetDefenceWithBonus().ToString("D4");
            var attackPoints = card.GetAttackWithBonus().ToString("D4");

            var textOptions = new TextOptions(aphFont)
            {
                KerningMode = KerningMode.Normal,
                Dpi = 80,
            };
            var point = new Point(342, 538);
            textOptions.Origin = point;

            image.Mutate(x => x.DrawText(textOptions, attackPoints, Colors.DeepOrange));

            point.X += 27;
            point.Y += 565;
            image.Mutate(x => x.DrawText(textOptions, defencePoints, Colors.Azure));

            point.X -= 14;
            point.Y += 28;
            image.Mutate(x => x.DrawText(textOptions, healthPoints, Colors.BrightLightGreen));
        }

        private void ApplyLambdaStats(Image image, Card card)
        {
            var aphFont = GetOrCreateFont(_latoBold, 28);

            var healthPoints = card.GetHealthWithPenalty().ToString();
            var defencePoints = card.GetDefenceWithBonus().ToString();
            var attackPoints = card.GetAttackWithBonus().ToString();

            var textOptions = new TextOptions(aphFont);
            textOptions.Origin = new Point(1);

            var options = _options.CurrentValue;
            using var healthPointImage = new Image<Rgba32>(options.StatsImageWidth, options.StatsImageHeight);
            healthPointImage.Mutate(x => x.DrawText(textOptions, healthPoints, Colors.LightGreenishBlue));
            healthPointImage.Mutate(x => x.Rotate(-19));
            image.Mutate(x => x.DrawImage(healthPointImage, new Point(57, 555), 1));

            using var attackImage = new Image<Rgba32>(options.StatsImageWidth, options.StatsImageHeight);
            attackImage.Mutate(x => x.DrawText(textOptions, attackPoints, Colors.BlossomPink));
            attackImage.Mutate(x => x.Rotate(34));
            image.Mutate(x => x.DrawImage(attackImage, new Point(80, 485), 1));

            textOptions.Origin = new Point(326, 576);
            image.Mutate(x => x.DrawText(textOptions, defencePoints, Colors.BlueDiamond));
        }

        private Rgba32 GetThetaStatColorString(Card card)
        {
            return Rgba32.ParseHex(card.Dere switch
            {
                Dere.Bodere => "#ff2700",
                Dere.Dandere => "#00fd8b",
                Dere.Deredere => "#003bff",
                Dere.Kamidere => "#f6f901",
                Dere.Kuudere => "#008fff",
                Dere.Mayadere => "#ff00df",
                Dere.Raito => "#ffffff",
                Dere.Tsundere => "#ff0072",
                Dere.Yami => "#565656",
                Dere.Yandere => "#ffa100",
                Dere.Yato => "#ffffff",
                _ => "#ffffff",
            });
        }

        private void ApplyThetaStats(Image image, Card card)
        {
            var aphFont = GetOrCreateFont(_digital, 28);

            var healthPoints = card.GetHealthWithPenalty().ToString();
            var defencePoints = card.GetDefenceWithBonus().ToString();
            var attackPoints = card.GetAttackWithBonus().ToString();

            var thetaColor = GetThetaStatColorString(card);

            var textOptions = new TextOptions(aphFont)
            {
                KerningMode = KerningMode.Normal,
                Dpi = 80,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            var point = new Point(410, 517);
            textOptions.Origin = point;

            image.Mutate(x => x.DrawText(textOptions, attackPoints, thetaColor));

            point.Y += 37;
            image.Mutate(x => x.DrawText(textOptions, defencePoints, thetaColor));

            point.Y += 37;
            image.Mutate(x => x.DrawText(textOptions, healthPoints, thetaColor));
        }

        private string GetStatsString(Card card)
        {
            return card.Quality switch
            {
                Quality.Theta => string.Format(Paths.PWCGFolderFile, card.Quality, card.Dere),
                _ => string.Format(Paths.PWCGFolderFile, card.Quality, "Stats"),
            };
        }

        private async Task ApplyUltimateStatsAsync(Image image, Card card)
        {
            var path = GetStatsString(card);
            if (_fileSystem.Exists(path))
            {
                using var stats = await LoadImageAsync(path);
                image.Mutate(x => x.DrawImage(stats, _origin, 1));
            }

            switch (card.Quality)
            {
                case Quality.Alpha:
                    ApplyAlphaStats(image, card);
                    break;
                case Quality.Beta:
                    ApplyBetaStats(image, card);
                    break;
                case Quality.Gamma:
                    ApplyGammaStats(image, card);
                    break;
                case Quality.Delta:
                    ApplyDeltaStats(image, card);
                    break;
                case Quality.Epsilon:
                    ApplyEpsilonStats(image, card);
                    break;
                case Quality.Zeta:
                    ApplyZetaStats(image, card);
                    break;
                case Quality.Lambda:
                    ApplyLambdaStats(image, card);
                    break;
                case Quality.Theta:
                    ApplyThetaStats(image, card);
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
                    {
                        return false;
                    }

                    return true;

                default:
                    return false;
            }
        }

        private async Task ApplyStatsAsync(Image image, Card card, bool applyNegativeStats = false)
        {
            var health = card.GetHealthWithPenalty();
            var defence = card.GetDefenceWithBonus();
            var attack = card.GetAttackWithBonus();
            var position = _origin;
            var opacity = 1;

            using var shield = await LoadImageAsync(Paths.ShieldPicture);
            image.Mutate(x => x.DrawImage(shield, position, opacity));

            using var heart = await LoadImageAsync(Paths.HeartPicture);
            image.Mutate(x => x.DrawImage(heart, position, opacity));

            using var fire = await LoadImageAsync(Paths.FirePicture);
            image.Mutate(x => x.DrawImage(fire, position, opacity));

            var starType = card.GetCardStarType();
            var starCnt = card.GetCardStarCount();

            var starX = 239 - (18 * starCnt);
            for (var i = 0; i < starCnt; i++)
            {
                var filePath = string.Format(Paths.PWStarPicture, starType, card.StarStyle);
                using var starStyle = await LoadImageAsync(filePath);
                image.Mutate(x => x.DrawImage(starStyle, new Point(starX, 30), 1));

                starX += 36;
            }

            int startXDef = 390;
            if (defence < 10)
            {
                startXDef += 15;
            }

            if (defence > 99)
            {
                startXDef -= 15;
            }

            int startXAtk = 390;
            if (attack < 10)
            {
                startXAtk += 15;
            }

            if (attack > 99)
            {
                startXAtk -= 15;
            }

            int startXHp = 380;
            if (health < 10)
            {
                startXHp += 15;
            }

            if (health > 99)
            {
                startXHp -= 15;
            }

            var healthStr = health.ToString();
            var defenceStr = defence.ToString();
            var attackStr = attack.ToString();

            var numFont = GetOrCreateFont(_latoBold, 54);
            image.Mutate(x => x.DrawText(healthStr, numFont, Colors.Black, new Point(startXHp, 190)));
            image.Mutate(x => x.DrawText(attackStr, numFont, Colors.Black, new Point(startXAtk, 320)));
            image.Mutate(x => x.DrawText(defenceStr, numFont, Colors.Black, new Point(startXDef, 440)));

            if (applyNegativeStats)
            {
                using var negativeStatsPicture = await LoadImageAsync(Paths.NegativeStatsPicture);
                image.Mutate(x => x.DrawImage(negativeStatsPicture, _origin, 1));
            }
        }

        private async Task ApplyBorderBack(Image image, Card card)
        {
            var isFromFigureOriginalBorder = card.CustomBorderUrl == null && card.FromFigure;
            var backBorderStr = string.Format(Paths.BackBorderPicture, card.Quality);

            if (isFromFigureOriginalBorder && _fileSystem.Exists(backBorderStr))
            {
                using var backBorderImage = await LoadImageAsync(backBorderStr);
                image.Mutate(x => x.DrawImage(backBorderImage, _origin, 1));
            }
        }

        private async Task<Image> GetWaifuCardNoStatsAsync(Card card)
        {
            var image = new Image<Rgba32>(_options.CurrentValue.CharacterImageWidth, _options.CurrentValue.CharacterImageHeight);

            await ApplyBorderBack(image, card);

            using var chara = await GetCharacterPictureAsync(card.GetImage()!, card.FromFigure);
            var mov = card.FromFigure ? 0 : 13;
            image.Mutate(x => x.DrawImage(chara, new Point(mov, mov), 1));

            using var bord = await GenerateBorderAsync(card);
            image.Mutate(x => x.DrawImage(bord, _origin, 1));

            if (AllowStatsOnNoStatsImage(card))
            {
                await ApplyUltimateStatsAsync(image, card);
            }

            return image;
        }

        private FontFamily LoadFontFromStream(string resourceName)
        {
            var fontStream = _resourceManager.GetResourceStream(resourceName);
            return _fontCollection.Add(fontStream);
        }

        private async Task<Stream?> GetImageFromUrlAsync(Uri url, bool fixedExtension = false)
        {
            try
            {
                var stream = await _imageResolver.GetAsync(url);

                if (stream != null)
                {
                    return stream;
                }

                if (!fixedExtension)
                {
                    return null;
                }

                var splited = url.ToString().Split(".");

                foreach (var extension in _extensions)
                {
                    splited[splited.Length - 1] = extension;
                    stream = await _imageResolver.GetAsync(url);

                    return stream;
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
            var font = GetOrCreateFont(fontFamily, size);
            var textOptions = new TextOptions(font);
            var measured = TextMeasurer.Measure(text, textOptions);

            while (measured.Width > maxWidth)
            {
                if (--size < 1)
                {
                    break;
                }

                font = GetOrCreateFont(fontFamily, size);
                measured = TextMeasurer.Measure(text, textOptions);
            }

            return font;
        }

        private void CheckProfileImageSize(Image image, Size size, bool stretch)
        {
            if (image.Width > size.Width || image.Height > size.Height)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = size,
                }));

                return;
            }

            if (!stretch)
            {
                return;
            }

            if (image.Width < size.Width || image.Height < size.Height)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Stretch,
                    Size = size,
                }));
            }
        }

        private Task<Image> LoadImageAsync(string path)
        {
            using var stream = _fileSystem.OpenRead(path);
            return Image.LoadAsync(stream);
        }
    }
}
