'use strict';



var appConfig = window.appConfig || {};

appConfig.menu_speed = 200;

appConfig.smartSkin = "smart-style-0";

appConfig.skins = [
    {name: "smart-style-0",
        logo: "styles/img/logo.png",
        class: "btn btn-block btn-xs txt-color-white margin-right-5",
        style: "background-color:#4E463F;",
        label: "Smart Default"},

    {name: "smart-style-1",
        logo: "styles/img/logo.png",
        class: "btn btn-block btn-xs txt-color-white",
        style: "background:#3A4558;",
        label: "Dark Elegance"},

    {name: "smart-style-2",
        logo: "styles/img/logo.png",
        class: "btn btn-xs btn-block txt-color-darken margin-top-5",
        style: "background:#fff;",
        label: "Ultra Light"},

    {name: "smart-style-3",
        logo: "styles/img/logo.png",
        class: "btn btn-xs btn-block txt-color-white margin-top-5",
        style: "background:#f78c40",
        label: "Google Skin"},

    {name: "smart-style-4",
    logo: "styles/img/logo.png",
        class: "btn btn-xs btn-block txt-color-white margin-top-5",
        style: "background: #bbc0cf; border: 1px solid #59779E; color: #17273D !important;",
        label: "PixelSmash"},

    {name: "smart-style-5",
        logo: "styles/img/logo.png",
        class: "btn btn-xs btn-block txt-color-white margin-top-5",
        style: "background: rgba(153, 179, 204, 0.2); border: 1px solid rgba(121, 161, 221, 0.8); color: #17273D !important;",
        label: "Glass"}
];



appConfig.sound_path = "sound/";
appConfig.sound_on = true; 


/*
* DEBUGGING MODE
* debugState = true; will spit all debuging message inside browser console.
* The colors are best displayed in chrome browser.
*/


appConfig.debugState = false;	
appConfig.debugStyle = 'font-weight: bold; color: #00f;';
appConfig.debugStyle_green = 'font-weight: bold; font-style:italic; color: #46C246;';
appConfig.debugStyle_red = 'font-weight: bold; color: #ed1c24;';
appConfig.debugStyle_warning = 'background-color:yellow';
appConfig.debugStyle_success = 'background-color:green; font-weight:bold; color:#fff;';
appConfig.debugStyle_error = 'background-color:#ed1c24; font-weight:bold; color:#fff;';


appConfig.voice_command = true;
appConfig.voice_command_auto = false;

/*
 *  Sets the language to the default 'en-US'. (supports over 50 languages 
 *  by google)
 * 
 *  Afrikaans         ['af-ZA']
 *  Bahasa Indonesia  ['id-ID']
 *  Bahasa Melayu     ['ms-MY']
 *  CatalГ            ['ca-ES']
 *  ДЊeЕЎtina         ['cs-CZ']
 *  Deutsch           ['de-DE']
 *  English           ['en-AU', 'Australia']
 *                    ['en-CA', 'Canada']
 *                    ['en-IN', 'India']
 *                    ['en-NZ', 'New Zealand']
 *                    ['en-ZA', 'South Africa']
 *                    ['en-GB', 'United Kingdom']
 *                    ['en-US', 'United States']
 *  EspaГ±ol          ['es-AR', 'Argentina']
 *                    ['es-BO', 'Bolivia']
 *                    ['es-CL', 'Chile']
 *                    ['es-CO', 'Colombia']
 *                    ['es-CR', 'Costa Rica']
 *                    ['es-EC', 'Ecuador']
 *                    ['es-SV', 'El Salvador']
 *                    ['es-ES', 'EspaГ±a']
 *                    ['es-US', 'Estados Unidos']
 *                    ['es-GT', 'Guatemala']
 *                    ['es-HN', 'Honduras']
 *                    ['es-MX', 'MГ©xico']
 *                    ['es-NI', 'Nicaragua']
 *                    ['es-PA', 'PanamГЎ']
 *                    ['es-PY', 'Paraguay']
 *                    ['es-PE', 'PerГє']
 *                    ['es-PR', 'Puerto Rico']
 *                    ['es-DO', 'RepГєblica Dominicana']
 *                    ['es-UY', 'Uruguay']
 *                    ['es-VE', 'Venezuela']
 *  Euskara           ['eu-ES']
 *  FranГ§ais         ['fr-FR']
 *  Galego            ['gl-ES']
 *  Hrvatski          ['hr_HR']
 *  IsiZulu           ['zu-ZA']
 *  ГЌslenska         ['is-IS']
 *  Italiano          ['it-IT', 'Italia']
 *                    ['it-CH', 'Svizzera']
 *  Magyar            ['hu-HU']
 *  Nederlands        ['nl-NL']
 *  Norsk bokmГҐl     ['nb-NO']
 *  Polski            ['pl-PL']
 *  PortuguГЄs        ['pt-BR', 'Brasil']
 *                    ['pt-PT', 'Portugal']
 *  RomГўnДѓ          ['ro-RO']
 *  SlovenДЌina       ['sk-SK']
 *  Suomi             ['fi-FI']
 *  Svenska           ['sv-SE']
 *  TГјrkГ§e          ['tr-TR']
 *  Р±СЉР»РіР°СЂСЃРєРё['bg-BG']
 *  PСѓСЃСЃРєРёР№     ['ru-RU']
 *  РЎСЂРїСЃРєРё      ['sr-RS']
 *  н•њкµ­м–ґ         ['ko-KR']
 *  дё­ж–‡            ['cmn-Hans-CN', 'ж™®йЂљиЇќ (дё­е›Ѕе¤§й™†)']
 *                    ['cmn-Hans-HK', 'ж™®йЂљиЇќ (й¦™жёЇ)']
 *                    ['cmn-Hant-TW', 'дё­ж–‡ (еЏ°зЃЈ)']
 *                    ['yue-Hant-HK', 'зІµиЄћ (й¦™жёЇ)']
 *  ж—Ґжњ¬иЄћ         ['ja-JP']
 *  Lingua latД«na    ['la']
 */
appConfig.voice_command_lang = 'en-US';
/*
 *  Use localstorage to remember on/off (best used with HTML Version)
 */ 
appConfig.voice_localStorage = false;
/*
 * Voice Commands
 * Defines all voice command variables and functions
 */ 
if (appConfig.voice_command) {
        
     	appConfig.commands = {
                
        'show dashboard' : function() { window.location.hash = "dashboard" },
        'show inbox' : function() {  window.location.hash = "inbox/" },
        'show graphs' : function() {  window.location.hash = "graphs/flot" },
        'show flotchart' : function() { window.location.hash = "graphs/flot" },
        'show morris chart' : function() { window.location.hash = "graphs/morris" },
        'show inline chart' : function() { window.location.hash = "graphs/inline-charts" },
        'show dygraphs' : function() { window.location.hash = "graphs/dygraphs" },
        'show tables' : function() { window.location.hash = "tables/table" },
        'show data table' : function() { window.location.hash = "tables/datatable" },
        'show jquery grid' : function() { window.location.hash = "tables/jqgrid" },
        'show form' : function() { window.location.hash = "forms/form-elements" },
        'show form layouts' : function() { window.location.hash = "forms/form-templates" },
        'show form validation' : function() { window.location.hash = "forms/validation" },
        'show form elements' : function() { window.location.hash = "forms/bootstrap-forms" },
        'show form plugins' : function() { window.location.hash = "forms/plugins" },
        'show form wizards' : function() { window.location.hash = "forms/wizards" },
        'show bootstrap editor' : function() { window.location.hash = "forms/other-editors" },
        'show dropzone' : function() { window.location.hash = "forms/dropzone" },
        'show image cropping' : function() { window.location.hash = "forms/image-editor" },
        'show general elements' : function() { window.location.hash = "ui/general-elements" },
        'show buttons' : function() { window.location.hash = "ui/buttons" },
        'show fontawesome' : function() { window.location.hash = "ui/icons/fa" },
        'show glyph icons' : function() { window.location.hash = "ui/icons/glyph" },
        'show flags' : function() { window.location.hash = "ui/icons/flags" },
        'show grid' : function() { window.location.hash = "ui/grid" },
        'show tree view' : function() { window.location.hash = "ui/treeview" },
        'show nestable lists' : function() { window.location.hash = "ui/nestable-list" },
        'show jquery U I' : function() { window.location.hash = "ui/jqui" },
        'show typography' : function() { window.location.hash = "ui/typography" },
        'show calendar' : function() { window.location.hash = "calendar" },
        'show widgets' : function() { window.location.hash = "widgets" },
        'show gallery' : function() { window.location.hash = "gallery" },
        'show maps' : function() { window.location.hash = "gmap-xml" },
        'go back' :  function() { history.back(1); }, 
        'scroll up' : function () { $('html, body').animate({ scrollTop: 0 }, 100); },
        'scroll down' : function () { $('html, body').animate({ scrollTop: $(document).height() }, 100);},
        'hide navigation' : function() { 
            if ($( ":root" ).hasClass("container") && !$( ":root" ).hasClass("menu-on-top")){
                $('span.minifyme').trigger("click");
            } else {
                $('#hide-menu > span > a').trigger("click"); 
            }
        },
        'show navigation' : function() { 
            if ($( ":root" ).hasClass("container") && !$( ":root" ).hasClass("menu-on-top")){
                $('span.minifyme').trigger("click");
            } else {
                $('#hide-menu > span > a').trigger("click"); 
            }
        },
        'mute' : function() {
            appConfig.sound_on = false;
            $.smallBox({
                title : "MUTE",
                content : "All sounds have been muted!",
                color : "#a90329",
                timeout: 4000,
                icon : "fa fa-volume-off"
            });
        },
        'sound on' : function() {
            appConfig.sound_on = true;
            $.speechApp.playConfirmation();
            $.smallBox({
                title : "UNMUTE",
                content : "All sounds have been turned on!",
                color : "#40ac2b",
                sound_file: 'voice_alert',
                timeout: 5000,
                icon : "fa fa-volume-up"
            });
        },
        'stop' : function() {
            smartSpeechRecognition.abort();
            $( ":root" ).removeClass("voice-command-active");
            $.smallBox({
                title : "VOICE COMMAND OFF",
                content : "Your voice commands has been successfully turned off. Click on the <i class='fa fa-microphone fa-lg fa-fw'></i> icon to turn it back on.",
                color : "#40ac2b",
                sound_file: 'voice_off',
                timeout: 8000,
                icon : "fa fa-microphone-slash"
            });
            if ($('#speech-btn .popover').is(':visible')) {
                $('#speech-btn .popover').fadeOut(250);
            }
        },
        'help' : function() {

            $('#voiceModal').removeData('modal').modal( { remote: "app/layout/partials/voice-commands.tpl.html", show: true } );
            if ($('#speech-btn .popover').is(':visible')) {
                $('#speech-btn .popover').fadeOut(250);
            }

        },      
        'got it' : function() {
            $('#voiceModal').modal('hide');
        },  
        'logout' : function() {
            $.speechApp.stop();
            window.location = $('#logout > span > a').attr("href");
        }
    };
}

appConfig.apiRootUrl = './api';
appConfig.avatarsUrl = 'styles/custom/avatars/'
appConfig.ebaasRootUrl = '.'; // api base url
appConfig.hashedBaseUrls = {}; // hash codes to server urls mapping

window.appConfig = appConfig;

/*
* END APP.appConfig
*/
'use strict';

$.sound_path = appConfig.sound_path;
$.sound_on = appConfig.sound_on;

'use strict';

/**
 * @ngdoc overview
 * @name app [smartadminApp]
 * @description
 * # app [smartadminApp]
 *
 * Main module of the application.
 */

angular.module('app', [
    //'ngSanitize',
    'ngAnimate',
    'restangular',
    'ui.router',
    'ui.bootstrap',
    'angular-intro',

    'summernote',

    // Smartadmin Angular Common Module
    'SmartAdmin',

    // angular-local-storage
    'LocalStorageModule',

    // App
    'app.auth',
    'app.layout',
    'app.dashboard',
    'app.graphs',
    'app.tables',
    'app.forms',
    'app.smarttables',
    'app.taskviewer',
    'app.smartforms',
    'app.ui',
    'app.wizards',
    'app.tasks',
    'app.stations',
    'app.homepage',
    'app.user',
    'app.attachments',
    'app.taskforum',
    'app.taskkanban',
    'app.datacatalog',
    'app.dataviewer',
    "app.galleryview",
    "app.forum",
    "app.blog",
    "app.userdirectory",
    "app.myspace",
    "app.tasktrack",
    "app.filemanager",
    'app.formeditor',
    "app.smartreports",
    "app.bulletinboard",
    "app.hub",
    "app.dataImporter",
    "app.logs",
    "app.mldashboard",
    "app.fulltextsearch"
])
.config(function ($provide, $httpProvider) {


    // Intercept http calls.
    $provide.factory('ErrorHttpInterceptor', function ($q) {
        var errorCounter = 0;

        function notifyError(rejection) {

            if (rejection.status === 401 ||
                rejection.status === 500) {
                console.log(rejection);
            }
            else if (rejection.status === 400)
            {
                if (rejection.data.error &&
                    rejection.data.error === "invalid_grant") {
                    return;
                }
                else {
                    //console.log(rejection);
                    $.bigBox({
                        title: 'The request encounters an error',
                        content: rejection.data.message,
                        color: "#C46A69",
                        icon: "fa fa-warning shake animated",
                        number: ++errorCounter,
                        timeout: 6000
                    });
                }
            }
            else if (rejection.status === 404) {
                    //console.log(rejection);
                $.bigBox({
                    title: 'The request encounters an error',
                    content: "API not found",
                    color: "#C46A69",
                    icon: "fa fa-warning shake animated",
                    number: ++errorCounter,
                    timeout: 6000
                });
            }
            else {
                //console.log(rejection);
                var msg = "Server is down.";
                if (rejection && rejection.data)
                {
                    if (rejection.data.message)
                    {
                        msg = rejection.data.message;
                    }
                    else
                    {
                        msg = "Unknown error type, please check the server log."
                    }
                }
                $.bigBox({
                    title: 'The request encounters an error',
                    content: msg,
                    color: "#C46A69",
                    icon: "fa fa-warning shake animated",
                    number: ++errorCounter,
                    timeout: 6000
                });
            }
        }

        return {
            // On request failure
            requestError: function (rejection) {
                // show notification
                notifyError(rejection);

                // Return the promise rejection.
                return $q.reject(rejection);
            },

            // On response failure
            responseError: function (rejection) {
                // show notification
                notifyError(rejection);
                // Return the promise rejection.
                return $q.reject(rejection);
            }
        };
    });

    // Add the interceptor to the $httpProvider.
    $httpProvider.interceptors.push('ErrorHttpInterceptor');

})
.config(['$httpProvider', function ($httpProvider) {
    //initialize get if not there
    if (!$httpProvider.defaults.headers.get) {
        $httpProvider.defaults.headers.get = {};
    }

    //disable IE ajax request caching
    $httpProvider.defaults.headers.get['If-Modified-Since'] = 'Mon, 26 Jul 1997 05:00:00 GMT';
    // extra
    $httpProvider.defaults.headers.get['Cache-Control'] = 'no-cache';
    $httpProvider.defaults.headers.get['Pragma'] = 'no-cache';
}])
.config(function (localStorageServiceProvider) {
    localStorageServiceProvider
      .setPrefix('NewteraTDM')
      .setStorageType('sessionStorage')
      .setNotify(true, true)
})
.config(function ($httpProvider) {
    $httpProvider.interceptors.push('AuthInterceptorService');
})
.constant('APP_CONFIG', window.appConfig)

