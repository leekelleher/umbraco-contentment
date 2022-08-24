/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

(function () {
    "use strict";

    function contentmentItemsEditorFactory(displayMode) {
        return {
            bindings: {
                addButtonLabel: "@?",
                addButtonLabelKey: "<?",
                allowAdd: "<?",
                allowEdit: "<?",
                allowRemove: "<?",
                allowSort: "<?",
                blockActions: "<?",
                defaultIcon: "<?",
                displayMode: "<?",
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
                previews: "<?",
            },
            controllerAs: "vm",
            controller: [
                "$scope",
                "localizationService",
                function ($scope, localizationService) {

                    var vm = this;

                    vm.$onInit = function () {

                        var _displayMode = displayMode || vm.displayMode || "list";

                        //console.log("contentmentItemsEditorFactory", _displayMode, vm);

                        vm.templateUrl = "/App_Plugins/Contentment/components/" + _displayMode + "-editor.html";

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

                        // NOTE: Sortable options for specific modes.
                        if (_displayMode === "blocks") {
                            Object.assign(vm.sortableOptions, {
                                cancel: "input,textarea,select,option",
                                classes: ".blockelement--dragging",
                                cursor: "grabbing",
                                distance: 5,
                                handle: ".blockelement__draggable-element"
                            });
                        } else if (_displayMode === "cards") {
                            Object.assign(vm.sortableOptions, {
                                axis: false,
                                "ui-floating": true,
                                items: ".umb-block-card",
                                cursor: "grabbing",
                                placeholder: "umb-block-card --sortable-placeholder",
                            });
                        }

                        vm.add = add;
                        vm.canEdit = canEdit;
                        vm.canRemove = canRemove;
                        vm.edit = edit;
                        vm.populate = populate;
                        vm.populateStyle = populateStyle;
                        vm.remove = remove;

                        vm.isSingle = vm.allowAdd === false && vm.ngModel.length === 1 ? "" : undefined;

                        if (vm.addButtonLabelKey) {
                            localizationService.localize(vm.addButtonLabelKey).then(function (label) {
                                vm.addButtonLabel = label;
                            });
                        }

                        if (vm.propertyActions && vm.propertyActions.length > 0) {
                            vm.umbProperty.setPropertyActions(vm.propertyActions);
                        }

                        if (vm.blockActions && vm.blockActions.length > 0) {
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
                        if (typeof vm.onAdd === "function") {
                            vm.onAdd();
                        }
                    };

                    function canEdit(item, $index) {
                        switch (typeof vm.allowEdit) {
                            case "boolean":
                                return vm.allowEdit;
                            case "function":
                                return vm.allowEdit(item, $index);
                            default:
                                return true;
                        }
                    };

                    function canRemove(item, $index) {
                        switch (typeof vm.allowRemove) {
                            case "boolean":
                                return vm.allowRemove;
                            case "function":
                                return vm.allowRemove(item, $index);
                            default:
                                return true;
                        }
                    };

                    function edit($index) {
                        if (typeof vm.onEdit === "function") {
                            vm.onEdit($index);
                        }
                    };

                    function populate(item, $index, propertyName) {
                        if (typeof vm.getItem === "function") {
                            return vm.getItem(item, $index, propertyName);
                        }

                        switch (propertyName) {
                            case "icon":
                                return typeof vm.getItemIcon === "function"
                                    ? vm.getItemIcon(item, $index)
                                    : item.icon || vm.defaultIcon;

                            case "name":
                                return typeof vm.getItemName === "function"
                                    ? vm.getItemName(item, $index)
                                    : item.name;

                            case "description":
                                return typeof vm.getItemDescription === "function"
                                    ? vm.getItemDescription(item, $index)
                                    : item.description;

                            default:
                                return item.hasOwnProperty(propertyName) === true
                                    ? item[propertyName]
                                    : undefined;
                        }
                    };

                    function populateStyle(item, $index, propertyName, styleProperty) {
                        var style = {};
                        style[styleProperty] = populate(item, $index, propertyName);
                        return style;
                    };

                    function remove($index) {
                        if (typeof vm.onRemove === "function") {
                            vm.onRemove($index);
                        }
                    };
                }],
            require: {
                propertyForm: "^form",
                umbProperty: "^"
            },
            template: "<ng-include src='vm.templateUrl'></ng-include>"
        };
    };

    angular.module("umbraco.directives").component("contentmentItemsEditor", contentmentItemsEditorFactory());

    // Obsolete, use `<contentment-items-editor display-mode="vm.displayMode"></contentment-items-editor>` instead.
    angular.module("umbraco.directives").component("contentmentListEditor", contentmentItemsEditorFactory("list"));
    angular.module("umbraco.directives").component("contentmentStackEditor", contentmentItemsEditorFactory("stack"));
    angular.module("umbraco.directives").component("contentmentBlocksEditor", contentmentItemsEditorFactory("blocks"));
    angular.module("umbraco.directives").component("contentmentCardsEditor", contentmentItemsEditorFactory("cards"));

})();
