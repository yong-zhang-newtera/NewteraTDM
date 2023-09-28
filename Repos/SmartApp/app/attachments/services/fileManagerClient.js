'use strict';

angular.module('app.attachments').factory('fileManagerClient', function ($resource, APP_CONFIG) {

    return $resource(APP_CONFIG.ebaasRootUrl + "/:api/:schema/:cls/:oid?prefix=:prefix",
        { id: "@Id" },
        {
            'query': { method: 'GET', params: {api: "api", schema: "schema", cls: "cls", oid: "oid", prefix: "prefix"} },
            'save': { method: 'POST', params: { api: "api", schema: "schema", cls: "cls", oid: "oid", prefix: 'prefix' }, transformRequest: angular.identity, headers: { 'Content-Type': undefined } },
            'remove': { method: 'DELETE', url: APP_CONFIG.ebaasRootUrl + '/:api/:schema/:cls/:oid/:fileId?prefix=:prefix', params: {api: "api", schema: "schema", cls: "cls", oid: "oid", fileId: "fileId", prefix: "prefix"} }
        });
});