/*
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).
 *
 * The version of the OpenAPI document: 3.9.11
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

namespace TheBlueAlliance.Model;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Insights for FIRST STEAMWORKS qualification and elimination matches.
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public partial record EventInsights2017
{
    // yup
    /// <summary>
    /// Initializes a new instance of the <see cref="EventInsights2017" /> class.
    /// </summary>
    [JsonConstructor]
    protected EventInsights2017()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventInsights2017" /> class.
    /// </summary>
    /// <param name="averageFoulScore">Average foul score. (required).</param>
    /// <param name="averageFuelPoints">Average fuel points scored. (required).</param>
    /// <param name="averageFuelPointsAuto">Average fuel points scored during auto. (required).</param>
    /// <param name="averageFuelPointsTeleop">Average fuel points scored during teleop. (required).</param>
    /// <param name="averageHighGoals">Average points scored in the high goal. (required).</param>
    /// <param name="averageHighGoalsAuto">Average points scored in the high goal during auto. (required).</param>
    /// <param name="averageHighGoalsTeleop">Average points scored in the high goal during teleop. (required).</param>
    /// <param name="averageLowGoals">Average points scored in the low goal. (required).</param>
    /// <param name="averageLowGoalsAuto">Average points scored in the low goal during auto. (required).</param>
    /// <param name="averageLowGoalsTeleop">Average points scored in the low goal during teleop. (required).</param>
    /// <param name="averageMobilityPointsAuto">Average mobility points scored during auto. (required).</param>
    /// <param name="averagePointsAuto">Average points scored during auto. (required).</param>
    /// <param name="averagePointsTeleop">Average points scored during teleop. (required).</param>
    /// <param name="averageRotorPoints">Average rotor points scored. (required).</param>
    /// <param name="averageRotorPointsAuto">Average rotor points scored during auto. (required).</param>
    /// <param name="averageRotorPointsTeleop">Average rotor points scored during teleop. (required).</param>
    /// <param name="averageScore">Average score. (required).</param>
    /// <param name="averageTakeoffPointsTeleop">Average takeoff points scored during teleop. (required).</param>
    /// <param name="averageWinMargin">Average margin of victory. (required).</param>
    /// <param name="averageWinScore">Average winning score. (required).</param>
    /// <param name="highKpa">An array with three values, kPa scored, match key from the match with the high kPa, and the name of the match (required).</param>
    /// <param name="highScore">An array with three values, high score, match key from the match with the high score, and the name of the match (required).</param>
    /// <param name="kpaAchieved">An array with three values, number of times kPa bonus achieved, number of opportunities to bonus, and percentage. (required).</param>
    /// <param name="mobilityCounts">An array with three values, number of times mobility bonus achieved, number of opportunities to bonus, and percentage. (required).</param>
    /// <param name="rotor1Engaged">An array with three values, number of times rotor 1 engaged, number of opportunities to engage, and percentage. (required).</param>
    /// <param name="rotor1EngagedAuto">An array with three values, number of times rotor 1 engaged in auto, number of opportunities to engage in auto, and percentage. (required).</param>
    /// <param name="rotor2Engaged">An array with three values, number of times rotor 2 engaged, number of opportunities to engage, and percentage. (required).</param>
    /// <param name="rotor2EngagedAuto">An array with three values, number of times rotor 2 engaged in auto, number of opportunities to engage in auto, and percentage. (required).</param>
    /// <param name="rotor3Engaged">An array with three values, number of times rotor 3 engaged, number of opportunities to engage, and percentage. (required).</param>
    /// <param name="rotor4Engaged">An array with three values, number of times rotor 4 engaged, number of opportunities to engage, and percentage. (required).</param>
    /// <param name="takeoffCounts">An array with three values, number of times takeoff was counted, number of opportunities to takeoff, and percentage. (required).</param>
    /// <param name="unicornMatches">An array with three values, number of times a unicorn match (Win + kPa &amp; Rotor Bonuses) occured, number of opportunities to have a unicorn match, and percentage. (required).</param>
    public EventInsights2017(float averageFoulScore, float averageFuelPoints, float averageFuelPointsAuto, float averageFuelPointsTeleop, float averageHighGoals, float averageHighGoalsAuto, float averageHighGoalsTeleop, float averageLowGoals, float averageLowGoalsAuto, float averageLowGoalsTeleop, float averageMobilityPointsAuto, float averagePointsAuto, float averagePointsTeleop, float averageRotorPoints, float averageRotorPointsAuto, float averageRotorPointsTeleop, float averageScore, float averageTakeoffPointsTeleop, float averageWinMargin, float averageWinScore, Collection<string> highKpa, Collection<string> highScore, Collection<float> kpaAchieved, Collection<float> mobilityCounts, Collection<float> rotor1Engaged, Collection<float> rotor1EngagedAuto, Collection<float> rotor2Engaged, Collection<float> rotor2EngagedAuto, Collection<float> rotor3Engaged, Collection<float> rotor4Engaged, Collection<float> takeoffCounts, Collection<float> unicornMatches)
    {
        this.AverageFoulScore = averageFoulScore;
        this.AverageFuelPoints = averageFuelPoints;
        this.AverageFuelPointsAuto = averageFuelPointsAuto;
        this.AverageFuelPointsTeleop = averageFuelPointsTeleop;
        this.AverageHighGoals = averageHighGoals;
        this.AverageHighGoalsAuto = averageHighGoalsAuto;
        this.AverageHighGoalsTeleop = averageHighGoalsTeleop;
        this.AverageLowGoals = averageLowGoals;
        this.AverageLowGoalsAuto = averageLowGoalsAuto;
        this.AverageLowGoalsTeleop = averageLowGoalsTeleop;
        this.AverageMobilityPointsAuto = averageMobilityPointsAuto;
        this.AveragePointsAuto = averagePointsAuto;
        this.AveragePointsTeleop = averagePointsTeleop;
        this.AverageRotorPoints = averageRotorPoints;
        this.AverageRotorPointsAuto = averageRotorPointsAuto;
        this.AverageRotorPointsTeleop = averageRotorPointsTeleop;
        this.AverageScore = averageScore;
        this.AverageTakeoffPointsTeleop = averageTakeoffPointsTeleop;
        this.AverageWinMargin = averageWinMargin;
        this.AverageWinScore = averageWinScore;
        // to ensure "highKpa" is required (not null)
        ArgumentNullException.ThrowIfNull(highKpa);
        this.HighKpa = highKpa;

        // to ensure "highScore" is required (not null)
        ArgumentNullException.ThrowIfNull(highScore);
        this.HighScore = highScore;

        // to ensure "kpaAchieved" is required (not null)
        ArgumentNullException.ThrowIfNull(kpaAchieved);
        this.KpaAchieved = kpaAchieved;

        // to ensure "mobilityCounts" is required (not null)
        ArgumentNullException.ThrowIfNull(mobilityCounts);
        this.MobilityCounts = mobilityCounts;

        // to ensure "rotor1Engaged" is required (not null)
        ArgumentNullException.ThrowIfNull(rotor1Engaged);
        this.Rotor1Engaged = rotor1Engaged;

        // to ensure "rotor1EngagedAuto" is required (not null)
        ArgumentNullException.ThrowIfNull(rotor1EngagedAuto);
        this.Rotor1EngagedAuto = rotor1EngagedAuto;

        // to ensure "rotor2Engaged" is required (not null)
        ArgumentNullException.ThrowIfNull(rotor2Engaged);
        this.Rotor2Engaged = rotor2Engaged;

        // to ensure "rotor2EngagedAuto" is required (not null)
        ArgumentNullException.ThrowIfNull(rotor2EngagedAuto);
        this.Rotor2EngagedAuto = rotor2EngagedAuto;

        // to ensure "rotor3Engaged" is required (not null)
        ArgumentNullException.ThrowIfNull(rotor3Engaged);
        this.Rotor3Engaged = rotor3Engaged;

        // to ensure "rotor4Engaged" is required (not null)
        ArgumentNullException.ThrowIfNull(rotor4Engaged);
        this.Rotor4Engaged = rotor4Engaged;

        // to ensure "takeoffCounts" is required (not null)
        ArgumentNullException.ThrowIfNull(takeoffCounts);
        this.TakeoffCounts = takeoffCounts;

        // to ensure "unicornMatches" is required (not null)
        ArgumentNullException.ThrowIfNull(unicornMatches);
        this.UnicornMatches = unicornMatches;
    }

    /// <summary>
    /// Average foul score.
    /// </summary>
    /// <value>Average foul score.</value>
    [JsonRequired]
    [JsonPropertyName("average_foul_score")]
    public float AverageFoulScore { get; set; }

    /// <summary>
    /// Average fuel points scored.
    /// </summary>
    /// <value>Average fuel points scored.</value>
    [JsonRequired]
    [JsonPropertyName("average_fuel_points")]
    public float AverageFuelPoints { get; set; }

    /// <summary>
    /// Average fuel points scored during auto.
    /// </summary>
    /// <value>Average fuel points scored during auto.</value>
    [JsonRequired]
    [JsonPropertyName("average_fuel_points_auto")]
    public float AverageFuelPointsAuto { get; set; }

    /// <summary>
    /// Average fuel points scored during teleop.
    /// </summary>
    /// <value>Average fuel points scored during teleop.</value>
    [JsonRequired]
    [JsonPropertyName("average_fuel_points_teleop")]
    public float AverageFuelPointsTeleop { get; set; }

    /// <summary>
    /// Average points scored in the high goal.
    /// </summary>
    /// <value>Average points scored in the high goal.</value>
    [JsonRequired]
    [JsonPropertyName("average_high_goals")]
    public float AverageHighGoals { get; set; }

    /// <summary>
    /// Average points scored in the high goal during auto.
    /// </summary>
    /// <value>Average points scored in the high goal during auto.</value>
    [JsonRequired]
    [JsonPropertyName("average_high_goals_auto")]
    public float AverageHighGoalsAuto { get; set; }

    /// <summary>
    /// Average points scored in the high goal during teleop.
    /// </summary>
    /// <value>Average points scored in the high goal during teleop.</value>
    [JsonRequired]
    [JsonPropertyName("average_high_goals_teleop")]
    public float AverageHighGoalsTeleop { get; set; }

    /// <summary>
    /// Average points scored in the low goal.
    /// </summary>
    /// <value>Average points scored in the low goal.</value>
    [JsonRequired]
    [JsonPropertyName("average_low_goals")]
    public float AverageLowGoals { get; set; }

    /// <summary>
    /// Average points scored in the low goal during auto.
    /// </summary>
    /// <value>Average points scored in the low goal during auto.</value>
    [JsonRequired]
    [JsonPropertyName("average_low_goals_auto")]
    public float AverageLowGoalsAuto { get; set; }

    /// <summary>
    /// Average points scored in the low goal during teleop.
    /// </summary>
    /// <value>Average points scored in the low goal during teleop.</value>
    [JsonRequired]
    [JsonPropertyName("average_low_goals_teleop")]
    public float AverageLowGoalsTeleop { get; set; }

    /// <summary>
    /// Average mobility points scored during auto.
    /// </summary>
    /// <value>Average mobility points scored during auto.</value>
    [JsonRequired]
    [JsonPropertyName("average_mobility_points_auto")]
    public float AverageMobilityPointsAuto { get; set; }

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
    /// Average rotor points scored.
    /// </summary>
    /// <value>Average rotor points scored.</value>
    [JsonRequired]
    [JsonPropertyName("average_rotor_points")]
    public float AverageRotorPoints { get; set; }

    /// <summary>
    /// Average rotor points scored during auto.
    /// </summary>
    /// <value>Average rotor points scored during auto.</value>
    [JsonRequired]
    [JsonPropertyName("average_rotor_points_auto")]
    public float AverageRotorPointsAuto { get; set; }

    /// <summary>
    /// Average rotor points scored during teleop.
    /// </summary>
    /// <value>Average rotor points scored during teleop.</value>
    [JsonRequired]
    [JsonPropertyName("average_rotor_points_teleop")]
    public float AverageRotorPointsTeleop { get; set; }

    /// <summary>
    /// Average score.
    /// </summary>
    /// <value>Average score.</value>
    [JsonRequired]
    [JsonPropertyName("average_score")]
    public float AverageScore { get; set; }

    /// <summary>
    /// Average takeoff points scored during teleop.
    /// </summary>
    /// <value>Average takeoff points scored during teleop.</value>
    [JsonRequired]
    [JsonPropertyName("average_takeoff_points_teleop")]
    public float AverageTakeoffPointsTeleop { get; set; }

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
    /// An array with three values, kPa scored, match key from the match with the high kPa, and the name of the match
    /// </summary>
    /// <value>An array with three values, kPa scored, match key from the match with the high kPa, and the name of the match</value>
    [JsonRequired]
    [JsonPropertyName("high_kpa")]
    public Collection<string> HighKpa { get; set; }

    /// <summary>
    /// An array with three values, high score, match key from the match with the high score, and the name of the match
    /// </summary>
    /// <value>An array with three values, high score, match key from the match with the high score, and the name of the match</value>
    [JsonRequired]
    [JsonPropertyName("high_score")]
    public Collection<string> HighScore { get; set; }

    /// <summary>
    /// An array with three values, number of times kPa bonus achieved, number of opportunities to bonus, and percentage.
    /// </summary>
    /// <value>An array with three values, number of times kPa bonus achieved, number of opportunities to bonus, and percentage.</value>
    [JsonRequired]
    [JsonPropertyName("kpa_achieved")]
    public Collection<float> KpaAchieved { get; set; }

    /// <summary>
    /// An array with three values, number of times mobility bonus achieved, number of opportunities to bonus, and percentage.
    /// </summary>
    /// <value>An array with three values, number of times mobility bonus achieved, number of opportunities to bonus, and percentage.</value>
    [JsonRequired]
    [JsonPropertyName("mobility_counts")]
    public Collection<float> MobilityCounts { get; set; }

    /// <summary>
    /// An array with three values, number of times rotor 1 engaged, number of opportunities to engage, and percentage.
    /// </summary>
    /// <value>An array with three values, number of times rotor 1 engaged, number of opportunities to engage, and percentage.</value>
    [JsonRequired]
    [JsonPropertyName("rotor_1_engaged")]
    public Collection<float> Rotor1Engaged { get; set; }

    /// <summary>
    /// An array with three values, number of times rotor 1 engaged in auto, number of opportunities to engage in auto, and percentage.
    /// </summary>
    /// <value>An array with three values, number of times rotor 1 engaged in auto, number of opportunities to engage in auto, and percentage.</value>
    [JsonRequired]
    [JsonPropertyName("rotor_1_engaged_auto")]
    public Collection<float> Rotor1EngagedAuto { get; set; }

    /// <summary>
    /// An array with three values, number of times rotor 2 engaged, number of opportunities to engage, and percentage.
    /// </summary>
    /// <value>An array with three values, number of times rotor 2 engaged, number of opportunities to engage, and percentage.</value>
    [JsonRequired]
    [JsonPropertyName("rotor_2_engaged")]
    public Collection<float> Rotor2Engaged { get; set; }

    /// <summary>
    /// An array with three values, number of times rotor 2 engaged in auto, number of opportunities to engage in auto, and percentage.
    /// </summary>
    /// <value>An array with three values, number of times rotor 2 engaged in auto, number of opportunities to engage in auto, and percentage.</value>
    [JsonRequired]
    [JsonPropertyName("rotor_2_engaged_auto")]
    public Collection<float> Rotor2EngagedAuto { get; set; }

    /// <summary>
    /// An array with three values, number of times rotor 3 engaged, number of opportunities to engage, and percentage.
    /// </summary>
    /// <value>An array with three values, number of times rotor 3 engaged, number of opportunities to engage, and percentage.</value>
    [JsonRequired]
    [JsonPropertyName("rotor_3_engaged")]
    public Collection<float> Rotor3Engaged { get; set; }

    /// <summary>
    /// An array with three values, number of times rotor 4 engaged, number of opportunities to engage, and percentage.
    /// </summary>
    /// <value>An array with three values, number of times rotor 4 engaged, number of opportunities to engage, and percentage.</value>
    [JsonRequired]
    [JsonPropertyName("rotor_4_engaged")]
    public Collection<float> Rotor4Engaged { get; set; }

    /// <summary>
    /// An array with three values, number of times takeoff was counted, number of opportunities to takeoff, and percentage.
    /// </summary>
    /// <value>An array with three values, number of times takeoff was counted, number of opportunities to takeoff, and percentage.</value>
    [JsonRequired]
    [JsonPropertyName("takeoff_counts")]
    public Collection<float> TakeoffCounts { get; set; }

    /// <summary>
    /// An array with three values, number of times a unicorn match (Win + kPa &amp; Rotor Bonuses) occured, number of opportunities to have a unicorn match, and percentage.
    /// </summary>
    /// <value>An array with three values, number of times a unicorn match (Win + kPa &amp; Rotor Bonuses) occured, number of opportunities to have a unicorn match, and percentage.</value>
    [JsonRequired]
    [JsonPropertyName("unicorn_matches")]
    public Collection<float> UnicornMatches { get; set; }

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
