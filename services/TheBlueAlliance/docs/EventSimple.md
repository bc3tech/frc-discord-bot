# TheBlueAlliance.Model.EventSimple

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**City** | **string** | City, town, village, etc. the event is located in. | 
**Country** | **string** | Country the event is located in. | 
**District** | [**DistrictList**](DistrictList.md) |  | 
**EndDate** | **DateOnly** | Event end date in &#x60;yyyy-mm-dd&#x60; format. | 
**EventCode** | **string** | Event short code, as provided by FIRST. | 
**EventType** | **int** | Event Type, as defined here: https://github.com/the-blue-alliance/the-blue-alliance/blob/master/consts/event_type.py#L2 | 
**Key** | **string** | TBA event key with the format yyyy[EVENT_CODE], where yyyy is the year, and EVENT_CODE is the event code of the event. | 
**Name** | **string** | Official name of event on record either provided by FIRST or organizers of offseason event. | 
**StartDate** | **DateOnly** | Event start date in &#x60;yyyy-mm-dd&#x60; format. | 
**StateProv** | **string** | State or Province the event is located in. | 
**Year** | **int** | Year the event data is for. | 

[[Back to Model list]](../../README.md#documentation-for-models) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to README]](../../README.md)

