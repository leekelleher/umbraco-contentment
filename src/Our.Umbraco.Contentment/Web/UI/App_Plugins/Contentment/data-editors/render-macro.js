angular.module("umbraco").controller("Our.Umbraco.Contentment.DataEditors.RenderMacro.Controller", [
    "$scope",
    "$routeParams",
    "editorService",
    "macroResource",
    function ($scope, $routeParams, editorService, macroResource) {

        var vm = this;
        vm.html = "";

        // TODO: Show the "loading" panel thing.

        if (_.isEmpty($scope.model.config.macro) === false) {
            var macro = _.first($scope.model.config.macro);
            macroResource.getMacroResultAsHtmlForEditor(macro.alias, $routeParams.id, macro.params).then(function (result) {
                vm.html = result;
            });

        } else {
            vm.html = "<p>Please configure this data-type.</p>"; // TODO: Make this as a warning. [LK]
        }
    }
]);
