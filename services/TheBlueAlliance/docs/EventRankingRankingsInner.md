# TheBlueAlliance.Api.Model.EventRankingRankingsInner

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**MatchesPlayed** | **int** | Number of matches played by this team. | 
**QualAverage** | **int?** | The average match score during qualifications. Year specific. May be null if not relevant for a given year. | 
**ExtraStats** | **List&lt;decimal&gt;** | Additional special data on the team&#39;s performance calculated by TBA. | 
**SortOrders** | **List&lt;decimal&gt;** | Additional year-specific information, may be null. See parent &#x60;sort_order_info&#x60; for details. | 
**Record** | [**WLTRecord**](WLTRecord.md) |  | 
**Rank** | **int** | The team&#39;s rank at the event as provided by FIRST. | 
**Dq** | **int** | Number of times disqualified. | 
**TeamKey** | **string** | The team with this rank. | 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

