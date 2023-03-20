"use strict";	

angular.module('app.homepage').controller("myActivitiesCtrl", function ActivitiesCtrl($scope, $log, $state, APP_CONFIG, User, myActivityService) {

	$scope.activeTab = 'default';

	// Getting different type of activites
	myActivityService.get(function(data){
		$scope.activities = data.activities;
	});

	$scope.isActive = function (tab) {
		return $scope.activeTab === tab;
	};

	$scope.setTab = function (activityType) {
		$scope.activeTab = activityType;

		myActivityService.getbytype(activityType, function(data) {
		    myActivityService.MessageModel.items = data;
		});

	};

	$scope.setTab("msgs");

	$scope.getPosterImage = function(posterId)
	{
	    return User.getUserImage(posterId);
	}

	$scope.getMsgItems = function()
	{
	    return myActivityService.MessageModel.items;
	}

	$scope.hasMsgs = function()
	{
	    if (myActivityService.MessageModel.items &&
            myActivityService.MessageModel.items.length > 0)
	    {
	        return true;
	    }
	    else
	    {
	        return false;
	    }
	}

	$scope.readMsg = function(msg)
	{
	    var url = msg.url;
	    var urlparams = msg.urlparams;
        
	    urlparams = urlparams.replace(/msg.dbschema/, "\"" + msg.dbschema + "\""); // replace msg.dbschema
	    urlparams = urlparams.replace(/msg.dbclass/, "\"" + msg.dbclass + "\""); // replace msg.dbclass
	    urlparams = urlparams.replace(/msg.oid/, "\"" + msg.oid + "\""); // replace msg.dbclass

	    var params = JSON.parse(urlparams);

	    if (url) {
	        $state.go(url, params);
	    }
	}

	$scope.deleteMsg = function (msg) {
	    var found = false;
	    var index = undefined;

	    for (var i = 0; i < myActivityService.MessageModel.items.length; i++) {
	        var activity = myActivityService.MessageModel.items[i];
	        if (activity.objId === msg.objId) {
	            index = i;
	            found = true;
	            break;
	        }
	    }

	    if (found) {
	        myActivityService.MessageModel.items.splice(index, 1);
	    }

	    myActivityService.remove("msgs", msg.objId, function (data) {
	        myActivityService.MessageModel.count = myActivityService.MessageModel.items.length;
	    });
	}

	$scope.ClearUserMessages = function()
	{
	    myActivityService.removeAll("msgs", function () {
	        myActivityService.MessageModel.items = [];
	        myActivityService.MessageModel.count = 0;
	    });
	}
});