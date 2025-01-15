// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System;
namespace Common.Tba.Api.Models
{
    /// <summary>
    /// A list of alliances, the teams on the alliances, and their score.
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class Match_alliances : IAdditionalDataHolder, IParsable
    {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The blue property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::Common.Tba.Api.Models.Match_alliance? Blue { get; set; }
#nullable restore
#else
        public global::Common.Tba.Api.Models.Match_alliance Blue { get; set; }
#endif
        /// <summary>The red property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::Common.Tba.Api.Models.Match_alliance? Red { get; set; }
#nullable restore
#else
        public global::Common.Tba.Api.Models.Match_alliance Red { get; set; }
#endif
        /// <summary>
        /// Instantiates a new <see cref="global::Common.Tba.Api.Models.Match_alliances"/> and sets the default values.
        /// </summary>
        public Match_alliances()
        {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::Common.Tba.Api.Models.Match_alliances"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::Common.Tba.Api.Models.Match_alliances CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::Common.Tba.Api.Models.Match_alliances();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "blue", n => { Blue = n.GetObjectValue<global::Common.Tba.Api.Models.Match_alliance>(global::Common.Tba.Api.Models.Match_alliance.CreateFromDiscriminatorValue); } },
                { "red", n => { Red = n.GetObjectValue<global::Common.Tba.Api.Models.Match_alliance>(global::Common.Tba.Api.Models.Match_alliance.CreateFromDiscriminatorValue); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteObjectValue<global::Common.Tba.Api.Models.Match_alliance>("blue", Blue);
            writer.WriteObjectValue<global::Common.Tba.Api.Models.Match_alliance>("red", Red);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
#pragma warning restore CS0618
