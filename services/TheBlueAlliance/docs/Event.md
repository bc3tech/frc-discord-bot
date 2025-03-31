# TheBlueAlliance.Model.Event

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Address** | **string** | Address of the event&#39;s venue, if available. | 
**City** | **string** | City, town, village, etc. the event is located in. | 
**Country** | **string** | Country the event is located in. | 
**District** | [**DistrictList**](DistrictList.md) |  | 
**DivisionKeys** | **Collection&lt;string&gt;** | An array of event keys for the divisions at this event. | 
**EndDate** | **DateOnly** | Event end date in &#x60;yyyy-mm-dd&#x60; format. | 
**EventCode** | **string** | Event short code, as provided by FIRST. | 
**EventType** | **int** | Event Type, as defined here: https://github.com/the-blue-alliance/the-blue-alliance/blob/master/consts/event_type.py#L2 | 
**EventTypeString** | **string** | Event Type, eg Regional, District, or Offseason. | 
**FirstEventCode** | **string** | Public facing event code used by FIRST (on frc-events.firstinspires.org, for example) | 
**FirstEventId** | **string** | The FIRST internal Event ID, used to link to the event on the FRC webpage. | 
**GmapsPlaceId** | **string** | Google Maps Place ID for the event address. | 
**GmapsUrl** | **string** | Link to address location on Google Maps. | 
**Key** | **string** | TBA event key with the format yyyy[EVENT_CODE], where yyyy is the year, and EVENT_CODE is the event code of the event. | 
**Lat** | **double?** | Latitude for the event address. | 
**Lng** | **double?** | Longitude for the event address. | 
**LocationName** | **string** | Name of the location at the address for the event, eg. Blue Alliance High School. | 
**Name** | **string** | Official name of event on record either provided by FIRST or organizers of offseason event. | 
**ParentEventKey** | **string** | The TBA Event key that represents the event&#39;s parent. Used to link back to the event from a division event. It is also the inverse relation of &#x60;divison_keys&#x60;. | 
**PlayoffType** | **int?** | Playoff Type, as defined here: https://github.com/the-blue-alliance/the-blue-alliance/blob/master/consts/playoff_type.py#L4, or null. | 
**PlayoffTypeString** | **string** | String representation of the &#x60;playoff_type&#x60;, or null. | 
**PostalCode** | **string** | Postal code from the event address. | 
**ShortName** | **string** | Same as &#x60;name&#x60; but doesn&#39;t include event specifiers, such as &#39;Regional&#39; or &#39;District&#39;. May be null. | 
**StartDate** | **DateOnly** | Event start date in &#x60;yyyy-mm-dd&#x60; format. | 
**StateProv** | **string** | State or Province the event is located in. | 
**Timezone** | **string** | Timezone name. | 
**Webcasts** | [**Collection&lt;Webcast&gt;**](Webcast.md) |  | 
**Website** | **string** | The event&#39;s website, if any. | 
**Week** | **int?** | Week of the event relative to the first official season event, zero-indexed. Only valid for Regionals, Districts, and District Championships. Null otherwise. (Eg. A season with a week 0 &#39;preseason&#39; event does not count, and week 1 events will show 0 here. Seasons with a week 0.5 regional event will show week 0 for those event(s) and week 1 for week 1 events and so on.) | 
**Year** | **int** | Year the event data is for. | 

[[Back to Model list]](../../README.md#documentation-for-models) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to README]](../../README.md)

