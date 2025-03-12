namespace TheBlueAlliance.Model;
using System.Text;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "This is its name, sorry")]
public partial record Event
{
    public string GetLabel(bool shortName = false, bool includeYear = false, bool includeCity = false, bool includeStateProv = false, bool includeCountry = false)
    {
        var location = new StringBuilder();
        if (includeCity && !string.IsNullOrEmpty(this.City))
        {
            location.Append(this.City);
        }

        if (includeStateProv && !string.IsNullOrEmpty(this.StateProv))
        {
            if (location.Length > 0)
            {
                location.Append(", ");
            }

            location.Append(this.StateProv);
        }

        if (includeCountry && !string.IsNullOrEmpty(this.Country))
        {
            if (location.Length > 0)
            {
                location.Append(", ");
            }

            location.Append(this.Country);
        }

        return $"{(includeYear ? $"{this.Year} " : string.Empty)}{(shortName ? this.ShortName : this.Name)}{(location.Length > 0 ? $" - {location}" : string.Empty)}";
    }

    public static implicit operator EventSimple(Event e) => new(e.City, e.Country, e.District, e.EndDate, e.EventCode, e.EventType, e.Key, e.Name, e.StartDate, e.StateProv, e.Year);
}
