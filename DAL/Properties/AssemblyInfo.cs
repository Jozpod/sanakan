using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: ExcludeFromCodeCoverage]
[assembly: InternalsVisibleTo("Sanakan.DAL.Tests")]
[assembly: InternalsVisibleTo("Sanakan.Daemon.Tests")]
[assembly: InternalsVisibleTo("Sanakan.DAL.MySql.Migrator")]
[assembly: InternalsVisibleTo("Sanakan.DAL.MySql.Schema")]
[assembly: InternalsVisibleTo("Sanakan.DiscordBot.Integration.Tests")]