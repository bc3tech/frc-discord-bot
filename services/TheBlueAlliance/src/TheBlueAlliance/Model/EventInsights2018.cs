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
/// Insights for FIRST Power Up qualification and elimination matches.
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public partial record EventInsights2018
  {
              // yup
              /// <summary>
              /// Initializes a new instance of the <see cref="EventInsights2018" /> class.
              /// </summary>
              [JsonConstructor]
              protected EventInsights2018() { 
            }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="EventInsights2018" /> class.
        /// </summary>
            /// <param name="autoQuestAchieved">An array with three values, number of times auto quest was completed, number of opportunities to complete the auto quest, and percentage. (required).</param>
            /// <param name="averageBoostPlayed">Average number of boost power up scored (out of 3). (required).</param>
            /// <param name="averageEndgamePoints">Average endgame points. (required).</param>
            /// <param name="averageForcePlayed">Average number of force power up scored (out of 3). (required).</param>
            /// <param name="averageFoulScore">Average foul score. (required).</param>
            /// <param name="averagePointsAuto">Average points scored during auto. (required).</param>
            /// <param name="averagePointsTeleop">Average points scored during teleop. (required).</param>
            /// <param name="averageRunPointsAuto">Average mobility points scored during auto. (required).</param>
            /// <param name="averageScaleOwnershipPoints">Average scale ownership points scored. (required).</param>
            /// <param name="averageScaleOwnershipPointsAuto">Average scale ownership points scored during auto. (required).</param>
            /// <param name="averageScaleOwnershipPointsTeleop">Average scale ownership points scored during teleop. (required).</param>
            /// <param name="averageScore">Average score. (required).</param>
            /// <param name="averageSwitchOwnershipPoints">Average switch ownership points scored. (required).</param>
            /// <param name="averageSwitchOwnershipPointsAuto">Average switch ownership points scored during auto. (required).</param>
            /// <param name="averageSwitchOwnershipPointsTeleop">Average switch ownership points scored during teleop. (required).</param>
            /// <param name="averageVaultPoints">Average value points scored. (required).</param>
            /// <param name="averageWinMargin">Average margin of victory. (required).</param>
            /// <param name="averageWinScore">Average winning score. (required).</param>
            /// <param name="boostPlayedCounts">An array with three values, number of times a boost power up was played, number of opportunities to play a boost power up, and percentage. (required).</param>
            /// <param name="climbCounts">An array with three values, number of times a climb occurred, number of opportunities to climb, and percentage. (required).</param>
            /// <param name="faceTheBossAchieved">An array with three values, number of times an alliance faced the boss, number of opportunities to face the boss, and percentage. (required).</param>
            /// <param name="forcePlayedCounts">An array with three values, number of times a force power up was played, number of opportunities to play a force power up, and percentage. (required).</param>
            /// <param name="highScore">An array with three values, high score, match key from the match with the high score, and the name of the match (required).</param>
            /// <param name="levitatePlayedCounts">An array with three values, number of times a levitate power up was played, number of opportunities to play a levitate power up, and percentage. (required).</param>
            /// <param name="runCountsAuto">An array with three values, number of times a team scored mobility points in auto, number of opportunities to score mobility points in auto, and percentage. (required).</param>
            /// <param name="scaleNeutralPercentage">Average scale neutral percentage. (required).</param>
            /// <param name="scaleNeutralPercentageAuto">Average scale neutral percentage during auto. (required).</param>
            /// <param name="scaleNeutralPercentageTeleop">Average scale neutral percentage during teleop. (required).</param>
            /// <param name="switchOwnedCountsAuto">An array with three values, number of times a switch was owned during auto, number of opportunities to own a switch during auto, and percentage. (required).</param>
            /// <param name="unicornMatches">An array with three values, number of times a unicorn match (Win + Auto Quest + Face the Boss) occurred, number of opportunities to have a unicorn match, and percentage. (required).</param>
            /// <param name="winningOppSwitchDenialPercentageTeleop">Average opposing switch denail percentage for the winning alliance during teleop. (required).</param>
            /// <param name="winningOwnSwitchOwnershipPercentage">Average own switch ownership percentage for the winning alliance. (required).</param>
            /// <param name="winningOwnSwitchOwnershipPercentageAuto">Average own switch ownership percentage for the winning alliance during auto. (required).</param>
            /// <param name="winningOwnSwitchOwnershipPercentageTeleop">Average own switch ownership percentage for the winning alliance during teleop. (required).</param>
            /// <param name="winningScaleOwnershipPercentage">Average scale ownership percentage for the winning alliance. (required).</param>
            /// <param name="winningScaleOwnershipPercentageAuto">Average scale ownership percentage for the winning alliance during auto. (required).</param>
            /// <param name="winningScaleOwnershipPercentageTeleop">Average scale ownership percentage for the winning alliance during teleop. (required).</param>
        public EventInsights2018(Collection<float> autoQuestAchieved, float averageBoostPlayed, float averageEndgamePoints, float averageForcePlayed, float averageFoulScore, float averagePointsAuto, float averagePointsTeleop, float averageRunPointsAuto, float averageScaleOwnershipPoints, float averageScaleOwnershipPointsAuto, float averageScaleOwnershipPointsTeleop, float averageScore, float averageSwitchOwnershipPoints, float averageSwitchOwnershipPointsAuto, float averageSwitchOwnershipPointsTeleop, float averageVaultPoints, float averageWinMargin, float averageWinScore, Collection<float> boostPlayedCounts, Collection<float> climbCounts, Collection<float> faceTheBossAchieved, Collection<float> forcePlayedCounts, Collection<string> highScore, Collection<float> levitatePlayedCounts, Collection<float> runCountsAuto, float scaleNeutralPercentage, float scaleNeutralPercentageAuto, float scaleNeutralPercentageTeleop, Collection<float> switchOwnedCountsAuto, Collection<float> unicornMatches, float winningOppSwitchDenialPercentageTeleop, float winningOwnSwitchOwnershipPercentage, float winningOwnSwitchOwnershipPercentageAuto, float winningOwnSwitchOwnershipPercentageTeleop, float winningScaleOwnershipPercentage, float winningScaleOwnershipPercentageAuto, float winningScaleOwnershipPercentageTeleop)
        {
                      // to ensure "autoQuestAchieved" is required (not null)
                      ArgumentNullException.ThrowIfNull(autoQuestAchieved);
                      this.AutoQuestAchieved = autoQuestAchieved;
                        
                      this.AverageBoostPlayed = averageBoostPlayed;
                      this.AverageEndgamePoints = averageEndgamePoints;
                      this.AverageForcePlayed = averageForcePlayed;
                      this.AverageFoulScore = averageFoulScore;
                      this.AveragePointsAuto = averagePointsAuto;
                      this.AveragePointsTeleop = averagePointsTeleop;
                      this.AverageRunPointsAuto = averageRunPointsAuto;
                      this.AverageScaleOwnershipPoints = averageScaleOwnershipPoints;
                      this.AverageScaleOwnershipPointsAuto = averageScaleOwnershipPointsAuto;
                      this.AverageScaleOwnershipPointsTeleop = averageScaleOwnershipPointsTeleop;
                      this.AverageScore = averageScore;
                      this.AverageSwitchOwnershipPoints = averageSwitchOwnershipPoints;
                      this.AverageSwitchOwnershipPointsAuto = averageSwitchOwnershipPointsAuto;
                      this.AverageSwitchOwnershipPointsTeleop = averageSwitchOwnershipPointsTeleop;
                      this.AverageVaultPoints = averageVaultPoints;
                      this.AverageWinMargin = averageWinMargin;
                      this.AverageWinScore = averageWinScore;
                      // to ensure "boostPlayedCounts" is required (not null)
                      ArgumentNullException.ThrowIfNull(boostPlayedCounts);
                      this.BoostPlayedCounts = boostPlayedCounts;
                        
                      // to ensure "climbCounts" is required (not null)
                      ArgumentNullException.ThrowIfNull(climbCounts);
                      this.ClimbCounts = climbCounts;
                        
                      // to ensure "faceTheBossAchieved" is required (not null)
                      ArgumentNullException.ThrowIfNull(faceTheBossAchieved);
                      this.FaceTheBossAchieved = faceTheBossAchieved;
                        
                      // to ensure "forcePlayedCounts" is required (not null)
                      ArgumentNullException.ThrowIfNull(forcePlayedCounts);
                      this.ForcePlayedCounts = forcePlayedCounts;
                        
                      // to ensure "highScore" is required (not null)
                      ArgumentNullException.ThrowIfNull(highScore);
                      this.HighScore = highScore;
                        
                      // to ensure "levitatePlayedCounts" is required (not null)
                      ArgumentNullException.ThrowIfNull(levitatePlayedCounts);
                      this.LevitatePlayedCounts = levitatePlayedCounts;
                        
                      // to ensure "runCountsAuto" is required (not null)
                      ArgumentNullException.ThrowIfNull(runCountsAuto);
                      this.RunCountsAuto = runCountsAuto;
                        
                      this.ScaleNeutralPercentage = scaleNeutralPercentage;
                      this.ScaleNeutralPercentageAuto = scaleNeutralPercentageAuto;
                      this.ScaleNeutralPercentageTeleop = scaleNeutralPercentageTeleop;
                      // to ensure "switchOwnedCountsAuto" is required (not null)
                      ArgumentNullException.ThrowIfNull(switchOwnedCountsAuto);
                      this.SwitchOwnedCountsAuto = switchOwnedCountsAuto;
                        
                      // to ensure "unicornMatches" is required (not null)
                      ArgumentNullException.ThrowIfNull(unicornMatches);
                      this.UnicornMatches = unicornMatches;
                        
                      this.WinningOppSwitchDenialPercentageTeleop = winningOppSwitchDenialPercentageTeleop;
                      this.WinningOwnSwitchOwnershipPercentage = winningOwnSwitchOwnershipPercentage;
                      this.WinningOwnSwitchOwnershipPercentageAuto = winningOwnSwitchOwnershipPercentageAuto;
                      this.WinningOwnSwitchOwnershipPercentageTeleop = winningOwnSwitchOwnershipPercentageTeleop;
                      this.WinningScaleOwnershipPercentage = winningScaleOwnershipPercentage;
                      this.WinningScaleOwnershipPercentageAuto = winningScaleOwnershipPercentageAuto;
                      this.WinningScaleOwnershipPercentageTeleop = winningScaleOwnershipPercentageTeleop;
        }
        
              /// <summary>
              /// An array with three values, number of times auto quest was completed, number of opportunities to complete the auto quest, and percentage.
              /// </summary>
              /// <value>An array with three values, number of times auto quest was completed, number of opportunities to complete the auto quest, and percentage.</value>
                [JsonRequired]
                  [JsonPropertyName("auto_quest_achieved")]
                  public Collection<float> AutoQuestAchieved { get; set; }
                  
              /// <summary>
              /// Average number of boost power up scored (out of 3).
              /// </summary>
              /// <value>Average number of boost power up scored (out of 3).</value>
                [JsonRequired]
                  [JsonPropertyName("average_boost_played")]
                  public float AverageBoostPlayed { get; set; }
                  
              /// <summary>
              /// Average endgame points.
              /// </summary>
              /// <value>Average endgame points.</value>
                [JsonRequired]
                  [JsonPropertyName("average_endgame_points")]
                  public float AverageEndgamePoints { get; set; }
                  
              /// <summary>
              /// Average number of force power up scored (out of 3).
              /// </summary>
              /// <value>Average number of force power up scored (out of 3).</value>
                [JsonRequired]
                  [JsonPropertyName("average_force_played")]
                  public float AverageForcePlayed { get; set; }
                  
              /// <summary>
              /// Average foul score.
              /// </summary>
              /// <value>Average foul score.</value>
                [JsonRequired]
                  [JsonPropertyName("average_foul_score")]
                  public float AverageFoulScore { get; set; }
                  
              /// <summary>
              /// Average points scored during auto.
              /// </summary>
              /// <value>Average points scored during auto.</value>
                [JsonRequired]
                  [JsonPropertyName("average_points_auto")]
                  public float AveragePointsAuto { get; set; }
                  
              /// <summary>
              /// Average points scored during teleop.
              /// </summary>
              /// <value>Average points scored during teleop.</value>
                [JsonRequired]
                  [JsonPropertyName("average_points_teleop")]
                  public float AveragePointsTeleop { get; set; }
                  
              /// <summary>
              /// Average mobility points scored during auto.
              /// </summary>
              /// <value>Average mobility points scored during auto.</value>
                [JsonRequired]
                  [JsonPropertyName("average_run_points_auto")]
                  public float AverageRunPointsAuto { get; set; }
                  
              /// <summary>
              /// Average scale ownership points scored.
              /// </summary>
              /// <value>Average scale ownership points scored.</value>
                [JsonRequired]
                  [JsonPropertyName("average_scale_ownership_points")]
                  public float AverageScaleOwnershipPoints { get; set; }
                  
              /// <summary>
              /// Average scale ownership points scored during auto.
              /// </summary>
              /// <value>Average scale ownership points scored during auto.</value>
                [JsonRequired]
                  [JsonPropertyName("average_scale_ownership_points_auto")]
                  public float AverageScaleOwnershipPointsAuto { get; set; }
                  
              /// <summary>
              /// Average scale ownership points scored during teleop.
              /// </summary>
              /// <value>Average scale ownership points scored during teleop.</value>
                [JsonRequired]
                  [JsonPropertyName("average_scale_ownership_points_teleop")]
                  public float AverageScaleOwnershipPointsTeleop { get; set; }
                  
              /// <summary>
              /// Average score.
              /// </summary>
              /// <value>Average score.</value>
                [JsonRequired]
                  [JsonPropertyName("average_score")]
                  public float AverageScore { get; set; }
                  
              /// <summary>
              /// Average switch ownership points scored.
              /// </summary>
              /// <value>Average switch ownership points scored.</value>
                [JsonRequired]
                  [JsonPropertyName("average_switch_ownership_points")]
                  public float AverageSwitchOwnershipPoints { get; set; }
                  
              /// <summary>
              /// Average switch ownership points scored during auto.
              /// </summary>
              /// <value>Average switch ownership points scored during auto.</value>
                [JsonRequired]
                  [JsonPropertyName("average_switch_ownership_points_auto")]
                  public float AverageSwitchOwnershipPointsAuto { get; set; }
                  
              /// <summary>
              /// Average switch ownership points scored during teleop.
              /// </summary>
              /// <value>Average switch ownership points scored during teleop.</value>
                [JsonRequired]
                  [JsonPropertyName("average_switch_ownership_points_teleop")]
                  public float AverageSwitchOwnershipPointsTeleop { get; set; }
                  
              /// <summary>
              /// Average value points scored.
              /// </summary>
              /// <value>Average value points scored.</value>
                [JsonRequired]
                  [JsonPropertyName("average_vault_points")]
                  public float AverageVaultPoints { get; set; }
                  
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
              /// An array with three values, number of times a boost power up was played, number of opportunities to play a boost power up, and percentage.
              /// </summary>
              /// <value>An array with three values, number of times a boost power up was played, number of opportunities to play a boost power up, and percentage.</value>
                [JsonRequired]
                  [JsonPropertyName("boost_played_counts")]
                  public Collection<float> BoostPlayedCounts { get; set; }
                  
              /// <summary>
              /// An array with three values, number of times a climb occurred, number of opportunities to climb, and percentage.
              /// </summary>
              /// <value>An array with three values, number of times a climb occurred, number of opportunities to climb, and percentage.</value>
                [JsonRequired]
                  [JsonPropertyName("climb_counts")]
                  public Collection<float> ClimbCounts { get; set; }
                  
              /// <summary>
              /// An array with three values, number of times an alliance faced the boss, number of opportunities to face the boss, and percentage.
              /// </summary>
              /// <value>An array with three values, number of times an alliance faced the boss, number of opportunities to face the boss, and percentage.</value>
                [JsonRequired]
                  [JsonPropertyName("face_the_boss_achieved")]
                  public Collection<float> FaceTheBossAchieved { get; set; }
                  
              /// <summary>
              /// An array with three values, number of times a force power up was played, number of opportunities to play a force power up, and percentage.
              /// </summary>
              /// <value>An array with three values, number of times a force power up was played, number of opportunities to play a force power up, and percentage.</value>
                [JsonRequired]
                  [JsonPropertyName("force_played_counts")]
                  public Collection<float> ForcePlayedCounts { get; set; }
                  
              /// <summary>
              /// An array with three values, high score, match key from the match with the high score, and the name of the match
              /// </summary>
              /// <value>An array with three values, high score, match key from the match with the high score, and the name of the match</value>
                [JsonRequired]
                  [JsonPropertyName("high_score")]
                  public Collection<string> HighScore { get; set; }
                  
              /// <summary>
              /// An array with three values, number of times a levitate power up was played, number of opportunities to play a levitate power up, and percentage.
              /// </summary>
              /// <value>An array with three values, number of times a levitate power up was played, number of opportunities to play a levitate power up, and percentage.</value>
                [JsonRequired]
                  [JsonPropertyName("levitate_played_counts")]
                  public Collection<float> LevitatePlayedCounts { get; set; }
                  
              /// <summary>
              /// An array with three values, number of times a team scored mobility points in auto, number of opportunities to score mobility points in auto, and percentage.
              /// </summary>
              /// <value>An array with three values, number of times a team scored mobility points in auto, number of opportunities to score mobility points in auto, and percentage.</value>
                [JsonRequired]
                  [JsonPropertyName("run_counts_auto")]
                  public Collection<float> RunCountsAuto { get; set; }
                  
              /// <summary>
              /// Average scale neutral percentage.
              /// </summary>
              /// <value>Average scale neutral percentage.</value>
                [JsonRequired]
                  [JsonPropertyName("scale_neutral_percentage")]
                  public float ScaleNeutralPercentage { get; set; }
                  
              /// <summary>
              /// Average scale neutral percentage during auto.
              /// </summary>
              /// <value>Average scale neutral percentage during auto.</value>
                [JsonRequired]
                  [JsonPropertyName("scale_neutral_percentage_auto")]
                  public float ScaleNeutralPercentageAuto { get; set; }
                  
              /// <summary>
              /// Average scale neutral percentage during teleop.
              /// </summary>
              /// <value>Average scale neutral percentage during teleop.</value>
                [JsonRequired]
                  [JsonPropertyName("scale_neutral_percentage_teleop")]
                  public float ScaleNeutralPercentageTeleop { get; set; }
                  
              /// <summary>
              /// An array with three values, number of times a switch was owned during auto, number of opportunities to own a switch during auto, and percentage.
              /// </summary>
              /// <value>An array with three values, number of times a switch was owned during auto, number of opportunities to own a switch during auto, and percentage.</value>
                [JsonRequired]
                  [JsonPropertyName("switch_owned_counts_auto")]
                  public Collection<float> SwitchOwnedCountsAuto { get; set; }
                  
              /// <summary>
              /// An array with three values, number of times a unicorn match (Win + Auto Quest + Face the Boss) occurred, number of opportunities to have a unicorn match, and percentage.
              /// </summary>
              /// <value>An array with three values, number of times a unicorn match (Win + Auto Quest + Face the Boss) occurred, number of opportunities to have a unicorn match, and percentage.</value>
                [JsonRequired]
                  [JsonPropertyName("unicorn_matches")]
                  public Collection<float> UnicornMatches { get; set; }
                  
              /// <summary>
              /// Average opposing switch denail percentage for the winning alliance during teleop.
              /// </summary>
              /// <value>Average opposing switch denail percentage for the winning alliance during teleop.</value>
                [JsonRequired]
                  [JsonPropertyName("winning_opp_switch_denial_percentage_teleop")]
                  public float WinningOppSwitchDenialPercentageTeleop { get; set; }
                  
              /// <summary>
              /// Average own switch ownership percentage for the winning alliance.
              /// </summary>
              /// <value>Average own switch ownership percentage for the winning alliance.</value>
                [JsonRequired]
                  [JsonPropertyName("winning_own_switch_ownership_percentage")]
                  public float WinningOwnSwitchOwnershipPercentage { get; set; }
                  
              /// <summary>
              /// Average own switch ownership percentage for the winning alliance during auto.
              /// </summary>
              /// <value>Average own switch ownership percentage for the winning alliance during auto.</value>
                [JsonRequired]
                  [JsonPropertyName("winning_own_switch_ownership_percentage_auto")]
                  public float WinningOwnSwitchOwnershipPercentageAuto { get; set; }
                  
              /// <summary>
              /// Average own switch ownership percentage for the winning alliance during teleop.
              /// </summary>
              /// <value>Average own switch ownership percentage for the winning alliance during teleop.</value>
                [JsonRequired]
                  [JsonPropertyName("winning_own_switch_ownership_percentage_teleop")]
                  public float WinningOwnSwitchOwnershipPercentageTeleop { get; set; }
                  
              /// <summary>
              /// Average scale ownership percentage for the winning alliance.
              /// </summary>
              /// <value>Average scale ownership percentage for the winning alliance.</value>
                [JsonRequired]
                  [JsonPropertyName("winning_scale_ownership_percentage")]
                  public float WinningScaleOwnershipPercentage { get; set; }
                  
              /// <summary>
              /// Average scale ownership percentage for the winning alliance during auto.
              /// </summary>
              /// <value>Average scale ownership percentage for the winning alliance during auto.</value>
                [JsonRequired]
                  [JsonPropertyName("winning_scale_ownership_percentage_auto")]
                  public float WinningScaleOwnershipPercentageAuto { get; set; }
                  
              /// <summary>
              /// Average scale ownership percentage for the winning alliance during teleop.
              /// </summary>
              /// <value>Average scale ownership percentage for the winning alliance during teleop.</value>
                [JsonRequired]
                  [JsonPropertyName("winning_scale_ownership_percentage_teleop")]
                  public float WinningScaleOwnershipPercentageTeleop { get; set; }
                  
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
