# Playwright Action

The Playwright action function is designed to allows [playwright locator](https://playwright.dev/docs/locators#locate-by-css-or-xpath) and [actions](https://playwright.dev/dotnet/docs/api/class-locator) to be called to include interaction with the overall Power Platform site and Power Apps Component Framework HTML components inside a Power App test.

## Locator and Actions

PlaywrightAction(url, "Navigate")

- url: Contain the https web address to navigate
    {EnvironmentId} - Replaces the Environment Id in the locator url
    {ApplicationId} - Replaces the Application Id in the locator url

PlaywrightAction(locator, action)

- locator: Contains the string to [selector](https://playwright.dev/dotnet/docs/api/class-page#page-locator) for the current page
- action: Contains string of the locator action to execute. For example
    "Clear" - Clear the input field
    "Click" - Click an element
    "Check" - Ensure that checkbox or radio element is checked.
    "Hover" - Hover over the matching element
    "Tap" - Perform a tap gesture on the element matching the locator
    "Uncheck" - Ensure that checkbox or radio element is unchecked
    "Wait" - Wait until locator exists

PlaywrightAction(locator, action, value)

- action: Contains string of the locator action to execute. For example
    "Fill" - Set a value to the input field
    "SelectOption" - Selects option or options (comma delimited)
- value: Text value of action

- ## Sample

The [Playwright Action](../../samples/playwrightaction/testPlan.fx.yaml) sample provides a work in progress example of using the PlaywrightAction function.

## Examples

`PlaywrightAction("https://admin.powerplatform.microsoft.com/environments/environment/{EnvironmentId}/hub", "Navigate")`

`PlaywrightAction("button:has-text('Open')", "Click")`
