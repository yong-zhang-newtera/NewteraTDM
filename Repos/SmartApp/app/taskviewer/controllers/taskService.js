"use strict";

angular.module('app.taskviewer').factory('taskService', function ($http, $q, APP_CONFIG) {

    var createTaskTree = function (treeData) {
        var node, rootMenuItem, roots = [];
        var params = {};
        params.index = 0;
        node = treeData;

        var rootMenuItem = {};
        rootMenuItem.content = "<span class='label label-info'><i class=\"fa fa-lg fa-plus-circle\"></i>&nbsp;&nbsp;<a class=\"station-a\" href=\"javascript:angular.element(document.getElementById('taskDataTree')).scope().GoToTaskInfoView('" + node.ClassName + "', '" + node.ID + "', '" + escapePath(node.Prefix) + "');\">" + node.Name + "</a></span>";
        rootMenuItem.index = params.index;
        rootMenuItem.children = [];
        rootMenuItem.expanded = true;
        roots.push(rootMenuItem);

        addChildMenuItems(rootMenuItem, treeData.Children, params);

        return roots;
    };

    var addChildMenuItems = function (parentItem, nodes, params) {
        var node, menuItem;
        if (nodes != null) {
            for (var i = 0; i < nodes.length; i += 1) {
                node = nodes[i];
                params.index++;
                menuItem = {};
                menuItem.children = [];
                menuItem.index = params.index;

                if (node.Children.length > 0) {
                    menuItem.expanded = true;
                    menuItem.content = "<span class='label label-info'><i class=\"fa fa-lg fa-plus-circle\"></i>&nbsp;&nbsp;<a class=\"station-a\" href=\"javascript:angular.element(document.getElementById('taskDataTree')).scope().GoToTaskInfoView('" + node.ClassName + "', '" + node.ID + "', '" + escapePath(node.Prefix) + "');\">" + node.Name + "</a></span>";
                } else {
                    menuItem.content = "<span class='label label-info'><a class=\"station-a\" href=\"javascript:angular.element(document.getElementById('taskDataTree')).scope().GoToTaskInfoView('" + node.ClassName + "', '" + node.ID + "', '" + escapePath(node.Prefix) + "');\">" + node.Name + "</a></span>";
                }

                parentItem.children.push(menuItem);

                addChildMenuItems(menuItem, node.Children, params);
            }
        }
    }

    var escapePath = function (path) {
        if (path) {
            path = path.replaceAll("\\", "\\\\");
        }

        return path;
    }

    var flattenTreeNodes = function (treeNode) {
        var nodes = [];
        var params = {};
        params.index = 0;
        var node = {};
        node.index = params.index;
        node.isTaskNode = true;
        node.className = treeNode.ClassName;
        node.childClass = treeNode.ChildClass;
        node.objId = treeNode.ID;
        nodes.push(node);

        flatternChildNodes(nodes, treeNode.Children, params);

        return nodes;
    }

    var flatternChildNodes = function (nodes, childTreeNodes, params) {
        var childTreeNode, node;
        if (childTreeNodes != null) {
            for (var i = 0; i < childTreeNodes.length; i += 1) {
                childTreeNode = childTreeNodes[i];
                params.index++;
                node = {};
                node.index = params.index;
                if (childTreeNode.Type === "TestItem") {
                    node.isItemNode = true;
                }
                else if (childTreeNode.Type === "TestPacket") {
                    node.isPacketNode = true;
                }
                node.className = childTreeNode.ClassName;
                node.childClass = childTreeNode.ChildClass;
                node.objId = childTreeNode.ID;

                nodes.push(node);

                flatternChildNodes(nodes, childTreeNode.Children, params);
            }
        }
    }

    function getTaskTree(parameters, callback) {

        // url to get task instance
        var url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent(parameters.schema) + "/" + parameters.taskClass + "/" + parameters.taskOid + "/custom/GetTaskTree";
        var urlWithParams = url + "?itemClass=" + parameters.itemClass;
        urlWithParams += "&packetClass=" + parameters.packetClass;
        urlWithParams += "&taskNodeAttribute=" + parameters.taskNodeAttribute;
        urlWithParams += "&itemNodeAttribute=" + parameters.itemNodeAttribute;
        urlWithParams += "&packetNodeAttribute=" + parameters.packetNodeAttribute;
        $http.get(urlWithParams).success(function (data) {
            var treeData = data;
            if (callback != null) {
                callback(createTaskTree(treeData),
                    flattenTreeNodes(treeData),
                );
            }
        }).error(function () {
            callback(undefined);
        });
    }

    function hasValue(val) {
        if (val == null || val == undefined || val == "") {
            return false;
        }
        else {
            return true;
        }
    }
	
	return {
        getTaskTree: function (parameters, callback) {
            return getTaskTree(parameters, callback);
        },
        hasValue: function (val) {
            return hasValue(val);
        }
	}
});