.run(function ($rootScope, $http, APP_CONFIG,
    $state, $stateParams, TasksInfo, myActivityService, searchContext, Language
    ) {
    $rootScope.$state = $state;
    $rootScope.$stateParams = $stateParams;

    $rootScope.getTaskCount = function () {
        return TasksInfo.count;
    }

    $rootScope.getMsgCount = function () {
        return myActivityService.MessageModel.count;
    }

    $rootScope.getTotalCount = function () {
        return TasksInfo.count + myActivityService.MessageModel.count;
    }

    $rootScope.searchContext = searchContext;

    $rootScope.suggestionText = "Hello World";

    Globalize.culture("us");

    // to ensure that pulldown menu or datepicker in the filter row can appear in the modal popup
    DevExpress.ui.dxOverlay.baseZIndex(2000);

    // get default settings from the sitemap
    $http.get(APP_CONFIG.ebaasRootUrl + "/api/sitemap/settings")
       .success(function (settings) {
           if (settings.database) {
               // remember the default schema name in the APP_CONFIG
               var pos = settings.database.indexOf(" ");
               if (pos !== -1) {
                   APP_CONFIG.dbschema = settings.database.slice(0, pos); // get db name without version number
               }
               else {
                   throw "Missing database value in the sitemap";
               }

               $rootScope.setLanguage(settings.language);

               Language.getLanguages(function (data) {

                   var langIndex = $rootScope.langIndex;

                   $rootScope.currentLanguage = data[langIndex];

                   $rootScope.languages = data;

                   Language.getLang(data[langIndex].key, function (data) {

                       $rootScope.lang = data;
                   });

               });
           }
           else {
               throw "Missing database value in the sitemap";
           }
       });

    // get information whether the full-text search is enabled
    $http.get(APP_CONFIG.ebaasRootUrl + "/api/search/enabled")
       .success(function (status) {
           $rootScope.searchEnabled = status;
       });

    // setup default language here as us
    $rootScope.lang = {};

    $rootScope.setLanguage = function(lang)
    {
        switch (lang) {
            case "us":
                $rootScope.langIndex = 0;
                break;
            case "cn":
                $rootScope.langIndex = 1;
                break;
            default:
                $rootScope.langIndex = 1;
                break;
        }
    }

    $rootScope.getWord = function (key) {
        if (angular.isDefined($rootScope.lang[key])) {
            return $rootScope.lang[key];
        }
        else {
            return key;
        }
    }

    $rootScope.isChrome = function () {
        return !!window.chrome && !!window.chrome.webstore;
    }

    $rootScope.getSuggestions = function (typedText) {
        searchContext.typedText = typedText;
        if (typedText && typedText.length > 1) {
            // get suggestions for the type text in search text box
            var url = APP_CONFIG.ebaasRootUrl + "/api/search/raw/" + encodeURIComponent($rootScope.fullText.schema) + "/" + encodeURIComponent($rootScope.fullText.class) + "?searchtext=" + encodeURIComponent(typedText) + "&size=15";

            var promise = $http.get(url).then(function (response) {
                _.forEach(response.data, function (item) {
                    item.fullText = new Object();
                    item.fullText.oid = item["obj_id"];
                    item.fullText.title = item[$rootScope.fullText.title];
                    item.fullText.ID = item[$rootScope.fullText.ID];
                    item.fullText.display = truncateText(item.fullText.title, 26) + "<br>" + item.fullText.ID;
                })

                if (response.data.length > 0) {
                    var showAllMatch = new Object();
                    showAllMatch.fullText = new Object();
                    showAllMatch.fullText.display = $rootScope.getWord("Show All Matches");
                    showAllMatch.fullText.title = "all";
                    showAllMatch.fullText.ID = typedText;
                    response.data.push(showAllMatch);
                }

                return response.data;
            });
            // Return the promise to the controller
            return promise;
        }
        else
            return [];
    }

    $rootScope.onSuggestionSelect = function ($item, $model, $label) {
        if ($item.fullText.title === "all") {
            $state.go("app.smarttables.fulltextdatagrid", { schema: $rootScope.fullText.schema, class: $rootScope.fullText.class, track: true, attachment: false, search: 'fulltext' });
        }
        else {
            var params = new Object();
            params.schema = $rootScope.fullText.schema;
            params.class = $rootScope.fullText.class;
            params.oid = $item.fullText.oid;
            params.itemClass = $rootScope.fullText.itemClass;
            params.packetClass = $rootScope.fullText.packetClass;
            params.taskNodeAttribute = $rootScope.fullText.taskNodeAttribute;
            params.itemNodeAttribute = $rootScope.fullText.itemNodeAttribute;
            params.packetNodeAttribute = $rootScope.fullText.packetNodeAttribute;
            params.taskTemplate = $rootScope.fullText.taskTemplate;
            params.itemTemplate = $rootScope.fullText.itemTemplate;
            params.packetTemplate = $rootScope.fullText.packetTemplate;
            params.activeTabId = "tasktab";
            $state.go("app.taskviewer.details", params, { reload: true });
        }
    }

    $rootScope.fullTextSearch = function () {
        $state.go("app.smarttables.fulltextdatagrid", { schema: $rootScope.fullText.schema, class: $rootScope.fullText.class, track: true, attachment: false, search: 'fulltext'});
    }

    $rootScope.formatLabel = function (model) {
        if (model && model.fullText) {
            return model.fullText.ID;
        }
        else {
            return "";
        }
    }

    // editableOptions.theme = 'bs3';

    var GoToTaskInfoView = function (dbschema, dbclass, objId) {
        console.debug("GoToTaskInfoView");
    }

    var truncateText = function(text, length) {
        if (text.length <= length) {
            return text;
        }

        return text.substr(0, length) + '\u2026'
    }
});

