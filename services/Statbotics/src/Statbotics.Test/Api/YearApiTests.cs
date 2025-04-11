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

namespace Statbotics.Test.Api


    /// <summary>
    ///  Class for testing YearApi
    /// </summary>
    /// <remarks>
    /// This file is automatically generated by OpenAPI Generator (https://openapi-generator.tech).
    /// Please update the test case below to test the API endpoint.
    /// </remarks>
    public class YearApiTests : IDisposable
    {
        private YearApi instance;

        public YearApiTests()
        {
            instance = new YearApi();
        }

        public void Dispose()
        {
            // Cleanup when everything is done.
        }

        /// <summary>
        /// Test an instance of YearApi
        /// </summary>
        [Fact]
        public void InstanceTest()
        {
            // TODO uncomment below to test 'IsType' YearApi
            //Assert.IsType<YearApi>(instance);
        }

        /// <summary>
        /// Test ReadYearV3YearYearGet
        /// </summary>
        [Fact]
        public void ReadYearV3YearYearGetTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //int year = null;
            //var response = instance.ReadYearV3YearYearGet(year);
            //Assert.IsType<Object>(response);
        }

        /// <summary>
        /// Test ReadYearsV3YearsGet
        /// </summary>
        [Fact]
        public void ReadYearsV3YearsGetTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //bool? ascending = null;
            //int? limit = null;
            //string? metric = null;
            //int? offset = null;
            //var response = instance.ReadYearsV3YearsGet(ascending, limit, metric, offset);
            //Assert.IsType<Collection<Object>>(response);
        }
    }

