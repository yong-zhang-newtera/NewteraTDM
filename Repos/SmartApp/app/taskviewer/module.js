"use strict";

angular.module("app.taskviewer", ["ui.router", "ui.bootstrap"]);

angular.module("app.taskviewer").config(function ($stateProvider, modalStateProvider) {

    $stateProvider
        .state('app.taskviewer', {
            url: '/taskviewer/:schema/:class/:oid/:edit/:delete/:insert/:itemClass/:packetClass/:hash',
            data: {
                title: 'Task Viewer'
            },
            views: {
                "content@app": {
                    templateUrl: "app/taskviewer/views/task-viewer-layout.html",
                    controller: 'TaskViewerLayoutCtrl'
                }
            }
        })
        .state('app.taskviewer.taskinfoview', {
            url: '/infoview/:schema/:class/:node/:itemClass/:packetClass',
            data: {
                title: 'Task Info View'
            },
            views: {
                "taskinfoview@app.taskviewer": {
                    controller: 'TaskInfoViewCtrl',
                    templateUrl: "app/taskviewer/views/task-info-view.html"
                }
            },
            authenticate: true,
            resolve: {
                scripts: function (lazyScript) {
                    return lazyScript.register(
                        [
                            'flot',
                            'flot-resize',
                            'flot-selection',
                            'flot-fillbetween',
                            'flot-orderBar',
                            'flot-pie',
                            'flot-time',
                            'flot-tooltip',
                            'dropzone',
                            'summernote'
                        ])
                }
            }
        });
});