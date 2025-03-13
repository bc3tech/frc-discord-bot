namespace TheBlueAlliance.Model;

using System;
using System.Text;

public partial record Team
{
    public string FirstUrl => $"https://frc.link/t/{this.TeamNumber}";
    public string TbaUrl => $"https://frc.link/tba/{this.TeamNumber}";
    public string TeamSiteUrl => $"https://frc.link/w/{this.TeamNumber}";

    public string GetLabel(bool includeNumber = true, bool includeName = true, bool includeLocation = true, bool asMarkdownLink = true)
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

        return asMarkdownLink ? $"[{details}]({this.TbaUrl})" : details.ToString();
    }

    public string GetLabelWithHighlight(ulong? highlightIfIsTeamNumber, bool asMarkdownLink = true)
    {
        var teamLabel = GetLabel(asMarkdownLink: asMarkdownLink);
        return highlightIfIsTeamNumber is not null && teamLabel.StartsWith(highlightIfIsTeamNumber.ToString()!, StringComparison.Ordinal)
            ? $"**{teamLabel}**"
            : teamLabel;
    }

    public static implicit operator TeamSimple(Team t) => new(t.City, t.Country, t.Key, t.Name, t.Nickname, t.StateProv, t.TeamNumber);
}
