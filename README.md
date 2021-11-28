<h1><img width="200px" alt="Sanakan" src="logo.webp" /></h1>

# Sanakan #

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

> ℹ️ Make sure to customize settings to your needs. By default the app will use Mysql server.

```console
dotnet run
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

## Commands ##

### Moderation ###
|                   Command                    | Example |             Description             | Permissions requried |
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
|  **[`ProfilePeriod`](#ProfilePeriod)**  | `{String\|TimeSpan}` | `00:00:05` | The inital amount of time to delay before <>. |
| **[`ProfileDueTime`](#ProfileDueTime)** | `{String\|TimeSpan}` | `00:00:30` | The time interval between invocations of <>. |
| **[`ChaosDueTime`](#ChaosDueTime)** | `{String\|TimeSpan}` | `01:00:00`  | The inital amount of time to delay before <>. |
| **[`ChaosPeriod`](#ChaosPeriod)** | `{String\|TimeSpan}` | `01:00:00`  | The time interval between invocations of <>. |
| **[`SessionDueTime`](#SessionDueTime)** | `{String\|TimeSpan}` | `00:00:05` | The inital amount of time to delay before <>. |
| **[`SessionPeriod`](#SessionPeriod)** | `{String\|TimeSpan}`  |  `00:00:30` | The time interval between invocations of <>. |
| **[`ModeratorDueTime`](#ModeratorDueTime)** | `{String\|TimeSpan}` | `00:01:00` | The inital amount of time to delay before <>. |
| **[`ModeratorPeriod`](#ModeratorPeriod)** | `{String\|TimeSpan}` | `00:30:00` | The time interval between invocations of <>. |
| **[`SupervisorDueTime`](#SupervisorDueTime)** | `{String\|TimeSpan}` | `00:05:00` | The inital amount of time to delay before <>. |
| **[`SupervisorPeriod`](#SupervisorPeriod)** | `{String\|TimeSpan}` | `00:05:00` | The time interval between invocations of <>. |

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
| **[`MessageCacheSize`](#MessageCacheSize)** | `{Number}` | `200` | The number of messages per channel that should be kept in cache.. |

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