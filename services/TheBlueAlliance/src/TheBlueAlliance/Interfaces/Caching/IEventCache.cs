namespace TheBlueAlliance.Interfaces.Caching;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using TheBlueAlliance.Model;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "EA0004:Make types declared in an executable internal", Justification = "No")]
public interface IEventCache
{
    Event this[string eventKey] { get; }

    IReadOnlyDictionary<string, Event> AllEvents { get; }

    ValueTask InitializeAsync(CancellationToken cancellationToken);
}