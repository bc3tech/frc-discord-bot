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
    public partial class Match_Score_Breakdown_2017_Alliance : IParsable
    #pragma warning restore CS1591
    {
        /// <summary>The adjustPoints property</summary>
        public int? AdjustPoints { get; set; }
        /// <summary>The autoFuelHigh property</summary>
        public int? AutoFuelHigh { get; set; }
        /// <summary>The autoFuelLow property</summary>
        public int? AutoFuelLow { get; set; }
        /// <summary>The autoFuelPoints property</summary>
        public int? AutoFuelPoints { get; set; }
        /// <summary>The autoMobilityPoints property</summary>
        public int? AutoMobilityPoints { get; set; }
        /// <summary>The autoPoints property</summary>
        public int? AutoPoints { get; set; }
        /// <summary>The autoRotorPoints property</summary>
        public int? AutoRotorPoints { get; set; }
        /// <summary>The foulCount property</summary>
        public int? FoulCount { get; set; }
        /// <summary>The foulPoints property</summary>
        public int? FoulPoints { get; set; }
        /// <summary>The kPaBonusPoints property</summary>
        public int? KPaBonusPoints { get; set; }
        /// <summary>The kPaRankingPointAchieved property</summary>
        public bool? KPaRankingPointAchieved { get; set; }
        /// <summary>The robot1Auto property</summary>
        public global::Common.Tba.Api.Models.Match_Score_Breakdown_2017_Alliance_robot1Auto? Robot1Auto { get; set; }
        /// <summary>The robot2Auto property</summary>
        public global::Common.Tba.Api.Models.Match_Score_Breakdown_2017_Alliance_robot2Auto? Robot2Auto { get; set; }
        /// <summary>The robot3Auto property</summary>
        public global::Common.Tba.Api.Models.Match_Score_Breakdown_2017_Alliance_robot3Auto? Robot3Auto { get; set; }
        /// <summary>The rotor1Auto property</summary>
        public bool? Rotor1Auto { get; set; }
        /// <summary>The rotor1Engaged property</summary>
        public bool? Rotor1Engaged { get; set; }
        /// <summary>The rotor2Auto property</summary>
        public bool? Rotor2Auto { get; set; }
        /// <summary>The rotor2Engaged property</summary>
        public bool? Rotor2Engaged { get; set; }
        /// <summary>The rotor3Engaged property</summary>
        public bool? Rotor3Engaged { get; set; }
        /// <summary>The rotor4Engaged property</summary>
        public bool? Rotor4Engaged { get; set; }
        /// <summary>The rotorBonusPoints property</summary>
        public int? RotorBonusPoints { get; set; }
        /// <summary>The rotorRankingPointAchieved property</summary>
        public bool? RotorRankingPointAchieved { get; set; }
        /// <summary>The techFoulCount property</summary>
        public int? TechFoulCount { get; set; }
        /// <summary>The teleopFuelHigh property</summary>
        public int? TeleopFuelHigh { get; set; }
        /// <summary>The teleopFuelLow property</summary>
        public int? TeleopFuelLow { get; set; }
        /// <summary>The teleopFuelPoints property</summary>
        public int? TeleopFuelPoints { get; set; }
        /// <summary>The teleopPoints property</summary>
        public int? TeleopPoints { get; set; }
        /// <summary>The teleopRotorPoints property</summary>
        public int? TeleopRotorPoints { get; set; }
        /// <summary>The teleopTakeoffPoints property</summary>
        public int? TeleopTakeoffPoints { get; set; }
        /// <summary>The totalPoints property</summary>
        public int? TotalPoints { get; set; }
        /// <summary>The touchpadFar property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? TouchpadFar { get; set; }
#nullable restore
#else
        public string TouchpadFar { get; set; }
#endif
        /// <summary>The touchpadMiddle property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? TouchpadMiddle { get; set; }
#nullable restore
#else
        public string TouchpadMiddle { get; set; }
#endif
        /// <summary>The touchpadNear property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? TouchpadNear { get; set; }
#nullable restore
#else
        public string TouchpadNear { get; set; }
#endif
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::Common.Tba.Api.Models.Match_Score_Breakdown_2017_Alliance"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::Common.Tba.Api.Models.Match_Score_Breakdown_2017_Alliance CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::Common.Tba.Api.Models.Match_Score_Breakdown_2017_Alliance();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "adjustPoints", n => { AdjustPoints = n.GetIntValue(); } },
                { "autoFuelHigh", n => { AutoFuelHigh = n.GetIntValue(); } },
                { "autoFuelLow", n => { AutoFuelLow = n.GetIntValue(); } },
                { "autoFuelPoints", n => { AutoFuelPoints = n.GetIntValue(); } },
                { "autoMobilityPoints", n => { AutoMobilityPoints = n.GetIntValue(); } },
                { "autoPoints", n => { AutoPoints = n.GetIntValue(); } },
                { "autoRotorPoints", n => { AutoRotorPoints = n.GetIntValue(); } },
                { "foulCount", n => { FoulCount = n.GetIntValue(); } },
                { "foulPoints", n => { FoulPoints = n.GetIntValue(); } },
                { "kPaBonusPoints", n => { KPaBonusPoints = n.GetIntValue(); } },
                { "kPaRankingPointAchieved", n => { KPaRankingPointAchieved = n.GetBoolValue(); } },
                { "robot1Auto", n => { Robot1Auto = n.GetEnumValue<global::Common.Tba.Api.Models.Match_Score_Breakdown_2017_Alliance_robot1Auto>(); } },
                { "robot2Auto", n => { Robot2Auto = n.GetEnumValue<global::Common.Tba.Api.Models.Match_Score_Breakdown_2017_Alliance_robot2Auto>(); } },
                { "robot3Auto", n => { Robot3Auto = n.GetEnumValue<global::Common.Tba.Api.Models.Match_Score_Breakdown_2017_Alliance_robot3Auto>(); } },
                { "rotor1Auto", n => { Rotor1Auto = n.GetBoolValue(); } },
                { "rotor1Engaged", n => { Rotor1Engaged = n.GetBoolValue(); } },
                { "rotor2Auto", n => { Rotor2Auto = n.GetBoolValue(); } },
                { "rotor2Engaged", n => { Rotor2Engaged = n.GetBoolValue(); } },
                { "rotor3Engaged", n => { Rotor3Engaged = n.GetBoolValue(); } },
                { "rotor4Engaged", n => { Rotor4Engaged = n.GetBoolValue(); } },
                { "rotorBonusPoints", n => { RotorBonusPoints = n.GetIntValue(); } },
                { "rotorRankingPointAchieved", n => { RotorRankingPointAchieved = n.GetBoolValue(); } },
                { "techFoulCount", n => { TechFoulCount = n.GetIntValue(); } },
                { "teleopFuelHigh", n => { TeleopFuelHigh = n.GetIntValue(); } },
                { "teleopFuelLow", n => { TeleopFuelLow = n.GetIntValue(); } },
                { "teleopFuelPoints", n => { TeleopFuelPoints = n.GetIntValue(); } },
                { "teleopPoints", n => { TeleopPoints = n.GetIntValue(); } },
                { "teleopRotorPoints", n => { TeleopRotorPoints = n.GetIntValue(); } },
                { "teleopTakeoffPoints", n => { TeleopTakeoffPoints = n.GetIntValue(); } },
                { "totalPoints", n => { TotalPoints = n.GetIntValue(); } },
                { "touchpadFar", n => { TouchpadFar = n.GetStringValue(); } },
                { "touchpadMiddle", n => { TouchpadMiddle = n.GetStringValue(); } },
                { "touchpadNear", n => { TouchpadNear = n.GetStringValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteIntValue("adjustPoints", AdjustPoints);
            writer.WriteIntValue("autoFuelHigh", AutoFuelHigh);
            writer.WriteIntValue("autoFuelLow", AutoFuelLow);
            writer.WriteIntValue("autoFuelPoints", AutoFuelPoints);
            writer.WriteIntValue("autoMobilityPoints", AutoMobilityPoints);
            writer.WriteIntValue("autoPoints", AutoPoints);
            writer.WriteIntValue("autoRotorPoints", AutoRotorPoints);
            writer.WriteIntValue("foulCount", FoulCount);
            writer.WriteIntValue("foulPoints", FoulPoints);
            writer.WriteIntValue("kPaBonusPoints", KPaBonusPoints);
            writer.WriteBoolValue("kPaRankingPointAchieved", KPaRankingPointAchieved);
            writer.WriteEnumValue<global::Common.Tba.Api.Models.Match_Score_Breakdown_2017_Alliance_robot1Auto>("robot1Auto", Robot1Auto);
            writer.WriteEnumValue<global::Common.Tba.Api.Models.Match_Score_Breakdown_2017_Alliance_robot2Auto>("robot2Auto", Robot2Auto);
            writer.WriteEnumValue<global::Common.Tba.Api.Models.Match_Score_Breakdown_2017_Alliance_robot3Auto>("robot3Auto", Robot3Auto);
            writer.WriteBoolValue("rotor1Auto", Rotor1Auto);
            writer.WriteBoolValue("rotor1Engaged", Rotor1Engaged);
            writer.WriteBoolValue("rotor2Auto", Rotor2Auto);
            writer.WriteBoolValue("rotor2Engaged", Rotor2Engaged);
            writer.WriteBoolValue("rotor3Engaged", Rotor3Engaged);
            writer.WriteBoolValue("rotor4Engaged", Rotor4Engaged);
            writer.WriteIntValue("rotorBonusPoints", RotorBonusPoints);
            writer.WriteBoolValue("rotorRankingPointAchieved", RotorRankingPointAchieved);
            writer.WriteIntValue("techFoulCount", TechFoulCount);
            writer.WriteIntValue("teleopFuelHigh", TeleopFuelHigh);
            writer.WriteIntValue("teleopFuelLow", TeleopFuelLow);
            writer.WriteIntValue("teleopFuelPoints", TeleopFuelPoints);
            writer.WriteIntValue("teleopPoints", TeleopPoints);
            writer.WriteIntValue("teleopRotorPoints", TeleopRotorPoints);
            writer.WriteIntValue("teleopTakeoffPoints", TeleopTakeoffPoints);
            writer.WriteIntValue("totalPoints", TotalPoints);
            writer.WriteStringValue("touchpadFar", TouchpadFar);
            writer.WriteStringValue("touchpadMiddle", TouchpadMiddle);
            writer.WriteStringValue("touchpadNear", TouchpadNear);
        }
    }
}
#pragma warning restore CS0618
