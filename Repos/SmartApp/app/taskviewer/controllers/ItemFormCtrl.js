'use strict';

angular.module('app.taskviewer').controller('ItemFormCtrl', function ($controller, $rootScope, $scope, $http, APP_CONFIG, $state, $stateParams, taskService, $document, $window) {

    // override the dbclass and oid with test item class and test item oid
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.itemClass;
    $scope.oid = $stateParams.itemOid;
    $rootScope.hasItemOid = taskService.hasValue($stateParams.itemOid);

    if ($stateParams.activeTabId == "itemtab") {
        var itemTabElement = $document[0].getElementById('itemtab');
        if (itemTabElement) {
            itemTabElement.click();
        }
    }

    angular.extend(this, $controller('ebaasFormBaseCtrl', { $rootScope: $rootScope, $scope: $scope, $http: $http, APP_CONFIG: APP_CONFIG }));

    $scope.openModal = function () {
        $state.go('.relatedform', { schema: $scope.dbschema, rclass: $scope.dbclass, roid: $scope.oid, rtemplate: $scope.formTemplate }, { location: false, notify: false });
    };

    $rootScope.$on('relatedModalFormClosed', function (event, args) {
        $state.reload();
    });
});