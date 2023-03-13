// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.


/* 
    This is code that handles communicating with the published app. Half of this code should live within the published app.
*/
var testEnginePluginName = "TestEnginePlugin";
var isPluginRegistered = false;

var callbackDictionary = {};

class TestEnginePlugin {
    contructor() {

    }

    processResult(commandInfo) {
        if (commandInfo.args[1].successful) {
            callbackDictionary[commandInfo.args[0]].complete(commandInfo.args[1].message);
        } else {
            callbackDictionary[commandInfo.args[0]].error(commandInfo.args[1].message);
        }
        delete callbackDictionary[commandInfo.args[0]];
    }
}

function getAppMagic() {
    if ( typeof AppDependencyHandler === "undefined" || typeof AppDependencyHandler.containers === "undefined") {
        return undefined;
    }
    var key = Object.keys(AppDependencyHandler.containers).filter(k => k != AppDependencyHandler.prebuiltContainerId)
    if ( key.length == 0) {
        return undefined;
    }
    return AppDependencyHandler.containers[key[0]].AppMagic;
}

function executePublishedAppScript(scriptToExecute) {


    var promise = new Promise((resolve) => {
        var result = eval(scriptToExecute);
        resolve(result)
    });

    return promise;
}

function getOngoingActionsInPublishedApp() {
    return executePublishedAppScript("AppDependencyHandler.containers[Object.keys(AppDependencyHandler.containers).filter(k => k != AppDependencyHandler.prebuiltContainerId)[0]].AppMagic.AuthoringTool.Runtime.existsOngoingAsync()");
}

function getControlObjectModel () {
    return executePublishedAppScript("buildControlObjectModel()");
}

function getPropertyValueFromPublishedApp(itemPath) {
    var script = `getPropertyValueFromControl(${JSON.stringify(itemPath)})`;
    return executePublishedAppScript(script);
}

function selectControl(itemPath) {
    var script = `selectControl(${JSON.stringify(itemPath)})`;
    return executePublishedAppScript(script);
}

function interactWithControl(itemPath, value) {
    var script = "";
    if (isArray(Object.values(value))) {        
        var valuesJsonArr = [];
        var values = Object.values(value);
        for (var index in values) {
            valuesJsonArr[`${index}`] = `${JSON.stringify(values[index])}`;
        }
        var valueJson = `{"${itemPath.propertyName}":${valuesJsonArr}}`;
        script = `interactWithControl(${JSON.stringify(itemPath)}, ${valueJson})`;
    } else {
        var valueJson = `{"${itemPath.propertyName}":${value}}`;
        script = `interactWithControl(${JSON.stringify(itemPath)}, ${valueJson})`;
    }
    return executePublishedAppScript(script);
}

function setPropertyValueForControl(itemPath, value) {
    if (typeof value == "object") {
        return interactWithControl(itemPath, value);
    } else if (typeof value == "string") {
        value = JSON.stringify(value);
    }
    var script = `setPropertyValueForControl(${JSON.stringify(itemPath)}, ${value})`;
    return executePublishedAppScript(script);
}

function fetchArrayItemCount(itemPath) {
    var script = `fetchArrayItemCount(${JSON.stringify(itemPath)})`;
    return executePublishedAppScript(script);
}

function isArray(obj) {
    return obj.constructor === Array;
}

/*
 These are the functions that will be called by the Test Engine
*/

