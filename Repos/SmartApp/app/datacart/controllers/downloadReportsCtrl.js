"use strict";

angular.module('app.datacart').controller('downloadReportsCtrl', function ($scope, $rootScope, $http, $stateParams, $modalInstance, APP_CONFIG, dataCartService, fileManager, MetaDataCache) {

    $scope.loading = false;
    $scope.schema = $stateParams.schema;
    $scope.dbclass = $stateParams.class;

    var key = $scope.schema + $scope.dbclass + "TotalCount";
    $scope.totalCount = MetaDataCache.getNamedData(key);

    $scope.selectedTemplate = "0";

    dataCartService.getReportTemplates($stateParams.schema, $stateParams.class, function (data) {
        $scope.templates = data;
    });

    $scope.generate = function ()
    {
        $scope.loading = true;
        var MaxSize = 10000
 
        if ($scope.totalCount > MaxSize) {
            BootstrapDialog.show({
                title: $rootScope.getWord("Info Dialog"),
                type: BootstrapDialog.TYPE_WARNING,
                message: $rootScope.getWord("Too Many Rows"),
                buttons: [{
                    label: $rootScope.getWord("Cancel"),
                    action: function (dialog) {
                        dialog.close();
                    }
                }]
            });
        }
        else {
            var key = $scope.schema + $scope.dbclass + "Filter";
            var filter = MetaDataCache.getNamedData(key);

            key = $scope.schema + $scope.dbclass + "View";

            var view = MetaDataCache.getNamedData(key);

            var getFileUrl = APP_CONFIG.apiRootUrl + "/report/" + encodeURIComponent($scope.schema) + "/" + $scope.dbclass + "?template=" + encodeURIComponent($scope.selectedTemplate);

            if (view)
            {
                getFileUrl += "&view=" + view;
            }
            if (filter)
            {
                getFileUrl += "&" + filter;
            }

            fileManager.performDownload(getFileUrl, function () {
                $scope.loading = false;
            });
        }
    }

    $scope.closeModal = function () {
        $modalInstance.dismiss("dismiss");
    };
});