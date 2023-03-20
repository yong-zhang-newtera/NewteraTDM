'use strict';

angular.module('app.myspace').controller('mySpaceCtrl', function ($scope, $rootScope, $http, $state, $stateParams, APP_CONFIG, User, TasksInfo, myActivityService, blogService, promiseTasks) {

    $scope.dbschema = $stateParams.schema;
    $scope.blogClass = "Blog";

    $scope.user = User;

    $scope.itemsByPage = 15;

    $scope.rowCollection = promiseTasks.data;

    $scope.numOfPages = Math.ceil(promiseTasks.data.length / $scope.itemsByPage);

    TasksInfo.tasks = promiseTasks.data;
    TasksInfo.count = promiseTasks.data.length; // other components are watching task number changes through this service
    $scope.taskCount = TasksInfo.count;

    myActivityService.getbytype("msgs", function (data) {
        myActivityService.MessageModel.items = data;

    });

    // Getting my blogs
    blogService.getMyBlogs("COMMON", $scope.blogClass, User.userName, function (result) {
        $scope.blogs = result.data;

    });

    $scope.getPosterImage = function(posterId)
    {
        return User.getUserImage(posterId);
    }

    $scope.getMsgItems = function () {
        return myActivityService.MessageModel.items;
    }

    $scope.getMsgCount = function () {
        return myActivityService.MessageModel.items.length;
    }

    $scope.readMsg = function (msg) {
        var url = msg.url;
        var urlparams = msg.urlparams;

        urlparams = urlparams.replace(/msg.dbschema/, "\"" + msg.dbschema + "\""); // replace msg.dbschema
        urlparams = urlparams.replace(/msg.dbclass/, "\"" + msg.dbclass + "\""); // replace msg.dbclass
        urlparams = urlparams.replace(/msg.oid/, "\"" + msg.oid + "\""); // replace msg.dbclass

        var params = JSON.parse(urlparams);

        if (url) {
            $state.go(url, params);
        }
    }

    $scope.deleteMsg = function (msg) {
        var found = false;
        var index = undefined;

        for (var i = 0; i < myActivityService.MessageModel.items.length; i++) {
            var activity = myActivityService.MessageModel.items[i];
            if (activity.objId === msg.objId) {
                index = i;
                found = true;
                break;
            }
        }

        if (found) {
            myActivityService.MessageModel.items.splice(index, 1);
        }

        myActivityService.remove("msgs", msg.objId, function (data) {
            myActivityService.MessageModel.count = myActivityService.MessageModel.items.length;
        });
    }

    $scope.RefreshTasks = function()
    {
        $state.reload();
    }

    $scope.OpenSetSubstitute = function () {
        $state.go(".substitute", { schema: $scope.dbschema });
    }

    $rootScope.$on('modalClosed', function (event, data) {
        if (data === "update")
            $scope.RefreshTasks();
    });

    $scope.finishedTasks = [];
    $scope.tableState;
    $scope.pageSize = 15;

    $scope.isLoading = true;

    $scope.callServer = function (tableState) {

        $scope.isLoading = true;

        $scope.tableState = tableState;

        var pagination = tableState.pagination;

        var start = pagination.start || 0;     // This is NOT the page number, but the index of item in the list that you want to use to display the table.
        var number = pagination.number || $scope.pageSize;  // Number of entries showed per page.

        $http.get(APP_CONFIG.ebaasRootUrl + "/api/tasks/finished/user/" + encodeURIComponent($scope.dbschema) + "?from=" + start + "&size=" + number).success(function (data) {
            // then get total count
            $http.get(APP_CONFIG.ebaasRootUrl + "/api/tasks/finished/user/" + encodeURIComponent($scope.dbschema) + "/count").success(function (count) {
                $scope.finishedTasks = data;

                tableState.pagination.numberOfPages = Math.ceil(count / $scope.pageSize);//set the number of pages so the pagination can update
                
                $scope.isLoading = false;
            });
        })
    };

    $scope.refreshFinishedTasks = function () {
        $scope.callServer($scope.tableState);
    }

    $scope.clearFinishedTasks = function()
    {
        $scope.isLoading = true;

        $http.delete(APP_CONFIG.ebaasRootUrl + "/api/tasks/finished/" + encodeURIComponent($scope.dbschema)).success(function () {

            $scope.isLoading = false;

            $scope.callServer($scope.tableState);
        })
    }

    $scope.hasFinishedTasks = function () {
        if ($scope.finishedTasks && $scope.finishedTasks.length > 0)
            return true;
        else
            return false;
    }
});