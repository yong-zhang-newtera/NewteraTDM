"use strict";

angular.module('app.fulltextsearch').factory('searchService', function ($http, APP_CONFIG) {

    function getResultCounts(dbschema, searchtext, callback) {
      
        var url = APP_CONFIG.ebaasRootUrl + "/api/search/" + encodeURIComponent(dbschema) + "/counts?searchtext=" + encodeURIComponent(searchtext);

	    $http.get(url).success(function (data) {
	        callback(data);
				
		}).error(function(){
		    callback([]);
		});
    }
	
	return {
	    getSearchResultCounts: function (dbschema, searchtext, callback) {
	        getResultCounts(dbschema, searchtext, callback);
	    }
	}
});