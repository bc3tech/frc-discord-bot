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
    public partial class Match : IParsable
    #pragma warning restore CS1591
    {
        /// <summary>UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of actual match start time.</summary>
        public long? ActualTime { get; set; }
        /// <summary>A list of alliances, the teams on the alliances, and their score.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::Common.Tba.Api.Models.Match_alliances? Alliances { get; set; }
#nullable restore
#else
        public global::Common.Tba.Api.Models.Match_alliances Alliances { get; set; }
#endif
        /// <summary>The competition level the match was played at.</summary>
        public global::Common.Tba.Api.Models.Match_comp_level? CompLevel { get; set; }
        /// <summary>Event key of the event the match was played at.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? EventKey { get; set; }
#nullable restore
#else
        public string EventKey { get; set; }
#endif
        /// <summary>TBA match key with the format `yyyy[EVENT_CODE]_[COMP_LEVEL]m[MATCH_NUMBER]`, where `yyyy` is the year, and `EVENT_CODE` is the event code of the event, `COMP_LEVEL` is (qm, ef, qf, sf, f), and `MATCH_NUMBER` is the match number in the competition level. A set number may be appended to the competition level if more than one match in required per set.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Key { get; set; }
#nullable restore
#else
        public string Key { get; set; }
#endif
        /// <summary>The match number of the match in the competition level.</summary>
        public int? MatchNumber { get; set; }
        /// <summary>UNIX timestamp (seconds since 1-Jan-1970 00:00:00) when the match result was posted.</summary>
        public long? PostResultTime { get; set; }
        /// <summary>UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of the TBA predicted match start time.</summary>
        public long? PredictedTime { get; set; }
        /// <summary>Score breakdown for auto, teleop, etc. points. Varies from year to year. May be null.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::Common.Tba.Api.Models.Match.Match_score_breakdown? ScoreBreakdown { get; set; }
#nullable restore
#else
        public global::Common.Tba.Api.Models.Match.Match_score_breakdown ScoreBreakdown { get; set; }
#endif
        /// <summary>The set number in a series of matches where more than one match is required in the match series.</summary>
        public int? SetNumber { get; set; }
        /// <summary>UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of the scheduled match time, as taken from the published schedule.</summary>
        public long? Time { get; set; }
        /// <summary>Array of video objects associated with this match.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<global::Common.Tba.Api.Models.Match_videos>? Videos { get; set; }
#nullable restore
#else
        public List<global::Common.Tba.Api.Models.Match_videos> Videos { get; set; }
#endif
        /// <summary>The color (red/blue) of the winning alliance. Will contain an empty string in the event of no winner, or a tie.</summary>
        public global::Common.Tba.Api.Models.Match_winning_alliance? WinningAlliance { get; set; }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::Common.Tba.Api.Models.Match"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::Common.Tba.Api.Models.Match CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::Common.Tba.Api.Models.Match();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "actual_time", n => { ActualTime = n.GetLongValue(); } },
                { "alliances", n => { Alliances = n.GetObjectValue<global::Common.Tba.Api.Models.Match_alliances>(global::Common.Tba.Api.Models.Match_alliances.CreateFromDiscriminatorValue); } },
                { "comp_level", n => { CompLevel = n.GetEnumValue<global::Common.Tba.Api.Models.Match_comp_level>(); } },
                { "event_key", n => { EventKey = n.GetStringValue(); } },
                { "key", n => { Key = n.GetStringValue(); } },
                { "match_number", n => { MatchNumber = n.GetIntValue(); } },
                { "post_result_time", n => { PostResultTime = n.GetLongValue(); } },
                { "predicted_time", n => { PredictedTime = n.GetLongValue(); } },
                { "score_breakdown", n => { ScoreBreakdown = n.GetObjectValue<global::Common.Tba.Api.Models.Match.Match_score_breakdown>(global::Common.Tba.Api.Models.Match.Match_score_breakdown.CreateFromDiscriminatorValue); } },
                { "set_number", n => { SetNumber = n.GetIntValue(); } },
                { "time", n => { Time = n.GetLongValue(); } },
                { "videos", n => { Videos = n.GetCollectionOfObjectValues<global::Common.Tba.Api.Models.Match_videos>(global::Common.Tba.Api.Models.Match_videos.CreateFromDiscriminatorValue)?.AsList(); } },
                { "winning_alliance", n => { WinningAlliance = n.GetEnumValue<global::Common.Tba.Api.Models.Match_winning_alliance>(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteLongValue("actual_time", ActualTime);
            writer.WriteObjectValue<global::Common.Tba.Api.Models.Match_alliances>("alliances", Alliances);
            writer.WriteEnumValue<global::Common.Tba.Api.Models.Match_comp_level>("comp_level", CompLevel);
            writer.WriteStringValue("event_key", EventKey);
            writer.WriteStringValue("key", Key);
            writer.WriteIntValue("match_number", MatchNumber);
            writer.WriteLongValue("post_result_time", PostResultTime);
            writer.WriteLongValue("predicted_time", PredictedTime);
            writer.WriteObjectValue<global::Common.Tba.Api.Models.Match.Match_score_breakdown>("score_breakdown", ScoreBreakdown);
            writer.WriteIntValue("set_number", SetNumber);
            writer.WriteLongValue("time", Time);
            writer.WriteCollectionOfObjectValues<global::Common.Tba.Api.Models.Match_videos>("videos", Videos);
            writer.WriteEnumValue<global::Common.Tba.Api.Models.Match_winning_alliance>("winning_alliance", WinningAlliance);
        }
        /// <summary>
        /// Composed type wrapper for classes <see cref="global::Common.Tba.Api.Models.Match_Score_Breakdown_2015"/>, <see cref="global::Common.Tba.Api.Models.Match_Score_Breakdown_2016"/>, <see cref="global::Common.Tba.Api.Models.Match_Score_Breakdown_2017"/>, <see cref="global::Common.Tba.Api.Models.Match_Score_Breakdown_2018"/>, <see cref="global::Common.Tba.Api.Models.Match_Score_Breakdown_2019"/>, <see cref="global::Common.Tba.Api.Models.Match_Score_Breakdown_2020"/>, <see cref="global::Common.Tba.Api.Models.Match_Score_Breakdown_2022"/>, <see cref="global::Common.Tba.Api.Models.Match_Score_Breakdown_2023"/>, <see cref="global::Common.Tba.Api.Models.Match_Score_Breakdown_2024"/>
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
        public partial class Match_score_breakdown : IComposedTypeWrapper, IParsable
        {
            /// <summary>Composed type representation for type <see cref="global::Common.Tba.Api.Models.Match_Score_Breakdown_2015"/></summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            public global::Common.Tba.Api.Models.Match_Score_Breakdown_2015? MatchScoreBreakdown2015 { get; set; }
#nullable restore
#else
            public global::Common.Tba.Api.Models.Match_Score_Breakdown_2015 MatchScoreBreakdown2015 { get; set; }
#endif
            /// <summary>Composed type representation for type <see cref="global::Common.Tba.Api.Models.Match_Score_Breakdown_2016"/></summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            public global::Common.Tba.Api.Models.Match_Score_Breakdown_2016? MatchScoreBreakdown2016 { get; set; }
#nullable restore
#else
            public global::Common.Tba.Api.Models.Match_Score_Breakdown_2016 MatchScoreBreakdown2016 { get; set; }
#endif
            /// <summary>Composed type representation for type <see cref="global::Common.Tba.Api.Models.Match_Score_Breakdown_2017"/></summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            public global::Common.Tba.Api.Models.Match_Score_Breakdown_2017? MatchScoreBreakdown2017 { get; set; }
#nullable restore
#else
            public global::Common.Tba.Api.Models.Match_Score_Breakdown_2017 MatchScoreBreakdown2017 { get; set; }
#endif
            /// <summary>Composed type representation for type <see cref="global::Common.Tba.Api.Models.Match_Score_Breakdown_2018"/></summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            public global::Common.Tba.Api.Models.Match_Score_Breakdown_2018? MatchScoreBreakdown2018 { get; set; }
#nullable restore
#else
            public global::Common.Tba.Api.Models.Match_Score_Breakdown_2018 MatchScoreBreakdown2018 { get; set; }
#endif
            /// <summary>Composed type representation for type <see cref="global::Common.Tba.Api.Models.Match_Score_Breakdown_2019"/></summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            public global::Common.Tba.Api.Models.Match_Score_Breakdown_2019? MatchScoreBreakdown2019 { get; set; }
#nullable restore
#else
            public global::Common.Tba.Api.Models.Match_Score_Breakdown_2019 MatchScoreBreakdown2019 { get; set; }
#endif
            /// <summary>Composed type representation for type <see cref="global::Common.Tba.Api.Models.Match_Score_Breakdown_2020"/></summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            public global::Common.Tba.Api.Models.Match_Score_Breakdown_2020? MatchScoreBreakdown2020 { get; set; }
#nullable restore
#else
            public global::Common.Tba.Api.Models.Match_Score_Breakdown_2020 MatchScoreBreakdown2020 { get; set; }
#endif
            /// <summary>Composed type representation for type <see cref="global::Common.Tba.Api.Models.Match_Score_Breakdown_2022"/></summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            public global::Common.Tba.Api.Models.Match_Score_Breakdown_2022? MatchScoreBreakdown2022 { get; set; }
#nullable restore
#else
            public global::Common.Tba.Api.Models.Match_Score_Breakdown_2022 MatchScoreBreakdown2022 { get; set; }
#endif
            /// <summary>Composed type representation for type <see cref="global::Common.Tba.Api.Models.Match_Score_Breakdown_2023"/></summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            public global::Common.Tba.Api.Models.Match_Score_Breakdown_2023? MatchScoreBreakdown2023 { get; set; }
#nullable restore
#else
            public global::Common.Tba.Api.Models.Match_Score_Breakdown_2023 MatchScoreBreakdown2023 { get; set; }
#endif
            /// <summary>Composed type representation for type <see cref="global::Common.Tba.Api.Models.Match_Score_Breakdown_2024"/></summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            public global::Common.Tba.Api.Models.Match_Score_Breakdown_2024? MatchScoreBreakdown2024 { get; set; }
#nullable restore
#else
            public global::Common.Tba.Api.Models.Match_Score_Breakdown_2024 MatchScoreBreakdown2024 { get; set; }
#endif
            /// <summary>
            /// Creates a new instance of the appropriate class based on discriminator value
            /// </summary>
            /// <returns>A <see cref="global::Common.Tba.Api.Models.Match.Match_score_breakdown"/></returns>
            /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
            public static global::Common.Tba.Api.Models.Match.Match_score_breakdown CreateFromDiscriminatorValue(IParseNode parseNode)
            {
                _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
                var mappingValue = parseNode.GetChildNode("")?.GetStringValue();
                var result = new global::Common.Tba.Api.Models.Match.Match_score_breakdown();
                if("Match_Score_Breakdown_2015".Equals(mappingValue, StringComparison.OrdinalIgnoreCase))
                {
                    result.MatchScoreBreakdown2015 = new global::Common.Tba.Api.Models.Match_Score_Breakdown_2015();
                }
                else if("Match_Score_Breakdown_2016".Equals(mappingValue, StringComparison.OrdinalIgnoreCase))
                {
                    result.MatchScoreBreakdown2016 = new global::Common.Tba.Api.Models.Match_Score_Breakdown_2016();
                }
                else if("Match_Score_Breakdown_2017".Equals(mappingValue, StringComparison.OrdinalIgnoreCase))
                {
                    result.MatchScoreBreakdown2017 = new global::Common.Tba.Api.Models.Match_Score_Breakdown_2017();
                }
                else if("Match_Score_Breakdown_2018".Equals(mappingValue, StringComparison.OrdinalIgnoreCase))
                {
                    result.MatchScoreBreakdown2018 = new global::Common.Tba.Api.Models.Match_Score_Breakdown_2018();
                }
                else if("Match_Score_Breakdown_2019".Equals(mappingValue, StringComparison.OrdinalIgnoreCase))
                {
                    result.MatchScoreBreakdown2019 = new global::Common.Tba.Api.Models.Match_Score_Breakdown_2019();
                }
                else if("Match_Score_Breakdown_2020".Equals(mappingValue, StringComparison.OrdinalIgnoreCase))
                {
                    result.MatchScoreBreakdown2020 = new global::Common.Tba.Api.Models.Match_Score_Breakdown_2020();
                }
                else if("Match_Score_Breakdown_2022".Equals(mappingValue, StringComparison.OrdinalIgnoreCase))
                {
                    result.MatchScoreBreakdown2022 = new global::Common.Tba.Api.Models.Match_Score_Breakdown_2022();
                }
                else if("Match_Score_Breakdown_2023".Equals(mappingValue, StringComparison.OrdinalIgnoreCase))
                {
                    result.MatchScoreBreakdown2023 = new global::Common.Tba.Api.Models.Match_Score_Breakdown_2023();
                }
                else if("Match_Score_Breakdown_2024".Equals(mappingValue, StringComparison.OrdinalIgnoreCase))
                {
                    result.MatchScoreBreakdown2024 = new global::Common.Tba.Api.Models.Match_Score_Breakdown_2024();
                }
                return result;
            }
            /// <summary>
            /// The deserialization information for the current model
            /// </summary>
            /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
            public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
            {
                if(MatchScoreBreakdown2015 != null)
                {
                    return MatchScoreBreakdown2015.GetFieldDeserializers();
                }
                else if(MatchScoreBreakdown2016 != null)
                {
                    return MatchScoreBreakdown2016.GetFieldDeserializers();
                }
                else if(MatchScoreBreakdown2017 != null)
                {
                    return MatchScoreBreakdown2017.GetFieldDeserializers();
                }
                else if(MatchScoreBreakdown2018 != null)
                {
                    return MatchScoreBreakdown2018.GetFieldDeserializers();
                }
                else if(MatchScoreBreakdown2019 != null)
                {
                    return MatchScoreBreakdown2019.GetFieldDeserializers();
                }
                else if(MatchScoreBreakdown2020 != null)
                {
                    return MatchScoreBreakdown2020.GetFieldDeserializers();
                }
                else if(MatchScoreBreakdown2022 != null)
                {
                    return MatchScoreBreakdown2022.GetFieldDeserializers();
                }
                else if(MatchScoreBreakdown2023 != null)
                {
                    return MatchScoreBreakdown2023.GetFieldDeserializers();
                }
                else if(MatchScoreBreakdown2024 != null)
                {
                    return MatchScoreBreakdown2024.GetFieldDeserializers();
                }
                return new Dictionary<string, Action<IParseNode>>();
            }
            /// <summary>
            /// Serializes information the current object
            /// </summary>
            /// <param name="writer">Serialization writer to use to serialize this model</param>
            public virtual void Serialize(ISerializationWriter writer)
            {
                _ = writer ?? throw new ArgumentNullException(nameof(writer));
                if(MatchScoreBreakdown2015 != null)
                {
                    writer.WriteObjectValue<global::Common.Tba.Api.Models.Match_Score_Breakdown_2015>(null, MatchScoreBreakdown2015);
                }
                else if(MatchScoreBreakdown2016 != null)
                {
                    writer.WriteObjectValue<global::Common.Tba.Api.Models.Match_Score_Breakdown_2016>(null, MatchScoreBreakdown2016);
                }
                else if(MatchScoreBreakdown2017 != null)
                {
                    writer.WriteObjectValue<global::Common.Tba.Api.Models.Match_Score_Breakdown_2017>(null, MatchScoreBreakdown2017);
                }
                else if(MatchScoreBreakdown2018 != null)
                {
                    writer.WriteObjectValue<global::Common.Tba.Api.Models.Match_Score_Breakdown_2018>(null, MatchScoreBreakdown2018);
                }
                else if(MatchScoreBreakdown2019 != null)
                {
                    writer.WriteObjectValue<global::Common.Tba.Api.Models.Match_Score_Breakdown_2019>(null, MatchScoreBreakdown2019);
                }
                else if(MatchScoreBreakdown2020 != null)
                {
                    writer.WriteObjectValue<global::Common.Tba.Api.Models.Match_Score_Breakdown_2020>(null, MatchScoreBreakdown2020);
                }
                else if(MatchScoreBreakdown2022 != null)
                {
                    writer.WriteObjectValue<global::Common.Tba.Api.Models.Match_Score_Breakdown_2022>(null, MatchScoreBreakdown2022);
                }
                else if(MatchScoreBreakdown2023 != null)
                {
                    writer.WriteObjectValue<global::Common.Tba.Api.Models.Match_Score_Breakdown_2023>(null, MatchScoreBreakdown2023);
                }
                else if(MatchScoreBreakdown2024 != null)
                {
                    writer.WriteObjectValue<global::Common.Tba.Api.Models.Match_Score_Breakdown_2024>(null, MatchScoreBreakdown2024);
                }
            }
        }
    }
}
#pragma warning restore CS0618
