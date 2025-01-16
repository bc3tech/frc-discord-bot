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
    public partial class Team_Event_Status : IParsable
    #pragma warning restore CS1591
    {
        /// <summary>The alliance property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::Common.Tba.Api.Models.Team_Event_Status_alliance? Alliance { get; set; }
#nullable restore
#else
        public global::Common.Tba.Api.Models.Team_Event_Status_alliance Alliance { get; set; }
#endif
        /// <summary>An HTML formatted string suitable for display to the user containing the team&apos;s alliance pick status.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? AllianceStatusStr { get; set; }
#nullable restore
#else
        public string AllianceStatusStr { get; set; }
#endif
        /// <summary>TBA match key for the last match the team played in at this event, or null.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? LastMatchKey { get; set; }
#nullable restore
#else
        public string LastMatchKey { get; set; }
#endif
        /// <summary>TBA match key for the next match the team is scheduled to play in at this event, or null.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? NextMatchKey { get; set; }
#nullable restore
#else
        public string NextMatchKey { get; set; }
#endif
        /// <summary>An HTML formatted string suitable for display to the user containing the team&apos;s overall status summary of the event.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? OverallStatusStr { get; set; }
#nullable restore
#else
        public string OverallStatusStr { get; set; }
#endif
        /// <summary>Playoff status for this team, may be null if the team did not make playoffs, or playoffs have not begun.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::Common.Tba.Api.Models.Team_Event_Status_playoff? Playoff { get; set; }
#nullable restore
#else
        public global::Common.Tba.Api.Models.Team_Event_Status_playoff Playoff { get; set; }
#endif
        /// <summary>An HTML formatter string suitable for display to the user containing the team&apos;s playoff status.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? PlayoffStatusStr { get; set; }
#nullable restore
#else
        public string PlayoffStatusStr { get; set; }
#endif
        /// <summary>The qual property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::Common.Tba.Api.Models.Team_Event_Status_rank? Qual { get; set; }
#nullable restore
#else
        public global::Common.Tba.Api.Models.Team_Event_Status_rank Qual { get; set; }
#endif
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::Common.Tba.Api.Models.Team_Event_Status"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::Common.Tba.Api.Models.Team_Event_Status CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::Common.Tba.Api.Models.Team_Event_Status();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "alliance", n => { Alliance = n.GetObjectValue<global::Common.Tba.Api.Models.Team_Event_Status_alliance>(global::Common.Tba.Api.Models.Team_Event_Status_alliance.CreateFromDiscriminatorValue); } },
                { "alliance_status_str", n => { AllianceStatusStr = n.GetStringValue(); } },
                { "last_match_key", n => { LastMatchKey = n.GetStringValue(); } },
                { "next_match_key", n => { NextMatchKey = n.GetStringValue(); } },
                { "overall_status_str", n => { OverallStatusStr = n.GetStringValue(); } },
                { "playoff", n => { Playoff = n.GetObjectValue<global::Common.Tba.Api.Models.Team_Event_Status_playoff>(global::Common.Tba.Api.Models.Team_Event_Status_playoff.CreateFromDiscriminatorValue); } },
                { "playoff_status_str", n => { PlayoffStatusStr = n.GetStringValue(); } },
                { "qual", n => { Qual = n.GetObjectValue<global::Common.Tba.Api.Models.Team_Event_Status_rank>(global::Common.Tba.Api.Models.Team_Event_Status_rank.CreateFromDiscriminatorValue); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteObjectValue<global::Common.Tba.Api.Models.Team_Event_Status_alliance>("alliance", Alliance);
            writer.WriteStringValue("alliance_status_str", AllianceStatusStr);
            writer.WriteStringValue("last_match_key", LastMatchKey);
            writer.WriteStringValue("next_match_key", NextMatchKey);
            writer.WriteStringValue("overall_status_str", OverallStatusStr);
            writer.WriteObjectValue<global::Common.Tba.Api.Models.Team_Event_Status_playoff>("playoff", Playoff);
            writer.WriteStringValue("playoff_status_str", PlayoffStatusStr);
            writer.WriteObjectValue<global::Common.Tba.Api.Models.Team_Event_Status_rank>("qual", Qual);
        }
    }
}
#pragma warning restore CS0618
