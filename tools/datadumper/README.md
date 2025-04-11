# TBA Data Dumper

This tool is used to dump data from The Blue Alliance API into an Azure Blob Storage account. The intent, then, is for this data to be indexed and consumed by any other services that need it. This is a one-time dump of data, not a live feed. The data is dumped in JSON format, creating files like `frc####` for each FRC Team, `YYYY{event code}` for each Event, and `YYYY{event code}_{comp level}{set}m{match}` for each match of an event.

## Configuration

The app requires that `TbaApiKey` be defined in appsettings, secrets, or environment variables. This is the API key for The Blue Alliance. The app also requires that `StorageAccountUri` be defined in appsettings, secrets, or environment variables. This is the Azure Blob Storage endpoint for the the target account. Authentication utilizes Managed Identity, so the user running the tool must have Blob Storage Data Contributor access to the target account.

## Execution

Simply run the built executable.
