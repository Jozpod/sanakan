all-update: backup update full-build

all-update-debug: update restore build-debug

full-build: restore build

run-debug:
	dotnet --project Web/Web.csproj --configuration Debug

build-debug:
	dotnet build -configuration Debug

run:
	dotnet run --project Web/Web.csproj --configuration Release --verbosity quiet

build:
	dotnet build --configuration Release

test:
	dotnet test Tests/Common.Tests/Common.Tests.csproj  --no-build --verbosity minimal --configuration Release
	dotnet test Tests/DAL.MySql.Migrator.Tests/DAL.MySql.Migrator.Tests.csproj --no-build --verbosity minimal --configuration Release
	dotnet test Tests/DAL.MySql.Schema.Tests/DAL.MySql.Schema.Tests.csproj --no-build --verbosity minimal --configuration Release
	dotnet test Tests/Web.Tests/Web.Tests.csproj --no-build --verbosity minimal --configuration Release /p:Exclude=\"[*]Sanakan.Web.Models*\"
	dotnet test Tests/DAL.Tests/DAL.Tests.csproj  --no-build --verbosity minimal --configuration Release
	dotnet test Tests/DiscordBot.Tests/DiscordBot.Tests.csproj --no-build --verbosity minimal --configuration Release
	dotnet test Tests/ShindenApi.Tests/ShindenApi.Tests.csproj --no-build --verbosity minimal --configuration Release /p:Exclude=\"[*]Sanakan.ShindenApi.Models*,[*]Sanakan.ShindenApi.Converters*\"
	dotnet test Tests/DiscordBot.Session.Tests/DiscordBot.Session.Tests.csproj --no-build --verbosity minimal --configuration Release
	dotnet test Tests/DiscordBot.Supervisor.Tests/DiscordBot.Supervisor.Tests.csproj --no-build --verbosity minimal --configuration Release
	dotnet test Tests/DiscordBot.Services.Tests/DiscordBot.Services.Tests.csproj  --no-build --verbosity minimal --configuration Release /p:Exclude=\"[*]Sanakan.DiscordBot.Services.Models*\"
	dotnet test Tests/Daemon.Tests/Daemon.Tests.csproj  --no-build --verbosity minimal --configuration Release
	dotnet test Tests/Game.Tests/Game.Tests.csproj --no-build --verbosity minimal --configuration Release /p:Exclude=\"[*]Sanakan.Game.Models*\"
	dotnet test Tests/TaskQueue.Tests/TaskQueue.Tests.csproj --no-build --verbosity minimal --configuration Release /p:Exclude=\"[*]Sanakan.TaskQueue.Messages*\"

publish-test-coverage:
	codecov -f "./Tests/TestResults/DAL.Tests.coverage.xml" `
	"./Tests/TestResults/Web.Tests.coverage.xml" `
	"./Tests/TestResults/DiscordBot.Tests.coverage.xml" `
	"./Tests/TestResults/ShindenApi.Tests.coverage.xml" `
	"./Tests/TestResults/DiscordBot.Session.Tests.coverage.xml" `
	"./Tests/TestResults/DiscordBot.Supervisor.Tests.coverage.xml" `
	"./Tests/TestResults/DiscordBot.Services.Tests.coverage.xml" `
	"./Tests/TestResults/Daemon.Tests.coverage.xml" `
	"./Tests/TestResults/Game.Tests.coverage.xml" `
	"./Tests/TestResults/TaskQueue.Tests.coverage.xml" `
	-t %SANAKAN_CODECOV% --b main

restore:
	dotnet restore

cleanup:
	dotnet clean

backup:
	cp -r ./bin/Release/ ./bin/Backup/

update:
	git pull https://github.com/MrZnake/sanakan.git
