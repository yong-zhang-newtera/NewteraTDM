'use strict';

angular.module('app.taskviewer').controller('TaskFormCtrl', function ($controller, $rootScope, $scope, $http, APP_CONFIG, $state, $stateParams, MetaDataCache) {
    angular.extend(this, $controller('ebaasFormBaseCtrl', { $rootScope: $rootScope, $scope: $scope, $http: $http, APP_CONFIG: APP_CONFIG }));
});