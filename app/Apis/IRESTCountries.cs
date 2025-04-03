namespace FunctionApp.Apis;

using System.Threading;
using System.Threading.Tasks;

internal interface IRESTCountries
{
    Task<string?> GetCountryCodeForFlagLookupAsync(string country, CancellationToken cancellationToken);
}