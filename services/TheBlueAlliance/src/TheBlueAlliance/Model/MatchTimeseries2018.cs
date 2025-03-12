/*
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).
 *
 * The version of the OpenAPI document: 3.9.11
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

namespace TheBlueAlliance.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Timeseries data for the 2018 game *FIRST* POWER UP. *WARNING:* This is *not* official data, and is subject to a significant possibility of error, or missing data. Do not rely on this data for any purpose. In fact, pretend we made it up. *WARNING:* This model is currently under active development and may change at any time, including in breaking ways.
/// </summary>

public partial record MatchTimeseries2018
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MatchTimeseries2018" /> class.
    /// </summary>
    /// <param name="blueAutoQuest">1 if the blue alliance is credited with the AUTO QUEST, 0 if not.</param>
    /// <param name="blueBoostCount">Number of POWER CUBES in the BOOST section of the blue alliance VAULT.</param>
    /// <param name="blueBoostPlayed">Returns 1 if the blue alliance BOOST was played, or 0 if not played.</param>
    /// <param name="blueCurrentPowerup">Name of the current blue alliance POWER UP being played, or &#x60;null&#x60;.</param>
    /// <param name="blueFaceTheBoss">1 if the blue alliance is credited with FACING THE BOSS, 0 if not.</param>
    /// <param name="blueForceCount">Number of POWER CUBES in the FORCE section of the blue alliance VAULT.</param>
    /// <param name="blueForcePlayed">Returns 1 if the blue alliance FORCE was played, or 0 if not played.</param>
    /// <param name="blueLevitateCount">Number of POWER CUBES in the LEVITATE section of the blue alliance VAULT.</param>
    /// <param name="blueLevitatePlayed">Returns 1 if the blue alliance LEVITATE was played, or 0 if not played.</param>
    /// <param name="bluePowerupTimeRemaining">Number of seconds remaining in the blue alliance POWER UP time, or 0 if none is active.</param>
    /// <param name="blueScaleOwned">1 if the blue alliance owns the SCALE, 0 if not.</param>
    /// <param name="blueScore">Current score for the blue alliance.</param>
    /// <param name="blueSwitchOwned">1 if the blue alliance owns their SWITCH, 0 if not.</param>
    /// <param name="eventKey">TBA event key with the format yyyy[EVENT_CODE], where yyyy is the year, and EVENT_CODE is the event code of the event.</param>
    /// <param name="matchId">Match ID consisting of the level, match number, and set number, eg &#x60;qm45&#x60; or &#x60;f1m1&#x60;.</param>
    /// <param name="mode">Current mode of play, can be &#x60;pre_match&#x60;, &#x60;auto&#x60;, &#x60;telop&#x60;, or &#x60;post_match&#x60;.</param>
    /// <param name="play">play.</param>
    /// <param name="redAutoQuest">1 if the red alliance is credited with the AUTO QUEST, 0 if not.</param>
    /// <param name="redBoostCount">Number of POWER CUBES in the BOOST section of the red alliance VAULT.</param>
    /// <param name="redBoostPlayed">Returns 1 if the red alliance BOOST was played, or 0 if not played.</param>
    /// <param name="redCurrentPowerup">Name of the current red alliance POWER UP being played, or &#x60;null&#x60;.</param>
    /// <param name="redFaceTheBoss">1 if the red alliance is credited with FACING THE BOSS, 0 if not.</param>
    /// <param name="redForceCount">Number of POWER CUBES in the FORCE section of the red alliance VAULT.</param>
    /// <param name="redForcePlayed">Returns 1 if the red alliance FORCE was played, or 0 if not played.</param>
    /// <param name="redLevitateCount">Number of POWER CUBES in the LEVITATE section of the red alliance VAULT.</param>
    /// <param name="redLevitatePlayed">Returns 1 if the red alliance LEVITATE was played, or 0 if not played.</param>
    /// <param name="redPowerupTimeRemaining">Number of seconds remaining in the red alliance POWER UP time, or 0 if none is active.</param>
    /// <param name="redScaleOwned">1 if the red alliance owns the SCALE, 0 if not.</param>
    /// <param name="redScore">Current score for the red alliance.</param>
    /// <param name="redSwitchOwned">1 if the red alliance owns their SWITCH, 0 if not.</param>
    /// <param name="timeRemaining">Amount of time remaining in the match, only valid during &#x60;auto&#x60; and &#x60;teleop&#x60; modes.</param>
    public MatchTimeseries2018(int? blueAutoQuest = default, int? blueBoostCount = default, int? blueBoostPlayed = default, string? blueCurrentPowerup = default, int? blueFaceTheBoss = default, int? blueForceCount = default, int? blueForcePlayed = default, int? blueLevitateCount = default, int? blueLevitatePlayed = default, string? bluePowerupTimeRemaining = default, int? blueScaleOwned = default, int? blueScore = default, int? blueSwitchOwned = default, string? eventKey = default, string? matchId = default, string? mode = default, int? play = default, int? redAutoQuest = default, int? redBoostCount = default, int? redBoostPlayed = default, string? redCurrentPowerup = default, int? redFaceTheBoss = default, int? redForceCount = default, int? redForcePlayed = default, int? redLevitateCount = default, int? redLevitatePlayed = default, string? redPowerupTimeRemaining = default, int? redScaleOwned = default, int? redScore = default, int? redSwitchOwned = default, int? timeRemaining = default)
    {
        this.BlueAutoQuest = blueAutoQuest;
        this.BlueBoostCount = blueBoostCount;
        this.BlueBoostPlayed = blueBoostPlayed;
        this.BlueCurrentPowerup = blueCurrentPowerup;
        this.BlueFaceTheBoss = blueFaceTheBoss;
        this.BlueForceCount = blueForceCount;
        this.BlueForcePlayed = blueForcePlayed;
        this.BlueLevitateCount = blueLevitateCount;
        this.BlueLevitatePlayed = blueLevitatePlayed;
        this.BluePowerupTimeRemaining = bluePowerupTimeRemaining;
        this.BlueScaleOwned = blueScaleOwned;
        this.BlueScore = blueScore;
        this.BlueSwitchOwned = blueSwitchOwned;
        this.EventKey = eventKey;
        this.MatchId = matchId;
        this.Mode = mode;
        this.Play = play;
        this.RedAutoQuest = redAutoQuest;
        this.RedBoostCount = redBoostCount;
        this.RedBoostPlayed = redBoostPlayed;
        this.RedCurrentPowerup = redCurrentPowerup;
        this.RedFaceTheBoss = redFaceTheBoss;
        this.RedForceCount = redForceCount;
        this.RedForcePlayed = redForcePlayed;
        this.RedLevitateCount = redLevitateCount;
        this.RedLevitatePlayed = redLevitatePlayed;
        this.RedPowerupTimeRemaining = redPowerupTimeRemaining;
        this.RedScaleOwned = redScaleOwned;
        this.RedScore = redScore;
        this.RedSwitchOwned = redSwitchOwned;
        this.TimeRemaining = timeRemaining;
    }

    /// <summary>
    /// 1 if the blue alliance is credited with the AUTO QUEST, 0 if not.
    /// </summary>
    /// <value>1 if the blue alliance is credited with the AUTO QUEST, 0 if not.</value>

    [JsonPropertyName("blue_auto_quest")]
    public int? BlueAutoQuest { get; set; }

    /// <summary>
    /// Number of POWER CUBES in the BOOST section of the blue alliance VAULT.
    /// </summary>
    /// <value>Number of POWER CUBES in the BOOST section of the blue alliance VAULT.</value>

    [JsonPropertyName("blue_boost_count")]
    public int? BlueBoostCount { get; set; }

    /// <summary>
    /// Returns 1 if the blue alliance BOOST was played, or 0 if not played.
    /// </summary>
    /// <value>Returns 1 if the blue alliance BOOST was played, or 0 if not played.</value>

    [JsonPropertyName("blue_boost_played")]
    public int? BlueBoostPlayed { get; set; }

    /// <summary>
    /// Name of the current blue alliance POWER UP being played, or &#x60;null&#x60;.
    /// </summary>
    /// <value>Name of the current blue alliance POWER UP being played, or &#x60;null&#x60;.</value>

    [JsonPropertyName("blue_current_powerup")]
    public string? BlueCurrentPowerup { get; set; }

    /// <summary>
    /// 1 if the blue alliance is credited with FACING THE BOSS, 0 if not.
    /// </summary>
    /// <value>1 if the blue alliance is credited with FACING THE BOSS, 0 if not.</value>

    [JsonPropertyName("blue_face_the_boss")]
    public int? BlueFaceTheBoss { get; set; }

    /// <summary>
    /// Number of POWER CUBES in the FORCE section of the blue alliance VAULT.
    /// </summary>
    /// <value>Number of POWER CUBES in the FORCE section of the blue alliance VAULT.</value>

    [JsonPropertyName("blue_force_count")]
    public int? BlueForceCount { get; set; }

    /// <summary>
    /// Returns 1 if the blue alliance FORCE was played, or 0 if not played.
    /// </summary>
    /// <value>Returns 1 if the blue alliance FORCE was played, or 0 if not played.</value>

    [JsonPropertyName("blue_force_played")]
    public int? BlueForcePlayed { get; set; }

    /// <summary>
    /// Number of POWER CUBES in the LEVITATE section of the blue alliance VAULT.
    /// </summary>
    /// <value>Number of POWER CUBES in the LEVITATE section of the blue alliance VAULT.</value>

    [JsonPropertyName("blue_levitate_count")]
    public int? BlueLevitateCount { get; set; }

    /// <summary>
    /// Returns 1 if the blue alliance LEVITATE was played, or 0 if not played.
    /// </summary>
    /// <value>Returns 1 if the blue alliance LEVITATE was played, or 0 if not played.</value>

    [JsonPropertyName("blue_levitate_played")]
    public int? BlueLevitatePlayed { get; set; }

    /// <summary>
    /// Number of seconds remaining in the blue alliance POWER UP time, or 0 if none is active.
    /// </summary>
    /// <value>Number of seconds remaining in the blue alliance POWER UP time, or 0 if none is active.</value>

    [JsonPropertyName("blue_powerup_time_remaining")]
    public string? BluePowerupTimeRemaining { get; set; }

    /// <summary>
    /// 1 if the blue alliance owns the SCALE, 0 if not.
    /// </summary>
    /// <value>1 if the blue alliance owns the SCALE, 0 if not.</value>

    [JsonPropertyName("blue_scale_owned")]
    public int? BlueScaleOwned { get; set; }

    /// <summary>
    /// Current score for the blue alliance.
    /// </summary>
    /// <value>Current score for the blue alliance.</value>

    [JsonPropertyName("blue_score")]
    public int? BlueScore { get; set; }

    /// <summary>
    /// 1 if the blue alliance owns their SWITCH, 0 if not.
    /// </summary>
    /// <value>1 if the blue alliance owns their SWITCH, 0 if not.</value>

    [JsonPropertyName("blue_switch_owned")]
    public int? BlueSwitchOwned { get; set; }

    /// <summary>
    /// TBA event key with the format yyyy[EVENT_CODE], where yyyy is the year, and EVENT_CODE is the event code of the event.
    /// </summary>
    /// <value>TBA event key with the format yyyy[EVENT_CODE], where yyyy is the year, and EVENT_CODE is the event code of the event.</value>

    [JsonPropertyName("event_key")]
    public string? EventKey { get; set; }

    /// <summary>
    /// Match ID consisting of the level, match number, and set number, eg &#x60;qm45&#x60; or &#x60;f1m1&#x60;.
    /// </summary>
    /// <value>Match ID consisting of the level, match number, and set number, eg &#x60;qm45&#x60; or &#x60;f1m1&#x60;.</value>

    [JsonPropertyName("match_id")]
    public string? MatchId { get; set; }

    /// <summary>
    /// Current mode of play, can be &#x60;pre_match&#x60;, &#x60;auto&#x60;, &#x60;telop&#x60;, or &#x60;post_match&#x60;.
    /// </summary>
    /// <value>Current mode of play, can be &#x60;pre_match&#x60;, &#x60;auto&#x60;, &#x60;telop&#x60;, or &#x60;post_match&#x60;.</value>

    [JsonPropertyName("mode")]
    public string? Mode { get; set; }

    /// <summary>
    /// Gets or Sets Play
    /// </summary>

    [JsonPropertyName("play")]
    public int? Play { get; set; }

    /// <summary>
    /// 1 if the red alliance is credited with the AUTO QUEST, 0 if not.
    /// </summary>
    /// <value>1 if the red alliance is credited with the AUTO QUEST, 0 if not.</value>

    [JsonPropertyName("red_auto_quest")]
    public int? RedAutoQuest { get; set; }

    /// <summary>
    /// Number of POWER CUBES in the BOOST section of the red alliance VAULT.
    /// </summary>
    /// <value>Number of POWER CUBES in the BOOST section of the red alliance VAULT.</value>

    [JsonPropertyName("red_boost_count")]
    public int? RedBoostCount { get; set; }

    /// <summary>
    /// Returns 1 if the red alliance BOOST was played, or 0 if not played.
    /// </summary>
    /// <value>Returns 1 if the red alliance BOOST was played, or 0 if not played.</value>

    [JsonPropertyName("red_boost_played")]
    public int? RedBoostPlayed { get; set; }

    /// <summary>
    /// Name of the current red alliance POWER UP being played, or &#x60;null&#x60;.
    /// </summary>
    /// <value>Name of the current red alliance POWER UP being played, or &#x60;null&#x60;.</value>

    [JsonPropertyName("red_current_powerup")]
    public string? RedCurrentPowerup { get; set; }

    /// <summary>
    /// 1 if the red alliance is credited with FACING THE BOSS, 0 if not.
    /// </summary>
    /// <value>1 if the red alliance is credited with FACING THE BOSS, 0 if not.</value>

    [JsonPropertyName("red_face_the_boss")]
    public int? RedFaceTheBoss { get; set; }

    /// <summary>
    /// Number of POWER CUBES in the FORCE section of the red alliance VAULT.
    /// </summary>
    /// <value>Number of POWER CUBES in the FORCE section of the red alliance VAULT.</value>

    [JsonPropertyName("red_force_count")]
    public int? RedForceCount { get; set; }

    /// <summary>
    /// Returns 1 if the red alliance FORCE was played, or 0 if not played.
    /// </summary>
    /// <value>Returns 1 if the red alliance FORCE was played, or 0 if not played.</value>

    [JsonPropertyName("red_force_played")]
    public int? RedForcePlayed { get; set; }

    /// <summary>
    /// Number of POWER CUBES in the LEVITATE section of the red alliance VAULT.
    /// </summary>
    /// <value>Number of POWER CUBES in the LEVITATE section of the red alliance VAULT.</value>

    [JsonPropertyName("red_levitate_count")]
    public int? RedLevitateCount { get; set; }

    /// <summary>
    /// Returns 1 if the red alliance LEVITATE was played, or 0 if not played.
    /// </summary>
    /// <value>Returns 1 if the red alliance LEVITATE was played, or 0 if not played.</value>

    [JsonPropertyName("red_levitate_played")]
    public int? RedLevitatePlayed { get; set; }

    /// <summary>
    /// Number of seconds remaining in the red alliance POWER UP time, or 0 if none is active.
    /// </summary>
    /// <value>Number of seconds remaining in the red alliance POWER UP time, or 0 if none is active.</value>

    [JsonPropertyName("red_powerup_time_remaining")]
    public string? RedPowerupTimeRemaining { get; set; }

    /// <summary>
    /// 1 if the red alliance owns the SCALE, 0 if not.
    /// </summary>
    /// <value>1 if the red alliance owns the SCALE, 0 if not.</value>

    [JsonPropertyName("red_scale_owned")]
    public int? RedScaleOwned { get; set; }

    /// <summary>
    /// Current score for the red alliance.
    /// </summary>
    /// <value>Current score for the red alliance.</value>

    [JsonPropertyName("red_score")]
    public int? RedScore { get; set; }

    /// <summary>
    /// 1 if the red alliance owns their SWITCH, 0 if not.
    /// </summary>
    /// <value>1 if the red alliance owns their SWITCH, 0 if not.</value>

    [JsonPropertyName("red_switch_owned")]
    public int? RedSwitchOwned { get; set; }

    /// <summary>
    /// Amount of time remaining in the match, only valid during &#x60;auto&#x60; and &#x60;teleop&#x60; modes.
    /// </summary>
    /// <value>Amount of time remaining in the match, only valid during &#x60;auto&#x60; and &#x60;teleop&#x60; modes.</value>

    [JsonPropertyName("time_remaining")]
    public int? TimeRemaining { get; set; }

    /// <summary>
    /// Returns the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }
}

