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
    public class ReloadFunctionTests
    {
        private Mock<ITestInfraFunctions> MockTestInfraFunctions;
        private Mock<ITestState> MockTestState;
        private Mock<ILogger> MockLogger;
        private TestSettings settings;

        public ReloadFunctionTests()
        {
            MockTestInfraFunctions = new Mock<ITestInfraFunctions>(MockBehavior.Strict);
            MockTestState = new Mock<ITestState>(MockBehavior.Strict);
            MockLogger = new Mock<ILogger>(MockBehavior.Strict);
            settings = new TestSettings();
        }

      

        [Fact]
        public void RereshFunctionsSucceeds()
        {
            LoggingTestHelper.SetupMock(MockLogger);

            MockTestInfraFunctions.Setup(x => x.ReloadAsync()).Returns(Task.CompletedTask);

            var pauseFunction = new ReloadFunction(MockTestInfraFunctions.Object, MockTestState.Object, MockLogger.Object);
            var result = pauseFunction.Execute();
        }
    }
}
