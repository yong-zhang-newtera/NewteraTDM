'use strict';

angular.module('app.blobmanager').controller('blobManagerCtrl', function ($scope, $rootScope, blobManager, APP_CONFIG, $stateParams, User) {

    /* jshint validthis:true */
    var vm = this;
    vm.title = 'Blob Manager';
    vm.files = blobManager.files;
    vm.uploading = false;
    vm.previewFile;
    vm.remove = blobManager.remove;
    vm.download = blobManager.download;
    vm.setPreviewFile = setPreviewFile;
    vm.getWord = getWord;
    vm.prefix = this.prefix;
    vm.bucketName = blobManager.bucketName;

    $scope.showUpload = false;
   
    if (!$stateParams.readonly)
    {
        vm.readonly = false;
    }
    else
    {
        vm.readonly = $stateParams.readonly;
    }

    blobManager.params.schema = this.dbschema;
    if (!blobManager.params.schema)
    {
        blobManager.params.schema = $stateParams.schema;
    }
   
    blobManager.params.cls = this.dbclass;
    if (!blobManager.params.cls) {
        blobManager.params.cls = $stateParams.class;
    }

    blobManager.params.oid = this.oid;
    if (!blobManager.params.oid) {
        blobManager.params.oid = $stateParams.oid;
    }

    blobManager.params.prefix = this.prefix;
    if (!blobManager.params.prefix) {
        blobManager.params.prefix = $stateParams.prefix;
    }

    $scope.baseUrl = APP_CONFIG.ebaasRootUrl;
    if (APP_CONFIG.hashedBaseUrls[$stateParams.cmdHash]) {
        $scope.baseUrl = APP_CONFIG.hashedBaseUrls[$stateParams.cmdHash];
    }

    blobManager.params.api = "api/blob"; // Indicating the filemanager is for blob

    blobManager.params.serviceBase = $scope.baseUrl;

    activate();

    function activate() {
        blobManager.load();
        blobManager.getBucketInfo();
    }

    function setPreviewFile(file) {
        vm.previewFile = file
    }

    function remove(file) {
        blobManager.remove(file).then(function () {
            setPreviewFile();
        });
    }

    function getWord(key)
    {
        return $rootScope.getWord(key);
    }

    $scope.uploadFile = function () {
        $scope.processDropzone(blobManager);
    }

    $scope.reset = function () {
        $scope.resetDropzone();
    }

    $scope.$on('directory.changedNode', function (event, args) {
        var path = getPath(args.newNode);
        path = encodeURIComponent(path);
       
        blobManager.params.prefix = path;
        blobManager.load();
    });

    $scope.setShowUpload = function (status) {
        $scope.showUpload = status;
    }

    function getPath(node) {
        var path = "";

        if (node.parent)
        {
            var parentPath = getPath(node.parent);
            if (parentPath)
            {
                path = parentPath + "\\" + node.name;
            }
            else
            {
                path = node.name;
            }
        }

        return path;
    }

    $scope.dropzoneConfig = {
        'options': {
            url: APP_CONFIG.ebaasRootUrl + "/" + blobManager.params.api + "/" + encodeURIComponent(blobManager.params.schema) + "/" + blobManager.params.cls + "/" + blobManager.params.oid + "?prefix=" + encodeURIComponent(blobManager.params.prefix) + "&user=" + encodeURIComponent(User.userName),
            maxFilesize: 100,
            maxFiles: 20,
            maxThumbnailFilesize: 10,
            previewTemplate: '<div class="dz-preview dz-file-preview"><div><div class="dz-filename"><span data-dz-name></span></span></div><span class="fa fa-lg fa-file-text-o"></span></div><div><span class="dz-size" data-dz-size></div><div><span class="dz-upload" data-dz-uploadprogress></span></div><div class="dz-success-mark"><span class="fa fa-check"></span></div><div class="dz-error-mark"><span class="fa fa-exclamation-triangle"></span></div><div class="dz-error-message"><span data-dz-errormessage></span></div></div>',
            addRemoveLinks: false,
            paramName: "uploadFile",
            parallelUploads: 20,
            autoProcessQueue: false,
            dictDefaultMessage: '<span class="text-center"><span class="font-md visible-lg-block"><span class="font-md"><i class="fa fa-caret-right text-danger"></i><span class="font-xs">' + $rootScope.getWord("DropZone") + '</span></span>',
            dictResponseError: $rootScope.getWord("UploadError"),
            dictCancelUpload: "Cancel Upload",
            dictRemoveFile: "Remove File",
        },
        'eventHandlers': {
            'addedFile': function (file) {
                scope.file = file;
                if (this.files[1] != null) {
                    this.removeFile(this.files[0]);
                }
                scope.$apply(function () {
                    scope.fileAdded = true;
                });
            },

            'success': function (file, response) {
            },

            'removedFile': function (file) {
                console.debug("Removed file called");
            },

            'queuecomplete': function () {
                blobManager.load();

                setTimeout(function () {
                    //scope.resetDropzone();
                }, 2000);
            }
        },
    };
});
