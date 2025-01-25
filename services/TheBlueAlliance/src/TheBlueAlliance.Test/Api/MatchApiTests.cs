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
    ///  Class for testing MatchApi
    /// </summary>
    /// <remarks>
    /// This file is automatically generated by OpenAPI Generator (https://openapi-generator.tech).
    /// Please update the test case below to test the API endpoint.
    /// </remarks>
    public class MatchApiTests : IDisposable
    {
        public MatchApiTests()
        {
        }

        public void Dispose()
        {
            // Cleanup when everything is done.
        }

        /// <summary>
        /// Test an instance of MatchApi
        /// </summary>
        [Fact]
        public void InstanceTest()
        {
            // TODO uncomment below to test 'IsType' MatchApi
            //Assert.IsType<MatchApi>(instance);
        }

        /// <summary>
        /// Test GetEventMatchTimeseries
        /// </summary>
        [Fact]
        public void GetEventMatchTimeseriesTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string eventKey = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetEventMatchTimeseries(eventKey, ifNoneMatch);
            //Assert.IsType<Collection<string>>(response);
        }

        /// <summary>
        /// Test GetEventMatches
        /// </summary>
        [Fact]
        public void GetEventMatchesTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string eventKey = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetEventMatches(eventKey, ifNoneMatch);
            //Assert.IsType<Collection<Match>>(response);
        }

        /// <summary>
        /// Test GetEventMatchesKeys
        /// </summary>
        [Fact]
        public void GetEventMatchesKeysTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string eventKey = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetEventMatchesKeys(eventKey, ifNoneMatch);
            //Assert.IsType<Collection<string>>(response);
        }

        /// <summary>
        /// Test GetEventMatchesSimple
        /// </summary>
        [Fact]
        public void GetEventMatchesSimpleTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string eventKey = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetEventMatchesSimple(eventKey, ifNoneMatch);
            //Assert.IsType<Collection<MatchSimple>>(response);
        }

        /// <summary>
        /// Test GetMatch
        /// </summary>
        [Fact]
        public void GetMatchTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string matchKey = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetMatch(matchKey, ifNoneMatch);
            //Assert.IsType<Match>(response);
        }

        /// <summary>
        /// Test GetMatchSimple
        /// </summary>
        [Fact]
        public void GetMatchSimpleTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string matchKey = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetMatchSimple(matchKey, ifNoneMatch);
            //Assert.IsType<MatchSimple>(response);
        }

        /// <summary>
        /// Test GetMatchTimeseries
        /// </summary>
        [Fact]
        public void GetMatchTimeseriesTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string matchKey = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetMatchTimeseries(matchKey, ifNoneMatch);
            //Assert.IsType<Collection<Object>>(response);
        }

        /// <summary>
        /// Test GetMatchZebra
        /// </summary>
        [Fact]
        public void GetMatchZebraTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string matchKey = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetMatchZebra(matchKey, ifNoneMatch);
            //Assert.IsType<Zebra>(response);
        }

        /// <summary>
        /// Test GetTeamEventMatches
        /// </summary>
        [Fact]
        public void GetTeamEventMatchesTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string eventKey = null;
            //string teamKey = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetTeamEventMatches(eventKey, teamKey, ifNoneMatch);
            //Assert.IsType<Collection<Match>>(response);
        }

        /// <summary>
        /// Test GetTeamEventMatchesKeys
        /// </summary>
        [Fact]
        public void GetTeamEventMatchesKeysTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string eventKey = null;
            //string teamKey = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetTeamEventMatchesKeys(eventKey, teamKey, ifNoneMatch);
            //Assert.IsType<Collection<string>>(response);
        }

        /// <summary>
        /// Test GetTeamEventMatchesSimple
        /// </summary>
        [Fact]
        public void GetTeamEventMatchesSimpleTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string eventKey = null;
            //string teamKey = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetTeamEventMatchesSimple(eventKey, teamKey, ifNoneMatch);
            //Assert.IsType<Collection<Match>>(response);
        }

        /// <summary>
        /// Test GetTeamMatchesByYear
        /// </summary>
        [Fact]
        public void GetTeamMatchesByYearTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string teamKey = null;
            //int year = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetTeamMatchesByYear(teamKey, year, ifNoneMatch);
            //Assert.IsType<Collection<Match>>(response);
        }

        /// <summary>
        /// Test GetTeamMatchesByYearKeys
        /// </summary>
        [Fact]
        public void GetTeamMatchesByYearKeysTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string teamKey = null;
            //int year = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetTeamMatchesByYearKeys(teamKey, year, ifNoneMatch);
            //Assert.IsType<Collection<string>>(response);
        }

        /// <summary>
        /// Test GetTeamMatchesByYearSimple
        /// </summary>
        [Fact]
        public void GetTeamMatchesByYearSimpleTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string teamKey = null;
            //int year = null;
            //string? ifNoneMatch = null;
            //var response = instance.GetTeamMatchesByYearSimple(teamKey, year, ifNoneMatch);
            //Assert.IsType<Collection<MatchSimple>>(response);
        }
    }

