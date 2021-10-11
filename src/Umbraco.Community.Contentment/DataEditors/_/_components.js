/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco.directives").component("contentmentListEditor", {
    templateUrl: "/App_Plugins/Contentment/components/list-editor.html",
    bindings: {
        addButtonLabel: "@?",
        addButtonLabelKey: "<?",
        allowAdd: "<?",
        allowEdit: "<?",
        allowRemove: "<?",
        allowSort: "<?",
        defaultIcon: "<?",
        getItem: "<?",
        getItemIcon: "<?",
        getItemName: "<?",
        getItemDescription: "<?",
        ngModel: "=",
        onAdd: "<?",
        onEdit: "<?",
        onRemove: "<?",
        onSort: "<?",
        propertyActions: "<?",
    },
    require: {
        propertyForm: "^form",
        umbProperty: "^"
    },
    controllerAs: "vm",
    controller: [
        "$scope",
        "localizationService",
        function ($scope, localizationService) {

            // console.log("contentmentListEditor", $scope.vm);

            var vm = this;

            vm.$onInit = function () {

                vm.propertyAlias = vm.umbProperty.property.alias;

                vm.sortableOptions = {
                    axis: "y",
                    containment: "parent",
                    cursor: "move",
                    disabled: vm.allowSort === false,
                    opacity: 0.7,
                    scroll: true,
                    tolerance: "pointer",
                    stop: function (e, ui) {

                        if (vm.onSort) {
                            vm.onSort();
                        }

                        if (vm.propertyForm) {
                            vm.propertyForm.$setDirty();
                        }
                    }
                };

                vm.add = add;
                vm.canEdit = canEdit;
                vm.canRemove = canRemove;
                vm.edit = edit;
                vm.populate = populate;
                vm.remove = remove;

                if (vm.addButtonLabelKey) {
                    localizationService.localize(vm.addButtonLabelKey).then(function (label) {
                        vm.addButtonLabel = label;
                    });
                }

                if (vm.propertyActions && vm.propertyActions.length > 0) {
                    vm.umbProperty.setPropertyActions(vm.propertyActions);
                }
            };

            function add() {
                if (typeof (vm.onAdd) === "function") {
                    vm.onAdd();
                }
            };

            function canEdit(item, $index) {
                switch (typeof (vm.allowEdit)) {
                    case "boolean":
                        return vm.allowEdit;
                    case "function":
                        return vm.allowEdit(item, $index);
                    default:
                        return true;
                }
            };

            function canRemove(item, $index) {
                switch (typeof (vm.allowRemove)) {
                    case "boolean":
                        return vm.allowRemove;
                    case "function":
                        return vm.allowRemove(item, $index);
                    default:
                        return true;
                }
            };

            function edit($index) {
                if (typeof (vm.onEdit) === "function") {
                    vm.onEdit($index);
                }
            };

            function populate(item, $index, propertyName) {
                if (typeof (vm.getItem) === "function") {
                    return vm.getItem(item, $index, propertyName);
                }

                switch (propertyName) {
                    case "icon":
                        return typeof (vm.getItemIcon) === "function"
                            ? vm.getItemIcon(item, $index)
                            : item.icon || vm.defaultIcon;

                    case "name":
                        return typeof (vm.getItemName) === "function"
                            ? vm.getItemName(item, $index)
                            : item.name;

                    case "description":
                        return typeof (vm.getItemDescription) === "function"
                            ? vm.getItemDescription(item, $index)
                            : item.description;

                    default:
                        return item[propertyName];
                }
            };

            function remove($index) {
                if (typeof (vm.onRemove) === "function") {
                    vm.onRemove($index);
                }
            };
        }]
});

angular.module("umbraco.directives").component("contentmentStackEditor", {
    templateUrl: "/App_Plugins/Contentment/components/stack-editor.html",
    bindings: {
        addButtonLabel: "@?",
        addButtonLabelKey: "<?",
        allowAdd: "<?",
        allowEdit: "<?",
        allowRemove: "<?",
        allowSort: "<?",
        blockActions: "<?",
        defaultIcon: "<?",
        getItemIcon: "<?",
        getItemName: "<?",
        ngModel: "=?",
        onAdd: "<?",
        onEdit: "<?",
        onRemove: "<?",
        onSort: "<?",
        propertyActions: "<?",
        previews: "<?",
    },
    require: {
        propertyForm: "^form",
        umbProperty: "^"
    },
    controllerAs: "vm",
    controller: [
        "$scope",
        "localizationService",
        function ($scope, localizationService) {

            // console.log("contentmentStackEditor", $scope.vm);

            var vm = this;

            vm.$onInit = function () {

                vm.propertyAlias = vm.umbProperty.property.alias;

                vm.sortableOptions = {
                    axis: "y",
                    containment: "parent",
                    cursor: "move",
                    disabled: vm.allowSort === false,
                    opacity: 0.7,
                    scroll: true,
                    tolerance: "pointer",
                    stop: function (e, ui) {

                        if (vm.onSort) {
                            vm.onSort();
                        }

                        if (vm.propertyForm) {
                            vm.propertyForm.$setDirty();
                        }
                    }
                };

                vm.add = add;
                vm.canEdit = canEdit;
                vm.canRemove = canRemove;
                vm.edit = edit;
                vm.populate = populate;
                vm.remove = remove;

                if (vm.addButtonLabelKey) {
                    localizationService.localize(vm.addButtonLabelKey).then(function (label) {
                        vm.addButtonLabel = label;
                    });
                }

                if (vm.propertyActions && vm.propertyActions.length > 0) {
                    vm.umbProperty.setPropertyActions(vm.propertyActions);
                }
            };

            function add() {
                if (typeof (vm.onAdd) === "function") {
                    vm.onAdd();
                }
            };

            function canEdit(item, $index) {
                switch (typeof (vm.allowEdit)) {
                    case "boolean":
                        return vm.allowEdit;
                    case "function":
                        return vm.allowEdit(item, $index);
                    default:
                        return true;
                }
            };

            function canRemove(item, $index) {
                switch (typeof (vm.allowRemove)) {
                    case "boolean":
                        return vm.allowRemove;
                    case "function":
                        return vm.allowRemove(item, $index);
                    default:
                        return true;
                }
            };

            function edit($index) {
                if (typeof (vm.onEdit) === "function") {
                    vm.onEdit($index);
                }
            };

            function populate(item, $index, propertyName) {
                if (typeof (vm.getItem) === "function") {
                    return vm.getItem(item, $index, propertyName);
                }

                switch (propertyName) {
                    case "icon":
                        return typeof (vm.getItemIcon) === "function"
                            ? vm.getItemIcon(item, $index)
                            : item.icon || vm.defaultIcon;

                    case "name":
                        return typeof (vm.getItemName) === "function"
                            ? vm.getItemName(item, $index)
                            : item.name;

                    case "description":
                        return typeof (vm.getItemDescription) === "function"
                            ? vm.getItemDescription(item, $index)
                            : item.description;

                    default:
                        return item[propertyName];
                }
            };

            function remove($index) {
                if (typeof (vm.onRemove) === "function") {
                    vm.onRemove($index);
                }
            };

        }]
});

