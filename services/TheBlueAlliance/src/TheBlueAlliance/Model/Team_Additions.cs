namespace TheBlueAlliance.Model;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial record Team
{
    public string GetLabel(bool includeNumber = true, bool includeName = true, bool includeLocation = true)
    {
        var details = new StringBuilder();
        if (includeNumber)
        {
            details.Append($"{this.TeamNumber}");
        }

        if (includeName && !string.IsNullOrEmpty(this.Nickname))
        {
            if (details.Length > 0)
            {
                details.Append(' ');
            }

            details.Append(this.Nickname);
        }

        if (includeLocation)
        {
            var location = new StringBuilder();
            if (!string.IsNullOrEmpty(this.City))
            {
                location.Append(this.City);
            }

            if (!string.IsNullOrWhiteSpace(this.StateProv))
            {
                if (location.Length > 0)
                {
                    location.Append(", ");
                }

                location.Append(this.StateProv);
            }

            if (!string.IsNullOrWhiteSpace(this.Country))
            {
                if (location.Length > 0)
                {
                    location.Append(", ");
                }

                location.Append(this.Country);
            }

            if (location.Length > 0)
            {
                details.Append($" - {location}");
            }
        }

        return details.ToString();
    }

    public string GetLabelWithHighlight(ulong? highlightIfIsTeamNumber)
    {
        var teamLabel = GetLabel();
        return highlightIfIsTeamNumber is not null && teamLabel.StartsWith(highlightIfIsTeamNumber.ToString()!, StringComparison.Ordinal)
            ? $"**{teamLabel}**"
            : teamLabel;
    }

    public static implicit operator TeamSimple(Team t) => new(t.City, t.Country, t.Key, t.Name, t.Nickname, t.StateProv, t.TeamNumber);
}
