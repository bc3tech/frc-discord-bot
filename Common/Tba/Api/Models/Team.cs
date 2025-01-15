// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System;
namespace Common.Tba.Api.Models
{
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    #pragma warning disable CS1591
    public partial class Team : IAdditionalDataHolder, IParsable
    #pragma warning restore CS1591
    {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>Will be NULL, for future development.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Address { get; set; }
#nullable restore
#else
        public string Address { get; set; }
#endif
        /// <summary>City of team derived from parsing the address registered with FIRST.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? City { get; set; }
#nullable restore
#else
        public string City { get; set; }
#endif
        /// <summary>Country of team derived from parsing the address registered with FIRST.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Country { get; set; }
#nullable restore
#else
        public string Country { get; set; }
#endif
        /// <summary>Will be NULL, for future development.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? GmapsPlaceId { get; set; }
#nullable restore
#else
        public string GmapsPlaceId { get; set; }
#endif
        /// <summary>Will be NULL, for future development.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? GmapsUrl { get; set; }
#nullable restore
#else
        public string GmapsUrl { get; set; }
#endif
        /// <summary>TBA team key with the format `frcXXXX` with `XXXX` representing the team number.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Key { get; set; }
#nullable restore
#else
        public string Key { get; set; }
#endif
        /// <summary>Will be NULL, for future development.</summary>
        public double? Lat { get; set; }
        /// <summary>Will be NULL, for future development.</summary>
        public double? Lng { get; set; }
        /// <summary>Will be NULL, for future development.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? LocationName { get; set; }
#nullable restore
#else
        public string LocationName { get; set; }
#endif
        /// <summary>Official long name registered with FIRST.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Name { get; set; }
#nullable restore
#else
        public string Name { get; set; }
#endif
        /// <summary>Team nickname provided by FIRST.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Nickname { get; set; }
#nullable restore
#else
        public string Nickname { get; set; }
#endif
        /// <summary>Postal code from the team address.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? PostalCode { get; set; }
#nullable restore
#else
        public string PostalCode { get; set; }
#endif
        /// <summary>First year the team officially competed.</summary>
        public int? RookieYear { get; set; }
        /// <summary>Name of team school or affilited group registered with FIRST.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? SchoolName { get; set; }
#nullable restore
#else
        public string SchoolName { get; set; }
#endif
        /// <summary>State of team derived from parsing the address registered with FIRST.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? StateProv { get; set; }
#nullable restore
#else
        public string StateProv { get; set; }
#endif
        /// <summary>Official team number issued by FIRST.</summary>
        public int? TeamNumber { get; set; }
        /// <summary>Official website associated with the team.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Website { get; set; }
#nullable restore
#else
        public string Website { get; set; }
#endif
        /// <summary>
        /// Instantiates a new <see cref="global::Common.Tba.Api.Models.Team"/> and sets the default values.
        /// </summary>
        public Team()
        {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::Common.Tba.Api.Models.Team"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::Common.Tba.Api.Models.Team CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::Common.Tba.Api.Models.Team();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "address", n => { Address = n.GetStringValue(); } },
                { "city", n => { City = n.GetStringValue(); } },
                { "country", n => { Country = n.GetStringValue(); } },
                { "gmaps_place_id", n => { GmapsPlaceId = n.GetStringValue(); } },
                { "gmaps_url", n => { GmapsUrl = n.GetStringValue(); } },
                { "key", n => { Key = n.GetStringValue(); } },
                { "lat", n => { Lat = n.GetDoubleValue(); } },
                { "lng", n => { Lng = n.GetDoubleValue(); } },
                { "location_name", n => { LocationName = n.GetStringValue(); } },
                { "name", n => { Name = n.GetStringValue(); } },
                { "nickname", n => { Nickname = n.GetStringValue(); } },
                { "postal_code", n => { PostalCode = n.GetStringValue(); } },
                { "rookie_year", n => { RookieYear = n.GetIntValue(); } },
                { "school_name", n => { SchoolName = n.GetStringValue(); } },
                { "state_prov", n => { StateProv = n.GetStringValue(); } },
                { "team_number", n => { TeamNumber = n.GetIntValue(); } },
                { "website", n => { Website = n.GetStringValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteStringValue("address", Address);
            writer.WriteStringValue("city", City);
            writer.WriteStringValue("country", Country);
            writer.WriteStringValue("gmaps_place_id", GmapsPlaceId);
            writer.WriteStringValue("gmaps_url", GmapsUrl);
            writer.WriteStringValue("key", Key);
            writer.WriteDoubleValue("lat", Lat);
            writer.WriteDoubleValue("lng", Lng);
            writer.WriteStringValue("location_name", LocationName);
            writer.WriteStringValue("name", Name);
            writer.WriteStringValue("nickname", Nickname);
            writer.WriteStringValue("postal_code", PostalCode);
            writer.WriteIntValue("rookie_year", RookieYear);
            writer.WriteStringValue("school_name", SchoolName);
            writer.WriteStringValue("state_prov", StateProv);
            writer.WriteIntValue("team_number", TeamNumber);
            writer.WriteStringValue("website", Website);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
#pragma warning restore CS0618
