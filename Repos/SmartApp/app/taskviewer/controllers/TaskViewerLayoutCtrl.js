'use strict';

angular.module('app.taskviewer').controller('TaskViewerLayoutCtrl', function ($http, APP_CONFIG, $scope, $state, $stateParams, MetaDataCache, taskService) {

    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.class;
    $scope.oid = $stateParams.oid;
    $scope.formAttribute = undefined;
    $scope.itemclass = $stateParams.itemClass;
    $scope.packetclass = $stateParams.packetClass;

    $scope.node = undefined;

    taskService.getTaskTree($stateParams.schema, $stateParams.class, $stateParams.oid, $stateParams.itemClass, $stateParams.packetClass, function (treeData) {
        $scope.taskDataTree = treeData;

        $state.go('app.taskviewer.taskinfoview', { schema: $scope.dbschema, class: $scope.dbclass, oid: $scope.oid, node: $scope.node, itemClass: $scope.itemclass, packetClass: $scope.packetclass});
    });

    $scope.GoToTaskInfoView = function GoToTaskInfoView(nodeName) {

        $state.go('app.taskviewer.taskinfoview', { schema: $scope.dbschema, class: $scope.dbclass, oid: $scope.oid, node: nodeName, itemClass: $scope.itemclass, packetClass: $scope.packetclass });
    }
});