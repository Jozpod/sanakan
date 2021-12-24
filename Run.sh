#!/bin/bash
rval=1
while ((rval == 1)); do
    dotnet publish Web/Web.csproj --configuration Release --runtime linux-x64 --output ./publish 
    rval=$?
    echo "$rval"
    if test -f "./updateNow";
    then
        rm ./updateNow
        dotnet ./publish/Sanakan.Web.dll
        rval=1
    fi
sleep 1
done