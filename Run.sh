#!/bin/bash
rval=1
while ((rval == 1)); do
    dotnet publish Web/Web.csproj --configuration Release --runtime linux-x64 /p:PublishSingleFile=true /p:DebugType=None /p:DebugSymbols=false --output ./publish 
    rval=$?
    echo "$rval"
    if test -f "./publish/updateNow";
    then
        rm ./publish/updateNow
        .publish/Sanakan.Web
        rval=1
    fi
sleep 1
done