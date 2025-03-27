namespace FunctionApp.Storage.Caching.Interfaces;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using TheBlueAlliance.Model;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "EA0004:Make types declared in an executable internal", Justification = "No")]
public interface ITeamCache
{
    Team this[string teamKey] { get; }
    Team this[ushort teamNumber] { get; }

    IReadOnlyDictionary<string, Team> AllTeams { get; }

    ValueTask InitializeAsync(CancellationToken cancellationToken);
}