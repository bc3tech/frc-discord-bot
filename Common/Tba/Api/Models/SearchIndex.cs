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
    public partial class SearchIndex : IAdditionalDataHolder, IParsable
    #pragma warning restore CS1591
    {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The events property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<global::Common.Tba.Api.Models.SearchIndex_events>? Events { get; set; }
#nullable restore
#else
        public List<global::Common.Tba.Api.Models.SearchIndex_events> Events { get; set; }
#endif
        /// <summary>The teams property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<global::Common.Tba.Api.Models.SearchIndex_teams>? Teams { get; set; }
#nullable restore
#else
        public List<global::Common.Tba.Api.Models.SearchIndex_teams> Teams { get; set; }
#endif
        /// <summary>
        /// Instantiates a new <see cref="global::Common.Tba.Api.Models.SearchIndex"/> and sets the default values.
        /// </summary>
        public SearchIndex()
        {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::Common.Tba.Api.Models.SearchIndex"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::Common.Tba.Api.Models.SearchIndex CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::Common.Tba.Api.Models.SearchIndex();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "events", n => { Events = n.GetCollectionOfObjectValues<global::Common.Tba.Api.Models.SearchIndex_events>(global::Common.Tba.Api.Models.SearchIndex_events.CreateFromDiscriminatorValue)?.AsList(); } },
                { "teams", n => { Teams = n.GetCollectionOfObjectValues<global::Common.Tba.Api.Models.SearchIndex_teams>(global::Common.Tba.Api.Models.SearchIndex_teams.CreateFromDiscriminatorValue)?.AsList(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteCollectionOfObjectValues<global::Common.Tba.Api.Models.SearchIndex_events>("events", Events);
            writer.WriteCollectionOfObjectValues<global::Common.Tba.Api.Models.SearchIndex_teams>("teams", Teams);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
#pragma warning restore CS0618
