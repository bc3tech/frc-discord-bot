namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using System;
using System.Collections.Generic;
using System.Text.Json;

class ScheduleDetail : IEmbedCreator<JsonElement>
{
    public IAsyncEnumerable<ResponseEmbedding> CreateAsync(JsonElement input, ushort? highlightTeam = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
