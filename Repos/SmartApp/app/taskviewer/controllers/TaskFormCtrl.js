'use strict';

angular.module('app.taskviewer').controller('TaskFormCtrl', function ($stateParams, $state, $controller, $rootScope, $scope, $http, APP_CONFIG, $document) {
    if (!$stateParams.activeTabId || $stateParams.activeTabId == "tasktab") {
        var itemTabElement = $document[0].getElementById('tasktab');
        if (itemTabElement) {
            itemTabElement.click();
        }
    }

    angular.extend(this, $controller('ebaasFormBaseCtrl', { $rootScope: $rootScope, $scope: $scope, $http: $http, APP_CONFIG: APP_CONFIG }));

    $scope.openModal = function () {
        $state.go('.modalform', { schema: $scope.dbschema, class: $scope.dbclass, oid: $scope.oid, template: $scope.formTemplate }, { location: true, notify: false, reload: true });
    };
});