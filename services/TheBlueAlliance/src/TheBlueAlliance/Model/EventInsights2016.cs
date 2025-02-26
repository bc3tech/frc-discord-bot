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
/// Insights for FIRST Stronghold qualification and elimination matches.
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public partial record EventInsights2016
  {
              // yup
              /// <summary>
              /// Initializes a new instance of the <see cref="EventInsights2016" /> class.
              /// </summary>
              [JsonConstructor]
              protected EventInsights2016() { 
            }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="EventInsights2016" /> class.
        /// </summary>
            /// <param name="aChevalDeFrise">For the Cheval De Frise - An array with three values, number of times damaged, number of opportunities to damage, and percentage. (required).</param>
            /// <param name="aPortcullis">For the Portcullis - An array with three values, number of times damaged, number of opportunities to damage, and percentage. (required).</param>
            /// <param name="averageAutoScore">Average autonomous score. (required).</param>
            /// <param name="averageBoulderScore">Average boulder score. (required).</param>
            /// <param name="averageCrossingScore">Average crossing score. (required).</param>
            /// <param name="averageFoulScore">Average foul score. (required).</param>
            /// <param name="averageHighGoals">Average number of high goals scored. (required).</param>
            /// <param name="averageLowGoals">Average number of low goals scored. (required).</param>
            /// <param name="averageScore">Average total score. (required).</param>
            /// <param name="averageTowerScore">Average tower score. (required).</param>
            /// <param name="averageWinMargin">Average margin of victory. (required).</param>
            /// <param name="averageWinScore">Average winning score. (required).</param>
            /// <param name="bMoat">For the Moat - An array with three values, number of times damaged, number of opportunities to damage, and percentage. (required).</param>
            /// <param name="bRamparts">For the Ramparts - An array with three values, number of times damaged, number of opportunities to damage, and percentage. (required).</param>
            /// <param name="breaches">An array with three values, number of times breached, number of opportunities to breach, and percentage. (required).</param>
            /// <param name="cDrawbridge">For the Drawbridge - An array with three values, number of times damaged, number of opportunities to damage, and percentage. (required).</param>
            /// <param name="cSallyPort">For the Sally Port - An array with three values, number of times damaged, number of opportunities to damage, and percentage. (required).</param>
            /// <param name="captures">An array with three values, number of times captured, number of opportunities to capture, and percentage. (required).</param>
            /// <param name="challenges">An array with three values, number of times challenged, number of opportunities to challenge, and percentage. (required).</param>
            /// <param name="dRockWall">For the Rock Wall - An array with three values, number of times damaged, number of opportunities to damage, and percentage. (required).</param>
            /// <param name="dRoughTerrain">For the Rough Terrain - An array with three values, number of times damaged, number of opportunities to damage, and percentage. (required).</param>
            /// <param name="highScore">An array with three values, high score, match key from the match with the high score, and the name of the match. (required).</param>
            /// <param name="lowBar">For the Low Bar - An array with three values, number of times damaged, number of opportunities to damage, and percentage. (required).</param>
            /// <param name="scales">An array with three values, number of times scaled, number of opportunities to scale, and percentage. (required).</param>
        public EventInsights2016(Collection<float> aChevalDeFrise, Collection<float> aPortcullis, float averageAutoScore, float averageBoulderScore, float averageCrossingScore, float averageFoulScore, float averageHighGoals, float averageLowGoals, float averageScore, float averageTowerScore, float averageWinMargin, float averageWinScore, Collection<float> bMoat, Collection<float> bRamparts, Collection<float> breaches, Collection<float> cDrawbridge, Collection<float> cSallyPort, Collection<float> captures, Collection<float> challenges, Collection<float> dRockWall, Collection<float> dRoughTerrain, Collection<string> highScore, Collection<float> lowBar, Collection<float> scales)
        {
                      // to ensure "aChevalDeFrise" is required (not null)
                      ArgumentNullException.ThrowIfNull(aChevalDeFrise);
                      this.AChevalDeFrise = aChevalDeFrise;
                        
                      // to ensure "aPortcullis" is required (not null)
                      ArgumentNullException.ThrowIfNull(aPortcullis);
                      this.APortcullis = aPortcullis;
                        
                      this.AverageAutoScore = averageAutoScore;
                      this.AverageBoulderScore = averageBoulderScore;
                      this.AverageCrossingScore = averageCrossingScore;
                      this.AverageFoulScore = averageFoulScore;
                      this.AverageHighGoals = averageHighGoals;
                      this.AverageLowGoals = averageLowGoals;
                      this.AverageScore = averageScore;
                      this.AverageTowerScore = averageTowerScore;
                      this.AverageWinMargin = averageWinMargin;
                      this.AverageWinScore = averageWinScore;
                      // to ensure "bMoat" is required (not null)
                      ArgumentNullException.ThrowIfNull(bMoat);
                      this.BMoat = bMoat;
                        
                      // to ensure "bRamparts" is required (not null)
                      ArgumentNullException.ThrowIfNull(bRamparts);
                      this.BRamparts = bRamparts;
                        
                      // to ensure "breaches" is required (not null)
                      ArgumentNullException.ThrowIfNull(breaches);
                      this.Breaches = breaches;
                        
                      // to ensure "cDrawbridge" is required (not null)
                      ArgumentNullException.ThrowIfNull(cDrawbridge);
                      this.CDrawbridge = cDrawbridge;
                        
                      // to ensure "cSallyPort" is required (not null)
                      ArgumentNullException.ThrowIfNull(cSallyPort);
                      this.CSallyPort = cSallyPort;
                        
                      // to ensure "captures" is required (not null)
                      ArgumentNullException.ThrowIfNull(captures);
                      this.Captures = captures;
                        
                      // to ensure "challenges" is required (not null)
                      ArgumentNullException.ThrowIfNull(challenges);
                      this.Challenges = challenges;
                        
                      // to ensure "dRockWall" is required (not null)
                      ArgumentNullException.ThrowIfNull(dRockWall);
                      this.DRockWall = dRockWall;
                        
                      // to ensure "dRoughTerrain" is required (not null)
                      ArgumentNullException.ThrowIfNull(dRoughTerrain);
                      this.DRoughTerrain = dRoughTerrain;
                        
                      // to ensure "highScore" is required (not null)
                      ArgumentNullException.ThrowIfNull(highScore);
                      this.HighScore = highScore;
                        
                      // to ensure "lowBar" is required (not null)
                      ArgumentNullException.ThrowIfNull(lowBar);
                      this.LowBar = lowBar;
                        
                      // to ensure "scales" is required (not null)
                      ArgumentNullException.ThrowIfNull(scales);
                      this.Scales = scales;
        }
        
              /// <summary>
              /// For the Cheval De Frise - An array with three values, number of times damaged, number of opportunities to damage, and percentage.
              /// </summary>
              /// <value>For the Cheval De Frise - An array with three values, number of times damaged, number of opportunities to damage, and percentage.</value>
                [JsonRequired]
                  [JsonPropertyName("A_ChevalDeFrise")]
                  public Collection<float> AChevalDeFrise { get; set; }
                  
              /// <summary>
              /// For the Portcullis - An array with three values, number of times damaged, number of opportunities to damage, and percentage.
              /// </summary>
              /// <value>For the Portcullis - An array with three values, number of times damaged, number of opportunities to damage, and percentage.</value>
                [JsonRequired]
                  [JsonPropertyName("A_Portcullis")]
                  public Collection<float> APortcullis { get; set; }
                  
              /// <summary>
              /// Average autonomous score.
              /// </summary>
              /// <value>Average autonomous score.</value>
                [JsonRequired]
                  [JsonPropertyName("average_auto_score")]
                  public float AverageAutoScore { get; set; }
                  
              /// <summary>
              /// Average boulder score.
              /// </summary>
              /// <value>Average boulder score.</value>
                [JsonRequired]
                  [JsonPropertyName("average_boulder_score")]
                  public float AverageBoulderScore { get; set; }
                  
              /// <summary>
              /// Average crossing score.
              /// </summary>
              /// <value>Average crossing score.</value>
                [JsonRequired]
                  [JsonPropertyName("average_crossing_score")]
                  public float AverageCrossingScore { get; set; }
                  
              /// <summary>
              /// Average foul score.
              /// </summary>
              /// <value>Average foul score.</value>
                [JsonRequired]
                  [JsonPropertyName("average_foul_score")]
                  public float AverageFoulScore { get; set; }
                  
              /// <summary>
              /// Average number of high goals scored.
              /// </summary>
              /// <value>Average number of high goals scored.</value>
                [JsonRequired]
                  [JsonPropertyName("average_high_goals")]
                  public float AverageHighGoals { get; set; }
                  
              /// <summary>
              /// Average number of low goals scored.
              /// </summary>
              /// <value>Average number of low goals scored.</value>
                [JsonRequired]
                  [JsonPropertyName("average_low_goals")]
                  public float AverageLowGoals { get; set; }
                  
              /// <summary>
              /// Average total score.
              /// </summary>
              /// <value>Average total score.</value>
                [JsonRequired]
                  [JsonPropertyName("average_score")]
                  public float AverageScore { get; set; }
                  
              /// <summary>
              /// Average tower score.
              /// </summary>
              /// <value>Average tower score.</value>
                [JsonRequired]
                  [JsonPropertyName("average_tower_score")]
                  public float AverageTowerScore { get; set; }
                  
              /// <summary>
              /// Average margin of victory.
              /// </summary>
              /// <value>Average margin of victory.</value>
                [JsonRequired]
                  [JsonPropertyName("average_win_margin")]
                  public float AverageWinMargin { get; set; }
                  
              /// <summary>
              /// Average winning score.
              /// </summary>
              /// <value>Average winning score.</value>
                [JsonRequired]
                  [JsonPropertyName("average_win_score")]
                  public float AverageWinScore { get; set; }
                  
              /// <summary>
              /// For the Moat - An array with three values, number of times damaged, number of opportunities to damage, and percentage.
              /// </summary>
              /// <value>For the Moat - An array with three values, number of times damaged, number of opportunities to damage, and percentage.</value>
                [JsonRequired]
                  [JsonPropertyName("B_Moat")]
                  public Collection<float> BMoat { get; set; }
                  
              /// <summary>
              /// For the Ramparts - An array with three values, number of times damaged, number of opportunities to damage, and percentage.
              /// </summary>
              /// <value>For the Ramparts - An array with three values, number of times damaged, number of opportunities to damage, and percentage.</value>
                [JsonRequired]
                  [JsonPropertyName("B_Ramparts")]
                  public Collection<float> BRamparts { get; set; }
                  
              /// <summary>
              /// An array with three values, number of times breached, number of opportunities to breach, and percentage.
              /// </summary>
              /// <value>An array with three values, number of times breached, number of opportunities to breach, and percentage.</value>
                [JsonRequired]
                  [JsonPropertyName("breaches")]
                  public Collection<float> Breaches { get; set; }
                  
              /// <summary>
              /// For the Drawbridge - An array with three values, number of times damaged, number of opportunities to damage, and percentage.
              /// </summary>
              /// <value>For the Drawbridge - An array with three values, number of times damaged, number of opportunities to damage, and percentage.</value>
                [JsonRequired]
                  [JsonPropertyName("C_Drawbridge")]
                  public Collection<float> CDrawbridge { get; set; }
                  
              /// <summary>
              /// For the Sally Port - An array with three values, number of times damaged, number of opportunities to damage, and percentage.
              /// </summary>
              /// <value>For the Sally Port - An array with three values, number of times damaged, number of opportunities to damage, and percentage.</value>
                [JsonRequired]
                  [JsonPropertyName("C_SallyPort")]
                  public Collection<float> CSallyPort { get; set; }
                  
              /// <summary>
              /// An array with three values, number of times captured, number of opportunities to capture, and percentage.
              /// </summary>
              /// <value>An array with three values, number of times captured, number of opportunities to capture, and percentage.</value>
                [JsonRequired]
                  [JsonPropertyName("captures")]
                  public Collection<float> Captures { get; set; }
                  
              /// <summary>
              /// An array with three values, number of times challenged, number of opportunities to challenge, and percentage.
              /// </summary>
              /// <value>An array with three values, number of times challenged, number of opportunities to challenge, and percentage.</value>
                [JsonRequired]
                  [JsonPropertyName("challenges")]
                  public Collection<float> Challenges { get; set; }
                  
              /// <summary>
              /// For the Rock Wall - An array with three values, number of times damaged, number of opportunities to damage, and percentage.
              /// </summary>
              /// <value>For the Rock Wall - An array with three values, number of times damaged, number of opportunities to damage, and percentage.</value>
                [JsonRequired]
                  [JsonPropertyName("D_RockWall")]
                  public Collection<float> DRockWall { get; set; }
                  
              /// <summary>
              /// For the Rough Terrain - An array with three values, number of times damaged, number of opportunities to damage, and percentage.
              /// </summary>
              /// <value>For the Rough Terrain - An array with three values, number of times damaged, number of opportunities to damage, and percentage.</value>
                [JsonRequired]
                  [JsonPropertyName("D_RoughTerrain")]
                  public Collection<float> DRoughTerrain { get; set; }
                  
              /// <summary>
              /// An array with three values, high score, match key from the match with the high score, and the name of the match.
              /// </summary>
              /// <value>An array with three values, high score, match key from the match with the high score, and the name of the match.</value>
                [JsonRequired]
                  [JsonPropertyName("high_score")]
                  public Collection<string> HighScore { get; set; }
                  
              /// <summary>
              /// For the Low Bar - An array with three values, number of times damaged, number of opportunities to damage, and percentage.
              /// </summary>
              /// <value>For the Low Bar - An array with three values, number of times damaged, number of opportunities to damage, and percentage.</value>
                [JsonRequired]
                  [JsonPropertyName("LowBar")]
                  public Collection<float> LowBar { get; set; }
                  
              /// <summary>
              /// An array with three values, number of times scaled, number of opportunities to scale, and percentage.
              /// </summary>
              /// <value>An array with three values, number of times scaled, number of opportunities to scale, and percentage.</value>
                [JsonRequired]
                  [JsonPropertyName("scales")]
                  public Collection<float> Scales { get; set; }
                  
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
