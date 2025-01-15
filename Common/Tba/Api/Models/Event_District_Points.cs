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
    public partial class Event_District_Points : IAdditionalDataHolder, IParsable
    #pragma warning restore CS1591
    {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>Points gained for each team at the event. Stored as a key-value pair with the team key as the key, and an object describing the points as its value.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::Common.Tba.Api.Models.Event_District_Points_points? Points { get; set; }
#nullable restore
#else
        public global::Common.Tba.Api.Models.Event_District_Points_points Points { get; set; }
#endif
        /// <summary>Tiebreaker values for each team at the event. Stored as a key-value pair with the team key as the key, and an object describing the tiebreaker elements as its value.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::Common.Tba.Api.Models.Event_District_Points_tiebreakers? Tiebreakers { get; set; }
#nullable restore
#else
        public global::Common.Tba.Api.Models.Event_District_Points_tiebreakers Tiebreakers { get; set; }
#endif
        /// <summary>
        /// Instantiates a new <see cref="global::Common.Tba.Api.Models.Event_District_Points"/> and sets the default values.
        /// </summary>
        public Event_District_Points()
        {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::Common.Tba.Api.Models.Event_District_Points"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::Common.Tba.Api.Models.Event_District_Points CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::Common.Tba.Api.Models.Event_District_Points();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "points", n => { Points = n.GetObjectValue<global::Common.Tba.Api.Models.Event_District_Points_points>(global::Common.Tba.Api.Models.Event_District_Points_points.CreateFromDiscriminatorValue); } },
                { "tiebreakers", n => { Tiebreakers = n.GetObjectValue<global::Common.Tba.Api.Models.Event_District_Points_tiebreakers>(global::Common.Tba.Api.Models.Event_District_Points_tiebreakers.CreateFromDiscriminatorValue); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteObjectValue<global::Common.Tba.Api.Models.Event_District_Points_points>("points", Points);
            writer.WriteObjectValue<global::Common.Tba.Api.Models.Event_District_Points_tiebreakers>("tiebreakers", Tiebreakers);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
#pragma warning restore CS0618
