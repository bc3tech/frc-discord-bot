/*
 * Statbotics REST API
 *
 * The REST API for Statbotics. Please be nice to our servers! If you are looking to do large-scale data science projects, use the CSV exports on the GitHub repo.
 *
 * The version of the OpenAPI document: 3.0.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

    namespace Statbotics.Model;
    
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
  using System.Collections.ObjectModel;
    
    /// <summary>
/// HTTPValidationError
/// </summary>
public partial class HTTPValidationError
{
      /// <summary>
      /// Initializes a new instance of the <see cref="HTTPValidationError" /> class.
      /// </summary>
          /// <param name="detail">detail.</param>
      public HTTPValidationError(Collection<ValidationError>? detail = default)
      {
                    this.Detail = detail;
      }
      
            /// <summary>
            /// Gets or Sets Detail
            /// </summary>
              
                [JsonPropertyName("detail")]
                public Collection<ValidationError>? Detail { get; set; }
                
            /// <summary>
            /// Returns the string presentation of the object
            /// </summary>
            /// <returns>string presentation of the object</returns>
            public override string ToString()
            {
              StringBuilder sb = new();
              sb.AppendLine("class HTTPValidationError {");
                  sb.Append("  Detail: ").AppendLine($"{(Detail is null ? "[null]" : string.Join(", ", Detail))}");
              sb.AppendLine("}");
              return sb.ToString();
            }
            
            /// <summary>
            /// Returns the JSON string presentation of the object
            /// </summary>
            /// <returns>JSON string presentation of the object</returns>
            public string ToJson()
            {
              return JsonSerializer.Serialize(this);
            }
          }
          
