"use strict";

angular.module("app.blobmanager", ["ui.router", "ui.bootstrap"]);

angular.module("app.blobmanager").config(function ($stateProvider, modalStateProvider) {

    $stateProvider
        .state('app.blobmanager', {
            url: '/blobmanager/:schema/:class/:oid/:prefix/:cmdHash',
            data: {
                title: 'blobmanager Viewer'
            }
        });
    });