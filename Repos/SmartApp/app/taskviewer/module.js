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
            url: '/details/:schema/:class/:oid/:taskNodeAttribute/:itemClass/:itemOid/:itemNodeAttribute/:packetClass/:packetOid/:packetNodeAttribute/:packetPrefix/:packetPrefixAttribute/:activeTabId',
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

    modalStateProvider.state('app.taskviewer.details.modalform', {
        url: '^/detailsmodalform/:schema/:class/:readonly/:oid/:template/:formAttribute/:duplicate/:cmd/:sref',
        templateUrl: "app/smartforms/views/ebaas-form-modal.html",
        controller: 'ebaasFormModalCtrl',
        backdrop: 'static', /*  this prevent user interaction with the background  */
        keyboard: false,
        animation: false,
        size: 'lg'
    });

    modalStateProvider.state('app.taskviewer.details.relatedform', {
        url: '^/detailsmodalform/:rclass/:roid/:rtemplate/:rformAttribute/:readonly',
        templateUrl: "app/smartforms/views/related-form-modal.html",
        controller: 'relatedFormModalCtrl',
        backdrop: 'static', /*  this prevent user interaction with the background  */
        keyboard: false,
        animation: false,
        size: 'lg'
    });
});