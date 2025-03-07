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
    ///  Class for testing MatchApi
    /// </summary>
    /// <remarks>
    /// This file is automatically generated by OpenAPI Generator (https://openapi-generator.tech).
    /// Please update the test case below to test the API endpoint.
    /// </remarks>
    public class MatchApiTests : IDisposable
    {
        private MatchApi instance;

        public MatchApiTests()
        {
            instance = new MatchApi();
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
        /// Test ReadMatchV3MatchMatchGet
        /// </summary>
        [Fact]
        public void ReadMatchV3MatchMatchGetTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string match = null;
            //var response = instance.ReadMatchV3MatchMatchGet(match);
            //Assert.IsType<Object>(response);
        }

        /// <summary>
        /// Test ReadMatchesV3MatchesGet
        /// </summary>
        [Fact]
        public void ReadMatchesV3MatchesGetTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //bool? ascending = null;
            //bool? elim = null;
            //int? limit = null;
            //string? metric = null;
            //bool? offseason = null;
            //int? offset = null;
            //string? team = null;
            //string? varEvent = null;
            //int? week = null;
            //int? year = null;
            //var response = instance.ReadMatchesV3MatchesGet(ascending, elim, limit, metric, offseason, offset, team, varEvent, week, year);
            //Assert.IsType<Collection<Object>>(response);
        }
    }

