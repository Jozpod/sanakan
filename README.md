# Sanakan #

Discord bot which allows 

## Tech Stack

built with [Discord.NET](https://github.com/RogueException/Discord.Net).

## Requirements ##

- .NET 5.0

- MySql Database Version 8.0.27

- Shinden API Key

- Discord Bot token

## Getting Started ##

```console
dotnet run
```

### Compilation ###

1. Go to `src` directory
2. Run `make full-build`

## Options
### Locale
|                   Name                    |         Type         |                 Default                 | Description                                                       |
| :---------------------------------------: | :------------------: | :-------------------------------------: | :---------------------------------------------------------------- |
|  **[`TimeZone`](#TimeZone)**  |  `{String}`  |                 `Central European Standard Time`                  | Timezone which bot will use when displaying datetime information.                                 |
|     **[`Language`](#Language)**     | `{String}` | `pl-PL` | Language which bot will use when displaying .                                                 |


### Database
|                   Name                    |         Type         |                 Default                 | Description                                                       |
| :---------------------------------------: | :------------------: | :-------------------------------------: | :---------------------------------------------------------------- |
|  **[`Provider`](#Provider)**  |  `{String}`  |                 `MySql`                  | The database engine to use. Currently supported MySql, Sqlite and SqlServer                              |
|     **[`Version`](#Version)**     | `{String}` | `pl-PL` | The database engine version if supported.                                                 |
|  **[`ConnectionString`](#ConnectionString)**  |  `{String}`  |                 `Server=localhost;Database=database;Uid=root;Pwd=password;`                  | connection string.                                 |

### Cache
|                   Name                    |         Type         |                 Default                 | Description                                                       |
| :---------------------------------------: | :------------------: | :-------------------------------------: | :---------------------------------------------------------------- |
|     **[`Prefix`](#Prefix)**     | `{String}` | . | Options for Sass.                                                 |
|       **[`BotToken`](#BotToken)**       |     `{String}`      |                       | The discord bot token.                       |
|  **[`Supervision`](#Supervision)**  | `{Boolean}` |               `true`               | Prepends/Appends `Sass`/`SCSS` code before the actual entry file. |
| **[`Demonization`](#Demonization)** |     `{Boolean}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`SafariEnabled`](#SafariEnabled)** |     `{Boolean}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`Shinden`](#Shinden)** |     `{Object}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`CharPerPacket`](#CharPerPacket)** |     `{Number}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`Experience`](#Experience)** |     `{Object}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`Demonization`](#Demonization)** |     `{Boolean}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`Demonization`](#webpackimporter)** |     `{Boolean}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
### Daemon
|                   Name                    |         Type         |                 Default                 | Description                                                       |
| :---------------------------------------: | :------------------: | :-------------------------------------: | :---------------------------------------------------------------- |
|     **[`Prefix`](#Prefix)**     | `{String}` | . | Options for Sass.                                                 |
|       **[`BotToken`](#BotToken)**       |     `{String}`      |                       | The discord bot token.                       |
|  **[`Supervision`](#Supervision)**  | `{Boolean}` |               `true`               | Prepends/Appends `Sass`/`SCSS` code before the actual entry file. |
| **[`Demonization`](#Demonization)** |     `{Boolean}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`SafariEnabled`](#SafariEnabled)** |     `{Boolean}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`Shinden`](#Shinden)** |     `{Object}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`CharPerPacket`](#CharPerPacket)** |     `{Number}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`Experience`](#Experience)** |     `{Object}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`Demonization`](#Demonization)** |     `{Boolean}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`Demonization`](#webpackimporter)** |     `{Boolean}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
### Discord
|                   Name                    |         Type         |                 Default                 | Description                                                       |
| :---------------------------------------: | :------------------: | :-------------------------------------: | :---------------------------------------------------------------- |
|     **[`Prefix`](#Prefix)**     | `{String}` | . | Options for Sass.                                                 |
|       **[`BotToken`](#BotToken)**       |     `{String}`      |                       | The discord bot token.                       |
|  **[`Supervision`](#Supervision)**  | `{Boolean}` |               `true`               | Prepends/Appends `Sass`/`SCSS` code before the actual entry file. |
| **[`Demonization`](#Demonization)** |     `{Boolean}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`SafariEnabled`](#SafariEnabled)** |     `{Boolean}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`Shinden`](#Shinden)** |     `{Object}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`CharPerPacket`](#CharPerPacket)** |     `{Number}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`Experience`](#Experience)** |     `{Object}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`Demonization`](#Demonization)** |     `{Boolean}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`Demonization`](#webpackimporter)** |     `{Boolean}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |

### Experience
|                   Name                    |         Type         |                 Default                 | Description                                                       |
| :---------------------------------------: | :------------------: | :-------------------------------------: | :---------------------------------------------------------------- |
|     **[`Prefix`](#Prefix)**     | `{String}` | . | Options for Sass.                                                 |
|       **[`BotToken`](#BotToken)**       |     `{String}`      |                       | The discord bot token.                       |
|  **[`Supervision`](#Supervision)**  | `{Boolean}` |               `true`               | Prepends/Appends `Sass`/`SCSS` code before the actual entry file. |
| **[`Demonization`](#Demonization)** |     `{Boolean}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`SafariEnabled`](#SafariEnabled)** |     `{Boolean}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`Shinden`](#Shinden)** |     `{Object}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`CharPerPacket`](#CharPerPacket)** |     `{Number}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`Experience`](#Experience)** |     `{Object}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`Demonization`](#Demonization)** |     `{Boolean}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`Demonization`](#webpackimporter)** |     `{Boolean}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |

### Running ###

Run `Run.sh` script from `src` directory.

### Runtime configuration ###

Invite bot to your server and setup it with `.mod` commands, `.mod h` will list all moderation commands.
