﻿/* Copyright 2013-2014 MongoDB Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using FluentAssertions;
using MongoDB.Driver.Core.Clusters;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.ConnectionPools;
using MongoDB.Driver.Core.Connections;
using MongoDB.Driver.Core.Servers;
using MongoDB.Driver.Core.Helpers;
using NSubstitute;
using NUnit.Framework;

namespace MongoDB.Driver.Core.ConnectionPools
{
    [TestFixture]
    public class ExclusiveConnectionPoolFactoryTests
    {
        private IConnectionFactory _connectionFactory;
        private DnsEndPoint _endPoint;
        private ServerId _serverId;
        private ConnectionPoolSettings _settings;

        [SetUp]
        public void Setup()
        {
            _connectionFactory = Substitute.For<IConnectionFactory>();
            _endPoint = new DnsEndPoint("localhost", 27017);
            _serverId = new ServerId(new ClusterId(), _endPoint);
            _settings = new ConnectionPoolSettings()
                .WithMaintenanceInterval(Timeout.InfiniteTimeSpan)
                .WithMaxConnections(4)
                .WithMinConnections(2)
                .WithWaitQueueSize(1);
        }

        [Test]
        public void Constructor_should_throw_when_settings_is_null()
        {
            Action act = () => new ExclusiveConnectionPoolFactory(null, _connectionFactory, null);

            act.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void Constructor_should_throw_when_connectionFactory_is_null()
        {
            Action act = () => new ExclusiveConnectionPoolFactory(_settings, null, null);

            act.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void CreateConnectionPool_should_throw_when_serverId_is_null()
        {
            var subject = new ExclusiveConnectionPoolFactory(_settings, _connectionFactory, null);

            Action act = () => subject.CreateConnectionPool(null, _endPoint);
            act.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void CreateConnectionPool_should_throw_when_endPoint_is_null()
        {
            var subject = new ExclusiveConnectionPoolFactory(_settings, _connectionFactory, null);

            Action act = () => subject.CreateConnectionPool(_serverId, null);
            act.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void CreateConnectionPool_should_return_a_ConnectionPool()
        {
            var subject = new ExclusiveConnectionPoolFactory(_settings, _connectionFactory, null);

            var result = subject.CreateConnectionPool(_serverId, _endPoint);

            result.Should().NotBeNull();
            result.Should().BeOfType<ExclusiveConnectionPool>();
        }
    }
}