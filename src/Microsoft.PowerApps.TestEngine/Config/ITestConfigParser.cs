﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.PowerApps.TestEngine.Config
{
    /// <summary>
    /// Handles parsing the test configuration
    /// </summary>
    public interface ITestConfigParser
    {
        public TestPlanDefinition ParseTestConfig(string testConfigFilePath);
    }
}