# cachet-monitor
Cachet automated monitoring made easy.

cachet-monitor comes with an integrated CLI configuration editor for easy management.

Built in .NET Core for multiplatform support.

# Why?
In my opinion none of the alternatives offered adequate configuration possibilites and were annoying to set up.

# Usage
`cachet-monitor` will run in daemon mode. `--config` should be ran if you do not already have a `./Config.json` file. This is considered regular operation.

`cachet-monitor --config`  will run in interactive configuration mode. No monitoring or API calls will be performed. Configuration will be saved to `./Config.json`

# Configuration
Configuration is stored in `Config.json` in the current working directory.
cachet-monitor supports many configuration possibilites. You can set multiple actions and checks with separate actions per check for each host.


**Monitoring types**
`http` HTTP monitoring

`ping` **NOT IMPLEMENTED YET!** Ping monitoring


**Possible actions**
`update_component` Sets component status to configured values

`create_incident` Creates an incident with configured values


**Remarks**
If you use actions `update_component` and `create_incident` at the same time and you specify `component_id` in `create_incident` as the same id as in  `update_incident` then  `update_incident` will be ignored completely. `update_component` is mainly meant for updating components that might be affected by the current host's status as a side effect.

# Build Linux
Make your active directory the project root (the folder with the .csproj file).
Run 
```
dotnet publish -c release -p:PublishSingleFile=true --self-contained --runtime linux-x64
```