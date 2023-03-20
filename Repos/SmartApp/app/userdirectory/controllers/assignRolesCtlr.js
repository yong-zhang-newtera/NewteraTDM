'use strict';

angular.module('app.userdirectory').controller('assignRolesCtrl', function ($scope, $controller, $rootScope, $http, APP_CONFIG, $stateParams, $modalInstance) {
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = "Role";
    $scope.masterclass = $stateParams.class;
    $scope.masterid = $stateParams.oid;
    $scope.roletype = $stateParams.roletype;

    if ($stateParams.dataview)
    {
        $scope.view = $stateParams.dataview;
    }
    else
    {
        $scope.view = undefined;
    }

    $scope.filter = "['RType', '=', '" + $scope.roletype + "']";

    angular.extend(this, $controller('dataGridBaseCtrl', { $scope: $scope, $rootScope: $rootScope, $http: $http, APP_CONFIG: APP_CONFIG }));

    $scope.gridInstance = null;
    $scope.existingKeys = null;
    $scope.currentKeys = null;
    $scope.isUpdated = false;
    $scope.loading = false;
    $scope.dataGridSettings = {
        dataSource: {
            store: $scope.customStore
        },
        columnAutoWidth: true,
        sorting: {
            mode: "multiple"
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
            showPageSizeSelector: true,
            showInfo: true
        },
        filterRow: {
            visible: true,
            applyFilter: "auto"
        },
        searchPanel: { visible: false },
        selection: { mode: 'multiple' },
        remoteOperations: true,
        bindingOptions: {
            columns: 'columns'
        },
        headerFilter: {
            visible: true
        },
        rowAlternationEnabled: true,
        onInitialized: function (e) {
            $scope.gridInstance = e.component;
        },
        onContentReady: function (e) {
            selectGridRows();
        },
        onSelectionChanged: function (e) {
            changeSelections(e.currentSelectedRowKeys, e.currentDeselectedRowKeys);
        }
    };

    var asyncLoop = function(o)
    {
        var i = -1;

        var loop = function() {
            i++;
            if (i == o.length)
            {
                o.callback();
                return;
            }

            o.functionToLoop(loop, i);
        }

        loop(); // init
    }

    var selectGridRows = function()
    {
        if ($scope.currentKeys) {
            var keys = $scope.currentKeys;
            var indexes = [];
            for (var i = 0; i < keys.length; i++) {
                var index = $scope.gridInstance.getRowIndexByKey(keys[i]);

                if (index >= 0)
                    indexes.push(index);
            }

            $scope.gridInstance.selectRowsByIndexes(indexes, true);
        }
        else {
            if ($scope.masterid) {
                $http.get(APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($stateParams.schema) + "/" + $scope.masterclass + "/" + $scope.masterid + "/" + $scope.dbclass)
                    .success(function (data) {
                        var keys = new Array();
                        if (data) {
                            for (var i = 0; i < data.length; i++) {
                                keys.push(data[i].obj_id);
                            }

                            if (keys.length > 0) {
                                // set the existing selections of rows
                                $scope.gridInstance.selectRows(keys, false);
                            }
                        }

                        $scope.existingKeys = keys; // keep the existing keys
                        // initialize the current keys
                        $scope.currentKeys = [];
                        for (var i = 0; i < keys.length; i++) {
                            $scope.currentKeys.push(keys[i]);
                        }
                    });
            }
        }
    }

    // keep the current keys in sync with grid row selections
    var changeSelections = function (selectedKeys, deselectedKeys) {
        var addedKeys = new Array();
        var removedKeys = new Array();
        var found;

        // find the newly selected keys
        if (selectedKeys && $scope.currentKeys) {
            for (var i = 0; i < selectedKeys.length; i++) {
                found = false;

                var index = $scope.gridInstance.getRowIndexByKey(selectedKeys[i]);
                if (index > -1) {
                    for (var j = 0; j < $scope.currentKeys.length; j++) {
                        if (selectedKeys[i] === $scope.currentKeys[j]) {
                            found = true;
                            break;
                        }
                    }
                }

                if (!found) {
                    addedKeys.push(selectedKeys[i]);
                }
            }
        }

        if (deselectedKeys && $scope.currentKeys) {
            // find the unselected keys
            for (var i = 0; i < $scope.currentKeys.length; i++) {
                found = false;

                for (var j = 0; j < deselectedKeys.length; j++) {
                    var index = $scope.gridInstance.getRowIndexByKey(deselectedKeys[j]);
                    if (index > -1) {
                        if ($scope.currentKeys[i] === deselectedKeys[j]) {
                            found = true;
                            break;
                        }
                    }
                }

                if (found) {
                    removedKeys.push($scope.currentKeys[i]);
                }
            }
        }

        for (var i = 0; i < addedKeys.length; i++) {
            $scope.currentKeys.push(addedKeys[i]);
        }

        for (var i = 0; i < removedKeys.length; i++) {
            var index = $scope.currentKeys.indexOf(removedKeys[i]);
            if (index > -1)
                $scope.currentKeys.splice(index, 1);
        }

        //console.log("after current keys = " + $scope.currentKeys);
    }

    $scope.saveSelection = function () {
        var addedKeys = new Array();
        var removedKeys = new Array();
        var found;

        // find the added selections
        for (var i = 0; i < $scope.currentKeys.length; i++) {
            found = false;

            for (var j = 0; j < $scope.existingKeys.length; j++) {
                if ($scope.currentKeys[i] === $scope.existingKeys[j]) {
                    found = true;
                    break;
                }
            }

            if (!found) {
                addedKeys.push($scope.currentKeys[i]);
            }
        }

        // find the removed selections
        for (var i = 0; i < $scope.existingKeys.length; i++) {
            found = false;

            for (var j = 0; j < $scope.currentKeys.length; j++) {
                if ($scope.existingKeys[i] === $scope.currentKeys[j]) {
                    found = true;
                    break;
                }
            }

            if (!found) {
                removedKeys.push($scope.existingKeys[i]);
            }
        }

        if (addedKeys.length > 0) {
            $scope.loading = true;

            // add relationhsips to the db
            asyncLoop({
                length: addedKeys.length,
                functionToLoop: function (loop, i) {
                    if ($scope.masterid) {
                        $http.post(APP_CONFIG.ebaasRootUrl + "/api/relationship/" + encodeURIComponent($stateParams.schema) + "/" + $scope.masterclass + "/" + $scope.masterid + "/" + $scope.dbclass + "/" + encodeURIComponent(addedKeys[i]))
                             .success(function (data) {
                                 loop();
                             });
                    }
                },
                callback: function () {
                    $scope.loading = false;
                    $scope.isUpdated = true;
                }
            })
        }

        if (removedKeys.length > 0) {
            $scope.loading = true;
            // delete relationhsips from the db
            asyncLoop({
                length: removedKeys.length,
                functionToLoop: function (loop, i) {
                    $http.delete(APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($stateParams.schema) + "/" + $scope.masterclass + "/" + $scope.masterid + "/" + $scope.dbclass + "/" + encodeURIComponent(removedKeys[i]))
                         .success(function (data) {
                             loop();
                         });
                },
                callback: function () {
                    $scope.existingKeys = [];
                    for (var i = 0; i < $scope.currentKeys.length; i++)
                        $scope.existingKeys.push($scope.currentKeys[i]);
                    $scope.loading = false;
                    $scope.isUpdated = true;
                }
            })
        }

    };

    $scope.goBack = function () {
        if ($scope.isUpdated) {
            $modalInstance.close({ "modal": "viewManyToMany"});
        }
        else {
            $modalInstance.dismiss("dismiss");
        }
    };
});
