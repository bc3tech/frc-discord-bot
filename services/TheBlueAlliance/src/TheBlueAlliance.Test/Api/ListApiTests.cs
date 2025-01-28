/*
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).
 *
 * The version of the OpenAPI document: 3.9.7
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Xunit;

using TheBlueAlliance.Client;
using TheBlueAlliance.Api;
// uncomment below to import models
//using TheBlueAlliance.Model;

namespace TheBlueAlliance.Test.Api


    /// <summary>
    ///  Class for testing ListApi
    /// </summary>
    /// <remarks>
    /// This file is automatically generated by OpenAPI Generator (https://openapi-generator.tech).
    /// Please update the test case below to test the API endpoint.
    /// </remarks>
    public class ListApiTests : IDisposable
    {
        private ListApi instance;

        public ListApiTests()
        {
            instance = new ListApi();
        }

        public void Dispose()
        {
            // Cleanup when everything is done.
        }

        /// <summary>
        /// Test an instance of ListApi
        /// </summary>
        [Fact]
        public void InstanceTest()
        {
            // TODO uncomment below to test 'IsType' ListApi
            //Assert.IsType<ListApi>(instance);
        }

        /// <summary>
        /// Test GetDistrictAwards
        /// </summary>
        [Fact]
        public void GetDistrictAwardsTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string districtKey = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetDistrictAwards(districtKey, ifNoneMatch);
            //Assert.IsType<Collection<Award>>(response);
        }

        /// <summary>
        /// Test GetDistrictEvents
        /// </summary>
        [Fact]
        public void GetDistrictEventsTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string districtKey = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetDistrictEvents(districtKey, ifNoneMatch);
            //Assert.IsType<Collection<Event>>(response);
        }

        /// <summary>
        /// Test GetDistrictEventsKeys
        /// </summary>
        [Fact]
        public void GetDistrictEventsKeysTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string districtKey = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetDistrictEventsKeys(districtKey, ifNoneMatch);
            //Assert.IsType<Collection<string>>(response);
        }

        /// <summary>
        /// Test GetDistrictEventsSimple
        /// </summary>
        [Fact]
        public void GetDistrictEventsSimpleTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string districtKey = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetDistrictEventsSimple(districtKey, ifNoneMatch);
            //Assert.IsType<Collection<EventSimple>>(response);
        }

        /// <summary>
        /// Test GetDistrictRankings
        /// </summary>
        [Fact]
        public void GetDistrictRankingsTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string districtKey = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetDistrictRankings(districtKey, ifNoneMatch);
            //Assert.IsType<Collection<DistrictRanking>>(response);
        }

        /// <summary>
        /// Test GetDistrictTeams
        /// </summary>
        [Fact]
        public void GetDistrictTeamsTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string districtKey = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetDistrictTeams(districtKey, ifNoneMatch);
            //Assert.IsType<Collection<Team>>(response);
        }

        /// <summary>
        /// Test GetDistrictTeamsKeys
        /// </summary>
        [Fact]
        public void GetDistrictTeamsKeysTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string districtKey = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetDistrictTeamsKeys(districtKey, ifNoneMatch);
            //Assert.IsType<Collection<string>>(response);
        }

        /// <summary>
        /// Test GetDistrictTeamsSimple
        /// </summary>
        [Fact]
        public void GetDistrictTeamsSimpleTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string districtKey = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetDistrictTeamsSimple(districtKey, ifNoneMatch);
            //Assert.IsType<Collection<TeamSimple>>(response);
        }

        /// <summary>
        /// Test GetEventTeams
        /// </summary>
        [Fact]
        public void GetEventTeamsTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string eventKey = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetEventTeams(eventKey, ifNoneMatch);
            //Assert.IsType<Collection<Team>>(response);
        }

        /// <summary>
        /// Test GetEventTeamsKeys
        /// </summary>
        [Fact]
        public void GetEventTeamsKeysTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string eventKey = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetEventTeamsKeys(eventKey, ifNoneMatch);
            //Assert.IsType<Collection<string>>(response);
        }

        /// <summary>
        /// Test GetEventTeamsSimple
        /// </summary>
        [Fact]
        public void GetEventTeamsSimpleTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string eventKey = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetEventTeamsSimple(eventKey, ifNoneMatch);
            //Assert.IsType<Collection<TeamSimple>>(response);
        }

        /// <summary>
        /// Test GetEventTeamsStatuses
        /// </summary>
        [Fact]
        public void GetEventTeamsStatusesTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string eventKey = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetEventTeamsStatuses(eventKey, ifNoneMatch);
            //Assert.IsType<Dictionary<string, GetTeamEventsStatusesByYear200ResponseValue>>(response);
        }

        /// <summary>
        /// Test GetEventsByYear
        /// </summary>
        [Fact]
        public void GetEventsByYearTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //int year = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetEventsByYear(year, ifNoneMatch);
            //Assert.IsType<Collection<Event>>(response);
        }

        /// <summary>
        /// Test GetEventsByYearKeys
        /// </summary>
        [Fact]
        public void GetEventsByYearKeysTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //int year = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetEventsByYearKeys(year, ifNoneMatch);
            //Assert.IsType<Collection<string>>(response);
        }

        /// <summary>
        /// Test GetEventsByYearSimple
        /// </summary>
        [Fact]
        public void GetEventsByYearSimpleTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //int year = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetEventsByYearSimple(year, ifNoneMatch);
            //Assert.IsType<Collection<EventSimple>>(response);
        }

        /// <summary>
        /// Test GetInsightsLeaderboardsYear
        /// </summary>
        [Fact]
        public void GetInsightsLeaderboardsYearTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //int year = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetInsightsLeaderboardsYear(year, ifNoneMatch);
            //Assert.IsType<Collection<LeaderboardInsight>>(response);
        }

        /// <summary>
        /// Test GetInsightsNotablesYear
        /// </summary>
        [Fact]
        public void GetInsightsNotablesYearTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //int year = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetInsightsNotablesYear(year, ifNoneMatch);
            //Assert.IsType<Collection<NotablesInsight>>(response);
        }

        /// <summary>
        /// Test GetTeamEventsStatusesByYear
        /// </summary>
        [Fact]
        public void GetTeamEventsStatusesByYearTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string teamKey = null;
            //int year = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetTeamEventsStatusesByYear(teamKey, year, ifNoneMatch);
            //Assert.IsType<Dictionary<string, GetTeamEventsStatusesByYear200ResponseValue>>(response);
        }

        /// <summary>
        /// Test GetTeams
        /// </summary>
        [Fact]
        public void GetTeamsTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //int pageNum = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetTeams(pageNum, ifNoneMatch);
            //Assert.IsType<Collection<Team>>(response);
        }

        /// <summary>
        /// Test GetTeamsByYear
        /// </summary>
        [Fact]
        public void GetTeamsByYearTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //int pageNum = null;
            //int year = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetTeamsByYear(pageNum, year, ifNoneMatch);
            //Assert.IsType<Collection<Team>>(response);
        }

        /// <summary>
        /// Test GetTeamsByYearKeys
        /// </summary>
        [Fact]
        public void GetTeamsByYearKeysTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //int pageNum = null;
            //int year = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetTeamsByYearKeys(pageNum, year, ifNoneMatch);
            //Assert.IsType<Collection<string>>(response);
        }

        /// <summary>
        /// Test GetTeamsByYearSimple
        /// </summary>
        [Fact]
        public void GetTeamsByYearSimpleTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //int pageNum = null;
            //int year = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetTeamsByYearSimple(pageNum, year, ifNoneMatch);
            //Assert.IsType<Collection<TeamSimple>>(response);
        }

        /// <summary>
        /// Test GetTeamsKeys
        /// </summary>
        [Fact]
        public void GetTeamsKeysTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //int pageNum = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetTeamsKeys(pageNum, ifNoneMatch);
            //Assert.IsType<Collection<string>>(response);
        }

        /// <summary>
        /// Test GetTeamsSimple
        /// </summary>
        [Fact]
        public void GetTeamsSimpleTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //int pageNum = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetTeamsSimple(pageNum, ifNoneMatch);
            //Assert.IsType<Collection<TeamSimple>>(response);
        }
    }

