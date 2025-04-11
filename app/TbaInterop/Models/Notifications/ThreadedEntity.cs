namespace FunctionApp.TbaInterop.Models.Notifications;

using System.Text.Json.Serialization;

internal record ThreadedEntity([property: JsonIgnore] string PartitionKey, [property: JsonIgnore] string RowKey);