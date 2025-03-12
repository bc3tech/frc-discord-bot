namespace FRCColors;

using Common.Extensions;

using FRCColors.Models;

using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;

public sealed class Client(IHttpClientFactory clientFactory)
{
    private readonly HttpClient _client = clientFactory.CreateClient();

    public Task<(Color? primaryColor, Color? secondaryColor)> GetColorsForTeamAsync(string teamKey, CancellationToken cancellationToken) => GetColorsForTeamAsync(teamKey.ToTeamNumber(), cancellationToken);

    public async Task<(Color? primaryColor, Color? secondaryColor)> GetColorsForTeamAsync(ushort? teamNumber, CancellationToken cancellationToken)
    {
        if (teamNumber.HasValue)
        {
            var resp = await _client.GetFromJsonAsync<SingleTeamColors>($"https://api.frc-colors.com/v1/team/{teamNumber.Value}", cancellationToken: cancellationToken).ConfigureAwait(false);
            (bool flowControl, (Color? primaryColor, Color? secondaryColor) value) = ParseColors(resp);
            if (!flowControl)
            {
                return value;
            }
        }

        return (null, null);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD002:Avoid problematic synchronous waits", Justification = "Should be good as we're using `RunSynchronously()`")]
    public (Color? primaryColor, Color? secondaryColor) GetColorsForTeam(ushort? teamNumber, CancellationToken cancellationToken)
    {
        if (teamNumber.HasValue)
        {
            var resp = _client.GetFromJson<SingleTeamColors>(new($"https://api.frc-colors.com/v1/team/{teamNumber.Value}"), cancellationToken);
            (bool flowControl, (Color? primaryColor, Color? secondaryColor) value) = ParseColors(resp);
            if (!flowControl)
            {
                return value;
            }
        }

        return (null, null);
    }

    private static (bool flowControl, (Color? primaryColor, Color? secondaryColor) value) ParseColors(SingleTeamColors? resp)
    {
        if (resp?.Colors is not null)
        {
            Color? primaryColor = string.IsNullOrWhiteSpace(resp.Colors.PrimaryHex) ? null : Color.FromArgb(255, int.Parse(resp.Colors.PrimaryHex[1..3], System.Globalization.NumberStyles.HexNumber), int.Parse(resp.Colors.PrimaryHex[3..5], System.Globalization.NumberStyles.HexNumber), int.Parse(resp.Colors.PrimaryHex[5..7], System.Globalization.NumberStyles.HexNumber));
            Color? secondaryColor = string.IsNullOrWhiteSpace(resp.Colors.SecondaryHex) ? null : Color.FromArgb(255, int.Parse(resp.Colors.SecondaryHex[1..3], System.Globalization.NumberStyles.HexNumber), int.Parse(resp.Colors.SecondaryHex[3..5], System.Globalization.NumberStyles.HexNumber), int.Parse(resp.Colors.SecondaryHex[5..7], System.Globalization.NumberStyles.HexNumber));

            return (flowControl: false, value: (primaryColor, secondaryColor));
        }

        return (flowControl: true, value: default);
    }
}
