'use strict';

angular.module('app.attachments').directive('dropzone', function ($rootScope, APP_CONFIG, User) {
    return {
        restrict: 'C',
        link: function (scope, element, attributes) {

            var config = scope.dropzoneConfig;

            var dropzone = new Dropzone(element[0], config.options);

            angular.forEach(config.eventHandlers, function (handler, event) {
                dropzone.on(event, handler);
            });

            scope.processDropzone = function (fileManager) {

                var url = undefined;
                if (fileManager.params.oid)
                {
                    url = APP_CONFIG.ebaasRootUrl + "/" + fileManager.params.api + "/" + encodeURIComponent(fileManager.params.schema) + "/" + fileManager.params.cls + "/" + fileManager.params.oid + "?prefix=" + encodeURIComponent(fileManager.params.prefix) + "&user=" + encodeURIComponent(User.userName);
                }

                dropzone.options.url = url;
                dropzone.processQueue();
            };

            scope.resetDropzone = function () {
                dropzone.removeAllFiles();
            }
        }
    }
});
