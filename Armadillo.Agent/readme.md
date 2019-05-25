Agent must be installed on the machine in on premise network.

## How to publish agent 
* Run `dotnet publish --configuration release`
* Copy content of the `\Armadillo.Agent\bin\release\netcoreapp2.1\publish` to the target machine
* Run `dotnet Armadillo.Agent.dll` there