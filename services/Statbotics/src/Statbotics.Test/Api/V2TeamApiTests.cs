/*
 * Statbotics REST API
 *
 * The REST API for Statbotics. Please be nice to our servers! If you are looking to do large-scale data science projects, use the CSV exports on the GitHub repo.
 *
 * The version of the OpenAPI document: 3.0.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Xunit;

using Statbotics.Client;
using Statbotics.Api;
// uncomment below to import models
//using Statbotics.Model;

namespace Statbotics.Test.Api;
{
    /// <summary>
    ///  Class for testing V2TeamApi
    /// </summary>
    /// <remarks>
    /// This file is automatically generated by OpenAPI Generator (https://openapi-generator.tech).
    /// Please update the test case below to test the API endpoint.
    /// </remarks>
    public class V2TeamApiTests : IDisposable
    {
        public V2TeamApiTests()
        {
        }

        public void Dispose()
        {
            // Cleanup when everything is done.
        }

        /// <summary>
        /// Test an instance of V2TeamApi
        /// </summary>
        [Fact]
        public void InstanceTest()
        {
            // TODO uncomment below to test 'IsType' V2TeamApi
            //Assert.IsType<V2TeamApi>(instance);
        }

        /// <summary>
        /// Test ReadTeamV2TeamTeamGet
        /// </summary>
        [Fact]
        public void ReadTeamV2TeamTeamGetTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //int team = null;
            //var response = instance.ReadTeamV2TeamTeamGet(team);
            //Assert.IsType<Object>(response);
        }

        /// <summary>
        /// Test ReadTeamsDistrictV2TeamsDistrictDistrictGet
        /// </summary>
        [Fact]
        public void ReadTeamsDistrictV2TeamsDistrictDistrictGetTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string district = null;
            //var response = instance.ReadTeamsDistrictV2TeamsDistrictDistrictGet(district);
            //Assert.IsType<List<Object>>(response);
        }

        /// <summary>
        /// Test ReadTeamsStateV2TeamsStateStateGet
        /// </summary>
        [Fact]
        public void ReadTeamsStateV2TeamsStateStateGetTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string state = null;
            //var response = instance.ReadTeamsStateV2TeamsStateStateGet(state);
            //Assert.IsType<List<Object>>(response);
        }

        /// <summary>
        /// Test ReadTeamsV2TeamsGet
        /// </summary>
        [Fact]
        public void ReadTeamsV2TeamsGetTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //bool? active = null;
            //bool? ascending = null;
            //string? country = null;
            //string? district = null;
            //int? limit = null;
            //string? metric = null;
            //bool? offseason = null;
            //int? offset = null;
            //string? state = null;
            //var response = instance.ReadTeamsV2TeamsGet(active, ascending, country, district, limit, metric, offseason, offset, state);
            //Assert.IsType<List<Object>>(response);
        }
    }
}
