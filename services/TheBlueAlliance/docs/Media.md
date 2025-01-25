# TheBlueAlliance.Model.Media
The `Media` object contains a reference for most any media associated with a team or event on TBA.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Details** | **Dictionary&lt;string, Object&gt;** | If required, a JSON dict of additional media information. | [optional] 
**DirectUrl** | **string** | Direct URL to the media. | [optional] 
**ForeignKey** | **string** | The key used to identify this media on the media site. | 
**Preferred** | **bool** | True if the media is of high quality. | [optional] 
**TeamKeys** | **Collection&lt;string&gt;** | List of teams that this media belongs to. Most likely length 1. | 
**Type** | **string** | String type of the media element. | 
**ViewUrl** | **string** | The URL that leads to the full web page for the media, if one exists. | [optional] 

[[Back to Model list]](../../README.md#documentation-for-models) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to README]](../../README.md)

