"use strict";

angular.module("app.newtask", ["ui.router", "ui.bootstrap"]);

angular.module("app.newtask").config(function ($stateProvider, modalStateProvider) {

    $stateProvider
        .state('app.newtask', {
            abstract: true,
            data: {
                title: 'New Task',
                animation: false /* disable the content loading animation since $viewContentLoaded will not fire when opening modal */
            }
        })
        .state('app.newtask.create', {
            url: '/newtask/create/:schema/:class/:template/:hash',
            data: {
                title: 'Create Task'
            },
            authenticate: true,
            views: {
                "content@app": {
                    templateUrl: 'app/newtask/views/create-task.html',
                    controller: 'CreateTaskCtrl'
                }
            },
            resolve: {
                propmisedParams: function ($http, APP_CONFIG, $stateParams) {
                    if ($stateParams.hash) {
                        return $http.get(APP_CONFIG.ebaasRootUrl + "/api/sitemap/parameters/" + $stateParams.hash);
                    }
                    else {
                        return [];
                    }
                }
            }
        });
});