﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.PowerApps.TestEngine.Config;
using Microsoft.PowerApps.TestEngine.PowerApps;
using Moq;
using System;
using Xunit;

namespace Microsoft.PowerApps.TestEngine.Tests.PowerApps
{
    public class PowerAppsUrlMapperTests
    {
        private Mock<ITestState> MockTestState;
        private Mock<ISingleTestInstanceState> MockSingleTestInstanceState;

        public PowerAppsUrlMapperTests()
        {
            MockTestState = new Mock<ITestState>(MockBehavior.Strict);
            MockSingleTestInstanceState = new Mock<ISingleTestInstanceState>(MockBehavior.Strict);
        }

        [Theory]
        [InlineData("myEnvironment", "Prod", "https://make.powerapps.com/environments/myEnvironment/home")]
        [InlineData("defaultEnvironment", "Test", "https://make.test.powerapps.com/environments/defaultEnvironment/home")]
        [InlineData("defaultEnvironment", "test", "https://make.test.powerapps.com/environments/defaultEnvironment/home")]
        [InlineData("myEnvironment", "PROD", "https://make.powerapps.com/environments/myEnvironment/home")]
        [InlineData("myEnvironment", "prod", "https://make.powerapps.com/environments/myEnvironment/home")]
        [InlineData("defaultEnvironment", "", "https://make.powerapps.com/environments/defaultEnvironment/home")]
        [InlineData("defaultEnvironment", null, "https://make.powerapps.com/environments/defaultEnvironment/home")]
        public void GenerateLoginUrlTest(string environmentId, string? cloud, string expectedLoginUrl)
        {
            MockTestState.Setup(x => x.GetEnvironment()).Returns(environmentId);
            MockTestState.Setup(x => x.GetCloud()).Returns(cloud);
            var powerAppUrlMapper = new PowerAppsUrlMapper(MockTestState.Object, MockSingleTestInstanceState.Object);
            Assert.Equal(expectedLoginUrl, powerAppUrlMapper.GenerateLoginUrl());
            MockTestState.Verify(x => x.GetEnvironment(), Times.Once());
            MockTestState.Verify(x => x.GetCloud(), Times.Once());
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GenerateLoginUrlThrowsOnNoEnvironmentTest(string? environmentId)
        {
            MockTestState.Setup(x => x.GetEnvironment()).Returns(environmentId);
            var powerAppUrlMapper = new PowerAppsUrlMapper(MockTestState.Object, MockSingleTestInstanceState.Object);
            Assert.Throws<InvalidOperationException>(() => powerAppUrlMapper.GenerateLoginUrl());
        }

        [Theory]
        [InlineData("myEnvironment", "Prod", "myApp", "myTenant", "https://apps.powerapps.com/play/e/myEnvironment/an/myApp?tenantId=myTenant")]
        [InlineData("defaultEnvironment", "Test", "defaultApp", "defaultTenant", "https://apps.test.powerapps.com/play/e/defaultEnvironment/an/defaultApp?tenantId=defaultTenant")]
        [InlineData("defaultEnvironment", "test", "defaultApp", "defaultTenant", "https://apps.test.powerapps.com/play/e/defaultEnvironment/an/defaultApp?tenantId=defaultTenant")]
        [InlineData("myEnvironment", "PROD", "myApp", "myTenant", "https://apps.powerapps.com/play/e/myEnvironment/an/myApp?tenantId=myTenant")]
        [InlineData("myEnvironment", "prod", "myApp", "myTenant", "https://apps.powerapps.com/play/e/myEnvironment/an/myApp?tenantId=myTenant")]
        [InlineData("defaultEnvironment", "", "defaultApp", "defaultTenant", "https://apps.powerapps.com/play/e/defaultEnvironment/an/defaultApp?tenantId=defaultTenant")]
        [InlineData("defaultEnvironment", null, "defaultApp", "defaultTenant", "https://apps.powerapps.com/play/e/defaultEnvironment/an/defaultApp?tenantId=defaultTenant")]
        public void GenerateAppUrlTest(string environmentId, string? cloud, string appLogicalName, string tenantId, string expectedAppUrl)
        {
            MockTestState.Setup(x => x.GetEnvironment()).Returns(environmentId);
            MockTestState.Setup(x => x.GetCloud()).Returns(cloud);
            MockSingleTestInstanceState.Setup(x => x.GetTestDefinition()).Returns(new TestDefinition() { AppLogicalName = appLogicalName });
            MockTestState.Setup(x => x.GetTenant()).Returns(tenantId);
            var powerAppUrlMapper = new PowerAppsUrlMapper(MockTestState.Object, MockSingleTestInstanceState.Object);
            Assert.Equal(expectedAppUrl, powerAppUrlMapper.GenerateAppUrl());
            MockTestState.Verify(x => x.GetEnvironment(), Times.Once());
            MockTestState.Verify(x => x.GetCloud(), Times.Once());
            MockSingleTestInstanceState.Verify(x => x.GetTestDefinition(), Times.Once());
            MockTestState.Verify(x => x.GetTenant(), Times.Once());
        }

        [Theory]
        [InlineData("", "appLogicalName", "tenantId")]
        [InlineData(null, "appLogicalName", "tenantId")]
        [InlineData("environmentId", "", "tenantId")]
        [InlineData("environmentId", null, "tenantId")]
        [InlineData("environmentId", "appLogicalName", "")]
        [InlineData("environmentId", "appLogicalName", null)]
        public void GenerateLoginUrlThrowsOnInvalidSetupTest(string? environmentId, string? appLogicalName, string? tenantId)
        {
            MockTestState.Setup(x => x.GetEnvironment()).Returns(environmentId);
            MockSingleTestInstanceState.Setup(x => x.GetTestDefinition()).Returns(new TestDefinition() { AppLogicalName = appLogicalName });
            MockTestState.Setup(x => x.GetTenant()).Returns(tenantId);
            var powerAppUrlMapper = new PowerAppsUrlMapper(MockTestState.Object, MockSingleTestInstanceState.Object);
            Assert.Throws<InvalidOperationException>(() => powerAppUrlMapper.GenerateAppUrl());
        }

        [Fact]
        public void GenerateLoginUrlThrowsOnInvalidTestDefinitionTest()
        {
            TestDefinition testDefinition = null;
            MockTestState.Setup(x => x.GetEnvironment()).Returns("environmentId");
            MockSingleTestInstanceState.Setup(x => x.GetTestDefinition()).Returns(testDefinition);
            MockTestState.Setup(x => x.GetTenant()).Returns("tenantId");
            var powerAppUrlMapper = new PowerAppsUrlMapper(MockTestState.Object, MockSingleTestInstanceState.Object);
            Assert.Throws<InvalidOperationException>(() => powerAppUrlMapper.GenerateAppUrl());
        }
    }
}