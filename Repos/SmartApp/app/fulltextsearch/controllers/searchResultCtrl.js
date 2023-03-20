"use strict";


angular.module('app.fulltextsearch').controller('searchResultCtrl', function ($scope, $http, $state, $stateParams, APP_CONFIG, searchContext, searchService) {

    $scope.searchCounts = [];
    $scope.loading = true;
    searchService.getSearchResultCounts(APP_CONFIG.dbschema, searchContext.searchText, function (counts) {

        $scope.searchCounts = counts;
        $scope.loading = false;

        if (counts.length == 1) {
            // show the matched items if there is only one class contains them
            $state.go('app.smarttables.datagrid', { schema: APP_CONFIG.dbschema, class: counts[0].className, search: 'fulltext' });
        }
    });

    $scope.showClassData = function (className) {
        $state.go('app.smarttables.datagrid', { schema: APP_CONFIG.dbschema, class: className, search: 'fulltext' });
    }
});