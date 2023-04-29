// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Extensions.Logging;
using Microsoft.PowerApps.TestEngine.Config;
using Microsoft.PowerApps.TestEngine.System;
using Microsoft.PowerApps.TestEngine.TestInfra;
using Microsoft.PowerFx;
using Microsoft.PowerFx.Types;

namespace Microsoft.PowerApps.TestEngine.PowerFx.Functions
{
    /// <summary>
    /// This will reload the current page
    /// </summary>
    public class ReloadFunction : ReflectionFunction
    {
        private readonly ITestInfraFunctions _testInfraFunctions;
        private readonly ITestState _testState;
        private readonly ILogger _logger;

        public ReloadFunction(ITestInfraFunctions testInfraFunctions, ITestState testState, ILogger logger)
            : base("Reload", FormulaType.Blank)
        {
            _testInfraFunctions = testInfraFunctions;
            _testState = testState;
            _logger = logger;
        }

        public BlankValue Execute()
        {
            _logger.LogInformation("------------------------------\n\n" +
                "Executing Refresh function.");

            _testInfraFunctions.ReloadAsync().Wait();
            
            _logger.LogInformation("Successfully finished executing Reload function.");
            

            return FormulaValue.NewBlank();
        }
    }
}
