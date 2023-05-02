'use strict';

angular.module('app.taskviewer').controller('TaskFormCtrl', function ($stateParams, $state, $controller, $rootScope, $scope, $http, APP_CONFIG, $document) {
    if (!$stateParams.activeTabId || $stateParams.activeTabId == "tasktab") {
        var itemTabElement = $document[0].getElementById('tasktab');
        if (itemTabElement) {
            itemTabElement.click();
        }
    }

    angular.extend(this, $controller('ebaasFormBaseCtrl', { $rootScope: $rootScope, $scope: $scope, $http: $http, APP_CONFIG: APP_CONFIG }));

    $scope.editForm = function () {
        $scope.$broadcast('editParentNodeEvent', {
            parentClass: $scope.dbclass,
            parentObjId: $scope.oid,
            childNodeType: "TaskNode"
        });
    };

    $scope.$on('editParentNodeEvent', function (e, args) {
        if (args.childNodeType === "TaskNode") {
            $state.go('.modalform', { schema: $scope.dbschema, class: args.parentClass, oid: args.parentObjId }, { location: false, notify: false });
        }
    });

    $scope.$on('deleteParentNodeEvent', function (e, args) {
        if (args.childNodeType === "TaskNode") {

        }
    });
});