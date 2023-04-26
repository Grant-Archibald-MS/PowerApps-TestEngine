// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.PowerApps.TestEngine.Config;
using Microsoft.PowerApps.TestEngine.PowerFx.Functions;
using Microsoft.PowerApps.TestEngine.TestInfra;
using Microsoft.PowerApps.TestEngine.Tests.Helpers;
using Microsoft.PowerFx.Types;
using Moq;
using Xunit;

namespace Microsoft.PowerApps.TestEngine.Tests.PowerFx.Functions
{
    public class PauseFunctionTests
    {
        private Mock<ITestInfraFunctions> MockTestInfraFunctions;
        private Mock<ITestState> MockTestState;
        private Mock<ILogger> MockLogger;
        private TestSettings settings;

        public PauseFunctionTests()
        {
            MockTestInfraFunctions = new Mock<ITestInfraFunctions>(MockBehavior.Strict);
            MockTestState = new Mock<ITestState>(MockBehavior.Strict);
            MockLogger = new Mock<ILogger>(MockBehavior.Strict);
            settings = new TestSettings();
        }

        [Fact]
        public void PauseFunctionNotHeadlessSucceeds()
        {
            LoggingTestHelper.SetupMock(MockLogger);

            settings.Headless = false;

            MockTestInfraFunctions.Setup(mock => mock.PauseAsync()).Returns(Task.CompletedTask);
            MockTestState.Setup(mock => mock.GetTestSettings()).Returns(settings);

            var pauseFunction = new PauseFunction(MockTestInfraFunctions.Object, MockTestState.Object, MockLogger.Object);
            var result = pauseFunction.Execute();
            Assert.IsType<BlankValue>(result);
        }

        [Fact]
        public void PauseFunctionHeadlessSucceeds()
        {
            LoggingTestHelper.SetupMock(MockLogger);

            settings.Headless = true;

            MockTestState.Setup(mock => mock.GetTestSettings()).Returns(settings);

            var pauseFunction = new PauseFunction(MockTestInfraFunctions.Object, MockTestState.Object, MockLogger.Object);
            var result = pauseFunction.Execute();
        }
    }
}
