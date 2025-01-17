namespace Common.Tba.Notifications;

using Common.Tba.Api.Models;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Reduce boilerplate by just matching exact JSON body")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Reduce boilerplate by just matching exact JSON body")]
public record AllianceSelection(string event_key, string? team_key, string event_name) : IRequireCombinedSerialization<Event>
{
    [DataMember]
    public Event? Event { get; set; }

    [JsonIgnore, JsonPropertyName("event")]
    public Event? Model { get => this.Event; private set => this.Event = value; }
}
