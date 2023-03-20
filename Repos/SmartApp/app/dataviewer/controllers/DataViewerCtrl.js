'use strict';

angular.module('app.dataviewer').controller('DataViewerCtrl', function ($controller, $scope, $rootScope, $http, $state, $stateParams, APP_CONFIG, DataViewerService, FlotConfig) {

    $scope.backTitle = $rootScope.getWord("Back");
    $scope.title = $rootScope.getWord("Test Data");

    $scope.goBackToPrev = function () {
        history.back(1);
    }

    $scope.selectData = function () {
        $state.go(".selectdata", { "schema": $scope.dbschema, "class": $scope.dbclass, "oid": $scope.oid, "xmlschema": $scope.xmlschema, "category": $scope.currentCategory, "api": $scope.customApi});
    }

    angular.extend(this, $controller('DataViewerBaseCtrl', { $scope: $scope, $rootScope: $rootScope, $http: $http, $stateParams: $stateParams, APP_CONFIG: APP_CONFIG, DataViewerService: DataViewerService, FlotConfig: FlotConfig }));
  
});
