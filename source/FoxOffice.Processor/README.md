Create `local.settings.json` file in the project directory to execute the function application on local machine.

Example

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet"
  },
  "ConnectionStrings": {
    "Storage": "UseDevelopmentStorage=true"
  },
  "Messaging": {
    "Storage": {
      "QueueName": "messages"
    }
  },
  "Domain": {
    "Storage": {
      "EventStoreTableName": "FoxOfficeEventStore"
    }
  },
  "ReadModel": {
    "CosmosDb": {
      "Endpoint": "https://localhost:8081",
      "AuthKey": "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
      "DatabaseId": "FoxOfficeDatabase",
      "CollectionId": "FoxOfficeCollection"
    }
  }
}
```