var PowerAppsTestEngine = {
    getAppStatus: function () {
        var appMagic = getAppMagic();
        if (typeof appMagic === "undefined" || typeof appMagic.Runtime === "undefined") {
            return "Loading";
        }
        //if (appMagic.Runtime.WebPlayerRuntime._appHostManager._appIsLoading) {
        //    return "Loading";
        //}
        //else {
            // Determine interaction required and error states

            // App is loaded, register plugin
            // When this is ported into PowerApps, need to do the proper plugin registration
            //if (!isPluginRegistered) {
            //    appMagic.Runtime.WebPlayerRuntime._appHostManager._apiHandler.registerHandler(testEnginePluginName, new TestEnginePlugin());
            //}
            return getOngoingActionsInPublishedApp().then((ongoingAppActionRunning) => {
                if (ongoingAppActionRunning) {
                    return "Busy";
                } else {
                    return "Idle";
                }
            });
        //}
    },

     buildObjectModel: function() {
        return getControlObjectModel().then((controlObjectModel) => {
            return {
                controls: controlObjectModel
            };
        })
    },

     getPropertyValue: function(itemPath) {
        return getPropertyValueFromPublishedApp(itemPath)
    },

    select: function(itemPath) {
        return selectControl(itemPath)
    },

    setPropertyValue: function(itemPath, value) {
        return setPropertyValueForControl(itemPath, value);
    },

     getItemCount: function(itemPath) {
        return fetchArrayItemCount(itemPath);
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

function parseControl(controlName, controlObject) {
    var propertiesList = [];
    var properties = Object.keys(controlObject.modelProperties);
    var controls = [];
    if (controlObject.controlWidget.replicatedContextManager) {
        var childrenControlContext = controlObject.controlWidget.replicatedContextManager.authoringAreaBindingContext.controlContexts;
        var childControlNames = Object.keys(childrenControlContext);
        childControlNames.forEach((childControlName) => {
            var childControlObject = childrenControlContext[childControlName];
            var childControlsList = parseControl(childControlName, childControlObject);
            controls = controls.concat(childControlsList);
        });
    }

    var appMagic = getAppMagic()

    var componentBindingContext = appMagic.Controls.GlobalContextManager.bindingContext.componentBindingContexts.lookup(controlName);
    if (componentBindingContext) {
        var componentChildrenControlContext = componentBindingContext.controlContexts;
        var componentChildrenControlNames = Object.keys(componentChildrenControlContext);
        componentChildrenControlNames.forEach((childControlName) => {
            if (childControlName !== controlName) {
                var componentChildControlObject = componentChildrenControlContext[childControlName];
                var childControlsList = parseControl(childControlName, componentChildControlObject);
                controls = controls.concat(childControlsList);
                propertiesList.push({ propertyName: childControlName, propertyType: childControlName });
            }
        });
    }

    properties.forEach((propertyName) => {
        var propertyType = controlObject.controlWidget.controlProperties[propertyName].propertyType;

        propertiesList.push({ propertyName: propertyName, propertyType: propertyType });
    })

    var control = { name: controlName, properties: propertiesList };
    controls.push(control);

    return controls;
}

function buildControlObjectModel() {

    var appMagic = getAppMagic()

    var controls = [];
    var controlContext = appMagic.Controls.GlobalContextManager.bindingContext.controlContexts;
    var controlNames = Object.keys(controlContext);
    controlNames.forEach((controlName) => {
        var control = controlContext[controlName];
        var controlList = parseControl(controlName, control);
        controls = controls.concat(controlList);
    });

    return controls;
}

function fetchArrayItemCount(itemPath) {
    if (itemPath.parentControl && itemPath.parentControl.index === null) {
        // Components do not have an item count
        throw "Not a gallery, no item count available. Most likely a component";
    }

    var appMagic = getAppMagic();

    var replicatedContexts = appMagic.Controls.GlobalContextManager.bindingContext.replicatedContexts

    if (itemPath.parentControl && itemPath.parentControl.index !== null) {
        // Nested gallery - Power Apps only supports one level of nesting so we don't have to go recursively to find it
        // Get parent replicated context
        replicatedContexts = OpenAjax.widget.byId(itemPath.parentControl.controlName).OpenAjax.getAuthoringControlContext()._replicatedContext.bindingContextAt(itemPath.parentControl.index).replicatedContexts;
    }

    var replicatedContext = OpenAjax.widget.byId(itemPath.controlName).OpenAjax.getAuthoringControlContext()._replicatedContext;

    if (!replicatedContext) {
        // This is not a gallery
        throw "Not a gallery, no item count available. Most likely a control";
    }

    var managerId = replicatedContext.manager.managerId;
    return replicatedContexts[managerId].getBindingContextCount()
}

function getBindingContext(itemPath) {
    var appMagic = getAppMagic()

    var bindingContext = appMagic.Controls.GlobalContextManager.bindingContext;

    if (itemPath.parentControl) {
        // Control is inside a component or gallery
        bindingContext = getBindingContext(itemPath.parentControl);
    }

    if (itemPath.index !== null) {
        // Gallery control
        var managerId = OpenAjax.widget.byId(itemPath.controlName).OpenAjax.getAuthoringControlContext()._replicatedContext.manager.managerId;
        return bindingContext.replicatedContexts[managerId].bindingContextAt(itemPath.index);
    }

    var componentBindingContext = appMagic.Controls.GlobalContextManager.bindingContext.componentBindingContexts.lookup(itemPath.controlName);

    if (typeof componentBindingContext !== 'undefined') {
        // Component control
        return componentBindingContext;
    }

    return bindingContext;
}

function getPropertyValueFromControl(itemPath) {
    var bindingContext = getBindingContext(itemPath);

    var propertyValue = bindingContext.controlContexts[itemPath.controlName].modelProperties[itemPath.propertyName].getValue();

    return {
        propertyValue: propertyValue
    };
}

function selectControl(itemPath) {
    var appMagic = getAppMagic();

    var screenId = appMagic.AuthoringTool.Runtime.getNamedControl(appMagic.AuthoringTool.Runtime.getCurrentScreenName()).OpenAjax.uniqueId;

    if (itemPath.parentControl && itemPath.parentControl.index !== null ) {
            // Gallery
        var bindingContext = appMagic.Controls.GlobalContextManager.bindingContext;
        if (itemPath.parentControl.parentControl) {
            // Nested gallery
            bindingContext = getBindingContext(itemPath.parentControl);
            var currentControl = bindingContext.controlContexts[itemPath.controlName].controlWidget;
            return appMagic.Functions.select(null,
                                    bindingContext,
                                    currentControl.control,
                                    null, // row number
                                    null, // child control
                                    screenId)
        }
        else {
            // One level gallery - the nested gallery approach doesn't work for one level so we have to do it differently
            // select function is starts with 1, while the C# code indexes from 0
            rowOrColumnNumber = itemPath.parentControl.index + 1;
            return appMagic.Functions.select(null,
                bindingContext,
                appMagic.AuthoringTool.Runtime.getNamedControl(itemPath.parentControl.controlName),
                rowOrColumnNumber,
                appMagic.AuthoringTool.Runtime.getNamedControl(itemPath.controlName).OpenAjax._icontrol,
                screenId)
        }        
    }

    if (itemPath.parentControl) {
        // Component
        var bindingContext = appMagic.Controls.GlobalContextManager.bindingContext.componentBindingContexts.lookup(itemPath.parentControl.controlName);
        var buttonWidget = bindingContext.controlContexts[itemPath.controlName].controlWidget;
        var controlContext = buttonWidget.getOnSelectControlContext(bindingContext);
        buttonWidget.select(controlContext);
        return true;
    }

    return appMagic.Functions.select(null,
        appMagic.Controls.GlobalContextManager.bindingContext,
        appMagic.AuthoringTool.Runtime.getNamedControl(itemPath.controlName),
        null,
        null,
        screenId);
}

function setPropertyValueForControl(itemPath, value) {
    var appMagic = getAppMagic()

    if (itemPath.parentControl && itemPath.parentControl.index !== null) {
        // Gallery & Nested gallery
        var galleryBindingContext = getBindingContext(itemPath.parentControl);     
        return appMagic.AuthoringTool.Runtime.getNamedControl(itemPath.controlName, galleryBindingContext).OpenAjax.setPropertyValueInternal(itemPath.propertyName, value, galleryBindingContext)
    }

    if (itemPath.parentControl) {
        // Component
        var componentBindingContext = appMagic.Controls.GlobalContextManager.bindingContext.componentBindingContexts.lookup(itemPath.parentControl.controlName);
        return (appMagic.AuthoringTool.Runtime.getNamedControl(itemPath.controlName, componentBindingContext).OpenAjax.setPropertyValueInternal(itemPath.propertyName, value, componentBindingContext));
    }
    
    return appMagic.AuthoringTool.Runtime.getNamedControl(itemPath.controlName, appMagic.Controls.GlobalContextManager.bindingContext).OpenAjax.setPropertyValueInternal(itemPath.propertyName, value, appMagic.Controls.GlobalContextManager.bindingContext);
}

function interactWithControl(itemPath, value) {
    var appMagic = getAppMagic()

    var e = appMagic.AuthoringTool.Runtime.getNamedControl(itemPath.controlName, appMagic.Controls.GlobalContextManager.bindingContext).OpenAjax.interactWithControlAsync(appMagic.Controls.GlobalContextManager.bindingContext.controlContexts[itemPath.controlName], value).then(() => true,() => false);

    return e._value;
}
