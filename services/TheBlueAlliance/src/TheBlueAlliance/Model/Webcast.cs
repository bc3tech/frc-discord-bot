/*
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).
 *
 * The version of the OpenAPI document: 3.9.11
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

    namespace TheBlueAlliance.Model;
    
    using System.Runtime.Serialization;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
  using System.Collections.ObjectModel;
    
    /// <summary>
/// Webcast
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public partial record Webcast
  {
            /// <summary>
  /// Type of webcast, typically descriptive of the streaming provider.
  /// </summary>
    /// <value>Type of webcast, typically descriptive of the streaming provider.</value>
  [Microsoft.Extensions.EnumStrings.EnumStrings(ExtensionNamespace = "TheBlueAlliance.Model.WebcastExtensions", ExtensionClassModifiers ="public static")]
  [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberSupport<TypeEnum>))]
  public enum TypeEnum
  {
        /// <summary>
        /// Enum Youtube for value: youtube
        /// </summary>
        [EnumMember(Value = "youtube")]
        Youtube = 1,
          
        /// <summary>
        /// Enum Twitch for value: twitch
        /// </summary>
        [EnumMember(Value = "twitch")]
        Twitch = 2,
          
        /// <summary>
        /// Enum Ustream for value: ustream
        /// </summary>
        [EnumMember(Value = "ustream")]
        Ustream = 3,
          
        /// <summary>
        /// Enum Iframe for value: iframe
        /// </summary>
        [EnumMember(Value = "iframe")]
        Iframe = 4,
          
        /// <summary>
        /// Enum Html5 for value: html5
        /// </summary>
        [EnumMember(Value = "html5")]
        Html5 = 5,
          
        /// <summary>
        /// Enum Rtmp for value: rtmp
        /// </summary>
        [EnumMember(Value = "rtmp")]
        Rtmp = 6,
          
        /// <summary>
        /// Enum Livestream for value: livestream
        /// </summary>
        [EnumMember(Value = "livestream")]
        Livestream = 7,
          
        /// <summary>
        /// Enum DirectLink for value: direct_link
        /// </summary>
        [EnumMember(Value = "direct_link")]
        DirectLink = 8,
          
        /// <summary>
        /// Enum Mms for value: mms
        /// </summary>
        [EnumMember(Value = "mms")]
        Mms = 9,
          
        /// <summary>
        /// Enum Justin for value: justin
        /// </summary>
        [EnumMember(Value = "justin")]
        Justin = 10,
          
        /// <summary>
        /// Enum Stemtv for value: stemtv
        /// </summary>
        [EnumMember(Value = "stemtv")]
        Stemtv = 11,
          
        /// <summary>
        /// Enum Dacast for value: dacast
        /// </summary>
        [EnumMember(Value = "dacast")]
        Dacast = 12
  }
    
    /// <summary>
    /// Returns a <see cref="TypeEnum"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TypeEnum? TypeEnumFromStringOrDefault(string value)
    {
      return value switch
      {
            "youtube" => TypeEnum.Youtube,
            "twitch" => TypeEnum.Twitch,
            "ustream" => TypeEnum.Ustream,
            "iframe" => TypeEnum.Iframe,
            "html5" => TypeEnum.Html5,
            "rtmp" => TypeEnum.Rtmp,
            "livestream" => TypeEnum.Livestream,
            "direct_link" => TypeEnum.DirectLink,
            "mms" => TypeEnum.Mms,
            "justin" => TypeEnum.Justin,
            "stemtv" => TypeEnum.Stemtv,
            "dacast" => TypeEnum.Dacast,
        _ => null
      };
    }

        
        /// <summary>
        /// Type of webcast, typically descriptive of the streaming provider.
        /// </summary>
          /// <value>Type of webcast, typically descriptive of the streaming provider.</value>
          [JsonRequired]
            [JsonPropertyName("type")]
            public TypeEnum Type { get; set; }
              // yup
              /// <summary>
              /// Initializes a new instance of the <see cref="Webcast" /> class.
              /// </summary>
              [JsonConstructor]
              protected Webcast() { 
            }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="Webcast" /> class.
        /// </summary>
            /// <param name="channel">Type specific channel information. May be the YouTube stream, or Twitch channel name. In the case of iframe types, contains HTML to embed the stream in an HTML iframe. (required).</param>
            /// <param name="date">The date for the webcast in &#x60;yyyy-mm-dd&#x60; format. May be null.</param>
            /// <param name="file">File identification as may be required for some types. May be null.</param>
            /// <param name="type">Type of webcast, typically descriptive of the streaming provider. (required).</param>
        public Webcast(string channel, TypeEnum type, string? date = default, string? file = default)
        {
                      // to ensure "channel" is required (not null)
                      ArgumentNullException.ThrowIfNull(channel);
                      this.Channel = channel;
                        
                      this.Date = date;
                      this.File = file;
                      this.Type = type;
        }
        
              /// <summary>
              /// Type specific channel information. May be the YouTube stream, or Twitch channel name. In the case of iframe types, contains HTML to embed the stream in an HTML iframe.
              /// </summary>
              /// <value>Type specific channel information. May be the YouTube stream, or Twitch channel name. In the case of iframe types, contains HTML to embed the stream in an HTML iframe.</value>
                [JsonRequired]
                  [JsonPropertyName("channel")]
                  public string Channel { get; set; }
                  
              /// <summary>
              /// The date for the webcast in &#x60;yyyy-mm-dd&#x60; format. May be null.
              /// </summary>
              /// <value>The date for the webcast in &#x60;yyyy-mm-dd&#x60; format. May be null.</value>
                
                  [JsonPropertyName("date")]
                  public string? Date { get; set; }
                  
              /// <summary>
              /// File identification as may be required for some types. May be null.
              /// </summary>
              /// <value>File identification as may be required for some types. May be null.</value>
                
                  [JsonPropertyName("file")]
                  public string? File { get; set; }
                  
              /// <summary>
              /// Returns the JSON string presentation of the object
              /// </summary>
              /// <returns>JSON string presentation of the object</returns>
              public string ToJson()
              {
                return JsonSerializer.Serialize(this);
              }
            }
            #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
