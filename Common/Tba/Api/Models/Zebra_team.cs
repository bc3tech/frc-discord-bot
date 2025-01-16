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
    public partial class Zebra_team : IParsable
    #pragma warning restore CS1591
    {
        /// <summary>The TBA team key for the Zebra MotionWorks data.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? TeamKey { get; set; }
#nullable restore
#else
        public string TeamKey { get; set; }
#endif
        /// <summary>A list containing doubles and nulls representing a teams X position in feet at the corresponding timestamp. A null value represents no tracking data for a given timestamp.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<double?>? Xs { get; set; }
#nullable restore
#else
        public List<double?> Xs { get; set; }
#endif
        /// <summary>A list containing doubles and nulls representing a teams Y position in feet at the corresponding timestamp. A null value represents no tracking data for a given timestamp.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<double?>? Ys { get; set; }
#nullable restore
#else
        public List<double?> Ys { get; set; }
#endif
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::Common.Tba.Api.Models.Zebra_team"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::Common.Tba.Api.Models.Zebra_team CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::Common.Tba.Api.Models.Zebra_team();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "team_key", n => { TeamKey = n.GetStringValue(); } },
                { "xs", n => { Xs = n.GetCollectionOfPrimitiveValues<double?>()?.AsList(); } },
                { "ys", n => { Ys = n.GetCollectionOfPrimitiveValues<double?>()?.AsList(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteStringValue("team_key", TeamKey);
            writer.WriteCollectionOfPrimitiveValues<double?>("xs", Xs);
            writer.WriteCollectionOfPrimitiveValues<double?>("ys", Ys);
        }
    }
}
#pragma warning restore CS0618
