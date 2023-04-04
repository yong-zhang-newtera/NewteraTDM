'use strict';

angular.module('app.taskviewer').controller('TaskInfoViewCtrl', function ($controller, $rootScope, $scope, $http, APP_CONFIG, $state, $stateParams, MetaDataCache) {

    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.class;
    $scope.oid = $stateParams.oid;
    $scope.itemclass = $stateParams.itemClass;
    $scope.packetclass = $stateParams.packetClass;

    if ($stateParams.insert && $stateParams.insert === "false") {
        $scope.add = false;
    }
    else {
        $scope.add = true;
    }

    angular.extend(this, $controller('ebaasFormBaseCtrl', { $rootScope: $rootScope, $scope: $scope, $http: $http, APP_CONFIG: APP_CONFIG }));
});