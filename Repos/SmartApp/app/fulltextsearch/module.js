"use strict";

angular.module("app.fulltextsearch", ["ngResource", "ui.router", "ui.bootstrap", "ui.bootstrap.modal", 'ui.select']);

angular.module("app.fulltextsearch")
    .config(function ($stateProvider, modalStateProvider) {

        $stateProvider
            .state('app.fulltextsearch', {
                abstract: true,
                data: {
                    title: 'Full Text Search'
                }
            })
            .state('app.fulltextsearch.result', {
                url: '/fulltext/searchresult',
                data: {
                    title: 'Search Result'
                },
                views: {
                    "content@app": {
                        templateUrl: "app/fulltextsearch/views/search-result.html",
                        controller: "searchResultCtrl"
                    }
                },
                authenticate: true
            })
    });