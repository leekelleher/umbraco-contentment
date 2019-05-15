angular.module("umbraco").controller("Our.Umbraco.Contentment.DataEditors.EntityPicker.Controller", [
    "$scope",
    "entityResource",
    "editorService",
    "versionHelper",
    function ($scope, entityResource, editorService, versionHelper) {

        //console.log("model", $scope.model);

        var defaultConfig = {
            entityType: "DocumentType",
            maxItems: 0,
            allowDuplicates: 0,
            disableSorting: 0,
            supportedTypes: ["DocumentType"],
            semisupportedTypes: [],
            unsupportedTypes: []
        };
        var config = angular.merge({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            vm.loading = true;

            $scope.model.value = $scope.model.value || { entityType: config.entityType, items: [] };

            if (_.isArray($scope.model.value)) {
                $scope.model.value = { entityType: config.entityType, items: $scope.model.value };
            }

            if (config.entityType !== $scope.model.value.entityType) {
                vm.error = {
                    title: "Entity type misconfiguration",
                    message: "There is a misconfiguration between the expected entity type and existing values.<br>The data type has been configured with '<b>" + config.entityType + "</b>', but it contains a values of type '<b>" + $scope.model.value.entityType + "</b>'.<br>Here are the existing values...",
                    json: JSON.parse(JSON.stringify($scope.model.value)),
                    clear: "Click here if you wish to clear out the existing values."
                };
            }

            vm.items = [];
            vm.icon = "icon-science";
            vm.allowAdd = (config.maxItems === 0 || config.maxItems === "0") || $scope.model.value.items.length < config.maxItems;
            vm.allowEdit = true;
            vm.allowRemove = true;
            vm.published = true;
            vm.sortable = Object.toBoolean(config.disableSorting) === false && (config.maxItems !== 1 && config.maxItems !== "1");

            vm.sortableOptions = {
                axis: "y",
                containment: "parent",
                cursor: "move",
                disabled: !vm.sortable,
                opacity: 0.7,
                scroll: true,
                tolerance: "pointer",
                stop: function (e, ui) {
                    $scope.model.value.items = _.map(vm.items, function (x) { return x.udi });
                    setDirty();
                }
            };

            vm.add = add;
            vm.clear = clear;
            vm.edit = edit;
            vm.remove = remove;

            if (config.unsupportedTypes.length > 0 && _.contains(config.unsupportedTypes, config.entityType)) {
                vm.error = {
                    title: "Entity type not supported",
                    message: "Unfortunately the entity type '<b>" + config.entityType + "</b>' is not currently supported."
                };
            }

            if (!vm.error && $scope.model.value.items.length > 0) {

                if (config.supportedTypes.length > 0 && _.contains(config.supportedTypes, config.entityType)) {

                    entityResource.getByIds($scope.model.value.items, config.entityType).then(function (data) {
                        ensureIcons(data);
                        vm.items = data;
                    });

                } else if (config.semisupportedTypes.length > 0 && _.contains(config.semisupportedTypes, config.entityType)) {

                    // NOTE: Can't use `entityResource.getByIds` as many entityTypes aren't supported, e.g. Macros.
                    // https://github.com/umbraco/Umbraco-CMS/blob/release-8.0.2/src/Umbraco.Web/Editors/EntityController.cs#L735-L745
                    entityResource.getAll(config.entityType).then(function (data) {
                        ensureIcons(data);
                        _.each($scope.model.value.items, function (udi) {
                            var entity = _.find(data, function (item) { return item.udi === udi });
                            if (entity) {
                                vm.items.push(entity);
                            }
                        });
                    });

                } else {

                    vm.error = {
                        title: "Entity type unknown",
                        message: "Unfortunately the entity type '<b>" + config.entityType + "</b>' is unknown and not supported."
                    };

                }

            }

            if (config.entityType === "MemberType" && !vm.error && versionHelper.versionCompare(Umbraco.Sys.ServerVariables.application.version, "8.1", { zeroExtend: true }) < 0) {
                vm.error = {
                    title: "Bug with Member Type configuration",
                    message: "Unfortunately there is an existing bug in Umbraco that will return the <b>Media Types</b> instead of the Member Types.<br><a href='https://github.com/umbraco/Umbraco-CMS/pull/5432' target='_blank'>This bug has been reported and a patch submitted.</a>"
                };
            }

            vm.loading = false;
        };

        function add($event) {
            entityResource.getAll(config.entityType).then(function (data) {

                var availableItems = config.allowDuplicates === "1" || config.allowDuplicates === 1
                    ? data
                    : _.reject(data, function (x) { return _.find(vm.items, function (y) { return x.id === y.id; }); });

                ensureIcons(availableItems);

                var entityPicker = {
                    view: "itempicker",
                    title: "Choose " + config.entityType + "...",
                    availableItems: availableItems,
                    submit: function (model) {

                        vm.items.push(model.selectedItem)
                        $scope.model.value.items.push(model.selectedItem.udi);

                        if ((config.maxItems !== 0 && config.maxItems !== "0") && $scope.model.value.items.length >= config.maxItems) {
                            vm.allowAdd = false;
                        }

                        setDirty();

                        editorService.close();
                    },
                    close: function () {
                        editorService.close();
                    }
                };

                editorService.itemPicker(entityPicker);
            });
        };

        function clear($event) {
            vm.items = [];
            $scope.model.value = { entityType: config.entityType, items: [] };
            delete vm.error;
            setDirty();
        }

        function edit($index, item) {
            entityResource.getAll(config.entityType).then(function (data) {

                var availableItems = config.allowDuplicates === "1" || config.allowDuplicates === 1
                    ? data
                    : _.reject(data, function (x) { return _.find(vm.items, function (y) { return x.id === y.id; }); });

                ensureIcons(availableItems);

                var entityPicker = {
                    view: "itempicker",
                    title: "Edit " + config.entityType + "...",
                    availableItems: availableItems,
                    submit: function (model) {

                        vm.items[$index] = model.selectedItem;
                        $scope.model.value.items[$index] = model.selectedItem.udi;

                        setDirty();

                        editorService.close();
                    },
                    close: function () {
                        editorService.close();
                    }
                };

                editorService.itemPicker(entityPicker);
            });
        };

        function remove($index) {
            vm.items.splice($index, 1);
            $scope.model.value.items.splice($index, 1);

            if ((config.maxItems === 0 || config.maxItems === "0") || $scope.model.value.items.length < config.maxItems) {
                vm.allowAdd = true;
            }

            setDirty();
        };

        function ensureIcons(entities) {
            var missingIcons = {
                "DataType": "icon-autofill",
                "DocumentType": "icon-item-arrangement",
                "MediaType": "icon-thumbnails",
                "MemberType": "icon-users",
            };
            if (_.contains(_.keys(missingIcons), config.entityType)) {
                var icon = missingIcons[config.entityType];
                _.each(entities, function (x) {
                    x.icon = icon;
                });
            }
        }

        function setDirty() {
            if ($scope.propertyForm) {
                $scope.propertyForm.$setDirty();
            }
        };

        init();
    }
]);
