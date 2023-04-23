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
    }
    else {
        taskService.getTaskTree(parameters, function (treeData) {
            $scope.taskDataTree = treeData;
            MetaDataCache.setNamedData(treeName, treeData);
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

    const contextMenuItems = [
        {
            text: 'Share',
            items: [
                { text: 'Facebook' },
                { text: 'Twitter' }],
        },
        { text: 'Download' },
        { text: 'Comment' },
        { text: 'Favorite' },
    ];

    $scope.contextMenuOptions = {
        dataSource: "",
        width: 200,
        target: "#contextMenuImage",
        onItemClick(e) {
            if (!e.itemData.items) {
                DevExpress.ui.notify(`The "${e.itemData.text}" item was clicked`, 'success', 1500);
            }
        },
    };

    $rootScope.$on('modalClosed', function (event, args) {
        if (args === "update") {
            $state.reload();
        }
    });

});