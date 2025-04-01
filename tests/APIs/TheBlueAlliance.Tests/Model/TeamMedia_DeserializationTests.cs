namespace TheBlueAlliance.Tests.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using TheBlueAlliance.Model;

public class TeamMedia_DeserializationTests
{
    [Fact]
    public void MediaWithOnShapeCAD_CanBeDeserializedSuccesfully()
    {
        const string json = """
            [
            	{
            		"details": {
            			"base64Image": "iVBORw0KGgoAAAANSUhEUgAAACgAAAAoCAYAAACM/rhtAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwgAADsIBFShKgAAAAjZJREFUWEftlsFxAjEMRSmBEiiBAnJICRxz5JwTJaQESkgJlEAJlJAS0gH5z/gzwrsbe0mYyWT8ZjT2emVZlmTvLjqdTqfzzzifz2vJc378W+CYxCQn1b5LPmmT0h1o7irbWOWhdjTpTXKSLCVbiWHscOleyFNmo6kbyT73WSP1q0iRdBp2WAPju0u3vQyky0aXoV/PhpRSuNUSqXtoShk6kuSQWja4TS++Q0p7CdykUOAsKScl1CPROkrGaKpJ9CQ4iXNtdSzFMp0845APCUZx7kPi8XIOm2ThXTY7QO+Yh167cyDlWHuAAZxBSnAUsfNjbLLpKxrj0JEh5rYdCpAyC+GQIVp+xqBrkwUw7sgRCdI/xiA6jEkomXrNRbBWwMJE7phVbtC4I5eipLaM8iEpBjTGhtnYILJV8sSID8rktaF3Pii0ZRRvoqfnmB30eZ53QWsCpxR8QcNkKvTOTrGZqVqMpTIgm6ojXU5evF6ui2eVAXqHTioBtWMO+tSPkTaVDLUg5dIQxuO96BsfRxgnwq47dKODlMurJG7YoHvXt9fH34UM7uMILal3384DfaeR+nqRlIeGOfMPR4mM+A/DuKCJECkl0kTB+PrhHVGL7wz21mmBnyJDPihEoIyCiRtIXwxaSRyPzLvzasggTpLyWFfx54FxhMgStamNQPunbC4yzqcv1V1+pkaRJwkpnYpY5HdSW0ML+c9j7GROccrTH4cW4ZC0RGqMyTu00+l0Op1Hslh8ARA1/azJyMMuAAAAAElFTkSuQmCC"
            		},
            		"direct_url": "",
            		"foreign_key": "avatar_2025_frc5937",
            		"preferred": false,
            		"team_keys": [
            			"frc5937"
            		],
            		"type": "avatar",
            		"view_url": ""
            	},
            	{
            		"details": {},
            		"direct_url": "https://i.imgur.com/9QtWSlY.jpeg",
            		"foreign_key": "9QtWSlY",
            		"preferred": false,
            		"team_keys": [
            			"frc5937"
            		],
            		"type": "imgur",
            		"view_url": "https://imgur.com/9QtWSlY"
            	},
            	{
            		"details": {},
            		"direct_url": "https://i.imgur.com/HIrzWpR.jpeg",
            		"foreign_key": "HIrzWpR",
            		"preferred": false,
            		"team_keys": [
            			"frc5937"
            		],
            		"type": "imgur",
            		"view_url": "https://imgur.com/HIrzWpR"
            	},
            	{
            		"details": {},
            		"direct_url": "https://i.imgur.com/U2biwZ0.jpeg",
            		"foreign_key": "U2biwZ0",
            		"preferred": true,
            		"team_keys": [
            			"frc5937"
            		],
            		"type": "imgur",
            		"view_url": "https://imgur.com/U2biwZ0"
            	},
            	{
            		"details": {},
            		"direct_url": "https://i.imgur.com/zoR4iow.jpeg",
            		"foreign_key": "zoR4iow",
            		"preferred": true,
            		"team_keys": [
            			"frc5937"
            		],
            		"type": "imgur",
            		"view_url": "https://imgur.com/zoR4iow"
            	},
            	{
            		"details": {
            			"model_created": "2025-01-06T23:28:01.675+00:00",
            			"model_description": null,
            			"model_image": "https://cad.onshape.com/api/thumbnails/d/c0668f67246caf146a0f81cd/w/5733b7d38414815bea05e780/s/300x300",
            			"model_name": "Full Bot CAD"
            		},
            		"direct_url": "https://cad.onshape.com/api/thumbnails/d/c0668f67246caf146a0f81cd/w/5733b7d38414815bea05e780/s/600x340",
            		"foreign_key": "c0668f67246caf146a0f81cd/w/5733b7d38414815bea05e780",
            		"preferred": false,
            		"team_keys": [
            			"frc5937"
            		],
            		"type": "onshape",
            		"view_url": "https://cad.onshape.com/documents/c0668f67246caf146a0f81cd/w/5733b7d38414815bea05e780"
            	}
            ]
            """;

        Assert.NotNull(JsonSerializer.Deserialize<Media[]>(json));
    }
}
