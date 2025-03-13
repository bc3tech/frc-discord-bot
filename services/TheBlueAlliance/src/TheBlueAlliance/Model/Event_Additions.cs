namespace TheBlueAlliance.Model;
using System.Text;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "This is its name, sorry")]
public partial record Event
{
    public string GetLabel(bool shortName = false, bool includeYear = false, bool includeCity = false, bool includeStateProv = false, bool includeCountry = false)
    {
        var location = new StringBuilder();
        if (includeCity && !string.IsNullOrWhiteSpace(this.City))
        {
            location.Append(this.City);
        }

        if (includeStateProv && !string.IsNullOrWhiteSpace(this.StateProv))
        {
            if (location.Length > 0)
            {
                location.Append(", ");
            }

            location.Append(this.StateProv);
        }

        if (includeCountry && !string.IsNullOrWhiteSpace(this.Country))
        {
            if (location.Length > 0)
            {
                location.Append(", ");
            }

            location.Append(this.Country);
        }

        return $"{(includeYear ? $"{this.Year} " : string.Empty)}{(shortName ? this.ShortName : this.Name)}{(location.Length > 0 ? $" - {location}" : string.Empty)}";
    }

    private string? _locationString;
    public string LocationString
    {
        get
        {
            if (_locationString is null)
            {
                var location = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(this.LocationName))
                {
                    location.Append($"{this.LocationName}, ");
                }

                if (!string.IsNullOrWhiteSpace(this.City))
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

                _locationString = location.ToString();
            }

            return _locationString;
        }
    }

    public string ScheduleUrl => $"https://frc.link/e/g/{this.FirstEventCode}/{this.Year}";
    public string TbaUrl => $"https://frc.link/e/tba/{this.FirstEventCode}/{this.Year}";
    public string FirstUrl => $"https://frc.link/e/{this.FirstEventCode}/{this.Year}";

    public static implicit operator EventSimple(Event e) => new(e.City, e.Country, e.District, e.EndDate, e.EventCode, e.EventType, e.Key, e.Name, e.StartDate, e.StateProv, e.Year);
}
