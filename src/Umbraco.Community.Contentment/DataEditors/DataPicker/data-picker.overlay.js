angular.module("umbraco").controller("Umbraco.Community.Contentment.Overlays.DataPicker.Controller", [
    "$scope",
    "$http",
    "umbRequestHelper",
    function ($scope, $http, umbRequestHelper) {

        //console.log("data-picker.overlay", $scope.model);

        var defaultConfig = {
            dataTypeKey: null,
            enableMultiple: false,
            listType: "cards",
            pageSize: 12,
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            vm.title = "Select items...";
            vm.enableMultiple = config.enableMultiple;
            vm.listType = config.listType;

            vm.loading = true;
            vm.items = [];

            vm.allowSubmit = false;
            vm.selection = {};

            vm.searchOptions = {
                dataTypeKey: config.dataTypeKey,
                pageNumber: 1,
                pageSize: config.pageSize,
                query: "",
                totalPages: 0,
            };

            vm.close = close;
            vm.getImage = getImage;
            vm.isSelected = isSelected;
            vm.pagination = pagination;
            vm.search = search;
            vm.select = select;
            vm.submit = submit;

            _query();
        };

        const _debounce = _.debounce(() => $scope.$apply(_query), 500);

        function _query() {

            vm.loading = true;

            umbRequestHelper.resourcePromise(
                $http.get("backoffice/Contentment/DataPickerApi/Search", {
                    params: {
                        dataTypeKey: vm.searchOptions.dataTypeKey,
                        pageNumber: vm.searchOptions.pageNumber,
                        pageSize: vm.searchOptions.pageSize,
                        query: encodeURIComponent(vm.searchOptions.query)
                    }
                }),
                "Failed to retrieve search data.")
                .then(function (data) {
                    vm.loading = false;
                    vm.items = data.items;
                    vm.searchOptions.totalPages = data.totalPages;
                });
        };

        function close() {
            if ($scope.model.close) {
                $scope.model.close();
            }
        };

        function getImage(item) {
            return item && item.image
                ? { "background-image": `url('${item.image}')` }
                : null;
        };

        function isSelected(item) {
            return vm.selection.hasOwnProperty(item.value) === true;
        };

        function pagination(pageNumber) {
            vm.loading = true;
            vm.searchOptions.pageNumber = pageNumber;
            _query();
        };

        function search() {
            vm.loading = true;
            vm.searchOptions.pageNumber = 1;
            vm.searchOptions.totalPages = 0;
            _debounce();
        }

        function select(item, $event) {
            $event.stopPropagation();

            if (vm.enableMultiple === true) {

                if (vm.selection.hasOwnProperty(item.value) === false) {
                    vm.selection[item.value] = item;
                } else {
                    delete vm.selection[item.value];
                }

                vm.allowSubmit = true;

            } else {

                vm.selection = {};
                vm.selection[item.value] = item;
                submit();

            }
        };

        function submit() {
            if ($scope.model.submit) {
                $scope.model.submit(vm.selection);
            }
        };

        init();
    }
]);
