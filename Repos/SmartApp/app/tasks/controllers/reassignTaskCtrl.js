"use strict";

angular.module('app.tasks').controller('reassignTaskCtrl', function ($scope, $http, $stateParams, $modalInstance, APP_CONFIG, User) {


    var url = APP_CONFIG.ebaasRootUrl + "/api/accounts/users";

    $http.get(url).success(function (data) {

        $scope.userInfos = data;

        $scope.selected = { item: {}};
    });

    $scope.assignTask = function()
    {
        if ($scope.selected.item.userName) {
            $http.get(APP_CONFIG.ebaasRootUrl + "/api/tasks/reassign/" + encodeURIComponent($stateParams.schema) + "/" + $stateParams.taskid + "/" + User.userName + "/" + $scope.selected.item.userName)
                .success(function () {
                    $scope.selected = { item: {} };
                    $modalInstance.close("update");
            });
        }
    }

    $scope.closeModal = function () {
        $modalInstance.dismiss("dismiss");
    };
});