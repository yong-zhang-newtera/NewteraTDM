'use strict';

angular.module('app.taskviewer').controller('PacketFormCtrl', function ($stateParams, $rootScope, $scope, taskService) {
    // override the dbclass and oid with packet class and packet oid
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.packetClass;
    $scope.oid = $stateParams.packetOid;
    $scope.prefix = $stateParams.packetPrefix;

    $rootScope.hasPacketOid = taskService.hasValue($stateParams.packetOid);
});