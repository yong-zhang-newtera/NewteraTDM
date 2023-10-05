'use strict';

angular.module('app.taskviewer').controller('TaskViewerLayoutCtrl', function ($rootScope, $scope, $state, $stateParams, taskService, MetaDataCache) {

    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.class;
    $scope.oid = $stateParams.oid;
    $scope.formAttribute = undefined;
    $scope.itemClass = $stateParams.itemClass;
    $scope.itemTemplate = $stateParams.itemTemplate;
    $scope.itemOid = $stateParams.itemOid;
    $scope.packetClass = $stateParams.packetClass;
    $scope.packetOid = $stateParams.packetOid;
    $scope.packetTemplate = $stateParams.packetTemplate;
    $scope.packetPrefix = $stateParams.packetPrefix;
    $scope.taskNodeAttribute = $stateParams.taskNodeAttribute;
    $scope.taskTemplate = $stateParams.taskTemplate;
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
    parameters.taskTemplate = $stateParams.taskTemplate;
    parameters.taskNodeAttribute = $stateParams.taskNodeAttribute;
    parameters.itemClass = $stateParams.itemClass;
    parameters.itemTemplate = $stateParams.itemTemplate;
    parameters.itemNodeAttribute = $stateParams.itemNodeAttribute;
    parameters.packetClass = $stateParams.packetClass;
    parameters.packetNodeAttribute = $stateParams.packetNodeAttribute;
    parameters.packetTemplate = $stateParams.packetTemplate;
    parameters.packetPrefixAttribute = $stateParams.packetPrefixAttribute;

    var taskTreeName = "Task_Tree_" + $scope.dbschema + $scope.dbclass + $scope.oid;
    var flatternTreeName = "Flattern_Task_Tree_" + $scope.dbschema + $scope.dbclass + $scope.oid;

    if (MetaDataCache.getNamedData(taskTreeName)) {
        $scope.taskDataTree = MetaDataCache.getNamedData(taskTreeName);
        $scope.nodes = MetaDataCache.getNamedData(flatternTreeName);

        $state.go("app.taskviewer.details", parameters);
    }
    else {
        taskService.getTaskTree(parameters, function (taskTree, flatternNodes) {
            $scope.taskDataTree = taskTree;
            $scope.nodes = flatternNodes;
            MetaDataCache.setNamedData(taskTreeName, taskTree);
            MetaDataCache.setNamedData(flatternTreeName, $scope.nodes);

            $state.go("app.taskviewer.details", parameters);
        });
    }

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
        params.taskTemplate = $scope.taskTemplate;
        params.itemTemplate = $scope.itemTemplate;
        params.packetTemplate = $scope.packetTemplate;

        if (nodeClass == $scope.itemClass) {
            params.itemOid = nodeOid;
            params.packetOid = null;
            params.packetPrefix = "";
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
            params.packetPrefix = "";
            $state.go("app.taskviewer.details", params, { reload: true });
        }
    }

    $scope.Refresh = function () {
        if ($rootScope.RefreshTaskTree) {
            var treePrefix = "Task_Tree_" + $scope.dbschema + $scope.dbclass;
            var flatternTreePrefix = "Flattern_Task_Tree_" + $scope.dbschema + $scope.dbclass;
            MetaDataCache.clearNamedData(treePrefix);
            MetaDataCache.clearNamedData(flatternTreePrefix);
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

    $scope.$on('deleteParentNodeEvent', function (event, args) {
        var result = confirm($rootScope.getWord("Confirm Delete Test Item"));
        if (result) {
            var params = new Object();
            params.schema = $scope.dbschema;
            params.class = args.parentClass;
            params.oid = args.parentObjId;
            taskService.deleteTreeNode(params, function (result) {
                $rootScope.RefreshTaskTree = true;
                $scope.Refresh();
            });
        }
    });
});