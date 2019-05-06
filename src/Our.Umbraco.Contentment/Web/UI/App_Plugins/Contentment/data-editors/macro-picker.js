angular.module("umbraco").controller("Our.Umbraco.Contentment.DataEditors.MacroPicker.Controller", [
    "$scope",
    "$routeParams",
    "editorService",
    "macroResource",
    "macroService",
    function ($scope, $routeParams, editorService, macroResource, macroService) {

        $scope.model.value = $scope.model.value || [];

        var config = angular.merge({}, { maxItems: 0 }, $scope.model.config);

        var vm = this;
        vm.icon = "icon-settings-alt";
        vm.allowAdd = (config.maxItems === 0 || config.maxItems === "0") || $scope.model.value.length < config.maxItems;
        vm.allowEdit = true;
        vm.allowRemove = true;
        vm.published = true;
        vm.sortable = config.maxItems !== 1 && config.maxItems !== "1";

        vm.sortableOptions = {
            axis: "y",
            containment: "parent",
            cursor: "move",
            disabled: !vm.sortable,
            opacity: 0.7,
            scroll: true,
            tolerance: "pointer",
            stop: function (e, ui) {
                setDirty();
            }
        };

        vm.add = add;
        vm.edit = edit;
        vm.remove = remove;

        function add($event) {
            var macroPicker = {
                dialogData: { richTextEditor: false, macroData: { macroAlias: "" } },
                submit: function (model) {
                    $scope.model.value.push({
                        udi: model.selectedMacro.udi,
                        name: model.selectedMacro.name,
                        alias: model.selectedMacro.alias,
                        params: _.object(_.map(model.macroParams, function (p) { return [p.alias, p.value]; }))
                    });

                    if ((config.maxItems !== 0 && config.maxItems !== "0") && $scope.model.value.length >= config.maxItems) {
                        vm.allowAdd = false;
                    }

                    setDirty();

                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            }

            editorService.macroPicker(macroPicker);
        };

        function edit($index, item) {
            var macroPicker = {
                dialogData: { richTextEditor: false, macroData: { macroAlias: item.alias, macroParamsDictionary: item.params } },
                submit: function (model) {
                    $scope.model.value[$index] = {
                        udi: model.selectedMacro.udi,
                        name: model.selectedMacro.name,
                        alias: model.selectedMacro.alias,
                        params: _.object(_.map(model.macroParams, function (p) { return [p.alias, p.value]; }))
                    };

                    setDirty();

                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            }

            editorService.macroPicker(macroPicker);
        };

        function remove($index) {
            $scope.model.value.splice($index, 1);

            if ((config.maxItems === 0 || config.maxItems === "0") || $scope.model.value.length < config.maxItems) {
                vm.allowAdd = true;
            }

            setDirty();
        };

        function setDirty() {
            if ($scope.propertyForm) {
                $scope.propertyForm.$setDirty();
            }
        };
    }
]);
