Agent must be installed on the machine in on premise network.

## How to publish agent 
* Run `dotnet publish --configuration release`
* Copy content of the `\Armadillo.Agent\bin\release\netcoreapp2.1\publish` to the target machine
* Configure CosmosDB access by setting endpoint URI and key. To do that, run the following commands
    ```
    dotnet user-secrets set "CosmosDB:EndpointUri" "https://subcasemonitor.documents.azure.com:443/" --id 28e6f711-a4c4-4cef-9e37-50ebfee35f91
    dotnet user-secrets set "CosmosDB:PrimaryKey" "<access key>" --id 28e6f711-a4c4-4cef-9e37-50ebfee35f91
    ```
* Run `dotnet Armadillo.Agent.dll` 