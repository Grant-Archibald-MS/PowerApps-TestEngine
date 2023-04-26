# Pause

The Pause function in Test Engine can be particularly useful for debugging and investigating issues during test execution. However, it is important to note that this function will only be executed when headless mode is set to false, so that the browser is visible.

When running tests in headless mode, the browser runs in the background without a graphical interface. This makes it difficult to interact with the browser and investigate the state of the web page during test execution. However, when headless mode is set to false, the browser is displayed on the screen, allowing developers to interact with it and inspect the DOM and other elements in real-time.

By using the Pause function with the browser visible, developers can pause the test execution at a specific point in the code, investigate the browser state, and debug any issues that may have arisen. They can use the browser's developer tools to inspect the DOM, check for errors, and test different scenarios, making it easier to identify and resolve any issues.

Once ready you can continue execution of the test by Using the Resume from the inspector. More information on the debug / inspection is available from the underlying Playwright [Run a test from a specific breakpoint](https://playwright.dev/dotnet/docs/debug#run-a-test-from-a-specific-breakpoint) documentation.

## Configuration

Read the [test settings](../Yaml/testSettings.md) documentation on how to configure headless mode.

## Sample

The [Button Clicker Pause](../../samples/buttonclicker/testPlan-pause.fx.yaml) sample provides an example of using the Pause function.

## Example

`Pause()`