angular.module("umbraco.directives").component("contentmentBlocksEditor", {
    templateUrl: "/App_Plugins/Contentment/components/blocks-editor.html",
    bindings: {
        addButtonLabel: "@?",
        addButtonLabelKey: "<?",
        allowAdd: "<?",
        allowEdit: "<?",
        allowRemove: "<?",
        allowSort: "<?",
        blockActions: "<?",
        defaultIcon: "<?",
        getItemIcon: "<?",
        getItemName: "<?",
        ngModel: "=?",
        onAdd: "<?",
        onEdit: "<?",
        onRemove: "<?",
        onSort: "<?",
        propertyActions: "<?",
        previews: "<?",
    },
    require: {
        propertyForm: "^form",
        umbProperty: "^"
    },
    controllerAs: "vm",
    controller: [
        "$scope",
        "localizationService",
        function ($scope, localizationService) {

            // console.log("contentmentBlocksEditor", $scope.vm);

            var vm = this;

            vm.$onInit = function () {

                vm.propertyAlias = vm.umbProperty.property.alias;

                vm.sortableOptions = {
                    axis: "y",
                    cancel: "input,textarea,select,option",
                    classes: ".blockelement--dragging",
                    containment: "parent",
                    cursor: "grabbing",
                    disabled: vm.allowSort === false,
                    distance: 5,
                    handle: ".blockelement__draggable-element",
                    opacity: 0.7,
                    scroll: true,
                    tolerance: "pointer",
                    stop: function (e, ui) {

                        if (vm.onSort) {
                            vm.onSort();
                        }

                        if (vm.propertyForm) {
                            vm.propertyForm.$setDirty();
                        }
                    }
                };

                vm.add = add;
                vm.canEdit = canEdit;
                vm.canRemove = canRemove;
                vm.edit = edit;
                vm.populate = populate;
                vm.remove = remove;

                if (vm.addButtonLabelKey) {
                    localizationService.localize(vm.addButtonLabelKey).then(function (label) {
                        vm.addButtonLabel = label;
                    });
                }

                if (vm.propertyActions && vm.propertyActions.length > 0) {
                    vm.umbProperty.setPropertyActions(vm.propertyActions);
                }

                if (vm.blockActions && vm.blockActions.length > 0) {
                    // TODO: [LK:2021-06-16] Can be optimised. Duplicate localize lookups happening here.
                    vm.blockActions.forEach(function (x) {
                        x.forEach(function (y) {
                            localizationService.localize(y.labelKey).then(function (label) {
                                y.label = label;
                            });
                        });
                    });
                }
            };

            function add() {
                if (typeof (vm.onAdd) === "function") {
                    vm.onAdd();
                }
            };

            function canEdit(item, $index) {
                switch (typeof (vm.allowEdit)) {
                    case "boolean":
                        return vm.allowEdit;
                    case "function":
                        return vm.allowEdit(item, $index);
                    default:
                        return true;
                }
            };

            function canRemove(item, $index) {
                switch (typeof (vm.allowRemove)) {
                    case "boolean":
                        return vm.allowRemove;
                    case "function":
                        return vm.allowRemove(item, $index);
                    default:
                        return true;
                }
            };

            function edit($index) {
                if (typeof (vm.onEdit) === "function") {
                    vm.onEdit($index);
                }
            };

            function populate(item, $index, propertyName) {
                if (typeof (vm.getItem) === "function") {
                    return vm.getItem(item, $index, propertyName);
                }

                switch (propertyName) {
                    case "icon":
                        return typeof (vm.getItemIcon) === "function"
                            ? vm.getItemIcon(item, $index)
                            : item.icon || vm.defaultIcon;

                    case "name":
                        return typeof (vm.getItemName) === "function"
                            ? vm.getItemName(item, $index)
                            : item.name;

                    case "description":
                        return typeof (vm.getItemDescription) === "function"
                            ? vm.getItemDescription(item, $index)
                            : item.description;

                    default:
                        return item[propertyName];
                }
            };

            function remove($index) {
                if (typeof (vm.onRemove) === "function") {
                    vm.onRemove($index);
                }
            };

        }]
});
