angular.module("umbraco").controller("Our.Umbraco.Contentment.DataEditors.DataTable.Controller", [
    "$scope",
    function ($scope) {

        //console.log("model", $scope.model);

        var defaultConfig = { fields: [], disableSorting: 0, maxItems: 0, usePrevalueEditors: 1 };
        var config = angular.merge({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || [];

            vm.headings = _.pluck(config.fields, "label");
            vm.items = [];

            _.each($scope.model.value, function (value, row) {
                var fields = angular.copy(config.fields);

                _.each(fields, function (field, cell) {
                    field.alias = [$scope.model.alias, row, cell].join("_");
                    field.value = value[field.key];
                });

                vm.items.push(fields);
            });

            vm.usePrevalueEditors = Object.toBoolean(config.usePrevalueEditors) ? true : null;
            vm.allowAdd = (config.maxItems === 0 || config.maxItems === "0") || vm.items.length < config.maxItems;
            vm.allowRemove = true;
            vm.sortable = Object.toBoolean(config.disableSorting) === false && (config.maxItems !== 1 && config.maxItems !== "1");

            vm.sortableOptions = {
                axis: "y",
                containment: "parent",
                cursor: "move",
                disabled: !vm.sortable,
                forcePlaceholderSize: true,
                handle: ".handle",
                helper: function (e, ui) {
                    ui.children().each(function () {
                        $(this).width($(this).width());
                    });
                    return ui.clone();
                },
                items: "> tr",
                opacity: 0.7,
                scroll: true,
                tolerance: "pointer",
                stop: function (e, ui) {
                    setDirty();
                }
            };

            vm.showPrompt = showPrompt;
            vm.hidePrompt = hidePrompt;

            vm.add = add;
            vm.remove = remove;
        };

        function add($event) {
            vm.items.push(angular.copy(config.fields));

            if ((config.maxItems !== 0 && config.maxItems !== "0") && vm.items.length >= config.maxItems) {
                vm.allowAdd = false;
            }

            setDirty();
        };

        function remove($index) {
            vm.items.splice($index, 1);

            if ((config.maxItems === 0 || config.maxItems === "0") || vm.items.length < config.maxItems) {
                vm.allowAdd = true;
            }

            setDirty();
        };

        function showPrompt(row) {
            row.prompt = true;
        }

        function hidePrompt(row) {
            delete row.prompt;
        }

        function setDirty() {
            if ($scope.propertyForm) {
                $scope.propertyForm.$setDirty();
            }
        };

        var unsubscribe = $scope.$on("formSubmitting", function (ev, args) {
            $scope.model.value = [];
            _.each(vm.items, function (row) {
                var obj = {};

                _.each(row, function (cell) {
                    obj[cell.key] = cell.value;
                });

                $scope.model.value.push(obj);
            });
        });

        $scope.$on("$destroy", function () {
            unsubscribe();
        });

        init();
    }
]);
