"use strict";

angular.module("app.masterdetailtable", ["ngResource", "smart-table", "dx", "ui.router", "ui.bootstrap", "ui.bootstrap.modal", "ngProgress"]);

angular.module("app.masterdetailtable")
    .provider('modalState', function ($stateProvider, $injector) {
        var provider = this;
        this.$get = function () {
            return provider;
        }
        this.state = function (stateName, options) {
            var modalInstance;
            $stateProvider.state(stateName, {
                url: options.url,
                data: {
                    title: 'Modal'
                },
                onEnter: function ($modal, $state, $injector) {
                    modalInstance = $modal.open(options);
                    modalInstance.result.then(function (data) {
                        // modal closed
                        var rScope = $injector.get('$rootScope');
                        rScope.$emit('modalClosed', data);

                    }, function () {
                        // modal dismissed
                        var rScope = $injector.get('$rootScope');
                        rScope.$emit('modalDismissed', "");
                    })['finally'](function () {
                        modalInstance = null;
                        if ($state.$current.name === stateName) {
                            $state.go('^', {}, {location:false, notify: false });
                        }
                    });
                },
                onExit: function () {
                    if (modalInstance) {
                        modalInstance.close();
                    }
                },
                resolve : options.resolve
            });
        };
    })
    .config(function ($stateProvider, modalStateProvider, $urlRouterProvider) {

        $stateProvider
            .state('app.masterdetailtable', {
                abstract: true,
                data: {
                    title: 'Master Detail Table'
                },
                resolve: {
                    scripts: function (lazyScript) {
                        return lazyScript.register('dropzone')
                    }
                }
            })
            .state('app.masterdetailtable.datagrid', {
                url: '/datagrid/:schema/:class/:edit/:delete/:insert/:track/:export/:import/:cart/:search/:reports/:attachment/:hash',
                data: {
                    title: 'Smart Data Table',
                    animation: false /* disable the content loading animation since $viewContentLoaded will not fire when opening modal */
                },
                authenticate: true,
                views: {
                    "content@app": {
                        controller: 'dataGridCtrl',
                        templateUrl: "app/masterdetailtable/views/datagrid.html"
                    }
                },
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
                    },
                    propmisedParams: function ($http, APP_CONFIG, $stateParams) {
                        if ($stateParams.hash) {
                            return $http.get(APP_CONFIG.ebaasRootUrl + "/api/sitemap/parameters/" + $stateParams.hash);
                        }
                        else
                        {
                            return [];
                        }
                    }
                }
            })
            .state('app.masterdetailtable.datagrid.related', {
                url: '/datagridrelated/:schema/:class/:oid/:relatedclass/:insertrelated/:editrelated/:attachmentrelated/:deleterelated/:exportrelated/:importrelated',
                data: {
                    title: 'Smart Data Grid',
                    animation: false /* disable the content loading animation since $viewContentLoaded will not fire when opening modal */
                },
                views: {
                    "content@app": {
                        controller: 'relatedDataGridCtrl',
                        templateUrl: "app/masterdetailtable/views/related-datagrid.html"
                    }
                },
                resolve: {
                    promiseParentClassInfo: function ($http, APP_CONFIG, $stateParams) {
                        var url = APP_CONFIG.ebaasRootUrl + "/api/metadata/class/" + encodeURIComponent($stateParams.schema) + "/" + $stateParams.class;
                        return $http.get(url);
                    }
                }
            })
            .state('app.masterdetailtable.datagrid.form', {
                url: '^/datagridform/:schema/:class/:oid/:template/:formAttribute',
                data: {
                    title: 'Smart Form',
                    animation: false /* disable the content loading animation since $viewContentLoaded will not fire when opening modal */
                },
                views: {
                    "content@app": {
                        controller: 'ebaasFormCtrl',
                        templateUrl: "app/smartforms/views/ebaas-form.html"
                    }
                },
                resolve: {
                    parentStateName: function ($state) {
                        return $state.current.name;
                    }
                }
            });

        modalStateProvider.state('app.masterdetailtable.datagrid.help', {
            url: '^/datagridhelp/:hash',
            templateUrl: "app/layout/partials/help-viewer.tpl.html",
            controller: 'helpViewerCtlr',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.masterdetailtable.datagrid.form.pickpk', {
            url: '^/datagridformpickpk/:pkclass/:property/:filter/:callback',
            templateUrl: "app/smartforms/views/pick-primary-key.html",
            controller: 'pickPrimaryKeyCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.masterdetailtable.datagrid.form.viewmanytomany', {
            url: '^/datagridformviewmanytomany/:masterclass/:relatedclass/:masterid',
            templateUrl: "app/smartforms/views/view-many-to-many.html",
            controller: 'viewManyToManyCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.masterdetailtable.datagrid.form.uploadimage', {
            url: '^/datagridformuploadimage/:property/:imageid',
            templateUrl: "app/smartforms/views/upload-image.html",
            controller: 'uploadImageCtrl',
            animation: false,
            size: 'md'
        });

        modalStateProvider.state('app.masterdetailtable.datagrid.form.viewlog', {
            url: '^/datagridformviewlog/:logschema/:logclass/:logoid/:logproperty',
            templateUrl: "app/logs/views/change-log-viewer.html",
            controller: 'changeLogViewerCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.masterdetailtable.datagrid.form.relatedform', {
            url: '^/datagridformrelatedform/:rclass/:roid/:rtemplate/:rformAttribute/:readonly',
            templateUrl: "app/smartforms/views/related-form-modal.html",
            controller: 'relatedFormModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.masterdetailtable.datagrid.related.relatedform', {
            url: '^/relatedform/:schema/:rclass/:roid/:rtemplate/:rformAttribute/:readonly',
            templateUrl: "app/smartforms/views/related-form-modal.html",
            controller: 'relatedFormModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.masterdetailtable.datagrid.related.relatedform.pickpk', {
            url: '^/datagridrelatedrelatedformpickpk/:pkclass/:property/:filter/:callback',
            templateUrl: "app/smartforms/views/pick-primary-key.html",
            controller: 'pickPrimaryKeyCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.masterdetailtable.datagrid.related.relatedform.viewmanytomany', {
            url: '^/datagridrelatedrelatedformviewmanytomany/:masterclass/:relatedclass/:masterid',
            templateUrl: "app/smartforms/views/view-many-to-many.html",
            controller: 'viewManyToManyCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.masterdetailtable.datagrid.related.relatedform.uploadimage', {
            url: '^/datagridrelatedrelatedformuploadimage/:property/:imageid',
            templateUrl: "app/smartforms/views/upload-image.html",
            controller: 'uploadImageCtrl',
            animation: false,
            size: 'md'
        });

        modalStateProvider.state('app.masterdetailtable.datagrid.related.relatedform.viewlog', {
            url: '^/datagridrelatedrelatedformviewlog/:logschema/:logclass/:logoid/:logproperty',
            templateUrl: "app/logs/views/change-log-viewer.html",
            controller: 'changeLogViewerCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.masterdetailtable.datagrid.related.attachments', {
            url: '^/relatedachments/:schema/:class/:oid?readonly',
            templateUrl: "app/attachments/views/attachments-modal.html",
            controller: 'attachmentsModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.masterdetailtable.datagrid.filemanager', {
            url: '^/datagridfilemanager/:schema/:class/:oid/:cmdHash',
            templateUrl: "app/fileManager/views/file-manager-viewer.html",
            controller: 'fileManagerViewerCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.masterdetailtable.datagrid.dataviewer', {
            url: '^/datagriddataviewer/:schema/:class/:oid/:xmlschema',
            templateUrl: "app/dataviewer/views/data-viewer-modal.html",
            controller: 'DataViewerModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.masterdetailtable.datagrid.related.modalform', {
            url: '^/datagridrelatedmodalform/:schema/:class/:oid/:readonly/:template/:formAttribute/:duplicate/:cmd/:sref',
            templateUrl: "app/smartforms/views/ebaas-form-modal.html",
            controller: 'ebaasFormModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.masterdetailtable.datagrid.related.modalform.relatedform', {
            url: '^/datagridrelatedformmodalformrelatedform/:schema/:rclass/:roid/:rtemplate/:rformAttribute/:readonly',
            templateUrl: "app/smartforms/views/related-form-modal.html",
            controller: 'relatedFormModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.masterdetailtable.datagrid.related.modalform.pickpk', {
            url: '^/datagridrelatedformmodalformpickpk/:pkclass/:property/:filter/:callback',
            templateUrl: "app/smartforms/views/pick-primary-key.html",
            controller: 'pickPrimaryKeyCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.masterdetailtable.datagrid.related.modalform.viewmanytomany', {
            url: '^/datagridrelatedformmodalformviewmanytomany/:masterclass/:relatedclass/:masterid',
            templateUrl: "app/smartforms/views/view-many-to-many.html",
            controller: 'viewManyToManyCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.masterdetailtable.datagrid.related.modalform.uploadimage', {
            url: '^/datagridrelatedformmodalformuploadimage/:property/:imageid',
            templateUrl: "app/smartforms/views/upload-image.html",
            controller: 'uploadImageCtrl',
            animation: false,
            size: 'md'
        });

        modalStateProvider.state('app.masterdetailtable.datagrid.related.modalform.viewlog', {
            url: '^/datagridrelatedformmodalformviewlog/:logschema/:logclass/:logoid/:logproperty',
            templateUrl: "app/logs/views/change-log-viewer.html",
            controller: 'changeLogViewerCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.masterdetailtable.datagrid.related.modalform.attachments', {
            url: '^/datagridrelatedformmodalformachments/:schema/:class/:oid?readonly',
            templateUrl: "app/attachments/views/attachments-modal.html",
            controller: 'attachmentsModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });
    });