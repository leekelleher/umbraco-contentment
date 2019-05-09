angular.module("umbraco").controller("Our.Umbraco.Contentment.DataEditors.Dropdown.Controller", [
    "$scope",
    function ($scope) {

        console.log("model", $scope.model);

        var defaultConfig = { items: [] };
        var config = angular.merge({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {
            $scope.model.value = $scope.model.value || "";
            vm.items = config.items;
        }

        init();
    }
]);
