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

using Org.OpenAPITools.Client;
using Org.OpenAPITools.Api;
// uncomment below to import models
//using Org.OpenAPITools.Model;

namespace Org.OpenAPITools.Test.Api


    /// <summary>
    ///  Class for testing EventApi
    /// </summary>
    /// <remarks>
    /// This file is automatically generated by OpenAPI Generator (https://openapi-generator.tech).
    /// Please update the test case below to test the API endpoint.
    /// </remarks>
    public class EventApiTests : IDisposable
    {
        private EventApi instance;

        public EventApiTests()
        {
            instance = new EventApi();
        }

        public void Dispose()
        {
            // Cleanup when everything is done.
        }

        /// <summary>
        /// Test an instance of EventApi
        /// </summary>
        [Fact]
        public void InstanceTest()
        {
            // TODO uncomment below to test 'IsType' EventApi
            //Assert.IsType<EventApi>(instance);
        }

        /// <summary>
        /// Test ReadEventV3EventEventGet
        /// </summary>
        [Fact]
        public void ReadEventV3EventEventGetTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string varEvent = null;
            //var response = instance.ReadEventV3EventEventGet(varEvent);
            //Assert.IsType<Object>(response);
        }

        /// <summary>
        /// Test ReadEventsV3EventsGet
        /// </summary>
        [Fact]
        public void ReadEventsV3EventsGetTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //int? year = null;
            //string? country = null;
            //string? state = null;
            //string? district = null;
            //string? type = null;
            //int? week = null;
            //bool? offseason = null;
            //string? metric = null;
            //bool? ascending = null;
            //int? limit = null;
            //int? offset = null;
            //var response = instance.ReadEventsV3EventsGet(year, country, state, district, type, week, offseason, metric, ascending, limit, offset);
            //Assert.IsType<List<Object>>(response);
        }
    }

