# cachet-monitor

**Due to heavy use of the incident updates feature, only version 2.4 and up of Cachet is supported**

Cachet automated monitoring made easy.

cachet-monitor comes with an integrated CLI configuration editor for easy management.

Built in .NET Core for multiplatform support.

# Why?
In my opinion none of the alternatives offered adequate configuration possibilites and were annoying to set up.

Differences from the alternatives:
* Support for choosing whether to fail on SSL.
* Separate actions per failed expectation.
* Specify all details about an action such as message and component status.
* Link incidents to component statuses.
* (Experimental) Add multiple entries of the same host with different parameters (for example one actionset for http code 301, but another for http code 500).
* Other things I couldn't think of right now.

# Deploying with Docker-Compose
You can deploy cachet-monitor with docker-compose. There are unfortunately no premade images, but the images are simple to make yourself.

```bash
#Clone source repository
git clone https://github.com/henrikx/cachet-monitor.git
#Enter cloned repository containing the source code for build, the Dockerfile and the docker-compose.yml file.
cd cachet-monitor
#Edit the docker-compose.yml file with values such as data location and restart values.
#New installations will need a config file. Enter the configuration CLI like so:
docker-compose run --rm cachet-monitor --config
#When you've saved and exited your configuration, you can run the service
docker-compose up -d
```
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

If you use actions `update_component` and `create_incident` at the same time and you specify `component_id` in `create_incident` as the same id as in  `update_incident` then  `update_incident` will be ignored completely. `update_component` is mainly meant for updating components that might be affected by the current host's status as a side effect or if you don't want to create an incident.

# Build Linux
Binaries for Linux are available in releases.

Make your active directory the project root (the folder with the .csproj file).
Run 
```
dotnet publish -c release -p:PublishSingleFile=true --self-contained --runtime linux-x64
```
