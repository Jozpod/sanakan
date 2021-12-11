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
	dotnet build -configuration Release

restore:
	dotnet restore

cleanup:
	dotnet clean

backup:
	cp -r ./bin/Release/ ./bin/Backup/

update:
	git pull https://github.com/MrZnake/sanakan.git
