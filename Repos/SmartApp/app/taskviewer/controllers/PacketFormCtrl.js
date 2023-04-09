'use strict';

angular.module('app.taskviewer').controller('PacketFormCtrl', function ($stateParams, $scope) {
    // override the dbclass and oid with packet class and packet oid
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.packetClass;
    $scope.oid = $stateParams.packetOid;
});