'use strict';

angular.module('app.taskviewer').controller('ItemFormCtrl', function ($controller, $rootScope, $scope, $http, APP_CONFIG, $state, $stateParams, taskService, $document, $window) {

    // override the dbclass and oid with test item class and test item oid
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.itemClass;
    $scope.oid = $stateParams.itemOid;
    $scope.template = $stateParams.itemTemplate;
    $rootScope.hasItemOid = taskService.hasValue($stateParams.itemOid);

    if ($stateParams.activeTabId == "itemtab") {
        var itemTabElement = $document[0].getElementById('itemtab');
        if (itemTabElement) {
            itemTabElement.click();
        }
    }

    angular.extend(this, $controller('ebaasFormBaseCtrl', { $rootScope: $rootScope, $scope: $scope, $http: $http, APP_CONFIG: APP_CONFIG }));

    $scope.editForm = function () {
        $scope.$broadcast('editParentNodeEvent', {
            parentClass: $scope.dbclass,
            parentObjId: $scope.oid,
            childNodeType: "ItemNode"
        });
    };

    $rootScope.$on('relatedModalFormClosed', function (event, args) {
        $state.reload();
    });

    $scope.$on('addChildNodeEvent', function (e, args) {
        if (args.childNodeType === "ItemNode") {
            $state.go('.relatedform', { schema: $scope.dbschema, masterclass: args.parentClass, masteroid: args.parentObjId, rclass: args.childClass, rtemplate: $scope.template }, { location: false, notify: false });
        }
    });

    $scope.$on('editParentNodeEvent', function (e, args) {
        if (args.childNodeType === "ItemNode") {
            $state.go('.relatedform', { schema: $scope.dbschema, rclass: args.parentClass, roid: args.parentObjId, rtemplate: $scope.template }, { location: false, notify: false });
        }
    });
});