angular.module("app").run(["$templateCache", function($templateCache) {$templateCache.put("app/dashboard/live-feeds.tpl.html","<div jarvis-widget id=\"live-feeds-widget\" data-widget-togglebutton=\"false\" data-widget-editbutton=\"false\"\r\n     data-widget-fullscreenbutton=\"false\" data-widget-colorbutton=\"false\" data-widget-deletebutton=\"false\">\r\n<!-- widget options:\r\nusage: <div class=\"jarviswidget\" id=\"wid-id-0\" data-widget-editbutton=\"false\">\r\n\r\ndata-widget-colorbutton=\"false\"\r\ndata-widget-editbutton=\"false\"\r\ndata-widget-togglebutton=\"false\"\r\ndata-widget-deletebutton=\"false\"\r\ndata-widget-fullscreenbutton=\"false\"\r\ndata-widget-custombutton=\"false\"\r\ndata-widget-collapsed=\"true\"\r\ndata-widget-sortable=\"false\"\r\n\r\n-->\r\n<header>\r\n    <span class=\"widget-icon\"> <i class=\"glyphicon glyphicon-stats txt-color-darken\"></i> </span>\r\n\r\n    <h2>Live Feeds </h2>\r\n\r\n    <ul class=\"nav nav-tabs pull-right in\" id=\"myTab\">\r\n        <li class=\"active\">\r\n            <a data-toggle=\"tab\" href=\"#s1\"><i class=\"fa fa-clock-o\"></i> <span class=\"hidden-mobile hidden-tablet\">Live Stats</span></a>\r\n        </li>\r\n\r\n        <li>\r\n            <a data-toggle=\"tab\" href=\"#s2\"><i class=\"fa fa-facebook\"></i> <span class=\"hidden-mobile hidden-tablet\">Social Network</span></a>\r\n        </li>\r\n\r\n        <li>\r\n            <a data-toggle=\"tab\" href=\"#s3\"><i class=\"fa fa-dollar\"></i> <span class=\"hidden-mobile hidden-tablet\">Revenue</span></a>\r\n        </li>\r\n    </ul>\r\n\r\n</header>\r\n\r\n<!-- widget div-->\r\n<div class=\"no-padding\">\r\n\r\n    <div class=\"widget-body\">\r\n        <!-- content -->\r\n        <div id=\"myTabContent\" class=\"tab-content\">\r\n            <div class=\"tab-pane fade active in padding-10 no-padding-bottom\" id=\"s1\">\r\n                <div class=\"row no-space\">\r\n                    <div class=\"col-xs-12 col-sm-12 col-md-8 col-lg-8\">\r\n														<span class=\"demo-liveupdate-1\"> <span\r\n                                                                class=\"onoffswitch-title\">Live switch</span> <span\r\n                                                                class=\"onoffswitch\">\r\n																<input type=\"checkbox\" name=\"start_interval\" ng-model=\"autoUpdate\"\r\n                                                                       class=\"onoffswitch-checkbox\" id=\"start_interval\">\r\n																<label class=\"onoffswitch-label\" for=\"start_interval\">\r\n                                                                    <span class=\"onoffswitch-inner\"\r\n                                                                          data-swchon-text=\"ON\"\r\n                                                                          data-swchoff-text=\"OFF\"></span>\r\n                                                                    <span class=\"onoffswitch-switch\"></span>\r\n                                                                </label> </span> </span>\r\n\r\n                        <div id=\"updating-chart\" class=\"chart-large txt-color-blue\" flot-basic flot-data=\"liveStats\" flot-options=\"liveStatsOptions\"></div>\r\n\r\n                    </div>\r\n                    <div class=\"col-xs-12 col-sm-12 col-md-4 col-lg-4 show-stats\">\r\n\r\n                        <div class=\"row\">\r\n                            <div class=\"col-xs-6 col-sm-6 col-md-12 col-lg-12\"><span class=\"text\"> My Tasks <span\r\n                                    class=\"pull-right\">130/200</span> </span>\r\n\r\n                                <div class=\"progress\">\r\n                                    <div class=\"progress-bar bg-color-blueDark\" style=\"width: 65%;\"></div>\r\n                                </div>\r\n                            </div>\r\n                            <div class=\"col-xs-6 col-sm-6 col-md-12 col-lg-12\"><span class=\"text\"> Transfered <span\r\n                                    class=\"pull-right\">440 GB</span> </span>\r\n\r\n                                <div class=\"progress\">\r\n                                    <div class=\"progress-bar bg-color-blue\" style=\"width: 34%;\"></div>\r\n                                </div>\r\n                            </div>\r\n                            <div class=\"col-xs-6 col-sm-6 col-md-12 col-lg-12\"><span class=\"text\"> Bugs Squashed<span\r\n                                    class=\"pull-right\">77%</span> </span>\r\n\r\n                                <div class=\"progress\">\r\n                                    <div class=\"progress-bar bg-color-blue\" style=\"width: 77%;\"></div>\r\n                                </div>\r\n                            </div>\r\n                            <div class=\"col-xs-6 col-sm-6 col-md-12 col-lg-12\"><span class=\"text\"> User Testing <span\r\n                                    class=\"pull-right\">7 Days</span> </span>\r\n\r\n                                <div class=\"progress\">\r\n                                    <div class=\"progress-bar bg-color-greenLight\" style=\"width: 84%;\"></div>\r\n                                </div>\r\n                            </div>\r\n\r\n                            <span class=\"show-stat-buttons\"> <span class=\"col-xs-12 col-sm-6 col-md-6 col-lg-6\"> <a\r\n                                    href-void class=\"btn btn-default btn-block hidden-xs\">Generate PDF</a> </span> <span\r\n                                    class=\"col-xs-12 col-sm-6 col-md-6 col-lg-6\"> <a href-void\r\n                                                                                     class=\"btn btn-default btn-block hidden-xs\">Report\r\n                                a bug</a> </span> </span>\r\n\r\n                        </div>\r\n\r\n                    </div>\r\n                </div>\r\n\r\n                <div class=\"show-stat-microcharts\" data-sparkline-container data-easy-pie-chart-container>\r\n                    <div class=\"col-xs-12 col-sm-3 col-md-3 col-lg-3\">\r\n\r\n                        <div class=\"easy-pie-chart txt-color-orangeDark\" data-percent=\"33\" data-pie-size=\"50\">\r\n                            <span class=\"percent percent-sign\">35</span>\r\n                        </div>\r\n                        <span class=\"easy-pie-title\"> Server Load <i class=\"fa fa-caret-up icon-color-bad\"></i> </span>\r\n                        <ul class=\"smaller-stat hidden-sm pull-right\">\r\n                            <li>\r\n                                <span class=\"label bg-color-greenLight\"><i class=\"fa fa-caret-up\"></i> 97%</span>\r\n                            </li>\r\n                            <li>\r\n                                <span class=\"label bg-color-blueLight\"><i class=\"fa fa-caret-down\"></i> 44%</span>\r\n                            </li>\r\n                        </ul>\r\n                        <div class=\"sparkline txt-color-greenLight hidden-sm hidden-md pull-right\"\r\n                             data-sparkline-type=\"line\" data-sparkline-height=\"33px\" data-sparkline-width=\"70px\"\r\n                             data-fill-color=\"transparent\">\r\n                            130, 187, 250, 257, 200, 210, 300, 270, 363, 247, 270, 363, 247\r\n                        </div>\r\n                    </div>\r\n                    <div class=\"col-xs-12 col-sm-3 col-md-3 col-lg-3\">\r\n                        <div class=\"easy-pie-chart txt-color-greenLight\" data-percent=\"78.9\" data-pie-size=\"50\">\r\n                            <span class=\"percent percent-sign\">78.9 </span>\r\n                        </div>\r\n                        <span class=\"easy-pie-title\"> Disk Space <i class=\"fa fa-caret-down icon-color-good\"></i></span>\r\n                        <ul class=\"smaller-stat hidden-sm pull-right\">\r\n                            <li>\r\n                                <span class=\"label bg-color-blueDark\"><i class=\"fa fa-caret-up\"></i> 76%</span>\r\n                            </li>\r\n                            <li>\r\n                                <span class=\"label bg-color-blue\"><i class=\"fa fa-caret-down\"></i> 3%</span>\r\n                            </li>\r\n                        </ul>\r\n                        <div class=\"sparkline txt-color-blue hidden-sm hidden-md pull-right\" data-sparkline-type=\"line\"\r\n                             data-sparkline-height=\"33px\" data-sparkline-width=\"70px\" data-fill-color=\"transparent\">\r\n                            257, 200, 210, 300, 270, 363, 130, 187, 250, 247, 270, 363, 247\r\n                        </div>\r\n                    </div>\r\n                    <div class=\"col-xs-12 col-sm-3 col-md-3 col-lg-3\">\r\n                        <div class=\"easy-pie-chart txt-color-blue\" data-percent=\"23\" data-pie-size=\"50\">\r\n                            <span class=\"percent percent-sign\">23 </span>\r\n                        </div>\r\n                        <span class=\"easy-pie-title\"> Transfered <i class=\"fa fa-caret-up icon-color-good\"></i></span>\r\n                        <ul class=\"smaller-stat hidden-sm pull-right\">\r\n                            <li>\r\n                                <span class=\"label bg-color-darken\">10GB</span>\r\n                            </li>\r\n                            <li>\r\n                                <span class=\"label bg-color-blueDark\"><i class=\"fa fa-caret-up\"></i> 10%</span>\r\n                            </li>\r\n                        </ul>\r\n                        <div class=\"sparkline txt-color-darken hidden-sm hidden-md pull-right\"\r\n                             data-sparkline-type=\"line\" data-sparkline-height=\"33px\" data-sparkline-width=\"70px\"\r\n                             data-fill-color=\"transparent\">\r\n                            200, 210, 363, 247, 300, 270, 130, 187, 250, 257, 363, 247, 270\r\n                        </div>\r\n                    </div>\r\n                    <div class=\"col-xs-12 col-sm-3 col-md-3 col-lg-3\">\r\n                        <div class=\"easy-pie-chart txt-color-darken\" data-percent=\"36\" data-pie-size=\"50\">\r\n                            <span class=\"percent degree-sign\">36 <i class=\"fa fa-caret-up\"></i></span>\r\n                        </div>\r\n                        <span class=\"easy-pie-title\"> Temperature <i\r\n                                class=\"fa fa-caret-down icon-color-good\"></i></span>\r\n                        <ul class=\"smaller-stat hidden-sm pull-right\">\r\n                            <li>\r\n                                <span class=\"label bg-color-red\"><i class=\"fa fa-caret-up\"></i> 124</span>\r\n                            </li>\r\n                            <li>\r\n                                <span class=\"label bg-color-blue\"><i class=\"fa fa-caret-down\"></i> 40 F</span>\r\n                            </li>\r\n                        </ul>\r\n                        <div class=\"sparkline txt-color-red hidden-sm hidden-md pull-right\" data-sparkline-type=\"line\"\r\n                             data-sparkline-height=\"33px\" data-sparkline-width=\"70px\" data-fill-color=\"transparent\">\r\n                            2700, 3631, 2471, 2700, 3631, 2471, 1300, 1877, 2500, 2577, 2000, 2100, 3000\r\n                        </div>\r\n                    </div>\r\n                </div>\r\n\r\n            </div>\r\n            <!-- end s1 tab pane -->\r\n\r\n            <div class=\"tab-pane fade\" id=\"s2\">\r\n                <div class=\"widget-body-toolbar bg-color-white\">\r\n\r\n                    <form class=\"form-inline\" role=\"form\">\r\n\r\n                        <div class=\"form-group\">\r\n                            <label class=\"sr-only\" for=\"s123\">Show From</label>\r\n                            <input type=\"email\" class=\"form-control input-sm\" id=\"s123\" placeholder=\"Show From\">\r\n                        </div>\r\n                        <div class=\"form-group\">\r\n                            <input type=\"email\" class=\"form-control input-sm\" id=\"s124\" placeholder=\"To\">\r\n                        </div>\r\n\r\n                        <div class=\"btn-group hidden-phone pull-right\">\r\n                            <a class=\"btn dropdown-toggle btn-xs btn-default\" data-toggle=\"dropdown\"><i\r\n                                    class=\"fa fa-cog\"></i> More <span class=\"caret\"> </span> </a>\r\n                            <ul class=\"dropdown-menu pull-right\">\r\n                                <li>\r\n                                    <a href-void><i class=\"fa fa-file-text-alt\"></i> Export to PDF</a>\r\n                                </li>\r\n                                <li>\r\n                                    <a href-void><i class=\"fa fa-question-sign\"></i> Help</a>\r\n                                </li>\r\n                            </ul>\r\n                        </div>\r\n\r\n                    </form>\r\n\r\n                </div>\r\n                <div class=\"padding-10\">\r\n                    <div id=\"statsChart\" class=\"chart-large has-legend-unique\" flot-basic flot-data=\"statsData\" flot-options=\"statsDisplayOptions\"></div>\r\n                </div>\r\n\r\n            </div>\r\n            <!-- end s2 tab pane -->\r\n\r\n            <div class=\"tab-pane fade\" id=\"s3\">\r\n\r\n                <div class=\"widget-body-toolbar bg-color-white smart-form\" id=\"rev-toggles\">\r\n\r\n                    <div class=\"inline-group\">\r\n\r\n                        <label for=\"gra-0\" class=\"checkbox\">\r\n                            <input type=\"checkbox\" id=\"gra-0\" ng-model=\"targetsShow\">\r\n                            <i></i> Target </label>\r\n                        <label for=\"gra-1\" class=\"checkbox\">\r\n                            <input type=\"checkbox\" id=\"gra-1\" ng-model=\"actualsShow\">\r\n                            <i></i> Actual </label>\r\n                        <label for=\"gra-2\" class=\"checkbox\">\r\n                            <input type=\"checkbox\" id=\"gra-2\" ng-model=\"signupsShow\">\r\n                            <i></i> Signups </label>\r\n                    </div>\r\n\r\n                    <div class=\"btn-group hidden-phone pull-right\">\r\n                        <a class=\"btn dropdown-toggle btn-xs btn-default\" data-toggle=\"dropdown\"><i\r\n                                class=\"fa fa-cog\"></i> More <span class=\"caret\"> </span> </a>\r\n                        <ul class=\"dropdown-menu pull-right\">\r\n                            <li>\r\n                                <a href-void><i class=\"fa fa-file-text-alt\"></i> Export to PDF</a>\r\n                            </li>\r\n                            <li>\r\n                                <a href-void><i class=\"fa fa-question-sign\"></i> Help</a>\r\n                            </li>\r\n                        </ul>\r\n                    </div>\r\n\r\n                </div>\r\n\r\n                <div class=\"padding-10\">\r\n                    <div id=\"flotcontainer\" class=\"chart-large has-legend-unique\" flot-basic flot-data=\"revenewData\" flot-options=\"revenewDisplayOptions\" ></div>\r\n                </div>\r\n            </div>\r\n            <!-- end s3 tab pane -->\r\n        </div>\r\n\r\n        <!-- end content -->\r\n    </div>\r\n\r\n</div>\r\n<!-- end widget div -->\r\n</div>\r\n");
$templateCache.put("app/layout/layout.tpl.html","<div ng-intro-options=\"IntroOptions\" ng-intro-method=\"CallMe\">\r\n<!-- HEADER -->\r\n<div data-smart-include=\"app/layout/partials/header.tpl.html\" class=\"placeholder-header\"></div>\r\n<!-- END HEADER -->\r\n\r\n\r\n<!-- Left panel : Navigation area -->\r\n<!-- Note: This width of the aside area can be adjusted through LESS variables -->\r\n<div data-smart-include=\"app/layout/partials/navigation.tpl.html\" class=\"placeholder-left-panel\"></div>\r\n\r\n<!-- END NAVIGATION -->\r\n\r\n<!-- MAIN PANEL -->\r\n<div id=\"main\" role=\"main\">\r\n    <demo-states></demo-states>\r\n\r\n    <!-- RIBBON -->\r\n    <div id=\"ribbon\">\r\n\r\n				<span id=\"reset-settings\" class=\"ribbon-button-alignment\">\r\n					<span id=\"refresh\" class=\"btn btn-ribbon\" reset-widgets\r\n                          tooltip-placement=\"bottom\"\r\n                          tooltip-html=\"<i class=\'text-warning fa fa-warning\'></i> Warning! This will reset all your widget settings.\">\r\n						<i class=\"fa fa-refresh\"></i>\r\n					</span>\r\n				</span>\r\n\r\n        <!-- breadcrumb -->\r\n        <state-breadcrumbs></state-breadcrumbs>\r\n        <!-- end breadcrumb -->\r\n\r\n\r\n    </div>\r\n    <!-- END RIBBON -->\r\n\r\n\r\n    <div data-smart-router-animation-wrap=\"content content@app\" data-wrap-for=\"#content\">\r\n        <div data-ui-view=\"content\" data-autoscroll=\"false\"></div>\r\n    </div>\r\n\r\n</div>\r\n<!-- END MAIN PANEL -->\r\n\r\n<!-- PAGE FOOTER -->\r\n<div data-smart-include=\"app/layout/partials/footer.tpl.html\"></div>\r\n\r\n<div data-smart-include=\"app/layout/shortcut/shortcut.tpl.html\"></div>\r\n\r\n<!-- END PAGE FOOTER -->\r\n </div>\r\n\r\n");
$templateCache.put("app/auth/directives/login-info.tpl.html","<div class=\"login-info ng-cloak\">\r\n    <span> <!-- User image size is adjusted inside CSS, it should stay as it -->\r\n        <a id=\"my-login-info\" href=\"\" toggle-shortcut>\r\n            <img ng-src=\"{{user.image()}}\" alt=\"me\" class=\"online\">\r\n                <span>{{user.displayName}}\r\n                </span>\r\n            <i class=\"fa fa-angle-down\"></i>\r\n        </a>\r\n     </span>\r\n</div>");
$templateCache.put("app/dashboard/projects/recent-projects.tpl.html","<div class=\"project-context hidden-xs dropdown\" dropdown>\r\n\r\n    <span class=\"label\">{{getWord(\'Projects\')}}:</span>\r\n    <span class=\"project-selector dropdown-toggle\" dropdown-toggle>{{getWord(\'Recent projects\')}} <i ng-if=\"projects.length\"\r\n            class=\"fa fa-angle-down\"></i></span>\r\n\r\n    <ul class=\"dropdown-menu\" ng-if=\"projects.length\">\r\n        <li ng-repeat=\"project in projects\">\r\n            <a href=\"{{project.href}}\">{{project.title}}</a>\r\n        </li>\r\n        <li class=\"divider\"></li>\r\n        <li>\r\n            <a ng-click=\"clearProjects()\"><i class=\"fa fa-power-off\"></i> Clear</a>\r\n        </li>\r\n    </ul>\r\n\r\n</div>");
$templateCache.put("app/dashboard/todo/todo-widget.tpl.html","<div id=\"todo-widget\" jarvis-widget data-widget-editbutton=\"false\" data-widget-color=\"blue\"\r\n     ng-controller=\"TodoCtrl\">\r\n    <header>\r\n        <span class=\"widget-icon\"> <i class=\"fa fa-check txt-color-white\"></i> </span>\r\n\r\n        <h2> ToDo\'s </h2>\r\n\r\n        <div class=\"widget-toolbar\">\r\n            <!-- add: non-hidden - to disable auto hide -->\r\n            <button class=\"btn btn-xs btn-default\" ng-class=\"{active: newTodo}\" ng-click=\"toggleAdd()\"><i ng-class=\"{ \'fa fa-plus\': !newTodo, \'fa fa-times\': newTodo}\"></i> Add</button>\r\n\r\n        </div>\r\n    </header>\r\n    <!-- widget div-->\r\n    <div>\r\n        <div class=\"widget-body no-padding smart-form\">\r\n            <!-- content goes here -->\r\n            <div ng-show=\"newTodo\">\r\n                <h5 class=\"todo-group-title\"><i class=\"fa fa-plus-circle\"></i> New Todo</h5>\r\n\r\n                <form name=\"newTodoForm\" class=\"smart-form\">\r\n                    <fieldset>\r\n                        <section>\r\n                            <label class=\"input\">\r\n                                <input type=\"text\" required class=\"input-lg\" ng-model=\"newTodo.title\"\r\n                                       placeholder=\"What needs to be done?\">\r\n                            </label>\r\n                        </section>\r\n                        <section>\r\n                            <div class=\"col-xs-6\">\r\n                                <label class=\"select\">\r\n                                    <select class=\"input-sm\" ng-model=\"newTodo.state\"\r\n                                            ng-options=\"state as state for state in states\"></select> <i></i> </label>\r\n                            </div>\r\n                        </section>\r\n                    </fieldset>\r\n                    <footer>\r\n                        <button ng-disabled=\"newTodoForm.$invalid\" type=\"button\" class=\"btn btn-primary\"\r\n                                ng-click=\"createTodo()\">\r\n                            Add\r\n                        </button>\r\n                        <button type=\"button\" class=\"btn btn-default\" ng-click=\"toggleAdd()\">\r\n                            Cancel\r\n                        </button>\r\n                    </footer>\r\n                </form>\r\n            </div>\r\n\r\n            <todo-list state=\"Critical\"  title=\"Critical Tasks\" icon=\"warning\" todos=\"todos\"></todo-list>\r\n\r\n            <todo-list state=\"Important\" title=\"Important Tasks\" icon=\"exclamation\" todos=\"todos\"></todo-list>\r\n\r\n            <todo-list state=\"Completed\" title=\"Completed Tasks\" icon=\"check\" todos=\"todos\"></todo-list>\r\n\r\n            <!-- end content -->\r\n        </div>\r\n\r\n    </div>\r\n    <!-- end widget div -->\r\n</div>");
$templateCache.put("app/homepage/views/sub-header.tpl.html","<div class=\"col-xs-12 col-sm-5 col-md-5 col-lg-8\" data-sparkline-container>\r\n    <ul id=\"sparks\" class=\"\">\r\n        <li class=\"sparks-info\">\r\n            <h5> {{getWord(\'TaskCount\')}} <span class=\"txt-color-blue\">1,271</span></h5>\r\n            <div class=\"sparkline txt-color-blue hidden-mobile hidden-md hidden-sm\">\r\n                130, 187, 250, 257, 200, 210, 300, 270, 363, 247, 270, 363, 247\r\n            </div>\r\n        </li>\r\n        <li class=\"sparks-info\">\r\n            <h5> {{getWord(\"Efficiency\")}} <span class=\"txt-color-purple\"><i class=\"fa fa-arrow-circle-up\"></i>&nbsp;25%</span></h5>\r\n            <div class=\"sparkline txt-color-purple hidden-mobile hidden-md hidden-sm\">\r\n                110,150,300,130,400,240,220,310,220,300, 270, 210\r\n            </div>\r\n        </li>\r\n        <li class=\"sparks-info\">\r\n            <h5> {{getWord(\"CompletedTasks\")}} <span class=\"txt-color-greenDark\"><i class=\"fa fa-check-circle\"></i>&nbsp;879</span></h5>\r\n            <div class=\"sparkline txt-color-greenDark hidden-mobile hidden-md hidden-sm\">\r\n                110,150,300,130,400,240,220,310,220,300, 270, 210\r\n            </div>\r\n        </li>\r\n    </ul>\r\n</div>\r\n			");
$templateCache.put("app/layout/language/language-selector.tpl.html","<ul class=\"header-dropdown-list hidden-xs ng-cloak\" ng-controller=\"LanguagesCtrl\">\r\n    <li class=\"dropdown\" dropdown>\r\n        <a class=\"dropdown-toggle\"  dropdown-toggle href> <img src=\"styles/img/blank.gif\" class=\"flag flag-{{currentLanguage.key}}\" alt=\"{{currentLanguage.alt}}\"> <span> {{currentLanguage.title}} </span>\r\n            <i class=\"fa fa-angle-down\"></i> </a>\r\n        <ul class=\"dropdown-menu pull-right\">\r\n            <li ng-class=\"{active: language==currentLanguage}\" ng-repeat=\"language in languages\">\r\n                <a ng-click=\"selectLanguage(language)\" ><img src=\"styles/img/blank.gif\" class=\"flag flag-{{language.key}}\"\r\n                                                   alt=\"{{language.alt}}\"> {{language.title}}</a>\r\n            </li>\r\n        </ul>\r\n    </li>\r\n</ul>");
$templateCache.put("app/layout/partials/footer.tpl.html","<div class=\"page-footer\">\r\n    <div class=\"row\">\r\n        <div class=\"col-xs-12 col-sm-6\">\r\n            <span class=\"txt-color-white\">Testing Data Management (TDM) System</span>\r\n        </div>\r\n\r\n        <div class=\"col-xs-6 col-sm-6 text-right hidden-xs\">\r\n            \r\n        </div>\r\n    </div>\r\n</div>");
$templateCache.put("app/layout/partials/header.tpl.html","<header id=\"header\">\r\n<div id=\"logo-group\">\r\n\r\n    <!-- PLACE YOUR LOGO HERE -->\r\n    <span id=\"logo\"><a ui-sref=\"app.homepage.mainmenu\"><img src=\"styles/img/logo.gif\" alt=\"TDM\"></a>\r\n    </span>\r\n    <!-- END LOGO PLACEHOLDER -->\r\n\r\n    <!-- Note: The activity badge color changes when clicked and resets the number to 0\r\n    Suggestion: You may want to set a flag when this happens to tick off all checked messages / notifications -->\r\n   \r\n    <!--\r\n    <span id=\"activity\"> \r\n        <a ui-sref=\"app.tasks.list\" ui-sref-opts=\"{reload: true}\" title=\"{{getWord(\'MyTasks\')}}\"><i class=\"fa fa-user\"></i>\r\n            <b class=\"badge bg-color-red\">{{getTaskCount()}}</b>\r\n        </a>\r\n    </span>\r\n    -->\r\n\r\n    <!-- Note: The activity badge color changes when clicked and resets the number to 0\r\n    Suggestion: You may want to set a flag when this happens to tick off all checked messages / notifications -->\r\n    <span id=\"activity\" class=\"activity-dropdown\" activities-dropdown-toggle>\r\n        <i class=\"fa fa-user\"></i>\r\n        <b class=\"badge bg-color-red\">{{getTotalCount()}}</b>\r\n    </span>\r\n    <div smart-include=\"app/homepage/views/my-activities.html\"></div>\r\n</div>\r\n\r\n<!--\r\n<div style=\"margin-left:300px\">\r\n    <h1 class=\"text-primary hidden-xs hidden-sm hidden-md\">{{getWord(\'AppName\')}}</h1>\r\n</div>\r\n-->\r\n\r\n<!-- pulled right: nav area -->\r\n<div class=\"pull-right\">\r\n\r\n    <!-- intro button -->\r\n    <div id=\"intro\" class=\"btn-header transparent pull-right\">\r\n        <span>\r\n            <a title=\"{{getWord(\'Intro\')}}\" ng-click=\"CallMe();\">\r\n                <i class=\"fa fa-info\"></i>\r\n            </a>\r\n        </span>\r\n    </div>\r\n    <!-- end intro button -->\r\n\r\n    <!-- collapse menu button -->\r\n    <div id=\"hide-menu\" class=\"btn-header pull-right\">\r\n        <span> <a toggle-menu title=\"{{getWord(\'CollapseMenu\')}}\"><i\r\n                class=\"fa fa-reorder\"></i></a> </span>\r\n    </div>\r\n    <!-- end collapse menu -->\r\n\r\n    <!-- #MOBILE -->\r\n    <!-- Top menu profile link : this shows only when top menu is active -->\r\n    <ul id=\"mobile-profile-img\" class=\"header-dropdown-list hidden-xs padding-5\">\r\n        <li class=\"\">\r\n            <a href=\"#\" class=\"dropdown-toggle no-margin userdropdown\" data-toggle=\"dropdown\">\r\n                <img src=\"styles/custom/avatars/male.png\" alt=\"John Doe\" class=\"online\"/>\r\n            </a>\r\n            <ul class=\"dropdown-menu pull-right\">\r\n                <li>\r\n                    <a href-void class=\"padding-10 padding-top-0 padding-bottom-0\"><i\r\n                            class=\"fa fa-cog\"></i> Setting</a>\r\n                </li>\r\n                <li class=\"divider\"></li>\r\n                <li>\r\n                    <a ui-sref=\"app.appViews.profileDemo\" class=\"padding-10 padding-top-0 padding-bottom-0\"> <i class=\"fa fa-user\"></i>\r\n                        <u>P</u>rofile</a>\r\n                </li>\r\n                <li class=\"divider\"></li>\r\n                <li>\r\n                    <a href-void class=\"padding-10 padding-top-0 padding-bottom-0\"\r\n                       data-action=\"toggleShortcut\"><i class=\"fa fa-arrow-down\"></i> <u>S</u>hortcut</a>\r\n                </li>\r\n                <li class=\"divider\"></li>\r\n                <li>\r\n                    <a href-void class=\"padding-10 padding-top-0 padding-bottom-0\"\r\n                       data-action=\"launchFullscreen\"><i class=\"fa fa-arrows-alt\"></i> Full <u>S</u>creen</a>\r\n                </li>\r\n                <li class=\"divider\"></li>\r\n                <li>\r\n                    <a href=\"#/login\" class=\"padding-10 padding-top-5 padding-bottom-5\" data-action=\"userLogout\"><i\r\n                            class=\"fa fa-sign-out fa-lg\"></i> <strong><u>L</u>ogout</strong></a>\r\n                </li>\r\n            </ul>\r\n        </li>\r\n    </ul>\r\n\r\n    <!-- logout button -->\r\n    <div id=\"logout\" class=\"btn-header transparent pull-right\">\r\n        <span> <a ui-sref=\"logout\" title=\"{{getWord(\'SignOut\')}}\" data-action=\"userLogout\"\r\n                  data-logout-msg=\"You can improve your security further after logging out by closing this opened browser\"><i\r\n                class=\"fa fa-sign-out\"></i></a> </span>\r\n    </div>\r\n    <!-- end logout button -->\r\n\r\n    <!-- search mobile button (this is hidden till mobile view port) -->\r\n    <div id=\"search-mobile\" class=\"btn-header transparent pull-right\" data-search-mobile>\r\n        <span> <a href=\"#\" title=\"Search\"><i class=\"fa fa-search\"></i></a> </span>\r\n    </div>\r\n    <!-- end search mobile button -->\r\n\r\n    <!-- fullscreen button -->\r\n    <div id=\"fullscreen\" class=\"btn-header transparent pull-right\">\r\n        <span> <a full-screen title=\"{{getWord(\'FullScreen\')}}\"><i\r\n                class=\"fa fa-arrows-alt\"></i></a> </span>\r\n    </div>\r\n    <!-- end fullscreen button -->\r\n\r\n    <!-- multiple lang dropdown : find all flags in the flags page -->\r\n    <language-selector></language-selector>\r\n    <!-- end multiple lang -->\r\n\r\n    <!-- input: full text search field -->\r\n    <form ng-show=\"searchEnabled\" ng-submit=\"fullTextSearch()\" class=\"header-search pull-right\" style=\"padding-right:10px\">\r\n        <input id=\"search-keywords\" name=\"searchKeywords\" type=\"text\" autocomplete=\"off\" size=\"50\" \r\n               placeholder=\"{{getWord(\'FindReports\')}}\"\r\n               ng-model=\"searchContext.searchText\"\r\n               typeahead=\"suggestion for suggestion in getSuggestions($viewValue)\"\r\n               typeahead-on-select=\"onSuggestionSelect($item, $model, $label)\">\r\n        <button type=\"submit\" style=\"padding-right:15px\">\r\n            <i class=\"fa fa-search\"></i>\r\n        </button>\r\n        <a href=\"$\" id=\"cancel-search-js\" title=\"Cancel Search\"><i class=\"fa fa-times\"></i></a>\r\n    </form>\r\n    <!-- end input: search field -->\r\n\r\n</div>\r\n<!-- end pulled right: nav area -->\r\n\r\n</header>");
$templateCache.put("app/layout/partials/help-viewer.tpl.html","<div id=\"content\" class=\"wrapper\">\r\n    <!-- widget grid -->\r\n    <section widget-grid id=\"widget-grid\">\r\n        <div class=\"row\">\r\n            <article class=\"col-sm-12 col-md-12 col-lg-12\">\r\n                <div jarvis-widget id=\"wid-id-1\" data-widget-editbutton=\"false\" data-widget-custombutton=\"false\">\r\n                    <header>\r\n                        <span class=\"widget-icon\"> <i class=\"fa fa-edit\"></i> </span>\r\n                        <h2>{{getWord(\"Help\")}}</h2>\r\n                        <div class=\"widget-toolbar\">\r\n                            <span class=\"jarviswidget-ctrls\" ng-click=\"$dismiss()\"> <i class=\"fa fa-close\"></i> </span>\r\n                        </div>\r\n                    </header>\r\n                    <div style=\"overflow-x:auto;\">\r\n                        <!-- widget content -->\r\n                        <div align=\"center\" class=\"padding-10\">\r\n                            <button ng-click=\"zoomIn()\"><i class=\"fa fa-plus\"></i></button>\r\n                            <button ng-click=\"fit()\"><span>100%</span></button>\r\n                            <button ng-click=\"zoomOut()\"><i class=\"fa fa-minus\"></i></button>\r\n                        </div>\r\n                        <div>\r\n                            <ng-pdf template-url=\"app/layout/partials/help-viewer..html\" scale=\"page-fit\"></ng-pdf>\r\n                        </div>\r\n                        <div align=\"center\" class=\"padding-10\">\r\n                            <button ng-click=\"goPrevious()\"><i class=\"fa fa-chevron-left\"></i><span> {{getWord(\"Prev Page\")}}</span></button>\r\n                            <button ng-click=\"goNext()\"><i class=\"fa fa-chevron-right\"></i><span> {{getWord(\"Next Page\")}}</span></button>\r\n                        </div>\r\n                    </div>\r\n                </div>\r\n            </article>\r\n        </div>\r\n    </section>\r\n</div>");
$templateCache.put("app/layout/partials/navigation.tpl.html","<aside id=\"left-panel\">\r\n\r\n    <!-- User info -->\r\n    <div login-info></div>\r\n    <!-- end user info -->\r\n\r\n    <nav id=\"sidemenu\">\r\n\r\n        <!-- NOTE: This allows you to pull menu items from server -->\r\n        <ul data-smart-menu-items=\"/api/sitemap/menu\"></ul>\r\n\r\n    </nav>\r\n\r\n  <span id=\"minimize-sidemenu\" class=\"minifyme\" data-action=\"minifyMenu\" minify-menu>\r\n    <i class=\"fa fa-arrow-circle-left hit\"></i>\r\n  </span>\r\n\r\n</aside>");
$templateCache.put("app/layout/partials/sub-header.tpl.html","<div class=\"col-xs-12 col-sm-5 col-md-5 col-lg-8\" data-sparkline-container>\r\n    <ul id=\"sparks\" class=\"\">\r\n        <li class=\"sparks-info\">\r\n            <h5> My Income <span class=\"txt-color-blue\">$47,171</span></h5>\r\n            <div class=\"sparkline txt-color-blue hidden-mobile hidden-md hidden-sm\">\r\n                1300, 1877, 2500, 2577, 2000, 2100, 3000, 2700, 3631, 2471, 2700, 3631, 2471\r\n            </div>\r\n        </li>\r\n        <li class=\"sparks-info\">\r\n            <h5> Site Traffic <span class=\"txt-color-purple\"><i class=\"fa fa-arrow-circle-up\"></i>&nbsp;45%</span></h5>\r\n            <div class=\"sparkline txt-color-purple hidden-mobile hidden-md hidden-sm\">\r\n                110,150,300,130,400,240,220,310,220,300, 270, 210\r\n            </div>\r\n        </li>\r\n        <li class=\"sparks-info\">\r\n            <h5> Site Orders <span class=\"txt-color-greenDark\"><i class=\"fa fa-shopping-cart\"></i>&nbsp;2447</span></h5>\r\n            <div class=\"sparkline txt-color-greenDark hidden-mobile hidden-md hidden-sm\">\r\n                110,150,300,130,400,240,220,310,220,300, 270, 210\r\n            </div>\r\n        </li>\r\n    </ul>\r\n</div>\r\n			");
$templateCache.put("app/layout/partials/voice-commands.tpl.html","<!-- TRIGGER BUTTON:\r\n<a href=\"/my-ajax-page.html\" data-toggle=\"modal\" data-target=\"#remoteModal\" class=\"btn btn-default\">Open Modal</a>  -->\r\n\r\n<!-- MODAL PLACE HOLDER\r\n<div class=\"modal fade\" id=\"remoteModal\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"remoteModalLabel\" aria-hidden=\"true\">\r\n<div class=\"modal-dialog\">\r\n<div class=\"modal-content\"></div>\r\n</div>\r\n</div>   -->\r\n<!--////////////////////////////////////-->\r\n\r\n<!--<div class=\"modal-header\">\r\n<button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-hidden=\"true\">\r\n&times;\r\n</button>\r\n<h4 class=\"modal-title\" id=\"myModalLabel\">Command List</h4>\r\n</div>-->\r\n<div class=\"modal-body\">\r\n\r\n	<h1><i class=\"fa fa-microphone text-muted\"></i>&nbsp;&nbsp; SmartAdmin Voice Command</h1>\r\n	<hr class=\"simple\">\r\n	<h5>Instruction</h5>\r\n\r\n	Click <span class=\"text-success\">\"Allow\"</span> to access your microphone and activate Voice Command.\r\n	You will notice a <span class=\"text-primary\"><strong>BLUE</strong> Flash</span> on the microphone icon indicating activation.\r\n	The icon will appear <span class=\"text-danger\"><strong>RED</strong></span> <span class=\"label label-danger\"><i class=\"fa fa-microphone fa-lg\"></i></span> if you <span class=\"text-danger\">\"Deny\"</span> access or don\'t have any microphone installed.\r\n	<br>\r\n	<br>\r\n	As a security precaution, your browser will disconnect the microphone every 60 to 120 seconds (sooner if not being used). In which case Voice Command will prompt you again to <span class=\"text-success\">\"Allow\"</span> or <span class=\"text-danger\">\"Deny\"</span> access to your microphone.\r\n	<br>\r\n	<br>\r\n	If you host your page over <strong>http<span class=\"text-success\">s</span></strong> (secure socket layer) protocol you can wave this security measure and have an unintrupted Voice Command.\r\n	<br>\r\n	<br>\r\n	<h5>Commands</h5>\r\n	<ul>\r\n		<li>\r\n			<strong>\'show\' </strong> then say the <strong>*page*</strong> you want to go to. For example <strong>\"show inbox\"</strong> or <strong>\"show calendar\"</strong>\r\n		</li>\r\n		<li>\r\n			<strong>\'mute\' </strong> - mutes all sound effects for the theme.\r\n		</li>\r\n		<li>\r\n			<strong>\'sound on\'</strong> - unmutes all sound effects for the theme.\r\n		</li>\r\n		<li>\r\n			<span class=\"text-danger\"><strong>\'stop\'</strong></span> - deactivates voice command.\r\n		</li>\r\n		<li>\r\n			<span class=\"text-primary\"><strong>\'help\'</strong></span> - brings up the command list\r\n		</li>\r\n		<li>\r\n			<span class=\"text-danger\"><strong>\'got it\'</strong></span> - closes help modal\r\n		</li>\r\n		<li>\r\n			<strong>\'hide navigation\'</strong> - toggle navigation collapse\r\n		</li>\r\n		<li>\r\n			<strong>\'show navigation\'</strong> - toggle navigation to open (can be used again to close)\r\n		</li>\r\n		<li>\r\n			<strong>\'scroll up\'</strong> - scrolls to the top of the page\r\n		</li>\r\n		<li>\r\n			<strong>\'scroll down\'</strong> - scrollts to the bottom of the page\r\n		</li>\r\n		<li>\r\n			<strong>\'go back\' </strong> - goes back in history (history -1 click)\r\n		</li>\r\n		<li>\r\n			<strong>\'logout\'</strong> - logs you out\r\n		</li>\r\n	</ul>\r\n	<br>\r\n	<h5>Adding your own commands</h5>\r\n	Voice Command supports up to 80 languages. Adding your own commands is extreamly easy. All commands are stored inside <strong>app.config.js</strong> file under the <code>var commands = {...}</code>. \r\n\r\n	<hr class=\"simple\">\r\n	<div class=\"text-right\">\r\n		<button type=\"button\" class=\"btn btn-success btn-lg\" data-dismiss=\"modal\">\r\n			Got it!\r\n		</button>\r\n	</div>\r\n\r\n</div>\r\n<!--<div class=\"modal-footer\">\r\n<button type=\"button\" class=\"btn btn-primary\" data-dismiss=\"modal\">Got it!</button>\r\n</div> -->");
$templateCache.put("app/layout/shortcut/shortcut.tpl.html","<div id=\"shortcut\">\r\n	<ul>\r\n		<li>\r\n			<a href=\"#/tasks/list\" class=\"jarvismetro-tile big-cubes bg-color-blue\"> <span class=\"iconbox\"> <i class=\"fa fa-folder-open fa-4x\"></i> <span>{{getWord(\'MyTasks\')}} <span class=\"label pull-right bg-color-darken\">{{getTaskCount()}}</span></span> </span> </a>\r\n		</li>\r\n		<li>\r\n			<a href=\"#/user/profile\" class=\"jarvismetro-tile big-cubes selected bg-color-pinkDark\"> <span class=\"iconbox\"> <i class=\"fa fa-user fa-4x\"></i> <span>{{getWord(\'My Profile\')}} </span> </span> </a>\r\n		</li>\r\n        <li>\r\n            <a href=\"#/user/password\" class=\"jarvismetro-tile big-cubes selected bg-color-darken\"> <span class=\"iconbox\"> <i class=\"fa fa-lock fa-4x\"></i> <span>{{getWord(\'Change Password\')}} </span> </span> </a>\r\n        </li>\r\n	</ul>\r\n</div>");
$templateCache.put("app/mldashboard/views/model-dashboard.tpl.html","<div jarvis-widget id=\"live-feeds-widget\" data-widget-togglebutton=\"false\" data-widget-editbutton=\"false\"\r\n     data-widget-fullscreenbutton=\"false\" data-widget-colorbutton=\"false\" data-widget-deletebutton=\"false\">\r\n    <header>\r\n        <span class=\"widget-icon\"> <i class=\"glyphicon glyphicon-stats txt-color-darken\"></i> </span>\r\n\r\n        <h2>{{project}}=>{{model}}</h2>\r\n\r\n        <ul class=\"nav nav-tabs pull-right in\" id=\"myTab\">\r\n            <li class=\"active\">\r\n                <a data-toggle=\"tab\" href=\"#s1\"><i class=\"fa fa-clock-o\"></i> <span class=\"hidden-mobile hidden-tablet\">{{getWord(\"Single Record\")}}</span></a>\r\n            </li>\r\n\r\n            <li>\r\n                <a data-toggle=\"tab\" href=\"#s2\"><i class=\"fa fa-cog\"></i> <span class=\"hidden-mobile hidden-tablet\">{{getWord(\"Batch Records\")}}</span></a>\r\n            </li>\r\n        </ul>\r\n\r\n    </header>\r\n\r\n    <!-- widget div-->\r\n    <div class=\"no-padding\">\r\n\r\n        <div class=\"widget-body\">\r\n            <!-- content -->\r\n            <div id=\"myTabContent\" class=\"tab-content\">\r\n                <div class=\"tab-pane fade active in padding-10 no-padding-bottom\" id=\"s1\">\r\n                    <!-- widget grid -->\r\n                    <section id=\"widget-grid\" class=\"\">\r\n\r\n                        <!-- START ROW -->\r\n                        <div class=\"row\">\r\n\r\n                            <!-- NEW COL START -->\r\n                            <article class=\"col-sm-12 col-md-12 col-lg-12\">\r\n\r\n                                <!-- Widget ID (each widget will need unique ID)-->\r\n                                <div class=\"jarviswidget\" id=\"wid-id-0\" data-widget-colorbutton=\"false\" data-widget-editbutton=\"false\" data-widget-custombutton=\"false\">\r\n                                                <!-- widget options:\r\n                                    usage: <div class=\"jarviswidget\" id=\"wid-id-0\" data-widget-editbutton=\"false\">\r\n\r\n                                    data-widget-colorbutton=\"false\"\r\n                                    data-widget-editbutton=\"false\"\r\n                                    data-widget-togglebutton=\"false\"\r\n                                    data-widget-deletebutton=\"false\"\r\n                                    data-widget-fullscreenbutton=\"false\"\r\n                                    data-widget-custombutton=\"false\"\r\n                                    data-widget-collapsed=\"true\"\r\n                                    data-widget-sortable=\"false\"\r\n\r\n                                    -->\r\n                                    <header>\r\n                                        <span class=\"widget-icon\"> <i class=\"fa fa-edit\"></i> </span>\r\n                                        <h2>{{getWord(\"ML Model Form\")}}</h2>\r\n\r\n                                    </header>\r\n\r\n                                    <!-- widget div-->\r\n                                    <div>\r\n                                        <!-- widget content -->\r\n                                        <div class=\"widget-body no-padding\">\r\n\r\n                                            <form class=\"smart-form\" ng-submit=\"submitModelForm()\">\r\n                                                <header>\r\n                                                    {{getWord(\"ML Model Inputs\")}}\r\n                                                </header>\r\n\r\n                                                <fieldset>\r\n                                                    <div class=\"row\">\r\n                                                        <section class=\"col col-3\" ng-repeat=\"inputField in inputFields\">\r\n                                                            <label class=\"label\">{{inputField.Label}}</label>\r\n                                                            <label class=\"input\">\r\n                                                                <input type=\"text\" id=\"{{inputField.Label}}\" required ng-model=\"inputField.Value\">\r\n                                                            </label>\r\n                                                        </section>\r\n                                                    </div>\r\n                                                </fieldset>\r\n\r\n                                                <header>\r\n                                                    {{getWord(\"ML Model Outputs\")}}\r\n                                                </header>\r\n\r\n                                                <fieldset>\r\n                                                    <div class=\"row\">\r\n                                                        <section class=\"col col-3\" ng-repeat=\"outputField in outputFields\">\r\n                                                            <label class=\"label\">{{outputField.Label}}</label>\r\n                                                            <label class=\"input\">\r\n                                                                <input type=\"text\" id=\"{{outputField.Label}}\" disabled=\"disabled\" ng-model=\"outputField.Value\">\r\n                                                            </label>\r\n                                                        </section>\r\n                                                    </div>\r\n\r\n                                                </fieldset>\r\n\r\n                                                <footer>\r\n                                                    <button type=\"submit\" class=\"btn btn-primary\" button-spinner=\"loading\">\r\n                                                        {{getWord(\"Evaluate\")}}\r\n                                                    </button>\r\n                                                </footer>\r\n                                            </form>\r\n\r\n                                        </div>\r\n                                        <!-- end widget content -->\r\n\r\n                                    </div>\r\n                                    <!-- end widget div -->\r\n\r\n                                </div>\r\n                                <!-- end widget -->\r\n\r\n                            </article>\r\n                            <!-- END COL -->\r\n\r\n                        </div>\r\n\r\n                        <!-- END ROW -->\r\n                    </section>\r\n                    <!-- end widget grid -->\r\n                </div>\r\n                <!-- end s1 tab pane -->\r\n\r\n                <div class=\"tab-pane fade\" id=\"s2\">\r\n                    <div class=\"padding-10\">\r\n                   \r\n                    </div>\r\n                </div>\r\n                <!-- end s2 tab pane -->\r\n            </div>\r\n            <!-- end content -->\r\n        </div>\r\n    </div>\r\n    <!-- end widget div -->\r\n</div>\r\n");
$templateCache.put("app/stations/views/live-feeds.tpl.html","<div jarvis-widget id=\"live-feeds-widget\" data-widget-togglebutton=\"false\" data-widget-editbutton=\"false\"\r\n     data-widget-fullscreenbutton=\"false\" data-widget-colorbutton=\"false\" data-widget-deletebutton=\"false\">\r\n<!-- widget options:\r\nusage: <div class=\"jarviswidget\" id=\"wid-id-0\" data-widget-editbutton=\"false\">\r\n\r\ndata-widget-colorbutton=\"false\"\r\ndata-widget-editbutton=\"false\"\r\ndata-widget-togglebutton=\"false\"\r\ndata-widget-deletebutton=\"false\"\r\ndata-widget-fullscreenbutton=\"false\"\r\ndata-widget-custombutton=\"false\"\r\ndata-widget-collapsed=\"true\"\r\ndata-widget-sortable=\"false\"\r\n\r\n-->\r\n<header>\r\n    <span class=\"widget-icon\"> <i class=\"glyphicon glyphicon-stats txt-color-darken\"></i> </span>\r\n\r\n    <h2>{{CurrentStationName}}</h2>\r\n\r\n    <ul class=\"nav nav-tabs pull-right in\" id=\"myTab\">\r\n        <li ng-class=\"showMonitor? \'active\' : null\" ng-show=\"showMonitor\">\r\n            <a data-toggle=\"tab\" href=\"#s1\"><i class=\"fa fa-clock-o\"></i> <span class=\"hidden-mobile hidden-tablet\">{{getWord(\"LiveData\")}}</span></a>\r\n        </li>\r\n\r\n        <li ng-class=\"!showMonitor? \'active\' : null\">\r\n            <a data-toggle=\"tab\" href=\"#s2\"><i class=\"fa fa-cog\"></i> <span class=\"hidden-mobile hidden-tablet\">{{getWord(\"StationConfig\")}}</span></a>\r\n        </li>\r\n\r\n        <li>\r\n            <a data-toggle=\"tab\" href=\"#s3\"><i class=\"fa fa-calendar\"></i> <span class=\"hidden-mobile hidden-tablet\">{{getWord(\"Scheduler\")}}</span></a>\r\n        </li>\r\n    </ul>\r\n\r\n</header>\r\n\r\n<!-- widget div-->\r\n<div class=\"no-padding\">\r\n\r\n    <div class=\"widget-body\">\r\n        <!-- content -->\r\n        <div id=\"myTabContent\" class=\"tab-content\">\r\n            <div ng-class=\"showMonitor? \'tab-pane fade active in padding-10 no-padding-bottom\' : \'tab-pane fade\'\" id=\"s1\" ng-show=\"showMonitor\">\r\n                <div class=\"row no-space\">\r\n                    <div class=\"col-xs-12 col-sm-12 col-md-8 col-lg-8\">\r\n						<span class=\"demo-liveupdate-1\"> <span\r\n                                class=\"onoffswitch-title\">{{getWord(\"LiveSwitch\")}}</span> <span\r\n                                class=\"onoffswitch\">\r\n								<input type=\"checkbox\" name=\"start_interval\" ng-model=\"autoUpdate\"\r\n                                        class=\"onoffswitch-checkbox\" id=\"start_interval\">\r\n								<label class=\"onoffswitch-label\" for=\"start_interval\">\r\n                                    <span class=\"onoffswitch-inner\"\r\n                                            data-swchon-text=\"{{getWord(\'SwitchOn\')}}\"\r\n                                            data-swchoff-text=\"{{getWord(\'SwitchOff\')}}\"></span>\r\n                                    <span class=\"onoffswitch-switch\"></span>\r\n                                </label> </span> </span>\r\n\r\n                        <div id=\"updating-chart\" class=\"chart-large txt-color-blue\" flot-basic flot-data=\"liveStats\" flot-options=\"liveStatsOptions\"></div>\r\n\r\n                    </div>\r\n                    <div class=\"col-xs-12 col-sm-12 col-md-4 col-lg-4 show-stats\">\r\n\r\n                        <div class=\"row\">\r\n                            <div class=\"col-xs-6 col-sm-6 col-md-12 col-lg-12\"><span class=\"text\"> {{settings.ProgressName1}} :<span\r\n                                    >{{settings.ProgressValue1}} {{settings.ProgressUnit1}}</span> </span>\r\n\r\n                                <div class=\"progress\">\r\n                                    <div class=\"progress-bar bg-color-blueDark\" data-smart-progressbar aria-valuenow=\"{{ settings.ProgressPercent1 }}\"  ng-style=\"{width : ( settings.ProgressPercent1 + \'%\' ) }\"></div>\r\n                                </div>\r\n                            </div>\r\n                            <div class=\"col-xs-6 col-sm-6 col-md-12 col-lg-12\"><span class=\"text\"> {{settings.ProgressName2}} : <span\r\n                                    >{{settings.ProgressValue2}} {{settings.ProgressUnit2}}</span> </span>\r\n\r\n                                <div class=\"progress\">\r\n                                    <div class=\"progress-bar bg-color-blue\" data-smart-progressbar aria-valuenow=\"{{ settings.ProgressPercent2 }}\" ng-style=\"{width : ( settings.ProgressPercent2 + \'%\' ) }\"></div>\r\n                                </div>\r\n                            </div>\r\n                            <div class=\"col-xs-6 col-sm-6 col-md-12 col-lg-12\"><span class=\"text\"> {{settings.ProgressName3}} : <span\r\n                                    >{{settings.ProgressValue3}} {{settings.ProgressUnit3}}</span> </span>\r\n\r\n                                <div class=\"progress\">\r\n                                    <div class=\"progress-bar bg-color-blue\" data-smart-progressbar aria-valuenow=\"{{ settings.ProgressPercent3 }}\" ng-style=\"{width : ( settings.ProgressPercent3 + \'%\' ) }\"></div>\r\n                                </div>\r\n                            </div>\r\n                            <div class=\"col-xs-6 col-sm-6 col-md-12 col-lg-12\"><span class=\"text\"> {{settings.ProgressName4}} : <span\r\n                                    >{{settings.ProgressValue4}} {{settings.ProgressUnit4}}</span> </span>\r\n\r\n                                <div class=\"progress\">\r\n                                    <div class=\"progress-bar bg-color-greenLight\" data-smart-progressbar ng-style=\"{width : ( settings.ProgressPercent4 + \'%\' ) }\">></div>\r\n                                </div>\r\n                            </div>\r\n\r\n                        </div>\r\n\r\n                    </div>\r\n                </div>\r\n\r\n                <div class=\"show-stat-microcharts\" data-sparkline-container data-easy-pie-chart-container>\r\n                    <div class=\"col-xs-12 col-sm-3 col-md-3 col-lg-3\">\r\n\r\n                        <div class=\"easy-pie-chart txt-color-orangeDark\" data-percent=\"0\" data-pie-size=\"50\" data-ng-model=\"PiePercent1\">\r\n                            <span class=\"percent percent-sign\">{{settings.PieValue1}}</span>\r\n                        </div>\r\n                        <span class=\"easy-pie-title\"> {{settings.PieName1}} <i class=\"fa fa-caret-up icon-color-bad\"></i> </span>\r\n                    </div>\r\n                    <div class=\"col-xs-12 col-sm-3 col-md-3 col-lg-3\">\r\n                        <div class=\"easy-pie-chart txt-color-greenLight\" data-percent=\"0\" data-pie-size=\"50\" data-ng-model=\"PiePercent2\">\r\n                            <span class=\"percent percent-sign\">{{settings.PieValue2}} </span>\r\n                        </div>\r\n                        <span class=\"easy-pie-title\"> {{settings.PieName2}} <i class=\"fa fa-caret-down icon-color-good\"></i></span>\r\n                    </div>\r\n                    <div class=\"col-xs-12 col-sm-3 col-md-3 col-lg-3\">\r\n                        <div class=\"easy-pie-chart txt-color-blue\" data-percent=\"0\" data-pie-size=\"50\" data-ng-model=\"PiePercent3\">\r\n                            <span class=\"percent percent-sign\">{{settings.PieValue3}} </span>\r\n                        </div>\r\n                        <span class=\"easy-pie-title\"> {{settings.PieName3}} <i class=\"fa fa-caret-up icon-color-good\"></i></span>\r\n                    </div>\r\n                    <div class=\"col-xs-12 col-sm-3 col-md-3 col-lg-3\">\r\n                        <div class=\"easy-pie-chart txt-color-darken\" data-percent=\"0\" data-pie-size=\"50\" data-ng-model=\"PiePercent4\">\r\n                            <span class=\"percent degree-sign\">{{settings.PieValue4}} <i class=\"fa fa-caret-up\"></i></span>\r\n                        </div>\r\n                        <span class=\"easy-pie-title\"> {{settings.PieName4}} <i\r\n                                class=\"fa fa-caret-down icon-color-good\"></i></span>\r\n                    </div>\r\n                </div>\r\n\r\n            </div>\r\n            <!-- end s1 tab pane -->\r\n\r\n            <div ng-class=\"!showMonitor? \'tab-pane fade active in padding-10 no-padding-bottom\' : \'tab-pane fade\'\" id=\"s2\">\r\n                <div class=\"padding-10\">\r\n                    <form name=\"ebaasform\" novalidate\">\r\n                        <ebaas-form-template dbschema=\"dbschema\" dbclass=\"dbclass\" oid=\"oid\" template=\"template\" formattribute=\"formAttribute\" readonly=\"true\"></ebaas-form-template>\r\n                    </form>\r\n                </div>\r\n            </div>\r\n            <!-- end s2 tab pane -->\r\n\r\n            <div ng-class=\"\'tab-pane fade\'\" id=\"s3\">\r\n                <div class=\"padding-10\">\r\n                    <div dx-scheduler=\"schedulerOptions\"></div>\r\n                </div>\r\n            </div>\r\n            <!-- end s3 tab pane -->\r\n        </div>\r\n\r\n        <!-- end content -->\r\n    </div>\r\n\r\n</div>\r\n<!-- end widget div -->\r\n</div>\r\n");
$templateCache.put("app/dashboard/chat/directives/aside-chat-widget.tpl.html","<ul>\r\n    <li>\r\n        <div class=\"display-users\">\r\n            <input class=\"form-control chat-user-filter\" placeholder=\"Filter\" type=\"text\">\r\n            <dl>\r\n                <dt>\r\n                    <a href=\"#\" class=\"usr\"\r\n                       data-chat-id=\"cha1\"\r\n                       data-chat-fname=\"Sadi\"\r\n                       data-chat-lname=\"Orlaf\"\r\n                       data-chat-status=\"busy\"\r\n                       data-chat-alertmsg=\"Sadi Orlaf is in a meeting. Please do not disturb!\"\r\n                       data-chat-alertshow=\"true\"\r\n                       popover-trigger=\"mouseenter\"\r\n                       popover-placement=\"right\"\r\n                       popover=\"\r\n										<div class=\'usr-card\'>\r\n											<img src=\'styles/img/avatars/5.png\' alt=\'Sadi Orlaf\'>\r\n											<div class=\'usr-card-content\'>\r\n												<h3>Sadi Orlaf</h3>\r\n												<p>Marketing Executive</p>\r\n											</div>\r\n										</div>\r\n									\">\r\n                        <i></i>Sadi Orlaf\r\n                    </a>\r\n                </dt>\r\n                <dt>\r\n                    <a href=\"#\" class=\"usr\"\r\n                       data-chat-id=\"cha2\"\r\n                       data-chat-fname=\"Jessica\"\r\n                       data-chat-lname=\"Dolof\"\r\n                       data-chat-status=\"online\"\r\n                       data-chat-alertmsg=\"\"\r\n                       data-chat-alertshow=\"false\"\r\n                       popover-trigger=\"mouseenter\"\r\n                       popover-placement=\"right\"\r\n                       popover=\"\r\n										<div class=\'usr-card\'>\r\n											<img src=\'styles/img/avatars/1.png\' alt=\'Jessica Dolof\'>\r\n											<div class=\'usr-card-content\'>\r\n												<h3>Jessica Dolof</h3>\r\n												<p>Sales Administrator</p>\r\n											</div>\r\n										</div>\r\n									\">\r\n                        <i></i>Jessica Dolof\r\n                    </a>\r\n                </dt>\r\n                <dt>\r\n                    <a href=\"#\" class=\"usr\"\r\n                       data-chat-id=\"cha3\"\r\n                       data-chat-fname=\"Zekarburg\"\r\n                       data-chat-lname=\"Almandalie\"\r\n                       data-chat-status=\"online\"\r\n                       popover-trigger=\"mouseenter\"\r\n                       popover-placement=\"right\"\r\n                       popover=\"\r\n										<div class=\'usr-card\'>\r\n											<img src=\'styles/img/avatars/3.png\' alt=\'Zekarburg Almandalie\'>\r\n											<div class=\'usr-card-content\'>\r\n												<h3>Zekarburg Almandalie</h3>\r\n												<p>Sales Admin</p>\r\n											</div>\r\n										</div>\r\n									\">\r\n                        <i></i>Zekarburg Almandalie\r\n                    </a>\r\n                </dt>\r\n                <dt>\r\n                    <a href=\"#\" class=\"usr\"\r\n                       data-chat-id=\"cha4\"\r\n                       data-chat-fname=\"Barley\"\r\n                       data-chat-lname=\"Krazurkth\"\r\n                       data-chat-status=\"away\"\r\n                       popover-trigger=\"mouseenter\"\r\n                       popover-placement=\"right\"\r\n                       popover=\"\r\n										<div class=\'usr-card\'>\r\n											<img src=\'styles/img/avatars/4.png\' alt=\'Barley Krazurkth\'>\r\n											<div class=\'usr-card-content\'>\r\n												<h3>Barley Krazurkth</h3>\r\n												<p>Sales Director</p>\r\n											</div>\r\n										</div>\r\n									\">\r\n                        <i></i>Barley Krazurkth\r\n                    </a>\r\n                </dt>\r\n                <dt>\r\n                    <a href=\"#\" class=\"usr offline\"\r\n                       data-chat-id=\"cha5\"\r\n                       data-chat-fname=\"Farhana\"\r\n                       data-chat-lname=\"Amrin\"\r\n                       data-chat-status=\"incognito\"\r\n                       popover-trigger=\"mouseenter\"\r\n                       popover-placement=\"right\"\r\n                       popover=\"\r\n										<div class=\'usr-card\'>\r\n											<img src=\'styles/img/avatars/female.png\' alt=\'Farhana Amrin\'>\r\n											<div class=\'usr-card-content\'>\r\n												<h3>Farhana Amrin</h3>\r\n												<p>Support Admin <small><i class=\'fa fa-music\'></i> Playing Beethoven Classics</small></p>\r\n											</div>\r\n										</div>\r\n									\">\r\n                        <i></i>Farhana Amrin (offline)\r\n                    </a>\r\n                </dt>\r\n                <dt>\r\n                    <a href=\"#\" class=\"usr offline\"\r\n                       data-chat-id=\"cha6\"\r\n                       data-chat-fname=\"Lezley\"\r\n                       data-chat-lname=\"Jacob\"\r\n                       data-chat-status=\"incognito\"\r\n                       popover-trigger=\"mouseenter\"\r\n                       popover-placement=\"right\"\r\n                       popover=\"\r\n										<div class=\'usr-card\'>\r\n											<img src=\'styles/img/avatars/male.png\' alt=\'Lezley Jacob\'>\r\n											<div class=\'usr-card-content\'>\r\n												<h3>Lezley Jacob</h3>\r\n												<p>Sales Director</p>\r\n											</div>\r\n										</div>\r\n									\">\r\n                        <i></i>Lezley Jacob (offline)\r\n                    </a>\r\n                </dt>\r\n            </dl>\r\n\r\n\r\n            <!--<a href=\"chat.html\" class=\"btn btn-xs btn-default btn-block sa-chat-learnmore-btn\">About the API</a>-->\r\n        </div>\r\n    </li>\r\n</ul>");
$templateCache.put("app/dashboard/chat/directives/chat-users.tpl.html","<div id=\"chat-container\" ng-class=\"{open: open}\">\r\n    <span class=\"chat-list-open-close\" ng-click=\"openToggle()\"><i class=\"fa fa-user\"></i><b>!</b></span>\r\n\r\n    <div class=\"chat-list-body custom-scroll\">\r\n        <ul id=\"chat-users\">\r\n            <li ng-repeat=\"chatUser in chatUsers | filter: chatUserFilter\">\r\n                <a ng-click=\"messageTo(chatUser)\"><img ng-src=\"{{chatUser.picture}}\">{{chatUser.username}} <span\r\n                        class=\"badge badge-inverse\">{{chatUser.username.length}}</span><span class=\"state\"><i\r\n                        class=\"fa fa-circle txt-color-green pull-right\"></i></span></a>\r\n            </li>\r\n        </ul>\r\n    </div>\r\n    <div class=\"chat-list-footer\">\r\n        <div class=\"control-group\">\r\n            <form class=\"smart-form\">\r\n                <section>\r\n                    <label class=\"input\" >\r\n                        <input type=\"text\" ng-model=\"chatUserFilter\" id=\"filter-chat-list\" placeholder=\"Filter\">\r\n                    </label>\r\n                </section>\r\n            </form>\r\n        </div>\r\n    </div>\r\n</div>");
$templateCache.put("app/dashboard/chat/directives/chat-widget.tpl.html","<div id=\"chat-widget\" jarvis-widget data-widget-color=\"blueDark\" data-widget-editbutton=\"false\"\r\n     data-widget-fullscreenbutton=\"false\">\r\n\r\n\r\n    <header>\r\n        <span class=\"widget-icon\"> <i class=\"fa fa-comments txt-color-white\"></i> </span>\r\n\r\n        <h2> SmartMessage </h2>\r\n\r\n        <div class=\"widget-toolbar\">\r\n            <!-- add: non-hidden - to disable auto hide -->\r\n\r\n            <div class=\"btn-group\" data-dropdown>\r\n                <button class=\"btn dropdown-toggle btn-xs btn-success\" dropdown-toggle>\r\n                    Status <i class=\"fa fa-caret-down\"></i>\r\n                </button>\r\n                <ul class=\"dropdown-menu pull-right js-status-update\">\r\n                    <li>\r\n                        <a href-void><i class=\"fa fa-circle txt-color-green\"></i> Online</a>\r\n                    </li>\r\n                    <li>\r\n                        <a href-void><i class=\"fa fa-circle txt-color-red\"></i> Busy</a>\r\n                    </li>\r\n                    <li>\r\n                        <a href-void><i class=\"fa fa-circle txt-color-orange\"></i> Away</a>\r\n                    </li>\r\n                    <li class=\"divider\"></li>\r\n                    <li>\r\n                        <a href-void><i class=\"fa fa-power-off\"></i> Log Off</a>\r\n                    </li>\r\n                </ul>\r\n            </div>\r\n        </div>\r\n    </header>\r\n\r\n    <!-- widget div-->\r\n    <div>\r\n        <div class=\"widget-body widget-hide-overflow no-padding\">\r\n            <!-- content goes here -->\r\n\r\n            <chat-users></chat-users>\r\n\r\n            <!-- CHAT BODY -->\r\n            <div id=\"chat-body\" class=\"chat-body custom-scroll\">\r\n                <ul>\r\n                    <li class=\"message\" ng-repeat=\"message in chatMessages\">\r\n                        <img class=\"message-picture online\" ng-src=\"{{message.user.picture}}\">\r\n\r\n                        <div class=\"message-text\">\r\n                            <time>\r\n                                {{message.date | date }}\r\n                            </time>\r\n                            <a ng-click=\"messageTo(message.user)\" class=\"username\">{{message.user.username}}</a>\r\n                            <div ng-bind-html=\"message.body\"></div>\r\n\r\n                        </div>\r\n                    </li>\r\n                </ul>\r\n            </div>\r\n\r\n            <!-- CHAT FOOTER -->\r\n            <div class=\"chat-footer\">\r\n\r\n                <!-- CHAT TEXTAREA -->\r\n                <div class=\"textarea-div\">\r\n\r\n                    <div class=\"typearea\">\r\n                        <textarea placeholder=\"Write a reply...\" id=\"textarea-expand\"\r\n                                  class=\"custom-scroll\" ng-model=\"newMessage\"></textarea>\r\n                    </div>\r\n\r\n                </div>\r\n\r\n                <!-- CHAT REPLY/SEND -->\r\n											<span class=\"textarea-controls\">\r\n												<button class=\"btn btn-sm btn-primary pull-right\" ng-click=\"sendMessage()\">\r\n                                                    Reply\r\n                                                </button> <span class=\"pull-right smart-form\"\r\n                                                                style=\"margin-top: 3px; margin-right: 10px;\"> <label\r\n                                                    class=\"checkbox pull-right\">\r\n                                                <input type=\"checkbox\" name=\"subscription\" id=\"subscription\">\r\n                                                <i></i>Press <strong> ENTER </strong> to send </label> </span> <a\r\n                                                    href-void class=\"pull-left\"><i\r\n                                                    class=\"fa fa-camera fa-fw fa-lg\"></i></a> </span>\r\n\r\n            </div>\r\n\r\n            <!-- end content -->\r\n        </div>\r\n\r\n    </div>\r\n    <!-- end widget div -->\r\n</div>");
$templateCache.put("app/dashboard/todo/directives/todo-list.tpl.html","<div>\r\n    <h5 class=\"todo-group-title\"><i class=\"fa fa-{{icon}}\"></i> {{title}} (\r\n        <small class=\"num-of-tasks\">{{scopeItems.length}}</small>\r\n        )\r\n    </h5>\r\n    <ul class=\"todo\">\r\n        <li ng-class=\"{complete: todo.completedAt}\" ng-repeat=\"todo in todos | orderBy: todo._id | filter: filter  track by todo._id\" >\r\n    	<span class=\"handle\"> <label class=\"checkbox\">\r\n            <input type=\"checkbox\" ng-click=\"todo.toggle()\" ng-checked=\"todo.completedAt\"\r\n                   name=\"checkbox-inline\">\r\n            <i></i> </label> </span>\r\n\r\n            <p>\r\n                <strong>Ticket #{{$index + 1}}</strong> - {{todo.title}}\r\n                <span class=\"text-muted\" ng-if=\"todo.description\">{{todo.description}}</span>\r\n                <span class=\"date\">{{todo.createdAt | date}} &dash; <a ng-click=\"deleteTodo(todo)\" class=\"text-muted\"><i\r\n                        class=\"fa fa-trash\"></i></a></span>\r\n\r\n            </p>\r\n        </li>\r\n    </ul>\r\n</div>");
$templateCache.put("app/_common/forms/directives/bootstrap-validation/bootstrap-attribute-form.tpl.html","<form id=\"attributeForm\" class=\"form-horizontal\"\r\n      data-bv-message=\"This value is not valid\"\r\n      data-bv-feedbackicons-valid=\"glyphicon glyphicon-ok\"\r\n      data-bv-feedbackicons-invalid=\"glyphicon glyphicon-remove\"\r\n      data-bv-feedbackicons-validating=\"glyphicon glyphicon-refresh\">\r\n\r\n    <fieldset>\r\n        <legend>\r\n            Set validator options via HTML attributes\r\n        </legend>\r\n\r\n        <div class=\"alert alert-warning\">\r\n            <code>&lt; input\r\n                data-bv-validatorname\r\n                data-bv-validatorname-validatoroption=\"...\" / &gt;</code>\r\n\r\n            <br>\r\n            <br>\r\n            More validator options can be found here:\r\n            <a href=\"http://bootstrapvalidator.com/validators/\" target=\"_blank\">http://bootstrapvalidator.com/validators/</a>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            <label class=\"col-lg-3 control-label\">Full name</label>\r\n            <div class=\"col-lg-4\">\r\n                <input type=\"text\" class=\"form-control\" name=\"firstName\" placeholder=\"First name\"\r\n                       data-bv-notempty=\"true\"\r\n                       data-bv-notempty-message=\"The first name is required and cannot be empty\" />\r\n            </div>\r\n            <div class=\"col-lg-4\">\r\n                <input type=\"text\" class=\"form-control\" name=\"lastName\" placeholder=\"Last name\"\r\n                       data-bv-notempty=\"true\"\r\n                       data-bv-notempty-message=\"The last name is required and cannot be empty\" />\r\n            </div>\r\n        </div>\r\n    </fieldset>\r\n\r\n    <fieldset>\r\n        <div class=\"form-group\">\r\n            <label class=\"col-lg-3 control-label\">Username</label>\r\n            <div class=\"col-lg-5\">\r\n                <input type=\"text\" class=\"form-control\" name=\"username\"\r\n                       data-bv-message=\"The username is not valid\"\r\n\r\n                       data-bv-notempty=\"true\"\r\n                       data-bv-notempty-message=\"The username is required and cannot be empty\"\r\n\r\n                       data-bv-regexp=\"true\"\r\n                       data-bv-regexp-regexp=\"^[a-zA-Z0-9_\\.]+$\"\r\n                       data-bv-regexp-message=\"The username can only consist of alphabetical, number, dot and underscore\"\r\n\r\n                       data-bv-stringlength=\"true\"\r\n                       data-bv-stringlength-min=\"6\"\r\n                       data-bv-stringlength-max=\"30\"\r\n                       data-bv-stringlength-message=\"The username must be more than 6 and less than 30 characters long\"\r\n\r\n                       data-bv-different=\"true\"\r\n                       data-bv-different-field=\"password\"\r\n                       data-bv-different-message=\"The username and password cannot be the same as each other\" />\r\n            </div>\r\n        </div>\r\n    </fieldset>\r\n\r\n    <fieldset>\r\n        <div class=\"form-group\">\r\n            <label class=\"col-lg-3 control-label\">Email address</label>\r\n            <div class=\"col-lg-5\">\r\n                <input class=\"form-control\" name=\"email\" type=\"email\"\r\n                       data-bv-emailaddress=\"true\"\r\n                       data-bv-emailaddress-message=\"The input is not a valid email address\" />\r\n            </div>\r\n        </div>\r\n    </fieldset>\r\n\r\n    <fieldset>\r\n        <div class=\"form-group\">\r\n            <label class=\"col-lg-3 control-label\">Password</label>\r\n            <div class=\"col-lg-5\">\r\n                <input type=\"password\" class=\"form-control\" name=\"password\"\r\n                       data-bv-notempty=\"true\"\r\n                       data-bv-notempty-message=\"The password is required and cannot be empty\"\r\n\r\n                       data-bv-identical=\"true\"\r\n                       data-bv-identical-field=\"confirmPassword\"\r\n                       data-bv-identical-message=\"The password and its confirm are not the same\"\r\n\r\n                       data-bv-different=\"true\"\r\n                       data-bv-different-field=\"username\"\r\n                       data-bv-different-message=\"The password cannot be the same as username\" />\r\n            </div>\r\n        </div>\r\n    </fieldset>\r\n\r\n    <fieldset>\r\n        <div class=\"form-group\">\r\n            <label class=\"col-lg-3 control-label\">Retype password</label>\r\n            <div class=\"col-lg-5\">\r\n                <input type=\"password\" class=\"form-control\" name=\"confirmPassword\"\r\n                       data-bv-notempty=\"true\"\r\n                       data-bv-notempty-message=\"The confirm password is required and cannot be empty\"\r\n\r\n                       data-bv-identical=\"true\"\r\n                       data-bv-identical-field=\"password\"\r\n                       data-bv-identical-message=\"The password and its confirm are not the same\"\r\n\r\n                       data-bv-different=\"true\"\r\n                       data-bv-different-field=\"username\"\r\n                       data-bv-different-message=\"The password cannot be the same as username\" />\r\n            </div>\r\n        </div>\r\n    </fieldset>\r\n\r\n    <fieldset>\r\n        <div class=\"form-group\">\r\n            <label class=\"col-lg-3 control-label\">Languages</label>\r\n            <div class=\"col-lg-5\">\r\n                <div class=\"checkbox\">\r\n                    <label>\r\n                        <input type=\"checkbox\" name=\"languages[]\" value=\"english\"\r\n                               data-bv-message=\"Please specify at least one language you can speak\"\r\n                               data-bv-notempty=\"true\" />\r\n                        English </label>\r\n                </div>\r\n                <div class=\"checkbox\">\r\n                    <label>\r\n                        <input type=\"checkbox\" name=\"languages[]\" value=\"french\" />\r\n                        French </label>\r\n                </div>\r\n                <div class=\"checkbox\">\r\n                    <label>\r\n                        <input type=\"checkbox\" name=\"languages[]\" value=\"german\" />\r\n                        German </label>\r\n                </div>\r\n                <div class=\"checkbox\">\r\n                    <label>\r\n                        <input type=\"checkbox\" name=\"languages[]\" value=\"russian\" />\r\n                        Russian </label>\r\n                </div>\r\n                <div class=\"checkbox\">\r\n                    <label>\r\n                        <input type=\"checkbox\" name=\"languages[]\" value=\"other\" />\r\n                        Other </label>\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </fieldset>\r\n\r\n    <div class=\"form-actions\">\r\n        <div class=\"row\">\r\n            <div class=\"col-md-12\">\r\n                <button class=\"btn btn-default\" type=\"submit\">\r\n                    <i class=\"fa fa-eye\"></i>\r\n                    Validate\r\n                </button>\r\n            </div>\r\n        </div>\r\n    </div>\r\n\r\n</form>\r\n     ");
$templateCache.put("app/_common/forms/directives/bootstrap-validation/bootstrap-button-group-form.tpl.html","<form id=\"buttonGroupForm\" method=\"post\" class=\"form-horizontal\">\r\n\r\n    <fieldset>\r\n        <legend>\r\n            Default Form Elements\r\n        </legend>\r\n        <div class=\"form-group\">\r\n            <label class=\"col-lg-3 control-label\">Gender</label>\r\n            <div class=\"col-lg-9\">\r\n                <div class=\"btn-group\" data-toggle=\"buttons\">\r\n                    <label class=\"btn btn-default\">\r\n                        <input type=\"radio\" name=\"gender\" value=\"male\" />\r\n                        Male </label>\r\n                    <label class=\"btn btn-default\">\r\n                        <input type=\"radio\" name=\"gender\" value=\"female\" />\r\n                        Female </label>\r\n                    <label class=\"btn btn-default\">\r\n                        <input type=\"radio\" name=\"gender\" value=\"other\" />\r\n                        Other </label>\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </fieldset>\r\n\r\n    <fieldset>\r\n        <div class=\"form-group\">\r\n            <label class=\"col-lg-3 control-label\">Languages</label>\r\n            <div class=\"col-lg-9\">\r\n                <div class=\"btn-group\" data-toggle=\"buttons\">\r\n                    <label class=\"btn btn-default\">\r\n                        <input type=\"checkbox\" name=\"languages[]\" value=\"english\" />\r\n                        English </label>\r\n                    <label class=\"btn btn-default\">\r\n                        <input type=\"checkbox\" name=\"languages[]\" value=\"german\" />\r\n                        German </label>\r\n                    <label class=\"btn btn-default\">\r\n                        <input type=\"checkbox\" name=\"languages[]\" value=\"french\" />\r\n                        French </label>\r\n                    <label class=\"btn btn-default\">\r\n                        <input type=\"checkbox\" name=\"languages[]\" value=\"russian\" />\r\n                        Russian </label>\r\n                    <label class=\"btn btn-default\">\r\n                        <input type=\"checkbox\" name=\"languages[]\" value=\"italian\">\r\n                        Italian </label>\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </fieldset>\r\n\r\n    <div class=\"form-actions\">\r\n        <div class=\"row\">\r\n            <div class=\"col-md-12\">\r\n                <button class=\"btn btn-default\" type=\"submit\">\r\n                    <i class=\"fa fa-eye\"></i>\r\n                    Validate\r\n                </button>\r\n            </div>\r\n        </div>\r\n    </div>\r\n\r\n</form>\r\n");
$templateCache.put("app/_common/forms/directives/bootstrap-validation/bootstrap-contact-form.tpl.html","<form id=\"contactForm\" method=\"post\" class=\"form-horizontal\">\r\n\r\n    <fieldset>\r\n        <legend>Showing messages in custom area</legend>\r\n        <div class=\"form-group\">\r\n            <label class=\"col-md-3 control-label\">Full name</label>\r\n            <div class=\"col-md-6\">\r\n                <input type=\"text\" class=\"form-control\" name=\"fullName\" />\r\n            </div>\r\n        </div>\r\n    </fieldset>\r\n\r\n    <fieldset>\r\n        <div class=\"form-group\">\r\n            <label class=\"col-md-3 control-label\">Email</label>\r\n            <div class=\"col-md-6\">\r\n                <input type=\"text\" class=\"form-control\" name=\"email\" />\r\n            </div>\r\n        </div>\r\n    </fieldset>\r\n\r\n    <fieldset>\r\n        <div class=\"form-group\">\r\n            <label class=\"col-md-3 control-label\">Title</label>\r\n            <div class=\"col-md-6\">\r\n                <input type=\"text\" class=\"form-control\" name=\"title\" />\r\n            </div>\r\n        </div>\r\n    </fieldset>\r\n\r\n    <fieldset>\r\n        <div class=\"form-group\">\r\n            <label class=\"col-md-3 control-label\">Content</label>\r\n            <div class=\"col-md-6\">\r\n                <textarea class=\"form-control\" name=\"content\" rows=\"5\"></textarea>\r\n            </div>\r\n        </div>\r\n    </fieldset>\r\n\r\n    <fieldset>\r\n        <!-- #messages is where the messages are placed inside -->\r\n        <div class=\"form-group\">\r\n            <div class=\"col-md-9 col-md-offset-3\">\r\n                <div id=\"messages\"></div>\r\n            </div>\r\n        </div>\r\n    </fieldset>\r\n\r\n    <div class=\"form-actions\">\r\n        <div class=\"row\">\r\n            <div class=\"col-md-12\">\r\n                <button class=\"btn btn-default\" type=\"submit\">\r\n                    <i class=\"fa fa-eye\"></i>\r\n                    Validate\r\n                </button>\r\n            </div>\r\n        </div>\r\n    </div>\r\n\r\n</form>\r\n");
$templateCache.put("app/_common/forms/directives/bootstrap-validation/bootstrap-movie-form.tpl.html","\r\n<form id=\"movieForm\" method=\"post\">\r\n\r\n    <fieldset>\r\n        <legend>\r\n            Default Form Elements\r\n        </legend>\r\n        <div class=\"form-group\">\r\n            <div class=\"row\">\r\n                <div class=\"col-md-8\">\r\n                    <label class=\"control-label\">Movie title</label>\r\n                    <input type=\"text\" class=\"form-control\" name=\"title\" />\r\n                </div>\r\n\r\n                <div class=\"col-md-4 selectContainer\">\r\n                    <label class=\"control-label\">Genre</label>\r\n                    <select class=\"form-control\" name=\"genre\">\r\n                        <option value=\"\">Choose a genre</option>\r\n                        <option value=\"action\">Action</option>\r\n                        <option value=\"comedy\">Comedy</option>\r\n                        <option value=\"horror\">Horror</option>\r\n                        <option value=\"romance\">Romance</option>\r\n                    </select>\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </fieldset>\r\n\r\n    <fieldset>\r\n        <div class=\"form-group\">\r\n            <div class=\"row\">\r\n                <div class=\"col-sm-12 col-md-4\">\r\n                    <label class=\"control-label\">Director</label>\r\n                    <input type=\"text\" class=\"form-control\" name=\"director\" />\r\n                </div>\r\n\r\n                <div class=\"col-sm-12 col-md-4\">\r\n                    <label class=\"control-label\">Writer</label>\r\n                    <input type=\"text\" class=\"form-control\" name=\"writer\" />\r\n                </div>\r\n\r\n                <div class=\"col-sm-12 col-md-4\">\r\n                    <label class=\"control-label\">Producer</label>\r\n                    <input type=\"text\" class=\"form-control\" name=\"producer\" />\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </fieldset>\r\n\r\n    <fieldset>\r\n        <div class=\"form-group\">\r\n            <div class=\"row\">\r\n                <div class=\"col-sm-12 col-md-6\">\r\n                    <label class=\"control-label\">Website</label>\r\n                    <input type=\"text\" class=\"form-control\" name=\"website\" />\r\n                </div>\r\n\r\n                <div class=\"col-sm-12 col-md-6\">\r\n                    <label class=\"control-label\">Youtube trailer</label>\r\n                    <input type=\"text\" class=\"form-control\" name=\"trailer\" />\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </fieldset>\r\n\r\n    <fieldset>\r\n        <div class=\"form-group\">\r\n            <label class=\"control-label\">Review</label>\r\n            <textarea class=\"form-control\" name=\"review\" rows=\"8\"></textarea>\r\n        </div>\r\n    </fieldset>\r\n\r\n    <fieldset>\r\n        <div class=\"form-group\">\r\n\r\n            <div class=\"row\">\r\n                <div class=\"col-sm-12 col-md-12\">\r\n                    <label class=\"control-label\">Rating</label>\r\n                </div>\r\n\r\n                <div class=\"col-sm-12 col-md-10\">\r\n\r\n                    <label class=\"radio radio-inline no-margin\">\r\n                        <input type=\"radio\" name=\"rating\" value=\"terrible\" class=\"radiobox style-2\" />\r\n                        <span>Terrible</span> </label>\r\n\r\n                    <label class=\"radio radio-inline\">\r\n                        <input type=\"radio\" name=\"rating\" value=\"watchable\" class=\"radiobox style-2\" />\r\n                        <span>Watchable</span> </label>\r\n                    <label class=\"radio radio-inline\">\r\n                        <input type=\"radio\" name=\"rating\" value=\"best\" class=\"radiobox style-2\" />\r\n                        <span>Best ever</span> </label>\r\n\r\n                </div>\r\n\r\n            </div>\r\n\r\n        </div>\r\n    </fieldset>\r\n\r\n    <div class=\"form-actions\">\r\n        <div class=\"row\">\r\n            <div class=\"col-md-12\">\r\n                <button class=\"btn btn-default\" type=\"submit\">\r\n                    <i class=\"fa fa-eye\"></i>\r\n                    Validate\r\n                </button>\r\n            </div>\r\n        </div>\r\n    </div>\r\n\r\n</form>\r\n\r\n ");
$templateCache.put("app/_common/forms/directives/bootstrap-validation/bootstrap-product-form.tpl.html","<form id=\"productForm\" class=\"form-horizontal\">\r\n\r\n    <fieldset>\r\n        <legend>\r\n            Default Form Elements\r\n        </legend>\r\n        <div class=\"form-group\">\r\n            <label class=\"col-xs-2 col-lg-3 control-label\">Price</label>\r\n            <div class=\"col-xs-9 col-lg-6 inputGroupContainer\">\r\n                <div class=\"input-group\">\r\n                    <input type=\"text\" class=\"form-control\" name=\"price\" />\r\n                    <span class=\"input-group-addon\">$</span>\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </fieldset>\r\n\r\n    <fieldset>\r\n        <div class=\"form-group\">\r\n            <label class=\"col-xs-2 col-lg-3 control-label\">Amount</label>\r\n            <div class=\"col-xs-9 col-lg-6 inputGroupContainer\">\r\n                <div class=\"input-group\">\r\n                    <span class=\"input-group-addon\">&#8364;</span>\r\n                    <input type=\"text\" class=\"form-control\" name=\"amount\" />\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </fieldset>\r\n\r\n    <fieldset>\r\n        <div class=\"form-group\">\r\n            <label class=\"col-xs-2 col-lg-3 control-label\">Color</label>\r\n            <div class=\"col-xs-9 col-lg-6 selectContainer\">\r\n                <select class=\"form-control\" name=\"color\">\r\n                    <option value=\"\">Choose a color</option>\r\n                    <option value=\"blue\">Blue</option>\r\n                    <option value=\"green\">Green</option>\r\n                    <option value=\"red\">Red</option>\r\n                    <option value=\"yellow\">Yellow</option>\r\n                    <option value=\"white\">White</option>\r\n                </select>\r\n            </div>\r\n        </div>\r\n    </fieldset>\r\n\r\n    <fieldset>\r\n        <div class=\"form-group\">\r\n            <label class=\"col-xs-2 col-lg-3 control-label\">Size</label>\r\n            <div class=\"col-xs-9 col-lg-6 selectContainer\">\r\n                <select class=\"form-control\" name=\"size\">\r\n                    <option value=\"\">Choose a size</option>\r\n                    <option value=\"S\">S</option>\r\n                    <option value=\"M\">M</option>\r\n                    <option value=\"L\">L</option>\r\n                    <option value=\"XL\">XL</option>\r\n                </select>\r\n            </div>\r\n        </div>\r\n    </fieldset>\r\n\r\n    <div class=\"form-actions\">\r\n        <div class=\"row\">\r\n            <div class=\"col-md-12\">\r\n                <button class=\"btn btn-default\" type=\"submit\">\r\n                    <i class=\"fa fa-eye\"></i>\r\n                    Validate\r\n                </button>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</form>\r\n\r\n");
$templateCache.put("app/_common/forms/directives/bootstrap-validation/bootstrap-profile-form.tpl.html","<form id=\"profileForm\">\r\n\r\n    <fieldset>\r\n        <legend>\r\n            Default Form Elements\r\n        </legend>\r\n        <div class=\"form-group\">\r\n            <label>Email address</label>\r\n            <input type=\"text\" class=\"form-control\" name=\"email\" />\r\n        </div>\r\n    </fieldset>\r\n    <fieldset>\r\n        <div class=\"form-group\">\r\n            <label>Password</label>\r\n            <input type=\"password\" class=\"form-control\" name=\"password\" />\r\n        </div>\r\n    </fieldset>\r\n\r\n    <div class=\"form-actions\">\r\n        <div class=\"row\">\r\n            <div class=\"col-md-12\">\r\n                <button class=\"btn btn-default\" type=\"submit\">\r\n                    <i class=\"fa fa-eye\"></i>\r\n                    Validate\r\n                </button>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</form>\r\n");
$templateCache.put("app/_common/forms/directives/bootstrap-validation/bootstrap-toggling-form.tpl.html","<form id=\"togglingForm\" method=\"post\" class=\"form-horizontal\">\r\n\r\n    <fieldset>\r\n        <legend>\r\n            Default Form Elements\r\n        </legend>\r\n        <div class=\"form-group\">\r\n            <label class=\"col-lg-3 control-label\">Full name <sup>*</sup></label>\r\n            <div class=\"col-lg-4\">\r\n                <input type=\"text\" class=\"form-control\" name=\"firstName\" placeholder=\"First name\" />\r\n            </div>\r\n            <div class=\"col-lg-4\">\r\n                <input type=\"text\" class=\"form-control\" name=\"lastName\" placeholder=\"Last name\" />\r\n            </div>\r\n        </div>\r\n    </fieldset>\r\n\r\n    <fieldset>\r\n        <div class=\"form-group\">\r\n            <label class=\"col-lg-3 control-label\">Company <sup>*</sup></label>\r\n            <div class=\"col-lg-5\">\r\n                <input type=\"text\" class=\"form-control\" name=\"company\"\r\n                       required data-bv-notempty-message=\"The company name is required\" />\r\n            </div>\r\n            <div class=\"col-lg-2\">\r\n                <button type=\"button\" class=\"btn btn-info btn-sm\" data-toggle=\"#jobInfo\">\r\n                    Add more info\r\n                </button>\r\n            </div>\r\n        </div>\r\n    </fieldset>\r\n\r\n    <!-- These fields will not be validated as long as they are not visible -->\r\n    <div id=\"jobInfo\" style=\"display: none;\">\r\n        <fieldset>\r\n            <div class=\"form-group\">\r\n                <label class=\"col-lg-3 control-label\">Job title <sup>*</sup></label>\r\n                <div class=\"col-lg-5\">\r\n                    <input type=\"text\" class=\"form-control\" name=\"job\" />\r\n                </div>\r\n            </div>\r\n        </fieldset>\r\n\r\n        <fieldset>\r\n            <div class=\"form-group\">\r\n                <label class=\"col-lg-3 control-label\">Department <sup>*</sup></label>\r\n                <div class=\"col-lg-5\">\r\n                    <input type=\"text\" class=\"form-control\" name=\"department\" />\r\n                </div>\r\n            </div>\r\n        </fieldset>\r\n    </div>\r\n\r\n    <fieldset>\r\n        <div class=\"form-group\">\r\n            <label class=\"col-lg-3 control-label\">Mobile phone <sup>*</sup></label>\r\n            <div class=\"col-lg-5\">\r\n                <input type=\"text\" class=\"form-control\" name=\"mobilePhone\" />\r\n            </div>\r\n            <div class=\"col-lg-2\">\r\n                <button type=\"button\" class=\"btn btn-info btn-sm\" data-toggle=\"#phoneInfo\">\r\n                    Add more phone numbers\r\n                </button>\r\n            </div>\r\n        </div>\r\n    </fieldset>\r\n    <!-- These fields will not be validated as long as they are not visible -->\r\n    <div id=\"phoneInfo\" style=\"display: none;\">\r\n\r\n        <fieldset>\r\n            <div class=\"form-group\">\r\n                <label class=\"col-lg-3 control-label\">Home phone</label>\r\n                <div class=\"col-lg-5\">\r\n                    <input type=\"text\" class=\"form-control\" name=\"homePhone\" />\r\n                </div>\r\n            </div>\r\n        </fieldset>\r\n        <fieldset>\r\n            <div class=\"form-group\">\r\n                <label class=\"col-lg-3 control-label\">Office phone</label>\r\n                <div class=\"col-lg-5\">\r\n                    <input type=\"text\" class=\"form-control\" name=\"officePhone\" />\r\n                </div>\r\n            </div>\r\n        </fieldset>\r\n    </div>\r\n\r\n    <div class=\"form-actions\">\r\n        <div class=\"row\">\r\n            <div class=\"col-md-12\">\r\n                <button class=\"btn btn-default\" type=\"submit\">\r\n                    <i class=\"fa fa-eye\"></i>\r\n                    Validate\r\n                </button>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</form>");
$templateCache.put("app/_common/layout/directives/demo/demo-states.tpl.html","<div class=\"demo\"><span id=\"demo-setting\"><i class=\"fa fa-cog txt-color-blueDark\"></i></span>\r\n\r\n    <form>\r\n        <legend class=\"no-padding margin-bottom-10\">Layout Options</legend>\r\n        <section>\r\n            <label><input type=\"checkbox\" ng-model=\"fixedHeader\"\r\n                          class=\"checkbox style-0\"><span>Fixed Header</span></label>\r\n            <label><input type=\"checkbox\"\r\n                          ng-model=\"fixedNavigation\"\r\n                          class=\"checkbox style-0\"><span>Fixed Navigation</span></label>\r\n            <label><input type=\"checkbox\"\r\n                          ng-model=\"fixedRibbon\"\r\n                          class=\"checkbox style-0\"><span>Fixed Ribbon</span></label>\r\n            <label><input type=\"checkbox\"\r\n                          ng-model=\"fixedPageFooter\"\r\n                          class=\"checkbox style-0\"><span>Fixed Footer</span></label>\r\n            <label><input type=\"checkbox\"\r\n                          ng-model=\"insideContainer\"\r\n                          class=\"checkbox style-0\"><span>Inside <b>.container</b></span></label>\r\n            <label><input type=\"checkbox\"\r\n                          ng-model=\"rtl\"\r\n                          class=\"checkbox style-0\"><span>RTL</span></label>\r\n            <label><input type=\"checkbox\"\r\n                          ng-model=\"menuOnTop\"\r\n                          class=\"checkbox style-0\"><span>Menu on <b>top</b></span></label>\r\n            <label><input type=\"checkbox\"\r\n                          ng-model=\"colorblindFriendly\"\r\n                          class=\"checkbox style-0\"><span>For Colorblind <div\r\n                    class=\"font-xs text-right\">(experimental)\r\n            </div></span>\r\n            </label><span id=\"smart-bgimages\"></span></section>\r\n        <section><h6 class=\"margin-top-10 semi-bold margin-bottom-5\">Clear Localstorage</h6><a\r\n                ng-click=\"factoryReset()\" class=\"btn btn-xs btn-block btn-primary\" id=\"reset-smart-widget\"><i\r\n                class=\"fa fa-refresh\"></i> Factory Reset</a></section>\r\n\r\n        <h6 class=\"margin-top-10 semi-bold margin-bottom-5\">SmartAdmin Skins</h6>\r\n\r\n\r\n        <section id=\"smart-styles\">\r\n            <a ng-repeat=\"skin in skins\" ng-click=\"setSkin(skin)\" class=\"{{skin.class}}\" style=\"{{skin.style}}\"><i ng-if=\"skin.name == $parent.smartSkin\" class=\"fa fa-check fa-fw\"></i> {{skin.label}}</a>\r\n        </section>\r\n    </form>\r\n</div>");}]);
