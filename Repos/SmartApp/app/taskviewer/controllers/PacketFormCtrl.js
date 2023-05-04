'use strict';

angular.module('app.taskviewer').controller('PacketFormCtrl', function ($controller, $state, $stateParams, $rootScope, $scope, $http, APP_CONFIG, taskService, $document) {
    // override the dbclass and oid with packet class and packet oid
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.packetClass;
    $scope.oid = $stateParams.packetOid;
    $scope.template = $stateParams.packetTemplate;
    $scope.prefix = $stateParams.packetPrefix;

    $rootScope.hasPacketOid = taskService.hasValue($stateParams.packetOid);

    if ($stateParams.activeTabId == "packettab") {
        var itemTabElement = $document[0].getElementById('packettab');
        if (itemTabElement) {
            itemTabElement.click();
        }
    }

    angular.extend(this, $controller('ebaasFormBaseCtrl', { $rootScope: $rootScope, $scope: $scope, $http: $http, APP_CONFIG: APP_CONFIG }));

    $scope.editForm = function () {
        $scope.$broadcast('editParentNodeEvent', {
            parentClass: $scope.dbclass,
            parentObjId: $scope.oid,
            childNodeType: "PacketNode"
        });
    };

    $scope.$on('addChildNodeEvent', function (e, args) {
        if (args.childNodeType === "PacketNode") {
            $state.go('.relatedform', { schema: $scope.dbschema, masterclass: args.parentClass, masteroid: args.parentObjId, rclass: args.childClass, rtemplate: $scope.template }, { location: false, notify: false });
        }
    });

    $scope.$on('editParentNodeEvent', function (e, args) {
        if (args.childNodeType === "PacketNode") {
            $state.go('.relatedform', { schema: $scope.dbschema, rclass: args.parentClass, roid: args.parentObjId, rtemplate: $scope.template }, { location: false, notify: false });
        }
    });
});