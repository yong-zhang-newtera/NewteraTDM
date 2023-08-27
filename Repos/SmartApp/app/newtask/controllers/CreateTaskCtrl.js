'use strict';

angular.module('app.newtask').controller('CreateTaskCtrl', function ($controller, $http, $scope, APP_CONFIG, $state, $rootScope, $stateParams, propmisedParams) {

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

    $scope.template = params['taskTemplate'];

    $scope.formId = "CreateNewTaskForm"; // this will be posted as headers to the server to identify the form(optional)

    $scope.submitFormCallback = function (data) {
        if (data) {
            var params = new Object();
            params.schema = $scope.dbschema;
            params.class = $scope.dbclass;
            params.oid = data.instance.obj_id;
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
    };

    $scope.onTaskSubmit = function () {
        $scope.submitForm($scope.submitFormCallback);
    };

    angular.extend(this, $controller('ebaasFormBaseCtrl', { $rootScope: $rootScope, $scope: $scope, $http: $http, APP_CONFIG: APP_CONFIG, $stateParams: $stateParams }));

});