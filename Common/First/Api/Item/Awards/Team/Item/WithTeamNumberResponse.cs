// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System;
namespace Common.First.Api.Item.Awards.Team.Item
{
    [Obsolete("This class is obsolete. Use WithTeamNumberGetResponse instead.")]
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    #pragma warning disable CS1591
    public partial class WithTeamNumberResponse : global::Common.First.Api.Item.Awards.Team.Item.WithTeamNumberGetResponse, IParsable
    #pragma warning restore CS1591
    {
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::Common.First.Api.Item.Awards.Team.Item.WithTeamNumberResponse"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static new global::Common.First.Api.Item.Awards.Team.Item.WithTeamNumberResponse CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::Common.First.Api.Item.Awards.Team.Item.WithTeamNumberResponse();
        }
    }
}
#pragma warning restore CS0618
