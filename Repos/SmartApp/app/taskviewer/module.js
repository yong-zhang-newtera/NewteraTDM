"use strict";

angular.module("app.taskviewer", ["ui.router", "ui.bootstrap"]);

angular.module("app.taskviewer").config(function ($stateProvider, modalStateProvider) {

    $stateProvider
        .state('app.taskviewer', {
            abstract: true,
            url: '/taskviewer/:schema/:class/:oid/:taskNodeAttribute/:itemClass/:itemNodeAttribute/:packetClass/:packetNodeAttribute/:packetPrefix/:packetPrefixAttribute/:hash',
            data: {
                title: 'Task Viewer'
            },
            views: {
                "content@app": {
                    templateUrl: "app/taskviewer/views/task-viewer-layout.html",
                    controller: 'TaskViewerLayoutCtrl'
                },
                'taskdetails@app.taskviewer': { templateUrl: 'app/taskviewer/views/task-details.html', },
            }
        })
        .state('app.taskviewer.details', {
            url: '/details/:schema/:class/:oid/:taskNodeAttribute/:itemClass/:itemOid/:itemNodeAttribute/:packetClass/:packetOid/:packetNodeAttribute/:packetPrefix/:packetPrefixAttribute',
            views: {
                "taskform@app.taskviewer": {
                    controller: 'TaskFormCtrl',
                    templateUrl: "app/taskviewer/views/task-form.html"
                },
                "itemform@app.taskviewer": {
                    controller: 'ItemFormCtrl',
                    templateUrl: "app/taskviewer/views/item-form.html"
                },
                "packetform@app.taskviewer": {
                    controller: 'PacketFormCtrl',
                    templateUrl: "app/taskviewer/views/packet-form.html"
                }
            },
        });
});