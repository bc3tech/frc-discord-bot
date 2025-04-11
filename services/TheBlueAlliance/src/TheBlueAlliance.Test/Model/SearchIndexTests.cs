/*
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).
 *
 * The version of the OpenAPI document: 3.9.7
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

using Xunit;

using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using TheBlueAlliance.Model;
using TheBlueAlliance.Client;
using System.Reflection;

namespace TheBlueAlliance.Test.Model


    /// <summary>
    ///  Class for testing SearchIndex
    /// </summary>
    /// <remarks>
    /// This file is automatically generated by OpenAPI Generator (https://openapi-generator.tech).
    /// Please update the test case below to test the model.
    /// </remarks>
    public class SearchIndexTests : IDisposable
    {
        // TODO uncomment below to declare an instance variable for SearchIndex
        //private SearchIndex instance;

        public SearchIndexTests()
        {
            // TODO uncomment below to create an instance of SearchIndex
            //instance = new SearchIndex();
        }

        public void Dispose()
        {
            // Cleanup when everything is done.
        }

        /// <summary>
        /// Test an instance of SearchIndex
        /// </summary>
        [Fact]
        public void SearchIndexInstanceTest()
        {
            // TODO uncomment below to test "IsType" SearchIndex
            //Assert.IsType<SearchIndex>(instance);
        }



        /// <summary>
        /// Test the property 'Events'
        /// </summary>
        [Fact]
        public void EventsTest()
        {
            // TODO unit test for the property 'Events'
        }


        /// <summary>
        /// Test the property 'Teams'
        /// </summary>
        [Fact]
        public void TeamsTest()
        {
            // TODO unit test for the property 'Teams'
        }
    }
}
