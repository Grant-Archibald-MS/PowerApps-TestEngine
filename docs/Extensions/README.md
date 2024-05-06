# Overview

Test Engine provides an extension model using [modules](./modules.md). Modules provide the following key benefits:

- A "no cliffs" extensibility model that code first developer can implement to extend the functionality of the test engine
- An extensible method of defining login credentials for the test session
- A provider module that different web resources can implement to that Power Fx expressions can be used
- An action model that allows new Power Fx test functions to be defined to simplify testing
- A security model where extensions can be signed by a trusted certificate provider
- A allow/deny security model that provides granular control on how actions are implemented

## Architecture

The extension model of test engine can be visualized in the following way

![Overview of Test Engine extension with Security checks for signed assemblies and Allow/Deny and MEF contracts for user authentication, providers and actions](./media/TestEngineOverview.svg)

### Security Checks

Where a possible MEF extension does not meet the defined security checks, it will not be made available as part of test engine test runs.

#### Signed Assemblies

When using assemblies compiled in release mode the Test Engine will validate that:

- Assemblies are signed by a trusted Root certificate provider
- Assemblies and intermediate certificates are currently valid.

#### Allow / Deny List

As defined in [modules](./modules.md) optional allow deny lists can be used to control the following:

- Which MEF modules to allow / deny
- Which .Net namespaces are allow / deny
- Which .Net methods and properties and allow / deny.

Using these settings can provide a granular control of what modules are available and what code can be implemented in the a ITestEngineModule implementations.

### MEF Extensions

Test Engine will search the same folder as the Test Engine executables for the following MEF contracts

- **[IUserManager](..\..\src\Microsoft.PowerApps.TestEngine\Users\IUserManager.cs)** provide implementations to interact with the provider to authenticate the test session.
- **[ITestWebProvider](..\..\src\Microsoft.PowerApps.TestEngine\Providers\ITestWebProvider.cs)** allow extensions to build on the authenticated Playwright session to present the object model of the provider to and from the Power Fx test state.
- **[ITestEngineModule](..\..\src\Microsoft.PowerApps.TestEngine\Modules\ITestEngineModule.cs)** allow extensions to interact with network calls and define Power Fx functions used in a test

## No Cliffs Extensibility

The MEF extensibility model provides a method of extending the range of scenarios that Test Engine can cover. By implementing the defined MEF interfaces .Net assemblies can be implemented that provide alternative user authentication and web based tests.

The ITestEngineModule interface allows new Power FX functions to be defined that simplify testing by extending the provider or adding low code functions that are implemented in .Net.