angular.module("umbraco").controller("Our.Umbraco.Contentment.DataEditors.Checkbox.Controller", function ($scope) {

    var defaultConfig = { showInline: 0 };
    var config = angular.merge({}, defaultConfig, $scope.model.config);

    var vm = this;

    function init() {

        vm.alias = $scope.model.alias;
        vm.true = 1;
        vm.false = 0;

        if (Object.toBoolean(config.showInline)) {
            vm.showInline = true;
            vm.label = $scope.model.label;
            vm.description = $scope.model.description;
        }

        $scope.model.value = Object.toBoolean($scope.model.value) ? vm.true : vm.false;

    };

    init();
});
