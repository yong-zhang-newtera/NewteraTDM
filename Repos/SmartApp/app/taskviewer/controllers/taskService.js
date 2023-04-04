"use strict";

angular.module('app.taskviewer').factory('taskService', function ($http, APP_CONFIG) {

    var createTaskTree = function (treeData) {
        var node, rootMenuItem, roots = [];
        node = treeData;

        var rootMenuItem = {};
        //rootMenuItem.content = "<span><i class=\"fa fa-lg fa-plus-circle\"></i> " + node.title + "</span>";
        rootMenuItem.content = "<span class='label label-info'><i class=\"fa fa-lg fa-plus-circle\"></i>&nbsp;&nbsp;<a class=\"station-a\" href=\"javascript:angular.element(document.getElementById('taskDataTree')).scope().GoToTaskInfoView('" + node.id + "');\">" + node.name + "</a></span>";
        rootMenuItem.children = [];
        rootMenuItem.expanded = true;
        roots.push(rootMenuItem);

        addChildMenuItems(rootMenuItem, treeData.children);

        return roots;
    };

    var addChildMenuItems = function (parentItem, nodes) {
        var node, menuItem;

        if (nodes != null) {
            for (var i = 0; i < nodes.length; i += 1) {
                node = nodes[i];

                menuItem = {};
                menuItem.children = [];

                if (node.children.length > 0) {
                    menuItem.content = "<span class='label label-info'><i class=\"fa fa-lg fa-plus-circle\"></i>&nbsp;&nbsp;<a class=\"station-a\" href=\"javascript:angular.element(document.getElementById('taskDataTree')).scope().GoToTaskInfoView('" + node.name + "');\">" + node.title + "</a></span>";
                } else {
                    menuItem.content = "<span class='label label-info'><a class=\"station-a\" href=\"javascript:angular.element(document.getElementById('taskDataTree')).scope().GoToTaskInfoView('" + node.name + "');\">" + node.title + "</a></span>";
                }

                parentItem.children.push(menuItem);

                addChildMenuItems(menuItem, node.children);
            }
        }
    }

    function getTaskTree(dbschema, taskclass, taskoid, itemclass, packetclass, callback) {

        // url to get task instance
        var url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent(dbschema) + "/" + taskclass + "/" + taskoid;
        $http.get(url).success(function (task) {
            var pageSize = 500;
            var taskNode = new Object();
            taskNode.id = task.obj_id;
            taskNode.name = task.OrderNumber;
            // url to get related test items of a task
            url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent(dbschema) + "/" + taskclass + "/" + taskoid + "/" + itemclass + "?size=" + pageSize;
            $http.get(url).success(function (items) {
                if (items != null) {
                    var itemNodes = [];
                    taskNode.children = itemNodes;
                    for (var i = 0; i < items.length; i += 1) {
                        var item = terms[i];
                        var itemNode = new Object();
                        itemNode.id = item.obj_id;
                        itemNode.name = item.ItemID;
                        itemNodes.push(itemNode);

                        // url to get related packets to the item
                        url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent(dbschema) + "/" + itemclass + "/" + item.objId + "/" + packetclass + "?size=" + pageSize;
                        $http.get(url).success(function (packets) {
                            if (packets != null) {
                                var packetNodes = [];
                                itemNodes.children = packetNodes;

                                for (var i = 0; i < packets.length; i += 1) {
                                    var packet = packets[i];
                                    var packetNode = new Object();
                                    packetNode.id = packet.obj_id;
                                    packetNode.name = packet.PacketNumber;
                                    packetNodes.push(packetNode);
                                }
                            }
                        }).error(function () {
                            callback(undefined);
                        })
                    }
                }
                callback(createTaskTree(taskNode));
            }).error(function () {
                callback(undefined);
            });
        }).error(function () {
            callback(undefined);
        })
    }
	
	return {
        getTaskTree: function (dbschema, taskclass, taskoid, itemclass, packetclass, callback) {
            return getTaskTree(dbschema, taskclass, taskoid, itemclass, packetclass, callback);
	    }
	}
});