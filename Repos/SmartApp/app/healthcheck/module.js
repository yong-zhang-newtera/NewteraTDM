"use strict";

angular.module("app.healthcheck", ["ui.router", "ui.bootstrap"])
.config(function ($stateProvider) {

    $stateProvider
        .state('healthcheck', {
            url: '/healthcheck',
            data: {
                title: 'Health Check'
            },
            views: {
                root: {
                    templateUrl: "app/healthcheck/views/health-check-view.html",
                    controller: "HealthCheckCtrl"
                }
            }
        });
    });