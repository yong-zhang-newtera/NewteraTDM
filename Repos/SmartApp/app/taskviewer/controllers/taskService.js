"use strict";

angular.module('app.taskviewer').factory('taskService', function ($http, $q, APP_CONFIG) {

    var createTaskTree = function (treeData) {
        var node, rootMenuItem, roots = [];
        node = treeData;

        var rootMenuItem = {};
        //rootMenuItem.content = "<span><i class=\"fa fa-lg fa-plus-circle\"></i> " + node.title + "</span>";
        rootMenuItem.content = "<span class='label label-info'><i class=\"fa fa-lg fa-plus-circle\"></i>&nbsp;&nbsp;<a class=\"station-a\" href=\"javascript:angular.element(document.getElementById('taskDataTree')).scope().GoToTaskInfoView('" + node.ClassName + "', '" + node.ID + "');\">" + node.Name + "</a></span>";
        rootMenuItem.children = [];
        rootMenuItem.expanded = true;
        roots.push(rootMenuItem);

        addChildMenuItems(rootMenuItem, treeData.Children);

        return roots;
    };

    var addChildMenuItems = function (parentItem, nodes) {
        var node, menuItem;

        if (nodes != null) {
            for (var i = 0; i < nodes.length; i += 1) {
                node = nodes[i];

                menuItem = {};
                menuItem.children = [];

                if (node.Children.length > 0) {
                    menuItem.expanded = true;
                    menuItem.content = "<span class='label label-info'><i class=\"fa fa-lg fa-plus-circle\"></i>&nbsp;&nbsp;<a class=\"station-a\" href=\"javascript:angular.element(document.getElementById('taskDataTree')).scope().GoToTaskInfoView('" + node.ClassName + "', '" + node.ID + "');\">" + node.Name + "</a></span>";
                } else {
                    menuItem.content = "<span class='label label-info'><a class=\"station-a\" href=\"javascript:angular.element(document.getElementById('taskDataTree')).scope().GoToTaskInfoView('" + node.ClassName + "', '" + node.ID + "');\">" + node.Name + "</a></span>";
                }

                parentItem.children.push(menuItem);

                addChildMenuItems(menuItem, node.Children);
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
                callback(createTaskTree(treeData));
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