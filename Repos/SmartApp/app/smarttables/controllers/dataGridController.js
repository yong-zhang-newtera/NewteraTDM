﻿'use strict';

angular.module('app.smarttables').controller('dataGridCtrl', function ($scope, $controller, $rootScope, $http, APP_CONFIG, $stateParams, $state, propmisedParams, hubService) {
   
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.class;
    $scope.oid = $stateParams.oid;

    var params = propmisedParams.data;

    if (params) {
        $scope.view = params['dataView'];
        $scope.formTemplate = params['formTemplate'];
    }
    else
    {
        $scope.view = undefined;
        $scope.formTemplate = undefined;
    }

    if ($stateParams.insert && $stateParams.insert === "false") {
        $scope.add = false;
    }
    else {
        $scope.add = true;
    }

    if ($stateParams.attachment && $stateParams.attachment === "false") {
        $scope.attachment = false;
    }
    else {
        $scope.attachment = true;
    }
 
    if ($stateParams.export && $stateParams.export === "true")
    {
        $scope.exportData = true;
    }
    else
    {
        $scope.exportData = false;
    }

    if ($stateParams.import && $stateParams.import === "true") {
        $scope.importData = true;
    }
    else {
        $scope.importData = false;
    }

    if ($stateParams.track && $stateParams.track === "true") {
        $scope.track = true;
    }
    else {
        $scope.track = false;
    }

    if ($stateParams.search && $stateParams.search === "fulltext")
    {
        $scope.isfulltextsearch = true;
    }
    else
    {
        $scope.isfulltextsearch = false;
    }

    angular.extend(this, $controller('dataGridBaseCtrl', { $scope: $scope, $rootScope: $rootScope, $http: $http, APP_CONFIG: APP_CONFIG}));

    $scope.openModal = function() {
        $state.go('.modalform', { schema: $scope.dbschema, class: $scope.dbclass, template: $scope.formTemplate }, { location: false, notify: false });
    };

    $scope.GetCommands = function (rowIndex, data)
    {
        var items = new Array();

        var url = APP_CONFIG.ebaasRootUrl + "/api/sitemap/commands/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + data.obj_id;

        $http.get(url).success(function (commands) {

            // custom commands
            $scope.commands = commands;
            var cmdInfo;
            var item;
            for (var cmd in commands) {
                if (commands.hasOwnProperty(cmd)) {
                    cmdInfo = commands[cmd];
                    item = new Object();
                    item.text = cmdInfo.title;
                    item.css = "btn btn-primary btn-md btn-nav";
                    item.track = false;
                    if (cmdInfo.icon) {
                        item.icon = cmdInfo.icon;
                    }
                    else
                    {
                        item.icon = "fa fa-lg fa-tasks";
                    }

                    item.onItemClick = function (text) {
                        gotoState(text, $scope.dbschema, data.type, data.obj_id, !data.allowWrite)
                    }

                    items.push(item);

                    if (cmdInfo.baseUrl && !APP_CONFIG.hashedBaseUrls[cmdInfo.hash]) {
                        APP_CONFIG.hashedBaseUrls[cmdInfo.hash] = cmdInfo.baseUrl;
                    }
                }
            }

            // add standard commands
            if (data.allowWrite && $stateParams.edit !== "false") {
                items.push({
                    text: $rootScope.getWord('Edit'),
                    icon: "fa fa-lg fa-edit",
                    css: "btn btn-default btn-md",
                    track : false,
                    onItemClick: function () {
                        $state.go('.modalform', { schema: $scope.dbschema, class: data.type, oid: data.obj_id, template: $scope.formTemplate, duplicate: "true" }, { location: false, notify: false });
                    }
                });
            }

            /*
            if (data.allowCreate && $stateParams.insert !== "false") {
                items.push({
                    text: $rootScope.getWord('Add'),
                    icon: "fa fa-lg fa-plus-square",
                    css: "btn btn-default btn-md btn-nav",
                    track : false,
                    onItemClick: function () {
                        $state.go('.modalform', { schema: $scope.dbschema, class: data.type }, { location: false, notify: false });
                    }
                });
            }
            */

            if (data.allowDelete && $stateParams.delete !== "false") {
                items.push({
                    text: $rootScope.getWord('Delete'),
                    icon: "fa fa-lg fa-times",
                    css: "btn btn-default btn-md",
                    track: false,
                    onItemClick: function () {
                        $scope.gridInstance.deleteRow(rowIndex);
                    }
                });
            }

            if ($scope.attachment) {
                items.push({
                    text: $rootScope.getWord('Attachments'),
                    icon: "fa fa-lg fa-file-archive-o",
                    css: "btn btn-default btn-md",
                    track: false,
                    onItemClick: function () {
                        $state.go('.attachments', { schema: $scope.dbschema, class: data.type, oid: data.obj_id, readonly: !data.allowWrite}, { location: false, notify: false });
                    }
                });
            }

            if ($scope.track) {
                var groupName = $scope.dbschema + "-" + data.type + "-" + data.obj_id;
                var isTracking = false;
                hubService.isUserInGroup(groupName, function (status) {
                    isTracking = status;

                    items.push({
                        text: $rootScope.getWord('Track Status'),
                        icon: "fa fa-lg fa-file-archive-o",
                        css: "btn btn-default btn-md",
                        track: true,
                        trackStatus: isTracking,
                        onItemClick: function () {

                            if (isTracking) {
                                hubService.removeFromGroup(groupName); // hubService removes the current user from the group
                            }
                            else {
                                hubService.addToGroup(groupName); // hubService adds the current user to the group
                            }
                        }
                    });
                });
            }
        });
        return items;
    }

    var gotoState = function (title, dbschema, dbclass, oid, readonly)
    {
        var commands = $scope.commands;
        var url = undefined;
        var cmdUrl = undefined;
        var params = undefined;
        var cmdInfo;
        for (var cmd in commands) {
            if (commands.hasOwnProperty(cmd)) {
                cmdInfo = commands[cmd];
                if (cmdInfo.title === title) {
                    url = cmdInfo.url;
                    cmdUrl = cmdInfo.url;
                    params = new Object();
                    params.schema = dbschema;
                    params.class = dbclass;
                    params.oid = oid;
                    params.readonly = readonly;
                    params.cmdHash = cmdInfo.hash;
   
                    // add command's parameters to the state parameters
                    if (cmdInfo.parameters) {
                        for (var key in cmdInfo.parameters) {
                            if (cmdInfo.parameters.hasOwnProperty(key)) {
                                params[key] = cmdInfo.parameters[key];
                            }
                        }
                    };

                    break;
                }
            }
        }

        if (url) {

            if (cmdUrl === ".modalform") {
                $state.go(url, params, { location: false, notify: false });
            }
            else {
                $state.go(url, params);
            }
        }
    }

    $scope.gridInstance = null;
    $scope.dataGridSettings = {
        dataSource : {
            store: $scope.customStore
        },
        columnAutoWidth: true,
        height: $rootScope.isChrome() === true ? '750px' : undefined,
        sorting: {
            mode: "multiple"
        },
        searchPanel: {
            visible: $stateParams.search && $stateParams.search === "true"? true: false,
            width: 300,
            placeholder: $rootScope.getWord("Keyword Search")
        },
        editing: {
            allowAdding: false,
            allowUpdating: false,
            allowDeleting: false
        },
        grouping: {
            autoExpandAll: false
        },
        pager: {
            visible: true,
            showPageSizeSelector: false,
            showInfo: true
        },
        filterRow: {
            visible: $scope.isfulltextsearch ? false : true,
            applyFilter: "auto"
        },
        selection: { mode: 'single' },
        remoteOperations: true,
        bindingOptions: {
            columns: 'columns',
            'scrolling.showScrollbar': 'never'
        },
        headerFilter: {
            visible: true
        },
        rowAlternationEnabled: true,
        masterDetail: {
            enabled: true,
            template: "detail"
        },
        onRowClick: function(e)
        {
            if (e.rowType === "data") {
                var isExpanded = $scope.gridInstance.isRowExpanded(e.key);
                $scope.gridInstance.collapseAll(-1); // collaspsed all
                if (!isExpanded)
                    {
                    $scope.gridInstance.expandRow(e.key);
                    }
            }
        },
        onInitialized: function (e) {
            $scope.gridInstance = e.component;
        }
    };

    $rootScope.$on('modalClosed', function (event, data) {
        if ($scope.gridInstance && data === "update")
            $scope.gridInstance.refresh();
    });

    $scope.downloadReports = function () {
        $state.go(".downloadreports", { schema: $scope.dbschema, class: $scope.dbclass }, { location: false, notify: false });
    }
});