<h1><img width="200px" alt="Sanakan" src="logo.webp" /></h1>

# Sanakan #



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

## Options
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
|  **[`FloodSpamSupervisionEnabled`](#SupervisorEnabled)**  | `{Boolean}` | `true` | Enables flood/spam supervision |
| **[`RestartWhenDisconnected - Demonization`](#RestartWhenDisconnected)** | `{Boolean}` | `true` | Restarts the Discord socket client when it is disconnected. |
| **[`SafariEnabled`](#SafariEnabled)** | `{Boolean}`  | `true` | If enabled it allows generating cards from user messages. |
| **[`AllowedToDebug`](#AllowedToDebug)** | `{Array}` | `[]` | The list of Discord user identifiers which can access diagnostics. |
| **[`BlacklistedGuilds`](#BlacklistedGuilds)** | `{Array}` | `[]` | The list of Discord guild ( servers ) identifiers to blacklist. |

### Experience
|                   Name                    |         Type         |                 Default                 | Description                                                       |
| :---------------------------------------: | :------------------: | :-------------------------------------: | :---------------------------------------------------------------- |
| **[`CharPerPacket`](#CharPerPacket)** | `{Number}` | 20000 | The amount of characters user has to type in Discord past which bundle of cards are received. |
| **[`CharPerPoint`](#CharPerPoint)**  | `{Number}` | 60 | The amount of characters per experience point ratio. |
| **[`MinPerMessage`](#MinPerMessage)**  | `{Number}` | 0.00005 | The minimum amount of experience points user receives per message. |
| **[`MaxPerMessage`](#MaxPerMessage)** | `{Number}` | 5 | The maximum amount of experience points user receives per message. |

### Shinden API
|                   Name                    |         Type         |                 Default                 | Description                                                       |
| :---------------------------------------: | :------------------: | :-------------------------------------: | :---------------------------------------------------------------- |
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

### Rich Message
|                   Name                    |         Type         |                 Default                 | Description                                                       |
| :---------------------------------------: | :------------------: | :-------------------------------------: | :---------------------------------------------------------------- |
| **[`RoleId`](#RoleId)** | `{Number}` | `` | The JWT token configuration. |
| **[`GuildId`](#GuildId)**  | `{Number}` | `` |  The list of 3rd party API Keys which will be provided with JWT. |
| **[`ChannelId`](#ChannelId)**  | `{Number}` | `` |  The list of 3rd party API Keys which will be provided with JWT. |
| **[`Type`](#Type)**  | `{String}` | `` |  The list of 3rd party API Keys which will be provided with JWT. |

### Runtime configuration ###

Invite bot to your server and setup it with `.mod` commands, `.mod h` will list all moderation commands.
