"use strict";	

angular.module('app.healthcheck').controller("HealthCheckCtrl", function ($http, APP_CONFIG, $scope) {
	var url = APP_CONFIG.ebaasRootUrl + "/api/health";

    $scope.loading = true;
    $http.get(url)
        .success(function (data) {
            $scope.results = data;
            $scope.loading = false;
        })
        .error(function (err) {
            $scope.results = null;
            $scope.loading = false;
        });
});