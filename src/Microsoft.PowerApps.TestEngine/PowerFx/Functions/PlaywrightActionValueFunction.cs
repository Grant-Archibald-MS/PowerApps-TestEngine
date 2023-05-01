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
    public class PlaywrightActionValueFunction : ReflectionFunction
    {
        private readonly ITestInfraFunctions _testInfraFunctions;
        private readonly ISingleTestInstanceState _singleTestInstanceState;
        private readonly IFileSystem _fileSystem;
        private readonly ITestState _testState;
        private readonly ILogger _logger;

        public PlaywrightActionValueFunction(ITestInfraFunctions testInfraFunctions, ISingleTestInstanceState singleTestInstanceState, IFileSystem fileSystem, ITestState testState, ILogger logger)
            : base("PlaywrightActionValue", FormulaType.Blank, FormulaType.String, FormulaType.String, FormulaType.String)
        {
            _testInfraFunctions = testInfraFunctions;
            _singleTestInstanceState = singleTestInstanceState;
            _fileSystem = fileSystem;
            _testState = testState;
            _logger = logger;
        }


        public BooleanValue Execute(StringValue locator, StringValue action, StringValue value)
        {
            _logger.LogInformation("------------------------------\n\n" +
                "Executing Playwright Action function.");

            if (string.IsNullOrEmpty(locator.Value))
            {
                _logger.LogError("locator cannot be empty.");
                throw new ArgumentException();
            }

            switch ( action.Value.ToLower() ) {
                case "fill":
                    _testInfraFunctions.FillAsync(locator.Value, value.Value).Wait();
                    break;
                case "screenshot":
                    var testResultDirectory = _singleTestInstanceState.GetTestResultsDirectory();
                    if (!_fileSystem.IsValidFilePath(testResultDirectory))
                    {
                        _logger.LogError("Test result directory needs to be set.");
                        throw new InvalidOperationException();
                    }

                    var fileName = value.Value;

                    if (string.IsNullOrEmpty(fileName))
                    {
                        _logger.LogTrace("File Name: " + nameof(fileName));
                        _logger.LogError("File must exist and cannot be empty.");
                        throw new ArgumentException();
                    }

                    if (Path.IsPathRooted(fileName))
                    {
                        _logger.LogError("Only support relative file paths");
                        throw new ArgumentException();
                    }

                    if (!fileName.EndsWith(".jpg") && !fileName.EndsWith(".jpeg") && !fileName.EndsWith("png"))
                    {
                        _logger.LogDebug("File extension: " + Path.GetExtension(fileName));
                        _logger.LogTrace("File name: " + fileName);
                        _logger.LogError("Only support jpeg and png files");
                        throw new ArgumentException();
                    }

                    var filePath = Path.Combine(testResultDirectory, fileName);

                    _logger.LogInformation("Screenshot item");
                    _testInfraFunctions.ScreenshotAsync(locator.Value, filePath).Wait();
                    break;
                default:
                    _logger.LogError("Action not found " + action.Value);
                    throw new ArgumentException();
            } 

            _logger.LogInformation("Successfully finished executing Playwright Action function.");

            return BooleanValue.New(true);
        }
    }
}
