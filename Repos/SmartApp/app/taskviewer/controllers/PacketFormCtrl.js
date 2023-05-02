'use strict';

angular.module('app.taskviewer').controller('PacketFormCtrl', function ($state, $stateParams, $rootScope, $scope, taskService, $document) {
    // override the dbclass and oid with packet class and packet oid
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.packetClass;
    $scope.oid = $stateParams.packetOid;
    $scope.prefix = $stateParams.packetPrefix;

    $rootScope.hasPacketOid = taskService.hasValue($stateParams.packetOid);

    if ($stateParams.activeTabId == "packettab") {
        var itemTabElement = $document[0].getElementById('packettab');
        if (itemTabElement) {
            itemTabElement.click();
        }
    }

    $scope.$on('addChildNodeEvent', function (e, args) {
        if (args.childNodeType === "PacketNode") {
            $state.go('.relatedform', { schema: $scope.dbschema, masterclass: args.parentClass, masteroid: args.parentObjId, rclass: args.childClass, rtemplate: $scope.formTemplate }, { location: false, notify: false });
        }
    });

    $scope.$on('editParentNodeEvent', function (e, args) {
        if (args.childNodeType === "PacketNode") {
            $state.go('.relatedform', { schema: $scope.dbschema, rclass: args.parentClass, roid: args.parentObjId, rtemplate: $scope.formTemplate }, { location: false, notify: false });
        }
    });

    $scope.$on('deleteParentNodeEvent', function (e, args) {
        if (args.childNodeType === "PacketNode") {
            var result = confirm($rootScope.getWord("Confirm Delete Test Data Packet"));
            if (result) {

            }
        }
    });
});