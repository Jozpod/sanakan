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

|                   Name                    |         Type         |                 Default                 | Description                                                       |
| :---------------------------------------: | :------------------: | :-------------------------------------: | :---------------------------------------------------------------- |
|  **[`ConnectionString`](#ConnectionString)**  |  `{String}`  |                 `Server=localhost;Database=database;Uid=root;Pwd=password;`                  | MySql connection string.                                 |
|     **[`Prefix`](#Prefix)**     | `{String}` | . | Options for Sass.                                                 |
|       **[`BotToken`](#BotToken)**       |     `{String}`      |                       | The discord bot token.                       |
|  **[`Supervision`](#Supervision)**  | `{Boolean}` |               `true`               | Prepends/Appends `Sass`/`SCSS` code before the actual entry file. |
| **[`Demonization`](#Demonization)** |     `{Boolean}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`SafariEnabled`](#SafariEnabled)** |     `{Boolean}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`Shinden`](#Shinden)** |     `{Object}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`CharPerPacket`](#CharPerPacket)** |     `{Number}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`Experience`](#Experience)** |     `{Boolean}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`Demonization`](#webpackimporter)** |     `{Boolean}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |
| **[`Demonization`](#webpackimporter)** |     `{Boolean}`      |                 `true`                  | Enables/Disables the default Webpack importer.                    |

### Running ###

Run `Run.sh` script from `src` directory.

### Runtime configuration ###

Invite bot to your server and setup it with `.mod` commands, `.mod h` will list all moderation commands.
