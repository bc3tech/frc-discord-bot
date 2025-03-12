namespace TheBlueAlliance.Model;
using System.Text;

public partial record EventSimple
{
    public string GetLabel( bool includeYear = false, bool includeCity = false, bool includeStateProv = false, bool includeCountry = false)
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

        return $"{(includeYear ? $"{this.Year} " : string.Empty)}{this.Name}{(location.Length > 0 ? $" - {location}" : string.Empty)}";
    }
}
