'use strict';

angular.module('app.taskviewer').controller('TaskViewerLayoutCtrl', function ($scope, $state, $stateParams, taskService) {

    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.class;
    $scope.oid = $stateParams.oid;
    $scope.formAttribute = undefined;
    $scope.itemClass = $stateParams.itemClass;
    $scope.itemOid = $stateParams.itemOid;
    $scope.packetClass = $stateParams.packetClass;
    $scope.packetOid = $stateParams.packetOid;
    $scope.taskNodeAttribute = $stateParams.taskNodeAttribute;
    $scope.itemNodeAttribute = $stateParams.itemNodeAttribute;
    $scope.packetNodeAttribute = $stateParams.packetNodeAttribute;
    $scope.hasItemOid = taskService.hasValue($scope.itemOid);
    $scope.hasPacketOid = taskService.hasValue($scope.packetOid);

    var parameters = {};
    parameters.schema = $stateParams.schema;
    parameters.class = $stateParams.class;
    parameters.oid = $stateParams.oid;
    parameters.taskClass = $stateParams.class;
    parameters.taskOid = $stateParams.oid;
    parameters.taskNodeAttribute = $stateParams.taskNodeAttribute;
    parameters.itemClass = $stateParams.itemClass;
    parameters.itemNodeAttribute = $stateParams.itemNodeAttribute;
    parameters.packetClass = $stateParams.packetClass;
    parameters.packetNodeAttribute = $stateParams.packetNodeAttribute;

    taskService.getTaskTree(parameters, function (treeData) {
        $scope.taskDataTree = treeData;
    });

    $state.go("app.taskviewer.details", parameters);

    $scope.GoToTaskInfoView = function GoToTaskInfoView(nodeClass, nodeOid) {
        var params = new Object();
        params.schema = $scope.dbschema;
        params.class = $scope.dbclass;
        params.oid = $scope.oid;
        params.itemClass = $scope.itemClass;
        params.packetClass = $scope.packetClass;
        params.taskNodeAttribute = $scope.taskNodeAttribute;
        params.itemNodeAttribute = $scope.itemNodeAttribute;
        params.packetNodeAttribute = $scope.packetNodeAttribute;

        if (nodeClass == $scope.itemClass) {
            params.itemOid = nodeOid;
            $scope.itemOid = nodeOid;
            $state.go("app.taskviewer.details", params, { reload: true });
        }
        else if (nodeClass == $scope.packetClass) {
            params.packetOid = nodeOid;
            $scope.packetOid = nodeOid;
            $state.go("app.taskviewer.details", params, { reload: true });
        }
        else {
            params.itemOid = "";
            params.packetOid = "";
            $scope.itemOid = "";
            $scope.packetOid = "";
            $state.go("app.taskviewer.details", params, { reload: true });
        }
    }
});