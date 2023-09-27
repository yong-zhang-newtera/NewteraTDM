'use strict';

angular.module('app.blobmanager').controller('blobManagerCtrl', function ($scope, $rootScope, blobManager, APP_CONFIG, $stateParams) {

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
        $scope.processDropzone();
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
});
