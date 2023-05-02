'use strict';

angular.module('app.taskviewer').controller('TaskViewerLayoutCtrl', function ($rootScope, $scope, $state, $stateParams, taskService, MetaDataCache) {

    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.class;
    $scope.oid = $stateParams.oid;
    $scope.formAttribute = undefined;
    $scope.itemClass = $stateParams.itemClass;
    $scope.itemOid = $stateParams.itemOid;
    $scope.packetClass = $stateParams.packetClass;
    $scope.packetOid = $stateParams.packetOid;
    $scope.packetPrefix = $stateParams.packetPrefix;
    $scope.taskNodeAttribute = $stateParams.taskNodeAttribute;
    $scope.itemNodeAttribute = $stateParams.itemNodeAttribute;
    $scope.packetNodeAttribute = $stateParams.packetNodeAttribute;
    $scope.packetPrefixAttribute = $stateParams.packetPrefixAttribute;

    $rootScope.IsReloaded = false;

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
    parameters.packetPrefixAttribute = $stateParams.packetPrefixAttribute;

    var treeName = $stateParams.schema + $stateParams.class + $stateParams.oid;
    if (MetaDataCache.getNamedData(treeName)) {
        $scope.taskDataTree = MetaDataCache.getNamedData(treeName);
        $scope.nodes = MetaDataCache.getNamedData(treeName + "-nodes");
    }
    else {
        taskService.getTaskTree(parameters, function (taskTree, flatternNodes) {
            $scope.taskDataTree = taskTree;
            $scope.nodes = flatternNodes;
            MetaDataCache.setNamedData(treeName, taskTree);
            MetaDataCache.setNamedData(treeName + "-nodes", $scope.nodes);
        });
    }

    $state.go("app.taskviewer.details", parameters);

    $scope.GoToTaskInfoView = function GoToTaskInfoView(nodeClass, nodeOid, nodePrefix) {
        var params = new Object();
        params.schema = $scope.dbschema;
        params.class = $scope.dbclass;
        params.oid = $scope.oid;
        params.itemClass = $scope.itemClass;
        params.packetClass = $scope.packetClass;
        params.taskNodeAttribute = $scope.taskNodeAttribute;
        params.itemNodeAttribute = $scope.itemNodeAttribute;
        params.packetNodeAttribute = $scope.packetNodeAttribute;
        params.packetPrefixAttribute = $scope.packetPrefixAttribute;

        if (nodeClass == $scope.itemClass) {
            params.itemOid = nodeOid;
            params.packetOid = null;
            params.activeTabId = "itemtab";
            $scope.itemOid = nodeOid;
            $scope.packetOid = null;
            $state.go("app.taskviewer.details", params, { reload: true });
        }
        else if (nodeClass == $scope.packetClass) {
            params.packetOid = nodeOid;
            $scope.packetOid = nodeOid;
            $scope.packetPrefix = nodePrefix;
            params.packetPrefix = nodePrefix;
            params.activeTabId = "packettab";
            $state.go("app.taskviewer.details", params, { reload: true });
        }
        else {
            params.itemOid = "";
            params.packetOid = "";
            $scope.itemOid = "";
            $scope.packetOid = "";
            params.activeTabId = "tasktab";
            $state.go("app.taskviewer.details", params, { reload: true });
        }
    }

    $scope.Refresh = function (reloadTree) {
        if ($rootScope.RefreshTaskTree) {
            var treeName = $stateParams.schema + $stateParams.class + $stateParams.oid;
            MetaDataCache.setNamedData(treeName, null);
            MetaDataCache.setNamedData(treeName + "-nodes", null);
            $rootScope.RefreshTaskTree = false;
        }

        // Hack as workaround to the issue with $state.reload() not working
        $scope.$$postDigest(function () {
            angular.element('#reloadTaskViewer').trigger('click');
        });
    };

    $scope.addNode = function (parentClass, parentObjId, childClass, childNodeType) {
        $scope.$broadcast('addChildNodeEvent', {
            parentClass: parentClass,
            parentObjId: parentObjId,
            childClass: childClass,
            childNodeType: childNodeType
        });

        $rootScope.RefreshTaskTree = true;
    };

    $scope.editNode = function (parentClass, parentObjId, childClass, childObjId, childNodeType) {
        $scope.$broadcast('editParentNodeEvent', {
            parentClass: parentClass,
            parentObjId: parentObjId,
            childClass: childClass,
            childObjId: childObjId,
            childNodeType: childNodeType
        });
    };

    $scope.deleteNode = function (parentClass, parentObjId, childClass, childObjId, childNodeType) {
        $scope.$broadcast('deleteParentNodeEvent', {
            parentClass: parentClass,
            parentObjId: parentObjId,
            childClass: childClass,
            childObjId: childObjId,
            childNodeType: childNodeType
        });
    };

    $rootScope.$on('modalClosed', function (event, data) {
        if (data === "update" && !$rootScope.IsReloaded) {
            event.preventDefault();
            event.stopPropagation();
            $rootScope.IsReloaded = true;
            $scope.Refresh();
        }
    });

    $rootScope.$on('relatedModalFormClosed', function (event, args) {
        $state.reload();
    });
});