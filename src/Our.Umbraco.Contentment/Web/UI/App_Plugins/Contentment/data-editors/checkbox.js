angular.module("umbraco").controller("Our.Umbraco.Contentment.DataEditors.Checkbox.Controller", function ($scope) {

    var vm = this;
    vm.alias = $scope.model.alias;
    vm.true = 1;
    vm.false = 0;

    if ($scope.model.config.showInline === 1 || $scope.model.config.showInline === "1") {
        vm.showInline = true;
        vm.label = $scope.model.label;
        vm.description = $scope.model.description;
    }

    $scope.model.value = ($scope.model.value === 1 || $scope.model.value === "1") ? vm.true : vm.false;
});
