# cachet-monitor
Cachet automated monitoring made easy

#Build Linux
Make your active directory the project root (the folder with the .csproj file).
Run 
```
dotnet publish -c release -p:PublishSingleFile=true --self-contained --runtime linux-x64
```