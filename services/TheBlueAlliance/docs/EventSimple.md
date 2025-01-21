# TheBlueAlliance.Api.Model.EventSimple

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Key** | **string** | TBA event key with the format yyyy[EVENT_CODE], where yyyy is the year, and EVENT_CODE is the event code of the event. | 
**Name** | **string** | Official name of event on record either provided by FIRST or organizers of offseason event. | 
**EventCode** | **string** | Event short code, as provided by FIRST. | 
**EventType** | **int** | Event Type, as defined here: https://github.com/the-blue-alliance/the-blue-alliance/blob/master/consts/event_type.py#L2 | 
**District** | [**DistrictList**](DistrictList.md) |  | 
**City** | **string** | City, town, village, etc. the event is located in. | 
**StateProv** | **string** | State or Province the event is located in. | 
**Country** | **string** | Country the event is located in. | 
**StartDate** | **DateOnly** | Event start date in &#x60;yyyy-mm-dd&#x60; format. | 
**EndDate** | **DateOnly** | Event end date in &#x60;yyyy-mm-dd&#x60; format. | 
**Year** | **int** | Year the event data is for. | 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

