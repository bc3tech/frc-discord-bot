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
    public partial class Elimination_Alliance : IParsable
    #pragma warning restore CS1591
    {
        /// <summary>Backup team called in, may be null.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::Common.Tba.Api.Models.Elimination_Alliance_backup? Backup { get; set; }
#nullable restore
#else
        public global::Common.Tba.Api.Models.Elimination_Alliance_backup Backup { get; set; }
#endif
        /// <summary>List of teams that declined the alliance.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<string>? Declines { get; set; }
#nullable restore
#else
        public List<string> Declines { get; set; }
#endif
        /// <summary>Alliance name, may be null.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Name { get; set; }
#nullable restore
#else
        public string Name { get; set; }
#endif
        /// <summary>List of team keys picked for the alliance. First pick is captain.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<string>? Picks { get; set; }
#nullable restore
#else
        public List<string> Picks { get; set; }
#endif
        /// <summary>The status property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::Common.Tba.Api.Models.Elimination_Alliance_status? Status { get; set; }
#nullable restore
#else
        public global::Common.Tba.Api.Models.Elimination_Alliance_status Status { get; set; }
#endif
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::Common.Tba.Api.Models.Elimination_Alliance"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::Common.Tba.Api.Models.Elimination_Alliance CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::Common.Tba.Api.Models.Elimination_Alliance();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "backup", n => { Backup = n.GetObjectValue<global::Common.Tba.Api.Models.Elimination_Alliance_backup>(global::Common.Tba.Api.Models.Elimination_Alliance_backup.CreateFromDiscriminatorValue); } },
                { "declines", n => { Declines = n.GetCollectionOfPrimitiveValues<string>()?.AsList(); } },
                { "name", n => { Name = n.GetStringValue(); } },
                { "picks", n => { Picks = n.GetCollectionOfPrimitiveValues<string>()?.AsList(); } },
                { "status", n => { Status = n.GetObjectValue<global::Common.Tba.Api.Models.Elimination_Alliance_status>(global::Common.Tba.Api.Models.Elimination_Alliance_status.CreateFromDiscriminatorValue); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteObjectValue<global::Common.Tba.Api.Models.Elimination_Alliance_backup>("backup", Backup);
            writer.WriteCollectionOfPrimitiveValues<string>("declines", Declines);
            writer.WriteStringValue("name", Name);
            writer.WriteCollectionOfPrimitiveValues<string>("picks", Picks);
            writer.WriteObjectValue<global::Common.Tba.Api.Models.Elimination_Alliance_status>("status", Status);
        }
    }
}
#pragma warning restore CS0618
