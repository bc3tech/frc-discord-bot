# TheBlueAlliance.Model.Match

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**ActualTime** | **long?** | UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of actual match start time. | 
**Alliances** | [**MatchSimpleAlliances**](MatchSimpleAlliances.md) |  | 
**CompLevel** | **string** | The competition level the match was played at. | 
**EventKey** | **string** | Event key of the event the match was played at. | 
**Key** | **string** | TBA match key with the format &#x60;yyyy[EVENT_CODE]_[COMP_LEVEL]m[MATCH_NUMBER]&#x60;, where &#x60;yyyy&#x60; is the year, and &#x60;EVENT_CODE&#x60; is the event code of the event, &#x60;COMP_LEVEL&#x60; is (qm, ef, qf, sf, f), and &#x60;MATCH_NUMBER&#x60; is the match number in the competition level. A set number may be appended to the competition level if more than one match in required per set. | 
**MatchNumber** | **int** | The match number of the match in the competition level. | 
**PostResultTime** | **long?** | UNIX timestamp (seconds since 1-Jan-1970 00:00:00) when the match result was posted. | 
**PredictedTime** | **long?** | UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of the TBA predicted match start time. | 
**ScoreBreakdown** | [**MatchScoreBreakdown**](MatchScoreBreakdown.md) |  | 
**SetNumber** | **int** | The set number in a series of matches where more than one match is required in the match series. | 
**Time** | **long?** | UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of the scheduled match time, as taken from the published schedule. | 
**Videos** | [**Collection&lt;MatchVideosInner&gt;**](MatchVideosInner.md) | Array of video objects associated with this match. | 
**WinningAlliance** | **string** | The color (red/blue) of the winning alliance. Will contain an empty string in the event of no winner, or a tie. | 

[[Back to Model list]](../../README.md#documentation-for-models) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to README]](../../README.md)

