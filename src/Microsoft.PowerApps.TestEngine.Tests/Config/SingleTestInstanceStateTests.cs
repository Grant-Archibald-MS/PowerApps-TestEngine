﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Extensions.Logging;
using Microsoft.PowerApps.TestEngine.Config;
using Moq;
using System;
using Xunit;

namespace Microsoft.PowerApps.TestEngine.Tests.Config
{
    public class SingleTestInstanceStateTests
    {
        [Fact]
        public void SingleTestInstanceStateSuccessTest()
        {
            var state = new SingleTestInstanceState();

            var testRunId = Guid.NewGuid().ToString();
            state.SetTestRunId(testRunId);
            Assert.Equal(testRunId, state.GetTestRunId());

            var testId = Guid.NewGuid().ToString();
            state.SetTestId(testId);
            Assert.Equal(testId, state.GetTestId());

            var testDefinition = new TestDefinition();
            state.SetTestDefinition(testDefinition);
            Assert.Equal(testDefinition, state.GetTestDefinition());

            var logger = new Mock<ILogger>(MockBehavior.Strict);
            state.SetLogger(logger.Object);
            Assert.Equal(logger.Object, state.GetLogger());

            var testResultsDirectory = Guid.NewGuid().ToString();
            state.SetTestResultsDirectory(testResultsDirectory);
            Assert.Equal(testResultsDirectory, state.GetTestResultsDirectory());

            var browserConfig = new BrowserConfiguration();
            state.SetBrowserConfig(browserConfig);
            Assert.Equal(browserConfig, state.GetBrowserConfig());
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SetTestRunIdThrowsOnInvalidInputTest(string invalidInput)
        {
            var state = new SingleTestInstanceState();
            Assert.Throws<ArgumentNullException>(() => state.SetTestRunId(invalidInput));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SetTestIdThrowsOnInvalidInputTest(string invalidInput)
        {
            var state = new SingleTestInstanceState();
            Assert.Throws<ArgumentNullException>(() => state.SetTestId(invalidInput));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SetTestResultsDirectoryThrowsOnInvalidInputTest(string invalidInput)
        {
            var state = new SingleTestInstanceState();
            Assert.Throws<ArgumentNullException>(() => state.SetTestResultsDirectory(invalidInput));
        }

        [Fact]
        public void SetTestDefinitionThrowsOnNullInput()
        {
            var state = new SingleTestInstanceState();
            Assert.Throws<ArgumentNullException>(() => state.SetTestDefinition(null));
        }

        [Fact]
        public void SetLoggerThrowsOnNullInput()
        {
            var state = new SingleTestInstanceState();
            Assert.Throws<ArgumentNullException>(() => state.SetLogger(null));
        }

        [Fact]
        public void SetBrowserConfigThrowsOnNullInput()
        {
            var state = new SingleTestInstanceState();
            Assert.Throws<ArgumentNullException>(() => state.SetBrowserConfig(null));
        }
    }
}