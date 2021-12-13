<h1><img width="200px" alt="Sanakan" src="logo.webp" /></h1>

# Sanakan #

[![Coverage Status](https://coveralls.io/repos/github/Jozpod/sanakan/badge.svg?branch=master)](https://coveralls.io/github/Jozpod/sanakan?branch=master)
[![Build status](https://img.shields.io/appveyor/build/Jozpod/sanakan)](https://ci.appveyor.com/project/jozpod/sanakan/branch/master) 
[![CodeFactor](https://img.shields.io/codefactor/grade/github/Jozpod/sanakan/master)](https://www.codefactor.io/repository/github/jozpod/sanakan)
[![License](https://img.shields.io/github/license/Jozpod/sanakan)](https://github.com/Jozpod/sanakan/blob/master/LICENSE)

Sanakan is .net solution which provides Discord bot, web API, and background services.

The Discord bot powers Pocked Waifu game tailored for Shinden discord server and it also contains
helpful commands for moderation, game events and access to Shinden API.

API allows rudimentary bot control, can connect discord user to discord platform, retrieve basic user details and provides utility methods for Pocket Waifu game.

Currently there are few background services which do following things
- Probe memory usage
- Periodically check if penalty applied to user has expired and if so unbans/unmutes him.
- Detect spam/flood/raid
- Process task queue
- Manage discord sessions established between bot and user
- Run Pocket Waifu events ( i.e. Safari Image hunt, Chaos, Experience progression, Timestatus change )

It was forked and refactored from [MrZnake/sanakan](https://github.com/MrZnake/sanakan).

## Tech Stack

![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
![Discord](https://img.shields.io/badge/Discord.NET-%237289DA.svg?style=for-the-badge&logo=discord&logoColor=white)
![Swagger](https://img.shields.io/badge/-Swagger-%23Clojure?style=for-the-badge&logo=swagger&logoColor=white)
![MySQL](https://img.shields.io/badge/mysql-%2300f.svg?style=for-the-badge&logo=mysql&logoColor=white)
![SQLite](https://img.shields.io/badge/sqlite-%2307405e.svg?style=for-the-badge&logo=sqlite&logoColor=white)

## Getting Started ##

> ℹ️ Make sure to customize settings to your needs. By default the app will use Mysql server and it would try to create database if not exist.

```console
dotnet run
```

You can also build project with `make` build automation tool.
> ℹ️  `make` is not shipped with windows. To install it download [chocolatey](https://chocolatey.org/install) and run `choco install make`

```console
make run
```

### Discord Configuration ###

Invite bot to your server

Discord invite template.

```
https://discord.com/api/oauth2/authorize?client_id=<client_id>&permissions=<permissions>&scope=<scope>
```

The next step is channel, role configuration.

For example to set trash spawn waifu channel one needs to invoke following command in a intended channel.

> ℹ️   The default command prefix is *.*

> ⚠️  This command requires guild administrator permission.

```
.mod tsafarich
```

See commands section below for list of available commands.

### Database Configuration ###

#### Sqlite ####

Replace Database section in `appsettings.json`

```json
{
  "Database": {
    "Provider": "Sqlite",
    "Version": "3.0.0",
    "ConnectionString": "Data Source=SanakanDBTest.db;"
  },
}
```

## Commands ##

### Moderation ###
|                   Command                    | Example |             Description             | Permissions required |
| :---------------------------------------: | :-------: | :----------------------------------- | ----------- |
| **[`kasuj`](#kasuj)** | `.mod kasuj {Number}` | Deletes messages. | Configured **Admin role**, **Moderator role**, **Guild Administrator** or **Manage Messages** permission |
| **[`kasuju`](#kasuju)** | `.mod kasuju {Number}` | Deletes user messages. | Configured **Admin role**, **Moderator role**, **Guild Administrator** or **Manage Messages** permission |
| **[`ban`](#ban)** | `.mod ban {User} {String\TimeSpan} {String}` | Bans user. | Configured **Admin role**, **Moderator role** or **Guild Administrator** |
| **[`mute`](#mute)** | `.mod mute {User} {String\TimeSpan} {String}` | Mutes user. | Configured **Admin role**, **Moderator role** or **Guild Administrator** |
| **[`mute mod`](#mutemod)** | `.mod mute mod {User} {String\TimeSpan} {String}` | Mutes user with moderator role. | Configured **Admin role** or **Guild Administrator** |
| **[`unmute`](#unmute)** | `.mod unmute {User}` | Unmutes user. | Configured **Admin role**, **Guild Administrator** or or **Manage Roles** permission |
| **[`show muted`](#showmuted)** | `.mod show muted` | Gets muted users. | Configured **Admin role**, **Guild Administrator** or or **Manage Roles** permission |
| **[`prefix`](#prefix)** | `.mod prefix {String}` | Sets command prefix. | Configured **Admin role** or **Guild Administrator** |
| **[`welcome`](#welcome)** | `.mod welcome {String}` | Sets welcome message which is displayed when user joins the guild. | Configured **Admin role** or **Guild Administrator** |
| **[`welcomepw`](#welcomepw)** | `.mod welcomepw {String}` | Sets welcome private message. | Configured **Admin role** or **Guild Administrator** |
| **[`goodbye`](#goodbye)** | `.mod goodbye {String}` | Sets goodbye message which is displayed when user leaves the guild. | Configured **Admin role** or **Guild Administrator** |
| **[`role`](#role)** | `.mod role` | Lists server roles. | Configured **Admin role**, **Guild Administrator** or **Manage Roles Permission** |
| **[`config`](#config)** | `.mod config {ConfigType?}` | Displays serveur configuration | Configured **Admin role**, **Guild Administrator** |
| **[`adminr`](#adminr)** | `.mod adminr {IRole}` | Sets admin role | Configured **Admin role**, **Guild Administrator** |
| **[`userr`](#userr)** | `.mod userr {IRole}` | Sets user role | Configured **Admin role**, **Guild Administrator** |
| **[`muter`](#muter)** | `.mod muter {IRole}` | Sets mute role | Configured **Admin role**, **Guild Administrator** |
| **[`mutemodr`](#mutemodr)** | `.mod mutemodr {IRole}` | Sets mute moderator role | Configured **Admin role**, **Guild Administrator** |
| **[`globalr`](#globalr)** | `.mod globalr {IRole}` | Sets global emote role | Configured **Admin role**, **Guild Administrator** |
| **[`waifur`](#waifur)** | `.mod waifur {IRole}` | Sets waifu role | Configured **Admin role**, **Guild Administrator** |
| **[`modr`](#modr)** | `.mod modr {IRole}` | Sets moderator role | Configured **Admin role**, **Guild Administrator** |
| **[`addur`](#addur)** | `.mod addur {IRole} {Level\Number}` | Sets level role | Configured **Admin role**, **Guild Administrator** |
| **[`selfrole`](#selfrole)** | `.mod selfrole` | Sets self governing role | Configured **Admin role**, **Guild Administrator** |
| **[`myland`](#myland)** | `.mod myland` | Sets my land role | Configured **Admin role**, **Guild Administrator** |
| **[`logch`](#logch)** | `.mod logch` | Sets log channel | Configured **Admin role**, **Guild Administrator** |
| **[`helloch`](#helloch)** | `.mod helloch` | Sets greetings channel | Configured **Admin role**, **Guild Administrator** |
| **[`notifch`](#notifch)** | `.mod notifch` | Sets notification channel | Configured **Admin role**, **Guild Administrator** |
| **[`raportch`](#raportch)** | `.mod raportch` | Sets report channel | Configured **Admin role**, **Guild Administrator** |
| **[`quizch`](#helloch)** | `.mod quizch` | Sets quiz channel | Configured **Admin role**, **Guild Administrator** |
| **[`todoch`](#todoch)** | `.mod todoch` | Sets todo channel | Configured **Admin role**, **Guild Administrator** |
| **[`nsfwch`](#nsfwch)** | `.mod nsfwch` | Sets NSFW ( Not safe for work ) channel | Configured **Admin role**, **Guild Administrator** |
| **[`tfightch`](#tfightch)** | `.mod tfightch` | Sets trash waifu fight channel | Configured **Admin role**, **Guild Administrator** |
| **[`tcmdch`](#tcmdch)** | `.mod tcmdch` | Sets trash waifu command channel | Configured **Admin role**, **Guild Administrator** |
| **[`tsafarich`](#tsafarich)** | `.mod tsafarich` | Sets waifu hunt channel | Configured **Admin role**, **Guild Administrator** |
| **[`marketch`](#marketch)** | `.mod marketch` | Sets waifu market channel | Configured **Admin role**, **Guild Administrator** |
| **[`duelch`](#duelch)** | `.mod duelch` | Sets waifu duel channel | Configured **Admin role**, **Guild Administrator** |
| **[`spawnch`](#spawnch)** | `.mod spawnch` | Sets waifu spawn channel | Configured **Admin role**, **Guild Administrator** |
| **[`fightch`](#fightch)** | `.mod fightch` | Sets waifu fight channel | Configured **Admin role**, **Guild Administrator** |
| **[`wcmdch`](#helloch)** | `.mod wcmdch` | Sets waifu command channel | Configured **Admin role**, **Guild Administrator** |
| **[`cmdch`](#cmdch)** | `.mod cmdch` | Sets command channel | Configured **Admin role**, **Guild Administrator** |
| **[`ignch`](#ignch)** | `.mod ignch` | Sets ignored channel ( means messages from given channel are not counted ) | Configured **Admin role**, **Guild Administrator** |
| **[`noexpch`](#noexpch)** | `.mod noexpch` | Sets no experience channel | Configured **Admin role**, **Guild Administrator** |
| **[`nosupch`](#nosupch)** | `.mod nosupch` | Toggles supervision option on given channel | Configured **Admin role**, **Guild Administrator** |
| **[`todo`](#todo)** | `.mod todo` | Adds todo message | Configured **Admin role**, **Moderator role**, **Guild Administrator** |
| **[`quote`](#quote)** | `.mod quote {MessageId\Number} {ChannelId\Number}` | Cites message and sends to given channel. | Configured **Admin role**, **Moderator role**, **Guild Administrator** |
| **[`tchaos`](#tchaos)** | `.mod tchaos` | Toggles chaos mode ( swaps nicknames of users ) | Configured **Admin role**, **Guild Administrator** |
| **[`tsup`](#tsup)** | `.mod tsup` | Toggles supervision | Configured **Admin role**, **Guild Administrator** |
| **[`check`](#check)** | `.mod check` | Checks whether the user is connected to Shinden. | Configured **Admin role**, **Guild Administrator** |
| **[`loteria`](#loteria)** | `.mod loteria` | Gives away cards to random user which reacted to sent message by bot. | Configured **Admin role**, **Guild Administrator** |
| **[`pary`](#pary)** | `.mod pary` | Generates pair of numbers. | Configured **Admin role**, **Guild Administrator** |
| **[`pozycja gracza`](#pozycjagracza)** | `.mod pozycja gracza` | Generates random number for given players. | Configured **Admin role**, **Guild Administrator** |
| **[`report`](#report)** | `.mod report` | Resolves report sent by user. | Configured **Admin role**, **Guild Administrator** |
| **[`help`](#help)** | `.mod help` | Lists the commands. | Configured **Admin role**, **Guild Administrator** |

### Fun ###
|                   Command                    | Example |             Description             | Permissions required |
| :---------------------------------------: | :-------: | :----------------------------------- | ----------- |
| **[`daily`](#daily)** | `.daily` | Receives daily SC coins. | Command channel if configured or **Guild Administrator** permission |
| **[`mute me`](#muteme)** | `.mute me` | Self mute |  |
| **[`hourly`](#hourly)** | `.hourly` | Receives hourly SC coins. | Command channel if configured or **Guild Administrator** permission |
| **[`beat`](#beat)** | `.beat {BetAmount\Number}` | Rolls a dice. | Command channel if configured or **Guild Administrator** permission |
| **[`set slot`](#setslot)** | `.set slot` | Sets mechine slot. | Command channel if configured or **Guild Administrator** permission |
| **[`slot machine`](#slotmachine)** | `.slot machine` | Runs slot machine. | Command channel if configured or **Guild Administrator** permission |
| **[`donatesc`](#donatesc)** | `.donatesc {IGuildUser} ` | Donates SC to another user. | Command channel if configured or **Guild Administrator** permission |
| **[`riddle`](#riddle)** | `.riddle {IGuildUser} ` | Generates random riddle. | Command channel if configured or **Guild Administrator** permission |
### Helper ###
|                   Command                    | Example |             Description             | Permissions required |
| :---------------------------------------: | :-------: | :----------------------------------- | ----------- |
| **[`help`](#help)** | `.help` | Displays the list of commands. | Command channel if configured or **Guild Administrator** permission |
| **[`whois`](#whois)** | `.whois` | Displays basic discord user information. | Command channel if configured or **Guild Administrator** permission |
| **[`ping`](#ping)** | `.ping` | Gets the estimated round-trip latency, in milliseconds, to the gateway server. | Command channel if configured or **Guild Administrator** permission |
| **[`serverinfo`](#serverinfo)** | `.serverinfo` | Gets basic discord server info. | Command channel if configured or **Guild Administrator** permission |
| **[`avatar`](#avatar)** | `.avatar` | Gets discord user avatar. | Command channel if configured or **Guild Administrator** permission |
| **[`info`](#info)** | `.info` | Gets basic bot information. | Command channel if configured or **Guild Administrator** permission |
| **[`report`](#report)** | `.report` | Reports the discord message. | User role if configured or **Guild Administrator** permission |

### Lands ###
|                   Command                    | Example |             Description             | Permissions required |
| :---------------------------------------: | :-------: | :----------------------------------- | ----------- |
| **[`population`](#population)** | `.population` | None |
| **[`land add`](#landadd)** | `.land add` | Add user to land. | None |
| **[`land remove`](#landremove)** | `.land remove` | Removed user from land. | None |

### Pocket Waifu ###
|                   Command                    | Example |             Description             | Permissions required |
| :---------------------------------------: | :-------: | :----------------------------------- | ----------- |
| **[`cards`](#cards)** | `.cards` | Displays cards owned by user. | Waifu command channel if configured or **Guild Administrator** permission |
| **[`items`](#items)** | `.items` | Displays item/items in user game deck. | Waifu command channel if configured or **Guild Administrator** permission |
| **[`card image`](#cardimage)** | `.card image` | Displays card image. | (Waifu) command channel if configured, level 40 or **Guild Administrator** permission |
| **[`card-`](#card-)** | `.card-` | Command channel if configured, level 40 or or **Guild Administrator** permission |
| **[`card`](#card)** | `.card` | Displays card details. | (Waifu) command channel if configured, level 40 or **Guild Administrator** permission |
| **[`pvp shop`](#pvpshop)** | `.pvp shop` | Opens Pvp shop which allows user to list the items or buy one. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`ac shop`](#acshop)** | `.ac shop` | Opens Activity shop which allows user to list the items or buy one. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`shop`](#shop)** | `.shop` | Opens Normal shop which allows user to list the items or buy one. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`use`](#use)** | `.use` | Consumes the item. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`booster`](#booster)** | `.booster` | Lists the card bundles  or consume card bundles (max 20). | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`reset`](#reset)** | `.reset` | Resets SSS card. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`update`](#update)** | `.update` | Updates the card details from Shinden platform. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`upgrade`](#upgrade)** | `.upgrade` | Upgrades the card. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`release`](#release)** | `.release` | Releases the card. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`destroy`](#destroy)** | `.destroy` | Destroys the card. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`chest`](#chest)** | `.chest` | Transfers experience from chest to the card. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`make chest`](#makechest)** | `.make chest` | Creates the experience chest. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`free card`](#freecard)** | `.free card` | Gets free daily card. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`market`](#market)** | `.market` | Goes to market. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`black market`](#blackmarket)** | `.black market` | Goes to black market. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`sacrifice`](#sacrifice)** | `.sacrifice` | Sacrifices the card. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`cage`](#cage)** | `.cage` | Opens the cage with card(s). | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`wadd`](#wadd)** | `.wadd` | Adds card/anime/manga/character to wishlist. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`wishlist view`](#wishlistview)** | `.wishlist view` | Toggles the hidden wishlist option. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`on wishlist`](#onwishlist)** | `.on wishlist {User?}` | Displays content of wishlist. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`wishlist`](#wishlist)** | `.wishlist` | Displays user wishlist. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`wishlistf`](#wishlistf)** | `.wishlistf` | Compares the wishlist between users. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`who wants`](#whowants)** | `.who wants {Number} {Boolean?}` | Looks up wishlists which contain given card. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`who wants anime`](#whowantsanime)** | `.who wants anime {Number} {Boolean?}` | Displays user wishlist. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`unleash`](#unleash)** | `.unleash` | Makes card tradeable. Costs TC. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`card limit`](#cardlimit)** | `.card limit` | Increases the card slot limit. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`site color`](#sitecolor)** | `.site color` | Changes main theme color of profile. Costs 500 TC. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`site foreground`](#siteforeground)** | `.site foreground` | Changes foreground image of profile. Costs 500 TC. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`site background`](#sitebackground)** | `.site background` | Changes background image of profile. Costs 2000 TC. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`site background position`](#sitebackgroundposition)** | `.site background position` | Changes background image position. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`site foreground position`](#siteforegroundposition)** | `.site foreground position` | Changes foreground image position. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`gallery`](#gallery)** | `.gallery` | Buys slots in gallery. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`crystal`](#crystal)** | `.crystal` | Converts necklace and flowers to crystal ball. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`tag`](#tag)** | `.tag {CardId\Number}...` | Adds tag to card. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`tag clean`](#tagclean)** | `.tag clean {CardId\Number}` | Clears card of tags. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`tag empty`](#tagempty)** | `.tag empty` | . | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`tag replace`](#tagreplace)** | `.tag replace` | Replaces tags in all cards. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`tag remove`](#tagremove)** | `.tag remove` | Removes tag from all cards. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`exchange conditions`](#)** | `.exchange conditions` | Sets exchange conditions. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`deck`](#deck)** | `.deck {CardId\Number}` | Changes deck status for given card. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`who`](#who)** | `.who {CharacterId\Number} {Boolean}` | . | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`favs`](#favs)** | `.favs {Boolean} {Boolean}` | . | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`which`](#which)** | `.which {SeriesId\Number} {Boolean}` | . | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`exchange`](#exchange)** | `.exchange {IGuildUser}` | Starts exchange session. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`exchange`](#exchange)** | `.exchange {IGuildUser}` | Starts exchange session. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`crafting`](#crafting)** | `.crafting` | Starts crafting session where user can create card from items. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`expedition status`](#expeditionstatus)** | `.expedition status` | Displays expedition progress. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`expedition end`](#expeditionend)** | `.expedition end` | Finishes expedition for given card. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`expedition`](#expedition)** | `.expedition {CardId\Number} {ExpeditionCardType}` | Starts expedition for given card. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`duel`](#duel)** | `.duel` | Performs dueling with another player. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`husbando`](#husbando)** | `.husbando {CardId\Number}` | Sets default favourite card. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`doante`](#doante)** | `.doante {CardId\Number}` | Converts card to angel or demon. Costs blood item. | (Waifu) command channel if configured or **Guild Administrator** permission |
| **[`cpf`](#cpf)** | `.cpf {IGuildUser}` | Shows pocket waifu profile. | (Waifu) command channel if configured or **Guild Administrator** permission |
### Profile ###
|                   Command                    | Example |             Description             | Permissions required |
| :---------------------------------------: | :-------: | :----------------------------------- | ----------- |
| **[`wallet`](#wallet)** | `.wallet` | Shows user wallet. | None |
| **[`sub`](#sub)** | `.sub` | Displays current user subscriptions. | Command channel if configured or **Guild Administrator** permission |
| **[`add role`](#addrole)** | `.add role {String}` | Grants user given role. | Command channel if configured or **Guild Administrator** permission |
| **[`remove role`](#removerole)** | `.remove role {String}` | Removes given role. | Command channel if configured or **Guild Administrator** permission |
| **[`wypisz role`](#wypiszrole)** | `.wypisz role` | Lists self managable roles. | None |
| **[`stats`](#stats)** | `.stats` | Displays user statistics. | None |
| **[`howmuchtolevelup`](#howmuchtolevelup)** | `.howmuchtolevelup` | Displays experience left for user to level up. | None |
| **[`top`](#top)** | `.top {TopType}` | Displays leaderboard by given criteria. | None |
| **[`waifu view`](#waifuview)** | `.waifu view` | Specifies whether to display waifu image on side panel. | Command channel if configured or **Guild Administrator** permission |
| **[`profile`](#profile)** | `.profile` | Displays user profile. | None |
| **[`quest`](#quest)** | `.quest` | Displays quest progress for given user. | Command channel if configured or **Guild Administrator** permission |
| **[`style`](#style)** | `.style` | Changes profile style. | Command channel if configured or **Guild Administrator** permission |
| **[`background`](#background)** | `.background` | Changes background. Costs 5000 SC/ 2500 TC. | Command channel if configured or **Guild Administrator** permission |
| **[`global`](#global)** | `.global` | Grants global emote role for a month. Costs 1000 TC. | Command channel if configured or **Guild Administrator** permission |
| **[`colour`](#colour)** | `.colour` | Changes user color. Costs 1000 TC. | Command channel if configured or **Guild Administrator** permission |

### Shinden ###
|                   Command                    | Example |             Description             | Permissions required |
| :---------------------------------------: | :-------: | :----------------------------------- | ----------- |
| **[`episodes`](#episodes)** | `.episodes` | Displays new epsiodes. |  |
| **[`anime`](#anime)** | `.anime {String}` | Displays information about given anime. |  |
| **[`manga`](#manga)** | `.manga {String}` | Displays information about given manga. |  |
| **[`character`](#character)** | `.character {String}` | Displays information about given character. |  |
| **[`site`](#site)** | `.site {IGuildUser?}` | Displays site statistics | |
| **[`connect`](#connect)** | `.connect {Url}` | Connects discord and shinden user. |  |

### Debug ###

All commands require developer permission.

|                   Command                    | Example |             Description             | Permissions required |
| :---------------------------------------: | :-------: | :----------------------------------- | ----------- |
| **[`poke`](#poke)** | `.poke` | Generates safari image. | None |
| **[`missingu`](#missingu)** | `.missingu` | Lists users which does not exist in database. | None |
| **[`blacklist`](#blacklist)** | `.blacklist {User}` | Adds user to blacklist. | None |
| **[`rmsg`](#rmsg)** | `.rmsg {GuildId/Number} {Number} {String}` | Sends response message to given channel. | None |
| **[`smsg`](#smsg)** | `.smsg {GuildId/Number} {Number} {String}` | Send message to given channel. | None |
| **[`semsg`](#semsg)** | `.semsg {GuildId/Number} {Number} {EMType} {String}` | Sends embed message to given channel. | None |
| **[`r2msg`](#r2msg)** | `.r2msg {GuildId/Number} {Number} {Number} {String}` | Add reaction to given message. | None |
| **[`cup`](#cup)** | `.cup` | Force updates cards. | None |
| **[`rozdajm`](#rozdajm)** | `.rozdajm {Number} {Number} {Number} {Number}` | Gives away cards periodically. | None |
| **[`rozdaj`](#rozdaj)** | `.rozdaj {Number} {Number} {Number}` | Gives away cards. | None |
| **[`tranc`](#tranc)** | `.tranc {Number} {Number}...` | Transfers card between users. | None |
| **[`rmcards`](#tranc)** | `.rmcards {Number}...` | Removes cards. | None |
| **[`level`](#level)** | `.level {User} {Level/Number}...` | Sets user level. | None |
| **[`mkick`](#mkick)** | `.mkick {User}...` | Kicks user from the server. | None |
| **[`mban`](#mban)** | `.mban {User}...` | Bans user from the server. | None |
| **[`restore`](#restore)** | `.restore {User}...` | Restores card back to given user. | None |
| **[`missingc`](#missingc)** | `.missingc {Boolean}` | Lists cards ids which are not visible by cards owners. | None |
| **[`cstats`](#cstats)** | `.cstats {CardId/Number}` | Generates card statistics. | None |
| **[`dusrcards`](#dusrcards)** | `.dusrcards {UserId/Number}` | Removes cards from given user. | None |
| **[`duser`](#duser)** | `.duser {UserId/Number} {IncludeCards/Boolean}` | Deletes user from database. | None |
| **[`tc duser`](#tcduser)** | `.tc duser` | Resets user gamedeck items, karma and CT. | None |
| **[`utitle`](#dusrcards)** | `.utitle {CardId/Number} {String}` | Updates card title. | None |
| **[`delq`](#delq)** | `.delq {QuizId/Number}` | Removes quiz. | None |
| **[`addq`](#addq)** | `.addq {UserId/Number}` | Adds new quiz. | None |
| **[`chpp`](#chpp)** | `.chpp {Count/Number} {Persist/Boolean}` | Sets char count per card bundle ratio. | None |
| **[`chpe`](#chpe)** | `.chpe {Count/Number} {Persist/Boolean}` | Adds Sets char count per experience ratio. | None |
| **[`turlban`](#turlban)** | `.turlban {Persist/Boolean}` | Enables/Disables supervisor which bans users when they spam channel with scam urls. | None |
| **[`tsafari`](#tsafari)** | `.tsafari {Persist/Boolean}` | Toggles safari mode. | None |
| **[`twevent`](#twevent)** | `.twevent {Persist/Boolean}` | Toggles waifu event. | None |
| **[`wevent`](#wevent)** | `.wevent {Persist/Boolean}` | Sets event ids. | None |
| **[`lvlbadge`](#lvlbadge)** | `.lvlbadge {IGuildUser}` | Generates user badge. | None |
| **[`devr`](#devr)** | `.devr` | Toggles developer mode. | None |
| **[`gitem`](#gitem)** | `.gitem {IGuildUser} {ItemType} {Count} {Quality}` | Generates item and assigns it to user. | None |
| **[`gcard`](#gcard)** | `.gcard {IGuildUser} {Number} {Rarity}` | Generates card and assigns it to user. | None |
| **[`ctou`](#ctou)** | `.ctou {CardId/Number} {Quality} {AttackPoints/Number} {DefencePoints/Number} {HealthPoints/Number}` | Converts card to ultimate. | None |
| **[`sc`](#sc)** | `.sc {IGuildUser} {Amount/Number}` | Changes amount of SC for given user. | None |
| **[`ac`](#ac)** | `.ac {IGuildUser} {Amount/Number}` | Changes amount of AC for given user. | None |
| **[`tc`](#tc)** | `.tc {IGuildUser} {Amount/Number}` | Changes amount of TC for given user. | None |
| **[`pc`](#pc)** | `.pc {IGuildUser} {Amount/Number}` | Changes amount of PC for given user. | None |
| **[`ct`](#ct)** | `.ct {IGuildUser} {Amount/Number}` | Changes amount of CT for given user. | None |
| **[`exp`](#exp)** | `.exp {IGuildUser} {Amount/Number}` | Changes amount of experience for given user. | None |
| **[`ost`](#ost)** | `.ost {IGuildUser} {Amount/Number}` | Changes amount of warnings for given user. | None |
| **[`sime`](#sime)** | `.sime {CardId/Number} {ExpeditionCardType} {Time/Number}` | Changes amount of experience for given user. | None |
| **[`kill`](#kill)** | `.kill` | Log outs the bot. | None |
| **[`update`](#update)** | `.update` | Log outs the bot and creates the file ./updateNow. | None |
| **[`rmconfig`](#rmconfig)** | `.rmconfig` | Displays rich message configuration. | None |
| **[`mrmconfig`](#mrmconfig)** | `.mrmconfig {RichMessageType} {ChannelId/Number} {RoleId/Number}` | Creates/Updates rich message config. | None |
| **[`ignore`](#ignore)** | `.ignore` | Adds server to blacklist. | None |
| **[`help`](#help)** | `.help` | Displays available command. | None |

## App Configuration ##
### Locale
|                   Name                    |         Type         |                 Default                 | Description                                                       |
| :---------------------------------------: | :------------------: | :-------------------------------------: | :---------------------------------------------------------------- |
| **[`TimeZone`](#TimeZone)** | `{String\|TimeZoneInfo}` | `Central European Standard Time` | Timezone which bot will use when displaying datetime information. |
| **[`Language`](#Language)** | `{String\|CultureInfo}` | `pl-PL` | Language which bot will use when displaying. |


### Database
|                   Name                    |         Type         |                 Default                 | Description                                                       |
| :---------------------------------------: | :------------------: | :-------------------------------------: | :---------------------------------------------------------------- |
| **[`Provider`](#Provider)**  | `{String}`  | `MySql` | The database engine to use. Currently supported MySql, Sqlite and SqlServer                              |
|  **[`Version`](#Version)** | `{String}` | `8.0.27` | The database engine version if supported.                                                 |
| **[`ConnectionString`](#ConnectionString)**  | `{String}` | `Server=localhost;Database=database;Uid=root;Pwd=password;` | Connection string. |

### Cache
|                   Name                    |         Type         |                 Default                 | Description                                                       |
| :---------------------------------------: | :------------------: | :-------------------------------------: | :---------------------------------------------------------------- |
|     **[`SlidingExpiration`](#SlidingExpiration)**     | `{String\|TimeSpan}` | `04:00:00` | how long a cache entry can be inactive (e.g. not accessed) before it will be removed. Default 4 hours.                                                 |
|       **[`AbsoluteExpirationRelativeToNow`](#AbsoluteExpirationRelativeToNow)** | `{String\|TimeSpan}` | `1.00:00:00` | an absolute expiration time, relative to now. Default one day. |


### Daemon
|                   Name                    |         Type         |                 Default                 | Description                                                       |
| :---------------------------------------: | :------------------: | :-------------------------------------: | :---------------------------------------------------------------- |
|  **[`CaptureMemoryUsageDueTime`](#CaptureMemoryUsageDueTime)** | `{String\|TimeSpan}` | `00:01:00` | The inital amount of time to delay before capturing memory usage. |
| **[`CaptureMemoryUsagePeriod`](#CaptureMemoryUsagePeriod)** | `{String\|TimeSpan}` | `00:01:00` | The time interval between invocations of memory usage. |
|  **[`ProfilePeriod`](#ProfilePeriod)**  | `{String\|TimeSpan}` | `00:00:05` | The inital amount of time to delay before time status check. |
| **[`ProfileDueTime`](#ProfileDueTime)** | `{String\|TimeSpan}` | `00:00:30` | The time interval between invocations of time status check. |
| **[`ChaosDueTime`](#ChaosDueTime)** | `{String\|TimeSpan}` | `01:00:00`  | The inital amount of time to delay before chaos mode reset. |
| **[`ChaosPeriod`](#ChaosPeriod)** | `{String\|TimeSpan}` | `01:00:00`  | The time interval between invocations of chaos mode reset. |
| **[`SessionDueTime`](#SessionDueTime)** | `{String\|TimeSpan}` | `00:00:05` | The inital amount of time to delay before session expiry check. |
| **[`SessionPeriod`](#SessionPeriod)** | `{String\|TimeSpan}`  |  `00:00:30` | The time interval between invocations of session expiry check. |
| **[`ModeratorDueTime`](#ModeratorDueTime)** | `{String\|TimeSpan}` | `00:01:00` | The inital amount of time to delay before penalty expiry check. |
| **[`ModeratorPeriod`](#ModeratorPeriod)** | `{String\|TimeSpan}` | `00:30:00` | The time interval between invocations of penalty expiry check. |
| **[`SupervisorDueTime`](#SupervisorDueTime)** | `{String\|TimeSpan}` | `00:05:00` | The inital amount of time to delay before supervisor subject reset. |
| **[`SupervisorPeriod`](#SupervisorPeriod)** | `{String\|TimeSpan}` | `00:05:00` | The time interval between invocations of supervisor subject reset. |

### Discord
|                   Name                    |         Type         |                 Default                 | Description                                                       |
| :---------------------------------------: | :------------------: | :-------------------------------------: | :---------------------------------------------------------------- |
| **[`Prefix`](#Prefix)** | `{String}` | `s` | Run commands on Discord only when they start with given prefix. |
| **[`BotToken`](#BotToken)**  |  `{String}` | `` | The discord bot token. |
| **[`MainGuild`](#MainGuild)**  | `{Number}` | `245931283031523330` | The main discord guild identifier which is used in Sanakan API. By default it is Shinden guild id. |
| **[`FloodSpamSupervisionEnabled`](#SupervisorEnabled)**  | `{Boolean}` | `true` | Enables flood/spam supervision |
| **[`RestartWhenDisconnected - Demonization`](#RestartWhenDisconnected)** | `{Boolean}` | `true` | Restarts the Discord socket client when it is disconnected. |
| **[`SafariEnabled`](#SafariEnabled)** | `{Boolean}`  | `true` | If enabled it allows generating cards from user messages. |
| **[`AllowedToDebug`](#AllowedToDebug)** | `{Array}` | `[]` | The list of Discord user identifiers which can access diagnostics. |
| **[`BlacklistedGuilds`](#BlacklistedGuilds)** | `{Array}` | `[]` | The list of Discord guild ( servers ) identifiers to blacklist. |
| **[`AlwaysDownloadUsers`](#AlwaysDownloadUsers)** | `{Boolean}` | `true` | Specifies whether or not all users should be downloaded as guilds come available. |
| **[`MessageCacheSize`](#MessageCacheSize)** | `{Number}` | `200` | The number of messages per channel that should be kept in cache. |
| **[`IconTheme`](#IconTheme)** | `{String}` | `Default` | The collection of emotes/emojis which bot will use. |

### Supervisor
|                   Name                    |         Type         |                 Default                 | Description                                                       |
| :---------------------------------------: | :------------------: | :-------------------------------------: | :---------------------------------------------------------------- |
| **[`MessagesLimit`](#MessagesLimit)** | `{Number}` | `12` | Run commands on Discord only when they start with given prefix. |
| **[`MessageLimit`](#MessageLimit)**  |  `{Number}` | `6` | The maximum amount of the same message user is allowed to send in a short time frame. |
| **[`MessageCommandLimit`](#MessageCommandLimit)**  | `{Number}` | `2` | Additional amount of messages user can send when message is command. |
| **[`MessageExpiry`](#MessageExpiry)**  | `{String\|TimeSpan}` | `00:02:00` | Specifies amount of time after which user message won't be analyzer nor be involved in decision mechanism. |
| **[`SameUsernameLimit`](#SameUsernameLimit)**  | `{Number}` | `3` | The maximum amount of users which can join guild with the same username. |
| **[`TimeIntervalBetweenMessages`](#TimeIntervalBetweenMessages)**  | `{String\|TimeSpan}` | `00:00:05` | Allowed time frame between sent messages by user. |
| **[`TimeIntervalBetweenUserGuildJoins`](#TimeIntervalBetweenUserGuildJoins)**  | `{String\|TimeSpan}` | `00:02:00` | Allowed time frame between events where user joins guild. |

### Experience
|                   Name                    |         Type         |                 Default                 | Description                                                       |
| :---------------------------------------: | :------------------: | :-------------------------------------: | :---------------------------------------------------------------- |
| **[`CharPerPacket`](#CharPerPacket)** | `{Number}` | 20000 | The amount of characters user has to type in Discord past which bundle of cards are received. |
| **[`CharPerPoint`](#CharPerPoint)**  | `{Number}` | 60 | The amount of characters per experience point ratio. |
| **[`MinPerMessage`](#MinPerMessage)**  | `{Number}` | 0.00005 | The minimum amount of experience points user receives per message. |
| **[`MaxPerMessage`](#MaxPerMessage)** | `{Number}` | 5 | The maximum amount of experience points user receives per message. |

### Imaging
|                   Name                    |         Type         |                 Default                 | Description                                                       |
| :---------------------------------------: | :------------------: | :-------------------------------------: | :---------------------------------------------------------------- |
| **[`CharacterImageWidth`](#CharacterImageWidth)** | `{Number}` | `475` | The width of the character picture. |
| **[`CharacterImageHeight`](#CharacterImageHeight)** | `{Number}` | `667` | The height of the character picture. |
| **[`StatsImageWidth`](#StatsImageHeight)** | `{Number}` | `120` | The width of the stats picture. |
| **[`StatsImageHeight`](#StatsImageHeight)** | `{Number}` | `40` | The height of the stats picture. |

### Shinden API
|                   Name                    |         Type         |                 Default                 | Description                                                       |
| :---------------------------------------: | :------------------: | :-------------------------------------: | :---------------------------------------------------------------- |
| **[`BaseUrl`](#BaseUrl)** | `{String}` | `` | The Shinden API base url. |
| **[`Token`](#Token)** | `{String}` | `` | The API token which is supplied in certain requests sent to Shinden API. |
| **[`UserAgent`](#UserAgent)**  | `{String}` | `` |  Lets servers and network peers identify the application, operating system, vendor, and/or version of the request. |
| **[`Marmolade`](#Marmolade)**  | `{Boolean}` | `` | Mysterious HTTP header used for Shinden API |
| **[`SessionExpiry`](#SessionExpiry)**  | `{String\|TimeSpan}` | `01:30:00` | The time interval after which session expires. It is used when authorized as a user. |

### Sanakan API
|                   Name                    |         Type         |                 Default                 | Description                                                       |
| :---------------------------------------: | :------------------: | :-------------------------------------: | :---------------------------------------------------------------- |
| **[`Jwt`](#Jwt)** | `{Object}` | `` | The JWT token configuration. |
| **[`ApiKeys`](#ApiKeys)**  | `{Array}` | `` |  The list of 3rd party API Keys which will be provided with JWT. |
| **[`TokenExpiry`](#TokenExpiry)**  | `{String\|TimeSpan}` | `1.00:00:00` |  The time span after JWT expires. |
| **[`UserWithTokenExpiry`](#UserWithTokenExpiry)**  | `{String\|TimeSpan}` | `00:30:00` |  The time span after JWT with user expires. |

### JWT
|                   Name                    |         Type         |                 Default                 | Description                                                       |
| :---------------------------------------: | :------------------: | :-------------------------------------: | :---------------------------------------------------------------- |
| **[`IssuerSigningKey`](#IssuerSigningKey)** | `{String}` | `` | The signing key. |
| **[`Issuer`](#Jwt)** | `{String}` | `` | The JWT issuer. |
| **[`TokenExpiry`](#TokenExpiry)**  | `{String\|TimeSpan}` | `1.00:00:00` |  The time span after JWT expires. |
| **[`UserWithTokenExpiry`](#UserWithTokenExpiry)**  | `{String\|TimeSpan}` | `00:30:00` |  The time span after JWT with user details expires. |

### Rich Message
|                   Name                    |         Type         |                 Default                 | Description                                                       |
| :---------------------------------------: | :------------------: | :-------------------------------------: | :---------------------------------------------------------------- |
| **[`RoleId`](#RoleId)** | `{Number}` | `` | The JWT token configuration. |
| **[`GuildId`](#GuildId)**  | `{Number}` | `` |  The list of 3rd party API Keys which will be provided with JWT. |
| **[`ChannelId`](#ChannelId)**  | `{Number}` | `` |  The list of 3rd party API Keys which will be provided with JWT. |
| **[`Type`](#Type)**  | `{String}` | `` |  The list of 3rd party API Keys which will be provided with JWT. |
