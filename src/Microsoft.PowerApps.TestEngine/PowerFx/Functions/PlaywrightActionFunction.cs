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
    /// This will execute playwright actions for the current page
    /// </summary>
    public class PlaywrightActionFunction : ReflectionFunction
    {
        private readonly ITestInfraFunctions _testInfraFunctions;
        private readonly ITestState _testState;
        private readonly ILogger _logger;

        public PlaywrightActionFunction(ITestInfraFunctions testInfraFunctions, ITestState testState, ILogger logger)
            : base("PlaywrightAction", FormulaType.Blank, FormulaType.String, FormulaType.String)
        {
            _testInfraFunctions = testInfraFunctions;
            _testState = testState;
            _logger = logger;
        }


        public BooleanValue Execute(StringValue locator, StringValue action)
        {
            _logger.LogInformation("------------------------------\n\n" +
                "Executing Playwright Action function.");

            switch ( locator.Value.ToLower() ) {
                case "disablepowerfxmodel":
                    _testState.GetTestSettings().DisablePowerFxModel = Boolean.Parse(action.Value);
                    return BooleanValue.New(true);
                case "sleep":
                    Thread.Sleep(int.Parse(action.Value));
                    return BooleanValue.New(true);
            }

            if (string.IsNullOrEmpty(locator.Value))
            {
                _logger.LogError("locator cannot be empty.");
                throw new ArgumentException();
            }

            switch ( action.Value.ToLower() ) {
                case "click":
                    _logger.LogInformation("Click item");
                    _testInfraFunctions.ClickAsync(locator.Value).Wait();
                    break;
                case "navigate":
                    _logger.LogInformation("Navigate to page");
                    string url = locator.Value;
                    _testInfraFunctions.GoToUrlAsync(url).Wait();
                    break;
                case "wait":
                    _logger.LogInformation("Wait for locator");
                    _testInfraFunctions.WaitAsync(locator.Value).Wait();
                    break;
                case "exists":
                    _logger.LogInformation("Check if locator exists");
                    Task<bool> result = _testInfraFunctions.ExistsAsync(locator.Value);
                    return BooleanValue.New(result.GetAwaiter().GetResult());
                default:
                    _logger.LogError("Action not found " + action.Value);
                    throw new ArgumentException();
            } 

            _logger.LogInformation("Successfully finished executing Playwright Action function.");

            return BooleanValue.New(true);
        }
    }
}
