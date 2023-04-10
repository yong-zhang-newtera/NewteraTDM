'use strict';

angular.module('app.taskviewer').controller('ItemFormCtrl', function ($controller, $rootScope, $scope, $http, APP_CONFIG, $state, $stateParams, taskService) {

    // override the dbclass and oid with test item class and test item oid
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.itemClass;
    $scope.oid = $stateParams.itemOid;
    $rootScope.hasItemOid = taskService.hasValue($stateParams.itemOid);

    angular.extend(this, $controller('ebaasFormBaseCtrl', { $rootScope: $rootScope, $scope: $scope, $http: $http, APP_CONFIG: APP_CONFIG }));
});