'use strict';

angular.module('app.smartforms').controller('relatedFormModalCtrl', function ($controller, $rootScope, $scope, $http, APP_CONFIG, $stateParams, $modalInstance, $state, TasksInfo) {
 
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.rclass;
    $scope.oid = $stateParams.roid;
    $scope.template = $stateParams.rtemplate;
    $scope.formAttribute = $stateParams.rformAttribute;
    $scope.masterClass = $stateParams.class;
    $scope.readonly = false;

    if ($stateParams.oid)
        $scope.masterOid = $stateParams.oid;
    else if (TasksInfo.currentTask) {
        // when the related form is launched from a WF task form, oid is missing from state params
        // it can be obtained from the current task info
        $scope.masterOid = TasksInfo.currentTask.bindingInstanceId;
        $scope.masterClass = TasksInfo.currentTask.bindingClassName;
    }
            
    if ($stateParams.readonly && $stateParams.readonly === "true")
    {
        $scope.readonly = true;
    }

    angular.extend(this, $controller('ebaasFormBaseCtrl', { $rootScope: $rootScope, $scope: $scope, $http: $http, APP_CONFIG: APP_CONFIG}));

    $scope.closeModal = function () {
        $rootScope.$broadcast('relatedModalFormClosed', { masterOid: $scope.masterOid });
        if ($scope.submitted)
            $modalInstance.close("update");
        else
            $modalInstance.dismiss("dismiss");
    };

    $scope.submitRelatedForm = function()
    {
        $scope.submitForm(function (result) {
            if ($scope.masterOid) {
                // link the related instance to the master instance
                $http.post(APP_CONFIG.ebaasRootUrl + "/api/relationship/" + encodeURIComponent($scope.dbschema) + "/" + $scope.masterClass + "/" + $scope.masterOid + "/" + $scope.dbclass + "/" + result.instance.obj_id)
                  .success(function (data) {
                      
                  });
            }
        });
    }
});
