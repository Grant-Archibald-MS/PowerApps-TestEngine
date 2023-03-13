// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Extensions.Logging;
using Microsoft.PowerApps.TestEngine.Config;

namespace Microsoft.PowerApps.TestEngine.PowerApps
{
    /// <summary>
    /// Map urls
    /// </summary>
    public class PowerAppsUrlMapper : IUrlMapper
    {
        private readonly ITestState _testState;
        private readonly ISingleTestInstanceState _singleTestInstanceState;

        public PowerAppsUrlMapper(ITestState testState, ISingleTestInstanceState singleTestInstanceState)
        {
            _testState = testState;
            _singleTestInstanceState = singleTestInstanceState;
        }

        public static bool IsModelApplication(string domain) {
            return !string.IsNullOrEmpty(domain) && domain.IndexOf("dynamics.com") >= 0;
        }

        public string GenerateTestUrl(string domain, string additionalQueryParams)
        {
            
            var environment = _testState.GetEnvironment();
            if (string.IsNullOrEmpty(environment) && !IsModelApplication(domain) )
            {
                _singleTestInstanceState.GetLogger().LogError("Environment cannot be empty.");
                throw new InvalidOperationException();
            }

            var testSuiteDefinition = _singleTestInstanceState.GetTestSuiteDefinition();
            if (testSuiteDefinition == null)
            {
                _singleTestInstanceState.GetLogger().LogError("Test definition must be specified.");
                throw new InvalidOperationException();
            }

            var appLogicalName = testSuiteDefinition.AppLogicalName;
            var appId = testSuiteDefinition.AppId;

            if (string.IsNullOrEmpty(appLogicalName) && string.IsNullOrEmpty(appId))
            {
                _singleTestInstanceState.GetLogger().LogError("At least one of the App Logical Name or App Id must be defined.");
                throw new InvalidOperationException();
            }

            
            if ( IsModelApplication(domain) ) {
                return $"https://{domain}/main.aspx?appid={appId}&pagetype=custom&name={appLogicalName}";
            }

            var tenantId = _testState.GetTenant();
            if (string.IsNullOrEmpty(tenantId))
            {
                _singleTestInstanceState.GetLogger().LogError("Tenant cannot be empty.");
                throw new InvalidOperationException();
            }

            var queryParametersForTestUrl = GetQueryParametersForTestUrl(tenantId, additionalQueryParams);

            return !string.IsNullOrEmpty(appLogicalName) ?
                   $"https://{domain}/play/e/{environment}/an/{appLogicalName}{queryParametersForTestUrl}" :
                   $"https://{domain}/play/e/{environment}/a/{appId}{queryParametersForTestUrl}";
        }

        private static string GetQueryParametersForTestUrl(string tenantId, string additionalQueryParams)
        {
            return $"?tenantId={tenantId}&source=testengine{additionalQueryParams}";
        }
    }
}
