"use strict";

angular.module("app.taskviewer", ["ui.router", "ui.bootstrap"]);

angular.module("app.taskviewer").config(function ($stateProvider, modalStateProvider) {

    $stateProvider
        .state('app.taskviewer', {
            abstract: true,
            url: '/taskviewer/:schema/:class/:oid/:taskNodeAttribute/:taskTemplate/:itemClass/:itemNodeAttribute/:itemTemplate/:packetClass/:packetNodeAttribute/:packetTemplate/:packetPrefix/:packetPrefixAttribute/:hash',
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
            url: '/details/:schema/:class/:oid/:taskNodeAttribute/:taskTemplate/:itemClass/:itemOid/:itemNodeAttribute/:itemTemplate/:packetClass/:packetOid/:packetNodeAttribute/:packetTemplate/:packetPrefix/:packetPrefixAttribute/:activeTabId',
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
        url: '^/detailsmodalform/:masterclass/:masteroid/:rclass/:roid/:rtemplate/:rformAttribute/:readonly',
        templateUrl: "app/smartforms/views/related-form-modal.html",
        controller: 'relatedFormModalCtrl',
        backdrop: 'static', /*  this prevent user interaction with the background  */
        keyboard: false,
        animation: false,
        size: 'lg'
    });

    modalStateProvider.state('app.taskviewer.details.relatedform.pickpk', {
        url: '^/detailrelatedformpickpk/:pkclass/:property/:filter/:callback',
        templateUrl: "app/smartforms/views/pick-primary-key.html",
        controller: 'pickPrimaryKeyCtrl',
        animation: false,
        size: 'lg'
    });

    modalStateProvider.state('app.taskviewer.details.relatedform.viewmanytomany', {
        url: '^/detailrelatedformviewmanytomany/:masterclass/:relatedclass/:masterid',
        templateUrl: "app/smartforms/views/view-many-to-many.html",
        controller: 'viewManyToManyCtrl',
        animation: false,
        size: 'lg'
    });
});