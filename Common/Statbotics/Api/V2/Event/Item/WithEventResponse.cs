// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System;
namespace Common.Statbotics.Api.V2.Event.Item
{
    [Obsolete("This class is obsolete. Use WithEventGetResponse instead.")]
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    #pragma warning disable CS1591
    public partial class WithEventResponse : global::Common.Statbotics.Api.V2.Event.Item.WithEventGetResponse, IParsable
    #pragma warning restore CS1591
    {
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::Common.Statbotics.Api.V2.Event.Item.WithEventResponse"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static new global::Common.Statbotics.Api.V2.Event.Item.WithEventResponse CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::Common.Statbotics.Api.V2.Event.Item.WithEventResponse();
        }
    }
}
#pragma warning restore CS0618
