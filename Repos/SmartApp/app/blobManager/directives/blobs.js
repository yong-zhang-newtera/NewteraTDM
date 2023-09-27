'use strict';

angular.module('app.blobmanager').directive('blobs', function () {
    return {
        restrict: 'E',
        templateUrl: 'app/blobmanager/views/blob-manager.html',
        replace: true,
        scope: {},
        bindToController: {
            dbschema: '=',
            dbclass: '=',
            oid: '=',
            prefix: '='
        },
        controllerAs: 'vm',
        controller: 'blobManagerCtrl',
        link: function (scope, element, attributes) {
        }
    }
});