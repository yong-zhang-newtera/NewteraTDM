/// <reference path='./DlhSoft.Kanban.Angular.Components.ts'/>
var KanbanBoard = DlhSoft.Controls.KanbanBoard;


var nextIteration = { groups: [], items: [] };
angular.module('app.taskkanban').controller('KanbanMainCtrl', function ($http, APP_CONFIG, $scope, $stateParams, $state, kanbanModel, propmisedParams, hubService) {
    // Bind data to the user interface.
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.class;

    var params = propmisedParams.data;
    $scope.itemClass = params['itemClass'];
    $scope.packetClass = params['packetClass'];
    $scope.taskNodeAttribute = params['taskNodeAttribute'];
    $scope.itemNodeAttribute = params['itemNodeAttribute'];
    $scope.packetNodeAttribute = params['packetNodeAttribute'];
    $scope.taskTemplate = params['taskTemplate'];
    $scope.itemTemplate = params['itemTemplate'];
    $scope.packetTemplate = params['packetTemplate'];

    $scope.stateAttribute = params['stateAttribute'];
    $scope.stateMapping = params['stateMapping'];

    $scope.groups = kanbanModel.data.groups;
    $scope.states = kanbanModel.data.states;
    $scope.items = kanbanModel.data.items;

    //console.debug("kanban model = " + JSON.stringify(kanbanModel.data));

    $scope.reload = function(pageIndex)
    {
        var params = new Object();
       
        $state.go($state.current, params, { reload: true }); //second parameter is for $stateParams
    }

    $scope.trackStatusChanged = function (group)
    {
        var groupName = $scope.dbschema + "-" + $scope.dbclass + "-" + group.objId;
        hubService.removeFromGroup(groupName, function () {
            // refresh
            $scope.reload($scope.pageIndex);
        }); // hubService removes the current user from the group
    }

    $scope.gotoItemDetail = function (itemName)
    {
        for (var i = 0; i < $scope.items.length; i++) {
            var item = $scope.items[i];
            if (item.name == itemName) {
                var params = new Object();
                params.schema = $scope.dbschema;
                params.class = $scope.dbclass;
                params.oid = item.objId;
                params.itemClass = $scope.itemClass;
                params.packetClass = $scope.packetClass;
                params.taskNodeAttribute = $scope.taskNodeAttribute;
                params.itemNodeAttribute = $scope.itemNodeAttribute;
                params.packetNodeAttribute = $scope.packetNodeAttribute;
                params.taskTemplate = $scope.taskTemplate;
                params.itemTemplate = $scope.itemTemplate;
                params.packetTemplate = $scope.packetTemplate;
                params.activeTabId = "tasktab";
                $state.go("app.taskviewer.details", params, { reload: true });
            }
        }
    }

    // Handle changes.
    $scope.onItemStateChanged = function (item, boardState) {
        var url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + encodeURIComponent($scope.dbclass) + "/" + item.objId + "?formformat=false";

        var model = new Object();
        var actualState = GetActualState(boardState.name);
        if (!actualState)
            return;

        model[$scope.stateAttribute] = actualState;

        $scope.loading = true;
        $http.post(url, model)
            .success(function (data) {
                $scope.loading = false;
            })
            .error(function (err) {
                console.debug("error=" + JSON.stringify(err));
                $scope.loading = false;
            });
    };
    $scope.onItemGroupChanged = function (item, group) {
        //console.log('Group of ' + item.name + ' was changed to: ' + group.name);
    };
    // Move items to the next iteration.
    $scope.nextIteration = nextIteration;
    $scope.moveItemToNextIteration = function (type, index) {
        if (type === DlhSoft.Controls.KanbanBoard.types.group) {
            // Move an entire group (story) and all its items.
            var group = groups[index];
            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                if (item.group === group) {
                    items.splice(i--, 1);
                    nextIteration.items.push(item);
                }
            }
            groups.splice(index, 1);
            if (nextIteration.groups.indexOf(group) < 0)
                nextIteration.groups.push(group);
            console.log('Group ' + group.name + ' and its items were moved to next iteration.');
        }
        else {
            // Move a single item, and copy the group (story) if needed.
            var item = items[index];
            items.splice(index, 1);
            nextIteration.items.push(item);
            var group = item.group;
            if (nextIteration.groups.indexOf(group) < 0)
                nextIteration.groups.push(group);
            console.log('Item ' + item.name + ' was moved to next iteration.');
        }
    };

    function GetActualState(displayState) {
        var states = $scope.stateMapping.split(";");
        var actualState = undefined;

        for (var i = 0; i < states.length; ++i) {
            var state = states[i];
            var keyValue = state.split(":");
            var key = keyValue[0];
            var value = keyValue[1];
            if (key === displayState) {
                var actualStates = value.split(",");
                if (actualStates.length > 0) {
                    // use the first state as default
                    actualState = actualStates[0];
                    break;
                }
            }
        }

        return actualState;
    }
});
//# sourceMappingURL=app.js.map