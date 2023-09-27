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


$(function () {

    // moment.js default language
    moment.locale('en')

    angular.bootstrap(document, ['app', 'app.custom']);
 
});

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
    'app.forms',
    'app.smarttables',
    'app.taskviewer',
    'app.smartforms',
    'app.ui',
    'app.stations',
    'app.homepage',
    'app.user',
    'app.attachments',
    'app.taskkanban',
    'app.dataviewer',
    "app.userdirectory",
    "app.newtask",
    "app.blobmanager",
    'app.formeditor',
    "app.smartreports",
    "app.hub",
    "app.dataImporter",
    "app.logs",
    "app.mldashboard",
    "app.fulltextsearch",
    "app.healthcheck"
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
    $state, $stateParams, myActivityService, searchContext, Language
    ) {
    $rootScope.$state = $state;
    $rootScope.$stateParams = $stateParams;

    $rootScope.getTaskCount = function () {
        return 0;
    }

    $rootScope.getMsgCount = function () {
        return myActivityService.MessageModel.count;
    }

    $rootScope.getTotalCount = function () {
        return myActivityService.MessageModel.count;
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
$templateCache.put("app/layout/shortcut/shortcut.tpl.html","<div id=\"shortcut\">\r\n	<ul>\r\n		<li>\r\n			<a href=\"#/user/profile\" class=\"jarvismetro-tile big-cubes selected bg-color-pinkDark\"> <span class=\"iconbox\"> <i class=\"fa fa-user fa-4x\"></i> <span>{{getWord(\'My Profile\')}} </span> </span> </a>\r\n		</li>\r\n        <li>\r\n            <a href=\"#/user/password\" class=\"jarvismetro-tile big-cubes selected bg-color-darken\"> <span class=\"iconbox\"> <i class=\"fa fa-lock fa-4x\"></i> <span>{{getWord(\'Change Password\')}} </span> </span> </a>\r\n        </li>\r\n	</ul>\r\n</div>");
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

"use strict";


angular.module('app.appViews', ['ui.router'])
.config(function ($stateProvider) {

    $stateProvider
        .state('app.appViews', {
            abstract: true,
            data: {
                title: 'App views'
            }
        })

        .state('app.appViews.projects', {
            url: '/projects',
            data: {
                title: 'Projects'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/app-views/views/project-list.html',
                    controller: 'ProjectsDemoCtrl',
                    resolve: {
                        projects: function($http, APP_CONFIG){
                            return $http.get(APP_CONFIG.apiRootUrl + '/project-list.json')
                        }
                    }
                }
            }, 
            resolve: {
                scripts: function(lazyScript){
                    return lazyScript.register([
                        'datatables',
                        'datatables-bootstrap',
                        'datatables-colvis',
                        'datatables-tools',
                        'datatables-responsive'
                    ]);
                }
            }
        })

        .state('app.appViews.blogDemo', {
            url: '/blog',
            data: {
                title: 'Blog'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/app-views/views/blog-demo.html'
                }
            }
        })

        .state('app.appViews.galleryDemo', {
            url: '/gallery',
            data: {
                title: 'Gallery'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/app-views/views/gallery-demo.html'
                }
            }, 
            resolve: {
                scripts: function(lazyScript){
                    return lazyScript.register([
                        'superbox'
                    ]);
                }
            }
        })

        .state('app.appViews.forumDemo', {
            url: '/forum',
            data: {
                title: 'Forum'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/app-views/views/forum-demo.html'
                }
            }
        })

        .state('app.appViews.forumTopicDemo', {
            url: '/forum-topic',
            data: {
                title: 'Forum Topic'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/app-views/views/forum-topic-demo.html'
                }
            }
        })

        .state('app.appViews.forumPostDemo', {
            url: '/forum-post',
            data: {
                title: 'Forum Post'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/app-views/views/forum-post-demo.html'
                }
            }, 
            resolve: {
                scripts: function(lazyScript){
                    return lazyScript.register([
                        'summernote'
                    ]);
                }
            }
        })


        .state('app.appViews.profileDemo', {
            url: '/profile',
            data: {
                title: 'Profile'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/app-views/views/profile-demo.html'
                }
            }
        })


        .state('app.appViews.timelineDemo', {
            url: '/timeline',
            data: {
                title: 'Timeline'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/app-views/views/timeline-demo.html'
                }
            }
        })
});

"use strict";

angular.module("app.attachments", ["ui.router", "ui.bootstrap"]);

angular.module("app.attachments").config(function ($stateProvider, modalStateProvider) {

    $stateProvider
        .state('app.attachments', {
            url: '/attachmentviewer/:schema/:class/:oid',
            data: {
                title: 'Attachments Viewer'
            },
            views: {
                "content@app": {
                    templateUrl: "app/attachments/views/attachments-viewer.html"
                }
            },
            resolve: {
                scripts: function(lazyScript){
                    return lazyScript.register('dropzone')
                }
            }
        });
    });
"use strict";

angular.module('app.auth', [
    'ui.router'
//        ,
//        'ezfb',
//        'googleplus'
]).config(function ($stateProvider
//        , ezfbProvider
//        , GooglePlusProvider
    ) {
//        GooglePlusProvider.init({
//            clientId: authKeys.googleClientId
//        });
//
//        ezfbProvider.setInitParams({
//            appId: authKeys.facebookAppId
//        });
    $stateProvider.state('realLogin', {
        url: '/real-login',

        views: {
            root: {
                templateUrl: "app/auth/login/login.html",
                controller: 'LoginCtrl'
            }
        },
        data: {
            title: 'Login',
            rootId: 'extra-page'
        }

    })

    .state('login', {
        url: '/login',
        views: {
            root: {
                templateUrl: 'app/auth/views/login.html'
            }
        },
        data: {
            title: 'Login',
            htmlId: 'extr-page'
        },
        resolve: {
            srcipts: function(lazyScript){
                return lazyScript.register([
                    'jquery-validation'
                ])

            }
        }
    })
    .state('logout', {
        url: '/logout',
        views: {
            root: {
                templateUrl: 'app/auth/views/login.html',
                controller:  'LogoutController'
            }
        },
        data: {
            title: 'Logout',
            htmlId: 'extr-page'
        },
        resolve: {
            srcipts: function (lazyScript) {
                return lazyScript.register([
                    'jquery-validation'
                ])

            }
        }
    })
    .state('register', {
        url: '/register',
        views: {
            root: {
                templateUrl: 'app/auth/views/register.html',
                controller: 'RegisterController'
            }
        },
        data: {
            title: 'Register',
            htmlId: 'extr-page'
        }
    })
    .state('forgotPassword', {
        url: '/forgot-password',
        views: {
            root: {
                templateUrl: 'app/auth/views/forgot-password.html'
            }
        },
        data: {
            title: 'Forgot Password',
            htmlId: 'extr-page'
        }
    })
    .state('lock', {
        url: '/lock',
        views: {
            root: {
                templateUrl: 'app/auth/views/lock.html'
            }
        },
        data: {
            title: 'Locked Screen',
            htmlId: 'lock-page'
        }
    })


}).constant('authKeys', {
    googleClientId: '',
    facebookAppId: ''
});

"use strict";

angular.module("app.blobmanager", ["ui.router", "ui.bootstrap"]);

angular.module("app.blobmanager").config(function ($stateProvider, modalStateProvider) {

    $stateProvider
        .state('app.blobmanager', {
            url: '/blobmanager/:schema/:class/:oid/:prefix/:cmdHash',
            data: {
                title: 'blobmanager Viewer'
            }
        });
    });
'use strict';

angular.module('app.dashboard', [
    'ui.router',
    'ngResource'
])

.config(function ($stateProvider) {
    $stateProvider
        .state('app.dashboard', {
            url: '/dashboard',
            views: {
                "content@app": {
                    controller: 'DashboardCtrl',
                    templateUrl: 'app/dashboard/dashboard.html'
                }
            },
            authenticate: true,
            data:{
                title: 'Dashboard'
            },
            resolve: {
                scripts: function(lazyScript){
                    return lazyScript.register([
                            'jquery-jvectormap-world-mill-en',
                            'flot-time',
                            'flot-resize'
                        ]);
                }
            }
        });
});

"use strict";

angular.module("app.dataImporter", ["ui.router", "ui.bootstrap", "ngFileUpload"]);

angular.module("app.dataImporter").config(function ($stateProvider, modalStateProvider) {

    $stateProvider
        .state('app.dataImporter', {
            url: '/dataimport/:schema/:class',
            data: {
                title: 'Data Importer'
            },
            views: {
                "content@app": {
                    controller: 'dataImportCtrl',
                    templateUrl: "app/dataImporter/views/dataimport-view.html"
                }
            }
        });
});
"use strict";

angular.module("app.dataviewer", ["ui.router", "ui.bootstrap"]);

angular.module("app.dataviewer").config(function ($stateProvider, modalStateProvider) {

    $stateProvider
        .state('app.dataviewer', {
            url: '/dataviewer/:schema/:class/:oid/:xmlschema',
            data: {
                title: 'Data Viewer'
            },
            views: {
                "content@app": {
                    controller: 'DataViewerCtrl',
                    templateUrl: "app/dataviewer/views/data-viewer.html"
                }
            }
        });
    });
"use strict";

angular.module("app.formeditor", ["ui.router", "ui.bootstrap", "angularTreeview"]);

angular.module("app.formeditor").config(function ($stateProvider, modalStateProvider) {

    $stateProvider
        .state('app.formeditor', {
            url: '/formeditor/:schema/:hash',
            data: {
                title: 'Form Editor',
                animation: false /* disable the content loading animation since $viewContentLoaded will not fire when opening modal */
            },
            views: {
                "content@app": {
                    controller: 'formEditorCtrl',
                    templateUrl: "app/formeditor/views/form-editor.html"
                }
            },
            resolve: {
                scripts: function(lazyScript){
                    return lazyScript.register([
                        'ckeditor',
                        'dropzone',
                        'summernote'
                    ])
                }
            }
        });

        modalStateProvider.state('app.formeditor.help', {
            url: '^/formeditorhelp/:hash',
            templateUrl: "app/layout/partials/help-viewer.tpl.html",
            controller: 'helpViewerCtlr',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.formeditor.preview', {
            url: '^/formpreview/:schema/:class/:template/:previewid',
            templateUrl: "app/smartforms/views/ebaas-form-modal.html",
            controller: 'ebaasFormModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.formeditor.preview.pickpk', {
            url: '^/previewpickpk/:pkclass/:property/:filter/:callback',
            templateUrl: "app/smartforms/views/pick-primary-key.html",
            controller: 'pickPrimaryKeyCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.formeditor.preview.viewmanytomany', {
            url: '^/previewviewmanytomany/:masterclass/:relatedclass/:masterid',
            templateUrl: "app/smartforms/views/view-many-to-many.html",
            controller: 'viewManyToManyCtrl',
            animation: false,
            size: 'lg'
        });
    });
"use strict";


angular.module('app.forms', ['ui.router'])


angular.module('app.forms').config(function ($stateProvider) {

    $stateProvider
        .state('app.form', {
            abstract: true,
            data: {
                title: 'Forms'
            }
        })

        .state('app.form.elements', {
            url: '/form/elements',
            data: {
                title: 'Form Elements'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/forms/views/form-elements.html'
                }
            }
        })

        .state('app.form.layouts', {
            url: '/form/layouts',
            data: {
                title: 'Form Layouts'
            },
            views: {
                "content@app": {
                    controller: 'FormLayoutsCtrl',
                    templateUrl: 'app/forms/views/form-layouts/form-layouts-demo.html'
                }
            }
        })

        .state('app.form.validation', {
            url: '/form/validation',
            data: {
                title: 'Form Validation'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/forms/views/form-validation.html'
                }
            }
        })

        .state('app.form.bootstrapForms', {
            url: '/form/bootstrap-forms',
            data: {
                title: 'Bootstrap Forms'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/forms/views/bootstrap-forms.html'
                }
            }
        })

        .state('app.form.bootstrapValidation', {
            url: '/form/bootstrap-validation',
            data: {
                title: 'Bootstrap Validation'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/forms/views/bootstrap-validation.html'
                }
            },
            resolve: {
                srcipts: function(lazyScript){
                    return lazyScript.register([
                        'bootstrap-validator'
                    ])

                }
            }
        })

        .state('app.form.plugins', {
            url: '/form/plugins',
            data: {
                title: 'Form Plugins'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/forms/views/form-plugins.html',
                    controller: 'FormPluginsCtrl'
                }
            },
            resolve: {
                srcipts: function(lazyScript){
                    return lazyScript.register([
                        'bootstrap-duallistbox',
                        'bootstrap-timepicker',
                        'clockpicker',
                        'bootstrap-colorpicker',
                        'bootstrap-tagsinput',
                        'jquery-maskedinput',
                        'jquery-knob',
                        'x-editable'
                    ])

                }
            }
        })
        .state('app.form.wizards', {
            url: '/form/wizards',
            data: {
                title: 'Wizards'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/forms/views/form-wizards.html',
                    controller: 'FormWizardCtrl'
                }
            },
            resolve: {
                srcipts: function(lazyScript){
                    return lazyScript.register([
                        'jquery-maskedinput',
                        'fuelux-wizard',
                        'jquery-validation'
                    ])

                }
            }
        })
        .state('app.form.editors', {
            url: '/form/editors',
            data: {
                title: 'Editors'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/forms/views/form-editors.html'
                }
            },
            resolve: {
                scripts: function(lazyScript){
                    return lazyScript.register([
                        'bootstrap-markdown',
                        'summernote'
                    ])
                }
            }
        })
        .state('app.form.dropzone', {
            url: '/form/dropzone',
            data: {
                title: 'Dropzone'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/forms/views/dropzone.html'
                }
            },
            resolve: {
                scripts: function(lazyScript){
                    return lazyScript.register('dropzone')
                }
            }
        })
        .state('app.form.imageEditor', {
            url: '/form/image-editor',
            data: {
                title: 'Image Editor'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/forms/views/image-editor.html',
                    controller: 'ImageEditorCtrl'
                }
            },
            resolve: {
                scripts: function(lazyScript){
                    return lazyScript.register([
                        'jcrop'
                    ])
                }
            }
        })


});
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
"use strict";

angular.module("app.healthcheck", ["ui.router", "ui.bootstrap"])
.config(function ($stateProvider) {

    $stateProvider
        .state('healthcheck', {
            url: '/healthcheck',
            data: {
                title: 'Health Check'
            },
            views: {
                root: {
                    templateUrl: "app/healthcheck/views/health-check-view.html",
                    controller: "HealthCheckCtrl"
                }
            }
        });
    });
"use strict";

angular.module("app.homepage", ["ui.router", "ui.bootstrap", "angular-carousel-3d"]);

angular.module("app.homepage").config(function ($stateProvider, modalStateProvider) {

    $stateProvider
        .state('app.homepage', {
            abstract: true,
            data: {
                title: 'Dashboard'
            }
        })
        .state('app.homepage.mainmenu', {
            url: '/home/mainmenu',
            data: {
                title: 'Main Menu'
            },
            views: {
                "content@app": {
                    templateUrl: "app/homepage/views/main-menu.html",
                    controller: "mainMenuCtrl"
                }
            },
            authenticate: true,
            resolve: {
                promisedMenuItems: function ($http, APP_CONFIG) {
                    return $http.get(APP_CONFIG.ebaasRootUrl + "/api/sitemap/menu")
                },
                scripts: function (lazyScript) {
                    return lazyScript.register('dropzone');
                }
            }

        })
        .state('app.homepage.charts', {
            url: '/home/maincharts/:hash',
            data: {
                title: 'Main Charts',
                animation: false /* disable the content loading animation since $viewContentLoaded will not fire when opening modal */
            },
            views: {
                "content@app": {
                    templateUrl: "app/homepage/views/main-charts.html",
                    controller: "mainChartsCtrl"
                }
            },
            resolve: {
                promisedParams: function ($http, APP_CONFIG, $stateParams) {
                    return $http.get(APP_CONFIG.ebaasRootUrl + "/api/sitemap/parameters/" + $stateParams.hash)
                },
                scripts: function (lazyScript) {
                    return lazyScript.register([
                        'chartjs'
                    ]);
                }
            }

        })

        modalStateProvider.state('app.homepage.charts.help', {
            url: '^/chartshelp/:hash',
            templateUrl: "app/layout/partials/help-viewer.tpl.html",
            controller: 'helpViewerCtlr',
            animation: false,
            size: 'lg'
        });
    });
"use strict";

angular.module("app.hub", ["ui.router"]);

angular.module("app.hub").config(function ($stateProvider) {

    $stateProvider
        .state('app.hub', {
            url: '/hub/:schema',
            data: {
                title: 'Message Hub'
            }
        });
    });
"use strict";


angular.module('app.layout', ['ui.router', 'pdf'])

.config(function ($stateProvider, $urlRouterProvider) {


    $stateProvider
        .state('app', {
            abstract: true,
            views: {
                root: {
                    controller: 'layoutCtrl',
                    templateUrl: 'app/layout/layout.tpl.html'
                }
            },
            resolve: {
                scripts: function(lazyScript){
                    return lazyScript.register([
                        'sparkline',
                        'easy-pie'
                        ]);
                }
            }
        });

    $urlRouterProvider.otherwise(function ($injector, $location) {

        var state = $injector.get('$state');

        return "/home/mainmenu";
    });

})


"use strict";

angular.module("app.logs", ["ui.router", "ui.bootstrap"]);

angular.module("app.logs").config(function ($stateProvider, modalStateProvider) {

    $stateProvider
        .state('app.logs', {
            url: '/changelogs/:logschema/:logclass/:logoid/:logproperty',
            data: {
                title: 'Change Log Viewer'
            },
            views: {
                "content@app": {
                    templateUrl: "app/logs/views/change-log-viewer.html"
                }
            }
        });
    });
"use strict";

angular.module("app.mldashboard", ["ui.router", "ui.bootstrap"]);

angular.module("app.mldashboard").config(function ($stateProvider, modalStateProvider) {

    $stateProvider
        .state('app.mldashboard', {
            url: '/mldashboard/:hash',
            data: {
                title: 'ML Dashboard'
            },
            views: {
                "content@app": {
                    templateUrl: "app/mldashboard/views/ml-dashboard-layout.html",
                    controller: 'MLDashboardLayoutCtrl'
                }
            },
            authenticate: true,
            resolve: {
                propmisedParams: function ($http, APP_CONFIG, $stateParams) {
                    return $http.get(APP_CONFIG.ebaasRootUrl + "/api/sitemap/parameters/" + $stateParams.hash)
                }
            }

        })
        .state('app.mldashboard.modeldashboard', {
            url: '/modeldashboard/:project/:model',
            data: {
                title: 'Model Dashboard'
            },
            views: {
                "modeldashboard@app.mldashboard": {
                    controller: 'MLModelDashboardCtrl',
                    templateUrl: "app/mldashboard/views/ml-model-dashboard.html"
                }
            },
            authenticate: true,
            resolve: {
                scripts: function (lazyScript) {
                    return lazyScript.register(
                        [
                            'flot',
                            'flot-resize',
                            'flot-selection',
                            'flot-fillbetween',
                            'flot-orderBar',
                            'flot-pie',
                            'flot-time',
                            'flot-tooltip'
                        ])
                }
            }
        });

        modalStateProvider.state('app.mldashboard.help', {
            url: '^/mldashboardhelp/:hash',
            templateUrl: "app/layout/partials/help-viewer.tpl.html",
            controller: 'helpViewerCtlr',
            animation: false,
            size: 'lg'
        });
    });
"use strict";

angular.module("app.newtask", ["ui.router", "ui.bootstrap"]);

angular.module("app.newtask").config(function ($stateProvider, modalStateProvider) {

    $stateProvider
        .state('app.newtask', {
            abstract: true,
            data: {
                title: 'New Task',
                animation: false /* disable the content loading animation since $viewContentLoaded will not fire when opening modal */
            }
        })
        .state('app.newtask.create', {
            url: '/newtask/create/:schema/:class/:template/:hash',
            data: {
                title: 'Create Task'
            },
            authenticate: true,
            views: {
                "content@app": {
                    templateUrl: 'app/newtask/views/create-task.html',
                    controller: 'CreateTaskCtrl'
                }
            },
            resolve: {
                propmisedParams: function ($http, APP_CONFIG, $stateParams) {
                    if ($stateParams.hash) {
                        return $http.get(APP_CONFIG.ebaasRootUrl + "/api/sitemap/parameters/" + $stateParams.hash);
                    }
                    else {
                        return [];
                    }
                }
            }
        });
});
"use strict";

angular.module("app.smartforms", ["ui.router", "ui.bootstrap"]);

angular.module("app.smartforms").config(function ($stateProvider, modalStateProvider) {

    $stateProvider
        .state('app.smartforms', {
            abstract: true,
            data: {
                title: 'Smart Forms'
            },
            resolve: {
                scripts: function (lazyScript) {
                    return lazyScript.register(
                    'dropzone',
                    'summernote')
                }
            }
        })
        .state('app.smartforms.ebaasform', {
            url: '/form/:schema/:class/:oid/:readonly/:template/:formAttribute',
            data: {
                title: 'Newtera Form',
                animation: false /* disable the content loading animation since $viewContentLoaded will not fire when opening modal */
            },
            views: {
                "content@app": {
                    controller: 'ebaasFormCtrl',
                    templateUrl: "app/smartforms/views/ebaas-form.html"
                }
            },
            resolve: {
                parentStateName : function($state)
                {
                    return undefined;
                }
            }
        });

        modalStateProvider.state('app.smartforms.ebaasform.modalform', {
            url: '^/formmodal/:class/:oid/:readonly/:formAttribute',
            templateUrl: "app/smartforms/views/ebaas-form-modal.html",
            controller: 'ebaasFormModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smartforms.ebaasform.relatedform', {
            url: '^/relatedform/:rclass/:roid/:rtemplate/:rformAttribute/:readonly',
            templateUrl: "app/smartforms/views/related-form-modal.html",
            controller: 'relatedFormModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smartforms.ebaasform.pickpk', {
            url: '^/formpickpk/:pkclass/:property/:filter/:callback',
            templateUrl: "app/smartforms/views/pick-primary-key.html",
            controller: 'pickPrimaryKeyCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smartforms.ebaasform.viewmanytomany', {
            url: '^/formviewmanytomany/:masterclass/:relatedclass/:masterid',
            templateUrl: "app/smartforms/views/view-many-to-many.html",
            controller: 'viewManyToManyCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smartforms.ebaasform.uploadimage', {
            url: '^/formuploadimage/:property/:imageid',
            templateUrl: "app/smartforms/views/upload-image.html",
            controller: 'uploadImageCtrl',
            animation: false,
            size: 'md'
        });

        modalStateProvider.state('app.smartforms.ebaasform.viewlog', {
            url: '^/formviewlog/:logschema/:logclass/:logoid/:logproperty',
            templateUrl: "app/logs/views/change-log-viewer.html",
            controller: 'changeLogViewerCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smartforms.ebaasform.report', {
            url: '^/formreport/:schema/:class/:oid/:template/:templateAttribute/:fileType/:cmdHash',
            templateUrl: "app/smartreports/views/download-report.html",
            controller: 'downloadReportCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'sm'
        });

        modalStateProvider.state('app.smartforms.ebaasform.filemanager', {
            url: '^/formfilemanager/:schema/:class/:oid/:cmdHash',
            templateUrl: "app/fileManager/views/file-manager-viewer.html",
            controller: 'fileManagerViewerCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

    });
"use strict";

angular.module("app.smarttables", ["ngResource", "smart-table", "dx", "ui.router", "ui.bootstrap", "ui.bootstrap.modal", "ngProgress"]);

angular.module("app.smarttables")
    .provider('modalState', function ($stateProvider, $injector) {
        var provider = this;
        this.$get = function () {
            return provider;
        }
        this.state = function (stateName, options) {
            var modalInstance;
            $stateProvider.state(stateName, {
                url: options.url,
                data: {
                    title: 'Modal'
                },
                onEnter: function ($modal, $state, $injector) {
                    modalInstance = $modal.open(options);
                    modalInstance.result.then(function (data) {
                        // modal closed
                        var rScope = $injector.get('$rootScope');
                        rScope.$emit('modalClosed', data);

                    }, function () {
                        // modal dismissed
                        var rScope = $injector.get('$rootScope');
                        rScope.$emit('modalDismissed', "");
                    })['finally'](function () {
                        modalInstance = null;
                        if ($state.$current.name === stateName) {
                            $state.go('^', {}, {location:false, notify: false });
                        }
                    });
                },
                onExit: function () {
                    if (modalInstance) {
                        modalInstance.close();
                    }
                },
                resolve : options.resolve
            });
        };
    })
    .config(function ($stateProvider, modalStateProvider, $urlRouterProvider) {

        $stateProvider
            .state('app.smarttables', {
                abstract: true,
                data: {
                    title: 'Smart Tables'
                },
                resolve: {
                    scripts: function (lazyScript) {
                        return lazyScript.register('dropzone')
                    }
                }
            })
            .state('app.smarttables.datagrid', {
                url: '/datagrid/:schema/:class/:edit/:delete/:insert/:track/:export/:import/:attachment/:hash',
                data: {
                    title: 'Smart Data Grid',
                    animation: false /* disable the content loading animation since $viewContentLoaded will not fire when opening modal */
                },
                authenticate: true,
                views: {
                    "content@app": {
                        controller: 'dataGridCtrl',
                        templateUrl: "app/smarttables/views/datagrid.html"
                    }
                },
                resolve: {
                    scripts: function (lazyScript) {
                        return lazyScript.register(
                            [
                                'flot',
                                'flot-resize',
                                'flot-selection',
                                'flot-fillbetween',
                                'flot-orderBar',
                                'flot-pie',
                                'flot-time',
                                'flot-tooltip',
                                'dropzone',
                                'summernote'
                            ])
                    },
                    propmisedParams: function ($http, APP_CONFIG, $stateParams) {
                        if ($stateParams.hash) {
                            return $http.get(APP_CONFIG.ebaasRootUrl + "/api/sitemap/parameters/" + $stateParams.hash);
                        }
                        else
                        {
                            return [];
                        }
                    }
                }
            })
            .state('app.smarttables.fulltextdatagrid', {
                url: '/fulltextdatagrid/:schema/:class/:edit/:delete/:insert/:track/:export/:import/:search/:attachment/:hash',
                data: {
                    title: 'Smart Data Grid',
                    animation: false /* disable the content loading animation since $viewContentLoaded will not fire when opening modal */
                },
                authenticate: true,
                views: {
                    "content@app": {
                        controller: 'dataGridCtrl',
                        templateUrl: "app/smarttables/views/datagrid.html"
                    }
                },
                resolve: {
                    scripts: function (lazyScript) {
                        return lazyScript.register(
                            [
                                'flot',
                                'flot-resize',
                                'flot-selection',
                                'flot-fillbetween',
                                'flot-orderBar',
                                'flot-pie',
                                'flot-time',
                                'flot-tooltip',
                                'dropzone',
                                'summernote'
                            ])
                    },
                    propmisedParams: function ($http, APP_CONFIG, $stateParams) {
                        if ($stateParams.hash) {
                            return $http.get(APP_CONFIG.ebaasRootUrl + "/api/sitemap/parameters/" + $stateParams.hash);
                        }
                        else {
                            return [];
                        }
                    }
                }
            })
            .state('app.smarttables.datagrid.taskview', {
                url: '/datagridtaskviewer/:schema/:class/:oid/:taskNodeAttribute/:taskTemplate/:itemClass/:itemNodeAttribute/:itemTemplate/:packetClass/:packetTemplate/:packetNodeAttribute/:packetPrefixAttribute',
                data: {
                    title: 'Task Viewer',
                    animation: false /* disable the content loading animation since $viewContentLoaded will not fire when opening modal */
                },
                views: {
                    "content@app": {
                        controller: 'TaskViewerLayoutCtrl',
                        templateUrl: "app/taskviewer/views/task-viewer-layout.html"
                    }
                }
            })
            .state('app.smarttables.datagrid.related', {
                url: '/datagridrelated/:schema/:class/:oid/:relatedclass/:insertrelated/:editrelated/:attachmentrelated/:deleterelated/:exportrelated/:importrelated',
                data: {
                    title: 'Smart Data Grid',
                    animation: false /* disable the content loading animation since $viewContentLoaded will not fire when opening modal */
                },
                views: {
                    "content@app": {
                        controller: 'relatedDataGridCtrl',
                        templateUrl: "app/smarttables/views/related-datagrid.html"
                    }
                },
                resolve: {
                    promiseParentClassInfo: function ($http, APP_CONFIG, $stateParams) {
                        var url = APP_CONFIG.ebaasRootUrl + "/api/metadata/class/" + encodeURIComponent($stateParams.schema) + "/" + $stateParams.class;
                        return $http.get(url);
                    }
                }
            })
            .state('app.smarttables.datagrid.requestwizard', {
                url: '/datagridrequestwizard/:schema/:class/:oid/:hash/:taskid',
                data: {
                    title: 'Request Wizard',
                    animation: false /* disable the content loading animation since $viewContentLoaded will not fire when opening modal */
                },
                views: {
                    "content@app": {
                        templateUrl: 'app/wizards/views/request-form-wizard.html',
                        controller: 'requestFormWizardCtrl'
                    }
                },
                resolve: {
                    promiseParams: function ($http, APP_CONFIG, $stateParams) {
                        return $http.get(APP_CONFIG.ebaasRootUrl + "/api/sitemap/parameters/" + $stateParams.hash)
                    },
                    promiseInstance: function ($http, APP_CONFIG, $stateParams) {
                        if ($stateParams.oid) {
                            var url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($stateParams.schema) + "/" + $stateParams.class + "/" + $stateParams.oid;
                            return $http.get(url);
                        }
                        else {
                            return undefined;
                        }
                    },
                    srcipts: function (lazyScript) {
                        return lazyScript.register([
                            'jquery-maskedinput',
                            'fuelux-wizard',
                            'jquery-validation'
                        ])

                    }
                }
            })
            .state('app.smarttables.datagrid.form', {
                url: '^/datagridform/:schema/:class/:oid/:template/:formAttribute',
                data: {
                    title: 'Smart Form',
                    animation: false /* disable the content loading animation since $viewContentLoaded will not fire when opening modal */
                },
                views: {
                    "content@app": {
                        controller: 'ebaasFormCtrl',
                        templateUrl: "app/smartforms/views/ebaas-form.html"
                    }
                },
                resolve: {
                    parentStateName: function ($state) {
                        return $state.current.name;
                    }
                }
            });

        modalStateProvider.state('app.smarttables.datagrid.help', {
            url: '^/datagridhelp/:hash',
            templateUrl: "app/layout/partials/help-viewer.tpl.html",
            controller: 'helpViewerCtlr',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.form.pickpk', {
            url: '^/datagridformpickpk/:pkclass/:property/:filter/:callback',
            templateUrl: "app/smartforms/views/pick-primary-key.html",
            controller: 'pickPrimaryKeyCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.form.viewmanytomany', {
            url: '^/datagridformviewmanytomany/:masterclass/:relatedclass/:masterid',
            templateUrl: "app/smartforms/views/view-many-to-many.html",
            controller: 'viewManyToManyCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.form.uploadimage', {
            url: '^/datagridformuploadimage/:property/:imageid',
            templateUrl: "app/smartforms/views/upload-image.html",
            controller: 'uploadImageCtrl',
            animation: false,
            size: 'md'
        });

        modalStateProvider.state('app.smarttables.datagrid.form.viewlog', {
            url: '^/datagridformviewlog/:logschema/:logclass/:logoid/:logproperty',
            templateUrl: "app/logs/views/change-log-viewer.html",
            controller: 'changeLogViewerCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.modalform', {
            url: '^/datagridmodalform/:schema/:class/:oid/:readonly/:template/:formAttribute/:duplicate/:cmd/:sref',
            templateUrl: "app/smartforms/views/ebaas-form-modal.html",
            controller: 'ebaasFormModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.modalform.relatedform', {
            url: '^/datagridrelatedform/:rclass/:roid/:rtemplate/:rformAttribute/:readonly',
            templateUrl: "app/smartforms/views/related-form-modal.html",
            controller: 'relatedFormModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.modalform.relatedform.pickpk', {
            url: '^/datagridmodalrelatedformpickpk/:pkclass/:property/:filter/:callback',
            templateUrl: "app/smartforms/views/pick-primary-key.html",
            controller: 'pickPrimaryKeyCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.modalform.relatedform.viewmanytomany', {
            url: '^/datagridmodalrelatedformviewmanytomany/:masterclass/:relatedclass/:masterid',
            templateUrl: "app/smartforms/views/view-many-to-many.html",
            controller: 'viewManyToManyCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.modalform.relatedform.uploadimage', {
            url: '^/datagridmodalrelatedformuploadimage/:property/:imageid',
            templateUrl: "app/smartforms/views/upload-image.html",
            controller: 'uploadImageCtrl',
            animation: false,
            size: 'md'
        });

        modalStateProvider.state('app.smarttables.datagrid.modalform.relatedform.viewlog', {
            url: '^/datagridmodalrelatedformviewlog/:logschema/:logclass/:logoid/:logproperty',
            templateUrl: "app/logs/views/change-log-viewer.html",
            controller: 'changeLogViewerCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.form.relatedform', {
            url: '^/datagridformrelatedform/:rclass/:roid/:rtemplate/:rformAttribute/:readonly',
            templateUrl: "app/smartforms/views/related-form-modal.html",
            controller: 'relatedFormModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.modalform.pickpk', {
            url: '^/datagridmodalformpickpk/:pkclass/:property/:filter/:callback',
            templateUrl: "app/smartforms/views/pick-primary-key.html",
            controller: 'pickPrimaryKeyCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.modalform.viewmanytomany', {
            url: '^/datagridmodalformviewmanytomany/:masterclass/:relatedclass/:masterid',
            templateUrl: "app/smartforms/views/view-many-to-many.html",
            controller: 'viewManyToManyCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.modalform.uploadimage', {
            url: '^/datagridmodalformuploadimage/:property/:imageid',
            templateUrl: "app/smartforms/views/upload-image.html",
            controller: 'uploadImageCtrl',
            animation: false,
            size: 'md'
        });

        modalStateProvider.state('app.smarttables.datagrid.modalform.viewlog', {
            url: '^/datagridmodalformviewlog/:logschema/:logclass/:logoid/:logproperty',
            templateUrl: "app/logs/views/change-log-viewer.html",
            controller: 'changeLogViewerCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.related.relatedform', {
            url: '^/relatedform/:schema/:rclass/:roid/:rtemplate/:rformAttribute/:readonly',
            templateUrl: "app/smartforms/views/related-form-modal.html",
            controller: 'relatedFormModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.related.relatedform.pickpk', {
            url: '^/datagridrelatedrelatedformpickpk/:pkclass/:property/:filter/:callback',
            templateUrl: "app/smartforms/views/pick-primary-key.html",
            controller: 'pickPrimaryKeyCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.related.relatedform.viewmanytomany', {
            url: '^/datagridrelatedrelatedformviewmanytomany/:masterclass/:relatedclass/:masterid',
            templateUrl: "app/smartforms/views/view-many-to-many.html",
            controller: 'viewManyToManyCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.related.relatedform.uploadimage', {
            url: '^/datagridrelatedrelatedformuploadimage/:property/:imageid',
            templateUrl: "app/smartforms/views/upload-image.html",
            controller: 'uploadImageCtrl',
            animation: false,
            size: 'md'
        });

        modalStateProvider.state('app.smarttables.datagrid.related.relatedform.viewlog', {
            url: '^/datagridrelatedrelatedformviewlog/:logschema/:logclass/:logoid/:logproperty',
            templateUrl: "app/logs/views/change-log-viewer.html",
            controller: 'changeLogViewerCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.report', {
            url: '^/datagridreport/:schema/:class/:oid/:template/:templateAttribute/:fileType/:cmdHash',
            templateUrl: "app/smartreports/views/download-report.html",
            controller: 'downloadReportCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'sm'
        });

        modalStateProvider.state('app.smarttables.datagrid.createrequest', {
            url: '^/datagridcreaterequest/:schema/:class/:oid/:sourcetemplate/:orderclass/:targettemplate/:api/:wizardhash',
            templateUrl: "app/wizards/views/create-request.html",
            controller: 'createRequestCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'sm'
        });

        modalStateProvider.state('app.smarttables.datagrid.related.importdata', {
            url: '^/datagridimportdata/:schema/:class/:oid/:relatedclass',
            templateUrl: "app/dataImporter/views/dataimport-view.html",
            controller: 'dataImportCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'md'
        });

        modalStateProvider.state('app.smarttables.datagrid.importdata', {
            url: '^/datagridrelatedimportdata/:schema/:class',
            templateUrl: "app/dataImporter/views/dataimport-view.html",
            controller: 'dataImportCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'md'
        });

        modalStateProvider.state('app.smarttables.datagrid.filemanager', {
            url: '^/datagridfilemanager/:schema/:class/:oid/:cmdHash',
            templateUrl: "app/fileManager/views/file-manager-viewer.html",
            controller: 'fileManagerViewerCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.dataviewer', {
            url: '^/datagriddataviewer/:schema/:class/:oid/:xmlschema',
            templateUrl: "app/dataviewer/views/data-viewer-modal.html",
            controller: 'DataViewerModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.postview', {
            url: '^/datagridpostview/:schema/:class/:oid/:postClass?from&size&subject&content&url&urlparams',
            templateUrl: "app/taskforum/views/post-view.html",
            controller: 'PostViewCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.processdata.selectdata', {
            url: '^/datagridselectprocessdata/:schema/:class/:oid/:xmlschema/:category/:api',
            templateUrl: "app/dataviewer/views/select-data-modal.html",
            controller: 'SelectDataModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.related.modalform', {
            url: '^/datagridrelatedmodalform/:schema/:class/:oid/:readonly/:template/:formAttribute/:duplicate/:cmd/:sref',
            templateUrl: "app/smartforms/views/ebaas-form-modal.html",
            controller: 'ebaasFormModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.related.modalform.relatedform', {
            url: '^/datagridrelatedformmodalformrelatedform/:schema/:rclass/:roid/:rtemplate/:rformAttribute/:readonly',
            templateUrl: "app/smartforms/views/related-form-modal.html",
            controller: 'relatedFormModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.related.modalform.pickpk', {
            url: '^/datagridrelatedformmodalformpickpk/:pkclass/:property/:filter/:callback',
            templateUrl: "app/smartforms/views/pick-primary-key.html",
            controller: 'pickPrimaryKeyCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.related.modalform.viewmanytomany', {
            url: '^/datagridrelatedformmodalformviewmanytomany/:masterclass/:relatedclass/:masterid',
            templateUrl: "app/smartforms/views/view-many-to-many.html",
            controller: 'viewManyToManyCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.related.modalform.uploadimage', {
            url: '^/datagridrelatedformmodalformuploadimage/:property/:imageid',
            templateUrl: "app/smartforms/views/upload-image.html",
            controller: 'uploadImageCtrl',
            animation: false,
            size: 'md'
        });

        modalStateProvider.state('app.smarttables.datagrid.related.modalform.viewlog', {
            url: '^/datagridrelatedformmodalformviewlog/:logschema/:logclass/:logoid/:logproperty',
            templateUrl: "app/logs/views/change-log-viewer.html",
            controller: 'changeLogViewerCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.smarttables.datagrid.related.report', {
            url: '^/datagridrelatedreport/:schema/:class/:oid/:template/:templateAttribute/:fileType/:cmdHash',
            templateUrl: "app/smartreports/views/download-report.html",
            controller: 'downloadReportCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'sm'
        });
    });
"use strict";

angular.module("app.smartreports", ["ui.router", "ui.bootstrap"]);

angular.module("app.smartreports").config(function ($stateProvider) {

    $stateProvider
        .state('app.smartreports', {
            url: '/smartreports/:schema/:class/:oid/:xmlschema/:fileType/:cmdHash',
            data: {
                title: 'Smart Reports'
            },
            views: {
                "content@app": {
                    controller: 'downloadReportCtrl',
                    templateUrl: "app/smartreports/views/download-report.html"
                }
            }
        });
    });
"use strict";

angular.module("app.stations", ["ui.router", "ui.bootstrap"]);

angular.module("app.stations").config(function ($stateProvider, modalStateProvider) {

    $stateProvider
        .state('app.stations', {
            url: '/stations/:schema/:class/:hash',
            data: {
                title: 'Stations',
                animation: false /* disable the content loading animation since $viewContentLoaded will not fire when opening modal */
            },
            views: {
                "content@app": {
                    templateUrl: "app/stations/views/stations-layout.html",
                    controller: 'StationsLayoutCtrl'
                }
            },
            resolve: {
                stationParams: function ($http, APP_CONFIG, $stateParams) {
                    return $http.get(APP_CONFIG.ebaasRootUrl + "/api/sitemap/parameters/" + $stateParams.hash);
                }
            }

        })
        .state('app.stations.dashboard', {
            url: '/stationdashboard/:schema/:class/:oid/:xmlschema/:index',
            data: {
                title: 'Station Dashboard',
                animation: false /* disable the content loading animation since $viewContentLoaded will not fire when opening modal */
            },
            views: {
                "dashboard@app.stations": {
                    controller: 'StationDashboardCtrl',
                    templateUrl: "app/stations/views/station-dashboard.html"
                }
            },
            authenticate: true,
            resolve: {
                scripts: function (lazyScript) {
                    return lazyScript.register([
                            'jquery-jvectormap-world-mill-en',
                            'flot-time',
                            'flot-resize'
                    ]);
                },
                promisedSettings: function ($http, APP_CONFIG, $stateParams) {
                    if ($stateParams.xmlschema) {
                        return $http.get(APP_CONFIG.ebaasRootUrl + "/api/data/extract/" + encodeURIComponent($stateParams.schema) + "/" + $stateParams.class + "/" + $stateParams.oid + "/" + $stateParams.xmlschema);
                    }
                    else
                    {
                        return undefined;
                    }
                }
            }
        });

        modalStateProvider.state('app.stations.help', {
            url: '^/stationshelp/:hash',
            templateUrl: "app/layout/partials/help-viewer.tpl.html",
            controller: 'helpViewerCtlr',
            animation: false,
            size: 'lg'
        });
    });
"use strict";

angular.module("app.taskviewer", ["ui.router", "ui.bootstrap"]);

angular.module("app.taskviewer").config(function ($stateProvider, modalStateProvider) {

    $stateProvider
        .state('app.taskviewer', {
            abstract: true,
            url: '/taskviewer/:schema/:class/:oid/:taskNodeAttribute/:taskTemplate/:itemClass/:itemNodeAttribute/:itemTemplate/:packetClass/:packetNodeAttribute/:packetTemplate/:packetPrefix/:packetPrefixAttribute/:hash',
            data: {
                title: 'Task Viewer'
            },
            views: {
                "content@app": {
                    templateUrl: "app/taskviewer/views/task-viewer-layout.html",
                    controller: 'TaskViewerLayoutCtrl'
                },
                'taskdetails@app.taskviewer': { templateUrl: 'app/taskviewer/views/task-details.html', },
            }
        })
        .state('app.taskviewer.details', {
            url: '/details/:schema/:class/:oid/:taskNodeAttribute/:taskTemplate/:itemClass/:itemOid/:itemNodeAttribute/:itemTemplate/:packetClass/:packetOid/:packetNodeAttribute/:packetTemplate/:packetPrefix/:packetPrefixAttribute/:activeTabId',
            views: {
                "taskform@app.taskviewer": {
                    controller: 'TaskFormCtrl',
                    templateUrl: "app/taskviewer/views/task-form.html"
                },
                "itemform@app.taskviewer": {
                    controller: 'ItemFormCtrl',
                    templateUrl: "app/taskviewer/views/item-form.html"
                },
                "packetform@app.taskviewer": {
                    controller: 'PacketFormCtrl',
                    templateUrl: "app/taskviewer/views/packet-form.html"
                }
            },
        });

    modalStateProvider.state('app.taskviewer.details.modalform', {
        url: '^/detailsmodalform/:schema/:class/:readonly/:oid/:template/:formAttribute/:duplicate/:cmd/:sref',
        templateUrl: "app/smartforms/views/ebaas-form-modal.html",
        controller: 'ebaasFormModalCtrl',
        backdrop: 'static', /*  this prevent user interaction with the background  */
        keyboard: false,
        animation: false,
        size: 'lg'
    });

    modalStateProvider.state('app.taskviewer.details.relatedform', {
        url: '^/detailsmodalform/:masterclass/:masteroid/:rclass/:roid/:rtemplate/:rformAttribute/:readonly',
        templateUrl: "app/smartforms/views/related-form-modal.html",
        controller: 'relatedFormModalCtrl',
        backdrop: 'static', /*  this prevent user interaction with the background  */
        keyboard: false,
        animation: false,
        size: 'lg'
    });

    modalStateProvider.state('app.taskviewer.details.relatedform.pickpk', {
        url: '^/detailrelatedformpickpk/:pkclass/:property/:filter/:callback',
        templateUrl: "app/smartforms/views/pick-primary-key.html",
        controller: 'pickPrimaryKeyCtrl',
        animation: false,
        size: 'lg'
    });

    modalStateProvider.state('app.taskviewer.details.relatedform.viewmanytomany', {
        url: '^/detailrelatedformviewmanytomany/:masterclass/:relatedclass/:masterid',
        templateUrl: "app/smartforms/views/view-many-to-many.html",
        controller: 'viewManyToManyCtrl',
        animation: false,
        size: 'lg'
    });
});
"use strict";

angular.module("app.taskkanban", ["ui.router", "ui.bootstrap", "DlhSoft.Kanban.Angular.Components"]);

angular.module("app.taskkanban").config(function ($stateProvider, modalStateProvider) {

    $stateProvider
        .state('app.taskkanban', {
            abstract: true,
            data: {
                title: 'Task Kanban'
            }
        })
        .state('app.taskkanban.kanbanmain', {
            url: '/taskkanban/kanbanmain/:schema/:class/:hash',
            data: {
                title: 'Task Kanban',
                animation: false /* disable the content loading animation since $viewContentLoaded will not fire when opening modal */
            },
            views: {
                "content@app": {
                    templateUrl: 'app/taskkanban/views/kanban-main.html',
                    controller: 'KanbanMainCtrl'
                }
            },
            authenticate: true,
            resolve: {
                scripts: function (lazyScript) {
                    return lazyScript.register(
                        [
                            'flot',
                            'flot-resize',
                            'flot-selection',
                            'flot-fillbetween',
                            'flot-orderBar',
                            'flot-pie',
                            'flot-time',
                            'flot-tooltip',
                            'dropzone',
                            'summernote'
                        ])
                },
                propmisedParams: function ($http, APP_CONFIG, $stateParams) {
                    return $http.get(APP_CONFIG.ebaasRootUrl + "/api/sitemap/parameters/" + $stateParams.hash)
                },
                kanbanModel: function ($http, APP_CONFIG, $stateParams, propmisedParams) {
                    var params = propmisedParams.data;

                    var url = APP_CONFIG.ebaasRootUrl + "/api/kanban/data/" + encodeURIComponent($stateParams.schema) + "/" + encodeURIComponent($stateParams.class);
                    url += "?group=" + params['group'];
                    url += "&state=" + params['state'];
                    url += "&ID=" + params['ID'];
                    url += "&title=" + params['title'];
                    url += "&stateMapping=" + encodeURIComponent(params['stateMapping']);
                    return $http.get(url);
                }
            }
        });

        modalStateProvider.state('app.taskkanban.kanbanmain.help', {
            url: '^/taskkanbanhelp/:hash',
            templateUrl: "app/layout/partials/help-viewer.tpl.html",
            controller: 'helpViewerCtlr',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.taskkanban.kanbanmain.modalform', {
            url: '^/kanbanmodalform/:schema/:class/:oid/:readonly/:template/:formAttribute',
            templateUrl: "app/smartforms/views/ebaas-form-modal.html",
            controller: 'ebaasFormModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg',
            resolve: {
                scripts: function (lazyScript) {
                    return lazyScript.register(
                        [
                            'flot',
                            'flot-resize',
                            'flot-selection',
                            'flot-fillbetween',
                            'flot-orderBar',
                            'flot-pie',
                            'flot-time',
                            'flot-tooltip',
                            'dropzone'
                        ])
                }
            }
        });

        modalStateProvider.state('app.taskkanban.kanbanmain.modalform.relatedform', {
            url: '^/kanbanrelatedform/:rclass/:roid/:rtemplate/:rformAttribute/:readonly',
            templateUrl: "app/smartforms/views/related-form-modal.html",
            controller: 'relatedFormModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.taskkanban.kanbanmain.modalform.pickpk', {
            url: '^/kanbanmodalformpickpk/:pkclass/:property/:filter/:callback',
            templateUrl: "app/smartforms/views/pick-primary-key.html",
            controller: 'pickPrimaryKeyCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.taskkanban.kanbanmain.modalform.viewmanytomany', {
            url: '^/kanbanmodalformviewmanytomany/:masterclass/:relatedclass/:masterid',
            templateUrl: "app/smartforms/views/view-many-to-many.html",
            controller: 'viewManyToManyCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.taskkanban.kanbanmain.modalform.uploadimage', {
            url: '^/kanbanmodalformuploadimage/:property/:imageid',
            templateUrl: "app/smartforms/views/upload-image.html",
            controller: 'uploadImageCtrl',
            animation: false,
            size: 'md'
        });

        modalStateProvider.state('app.taskkanban.kanbanmain.modalform.viewlog', {
            url: '^/kanbanmodalformviewlog/:logschema/:logclass/:logoid/:logproperty',
            templateUrl: "app/logs/views/change-log-viewer.html",
            controller: 'changeLogViewerCtrl',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.taskkanban.kanbanmain.filemanager', {
            url: '^/kanbanfilemanager/:schema/:class/:oid/:cmdHash',
            templateUrl: "app/fileManager/views/file-manager-viewer.html",
            controller: 'fileManagerViewerCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.taskkanban.kanbanmain.report', {
            url: '^/kanbanreport/:schema/:class/:oid/:template/:templateAttribute/:fileType/:cmdHash',
            templateUrl: "app/smartreports/views/download-report.html",
            controller: 'downloadReportCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'sm'
        });

        modalStateProvider.state('app.taskkanban.kanbanmain.postview', {
            url: '^/kanbanpostview/:schema/:class/:oid/:postClass?from&size&subject&content&url&urlparams',
            templateUrl: "app/taskforum/views/post-view.html",
            controller: 'PostViewCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.taskkanban.kanbanmain.processdata.selectdata', {
            url: '^/kanbanselectprocessdata/:schema/:class/:oid/:xmlschema/:category/:api',
            templateUrl: "app/dataviewer/views/select-data-modal.html",
            controller: 'SelectDataModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.taskkanban.kanbanmain.processdata.timeseries', {
            url: '^/kanbanprocessdatatimeseries/:frequency/:ts',
            templateUrl: "app/dataviewer/views/time-series-modal.html",
            controller: 'TimeSeriesModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });
    });
'use strict'

angular.module('app.ui', ['ui.router']);

angular.module('app.ui').config(function($stateProvider){

    $stateProvider
        .state('app.ui', {
            abstract: true,
            data: {
                title: 'UI Elements'
            }
        })
        .state('app.ui.general', {
            url: '/ui/general',
            data: {
                title: 'General Elements'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/ui/views/general-elements.html',
                    controller: 'GeneralElementsCtrl'
                }
            }
        })
        .state('app.ui.buttons', {
            url: '/ui/buttons',
            data: {
                title: 'Buttons'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/ui/views/buttons.html',
                    controller: 'GeneralElementsCtrl'
                }
            }
        })
        .state('app.ui.iconsFa', {
            url: '/ui/icons-font-awesome',
            data: {
                title: 'Font Awesome'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/ui/views/icons-fa.html'
                }
            }
        })
        .state('app.ui.iconsGlyph', {
            url: '/ui/icons-glyph',
            data: {
                title: 'Glyph Icons'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/ui/views/icons-glyph.html'
                }
            }
        })
        .state('app.ui.iconsFlags', {
            url: '/ui/icons-flags',
            data: {
                title: 'Flags'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/ui/views/icons-flags.html'
                }
            }
        })
        .state('app.ui.grid', {
            url: '/ui/grid',
            data: {
                title: 'Grid'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/ui/views/grid.html'
                }
            }
        })
        .state('app.ui.treeView', {
            url: '/ui/tree-view',
            data: {
                title: 'Tree View'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/ui/views/tree-view.html',
                    controller: 'TreeviewCtrl'
                }
            }
        })
        .state('app.ui.nestableLists', {
            url: '/ui/nestable-lists',
            data: {
                title: 'Nestable Lists'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/ui/views/nestable-lists.html'
                }
            },
            resolve: {
                srcipts: function(lazyScript){
                    return lazyScript.register([
                        'jquery-nestable'
                    ])

                }
            }
        })
        .state('app.ui.jqueryUi', {
            url: '/ui/jquery-ui',
            data: {
                title: 'JQuery UI'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/ui/views/jquery-ui.html',
                    controller: 'JquiCtrl'
                }
            },
            resolve: {
                srcipts: function(lazyScript){
                    return lazyScript.register([
                        'bootstrap-slider'
                    ])

                }
            }
        })
        .state('app.ui.typography', {
            url: '/ui/typography',
            data: {
                title: 'JQuery UI'
            },
            views: {
                "content@app": {
                    templateUrl: 'app/ui/views/typography.html'
                }
            }
        })
});
angular.module("app").run(["$templateCache", function($templateCache) {$templateCache.put("app/dashboard/live-feeds.tpl.html","<div jarvis-widget id=\"live-feeds-widget\" data-widget-togglebutton=\"false\" data-widget-editbutton=\"false\"\r\n     data-widget-fullscreenbutton=\"false\" data-widget-colorbutton=\"false\" data-widget-deletebutton=\"false\">\r\n<!-- widget options:\r\nusage: <div class=\"jarviswidget\" id=\"wid-id-0\" data-widget-editbutton=\"false\">\r\n\r\ndata-widget-colorbutton=\"false\"\r\ndata-widget-editbutton=\"false\"\r\ndata-widget-togglebutton=\"false\"\r\ndata-widget-deletebutton=\"false\"\r\ndata-widget-fullscreenbutton=\"false\"\r\ndata-widget-custombutton=\"false\"\r\ndata-widget-collapsed=\"true\"\r\ndata-widget-sortable=\"false\"\r\n\r\n-->\r\n<header>\r\n    <span class=\"widget-icon\"> <i class=\"glyphicon glyphicon-stats txt-color-darken\"></i> </span>\r\n\r\n    <h2>Live Feeds </h2>\r\n\r\n    <ul class=\"nav nav-tabs pull-right in\" id=\"myTab\">\r\n        <li class=\"active\">\r\n            <a data-toggle=\"tab\" href=\"#s1\"><i class=\"fa fa-clock-o\"></i> <span class=\"hidden-mobile hidden-tablet\">Live Stats</span></a>\r\n        </li>\r\n\r\n        <li>\r\n            <a data-toggle=\"tab\" href=\"#s2\"><i class=\"fa fa-facebook\"></i> <span class=\"hidden-mobile hidden-tablet\">Social Network</span></a>\r\n        </li>\r\n\r\n        <li>\r\n            <a data-toggle=\"tab\" href=\"#s3\"><i class=\"fa fa-dollar\"></i> <span class=\"hidden-mobile hidden-tablet\">Revenue</span></a>\r\n        </li>\r\n    </ul>\r\n\r\n</header>\r\n\r\n<!-- widget div-->\r\n<div class=\"no-padding\">\r\n\r\n    <div class=\"widget-body\">\r\n        <!-- content -->\r\n        <div id=\"myTabContent\" class=\"tab-content\">\r\n            <div class=\"tab-pane fade active in padding-10 no-padding-bottom\" id=\"s1\">\r\n                <div class=\"row no-space\">\r\n                    <div class=\"col-xs-12 col-sm-12 col-md-8 col-lg-8\">\r\n														<span class=\"demo-liveupdate-1\"> <span\r\n                                                                class=\"onoffswitch-title\">Live switch</span> <span\r\n                                                                class=\"onoffswitch\">\r\n																<input type=\"checkbox\" name=\"start_interval\" ng-model=\"autoUpdate\"\r\n                                                                       class=\"onoffswitch-checkbox\" id=\"start_interval\">\r\n																<label class=\"onoffswitch-label\" for=\"start_interval\">\r\n                                                                    <span class=\"onoffswitch-inner\"\r\n                                                                          data-swchon-text=\"ON\"\r\n                                                                          data-swchoff-text=\"OFF\"></span>\r\n                                                                    <span class=\"onoffswitch-switch\"></span>\r\n                                                                </label> </span> </span>\r\n\r\n                        <div id=\"updating-chart\" class=\"chart-large txt-color-blue\" flot-basic flot-data=\"liveStats\" flot-options=\"liveStatsOptions\"></div>\r\n\r\n                    </div>\r\n                    <div class=\"col-xs-12 col-sm-12 col-md-4 col-lg-4 show-stats\">\r\n\r\n                        <div class=\"row\">\r\n                            <div class=\"col-xs-6 col-sm-6 col-md-12 col-lg-12\"><span class=\"text\"> My Tasks <span\r\n                                    class=\"pull-right\">130/200</span> </span>\r\n\r\n                                <div class=\"progress\">\r\n                                    <div class=\"progress-bar bg-color-blueDark\" style=\"width: 65%;\"></div>\r\n                                </div>\r\n                            </div>\r\n                            <div class=\"col-xs-6 col-sm-6 col-md-12 col-lg-12\"><span class=\"text\"> Transfered <span\r\n                                    class=\"pull-right\">440 GB</span> </span>\r\n\r\n                                <div class=\"progress\">\r\n                                    <div class=\"progress-bar bg-color-blue\" style=\"width: 34%;\"></div>\r\n                                </div>\r\n                            </div>\r\n                            <div class=\"col-xs-6 col-sm-6 col-md-12 col-lg-12\"><span class=\"text\"> Bugs Squashed<span\r\n                                    class=\"pull-right\">77%</span> </span>\r\n\r\n                                <div class=\"progress\">\r\n                                    <div class=\"progress-bar bg-color-blue\" style=\"width: 77%;\"></div>\r\n                                </div>\r\n                            </div>\r\n                            <div class=\"col-xs-6 col-sm-6 col-md-12 col-lg-12\"><span class=\"text\"> User Testing <span\r\n                                    class=\"pull-right\">7 Days</span> </span>\r\n\r\n                                <div class=\"progress\">\r\n                                    <div class=\"progress-bar bg-color-greenLight\" style=\"width: 84%;\"></div>\r\n                                </div>\r\n                            </div>\r\n\r\n                            <span class=\"show-stat-buttons\"> <span class=\"col-xs-12 col-sm-6 col-md-6 col-lg-6\"> <a\r\n                                    href-void class=\"btn btn-default btn-block hidden-xs\">Generate PDF</a> </span> <span\r\n                                    class=\"col-xs-12 col-sm-6 col-md-6 col-lg-6\"> <a href-void\r\n                                                                                     class=\"btn btn-default btn-block hidden-xs\">Report\r\n                                a bug</a> </span> </span>\r\n\r\n                        </div>\r\n\r\n                    </div>\r\n                </div>\r\n\r\n                <div class=\"show-stat-microcharts\" data-sparkline-container data-easy-pie-chart-container>\r\n                    <div class=\"col-xs-12 col-sm-3 col-md-3 col-lg-3\">\r\n\r\n                        <div class=\"easy-pie-chart txt-color-orangeDark\" data-percent=\"33\" data-pie-size=\"50\">\r\n                            <span class=\"percent percent-sign\">35</span>\r\n                        </div>\r\n                        <span class=\"easy-pie-title\"> Server Load <i class=\"fa fa-caret-up icon-color-bad\"></i> </span>\r\n                        <ul class=\"smaller-stat hidden-sm pull-right\">\r\n                            <li>\r\n                                <span class=\"label bg-color-greenLight\"><i class=\"fa fa-caret-up\"></i> 97%</span>\r\n                            </li>\r\n                            <li>\r\n                                <span class=\"label bg-color-blueLight\"><i class=\"fa fa-caret-down\"></i> 44%</span>\r\n                            </li>\r\n                        </ul>\r\n                        <div class=\"sparkline txt-color-greenLight hidden-sm hidden-md pull-right\"\r\n                             data-sparkline-type=\"line\" data-sparkline-height=\"33px\" data-sparkline-width=\"70px\"\r\n                             data-fill-color=\"transparent\">\r\n                            130, 187, 250, 257, 200, 210, 300, 270, 363, 247, 270, 363, 247\r\n                        </div>\r\n                    </div>\r\n                    <div class=\"col-xs-12 col-sm-3 col-md-3 col-lg-3\">\r\n                        <div class=\"easy-pie-chart txt-color-greenLight\" data-percent=\"78.9\" data-pie-size=\"50\">\r\n                            <span class=\"percent percent-sign\">78.9 </span>\r\n                        </div>\r\n                        <span class=\"easy-pie-title\"> Disk Space <i class=\"fa fa-caret-down icon-color-good\"></i></span>\r\n                        <ul class=\"smaller-stat hidden-sm pull-right\">\r\n                            <li>\r\n                                <span class=\"label bg-color-blueDark\"><i class=\"fa fa-caret-up\"></i> 76%</span>\r\n                            </li>\r\n                            <li>\r\n                                <span class=\"label bg-color-blue\"><i class=\"fa fa-caret-down\"></i> 3%</span>\r\n                            </li>\r\n                        </ul>\r\n                        <div class=\"sparkline txt-color-blue hidden-sm hidden-md pull-right\" data-sparkline-type=\"line\"\r\n                             data-sparkline-height=\"33px\" data-sparkline-width=\"70px\" data-fill-color=\"transparent\">\r\n                            257, 200, 210, 300, 270, 363, 130, 187, 250, 247, 270, 363, 247\r\n                        </div>\r\n                    </div>\r\n                    <div class=\"col-xs-12 col-sm-3 col-md-3 col-lg-3\">\r\n                        <div class=\"easy-pie-chart txt-color-blue\" data-percent=\"23\" data-pie-size=\"50\">\r\n                            <span class=\"percent percent-sign\">23 </span>\r\n                        </div>\r\n                        <span class=\"easy-pie-title\"> Transfered <i class=\"fa fa-caret-up icon-color-good\"></i></span>\r\n                        <ul class=\"smaller-stat hidden-sm pull-right\">\r\n                            <li>\r\n                                <span class=\"label bg-color-darken\">10GB</span>\r\n                            </li>\r\n                            <li>\r\n                                <span class=\"label bg-color-blueDark\"><i class=\"fa fa-caret-up\"></i> 10%</span>\r\n                            </li>\r\n                        </ul>\r\n                        <div class=\"sparkline txt-color-darken hidden-sm hidden-md pull-right\"\r\n                             data-sparkline-type=\"line\" data-sparkline-height=\"33px\" data-sparkline-width=\"70px\"\r\n                             data-fill-color=\"transparent\">\r\n                            200, 210, 363, 247, 300, 270, 130, 187, 250, 257, 363, 247, 270\r\n                        </div>\r\n                    </div>\r\n                    <div class=\"col-xs-12 col-sm-3 col-md-3 col-lg-3\">\r\n                        <div class=\"easy-pie-chart txt-color-darken\" data-percent=\"36\" data-pie-size=\"50\">\r\n                            <span class=\"percent degree-sign\">36 <i class=\"fa fa-caret-up\"></i></span>\r\n                        </div>\r\n                        <span class=\"easy-pie-title\"> Temperature <i\r\n                                class=\"fa fa-caret-down icon-color-good\"></i></span>\r\n                        <ul class=\"smaller-stat hidden-sm pull-right\">\r\n                            <li>\r\n                                <span class=\"label bg-color-red\"><i class=\"fa fa-caret-up\"></i> 124</span>\r\n                            </li>\r\n                            <li>\r\n                                <span class=\"label bg-color-blue\"><i class=\"fa fa-caret-down\"></i> 40 F</span>\r\n                            </li>\r\n                        </ul>\r\n                        <div class=\"sparkline txt-color-red hidden-sm hidden-md pull-right\" data-sparkline-type=\"line\"\r\n                             data-sparkline-height=\"33px\" data-sparkline-width=\"70px\" data-fill-color=\"transparent\">\r\n                            2700, 3631, 2471, 2700, 3631, 2471, 1300, 1877, 2500, 2577, 2000, 2100, 3000\r\n                        </div>\r\n                    </div>\r\n                </div>\r\n\r\n            </div>\r\n            <!-- end s1 tab pane -->\r\n\r\n            <div class=\"tab-pane fade\" id=\"s2\">\r\n                <div class=\"widget-body-toolbar bg-color-white\">\r\n\r\n                    <form class=\"form-inline\" role=\"form\">\r\n\r\n                        <div class=\"form-group\">\r\n                            <label class=\"sr-only\" for=\"s123\">Show From</label>\r\n                            <input type=\"email\" class=\"form-control input-sm\" id=\"s123\" placeholder=\"Show From\">\r\n                        </div>\r\n                        <div class=\"form-group\">\r\n                            <input type=\"email\" class=\"form-control input-sm\" id=\"s124\" placeholder=\"To\">\r\n                        </div>\r\n\r\n                        <div class=\"btn-group hidden-phone pull-right\">\r\n                            <a class=\"btn dropdown-toggle btn-xs btn-default\" data-toggle=\"dropdown\"><i\r\n                                    class=\"fa fa-cog\"></i> More <span class=\"caret\"> </span> </a>\r\n                            <ul class=\"dropdown-menu pull-right\">\r\n                                <li>\r\n                                    <a href-void><i class=\"fa fa-file-text-alt\"></i> Export to PDF</a>\r\n                                </li>\r\n                                <li>\r\n                                    <a href-void><i class=\"fa fa-question-sign\"></i> Help</a>\r\n                                </li>\r\n                            </ul>\r\n                        </div>\r\n\r\n                    </form>\r\n\r\n                </div>\r\n                <div class=\"padding-10\">\r\n                    <div id=\"statsChart\" class=\"chart-large has-legend-unique\" flot-basic flot-data=\"statsData\" flot-options=\"statsDisplayOptions\"></div>\r\n                </div>\r\n\r\n            </div>\r\n            <!-- end s2 tab pane -->\r\n\r\n            <div class=\"tab-pane fade\" id=\"s3\">\r\n\r\n                <div class=\"widget-body-toolbar bg-color-white smart-form\" id=\"rev-toggles\">\r\n\r\n                    <div class=\"inline-group\">\r\n\r\n                        <label for=\"gra-0\" class=\"checkbox\">\r\n                            <input type=\"checkbox\" id=\"gra-0\" ng-model=\"targetsShow\">\r\n                            <i></i> Target </label>\r\n                        <label for=\"gra-1\" class=\"checkbox\">\r\n                            <input type=\"checkbox\" id=\"gra-1\" ng-model=\"actualsShow\">\r\n                            <i></i> Actual </label>\r\n                        <label for=\"gra-2\" class=\"checkbox\">\r\n                            <input type=\"checkbox\" id=\"gra-2\" ng-model=\"signupsShow\">\r\n                            <i></i> Signups </label>\r\n                    </div>\r\n\r\n                    <div class=\"btn-group hidden-phone pull-right\">\r\n                        <a class=\"btn dropdown-toggle btn-xs btn-default\" data-toggle=\"dropdown\"><i\r\n                                class=\"fa fa-cog\"></i> More <span class=\"caret\"> </span> </a>\r\n                        <ul class=\"dropdown-menu pull-right\">\r\n                            <li>\r\n                                <a href-void><i class=\"fa fa-file-text-alt\"></i> Export to PDF</a>\r\n                            </li>\r\n                            <li>\r\n                                <a href-void><i class=\"fa fa-question-sign\"></i> Help</a>\r\n                            </li>\r\n                        </ul>\r\n                    </div>\r\n\r\n                </div>\r\n\r\n                <div class=\"padding-10\">\r\n                    <div id=\"flotcontainer\" class=\"chart-large has-legend-unique\" flot-basic flot-data=\"revenewData\" flot-options=\"revenewDisplayOptions\" ></div>\r\n                </div>\r\n            </div>\r\n            <!-- end s3 tab pane -->\r\n        </div>\r\n\r\n        <!-- end content -->\r\n    </div>\r\n\r\n</div>\r\n<!-- end widget div -->\r\n</div>\r\n");
$templateCache.put("app/layout/layout.tpl.html","<div ng-intro-options=\"IntroOptions\" ng-intro-method=\"CallMe\">\r\n<!-- HEADER -->\r\n<div data-smart-include=\"app/layout/partials/header.tpl.html\" class=\"placeholder-header\"></div>\r\n<!-- END HEADER -->\r\n\r\n\r\n<!-- Left panel : Navigation area -->\r\n<!-- Note: This width of the aside area can be adjusted through LESS variables -->\r\n<div data-smart-include=\"app/layout/partials/navigation.tpl.html\" class=\"placeholder-left-panel\"></div>\r\n\r\n<!-- END NAVIGATION -->\r\n\r\n<!-- MAIN PANEL -->\r\n<div id=\"main\" role=\"main\">\r\n    <demo-states></demo-states>\r\n\r\n    <!-- RIBBON -->\r\n    <div id=\"ribbon\">\r\n\r\n				<span id=\"reset-settings\" class=\"ribbon-button-alignment\">\r\n					<span id=\"refresh\" class=\"btn btn-ribbon\" reset-widgets\r\n                          tooltip-placement=\"bottom\"\r\n                          tooltip-html=\"<i class=\'text-warning fa fa-warning\'></i> Warning! This will reset all your widget settings.\">\r\n						<i class=\"fa fa-refresh\"></i>\r\n					</span>\r\n				</span>\r\n\r\n        <!-- breadcrumb -->\r\n        <state-breadcrumbs></state-breadcrumbs>\r\n        <!-- end breadcrumb -->\r\n\r\n\r\n    </div>\r\n    <!-- END RIBBON -->\r\n\r\n\r\n    <div data-smart-router-animation-wrap=\"content content@app\" data-wrap-for=\"#content\">\r\n        <div data-ui-view=\"content\" data-autoscroll=\"false\"></div>\r\n    </div>\r\n\r\n</div>\r\n<!-- END MAIN PANEL -->\r\n\r\n<!-- PAGE FOOTER -->\r\n<div data-smart-include=\"app/layout/partials/footer.tpl.html\"></div>\r\n\r\n<div data-smart-include=\"app/layout/shortcut/shortcut.tpl.html\"></div>\r\n\r\n<!-- END PAGE FOOTER -->\r\n </div>\r\n\r\n");
$templateCache.put("app/auth/directives/login-info.tpl.html","<div class=\"login-info ng-cloak\">\r\n    <span> <!-- User image size is adjusted inside CSS, it should stay as it -->\r\n        <a id=\"my-login-info\" href=\"\" toggle-shortcut>\r\n            <img ng-src=\"{{user.image()}}\" alt=\"me\" class=\"online\">\r\n                <span>{{user.displayName}}\r\n                </span>\r\n            <i class=\"fa fa-angle-down\"></i>\r\n        </a>\r\n     </span>\r\n</div>");
$templateCache.put("app/dashboard/projects/recent-projects.tpl.html","<div class=\"project-context hidden-xs dropdown\" dropdown>\r\n\r\n    <span class=\"label\">{{getWord(\'Projects\')}}:</span>\r\n    <span class=\"project-selector dropdown-toggle\" dropdown-toggle>{{getWord(\'Recent projects\')}} <i ng-if=\"projects.length\"\r\n            class=\"fa fa-angle-down\"></i></span>\r\n\r\n    <ul class=\"dropdown-menu\" ng-if=\"projects.length\">\r\n        <li ng-repeat=\"project in projects\">\r\n            <a href=\"{{project.href}}\">{{project.title}}</a>\r\n        </li>\r\n        <li class=\"divider\"></li>\r\n        <li>\r\n            <a ng-click=\"clearProjects()\"><i class=\"fa fa-power-off\"></i> Clear</a>\r\n        </li>\r\n    </ul>\r\n\r\n</div>");
$templateCache.put("app/dashboard/todo/todo-widget.tpl.html","<div id=\"todo-widget\" jarvis-widget data-widget-editbutton=\"false\" data-widget-color=\"blue\"\r\n     ng-controller=\"TodoCtrl\">\r\n    <header>\r\n        <span class=\"widget-icon\"> <i class=\"fa fa-check txt-color-white\"></i> </span>\r\n\r\n        <h2> ToDo\'s </h2>\r\n\r\n        <div class=\"widget-toolbar\">\r\n            <!-- add: non-hidden - to disable auto hide -->\r\n            <button class=\"btn btn-xs btn-default\" ng-class=\"{active: newTodo}\" ng-click=\"toggleAdd()\"><i ng-class=\"{ \'fa fa-plus\': !newTodo, \'fa fa-times\': newTodo}\"></i> Add</button>\r\n\r\n        </div>\r\n    </header>\r\n    <!-- widget div-->\r\n    <div>\r\n        <div class=\"widget-body no-padding smart-form\">\r\n            <!-- content goes here -->\r\n            <div ng-show=\"newTodo\">\r\n                <h5 class=\"todo-group-title\"><i class=\"fa fa-plus-circle\"></i> New Todo</h5>\r\n\r\n                <form name=\"newTodoForm\" class=\"smart-form\">\r\n                    <fieldset>\r\n                        <section>\r\n                            <label class=\"input\">\r\n                                <input type=\"text\" required class=\"input-lg\" ng-model=\"newTodo.title\"\r\n                                       placeholder=\"What needs to be done?\">\r\n                            </label>\r\n                        </section>\r\n                        <section>\r\n                            <div class=\"col-xs-6\">\r\n                                <label class=\"select\">\r\n                                    <select class=\"input-sm\" ng-model=\"newTodo.state\"\r\n                                            ng-options=\"state as state for state in states\"></select> <i></i> </label>\r\n                            </div>\r\n                        </section>\r\n                    </fieldset>\r\n                    <footer>\r\n                        <button ng-disabled=\"newTodoForm.$invalid\" type=\"button\" class=\"btn btn-primary\"\r\n                                ng-click=\"createTodo()\">\r\n                            Add\r\n                        </button>\r\n                        <button type=\"button\" class=\"btn btn-default\" ng-click=\"toggleAdd()\">\r\n                            Cancel\r\n                        </button>\r\n                    </footer>\r\n                </form>\r\n            </div>\r\n\r\n            <todo-list state=\"Critical\"  title=\"Critical Tasks\" icon=\"warning\" todos=\"todos\"></todo-list>\r\n\r\n            <todo-list state=\"Important\" title=\"Important Tasks\" icon=\"exclamation\" todos=\"todos\"></todo-list>\r\n\r\n            <todo-list state=\"Completed\" title=\"Completed Tasks\" icon=\"check\" todos=\"todos\"></todo-list>\r\n\r\n            <!-- end content -->\r\n        </div>\r\n\r\n    </div>\r\n    <!-- end widget div -->\r\n</div>");
$templateCache.put("app/homepage/views/sub-header.tpl.html","<div class=\"col-xs-12 col-sm-5 col-md-5 col-lg-8\" data-sparkline-container>\r\n    <ul id=\"sparks\" class=\"\">\r\n        <li class=\"sparks-info\">\r\n            <h5> {{getWord(\'TaskCount\')}} <span class=\"txt-color-blue\">1,271</span></h5>\r\n            <div class=\"sparkline txt-color-blue hidden-mobile hidden-md hidden-sm\">\r\n                130, 187, 250, 257, 200, 210, 300, 270, 363, 247, 270, 363, 247\r\n            </div>\r\n        </li>\r\n        <li class=\"sparks-info\">\r\n            <h5> {{getWord(\"Efficiency\")}} <span class=\"txt-color-purple\"><i class=\"fa fa-arrow-circle-up\"></i>&nbsp;25%</span></h5>\r\n            <div class=\"sparkline txt-color-purple hidden-mobile hidden-md hidden-sm\">\r\n                110,150,300,130,400,240,220,310,220,300, 270, 210\r\n            </div>\r\n        </li>\r\n        <li class=\"sparks-info\">\r\n            <h5> {{getWord(\"CompletedTasks\")}} <span class=\"txt-color-greenDark\"><i class=\"fa fa-check-circle\"></i>&nbsp;879</span></h5>\r\n            <div class=\"sparkline txt-color-greenDark hidden-mobile hidden-md hidden-sm\">\r\n                110,150,300,130,400,240,220,310,220,300, 270, 210\r\n            </div>\r\n        </li>\r\n    </ul>\r\n</div>\r\n			");
$templateCache.put("app/layout/language/language-selector.tpl.html","<ul class=\"header-dropdown-list hidden-xs ng-cloak\" ng-controller=\"LanguagesCtrl\">\r\n    <li class=\"dropdown\" dropdown>\r\n        <a class=\"dropdown-toggle\"  dropdown-toggle href> <img src=\"styles/img/blank.gif\" class=\"flag flag-{{currentLanguage.key}}\" alt=\"{{currentLanguage.alt}}\"> <span> {{currentLanguage.title}} </span>\r\n            <i class=\"fa fa-angle-down\"></i> </a>\r\n        <ul class=\"dropdown-menu pull-right\">\r\n            <li ng-class=\"{active: language==currentLanguage}\" ng-repeat=\"language in languages\">\r\n                <a ng-click=\"selectLanguage(language)\" ><img src=\"styles/img/blank.gif\" class=\"flag flag-{{language.key}}\"\r\n                                                   alt=\"{{language.alt}}\"> {{language.title}}</a>\r\n            </li>\r\n        </ul>\r\n    </li>\r\n</ul>");
$templateCache.put("app/layout/partials/footer.tpl.html","<div class=\"page-footer\">\r\n    <div class=\"row\">\r\n        <div class=\"col-xs-12 col-sm-6\">\r\n            <span class=\"txt-color-white\">Newtera TDM</span>\r\n        </div>\r\n\r\n        <div class=\"col-xs-6 col-sm-6 text-right hidden-xs\">\r\n            \r\n        </div>\r\n    </div>\r\n</div>");
$templateCache.put("app/layout/partials/header.tpl.html","<header id=\"header\">\r\n<div id=\"logo-group\">\r\n\r\n    <!-- PLACE YOUR LOGO HERE -->\r\n    <span id=\"logo\"><a ui-sref=\"app.homepage.mainmenu\"><img src=\"styles/img/logo.gif\" alt=\"TDM\"></a>\r\n    </span>\r\n    <!-- END LOGO PLACEHOLDER -->\r\n\r\n    <!-- Note: The activity badge color changes when clicked and resets the number to 0\r\n    Suggestion: You may want to set a flag when this happens to tick off all checked messages / notifications -->\r\n   \r\n    <!--\r\n    <span id=\"activity\"> \r\n        <a ui-sref=\"app.tasks.list\" ui-sref-opts=\"{reload: true}\" title=\"{{getWord(\'MyTasks\')}}\"><i class=\"fa fa-user\"></i>\r\n            <b class=\"badge bg-color-red\">{{getTaskCount()}}</b>\r\n        </a>\r\n    </span>\r\n    -->\r\n\r\n    <!-- Note: The activity badge color changes when clicked and resets the number to 0\r\n    Suggestion: You may want to set a flag when this happens to tick off all checked messages / notifications -->\r\n    <span id=\"activity\" class=\"activity-dropdown\" activities-dropdown-toggle>\r\n        <i class=\"fa fa-user\"></i>\r\n        <b class=\"badge bg-color-red\">{{getTotalCount()}}</b>\r\n    </span>\r\n    <div smart-include=\"app/homepage/views/my-activities.html\"></div>\r\n</div>\r\n\r\n<!--\r\n<div style=\"margin-left:300px\">\r\n    <h1 class=\"text-primary hidden-xs hidden-sm hidden-md\">{{getWord(\'AppName\')}}</h1>\r\n</div>\r\n-->\r\n\r\n<!-- pulled right: nav area -->\r\n<div class=\"pull-right\">\r\n\r\n    <!-- intro button -->\r\n    <div id=\"intro\" class=\"btn-header transparent pull-right\">\r\n        <span>\r\n            <a title=\"{{getWord(\'Intro\')}}\" ng-click=\"CallMe();\">\r\n                <i class=\"fa fa-info\"></i>\r\n            </a>\r\n        </span>\r\n    </div>\r\n    <!-- end intro button -->\r\n    <!-- collapse menu button -->\r\n    <div id=\"hide-menu\" class=\"btn-header pull-right\">\r\n        <span>\r\n            <a toggle-menu title=\"{{getWord(\'CollapseMenu\')}}\">\r\n                <i class=\"fa fa-reorder\"></i>\r\n            </a>\r\n        </span>\r\n    </div>\r\n    <!-- end collapse menu -->\r\n    <!-- #MOBILE -->\r\n    <!-- Top menu profile link : this shows only when top menu is active -->\r\n    <ul id=\"mobile-profile-img\" class=\"header-dropdown-list hidden-xs padding-5\">\r\n        <li class=\"\">\r\n            <a href=\"#\" class=\"dropdown-toggle no-margin userdropdown\" data-toggle=\"dropdown\">\r\n                <img src=\"styles/custom/avatars/male.png\" alt=\"John Doe\" class=\"online\" />\r\n            </a>\r\n            <ul class=\"dropdown-menu pull-right\">\r\n                <li>\r\n                    <a href-void class=\"padding-10 padding-top-0 padding-bottom-0\">\r\n                        <i class=\"fa fa-cog\"></i> Setting\r\n                    </a>\r\n                </li>\r\n                <li class=\"divider\"></li>\r\n                <li>\r\n                    <a ui-sref=\"app.appViews.profileDemo\" class=\"padding-10 padding-top-0 padding-bottom-0\">\r\n                        <i class=\"fa fa-user\"></i>\r\n                        <u>P</u>rofile\r\n                    </a>\r\n                </li>\r\n                <li class=\"divider\"></li>\r\n                <li>\r\n                    <a href-void class=\"padding-10 padding-top-0 padding-bottom-0\"\r\n                       data-action=\"toggleShortcut\"><i class=\"fa fa-arrow-down\"></i> <u>S</u>hortcut</a>\r\n                </li>\r\n                <li class=\"divider\"></li>\r\n                <li>\r\n                    <a href-void class=\"padding-10 padding-top-0 padding-bottom-0\"\r\n                       data-action=\"launchFullscreen\"><i class=\"fa fa-arrows-alt\"></i> Full <u>S</u>creen</a>\r\n                </li>\r\n                <li class=\"divider\"></li>\r\n                <li>\r\n                    <a href=\"#/login\" class=\"padding-10 padding-top-5 padding-bottom-5\" data-action=\"userLogout\">\r\n                        <i class=\"fa fa-sign-out fa-lg\"></i> <strong><u>L</u>ogout</strong>\r\n                    </a>\r\n                </li>\r\n            </ul>\r\n        </li>\r\n    </ul>\r\n\r\n    <!-- logout button -->\r\n    <div id=\"logout\" class=\"btn-header transparent pull-right\">\r\n        <span>\r\n            <a ui-sref=\"logout\" title=\"{{getWord(\'SignOut\')}}\" data-action=\"userLogout\"\r\n               data-logout-msg=\"You can improve your security further after logging out by closing this opened browser\">\r\n                <i class=\"fa fa-sign-out\"></i>\r\n            </a>\r\n        </span>\r\n    </div>\r\n    <!-- end logout button -->\r\n    <!-- search mobile button (this is hidden till mobile view port) -->\r\n    <div id=\"search-mobile\" class=\"btn-header transparent pull-right\" data-search-mobile>\r\n        <span> <a href=\"#\" title=\"Search\"><i class=\"fa fa-search\"></i></a> </span>\r\n    </div>\r\n    <!-- end search mobile button -->\r\n    <!-- fullscreen button -->\r\n    <div id=\"fullscreen\" class=\"btn-header transparent pull-right\">\r\n        <span>\r\n            <a full-screen title=\"{{getWord(\'FullScreen\')}}\">\r\n                <i class=\"fa fa-arrows-alt\"></i>\r\n            </a>\r\n        </span>\r\n    </div>\r\n    <!-- end fullscreen button -->\r\n    <!-- multiple lang dropdown : find all flags in the flags page -->\r\n    <language-selector></language-selector>\r\n    <i ng-show=\"loadingNodeLabels\" class=\"glyphicon glyphicon-refresh\"></i>\r\n    <!-- end multiple lang -->\r\n    <!-- input: full text search field -->\r\n    <form ng-show=\"searchEnabled\" ng-submit=\"fullTextSearch()\" class=\"header-search pull-right\" style=\"padding-right:10px\">\r\n        <input id=\"search-keywords\" name=\"searchKeywords\" type=\"text\" autocomplete=\"off\" size=\"50\"\r\n               placeholder=\"{{getWord(\'FindReports\')}}\"\r\n               ng-model=\"searchContext.searchText\"\r\n               typeahead-input-formatter=\"formatLabel($model)\"\r\n               typeahead=\"suggestion as suggestion.fullText.display for suggestion in getSuggestions($viewValue) | limitTo:10\"\r\n               typeahead-loading=\"loadingNodeLabels\"\r\n               typeahead-on-select=\"onSuggestionSelect($item, $model, $label)\">\r\n        <button type=\"reset\">&times;</button>\r\n    </form>\r\n    <!-- end input: search field -->\r\n\r\n</div>\r\n<!-- end pulled right: nav area -->\r\n\r\n</header>");
$templateCache.put("app/layout/partials/help-viewer.tpl.html","<div id=\"content\" class=\"wrapper\">\r\n    <!-- widget grid -->\r\n    <section widget-grid id=\"widget-grid\">\r\n        <div class=\"row\">\r\n            <article class=\"col-sm-12 col-md-12 col-lg-12\">\r\n                <div jarvis-widget id=\"wid-id-1\" data-widget-editbutton=\"false\" data-widget-custombutton=\"false\">\r\n                    <header>\r\n                        <span class=\"widget-icon\"> <i class=\"fa fa-edit\"></i> </span>\r\n                        <h2>{{getWord(\"Help\")}}</h2>\r\n                        <div class=\"widget-toolbar\">\r\n                            <span class=\"jarviswidget-ctrls\" ng-click=\"$dismiss()\"> <i class=\"fa fa-close\"></i> </span>\r\n                        </div>\r\n                    </header>\r\n                    <div style=\"overflow-x:auto;\">\r\n                        <!-- widget content -->\r\n                        <div align=\"center\" class=\"padding-10\">\r\n                            <button ng-click=\"zoomIn()\"><i class=\"fa fa-plus\"></i></button>\r\n                            <button ng-click=\"fit()\"><span>100%</span></button>\r\n                            <button ng-click=\"zoomOut()\"><i class=\"fa fa-minus\"></i></button>\r\n                        </div>\r\n                        <div>\r\n                            <ng-pdf template-url=\"app/layout/partials/help-viewer..html\" scale=\"page-fit\"></ng-pdf>\r\n                        </div>\r\n                        <div align=\"center\" class=\"padding-10\">\r\n                            <button ng-click=\"goPrevious()\"><i class=\"fa fa-chevron-left\"></i><span> {{getWord(\"Prev Page\")}}</span></button>\r\n                            <button ng-click=\"goNext()\"><i class=\"fa fa-chevron-right\"></i><span> {{getWord(\"Next Page\")}}</span></button>\r\n                        </div>\r\n                    </div>\r\n                </div>\r\n            </article>\r\n        </div>\r\n    </section>\r\n</div>");
$templateCache.put("app/layout/partials/navigation.tpl.html","<aside id=\"left-panel\">\r\n\r\n    <!-- User info -->\r\n    <div login-info></div>\r\n    <!-- end user info -->\r\n\r\n    <nav id=\"sidemenu\">\r\n\r\n        <!-- NOTE: This allows you to pull menu items from server -->\r\n        <ul data-smart-menu-items=\"/api/sitemap/menu\"></ul>\r\n\r\n    </nav>\r\n\r\n  <span id=\"minimize-sidemenu\" class=\"minifyme\" data-action=\"minifyMenu\" minify-menu>\r\n    <i class=\"fa fa-arrow-circle-left hit\"></i>\r\n  </span>\r\n\r\n</aside>");
$templateCache.put("app/layout/partials/sub-header.tpl.html","<div class=\"col-xs-12 col-sm-5 col-md-5 col-lg-8\" data-sparkline-container>\r\n    <ul id=\"sparks\" class=\"\">\r\n        <li class=\"sparks-info\">\r\n            <h5> My Income <span class=\"txt-color-blue\">$47,171</span></h5>\r\n            <div class=\"sparkline txt-color-blue hidden-mobile hidden-md hidden-sm\">\r\n                1300, 1877, 2500, 2577, 2000, 2100, 3000, 2700, 3631, 2471, 2700, 3631, 2471\r\n            </div>\r\n        </li>\r\n        <li class=\"sparks-info\">\r\n            <h5> Site Traffic <span class=\"txt-color-purple\"><i class=\"fa fa-arrow-circle-up\"></i>&nbsp;45%</span></h5>\r\n            <div class=\"sparkline txt-color-purple hidden-mobile hidden-md hidden-sm\">\r\n                110,150,300,130,400,240,220,310,220,300, 270, 210\r\n            </div>\r\n        </li>\r\n        <li class=\"sparks-info\">\r\n            <h5> Site Orders <span class=\"txt-color-greenDark\"><i class=\"fa fa-shopping-cart\"></i>&nbsp;2447</span></h5>\r\n            <div class=\"sparkline txt-color-greenDark hidden-mobile hidden-md hidden-sm\">\r\n                110,150,300,130,400,240,220,310,220,300, 270, 210\r\n            </div>\r\n        </li>\r\n    </ul>\r\n</div>\r\n			");
$templateCache.put("app/layout/partials/voice-commands.tpl.html","<!-- TRIGGER BUTTON:\r\n<a href=\"/my-ajax-page.html\" data-toggle=\"modal\" data-target=\"#remoteModal\" class=\"btn btn-default\">Open Modal</a>  -->\r\n\r\n<!-- MODAL PLACE HOLDER\r\n<div class=\"modal fade\" id=\"remoteModal\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"remoteModalLabel\" aria-hidden=\"true\">\r\n<div class=\"modal-dialog\">\r\n<div class=\"modal-content\"></div>\r\n</div>\r\n</div>   -->\r\n<!--////////////////////////////////////-->\r\n\r\n<!--<div class=\"modal-header\">\r\n<button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-hidden=\"true\">\r\n&times;\r\n</button>\r\n<h4 class=\"modal-title\" id=\"myModalLabel\">Command List</h4>\r\n</div>-->\r\n<div class=\"modal-body\">\r\n\r\n	<h1><i class=\"fa fa-microphone text-muted\"></i>&nbsp;&nbsp; SmartAdmin Voice Command</h1>\r\n	<hr class=\"simple\">\r\n	<h5>Instruction</h5>\r\n\r\n	Click <span class=\"text-success\">\"Allow\"</span> to access your microphone and activate Voice Command.\r\n	You will notice a <span class=\"text-primary\"><strong>BLUE</strong> Flash</span> on the microphone icon indicating activation.\r\n	The icon will appear <span class=\"text-danger\"><strong>RED</strong></span> <span class=\"label label-danger\"><i class=\"fa fa-microphone fa-lg\"></i></span> if you <span class=\"text-danger\">\"Deny\"</span> access or don\'t have any microphone installed.\r\n	<br>\r\n	<br>\r\n	As a security precaution, your browser will disconnect the microphone every 60 to 120 seconds (sooner if not being used). In which case Voice Command will prompt you again to <span class=\"text-success\">\"Allow\"</span> or <span class=\"text-danger\">\"Deny\"</span> access to your microphone.\r\n	<br>\r\n	<br>\r\n	If you host your page over <strong>http<span class=\"text-success\">s</span></strong> (secure socket layer) protocol you can wave this security measure and have an unintrupted Voice Command.\r\n	<br>\r\n	<br>\r\n	<h5>Commands</h5>\r\n	<ul>\r\n		<li>\r\n			<strong>\'show\' </strong> then say the <strong>*page*</strong> you want to go to. For example <strong>\"show inbox\"</strong> or <strong>\"show calendar\"</strong>\r\n		</li>\r\n		<li>\r\n			<strong>\'mute\' </strong> - mutes all sound effects for the theme.\r\n		</li>\r\n		<li>\r\n			<strong>\'sound on\'</strong> - unmutes all sound effects for the theme.\r\n		</li>\r\n		<li>\r\n			<span class=\"text-danger\"><strong>\'stop\'</strong></span> - deactivates voice command.\r\n		</li>\r\n		<li>\r\n			<span class=\"text-primary\"><strong>\'help\'</strong></span> - brings up the command list\r\n		</li>\r\n		<li>\r\n			<span class=\"text-danger\"><strong>\'got it\'</strong></span> - closes help modal\r\n		</li>\r\n		<li>\r\n			<strong>\'hide navigation\'</strong> - toggle navigation collapse\r\n		</li>\r\n		<li>\r\n			<strong>\'show navigation\'</strong> - toggle navigation to open (can be used again to close)\r\n		</li>\r\n		<li>\r\n			<strong>\'scroll up\'</strong> - scrolls to the top of the page\r\n		</li>\r\n		<li>\r\n			<strong>\'scroll down\'</strong> - scrollts to the bottom of the page\r\n		</li>\r\n		<li>\r\n			<strong>\'go back\' </strong> - goes back in history (history -1 click)\r\n		</li>\r\n		<li>\r\n			<strong>\'logout\'</strong> - logs you out\r\n		</li>\r\n	</ul>\r\n	<br>\r\n	<h5>Adding your own commands</h5>\r\n	Voice Command supports up to 80 languages. Adding your own commands is extreamly easy. All commands are stored inside <strong>app.config.js</strong> file under the <code>var commands = {...}</code>. \r\n\r\n	<hr class=\"simple\">\r\n	<div class=\"text-right\">\r\n		<button type=\"button\" class=\"btn btn-success btn-lg\" data-dismiss=\"modal\">\r\n			Got it!\r\n		</button>\r\n	</div>\r\n\r\n</div>\r\n<!--<div class=\"modal-footer\">\r\n<button type=\"button\" class=\"btn btn-primary\" data-dismiss=\"modal\">Got it!</button>\r\n</div> -->");
$templateCache.put("app/layout/shortcut/shortcut.tpl.html","<div id=\"shortcut\">\r\n	<ul>\r\n		<li>\r\n			<a href=\"#/user/profile\" class=\"jarvismetro-tile big-cubes selected bg-color-pinkDark\"> <span class=\"iconbox\"> <i class=\"fa fa-user fa-4x\"></i> <span>{{getWord(\'My Profile\')}} </span> </span> </a>\r\n		</li>\r\n        <li>\r\n            <a href=\"#/user/password\" class=\"jarvismetro-tile big-cubes selected bg-color-darken\"> <span class=\"iconbox\"> <i class=\"fa fa-lock fa-4x\"></i> <span>{{getWord(\'Change Password\')}} </span> </span> </a>\r\n        </li>\r\n	</ul>\r\n</div>");
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
"use strict";

angular.module("app.user", ["ngResource", "ui.router", "ui.bootstrap", "ui.bootstrap.modal"]);

angular.module("app.user")
    .config(function ($stateProvider, modalStateProvider) {

        $stateProvider
            .state('app.user', {
                abstract: true,
                data: {
                    title: 'User'
                }
            })
            .state('app.user.profile', {
                url: '/user/profile',
                authenticate: true,
                data: {
                    title: 'User Profile',
                    animation: false /* disable the content loading animation since $viewContentLoaded will not fire when opening modal */
                },
                views: {
                    "content@app": {
                        templateUrl: 'app/user/views/edit-profile.html',
                        controller: 'EditProfileController'
                    }
                },
                resolve: {
                }
            })
            .state('app.user.password', {
                url: '/user/password',
                authenticate: true,
                data: {
                    title: 'Change Password',
                    animation: false /* disable the content loading animation since $viewContentLoaded will not fire when opening modal */
                },
                views: {
                    "content@app": {
                        templateUrl: 'app/user/views/change-password.html',
                        controller: 'ChangePasswordController'
                    }
                },
                resolve: {
                }
            });
    });
"use strict";

angular.module("app.userdirectory", ["ui.router", "ui.bootstrap"]);

angular.module("app.userdirectory").config(function ($stateProvider, modalStateProvider) {

    $stateProvider
        .state('app.userdirectory.userlist', {
            url: '/userdirectory/userlist/:schema/:class?functionObjId&unitObjId',
            data: {
                title: 'User List',
                animation: false /* disable the content loading animation since $viewContentLoaded will not fire when opening modal */
            },
            authenticate: true,
            views: {
                "content@app": {
                    templateUrl: 'app/userdirectory/views/user-list.html',
                    controller: 'UserListCtrl',
                    resolve: {
                        promisedUsers: function ($http, APP_CONFIG, $stateParams) {
                            var pageSize = 500;
                            var url;
                            if ($stateParams.functionObjId) {
                                url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($stateParams.schema) + "/Role/" + $stateParams.functionObjId + "/" + $stateParams.class + "?view=full&size=" + pageSize;
                            }
                            else if ($stateParams.unitObjId) {
                                url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($stateParams.schema) + "/Role/" + $stateParams.unitObjId + "/" + $stateParams.class + "?view=full&size=" + pageSize;

                            }
                            else {
                                url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($stateParams.schema) + "/" + $stateParams.class + "?view=full&size=" + pageSize;
                            }

                            return $http.get(url);
                        }
                    }
                }
            },
            resolve: {
                scripts: function (lazyScript) {
                    return lazyScript.register([
                        'datatables',
                        'datatables-bootstrap',
                        'datatables-colvis',
                        'datatables-tools',
                        'datatables-responsive'
                    ]);
                }
            }
        })
        .state('app.userdirectory', {
            url: '/userdirectory/:schema/:class/:edit/:delete/:insert/:search/:export/:import/:hash',
            data: {
                title: 'user directory',
                animation: false /* disable the content loading animation since $viewContentLoaded will not fire when opening modal */
            },
            views: {
                "content@app": {
                    templateUrl: "app/userdirectory/views/user-directory-layout.html",
                    controller: 'UserDirectoryLayoutCtrl'
                }
            },
            authenticate: true,
            resolve: {
                propmisedParams: function ($http, APP_CONFIG, $stateParams) {
                    return $http.get(APP_CONFIG.ebaasRootUrl + "/api/sitemap/parameters/" + $stateParams.hash)
                }
            }

        })
        .state('app.userdirectory.usertable', {
            url: '/userdirectoryusertable/:schema/:baseclass/:baseoid/:relatedclass/:view/:formtemplate',
            data: {
                title: 'User Table',
                animation: false /* disable the content loading animation since $viewContentLoaded will not fire when opening modal */
            },
            views: {
                "datatable@app.userdirectory": {
                    controller: 'UserListViewCtrl',
                    templateUrl: "app/userdirectory/views/user-list-view.html"
                }
            },
            authenticate: true
        })
        .state('app.userdirectory.roletable', {
            url: '/userdirectoryroletable/:schema/:baseclass/:roletype',
            data: {
                title: 'Role Table',
                animation: false /* disable the content loading animation since $viewContentLoaded will not fire when opening modal */
            },
            views: {
                "datatable@app.userdirectory": {
                    controller: 'RoleListViewCtrl',
                    templateUrl: "app/userdirectory/views/role-list-view.html"
                }
            },
            authenticate: true
        });

        modalStateProvider.state('app.userdirectory.help', {
            url: '^/userdirectoryhelp/:hash',
            templateUrl: "app/layout/partials/help-viewer.tpl.html",
            controller: 'helpViewerCtlr',
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.userdirectory.usertable.modalform', {
            url: '^/userdirectoryusersmodalform/:schema/:class/:oid/:readonly/:template/:formAttribute/:duplicate/:cmd/:sref',
            templateUrl: "app/smartforms/views/ebaas-form-modal.html",
            controller: 'ebaasFormModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.userdirectory.roletable.modalform', {
            url: '^/userdirectoryrolesmodalform/:schema/:class/:oid/:readonly/:template/:formAttribute/:duplicate/:cmd/:sref',
            templateUrl: "app/smartforms/views/ebaas-form-modal.html",
            controller: 'ebaasFormModalCtrl',
            backdrop: 'static', /*  this prevent user interaction with the background  */
            keyboard: false,
            animation: false,
            size: 'lg'
        });

        modalStateProvider.state('app.userdirectory.roletable.modalform.pickpk', {
            url: '^/userdirectoryrolesmodalformpickpk/:pkclass/:property/:filter/:callback',
            templateUrl: "app/smartforms/views/pick-primary-key.html",
            controller: 'pickPrimaryKeyCtrl',
            animation: false,
            size: 'md'
        });

        modalStateProvider.state('app.userdirectory.usertable.assignroles', {
            url: '^/userdirectoryassignroles/:schema/:class/:oid/:roletype/:dataview',
            templateUrl: "app/userdirectory/views/assign-roles.html",
            controller: 'assignRolesCtrl',
            animation: false,
            size: 'md'
        });
    });
(function(){
    "use strict";

    angular.module('SmartAdmin', [
        "SmartAdmin.Forms",
        "SmartAdmin.Layout"
    ]);
})();
    "use strict";


angular.module('app.chat', ['ngSanitize'])
.run(function ($templateCache) {

    $templateCache.put("template/popover/popover.html",
        "<div class=\"popover {{placement}}\" ng-class=\"{ in: isOpen(), fade: animation() }\">\n" +
        "  <div class=\"arrow\"></div>\n" +
        "\n" +
        "  <div class=\"popover-inner\">\n" +
        "      <h3 class=\"popover-title\" ng-bind-html=\"title | unsafe\" ng-show=\"title\"></h3>\n" +
        "      <div class=\"popover-content\"ng-bind-html=\"content | unsafe\"></div>\n" +
        "  </div>\n" +
        "</div>\n" +
        "");

}).filter('unsafe', ['$sce', function ($sce) {
    return function (val) {
        return $sce.trustAsHtml(val);
    };
}]);
(function(){
    "use strict";

    angular.module('SmartAdmin.Forms', []);
})();
(function(){
    "use strict";

    angular.module('SmartAdmin.Layout', []);
})();
(function (app) {
    app.factory('AuthInterceptorService', ['$q', '$location', 'localStorageService', function ($q, $location, localStorageService) {

        var authInterceptorServiceFactory = {};

        var _request = function (config) {

            config.headers = config.headers || {};

            var authData = localStorageService.get('authorizationData');
            if (authData) {
                config.headers.Authorization = 'Bearer ' + authData.token;
            }

            return config;
        }

        var _responseError = function (rejection) {
            if (rejection.status === 401) {
                $location.path('/home/mainmenu');
            }
            return $q.reject(rejection);
        }

        authInterceptorServiceFactory.request = _request;
        authInterceptorServiceFactory.responseError = _responseError;

        return authInterceptorServiceFactory;
    }]);
}(angular.module("app")));
'use strict';

angular.module('app.dashboard').controller('DashboardCtrl', function ($scope, $interval, CalendarEvent) {

    // Live Feeds Widget Data And Display Controls
    // Live Stats Tab


    function getFakeItem(index, prevValue){
        var limitUp = Math.min(100, prevValue + 5),
            limitDown = Math.abs(prevValue - 5);
        return [
            index,
            _.random(limitDown, limitUp, true)
        ]
    }

    function getFakeData() {
        return _(_.range(199)).reduce(function (out, number) {

            out.push(getFakeItem(number+1, _.last(out)[1]));
            return out;
        }, [
            [0, 50] // starting point
        ])
    }

    $scope.autoUpdate = false;

    var updateInterval;
    $scope.$watch('autoUpdate', function(autoUpdate){

        if(autoUpdate){
            updateInterval = $interval(function(){
                var stats = _.rest($scope.liveStats[0]).map(function(elem, i){
                    elem[0] = i;
                    return elem;
                });
                stats.push([199, _.last(stats)[1]]);
                $scope.liveStats = [stats];
            }, 1500)
        } else {
            $interval.cancel(updateInterval);
        }
    });


    $scope.liveStats = [getFakeData()];



    $scope.liveStatsOptions = {
        yaxis: {
            min: 0,
            max: 100
        },
        xaxis: {
            min: 0,
            max: 100
        },
        colors: ['rgb(87, 136, 156)'],
        series: {
            lines: {
                lineWidth: 1,
                fill: true,
                fillColor: {
                    colors: [
                        {
                            opacity: 0.4
                        },
                        {
                            opacity: 0
                        }
                    ]
                },
                steps: false

            }
        }
    };


    // Stats Display With Flot Chart

    var twitter = [
        [1, 27],
        [2, 34],
        [3, 51],
        [4, 48],
        [5, 55],
        [6, 65],
        [7, 61],
        [8, 70],
        [9, 65],
        [10, 75],
        [11, 57],
        [12, 59],
        [13, 62]
    ];
    var facebook = [
        [1, 25],
        [2, 31],
        [3, 45],
        [4, 37],
        [5, 38],
        [6, 40],
        [7, 47],
        [8, 55],
        [9, 43],
        [10, 50],
        [11, 47],
        [12, 39],
        [13, 47]
    ];
    $scope.statsData = [
        {
            label: "Twitter",
            data: twitter,
            lines: {
                show: true,
                lineWidth: 1,
                fill: true,
                fillColor: {
                    colors: [
                        {
                            opacity: 0.1
                        },
                        {
                            opacity: 0.13
                        }
                    ]
                }
            },
            points: {
                show: true
            }
        },
        {
            label: "Facebook",
            data: facebook,
            lines: {
                show: true,
                lineWidth: 1,
                fill: true,
                fillColor: {
                    colors: [
                        {
                            opacity: 0.1
                        },
                        {
                            opacity: 0.13
                        }
                    ]
                }
            },
            points: {
                show: true
            }
        }
    ];

    $scope.statsDisplayOptions = {
        grid: {
            hoverable: true
        },
        colors: ["#568A89", "#3276B1"],
        tooltip: true,
        tooltipOpts: {
            //content : "Value <b>$x</b> Value <span>$y</span>",
            defaultTheme: false
        },
        xaxis: {
            ticks: [
                [1, "JAN"],
                [2, "FEB"],
                [3, "MAR"],
                [4, "APR"],
                [5, "MAY"],
                [6, "JUN"],
                [7, "JUL"],
                [8, "AUG"],
                [9, "SEP"],
                [10, "OCT"],
                [11, "NOV"],
                [12, "DEC"],
                [13, "JAN+1"]
            ]
        },
        yaxes: {

        }
    };


    /* Live stats TAB 3: Revenew  */

    var trgt = [[1354586000000, 153], [1364587000000, 658], [1374588000000, 198], [1384589000000, 663], [1394590000000, 801], [1404591000000, 1080], [1414592000000, 353], [1424593000000, 749], [1434594000000, 523], [1444595000000, 258], [1454596000000, 688], [1464597000000, 364]],
        prft = [[1354586000000, 53], [1364587000000, 65], [1374588000000, 98], [1384589000000, 83], [1394590000000, 980], [1404591000000, 808], [1414592000000, 720], [1424593000000, 674], [1434594000000, 23], [1444595000000, 79], [1454596000000, 88], [1464597000000, 36]],
        sgnups = [[1354586000000, 647], [1364587000000, 435], [1374588000000, 784], [1384589000000, 346], [1394590000000, 487], [1404591000000, 463], [1414592000000, 479], [1424593000000, 236], [1434594000000, 843], [1444595000000, 657], [1454596000000, 241], [1464597000000, 341]];

    var targets = {
        label : "Target Profit",
        data : trgt,
        bars : {
            show : true,
            align : "center",
            barWidth : 30 * 30 * 60 * 1000 * 80
        }
    };
    $scope.targetsShow = true;

    $scope.$watch('targetsShow', function(toggle){
        reveniewElementToggle(targets, toggle);
    });


    var actuals = {
        label : "Actual Profit",
        data : prft,
        color : '#3276B1',
        lines : {
            show : true,
            lineWidth : 3
        },
        points : {
            show : true
        }
    };

    $scope.actualsShow = true;

    $scope.$watch('actualsShow', function(toggle){
        reveniewElementToggle(actuals, toggle);
    });

    var signups = {
        label : "Actual Signups",
        data : sgnups,
        color : '#71843F',
        lines : {
            show : true,
            lineWidth : 1
        },
        points : {
            show : true
        }
    };
    $scope.signupsShow = true;

    $scope.$watch('signupsShow', function(toggle){
        reveniewElementToggle(signups, toggle);
    });

    $scope.revenewData = [targets, actuals, signups];

    function reveniewElementToggle(element, toggle){
        if(toggle){
            if($scope.revenewData.indexOf(element) == -1)
                $scope.revenewData.push(element)
        } else {
            $scope.revenewData = _.without($scope.revenewData, element);
        }
    }

    $scope.revenewDisplayOptions = {
        grid : {
            hoverable : true
        },
        tooltip : true,
        tooltipOpts : {
            //content: '%x - %y',
            //dateFormat: '%b %y',
            defaultTheme : false
        },
        xaxis : {
            mode : "time"
        },
        yaxes : {
            tickFormatter : function(val, axis) {
                return "$" + val;
            },
            max : 1200
        }

    };

    // bird eye widget data
    $scope.countriesVisitsData = {
        "US": 4977,
        "AU": 4873,
        "IN": 3671,
        "BR": 2476,
        "TR": 1476,
        "CN": 146,
        "CA": 134,
        "BD": 100
    };

    $scope.events = [];

    // Queriing our events from CalendarEvent resource...
    // Scope update will automatically update the calendar
    CalendarEvent.query().$promise.then(function (events) {
        $scope.events = events;
    });


});
'use strict'

angular.module('app.forms').value('formsCommon', {
        countries: [
            {key: "244", value: "Aaland Islands"},
            {key: "1", value: "Afghanistan"},
            {key: "2", value: "Albania"},
            {key: "3", value: "Algeria"},
            {key: "4", value: "American Samoa"},
            {key: "5", value: "Andorra"},
            {key: "6", value: "Angola"},
            {key: "7", value: "Anguilla"},
            {key: "8", value: "Antarctica"},
            {key: "9", value: "Antigua and Barbuda"},
            {key: "10", value: "Argentina"},
            {key: "11", value: "Armenia"},
            {key: "12", value: "Aruba"},
            {key: "13", value: "Australia"},
            {key: "14", value: "Austria"},
            {key: "15", value: "Azerbaijan"},
            {key: "16", value: "Bahamas"},
            {key: "17", value: "Bahrain"},
            {key: "18", value: "Bangladesh"},
            {key: "19", value: "Barbados"},
            {key: "20", value: "Belarus"},
            {key: "21", value: "Belgium"},
            {key: "22", value: "Belize"},
            {key: "23", value: "Benin"},
            {key: "24", value: "Bermuda"},
            {key: "25", value: "Bhutan"},
            {key: "26", value: "Bolivia"},
            {key: "245", value: "Bonaire, Sint Eustatius and Saba"},
            {key: "27", value: "Bosnia and Herzegovina"},
            {key: "28", value: "Botswana"},
            {key: "29", value: "Bouvet Island"},
            {key: "30", value: "Brazil"},
            {key: "31", value: "British Indian Ocean Territory"},
            {key: "32", value: "Brunei Darussalam"},
            {key: "33", value: "Bulgaria"},
            {key: "34", value: "Burkina Faso"},
            {key: "35", value: "Burundi"},
            {key: "36", value: "Cambodia"},
            {key: "37", value: "Cameroon"},
            {key: "38", value: "Canada"},
            {key: "251", value: "Canary Islands"},
            {key: "39", value: "Cape Verde"},
            {key: "40", value: "Cayman Islands"},
            {key: "41", value: "Central African Republic"},
            {key: "42", value: "Chad"},
            {key: "43", value: "Chile"},
            {key: "44", value: "China"},
            {key: "45", value: "Christmas Island"},
            {key: "46", value: "Cocos (Keeling) Islands"},
            {key: "47", value: "Colombia"},
            {key: "48", value: "Comoros"},
            {key: "49", value: "Congo"},
            {key: "50", value: "Cook Islands"},
            {key: "51", value: "Costa Rica"},
            {key: "52", value: "Cote D'Ivoire"},
            {key: "53", value: "Croatia"},
            {key: "54", value: "Cuba"},
            {key: "246", value: "Curacao"},
            {key: "55", value: "Cyprus"},
            {key: "56", value: "Czech Republic"},
            {key: "237", value: "Democratic Republic of Congo"},
            {key: "57", value: "Denmark"},
            {key: "58", value: "Djibouti"},
            {key: "59", value: "Dominica"},
            {key: "60", value: "Dominican Republic"},
            {key: "61", value: "East Timor"},
            {key: "62", value: "Ecuador"},
            {key: "63", value: "Egypt"},
            {key: "64", value: "El Salvador"},
            {key: "65", value: "Equatorial Guinea"},
            {key: "66", value: "Eritrea"},
            {key: "67", value: "Estonia"},
            {key: "68", value: "Ethiopia"},
            {key: "69", value: "Falkland Islands (Malvinas)"},
            {key: "70", value: "Faroe Islands"},
            {key: "71", value: "Fiji"},
            {key: "72", value: "Finland"},
            {key: "74", value: "France, skypolitan"},
            {key: "75", value: "French Guiana"},
            {key: "76", value: "French Polynesia"},
            {key: "77", value: "French Southern Territories"},
            {key: "126", value: "FYROM"},
            {key: "78", value: "Gabon"},
            {key: "79", value: "Gambia"},
            {key: "80", value: "Georgia"},
            {key: "81", value: "Germany"},
            {key: "82", value: "Ghana"},
            {key: "83", value: "Gibraltar"},
            {key: "84", value: "Greece"},
            {key: "85", value: "Greenland"},
            {key: "86", value: "Grenada"},
            {key: "87", value: "Guadeloupe"},
            {key: "88", value: "Guam"},
            {key: "89", value: "Guatemala"},
            {key: "241", value: "Guernsey"},
            {key: "90", value: "Guinea"},
            {key: "91", value: "Guinea-Bissau"},
            {key: "92", value: "Guyana"},
            {key: "93", value: "Haiti"},
            {key: "94", value: "Heard and Mc Donald Islands"},
            {key: "95", value: "Honduras"},
            {key: "96", value: "Hong Kong"},
            {key: "97", value: "Hungary"},
            {key: "98", value: "Iceland"},
            {key: "99", value: "India"},
            {key: "100", value: "Indonesia"},
            {key: "101", value: "Iran (Islamic Republic of)"},
            {key: "102", value: "Iraq"},
            {key: "103", value: "Ireland"},
            {key: "104", value: "Israel"},
            {key: "105", value: "Italy"},
            {key: "106", value: "Jamaica"},
            {key: "107", value: "Japan"},
            {key: "240", value: "Jersey"},
            {key: "108", value: "Jordan"},
            {key: "109", value: "Kazakhstan"},
            {key: "110", value: "Kenya"},
            {key: "111", value: "Kiribati"},
            {key: "113", value: "Korea, Republic of"},
            {key: "114", value: "Kuwait"},
            {key: "115", value: "Kyrgyzstan"},
            {key: "116", value: "Lao People's Democratic Republic"},
            {key: "117", value: "Latvia"},
            {key: "118", value: "Lebanon"},
            {key: "119", value: "Lesotho"},
            {key: "120", value: "Liberia"},
            {key: "121", value: "Libyan Arab Jamahiriya"},
            {key: "122", value: "Liechtenstein"},
            {key: "123", value: "Lithuania"},
            {key: "124", value: "Luxembourg"},
            {key: "125", value: "Macau"},
            {key: "127", value: "Madagascar"},
            {key: "128", value: "Malawi"},
            {key: "129", value: "Malaysia"},
            {key: "130", value: "Maldives"},
            {key: "131", value: "Mali"},
            {key: "132", value: "Malta"},
            {key: "133", value: "Marshall Islands"},
            {key: "134", value: "Martinique"},
            {key: "135", value: "Mauritania"},
            {key: "136", value: "Mauritius"},
            {key: "137", value: "Mayotte"},
            {key: "138", value: "Mexico"},
            {key: "139", value: "Micronesia, Federated States of"},
            {key: "140", value: "Moldova, Republic of"},
            {key: "141", value: "Monaco"},
            {key: "142", value: "Mongolia"},
            {key: "242", value: "Montenegro"},
            {key: "143", value: "Montserrat"},
            {key: "144", value: "Morocco"},
            {key: "145", value: "Mozambique"},
            {key: "146", value: "Myanmar"},
            {key: "147", value: "Namibia"},
            {key: "148", value: "Nauru"},
            {key: "149", value: "Nepal"},
            {key: "150", value: "Netherlands"},
            {key: "151", value: "Netherlands Antilles"},
            {key: "152", value: "New Caledonia"},
            {key: "153", value: "New Zealand"},
            {key: "154", value: "Nicaragua"},
            {key: "155", value: "Niger"},
            {key: "156", value: "Nigeria"},
            {key: "157", value: "Niue"},
            {key: "158", value: "Norfolk Island"},
            {key: "112", value: "North Korea"},
            {key: "159", value: "Northern Mariana Islands"},
            {key: "160", value: "Norway"},
            {key: "161", value: "Oman"},
            {key: "162", value: "Pakistan"},
            {key: "163", value: "Palau"},
            {key: "247", value: "Palestinian Territory, Occupied"},
            {key: "164", value: "Panama"},
            {key: "165", value: "Papua New Guinea"},
            {key: "166", value: "Paraguay"},
            {key: "167", value: "Peru"},
            {key: "168", value: "Philippines"},
            {key: "169", value: "Pitcairn"},
            {key: "170", value: "Poland"},
            {key: "171", value: "Portugal"},
            {key: "172", value: "Puerto Rico"},
            {key: "173", value: "Qatar"},
            {key: "174", value: "Reunion"},
            {key: "175", value: "Romania"},
            {key: "176", value: "Russian Federation"},
            {key: "177", value: "Rwanda"},
            {key: "178", value: "Saint Kitts and Nevis"},
            {key: "179", value: "Saint Lucia"},
            {key: "180", value: "Saint Vincent and the Grenadines"},
            {key: "181", value: "Samoa"},
            {key: "182", value: "San Marino"},
            {key: "183", value: "Sao Tome and Principe"},
            {key: "184", value: "Saudi Arabia"},
            {key: "185", value: "Senegal"},
            {key: "243", value: "Serbia"},
            {key: "186", value: "Seychelles"},
            {key: "187", value: "Sierra Leone"},
            {key: "188", value: "Singapore"},
            {key: "189", value: "Slovak Republic"},
            {key: "190", value: "Slovenia"},
            {key: "191", value: "Solomon Islands"},
            {key: "192", value: "Somalia"},
            {key: "193", value: "South Africa"},
            {key: "194", value: "South Georgia &amp; South Sandwich Islands"},
            {key: "248", value: "South Sudan"},
            {key: "195", value: "Spain"},
            {key: "196", value: "Sri Lanka"},
            {key: "249", value: "St. Barthelemy"},
            {key: "197", value: "St. Helena"},
            {key: "250", value: "St. Martin (French part)"},
            {key: "198", value: "St. Pierre and Miquelon"},
            {key: "199", value: "Sudan"},
            {key: "200", value: "Suriname"},
            {key: "201", value: "Svalbard and Jan Mayen Islands"},
            {key: "202", value: "Swaziland"},
            {key: "203", value: "Sweden"},
            {key: "204", value: "Switzerland"},
            {key: "205", value: "Syrian Arab Republic"},
            {key: "206", value: "Taiwan"},
            {key: "207", value: "Tajikistan"},
            {key: "208", value: "Tanzania, United Republic of"},
            {key: "209", value: "Thailand"},
            {key: "210", value: "Togo"},
            {key: "211", value: "Tokelau"},
            {key: "212", value: "Tonga"},
            {key: "213", value: "Trinidad and Tobago"},
            {key: "214", value: "Tunisia"},
            {key: "215", value: "Turkey"},
            {key: "216", value: "Turkmenistan"},
            {key: "217", value: "Turks and Caicos Islands"},
            {key: "218", value: "Tuvalu"},
            {key: "219", value: "Uganda"},
            {key: "220", value: "Ukraine"},
            {key: "221", value: "United Arab Emirates"},
            {key: "222", value: "United Kingdom"},
            {key: "223", value: "United States"},
            {key: "224", value: "United States Minor Outlying Islands"},
            {key: "225", value: "Uruguay"},
            {key: "226", value: "Uzbekistan"},
            {key: "227", value: "Vanuatu"},
            {key: "228", value: "Vatican City State (Holy See)"},
            {key: "229", value: "Venezuela"},
            {key: "230", value: "Viet Nam"},
            {key: "231", value: "Virgin Islands (British)"},
            {key: "232", value: "Virgin Islands (U.S.)"},
            {key: "233", value: "Wallis and Futuna Islands"},
            {key: "234", value: "Western Sahara"},
            {key: "235", value: "Yemen"},
            {key: "238", value: "Zambia"},
            {key: "239", value: "Zimbabwe"}
        ],
        validateOptions: {
            errorElement: 'em',
            errorClass: 'invalid',
            highlight: function(element, errorClass, validClass) {
                $(element).addClass(errorClass).removeClass(validClass);
                $(element).parent().addClass('state-error').removeClass('state-success');

            },
            unhighlight: function(element, errorClass, validClass) {
                $(element).removeClass(errorClass).addClass(validClass);
                $(element).parent().removeClass('state-error').addClass('state-success');
            },
            errorPlacement : function(error, element) {
                error.insertAfter(element.parent());
            }
        }
    });
'use strict';

angular.module('app.appViews').controller('ProjectsDemoCtrl', function ($scope, projects) {

    $scope.projects = projects.data;

    $scope.tableOptions =  {
        "data": projects.data.data,
//            "bDestroy": true,
        "iDisplayLength": 15,
        "columns": [
            {
                "class":          'details-control',
                "orderable":      false,
                "data":           null,
                "defaultContent": ''
            },
            { "data": "name" },
            { "data": "est" },
            { "data": "contacts" },
            { "data": "status" },
            { "data": "target-actual" },
            { "data": "starts" },
            { "data": "ends" },
            { "data": "tracker" }
        ],
        "order": [[1, 'asc']]
    }
});
'use strict';

angular.module('app.attachments').controller('attachmentsCtrl', function ($scope, $rootScope, fileManager, APP_CONFIG, $stateParams) {

    /* jshint validthis:true */
    var vm = this;
    vm.title = 'File Manager';
    vm.files = fileManager.files;
    vm.uploading = false;
    vm.previewFile;
    vm.currentFile;
    vm.remove = fileManager.remove;
    vm.download = fileManager.download;
    vm.setPreviewFile = setPreviewFile;
    vm.setCurrentFile = setCurrentFile;
    vm.getWord = getWord;
    vm.readonly = false;

    $scope.showUpload = false;

    fileManager.params.schema = this.dbschema;
    if (!fileManager.params.schema)
    {
        fileManager.params.schema = $stateParams.schema;
    }
   
    fileManager.params.cls = this.dbclass;
    if (!fileManager.params.cls) {
        fileManager.params.cls = $stateParams.class;
    }

    fileManager.params.oid = this.oid;

    if (!fileManager.params.oid && !$stateParams.rclass) {
        fileManager.params.oid = $stateParams.oid;
    }

    if (fileManager.params.oid) {
        if ($stateParams.readonly && $stateParams.readonly === "true") {
            vm.readonly = true;
        }
        else if (this.read && this.read === true)
        {
            vm.readonly = true;
        }
        else {
            vm.readonly = false;
        }
    }
    else {
        vm.readonly = true;
    }

    fileManager.params.prefix = getTaskAttachmentPrefix(this.dbschema, this.dbclass, this.oid);

    fileManager.params.api = "api/blob";

    fileManager.params.serviceBase = APP_CONFIG.ebaasRootUrl;

    activate();

    function activate() {
        fileManager.load();
    }

    function setPreviewFile(file) {
        console.debug("setPreviewFile");
        vm.previewFile = file
    }

    function setCurrentFile(file) {
        console.debug("setCurrentFile");
        vm.currentFile = file
    }

    function remove(file) {
        fileManager.remove(file).then(function () {
            setPreviewFile();
        });
    }

    function getWord(key)
    {
        return $rootScope.getWord(key);
    }

    function getTaskAttachmentPrefix(schemaName, className, objId) {

        return "Attachments\\" + schemaName + " 1.0\\" + className + "\\" + objId;
    }

    $scope.uploadFile = function () {
        $scope.processDropzone();
    }

    $scope.reset = function () {
        $scope.resetDropzone();
    }

    $scope.$on('instanceCreated', function (event, args) {
        fileManager.params.oid = args.oid;
        vm.readonly = false;
    });

    $scope.setShowUpload = function(status)
    {
        $scope.showUpload = status;
    }

    $scope.$on('relatedModalFormClosed', function (event, args) {
        if (fileManager.params.oid != args.masterOid) {
            fileManager.params.oid = args.masterOid;
            fileManager.load();
        }
    });
});

'use strict';

angular.module('app.attachments').controller('attachmentsModalCtrl', function ($scope, $stateParams, $modalInstance) {
 
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.class;
    $scope.oid = $stateParams.oid;
    $scope.readonly = $stateParams.readonly;
    
    $scope.closeModal = function () {
        $modalInstance.dismiss("dismiss");
    };
});

'use strict';

angular.module('app.attachments').directive('attachments', function () {
    return {
        restrict: 'E',
        templateUrl: 'app/attachments/views/attachments.html',
        replace: true,
        scope: {},
        bindToController: {
            dbschema: '=',
            dbclass: '=',
            oid: '=',
            read: '='
        },
        controllerAs: 'vm',
        controller: 'attachmentsCtrl',
        link: function (scope, element, attributes) {
        }
    }
});
'use strict';

angular.module('app.attachments').directive('dropzone', function ($rootScope, APP_CONFIG, fileManager, User) {
    return {
        restrict: 'C',
        link: function (scope, element, attributes) {
            
            var config = {
                url: APP_CONFIG.ebaasRootUrl + "/" + fileManager.params.api + "/" + encodeURIComponent(fileManager.params.schema) + "/" + fileManager.params.cls + "/" + fileManager.params.oid + "?prefix=" + encodeURIComponent(fileManager.params.prefix) + "&user=" + encodeURIComponent(User.userName),
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

            };

            var eventHandlers = {
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

                'removedFile': function(file)
                {
                    console.debug("Removed file called");
                },

                'queuecomplete': function () {
                    fileManager.load();

                    setTimeout(function () {
                        //scope.resetDropzone();
                    }, 2000);
                }
            };

            var dropzone = new Dropzone(element[0], config);

            angular.forEach(eventHandlers, function (handler, event) {
                dropzone.on(event, handler);
            });

            scope.processDropzone = function () {

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

'use strict';

angular.module('app.attachments').directive('egAppStatus', function (loadingInfo) {
    var directive = {
        link: link,
        restrict: 'E',
        templateUrl: 'app/attachments/views/egAppStatus.html'
    };
    return directive;

    function link(scope, element, attrs) {
        scope.status = loadingInfo.status;
    }
});
'use strict';

angular.module('app.attachments').directive('egFiles', function () {

    var directive = {
        link: link,
        restrict: 'A',
        scope: {
            files: '=egFiles',
            hasFiles: '='
        }
    };
    return directive;

    function link(scope, element, attrs) {
        element.bind('change', function () {
            scope.$apply(function () {
                if (element[0].files) {
                    scope.files.length = 0;

                    angular.forEach(element[0].files, function (f) {
                        scope.files.push(f);
                    });

                    scope.hasFiles = true;
                }
            });
        });

        if (element[0].form) {
            angular.element(element[0].form)
                    .bind('reset', function () {
                        scope.$apply(function () {
                            scope.files.length = 0;
                            scope.hasFiles = false;
                        });
                    });
        }
    }
});
'use strict';

angular.module('app.attachments').directive('blobUpload', function ($timeout) {
    var directive = {
        link: link,
        restrict: 'A',
        scope: {
            upload: '&blobUpload'
        }
    };
    return directive;

    function link(scope, element, attrs) {
        var parentForm = element[0].form;
        if (parentForm) {
            element.on('click', function (event) {
                return scope.upload().then(function () {
                    //see:https://docs.angularjs.org/error/$rootScope/inprog?p0=$digest for why there is a need to use timeout to avoid conflict
                    $timeout(function () {
                        parentForm.reset();
                    });
                });
            });
        }
    }
});
'use strict';

angular.module('app.attachments').directive('fileDropzone', function ($rootScope) {
    return {
        restrict: 'A',
        compile: function (tElement, tAttributes) {
            tElement.removeAttr('smart-dropzone data-smart-dropzone');

            tElement.dropzone({
                addRemoveLinks : true,
                maxFilesize: 0.5,
                dictDefaultMessage: '<span class="text-center"><span class="font-lg visible-xs-block visible-sm-block visible-lg-block"><span class="font-lg"><i class="fa fa-caret-right text-danger"></i><span class="font-xs">' + $rootScope.getWord("DropZone") + '</span></span>',
                dictResponseError: $rootScope.getWord("UploadError")
            });
        }
    }
});

'use strict';

angular.module('app.attachments').directive('egFileUploader', function (loadingInfo, fileManager) {

    var directive = {
        link: link,
        restrict: 'E',
        templateUrl: 'app/attachments/views/fileUploader.html',
        scope: true
    };
    return directive;

    function link(scope, element, attrs) {
        scope.hasFiles = false;
        scope.files = [];
        scope.upload = fileManager.upload;
        scope.appStatus = loadingInfo.status;
        scope.fileManagerStatus = fileManager.status;
    }

});
'use strict';

angular.module('app.attachments').factory('fileManager', function ($q, fileManagerClient, $http, loadingInfo, User) {

    var service = {
        files: [],
        load: load,
        upload: upload,
        remove: remove,
        download: download,
        performDownload: performDownload,
        fileExists: fileExists,
        status: {
            uploading: false
        },
        params: {
            serviceBase: "",
            schema: "",
            cls: "",
            oid: "",
            api: "",
            prefix: ""
        }
    };

    return service;

    function load() {

        loadingInfo.setInfo({ busy: true, message: "loading files" })

        service.files.length = 0;

        if (!service.params.oid)
            return [];
        else
            return fileManagerClient.query({api: service.params.api, schema: service.params.schema, cls: service.params.cls, oid: service.params.oid, prefix: service.params.prefix })
                                .$promise
                                .then(function (result) {
                                    result.files
                                            .forEach(function (file) {
                                                service.files.push(file);
                                            });

                                    loadingInfo.setInfo({ message: "files loaded successfully" });

                                    return result.$promise;
                                },
                                function (result) {
                                    if (result.data) {
                                        loadingInfo.setInfo({ message: "something went wrong: " + result.data.message });
                                    }
                                    else
                                    {
                                        loadingInfo.setInfo({ message: "something went wrong: "});
                                    }
                                    return $q.reject(result);
                                })
                                ['finally'](
                                function () {
                                    loadingInfo.setInfo({ busy: false });
                                });
    }

    function upload(files) {
        
        service.status.uploading = true;
        loadingInfo.setInfo({ busy: true, message: "uploading files" });

        var formData = new FormData();

        angular.forEach(files, function (file) {
            console.debug("upload file name =" + file.name);
            formData.append(file.name, file);
        });

        return fileManagerClient.save({ api: service.params.api, schema: service.params.schema, cls: service.params.cls, oid: service.params.oid, prefix: service.params.prefix }, formData)
                                    .$promise
                                    .then(function (result) {
                                        if (result && result.files) {
                                            result.files.forEach(function (file) {
                                                if (!fileExists(file.name)) {
                                                    service.files.push(file);
                                                }
                                            });
                                        }

                                        loadingInfo.setInfo({ message: "files uploaded successfully" });

                                        return result.$promise;
                                    },
                                    function (result) {
                                        loadingInfo.setInfo({ message: "something went wrong: " + result.data.message });
                                        return $q.reject(result);
                                    })
                                    ['finally'](
                                    function () {
                                        loadingInfo.setInfo({ busy: false });
                                        service.status.uploading = false;
                                    });
    }

    function remove(file) {
        loadingInfo.setInfo({ busy: true, message: "deleting file " + file.name });
      
        return fileManagerClient.remove({ api: service.params.api, schema: service.params.schema, cls: service.params.cls, oid: service.params.oid, fileId: file.id, prefix: service.params.prefix })
                                    .$promise
                                    .then(function (result) {
                                        //if the file was deleted successfully remove it from the files array
                                        var i = service.files.indexOf(file);
                                        service.files.splice(i, 1);

                                        loadingInfo.setInfo({ message: "files deleted" });

                                        return result.$promise;
                                    },
                                    function (result) {
                                        loadingInfo.setInfo({ message: "something went wrong: " + result.data.message });
                                        return $q.reject(result);
                                    })
                                    ['finally'](
                                    function () {
                                        loadingInfo.setInfo({ busy: false });
                                    });
    }

    function download(file)
    {
        var getFileUrl = service.params.serviceBase + "/" + service.params.api + "/" + service.params.schema + "/" + service.params.cls + "/" + service.params.oid + "/" + file.id;
        if (service.params.prefix)
        {
            getFileUrl += "?prefix=" + encodeURIComponent(service.params.prefix) + "&user=" + encodeURIComponent(User.userName);
        }

        performDownload(getFileUrl, null);
    }

    function performDownload(url, callback) {
    
        // Use an arraybuffer
        $http.get(url, { responseType: 'arraybuffer' })
            .success(function (data, status, headers) {

                var octetStreamMime = 'application/octet-stream';
                var success = false;

                // Get the headers
                headers = headers();

                // Get the filename from the x-filename header or default to "download.bin"
                var filename = headers['x-filename'] || 'download.bin';
                filename = decodeURIComponent(filename);

                // Determine the content type from the header or default to "application/octet-stream"
                var contentType = headers['content-type'] || octetStreamMime;

                try {
                    // Try using msSaveBlob if supported
                    console.log("Trying saveBlob method ...");
                    var blob = new Blob([data], { type: contentType });
                    if (navigator.msSaveBlob)
                        navigator.msSaveBlob(blob, filename);
                    else {
                        // Try using other saveBlob implementations, if available
                        var saveBlob = navigator.webkitSaveBlob || navigator.mozSaveBlob || navigator.saveBlob;
                        if (saveBlob === undefined) throw "Not supported";
                        saveBlob(blob, filename);
                    }
                    console.log("saveBlob succeeded");
                    success = true;
                } catch (ex) {
                    console.log("saveBlob method failed with the following exception:");
                    console.log(ex);
                }

                if (!success) {
                    // Get the blob url creator
                    var urlCreator = window.URL || window.webkitURL || window.mozURL || window.msURL;
                    if (urlCreator) {
                        // Try to use a download link
                        var link = document.createElement('a');
                        if ('download' in link) {
                            // Try to simulate a click
                            try {
                                // Prepare a blob URL
                                console.log("Trying download link method with simulated click ...");
                                var blob = new Blob([data], { type: contentType });
                                var url = urlCreator.createObjectURL(blob);
                                link.setAttribute('href', url);

                                // Set the download attribute (Supported in Chrome 14+ / Firefox 20+)
                                link.setAttribute("download", filename);

                                // Simulate clicking the download link
                                var event = document.createEvent('MouseEvents');
                                event.initMouseEvent('click', true, true, window, 1, 0, 0, 0, 0, false, false, false, false, 0, null);
                                link.dispatchEvent(event);
                                console.log("Download link method with simulated click succeeded");
                                success = true;

                            } catch (ex) {
                                console.log("Download link method with simulated click failed with the following exception:");
                                console.log(ex);
                            }
                        }

                        if (!success) {
                            // Fallback to window.location method
                            try {
                                // Prepare a blob URL
                                // Use application/octet-stream when using window.location to force download
                                console.log("Trying download link method with window.location ...");
                                var blob = new Blob([data], { type: octetStreamMime });
                                var url = urlCreator.createObjectURL(blob);
                                window.location = url;
                                console.log("Download link method with window.location succeeded");
                                success = true;
                            } catch (ex) {
                                console.log("Download link method with window.location failed with the following exception:");
                                console.log(ex);
                            }
                        }

                    }
                }

                if (!success) {
                    // Fallback to window.open method
                    console.log("No methods worked for saving the arraybuffer, using last resort window.open");
                    window.open(httpPath, '_blank', '');
                }

                if (callback)
                {
                    callback();
                }
            })
        .error(function (data, status) {
            console.log("Request failed with status: " + status);

            // Optionally write the error out to scope
            //$scope.errorDetails = "Request failed with status: " + status;

            if (callback) {
                callback();
            }
        });
    }

    function fileExists(fileName) {
        var res = false
        service.files.forEach(function (file) {
            if (file.name === fileName) {
                res = true;
            }
        });

        return res;
    }
});
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

'use strict';

angular.module('app.attachments').factory('loadingInfo', function () {
    var service = {
        status: {
            busy: false,
            message: ''
        },
        setInfo: setInfo
    };

    return service;

    function setInfo(args) {
        if (args) {
            if (args.hasOwnProperty('busy')) {
                service.status.busy = args.busy;
            }
            if (args.hasOwnProperty('message')) {
                service.status.message = args.message;
            }
        } else {
            service.status.busy = false;
            service.status.message = '';
        }
    }
});
"use strict";

angular.module("app.auth").factory("authService", function($rootScope, $http, $q, localStorageService, APP_CONFIG) {

    var authServiceFactory = {};

    var _authentication = {
        isAuth: false,
        userName: ""
    };

    var _saveRegistration = function(registration) {

        _logOut();

        return $http.post(APP_CONFIG.ebaasRootUrl + '/api/accounts/create', registration).then(function(response) {
            return response;
        });
    };

    var _login = function(loginData) {

        var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.password;

        var deferred = $q.defer();

        var url = APP_CONFIG.ebaasRootUrl + '/oauth/token';

        $http.post(url, data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function(response) {

            localStorageService.set('authorizationData', { token: response.access_token, userName: loginData.userName });

            _authentication.isAuth = true;
            _authentication.userName = loginData.userName;

            deferred.resolve(response);

        }).error(function (err, status) {
            if (err.error === "invalid_grant")
            {
                err.error_description = $rootScope.getWord("Invalid user name or password");
            }
            _logOut();
            deferred.reject(err);
        });

        return deferred.promise;

    };

    var _logOut = function() {

        localStorageService.remove('authorizationData');

        _authentication.isAuth = false;
        _authentication.userName = "";

    };

    var _fillAuthData = function() {

        var authData = localStorageService.get('authorizationData');
        if (authData) {
            _authentication.isAuth = true;
            _authentication.userName = authData.userName;
        }

    }

    authServiceFactory.saveRegistration = _saveRegistration;
    authServiceFactory.login = _login;
    authServiceFactory.logOut = _logOut;
    authServiceFactory.fillAuthData = _fillAuthData;
    authServiceFactory.authentication = _authentication;

    return authServiceFactory;
});
(function (app) {
    var LoginController = function ($rootScope, $scope, $http, $state, $location, authService, APP_CONFIG, User, myActivityService, hubService) {

        $scope.loginData = {
            userName: "",
            password: ""
        };

        $scope.message = "";

        $scope.login = function () {
            authService.login($scope.loginData).then(function (response) {

                if ($rootScope.returnToState) {
                    $location.path($rootScope.returnToState);
                } else {
                    $location.path('/');
                }

                // load user's info
                User.load(function () {
                    hubService.connect(APP_CONFIG.dbschema, function (type, message) {
                        myActivityService.add(type, message);
                    }); // connect to server hub to receive messages
                }); // load user info

                //get user's message count to display at header
                $http.get(APP_CONFIG.ebaasRootUrl + "/api/messages/count")
                    .success(function (data) {
                        myActivityService.MessageModel.count = data;
                    });
            },
            function (err) {
                $scope.message = err.error_description;
            });
        };

        if (!String.format) {
            String.format = function (format) {
                var args = Array.prototype.slice.call(arguments, 1);
                return format.replace(/{(\d+)}/g, function (match, number) {
                    return typeof args[number] != 'undefined'
                      ? args[number]
                      : match
                    ;
                });
            };
        }
  
    }

    app.controller("LoginController", LoginController);

}(angular.module("app.auth")));
"use strict";

angular.module('app.auth').controller('LogoutController', function ($scope, authService, hubService) {
    hubService.disconnect();
    authService.logOut();
})
"use strict";

angular.module('app.auth').controller('RegisterController', function ($scope, $location, $timeout, authService) {
        $scope.savedSuccessfully = false;
        $scope.message = "";

        $scope.registration = {
            userName: "",
            lastName: "",
            firstName: "",
            email: "",
            password: "",
            confirmPassword: ""
        };

        $scope.signUp = function () {
            authService.saveRegistration($scope.registration).then(function (response) {

                $scope.savedSuccessfully = true;
                $scope.message = "User has been registered successfully, you will be redicted to login page in 2 seconds.";
                startTimer();

            },
             function (response) {
                 var errors = [];
                 for (var key in response.data.modelState) {
                     for (var i = 0; i < response.data.modelState[key].length; i++) {
                         errors.push(response.data.modelState[key][i]);
                     }
                 }
                 $scope.message = "Failed to register user due to:" + errors.join(' ');
             });
        };

        var startTimer = function () {
            var timer = $timeout(function () {
                $timeout.cancel(timer);
                $location.path('/login');
            }, 2000);
        }

    });
"use strict";

angular.module('app.auth').directive('loginInfo', function(User){

    return {
        restrict: 'A',
        templateUrl: 'app/auth/directives/login-info.tpl.html',
        link: function(scope, element){
            User.initialized.then(function () {
                scope.user = User
            });
        }
    }
})



'use strict';

angular.module('app.auth').factory('User', function ($http, $q, APP_CONFIG, authService) {
    var dfd = $q.defer();

    function imageExists(image_url) {

        var http = new XMLHttpRequest();

        http.open('HEAD', image_url, false);
        http.send();

        return http.status != 404;
    }

    var UserModel = {
        initialized: dfd.promise,
        userName: undefined,
        picture: undefined,
        email: undefined,
        phoneNumber : undefined,
        password: undefined,
        confirmPassword: undefined,
        firstName: undefined,
        lastName: undefined,
        displayName : undefined,
        division: undefined,
        address: undefined,
        imageUrl: undefined,
        pictureChangeTime: undefined,
        userImageUrls: undefined,
        load: function(callback)
        {
            $http.get(APP_CONFIG.ebaasRootUrl + '/api/accounts/user/' + authService.authentication.userName).then(function (response) {
                UserModel.userName = response.data.userName;
                UserModel.email = response.data.email;
                UserModel.password = response.data.password;
                UserModel.firstName = response.data.firstName;
                UserModel.lastName = response.data.lastName;
                UserModel.displayName = response.data.displayName;
                UserModel.phoneNumber = response.data.phoneNumber;
                UserModel.division = response.data.division;
                UserModel.address = response.data.address;
                UserModel.imageUrl = undefined;
                UserModel.userImageUrls = {};

                UserModel.picture = UserModel.userName + ".png";

                dfd.resolve(UserModel);

                if (callback) {
                    callback();
                }
            });
        },
        save : function (callback) {
            var model = {};
            model.userName = UserModel.userName;
            model.email = UserModel.email;
            model.phoneNumber = UserModel.phoneNumber;
            model.firstName = UserModel.firstName;
            model.lastName = UserModel.lastName;
            model.picture = UserModel.picture;
           
            $http.post(APP_CONFIG.ebaasRootUrl + '/api/accounts/update', model).success(function (data) {
                if (callback) {
                    callback();
                }
            });
        },
        image : function()
        {
            if (!UserModel.imageUrl) {
                var imageUrl = APP_CONFIG.avatarsUrl + UserModel.picture;
                if (!imageExists(imageUrl)) {
                    UserModel.imageUrl = APP_CONFIG.avatarsUrl + "male.png";
                }
                else {
                    UserModel.imageUrl = imageUrl + '?' + UserModel.pictureChangeTime;
                }
            }
            //console.debug(UserModel.imageUrl);
            return UserModel.imageUrl;
        },
        getUserImage: function (userId) {
            if (UserModel.userImageUrls) {
                var imageUrl = UserModel.userImageUrls[userId];
                if (!imageUrl) {
                    imageUrl = APP_CONFIG.avatarsUrl + userId + ".png";
                    if (!imageExists(imageUrl)) {
                        imageUrl = APP_CONFIG.avatarsUrl + "male.png";
                        UserModel.userImageUrls[userId] = imageUrl;
                    }
                    else {
                        UserModel.userImageUrls[userId] = imageUrl;
                    }
                }
            }
            else
            {
                return APP_CONFIG.avatarsUrl + "male.png";
            }
     
            return imageUrl;
        }
    };

    return UserModel;
});

"use strict";

angular.module('app.auth').controller('LoginCtrl', function ($scope, $state, GooglePlus, User, ezfb) {

    $scope.$on('event:google-plus-signin-success', function (event, authResult) {
        if (authResult.status.method == 'PROMPT') {
            GooglePlus.getUser().then(function (user) {
                User.userName = user.name;
                User.picture = user.picture;
                $state.go('app.dashboard');
            });
        }
    });

    $scope.$on('event:facebook-signin-success', function (event, authResult) {
        ezfb.api('/me', function (res) {
            User.userName = res.name;
            User.picture = 'https://graph.facebook.com/' + res.id + '/picture';
            $state.go('app.dashboard');
        });
    });
})

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
'use strict';

angular.module('app.blobmanager').factory('blobManager', function ($q, fileManagerClient, $http, loadingInfo, User) {

    var service = {
        files: [],
        load: load,
        upload: upload,
        remove: remove,
        download: download,
        performDownload: performDownload,
        fileExists: fileExists,
        status: {
            uploading: false
        },
        params: {
            serviceBase: "",
            schema: "",
            cls: "",
            oid: "",
            api: "",
            prefix: ""
        }
    };

    return service;

    function load() {

        loadingInfo.setInfo({ busy: true, message: "loading files" })

        service.files.length = 0;

        if (!service.params.oid)
            return [];
        else
            return fileManagerClient.query({api: service.params.api, schema: service.params.schema, cls: service.params.cls, oid: service.params.oid, prefix: service.params.prefix })
                                .$promise
                                .then(function (result) {
                                    result.files
                                            .forEach(function (file) {
                                                service.files.push(file);
                                            });

                                    loadingInfo.setInfo({ message: "files loaded successfully" });

                                    return result.$promise;
                                },
                                function (result) {
                                    if (result.data) {
                                        loadingInfo.setInfo({ message: "something went wrong: " + result.data.message });
                                    }
                                    else
                                    {
                                        loadingInfo.setInfo({ message: "something went wrong: "});
                                    }
                                    return $q.reject(result);
                                })
                                ['finally'](
                                function () {
                                    loadingInfo.setInfo({ busy: false });
                                });
    }

    function upload(files) {
        
        service.status.uploading = true;
        loadingInfo.setInfo({ busy: true, message: "uploading files" });

        var formData = new FormData();

        angular.forEach(files, function (file) {
            console.debug("upload file name =" + file.name);
            formData.append(file.name, file);
        });

        return fileManagerClient.save({ api: service.params.api, schema: service.params.schema, cls: service.params.cls, oid: service.params.oid, prefix: service.params.prefix }, formData)
                                    .$promise
                                    .then(function (result) {
                                        if (result && result.files) {
                                            result.files.forEach(function (file) {
                                                if (!fileExists(file.name)) {
                                                    service.files.push(file);
                                                }
                                            });
                                        }

                                        loadingInfo.setInfo({ message: "files uploaded successfully" });

                                        return result.$promise;
                                    },
                                    function (result) {
                                        loadingInfo.setInfo({ message: "something went wrong: " + result.data.message });
                                        return $q.reject(result);
                                    })
                                    ['finally'](
                                    function () {
                                        loadingInfo.setInfo({ busy: false });
                                        service.status.uploading = false;
                                    });
    }

    function remove(file) {
        loadingInfo.setInfo({ busy: true, message: "deleting file " + file.name });
      
        return fileManagerClient.remove({ api: service.params.api, schema: service.params.schema, cls: service.params.cls, oid: service.params.oid, fileId: file.id, prefix: service.params.prefix })
                                    .$promise
                                    .then(function (result) {
                                        //if the file was deleted successfully remove it from the files array
                                        var i = service.files.indexOf(file);
                                        service.files.splice(i, 1);

                                        loadingInfo.setInfo({ message: "files deleted" });

                                        return result.$promise;
                                    },
                                    function (result) {
                                        loadingInfo.setInfo({ message: "something went wrong: " + result.data.message });
                                        return $q.reject(result);
                                    })
                                    ['finally'](
                                    function () {
                                        loadingInfo.setInfo({ busy: false });
                                    });
    }

    function download(file)
    {
        var getFileUrl = service.params.serviceBase + "/" + service.params.api + "/" + service.params.schema + "/" + service.params.cls + "/" + service.params.oid + "/" + file.id;
        if (service.params.prefix)
        {
            getFileUrl += "?prefix=" + encodeURIComponent(service.params.prefix) + "&user=" + encodeURIComponent(User.userName);
        }

        performDownload(getFileUrl, null);
    }

    function performDownload(url, callback) {
    
        // Use an arraybuffer
        $http.get(url, { responseType: 'arraybuffer' })
            .success(function (data, status, headers) {

                var octetStreamMime = 'application/octet-stream';
                var success = false;

                // Get the headers
                headers = headers();

                // Get the filename from the x-filename header or default to "download.bin"
                var filename = headers['x-filename'] || 'download.bin';
                filename = decodeURIComponent(filename);

                // Determine the content type from the header or default to "application/octet-stream"
                var contentType = headers['content-type'] || octetStreamMime;

                try {
                    // Try using msSaveBlob if supported
                    console.log("Trying saveBlob method ...");
                    var blob = new Blob([data], { type: contentType });
                    if (navigator.msSaveBlob)
                        navigator.msSaveBlob(blob, filename);
                    else {
                        // Try using other saveBlob implementations, if available
                        var saveBlob = navigator.webkitSaveBlob || navigator.mozSaveBlob || navigator.saveBlob;
                        if (saveBlob === undefined) throw "Not supported";
                        saveBlob(blob, filename);
                    }
                    console.log("saveBlob succeeded");
                    success = true;
                } catch (ex) {
                    console.log("saveBlob method failed with the following exception:");
                    console.log(ex);
                }

                if (!success) {
                    // Get the blob url creator
                    var urlCreator = window.URL || window.webkitURL || window.mozURL || window.msURL;
                    if (urlCreator) {
                        // Try to use a download link
                        var link = document.createElement('a');
                        if ('download' in link) {
                            // Try to simulate a click
                            try {
                                // Prepare a blob URL
                                console.log("Trying download link method with simulated click ...");
                                var blob = new Blob([data], { type: contentType });
                                var url = urlCreator.createObjectURL(blob);
                                link.setAttribute('href', url);

                                // Set the download attribute (Supported in Chrome 14+ / Firefox 20+)
                                link.setAttribute("download", filename);

                                // Simulate clicking the download link
                                var event = document.createEvent('MouseEvents');
                                event.initMouseEvent('click', true, true, window, 1, 0, 0, 0, 0, false, false, false, false, 0, null);
                                link.dispatchEvent(event);
                                console.log("Download link method with simulated click succeeded");
                                success = true;

                            } catch (ex) {
                                console.log("Download link method with simulated click failed with the following exception:");
                                console.log(ex);
                            }
                        }

                        if (!success) {
                            // Fallback to window.location method
                            try {
                                // Prepare a blob URL
                                // Use application/octet-stream when using window.location to force download
                                console.log("Trying download link method with window.location ...");
                                var blob = new Blob([data], { type: octetStreamMime });
                                var url = urlCreator.createObjectURL(blob);
                                window.location = url;
                                console.log("Download link method with window.location succeeded");
                                success = true;
                            } catch (ex) {
                                console.log("Download link method with window.location failed with the following exception:");
                                console.log(ex);
                            }
                        }

                    }
                }

                if (!success) {
                    // Fallback to window.open method
                    console.log("No methods worked for saving the arraybuffer, using last resort window.open");
                    window.open(httpPath, '_blank', '');
                }

                if (callback)
                {
                    callback();
                }
            })
        .error(function (data, status) {
            console.log("Request failed with status: " + status);

            // Optionally write the error out to scope
            //$scope.errorDetails = "Request failed with status: " + status;

            if (callback) {
                callback();
            }
        });
    }

    function fileExists(fileName) {
        var res = false
        service.files.forEach(function (file) {
            if (file.name === fileName) {
                res = true;
            }
        });

        return res;
    }
});
"use strict";	

angular.module('app').controller("ActivitiesCtrl", function ActivitiesCtrl($scope, $log, activityService){

	$scope.activeTab = 'default';
	$scope.currentActivityItems = [];
	
	// Getting different type of activites
	activityService.get(function(data){

		$scope.activities = data.activities;
		
	});


	$scope.isActive = function(tab){
		return $scope.activeTab === tab;
	};

	$scope.setTab = function(activityType){
		$scope.activeTab = activityType;

		activityService.getbytype(activityType, function(data) {

			$scope.currentActivityItems = data.data;

		});

	};

});
"use strict";

// Speed up calls to hasOwnProperty
var hasOwnProperty = Object.prototype.hasOwnProperty;

function isEmpty(obj) {

    // null and undefined are "empty"
    if (obj == null) return true;

    // Assume if it has a length property with a non-zero value
    // that that property is correct.
    if (obj.length > 0) return false;
    if (obj.length === 0) return true;

    // Otherwise, does it have any properties of its own?
    // Note that this doesn't handle
    // toString and valueOf enumeration bugs in IE < 9
    for (var key in obj) {
        if (hasOwnProperty.call(obj, key)) return false;
    }

    return true;
}

angular.module('app').directive('activitiesDropdownToggle', function($log) {

	var link = function($scope,$element, attrs){
		var ajax_dropdown = null;

		$element.on('click', function () {
			var badge = $(this).find('.badge');

            /*
			if (badge.hasClass('bg-color-red')) {

				badge.removeClass('bg-color-red').text(0);

			}
            */

			ajax_dropdown = $(this).next('.ajax-dropdown');

			if (!ajax_dropdown.is(':visible')) {

				ajax_dropdown.fadeIn(150);

				$(this).addClass('active');

			}
			 else {
				
				ajax_dropdown.fadeOut(150);
				
				$(this).removeClass('active');

			}

		})

		$(document).mouseup(function (e) {
		    if (ajax_dropdown && !ajax_dropdown.is(e.target) && (ajax_dropdown.has(e.target).length === 0 || isEmpty(e.target))) {
				ajax_dropdown.fadeOut(150);
				$element.removeClass('active');
			}
		});
	}
	
	return{
		restrict:'EA',
		link:link
	}
});
"use strict";

angular.module('app').factory('activityService', function($http, $log, APP_CONFIG) {

	function getActivities(callback){

		$http.get(APP_CONFIG.apiRootUrl + '/activities/activity.json').success(function(data){

			callback(data);
				
		}).error(function(){

			$log.log('Error');
			callback([]);

		});

	}

	function getActivitiesByType(type, callback){

		$http.get(APP_CONFIG.apiRootUrl + '/activities/activity-' + type + '.json').success(function(data){

			callback(data);
				
		}).error(function(){

			$log.log('Error');
			callback([]);

		});

	}
	
	return{
		get:function(callback){
			getActivities(callback);
		},
		getbytype:function(type,callback){
			getActivitiesByType(type, callback);
		}
	}
});
"use strict";

angular.module('app').factory('Project', function($http, APP_CONFIG){
    return {
        list: $http.get(APP_CONFIG.apiRootUrl + '/projects.json')
    }
});
"use strict";

angular.module('app').directive('recentProjects', function(Project){
    return {
        restrict: "EA",
        replace: true,
        templateUrl: "app/dashboard/projects/recent-projects.tpl.html",
        scope: true,
        link: function(scope, element){

            Project.list.then(function(response){
                scope.projects = response.data;
            });
            scope.clearProjects = function(){
                scope.projects = [];
            }
        }
    }
});
"use strict";

angular.module('app').controller('TodoCtrl', function ($scope, $timeout, Todo) {
    $scope.newTodo = undefined;

    $scope.states = ['Critical', 'Important', 'Completed'];

    $scope.todos = Todo.getList().$object;

    // $scope.$watch('todos', function(){ }, true)

    $scope.toggleAdd = function () {
        if (!$scope.newTodo) {
            $scope.newTodo = {
                state: 'Important'
            };
        } else {
            $scope.newTodo = undefined;
        }
    };

    $scope.createTodo = function () {
        $scope.todos.push($scope.newTodo);
        $scope.newTodo = undefined;
        // $scope.newTodo.$save(function (respoonse) {
        //     $scope.todos.push(respoonse);
        //     $scope.newTodo = undefined;
        // });
    };

    $scope.deleteTodo = function (todo) {
        todo.remove().then(function () {
            $scope.todos.splice($scope.todos.indexOf(todo), 1);
        });

    };

});
'use strict';

angular.module('app.dataImporter').controller('dataImportCtrl', function ($scope, $http, $stateParams, $modalInstance, APP_CONFIG, Upload) {

    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.class;
    $scope.oid = $stateParams.oid;
    $scope.relatedclass = $stateParams.relatedclass;

    $scope.selectedScript = undefined;
    $scope.submitted = false;
    $scope.loading = false;

    var url;
    if (!$scope.relatedclass) {
        url = APP_CONFIG.ebaasRootUrl + "/api/import/scripts/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/All";
    }
    else
    {
        url = APP_CONFIG.ebaasRootUrl + "/api/import/scripts/" + encodeURIComponent($scope.dbschema) + "/" + $scope.relatedclass + "/All";
    }

    $http.get(url).success(function (data) {
        $scope.scripts = data.scripts;
    });

    // upload on file select or drop
    $scope.uploadFile = function (file) {
        $scope.loading = true;
        var uploadUrl;

        if ($scope.selectedScript.name === "Data Package") {
            uploadUrl = APP_CONFIG.ebaasRootUrl + "/api/import/datapackage/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass;
        }
        else {
            if (!$scope.relatedclass) {
                uploadUrl = APP_CONFIG.ebaasRootUrl + "/api/import/files/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + encodeURIComponent($scope.selectedScript.name);
            }
            else {
                uploadUrl = APP_CONFIG.ebaasRootUrl + "/api/import/files/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + $scope.oid + "/" + $scope.relatedclass + "/" + encodeURIComponent($scope.selectedScript.name);
            }
        }

        if ($scope.selectedScript) {
            
            Upload.upload({
                url: uploadUrl,
                data: { file: file }
            }).then(function (resp) {
                $scope.errorMsg = "";
                file.result = resp.data;
                $scope.loading = false;
                $scope.submitted = true;
                $scope.selectedScript = undefined;
                //console.log(resp);
            }, function (resp) {
                if (resp.status > 0)
                    $scope.errorMsg = resp.status + ': ' + resp.data.message;
                $scope.loading = false;
            }, function (evt) {
                var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                file.progress = progressPercentage;
            });
        }
    };

    $scope.selectFile = function()
    {
        var fileType = undefined
        if ($scope.file && $scope.file.name) {
            var extension = $scope.file.name.split('.').pop();
            if (extension) {
                switch (extension.toUpperCase()) {
                    case "XLS":
                    case "XLSX":
                        fileType = "Excel";
                        break;
                    case "TXT":
                    case "CSV":
                    case "DAT":

                        fileType = "Text";
                        break;
                    case "PAK":

                        fileType = "DataPackage";
                        break;
                    default:
                        fileType = "Other";
                        break;
                }

                var url;
                if (fileType != "DataPackage") {
                    if (!$scope.relatedclass) {
                        url = APP_CONFIG.ebaasRootUrl + "/api/import/scripts/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + fileType;
                    }
                    else {
                        url = APP_CONFIG.ebaasRootUrl + "/api/import/scripts/" + encodeURIComponent($scope.dbschema) + "/" + $scope.relatedclass + "/" + fileType;
                    }

                    $http.get(url).success(function (data) {
                        $scope.scripts = data.scripts;
                    });
                }
                else
                {
                    $scope.selectedScript = { name: "Data Package" };
                    $scope.scripts = [{ name: "Data Package" }];
                }
            }
        }
    }

    $scope.closeModal = function ()
    {
        if ($scope.submitted)
            $modalInstance.close("update");
        else
            $modalInstance.dismiss("dismiss");
    }
});
'use strict';

angular.module('app.dataviewer').controller('DataViewerBaseCtrl', function ($controller, $state, $scope, $rootScope, $http, $stateParams, APP_CONFIG, DataViewerService, FlotConfig) {

    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.class;
    $scope.oid = $stateParams.oid;
    $scope.xmlschema = $stateParams.xmlschema; // schema column name
    $scope.formAttribute = $stateParams.formAttribute;
    $scope.customApi = $stateParams.api;

    $scope.xmlSchemaName = undefined;

    $scope.displayed = [];
    $scope.currentField;
    $scope.fields = undefined;
    $scope.nonIndexFields = undefined;

    $scope.currentFrequency = "Second";
    $scope.frequencies = DataViewerService.getFrequencyArray();

    $scope.currentOperation = "Mean";
    $scope.operations = DataViewerService.getOperationArray();

    $scope.currentModelId = undefined;
    $scope.maxForecasts = [];

    $scope.lineChartData = [];
    $scope.isZoomIn = false;
    $scope.hasForecast = false;
    $scope.timeseries = undefined;

    $scope.isLoading = false;
    $scope.isReload = false;
    $scope.tableState;

    $scope.fromIndex = undefined;
    $scope.toIndex = undefined;
    $scope.fromPoint = undefined;
    $scope.toPoint = undefined;
    $scope.rangeFromIndex = undefined;
    $scope.rangeToIndex = undefined;

    $scope.categories = undefined;

    $scope.threshhold = undefined;
    
    $scope.currentCategory - undefined;
    $scope.timeSeriesMetricFields = undefined;
    $scope.timeSeriesXAxis = undefined;
    $scope.forecastModels = undefined;

    angular.extend(this, $controller('ebaasFormBaseCtrl', { $rootScope: $rootScope, $scope: $scope, $http: $http, APP_CONFIG: APP_CONFIG }));

    $scope.callServer = function callServer(tableState) {

        $scope.isLoading = true;
        $scope.tableState = tableState;

        var pagination = tableState.pagination;
        var start = pagination.start || 0;     // This is NOT the page number, but the index of item in the list that you want to use to display the table.
        var number = pagination.number || 20;  // Number of entries showed per page.

        if (!$scope.xmlSchemaName) {
            DataViewerService.getXmlSchemaName($scope.dbschema, $scope.dbclass, $scope.oid, $scope.xmlschema, function (name) {
                $scope.xmlSchemaName = name;
 
                if ($scope.xmlSchemaName) {
                    DataViewerService.getTimeSeriesCategories($scope.dbschema, $scope.dbclass, $scope.oid, $scope.xmlSchemaName, function (categories) {
                        $scope.categories = categories;
                        if (categories && categories.length > 0) {
                            $scope.currentCategory = categories[0];
                        }

                        DataViewerService.getTimeSeriesMetrciFields($scope.dbschema, $scope.dbclass, $scope.xmlSchemaName, function (fields) {
                            $scope.timeSeriesMetricFields = fields;
                            $scope.timeSeriesXAxis = GetXAxis(fields);
                            if (!$scope.isDateTimeXAxis()) {
                                $scope.currentFrequency = "None";
                            }

                            DataViewerService.getTimeSeriesMetric($scope.dbschema, $scope.dbclass, $scope.oid, $scope.xmlSchemaName, $scope.currentCategory, start, number, $scope.isReload, function (result) {
                                if (result) {
                                    tableState.pagination.numberOfPages = result.numberOfPages;//set the number of pages so the pagination can update

                                    $scope.displayData(result);

                                    if ($scope.currentField) {
                                        $scope.displayChart();
                                    }

                                    $scope.isLoading = false;
                                    $scope.isReload = false;
                                }
                            })
                        })
                    })
                }
                else
                {
                    $scope.isLoading = false;
                    $scope.isReload = false;
                }
            })
        }
        else {
            if ($scope.xmlSchemaName) {
                DataViewerService.getTimeSeriesMetric($scope.dbschema, $scope.dbclass, $scope.oid, $scope.xmlSchemaName, $scope.currentCategory, start, number, $scope.isReload, function (result) {
                    if (result) {
                        tableState.pagination.numberOfPages = result.numberOfPages;//set the number of pages so the pagination can update

                        $scope.displayData(result);

                        if ($scope.currentField) {
                            $scope.displayChart();
                        }

                        $scope.isLoading = false;
                        $scope.isReload = false;
                    }
                })
            }
        }
    }

    $scope.ceatingDir = false;
    $scope.renamingDir = false;

    $scope.createDirectory = function () {
        console.log("creatingDir is " + $scope.creatingDir);
        $scope.creatingDir = true;
    }

    $scope.renameDir = function () {
        $scope.renamingDir = true;
    }

    $scope.deleteDir = function () {
    }

    $scope.cancelAction = function () {
        $scope.ceatingDir = false;
        $scope.renamingDir = false;
    }

    $scope.closeModal = function () {
        $modalInstance.dismiss("dismiss");
    };

    $scope.displayData = function (result) {

        $scope.currentField = undefined;
        var data = result.data;
        
        if (data && data.length > 0) {
            var row = data[0];
            var fields = [];
            var nonIndexFields = [];

            for (var key in row) {
                if (row.hasOwnProperty(key)) {
                    fields.push(key);
                    if (key != "Index" && !isXAxis(key))
                    {
                        nonIndexFields.push(key);

                        if (!$scope.currentField) {
                            $scope.currentField = key;
                        }
                    }
                }
            }

            $scope.displayed = data;

            $scope.fields = fields;
            $scope.nonIndexFields = nonIndexFields;
        }
    };

    function GetXAxis(fields)
    {
        var xAxis = undefined;
        if (fields) {
            for (var i = 0; i < fields.length; i++)
            {
                var column = fields[i];

                if (column["xaxis"] && column["xaxis"] == true)
                {
                    xAxis = column;
                }
            }
        }

        return xAxis;
    }

    function isXAxis(key) {
        var status = false;

        if ($scope.timeSeriesXAxis &&
            $scope.timeSeriesXAxis["title"] == key)
        {
            status = true;
        }

        return status;
    }

    function getTimeSeriesName(title)
    {
        var name = title;

        if ($scope.timeSeriesMetricFields) {
            for (var i = 0; i < $scope.timeSeriesMetricFields.length; i++)
            {
                if ($scope.timeSeriesMetricFields[i]["title"] == title)
                {
                    name = $scope.timeSeriesMetricFields[i]["name"];
                    break;
                }
            }
        }

        return name;
    }

    function xAxisLabelGenerator(x) {
        if ($scope.timeseries)
        {
            if (x === parseInt(x, 10)) {
                if (x >= 0) {
                    if (x < $scope.timeseries.length) {
                        x = formatXVal($scope.timeseries[x]["Index"]);
                    }
                    else if ($scope.forecasteddata) {
                        var i = x - $scope.timeseries.length;
                        if (i < $scope.forecasteddata.length) {
                            x = formatXVal($scope.forecasteddata[i]["Index"]);
                        }
                    }
                }
            }
        }
   
        return x;
    }

    $scope.chartOptions =
        {
            series: {
                lines: {
                    show: true
                },
                points: {
                    show: true
                }
            },
            grid: {
                hoverable: true,
                clickable: true,
                tickColor: FlotConfig.chartBorderColor,
                borderWidth: 0,
                borderColor: FlotConfig.chartBorderColor
            },
            xaxis: {
                tickFormatter: xAxisLabelGenerator
            },
            selection: {
                mode: "x"
            },
            tooltip: false,
            tooltipOpts: {
                content: function (label, xval, yval, flotItem) {
                    return "";
                },
                defaultTheme: false
            },
            colors: [FlotConfig.chartSecond, FlotConfig.chartFourth]
        };

    $scope.hasModels = false;

    $scope.hasCategories = function () {
        if ($scope.categories && $scope.categories.length > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    $scope.loadCategoryData = function()
    {
        $scope.isReload = false;
        $scope.callServer($scope.tableState);
    }

    $scope.getXLable = function()
    {
        if ($scope.timeSeriesXAxis)
        {
            return $scope.timeSeriesXAxis["title"]
        }
        else
        {
            return "x axis";
        }
    }

    $scope.isDateTimeXAxis = function ()
    {
        if ($scope.timeSeriesXAxis) {
            if ($scope.timeSeriesXAxis["type"] &&
                ($scope.timeSeriesXAxis["type"] == "dateTime" ||
                 $scope.timeSeriesXAxis["type"] == "date"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else {
            return false;
        }
    }

    $scope.showThreshhold = function ()
    {
        if (isThreshholdInRange())
        {
            if ($scope.currentField) {
                $scope.displayChart();
            }
        }
        else
        {
            alert("The threshhold is out of range");
        }
    }

    $scope.displayChart = function () 
    {
        DataViewerService.getOneTimeSeries($scope.dbschema, $scope.dbclass, $scope.oid, $scope.xmlSchemaName,
            $scope.currentCategory,
            $scope.currentField,
            $scope.currentFrequency,
            $scope.currentOperation,
            function (result) {
                $scope.timeseries = result.data;
                if ($scope.timeseries &&
                    $scope.timeseries.length > 0) {
                    $scope.fromIndex = 0;
                    $scope.toIndex = $scope.timeseries.length - 1;
                    $scope.fromPoint = formatXVal($scope.timeseries[$scope.fromIndex]["Index"]);
                    $scope.toPoint = formatXVal($scope.timeseries[$scope.toIndex]["Index"]);
                }

   
                DataViewerService.getModelInfos($scope.dbschema, $scope.dbclass, $scope.xmlSchemaName,
                    getTimeSeriesName($scope.currentField),
                    $scope.currentFrequency,
                    function (modelInfos) {
                        if (modelInfos.length > 0)
                        {
                            $scope.hasModels = true;
                            $scope.forecastModels = modelInfos;
                            setMaxForecastOptions(modelInfos);
                        }
                        else
                        {
                            $scope.hasModels = false;
                            $scope.forecastModels = undefined;
                        }

                        drawChart(result.data, 0, result.data.length);
                    })
            });
    };

    function drawChart(data, from, to)
    {
        $scope.lineChartData = [
             {
                 data: _.range(from, to).map(function (i) {
                     return [i, data[i][$scope.currentField]];
                 }),
                 label: $scope.currentField
             }
        ];

        if ($scope.threshhold)
        {
            var line2 = {
                data: _.range(from, to).map(function (i) {
                    return [i, $scope.threshhold];
                }),
                dashes: { show: true },
                points: { show: false },
                color: "#83ce16"
            };

            $scope.lineChartData.push(line2);
        }

        var plot = $.plot($('#plotChart'), $scope.lineChartData, $scope.chartOptions);

        plot.setData($scope.lineChartData);
        plot.setupGrid(); //only necessary if your new data will change the axes or grid
        plot.draw();
    }

    function setMaxForecastOptions(modelInfos)
    {
        var options = [];
 
        for (var i = 0; i < modelInfos.length; i++)
        {
            var option = {};
            option["id"] = modelInfos[i]["ModelDirName"];
            var optionName = modelInfos[i]["MaxForecast"];
            if ($scope.isDateTimeXAxis()) {
                optionName += $scope.currentFrequency;
            }
            else
            {
                optionName += $scope.timeSeriesXAxis["title"];
            }
            option["name"] = optionName;
            options.push(option);
        }

        if (options.length > 0)
        {
            $scope.currentModelId = modelInfos[0]["ModelDirName"];
        }

        $scope.maxForecasts = options;
    }

    function isThreshholdInRange()
    {
        var status = true;

        return status;
    }

    $scope.canZoomout = function ()
    {
        return $scope.isZoomIn || $scope.hasForecast;
    }

    $scope.zoomOutChart = function () {
        zoomInOutChart(undefined, undefined);
    }

    $scope.canZoomIn = function ()
    {
        if ($scope.rangeFromIndex && $scope.rangeToIndex)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    $scope.zoomIn = function (from, to) {
        if (from && to) {
            $scope.rangeFromIndex = Math.round(from);
            $scope.rangeToIndex = Math.round(to);
            if ($scope.timeseries && $scope.timeseries.length > 0) {
                $scope.fromPoint = formatXVal($scope.timeseries[$scope.rangeFromIndex]["Index"]);
                $scope.toPoint = formatXVal($scope.timeseries[$scope.rangeToIndex]["Index"]);
            }
        }
    }

    $scope.zoomInChart = function () {
        zoomInOutChart($scope.rangeFromIndex, $scope.rangeToIndex);
    }

    function zoomInOutChart(from, to) {
        if (from && to) {
            $scope.fromIndex = Math.round(from);
            $scope.toIndex = Math.round(to);
            if ($scope.timeseries &&
                $scope.fromIndex >= 0 && $scope.fromIndex < $scope.timeseries.length &&
                $scope.toIndex >= 0 && $scope.toIndex < $scope.timeseries.length) {
                $scope.isZoomIn = true;

                drawChart($scope.timeseries, $scope.fromIndex, $scope.toIndex);
            }
        }
        else {
            $scope.isZoomIn = false;
            $scope.hasForecast = false;
            if ($scope.timeseries && $scope.timeseries.length > 0) {
                $scope.fromIndex = 0;
                $scope.toIndex = $scope.timeseries.length - 1;
                $scope.fromPoint = formatXVal($scope.timeseries[$scope.fromIndex]["Index"]);
                $scope.toPoint = formatXVal($scope.timeseries[$scope.toIndex]["Index"]);
                $scope.rangeFromIndex = undefined;
                $scope.rangeToIndex = undefined;
                drawChart($scope.timeseries, 0, $scope.timeseries.length);
            }
            else {
                $scope.fromIndex = undefined;
                $scope.toIndex = undefined;
                $scope.fromPoint = undefined;
                $scope.toPoint = undefined;
                $scope.rangeFromIndex = undefined;
                $scope.rangeToIndex = undefined;
            }
        }
    }

    $scope.forecasteddata = undefined;

    $scope.forecast = function()
    {
        var from = $scope.fromIndex;
        var to = $scope.toIndex; // to + 1 is the last point
        var rangeFrom = undefined;
        var rangTo = undefined;
        if ($scope.rangeFromIndex && $scope.rangeToIndex)
        {
            rangeFrom = $scope.rangeFromIndex;
            rangTo = $scope.rangeToIndex;
        }
        else
        {
            rangeFrom = from;
            rangTo = to;
        }

        var data = $scope.timeseries;
        // orignal line
        $scope.lineChartData = [
            {
                data: _.range(from, to + 1).map(function (i) {
                    return [i, data[i][$scope.currentField]];
                }),
                label: $scope.currentField
            }
        ];

        var timeSeries = getTimeSeriesArray($scope.timeseries, rangeFrom, rangTo);
        DataViewerService.forecastTimeSeries($scope.dbschema, $scope.dbclass, $scope.currentModelId, timeSeries, function (result) {
           
            $scope.forecasteddata = result.data;

            // connecting line
            var d2 = {
                data: [[rangTo, $scope.timeseries[rangTo][$scope.currentField]], [rangTo + 1, $scope.forecasteddata[0][$scope.currentField]]],
                label: "",
                color: "#FF7070"
            };
            $scope.lineChartData.splice(0, 0, d2);

            var d3 = {
                data: _.range($scope.forecasteddata.length).map(function (i) {
                    return [rangTo + 1 + i, $scope.forecasteddata[i][$scope.currentField]];
                }),
                label: $rootScope.getWord("Forecasted") + $scope.currentField,
                color: "#FF7070"
            };

            $scope.lineChartData.push(d3);
            $scope.hasForecast = true;

            if ($scope.threshhold) {
                var threshTo;
                if ($scope.rangeFromIndex && $scope.rangeToIndex) {
                    // has selection
                    threshTo = to + 1;
                }
                else
                {
                    threshTo = to + 1 + $scope.forecasteddata.length;
                }

                var l2 = {
                    data: _.range(from, threshTo).map(function (i) {
                        return [i, $scope.threshhold];
                    }),
                    dashes: { show: true },
                    points: { show: false },
                    color: "#83ce16"
                };

                $scope.lineChartData.push(l2);
            }

            var plot = $.plot($('#plotChart'), $scope.lineChartData, $scope.chartOptions);

            var ranges = {};
            ranges.xaxis = {};
            ranges.xaxis.from = rangeFrom;
            ranges.xaxis.to = rangTo;
            plot.setSelection(ranges, true);

            plot.setData($scope.lineChartData);
            plot.setupGrid(); //only necessary if your new data will change the axes or grid
            plot.draw();
        });
    }

    $scope.canExport = function () {
        if ($scope.xmlSchemaName)
        {
            return true;
        }
        else
        {
            return false;
        }
    };

    $scope.canDownload = function () {
        if ($scope.timeseries)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    $scope.downloadTimeSeries = function()
    {
        exportToCsv($scope.currentField + ".csv", $scope.timeseries, $scope.fromIndex, $scope.toIndex); // download as csv
    }

    $scope.RefreshData = function () {
        $scope.isReload = false;
        $scope.xmlSchemaName = undefined; // fresh load
        $scope.callServer($scope.tableState);
    }

    $scope.ExportCSV = function()
    {
        $scope.isLoading = true;
        if ($scope.xmlSchemaName) {
            DataViewerService.downloadTimeSeries($scope.dbschema, $scope.dbclass, $scope.oid, $scope.xmlSchemaName, $scope.currentCategory, function () {
                $scope.isLoading = false;
            })
        }
    }

    function exportToCsv(filename, rows, fromIndex, toIndex) {
        var processRow = function (row) {
            var finalVal = '';
            var j = 0;
            for (var key in row) {
                if (row.hasOwnProperty(key)) {
                    var innerValue = row[key] === null ? '' : row[key].toString();
                    var result = innerValue.replace(/"/g, '""');
                    if (result.search(/("|,|\n)/g) >= 0)
                        result = '"' + result + '"';
                    if (j > 0)
                        finalVal += ',';
                    finalVal += result;
                    j++;
                }
            }
            return finalVal + '\n';
        };

        var csvFile = '';
        var from = fromIndex;
        var length = toIndex + 1; // inlucde the last point
       
        var i = from;
        for (; i < length; i++) {
            csvFile += processRow(rows[i]);
        }

        var blob = new Blob([csvFile], { type: 'text/csv;charset=utf-8;' });
        if (navigator.msSaveBlob) { // IE 10+
            navigator.msSaveBlob(blob, filename);
        } else {
            var link = document.createElement("a");
            if (link.download !== undefined) { // feature detection
                // Browsers that support HTML5 download attribute
                var url = URL.createObjectURL(blob);
                link.setAttribute("href", url);
                link.setAttribute("download", filename);
                link.style.visibility = 'hidden';
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            }
        }
    }

    function getTimeSeries(rows, fromIndex, toIndex) {
        var processRow = function (row) {
            var finalVal = '';
            var j = 0;
            for (var key in row) {
                // we don't need index value
                if (key != "Index" &&
                    row.hasOwnProperty(key)) {
                    var innerValue = row[key] === null ? '' : row[key].toString();
                    var result = innerValue.replace(/"/g, '""');
                    if (result.search(/("|,|\n)/g) >= 0)
                        result = '"' + result + '"';
                    if (j > 0)
                        finalVal += ',';
                    finalVal += result;
                    j++;
                }
            }
            return finalVal;
        };

        var ts = "";
        var from = fromIndex;
        var length = toIndex + 1; // inlucde the last point
        
        var i = from;
        for (; i < length; i++) {
            if (ts.length > 0)
                ts += ";";
            ts += processRow(rows[i]);
        }

        return ts;
    }

    function getTimeSeriesArray(rows, fromIndex, toIndex) {
        var ts = [];
        var from = fromIndex;
        var length = toIndex + 1; // inlucde the last point

        var i = from;
        for (; i < length; i++) {
            var point = rows[i];
            ts.push(point);
        }

        return ts;
    }

    function isInt(value) {
        return !isNaN(value) &&
               parseInt(Number(value)) == value &&
               !isNaN(parseInt(value, 10));
    }

    function formatXVal(xval)
    {
        if (!isInt(xval)) {
            // formating datetime string which is in form of yyyy-MM-dd hh:mm:ss
            var dateTime = xval.split(" ");
            var str = xval;
            if (dateTime.length > 0)
            {
                var date = dateTime[0].split("-");
                str = "";
                if (date.length > 0)
                {
                    if (showDate()) {
                        str += date[0] + $rootScope.getWord("YearSeparator"); // year
                    }

                    if (date.length > 1)
                    {
                        if (showDate()) {
                            //var mm = date[1] - 1;
                            str += date[1] + $rootScope.getWord("MonthSeparator"); // month
                        }

                        if (date.length > 2)
                        {
                            if (showDate()) {
                                str += date[2] + $rootScope.getWord("DaySeparator"); // day
                            }

                            if (dateTime.length > 1)
                            {
                                var time = dateTime[1].split(":");
                                if (time.length > 0)
                                {
                                    str += time[0] + $rootScope.getWord("HourSeparator"); // hour

                                    if (time.length > 1)
                                    {
                                        str += time[1] + $rootScope.getWord("MinuteSeparator"); // minute

                                        if (time.length > 2)
                                        {
                                            //var s = parseInt(time[2]); //get rid of that 00.0;
                                            str += time[2] + $rootScope.getWord("SecondSeparator"); // second
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return str;
        }
        else
        {
            return xval; // if x value is integer, no formatting
        }
    }

    function showDate()
    {
        if ($scope.currentFrequency === "Minute" || 
            $scope.currentFrequency === "Second")
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // file directory code
    // url to get a directory tree
    $scope.baseUrl = APP_CONFIG.ebaasRootUrl;
    if (APP_CONFIG.hashedBaseUrls[$stateParams.cmdHash]) {
        $scope.baseUrl = APP_CONFIG.hashedBaseUrls[$stateParams.cmdHash];
    }

    var url = $scope.baseUrl + "/api/file/directory/" + $stateParams.schema + "/" + $stateParams.class + "/" + $stateParams.oid;
    $http.get(url).then(function (res) {

        var tree = createDirectoryTree(res.data);

        $scope.directoryTree = tree;
    });

    var createDirectoryTree = function (rootDir) {
        var roots = [];

        var root = {};
        root.name = rootDir.name;
        root.title = rootDir.name;
        root.children = [];
        root.expanded = true;
        root.parent = undefined;
        roots.push(root);

        addSubDirs(root, rootDir.subdirs);

        return roots;
    };

    var addSubDirs = function (parent, subDirs) {
        var subDir, node;

        for (var i = 0; i < subDirs.length; i += 1) {
            subDir = subDirs[i];
            node = {};
            node.children = [];

            node.name = subDir.name;
            node.title = subDir.name;
            node.children = [];
            node.expanded = true;
            node.parent = parent;

            parent.children.push(node);

            addSubDirs(node, subDir.subdirs);
        }
    };

    $scope.$watch('directory.currentNode', function (newObj, oldObj) {
        if ($scope.directory && angular.isObject($scope.directory.currentNode)) {

            $rootScope.$broadcast('directory.changedNode', { newNode: newObj });
        }
    }, false);

    $scope.selectedDir = undefined;

    $scope.$on('directory.changedNode', function (event, args) {
        var path = getPath(args.newNode);

        $scope.selectedDir = path;
    });

    $scope.addTimeSeries = function()
    {
        var frequency = $scope.currentFrequency;
        if (!$scope.isDateTimeXAxis())
        {
            frequency = "None";
        }
        $state.go(".timeseries", {frequency : frequency, ts: getTimeSeries($scope.timeseries, $scope.fromIndex, $scope.toIndex)});
    }

    var getPath = function(node) {
        var path = "";

        if (node.parent) {
            var parentPath = getPath(node.parent);
            if (parentPath) {
                path = parentPath + "\\" + node.name;
            }
            else {
                path = node.name;
            }
        }
        else
            path = node.name;

        return path;
    }
});

'use strict';

angular.module('app.dataviewer').controller('DataViewerCtrl', function ($controller, $scope, $rootScope, $http, $state, $stateParams, APP_CONFIG, DataViewerService, FlotConfig) {

    $scope.backTitle = $rootScope.getWord("Back");
    $scope.title = $rootScope.getWord("Test Data");

    $scope.goBackToPrev = function () {
        history.back(1);
    }

    $scope.selectData = function () {
        $state.go(".selectdata", { "schema": $scope.dbschema, "class": $scope.dbclass, "oid": $scope.oid, "xmlschema": $scope.xmlschema, "category": $scope.currentCategory, "api": $scope.customApi});
    }

    angular.extend(this, $controller('DataViewerBaseCtrl', { $scope: $scope, $rootScope: $rootScope, $http: $http, $stateParams: $stateParams, APP_CONFIG: APP_CONFIG, DataViewerService: DataViewerService, FlotConfig: FlotConfig }));
  
});

'use strict';

angular.module('app.dataviewer').controller('DataViewerModalCtrl', function ($controller, $scope, $rootScope, $http, $stateParams, APP_CONFIG, $modalInstance, DataViewerService, FlotConfig) {

    angular.extend(this, $controller('DataViewerBaseCtrl', { $scope: $scope, $rootScope: $rootScope, $http: $http, $stateParams: $stateParams, APP_CONFIG: APP_CONFIG, DataViewerService: DataViewerService, FlotConfig: FlotConfig }));
   
    $scope.closeModal = function () {
        $modalInstance.dismiss("dismiss");
    };

});

"use strict";

angular.module('app.dataviewer').factory('DataViewerService', function ($rootScope, $http, APP_CONFIG, fileManager) {

    function getTimeSeriesMetric(dbschema, dbclass, oid, xmlSchemaName, category, start, pageSize, reload, callback) {

        var url = APP_CONFIG.ebaasRootUrl + "/api/timeseries/metric/" + encodeURIComponent(dbschema) + "/" + dbclass + "/" + oid + "/" + xmlSchemaName + "?from=" + start + "&size=" + pageSize + "&reload=" + reload;
        if (category)
        {
            url += "&category=" + category;
        }

        $http.get(url).success(function (data) {
            var result = new Object();
            result.data = JSON.parse(data); // convert string to json
            
            // total point count
            url = APP_CONFIG.ebaasRootUrl + "/api/timeseries/metric/count/" + encodeURIComponent(dbschema) + "/" + dbclass + "/" + oid + "/" + xmlSchemaName;
            if (category) {
                url += "?category=" + category;
            }
            $http.get(url).success(function (count) {
               
                result.numberOfPages = Math.ceil(count / pageSize);
                result.count = count;
                callback(result);
            }).error(function () {
                callback({});
            });
        }).error(function () {
            callback({});
        });
    }

    function getTimeSeriesCategories(dbschema, dbclass, oid, xmlSchemaName, callback) {

        var url = APP_CONFIG.ebaasRootUrl + "/api/timeseries/metric/categories/" + encodeURIComponent(dbschema) + "/" + dbclass + "/" + oid + "/" + xmlSchemaName;

        $http.get(url).success(function (data) {
            callback(data);
        }).error(function () {
            callback({});
        });
    }

    function getOneTimeSeries(dbschema, dbclass, oid, xmlSchemaName, category, field, frequency, operation, callback) {

        var url = APP_CONFIG.ebaasRootUrl + "/api/timeseries/metric/" + encodeURIComponent(dbschema) + "/" + dbclass + "/" + oid + "/" + xmlSchemaName + "?field=" + field + "&frequency=" + frequency + "&operation=" + operation;

        if (category) {
            url += "&category=" + category;
        }

        $http.get(url).success(function (data) {
            var result = new Object();
            result.data = JSON.parse(data); // convert string to json
            callback(result);
        }).error(function () {
            callback({});
        });
    }

    function getXmlSchemaName(dbschema, dbclass, oid, xmlSchemaProperty, callback) {

        var url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent(dbschema) + "/" + dbclass + "/" + oid;

        $http.get(url).success(function (data) {
            var xmlSchemaName = data[xmlSchemaProperty];
            callback(xmlSchemaName);
        }).error(function () {
            callback([]);

        });
    }

    function downloadTimeSeries(dbschema, dbclass, oid, xmlSchemaName, category, callback) {

        var url = APP_CONFIG.ebaasRootUrl + "/api/timeseries/metric/file/" + encodeURIComponent(dbschema) + "/" + dbclass + "/" + oid + "/" + xmlSchemaName;

        if (category) {
            url += "?category=" + category;
        }

        fileManager.performDownload(url, function () {
            callback();
        });
    }

    // modelId is unique with a dbclass
    function forecastTimeSeries(dbschema, dbclass, modelId, timeSeries, callback) {

        var url = APP_CONFIG.ebaasRootUrl + "/api/timeseries/field/forecast/" + encodeURIComponent(dbschema) + "/" + dbclass + "/" + modelId;

        $http.post(url, timeSeries).success(function (data) {
            var result = new Object();
            result.data = JSON.parse(data); // convert string to json
            callback(result);
        }).error(function () {
            callback({});
        });
    }

    function getModelInfos(dbschema, dbclass, xmlSchemaName, fieldName, frequency, callback) {
        var url = APP_CONFIG.ebaasRootUrl + "/api/timeseries/field/model/" + encodeURIComponent(dbschema) + "/" + dbclass + "/" + xmlSchemaName + "?field=" + fieldName + "&frequency=" + frequency;

        $http.get(url).success(function (data) {
            var result = [];
            if (data) {
                result = data;
            }
            callback(result);
        }).error(function () {
            callback([]);
        });
    }

    function getTimeSeriesMetrciFields(dbschema, dbclass, xmlSchemaName, callback) {
        var url = APP_CONFIG.ebaasRootUrl + "/api/timeseries/metric/fields/" + encodeURIComponent(dbschema) + "/" + dbclass + "/" + xmlSchemaName;

        $http.get(url).success(function (data) {
            var result = [];
            if (data) {
                result = data;
            }
            callback(result);
        }).error(function () {
            callback([]);
        });
    }

    function sendTestData(dbschema, dbclass, oid, apiname, data, callback) {

        var url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent(dbschema) + "/" + dbclass + "/" + oid + "/custom/" + apiname;

        $http.post(url, data).success(function () {
            callback();
        });
    }

    function getFrequencyArray() {
        var frequencyArray = [];

        var item = {};
        item.id = "Second";
        item.name = $rootScope.getWord("Every Second");
        frequencyArray.push(item);

        item = {};
        item.id = "Minute";
        item.name = $rootScope.getWord("Every Minute");
        frequencyArray.push(item);

        item = {};
        item.id = "Hour";
        item.name = $rootScope.getWord("Every Hour");
        frequencyArray.push(item);

        item = {};
        item.id = "Day";
        item.name = $rootScope.getWord("Every Day");
        frequencyArray.push(item);

        item = {};
        item.id = "Month";
        item.name = $rootScope.getWord("Every Month");
        frequencyArray.push(item);

        return frequencyArray;
    }

    function getOperationArray() {
        var operationArray = [];

        var item = {};
        item.id = "Mean";
        item.name = $rootScope.getWord("Mean");
        operationArray.push(item);

        item = {};
        item.id = "Min";
        item.name = $rootScope.getWord("Min");
        operationArray.push(item);

        item = {};
        item.id = "Max";
        item.name = $rootScope.getWord("Max");
        operationArray.push(item);

        item = {};
        item.id = "Median";
        item.name = $rootScope.getWord("Median");
        operationArray.push(item);

        item = {};
        item.id = "FirstValue";
        item.name = $rootScope.getWord("FirstValue");
        operationArray.push(item);

        item = {};
        item.id = "LastValue";
        item.name = $rootScope.getWord("LastValue");
        operationArray.push(item);

        return operationArray;
    }

    return {
        getXmlSchemaName: function (dbschema, dbclass, oid, xmlSchemaProperty, callback)
        {
            getXmlSchemaName(dbschema, dbclass, oid, xmlSchemaProperty, callback);
        },
        getTimeSeriesMetric: function (dbschema, dbclass, oid, xmlSchemaName, category, start, pageSize, reload, callback) {
            getTimeSeriesMetric(dbschema, dbclass, oid, xmlSchemaName, category, start, pageSize, reload, callback);
        },
        getTimeSeriesCategories: function (dbschema, dbclass, oid, xmlSchemaName, callback) {
            getTimeSeriesCategories(dbschema, dbclass, oid, xmlSchemaName, callback);
        },
        getOneTimeSeries: function (dbschema, dbclass, oid, xmlSchemaName, category, field, frequency, operation, callback) {
            getOneTimeSeries(dbschema, dbclass, oid, xmlSchemaName, category, field, frequency, operation, callback);
        },
        downloadTimeSeries: function (dbschema, dbclass, oid, xmlSchemaName, category, callback) {
            downloadTimeSeries(dbschema, dbclass, oid, xmlSchemaName, category, callback);
        },
        forecastTimeSeries: function (dbschema, dbclass, modelId, timeSeries, callback) {
            forecastTimeSeries(dbschema, dbclass, modelId, timeSeries, callback);
        },
        getModelInfos: function (dbschema, dbclass, xmlSchemaName, fieldName, frequency, callback) {
            getModelInfos(dbschema, dbclass, xmlSchemaName, fieldName, frequency, callback);
        },
        getTimeSeriesMetrciFields: function (dbschema, dbclass, xmlSchemaName, callback) {
            getTimeSeriesMetrciFields(dbschema, dbclass, xmlSchemaName, callback);
        },
        getFrequencyArray: function () {
            return getFrequencyArray();
        },
        getOperationArray: function () {
            return getOperationArray();
        },
        sendTestData: function (dbschema, dbclass, oid, apiname, data, callback) {
            sendTestData(dbschema, dbclass, oid, apiname, data, callback);
        }
	}
});
"use strict";

angular.module('app.dataviewer').directive('flotLineChart', function (FlotConfig) {
    return {
        restrict: 'E',
        replace: true,
        template: '<div id="plotChart" class="chart" style="width:100%;height:400px"></div>',
        scope: {
            data: '=',
            timeseries: '=',
            forecasteddata: '=',
            frequency: "=",
            setSelectedRange: '='
        },
        link: function (scope, element) {

            var plot = $.plot(element, scope.data, scope.chartOptions);

            element.on("plothover", function (event, pos, item) {
                if (item) {
                    var x = "";
                    if (item.datapoint[0] < scope.timeseries.length) {
                        x = getXValue(scope.timeseries[item.datapoint[0]]['Index']);
                    }
                    else if (scope.forecasteddata && item.dataIndex < scope.forecasteddata.length)
                    {
                        x = getXValue(scope.forecasteddata[item.dataIndex]['Index']);
                    }
                    $("#hoverdata").text("(" + item.series.label + ":  x = " + x + ";  y = " + item.datapoint[1] + ")");
                    //plot.highlight(item.series, item.datapoint);
                }
            });

            element.on("plotselected", function (event, ranges) {
                if (ranges) {
                    if (typeof scope.setSelectedRange(ranges.xaxis.from, ranges.xaxis.to) === 'function') {
                        //scope.setSelectedRange()(ranges.xaxis.from, ranges.xaxis.to);
                    }
                }
            });

            element.on("plotunselected", function (event) {
                if (typeof scope.setSelectedRange(undefined, undefined) === 'function') {
                    //scope.setSelectedRange()(undefined, undefined);
                }
            });

            function getXValue(val)
            {
                if (scope.frequency === "Minute" ||
                    scope.frequency === "Second")
                {
                    var pos = val.indexOf(' ');
                    if (pos > 0)
                    {
                        return val.substring(pos + 1);
                    }
                    else
                    {
                        return val;
                    }

                }
                else
                {
                    return val;
                }
            }
        }
    }
});
'use strict';

angular.module('app.dataviewer').controller('SelectDataModalCtrl', function ($controller, $state, $scope, $rootScope, $http, $stateParams, APP_CONFIG, DataViewerService, $modalInstance) {
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.class;
    $scope.oid = $stateParams.oid;
    $scope.xmlschema = $stateParams.xmlschema; // schema column name
    $scope.currentCategory = $stateParams.category;
    $scope.customApi = $stateParams.api;

    $scope.xmlSchemaName = undefined;

    $scope.displayed = [];
    $scope.fields = undefined;

    $scope.isLoading = false;
    $scope.isReload = false;
    $scope.tableState;

    $scope.isSending = false;

    // indexes of selected rows
    $scope.selected = [];
    $scope.hashTable = {};

    // names of selected columns
    $scope.columnSelected = [];

    $scope.callServer = function callServer(tableState) {

        $scope.isLoading = true;
        $scope.tableState = tableState;

        var pagination = tableState.pagination;
        var start = pagination.start || 0;     // This is NOT the page number, but the index of item in the list that you want to use to display the table.
        var number = pagination.number || 20;  // Number of entries showed per page.

        if (!$scope.xmlSchemaName) {
            DataViewerService.getXmlSchemaName($scope.dbschema, $scope.dbclass, $scope.oid, $scope.xmlschema, function (name) {
                $scope.xmlSchemaName = name;

                if ($scope.xmlSchemaName) {
                    DataViewerService.getTimeSeriesMetrciFields($scope.dbschema, $scope.dbclass, $scope.xmlSchemaName, function (fields) {
                        $scope.timeSeriesMetricFields = fields;

                        DataViewerService.getTimeSeriesMetric($scope.dbschema, $scope.dbclass, $scope.oid, $scope.xmlSchemaName, $scope.currentCategory, start, number, $scope.isReload, function (result) {
                            if (result) {
                                tableState.pagination.numberOfPages = result.numberOfPages;//set the number of pages so the pagination can update

                                $scope.displayData(result);

                                $scope.isLoading = false;
                                $scope.isReload = false;
                            }
                        })
                    })
                }
                else {
                    $scope.isLoading = false;
                    $scope.isReload = false;
                }
            })
        }
        else {
            if ($scope.xmlSchemaName) {
                DataViewerService.getTimeSeriesMetric($scope.dbschema, $scope.dbclass, $scope.oid, $scope.xmlSchemaName, $scope.currentCategory, start, number, $scope.isReload, function (result) {
                    if (result) {
                        tableState.pagination.numberOfPages = result.numberOfPages;//set the number of pages so the pagination can update

                        $scope.displayData(result);

                        $scope.isLoading = false;
                        $scope.isReload = false;
                    }
                })
            }
        }
    };

    $scope.select = function (rowIndex) {

        var found = $scope.selected.indexOf(rowIndex);

        if (found == -1) {
            $scope.selected.push(rowIndex);

            $scope.hashTable[rowIndex] = getRowData(rowIndex);
        }
        else {
            $scope.selected.splice(found, 1);
            delete $scope.hashTable[rowIndex];
        }
    };

    function getRowData(rowIndex)
    {
        var found = undefined;

        for (var i = 0; i < $scope.displayed.length; i++)
        {
            if ($scope.displayed[i]["Index"] == rowIndex)
            {
                found = JSON.parse(JSON.stringify($scope.displayed[i]));
                break;
            }
        }

        return found;
    }

    $scope.toggleSelection = function(columName)
    {
        var found = $scope.columnSelected.indexOf(columName);

        if (found == -1)
            $scope.columnSelected.push(columName);
        else
            $scope.columnSelected.splice(found, 1);
    }

    $scope.displayData = function (result) {
        var data = result.data;

        if (data && data.length > 0) {
            var row = data[0];
            var fields = [];

            for (var key in row) {
                if (row.hasOwnProperty(key)) {
                    fields.push(key);
                }
            }

            if ($scope.columnSelected.length == 0)
            {
                // initialize the column selection
                for (var key in row) {
                    if (row.hasOwnProperty(key)) {
                        if (key != "Index") {
                            $scope.columnSelected.push(key);
                        }
                    }
                }
            }

            for (var i = 0; i < data.length; i++) {
                var found = $scope.selected.indexOf(data[i]["Index"]);

                if (found == -1)
                    data[i].isSelected = false;
                else
                    data[i].isSelected = true;
            }

            $scope.displayed = data;

            $scope.fields = fields;
        }
    };

    $scope.confirm = function () {
        if (!$scope.customApi)
            alert($rootScope.getWord("Missing Custom API"));
        else {
            if ($scope.selected.length == 0)
            {
                alert($rootScope.getWord("No rows selected"));
                return;
            }

            if ($scope.columnSelected.length == 0) {
                alert($rootScope.getWord("No columns selected"));
                return;
            }

            var selectedData = getSelectedData();

            $scope.isSending = true;
            DataViewerService.sendTestData($scope.dbschema, $scope.dbclass, $scope.oid, $scope.customApi, selectedData, function () {
                $scope.isSending = false;
                alert($rootScope.getWord("Data Sent"));
            });
        }
    }

    function getSelectedData() {
        var rows = [];
        var newRow;

        for (var i = 0; i < $scope.selected.length; i++)
        {
            var rowIndex = $scope.selected[i];
            var row = $scope.hashTable[rowIndex];

            newRow = {};

            for (var key in row) {
                if (row.hasOwnProperty(key)) {
                    var found = $scope.columnSelected.indexOf(key);

                    if (found != -1) {
                        newRow[key] = row[key];
                    }
                }
            }

            rows.push(newRow);
        }

        return {"rows": rows};
    }

    $scope.closeModal = function () {
        $modalInstance.dismiss("dismiss");
    };
});

'use strict';

angular.module('app.dataviewer').controller('TimeSeriesModalCtrl', function ($controller, $scope, $rootScope, $http, $stateParams, APP_CONFIG, $modalInstance, DataViewerService, FlotConfig) {

    $scope.frequency = $stateParams.frequency;
    $scope.ts = $stateParams.ts;
    $scope.isLoading = false;

    //localStorage.clear();
    $scope.rowCollection = [];

    if (localStorage.getItem("time-series-set")) {
        var savedRows = localStorage.getItem("time-series-set");
        if (savedRows.length > 0) {
            var rows = JSON.parse(savedRows);
            $scope.rowCollection = rows;
        }
    }

    var row = {};
    row.id = "ts" + $scope.rowCollection.length;
    row.frequency = $scope.frequency;
    row.data = $scope.ts;
    row.isSelected = true; // select the new row

    for (var i = 0; i < $scope.rowCollection.length; i++)
    {
        $scope.rowCollection[i].isSelected = false;
    }

    $scope.rowCollection.push(row);

    $scope.checkFileAPI = function() {
        if (window.File && window.FileReader && window.FileList && window.Blob) {
            // Great success! All the File APIs are supported.
        } else {
            alert('The File APIs are not fully supported in this browser.');
        }
    }

    $scope.handleSelectedFile = function(file)
    {
        var reader = new FileReader();

        if (file) {
            reader.readAsText(file);
        }
        
        reader.onloadend = function (e) {
            if (reader.result) {
                var lines = reader.result.split("\n");

                var rowCollection = getRowCollection(lines);
                var count = rowCollection.length;
                // append the current time series to the collection
                for (var i = 0; i < $scope.rowCollection.length; i++)
                {
                    $scope.rowCollection[i].id = "ts" + count;
                    rowCollection.push($scope.rowCollection[i]);
                    count++;
                }

                $scope.rowCollection = rowCollection;
            }
        }
    }

    function getRowCollection(lines)
    {
        var rows = [];
        var row;
        for (var i = 0; i < lines.length; i++)
        {
            if (lines[i].length > 0) {
                row = {};
                var items = lines[i].split(";");
                var index;
                if (items.length > 2) {
                    row.id = items[0];
                    row.frequency = items[1];
                    index = lines[i].indexOf(row.frequency) + row.frequency.length + 1;
                    row.data = lines[i].substring(index); // rest is data

                    rows.push(row);
                }
            }
        }

        return rows;
    }

    $scope.deleteRows = function()
    {
        var i = $scope.rowCollection.length;
        while (i--) {
            if ($scope.rowCollection[i].isSelected) {
                $scope.rowCollection.splice(i, 1);
            }
        }
    }

    $scope.clearRows = function()
    {
        var result = confirm($rootScope.getWord("Confirm Clear All"));
        if (result) {
            $scope.rowCollection = [];
        }
    }

    $scope.downloadFile = function()
    {
        $scope.isLoading = true;
        var filename = "ts.txt";
        var processRow = function (row) {
            var finalVal = '';
            var j = 0;
            for (var key in row) {
                if (row.hasOwnProperty(key) && key != "isSelected") {
                    var innerValue = row[key] === null ? '' : row[key].toString();
                    var result = innerValue.replace(/"/g, '""');
                    if (result.search(/("|,|\n)/g) >= 0)
                        result = '"' + result + '"';
                    if (j > 0)
                        finalVal += ';';
                    finalVal += result;
                    j++;
                }
            }
            return finalVal + '\r\n';
        };

        var content = '';
       
        for (var i= 0; i < $scope.rowCollection.length; i++) {
            content += processRow($scope.rowCollection[i]);
        }

        var blob = new Blob([content], { type: 'plain/text;charset=utf-8;' });
        if (navigator.msSaveBlob) { // IE 10+
            navigator.msSaveBlob(blob, filename);
        } else {
            var link = document.createElement("a");
            if (link.download !== undefined) { // feature detection
                // Browsers that support HTML5 download attribute
                var url = URL.createObjectURL(blob);
                link.setAttribute("href", url);
                link.setAttribute("download", filename);
                link.style.visibility = 'hidden';
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            }
        }

        $scope.isLoading = false;
    }

    $scope.closeModal = function () {
        if ($scope.rowCollection.length > 0) {
            localStorage.setItem('time-series-set', JSON.stringify($scope.rowCollection));
        }
        else
        {
            localStorage.setItem('time-series-set', "");
        }
        $modalInstance.dismiss("dismiss");
    };

});

angular.module('app.dataviewer').directive('filelistBind', function () {
    return function (scope, elm, attrs) {
        elm.bind('change', function (evt) {
            scope.$apply(function () {
                scope.handleSelectedFile(evt.target.files[0]);
            });
        });
    };
});

function rowSelect() {
    return {
        require: '^stTable',
        template: '<input type="checkbox">',
        scope: {
            row: '=rowSelect'
        },
        link: function (scope, element, attr, ctrl) {

            element.bind('click', function (evt) {

                scope.$apply(function () {

                    ctrl.select(scope.row, 'multiple');

                });

            });

            scope.$watch('row.isSelected', function (newValue) {
                if (newValue === true) {

                    element.parent().addClass('st-selected');
                    element.find('input').attr('checked', true);

                } else {

                    element.parent().removeClass('st-selected');
                    element.find('input').attr('checked', false);

                }
            });
        }
    };
}

angular
  .module('app.dataviewer')
  .directive('rowSelect', rowSelect)
'use strict';

angular.module('app.formeditor').controller('formEditorCtrl', function ($scope, $rootScope, $state, APP_CONFIG, $stateParams, formEditorService, $templateCache) {

    $scope.dbschema = $stateParams.schema;
    formEditorService.formInfo.dbclass = undefined;
    formEditorService.formInfo.classTitle = undefined;
    formEditorService.formInfo.formName = undefined;
    $scope.content =  $rootScope.getWord("Form Editor Tip");
    $scope.saved = false;
    $scope.previewId = 0;

    $scope.saveModel = function () {
        if (formEditorService.formInfo.dbclass && formEditorService.formInfo.formName) {
            formEditorService.saveFormFile($scope.dbschema, formEditorService.formInfo.dbclass, formEditorService.formInfo.formName, $scope.content, function () {
                BootstrapDialog.show({
                    title: $rootScope.getWord("Info Dialog"),
                    type: BootstrapDialog.TYPE_INFO,
                    message: $rootScope.getWord("Form Saved"),
                    buttons: [{
                        label: $rootScope.getWord("Cancel"),
                        action: function (dialog) {
                            dialog.close();
                        }
                    }]
                });
                $scope.saved = true;
            });
        }
        else
        {
            BootstrapDialog.show({
                title: $rootScope.getWord("Info Dialog"),
                type: BootstrapDialog.TYPE_INFO,
                message: $rootScope.getWord("Open Form First"),
                buttons: [{
                    label: $rootScope.getWord("Cancel"),
                    action: function (dialog) {
                        dialog.close();
                    }
                }]
            });
        }
    }

    $scope.previewForm = function () {
        if (formEditorService.formInfo.dbclass && formEditorService.formInfo.formName) {
            if ($scope.saved) {
                // previewId is used to prevent template caching, it has a different id each time
                $scope.previewId++;
                $state.go('.preview', { schema: $stateParams.schema, class: formEditorService.formInfo.dbclass, template: formEditorService.formInfo.formName + ".htm", previewid: $scope.previewId });
            }
            else
            {
                BootstrapDialog.show({
                    title: $rootScope.getWord("Info Dialog"),
                    type: BootstrapDialog.TYPE_INFO,
                    message: $rootScope.getWord("Save Form First"),
                    buttons: [{
                        label: $rootScope.getWord("Cancel"),
                        action: function (dialog) {
                            dialog.close();
                        }
                    }]
                });
            }
        }
        else
        {
            BootstrapDialog.show({
                title: $rootScope.getWord("Info Dialog"),
                type: BootstrapDialog.TYPE_INFO,
                message: $rootScope.getWord("Open Form First"),
                buttons: [{
                    label: $rootScope.getWord("Cancel"),
                    action: function (dialog) {
                        dialog.close();
                    }
                }]
            });
        }
    }

    $scope.loadForm = function()
    {
        if (formEditorService.formInfo.dbclass) {

            formEditorService.getFormFile($scope.dbschema, formEditorService.formInfo.dbclass, formEditorService.formInfo.formName, function (data) {
                $scope.content = data;
                if (data != "") {
                    $scope.saved = true; // allow preview an existing form
                }
            });
        }
        else
        {
            $scope.content = "";
            $scope.saved = false;
        }
    }

    $scope.getTitle = function()
    {
        var className = formEditorService.formInfo.classTitle;
        var formName = formEditorService.formInfo.formName;
        if (!className)
        {
            className = "None";
        }
        if (!formName)
        {
            formName = "None";
        }
        return String.format($rootScope.getWord("Form Info"), className, formName);
    }

    /*
    $scope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
        var answer = confirm("Are you sure you want to leave this page?")
        if (!answer) {
            event.preventDefault();
        }
    });
    */
});

"use strict";

angular.module('app.formeditor').factory('formEditorService', function ($http, APP_CONFIG) {

    var contextModel = {
        dbschema: undefined,
        dbclass: undefined,
        selectedProperty: undefined
    };

    var formInfo = {
        dbclass: undefined,
        classTitle: undefined,
        formName: undefined
    }

    function getClassTreeData(dbschema, callback) {

        var url = APP_CONFIG.ebaasRootUrl + "/api/metadata/schematree/" + encodeURIComponent(dbschema);

        $http.get(url).success(function (data) {
            callback(data);
        }).error(function () {
            callback([]);

        });
    }

    function getRelationshipTreeData(dbschema, dbclass, callback) {

        var url = APP_CONFIG.ebaasRootUrl + "/api/metadata/relationtree/" + encodeURIComponent(dbschema) + "/" + dbclass;

        $http.get(url).success(function (data) {
            callback(data);
        }).error(function () {
            callback([]);

        });
    }

    function getClassProperties(dbschema, dbclass, callback) {

        var url = APP_CONFIG.ebaasRootUrl + "/api/metadata/view/" + encodeURIComponent(dbschema) + "/" + dbclass + "?view=full"

        $http.get(url).success(function (data) {
            //console.debug(JSON.stringify(data));
            callback(data);
        }).error(function () {
            callback([]);

        });
    }

    function getLeafClasses(dbschema, dbclass, callback) {

        var url = APP_CONFIG.ebaasRootUrl + "/api/metadata/leafclasses/" + encodeURIComponent(dbschema) + "/" + dbclass;

        $http.get(url).success(function (data) {
            callback(data);
        }).error(function () {
            callback([]);

        });
    }

    function getFormFiles(dbschema, dbclass, callback) {

        var url = APP_CONFIG.ebaasRootUrl + "/api/form/layouts/" + dbschema + "/" + dbclass;

        $http.get(url).success(function (data) {
            callback(data);
        }).error(function () {
            callback([]);

        });
    }

    function getFormFile(dbschema, dbclass, formName, callback) {

        var url = APP_CONFIG.ebaasRootUrl + "/api/form/layout/" + dbschema + "/" + dbclass + "/" + formName;

        $http.get(url).success(function (data) {
            callback(data);
        }).error(function () {
            callback([]);

        });
    }

    function saveFormFile(dbschema, dbclass, formName, content, callback) {

        var url = APP_CONFIG.ebaasRootUrl + "/api/form/layout/" + dbschema + "/" + dbclass + "/" + formName;

        $http.post(url, JSON.stringify(content)).success(function () {
            callback();
        }).error(function () {
            callback();

        });
    }

    return {
        contextModel: function()
        {
            return contextModel;
        },
        formInfo : function ()
        {
            return formInfo;
        },
        getFormFiles: function (dbschema, dbclass, callback)
        {
            getFormFiles(dbschema, dbclass, callback);
        },
        getFormFile: function (dbschema, dbclass, formName, callback) {
            getFormFile(dbschema, dbclass, formName, callback);
        },
        saveFormFile: function (dbschema, dbclass, formName, content, callback) {
            saveFormFile(dbschema, dbclass, formName, content, callback);
        },
        getClassTreeData: function (dbschema, callback) {
            getClassTreeData(dbschema, callback);
        },
        getRelationshipTreeData: function (dbschema, dbclass, callback) {
            getRelationshipTreeData(dbschema, dbclass, callback);
        },
        getLeafClasses: function (dbschema, dbclass, callback) {
            getLeafClasses(dbschema, dbclass, callback);
        },
        getClassProperties: function (dbschema, dbclass, callback) {
            getClassProperties(dbschema, dbclass, callback);
        }
	}
});
'use strict';

angular.module('app.formeditor').controller('insertPropertyCtrl', function ($scope, $rootScope, APP_CONFIG, $stateParams, formEditorService) {

    $scope.Initialize = function () {
        $scope.properties = [];
        $scope.leafClasses = [];
        $scope.isReadOnly = false;
        $scope.includeLabel = true;
        $scope.selectedClass = undefined;

        formEditorService.getRelationshipTreeData(formEditorService.contextModel.dbschema, formEditorService.contextModel.dbclass, function (data) {
            var roots = [];
            roots.push(data);
            $scope.treedata = roots;

            formEditorService.getClassProperties(formEditorService.contextModel.dbschema, formEditorService.contextModel.dbclass, function (data) {
                $scope.selectedClass = formEditorService.contextModel.dbclass;
                $scope.properties = getProperties(data);
            });
        });
    };

    $scope.Initialize();

    $scope.reInitialize = function()
    {
        if ($scope.selectedClass &&
            $scope.selectedClass != formEditorService.contextModel.dbclass)
        {
            $scope.Initialize();
        }
    }

    $scope.$watch('classes.currentNode', function (newObj, oldObj) {
        if ($scope.classes && angular.isObject($scope.classes.currentNode)) {
            formEditorService.getClassProperties(formEditorService.contextModel.dbschema, $scope.classes.currentNode.name, function (data) {
                $scope.properties = getProperties(data);
                $scope.selectedClass = $scope.classes.currentNode.name;
                if (!$scope.classes.currentNode.leaf)
                {
                    formEditorService.getLeafClasses(formEditorService.contextModel.dbschema, $scope.classes.currentNode.name, function (data) {
                        $scope.leafClasses = data;
                    });
                }
                else
                {
                    $scope.leafClasses = [];
                }
            });
        }
    }, false);

    $scope.selectLeafClass = function()
    {
        if ($scope.selectedLeafClass) {
            formEditorService.getClassProperties(formEditorService.contextModel.dbschema, $scope.selectedLeafClass.name, function (data) {
                $scope.properties = getProperties(data);
                $scope.selectedClass = $scope.selectedLeafClass.name;
            });
        }
    }

    $scope.selectProperty = function () {
        $scope.isReadOnly = false;
        formEditorService.contextModel.selectedProperty = $scope.selectedClassProperty;
    }

    $scope.getFieldElement = function()
    {
        var selectedProperty = formEditorService.contextModel.selectedProperty;
        if (!selectedProperty)
        {
            return undefined;
        }
        else
        {
            var html = '<div class="form-group">';
            if ($scope.includeLabel)
            {
                html += '<label>' + selectedProperty.label + '</label>';
            }
            html += '<input class="form-control" name="' + $scope.selectedClass + '_' + selectedProperty.name + '"';
            if ($scope.isReadOnly)
            {
                html += ' read="true"';
            }
            html += '/></div>';

            return html;
        }
    }

    var getProperties = function(data)
    {
        var column;
        var columns = [];

        // data is a JSON Schema for the class
        var properties = data.properties; // data.properies contains infos of each property of the schema

        var propertyInfo;
        for (var property in properties) {
            if (properties.hasOwnProperty(property)) {
                propertyInfo = properties[property];
                column = {};
                column.name = property;
                column.label = propertyInfo["title"];
                if (!propertyInfo["description"])
                    column.title = propertyInfo["title"];
                else
                    column.title = propertyInfo["title"] + " (" + propertyInfo["description"] + ")";
                columns.push(column);
            }
        }

        return columns;
    }
});

'use strict';

angular.module('app.formeditor').controller('openFileCtrl', function ($scope, $rootScope, APP_CONFIG, $stateParams, formEditorService) {

    $scope.dbschema = $stateParams.schema;
    formEditorService.contextModel.dbschema = $stateParams.schema;
    formEditorService.formInfo.formName = undefined;
    formEditorService.formInfo.classTitle = undefined;
    formEditorService.formInfo.dbclass = undefined;
    $scope.nodeType = "Folder";
    $scope.selectedVal = undefined;
    $scope.editorService = formEditorService;

    $scope.filenames = [];

    formEditorService.getClassTreeData($scope.dbschema, function (data) {
        $scope.treedata = data;
    });

    $scope.$watch('classes.currentNode', function (newObj, oldObj) {
        if ($scope.classes && angular.isObject($scope.classes.currentNode)) {
            if ($scope.classes.currentNode["type"] &&
                $scope.classes.currentNode["type"] === "Folder") {
                $scope.nodeType = "Folder";
                formEditorService.formInfo.dbclass = undefined;
                formEditorService.formInfo.classTitle = undefined;
                $scope.filenames = [];
                formEditorService.formInfo.formName = undefined;
            }
            else {
                formEditorService.getFormFiles($scope.dbschema, $scope.classes.currentNode.name, function (data) {
                    formEditorService.formInfo.dbclass = $scope.classes.currentNode.name;
                    formEditorService.formInfo.classTitle = $scope.classes.currentNode.title;
                    $scope.nodeType = "Class";
                    $scope.filenames = data;
                    formEditorService.contextModel.dbclass = formEditorService.formInfo.dbclass;
                    if ($scope.filenames && $scope.filenames.length > 0) {
                        $scope.selectedVal = $scope.filenames[0];
                        formEditorService.formInfo.formName = $scope.filenames[0];
                    }
                    else {
                        formEditorService.formInfo.formName = undefined;
                    }
                });
            }
        }
    }, false);

    $scope.selectFile = function()
    {
        formEditorService.formInfo.formName = $scope.selectedVal;
    }
});

'use strict';

angular.module('app.smartforms').directive('ckEditor', function (APP_CONFIG, $stateParams, $compile) {
    return {
        require: '?ngModel',
        link: function (scope, elm, attr, ngModel) {
            CKEDITOR.config.contentsCss = ['plugin/bootstrap/dist/css/bootstrap.min.css'];
            var ck = CKEDITOR.replace(elm[0], { height: '450px', startupFocus: true });
            if (!ngModel) return;
            ck.scope = scope;
            ck.compile = $compile;
            ck.on('instanceReady', function () {
                ck.setData(ngModel.$viewValue);
            });
            function updateModel() {
                scope.$apply(function () {
                    ngModel.$setViewValue(ck.getData());
                });
            };
            function saveModel() {
                scope.saveModel();

                return false;
            }
            ck.on('change', updateModel);
            ck.on('key', updateModel);
            ck.on('dataReady', updateModel);
            ck.on('save', saveModel);

            ngModel.$render = function (value) {
                ck.setData(ngModel.$viewValue);
            };
        }
    };
});


"use strict";

angular.module('app.forms').controller('FormLayoutsCtrl', function($scope, $modal, $log){

    $scope.openModal = function () {
        var modalInstance = $modal.open({
            templateUrl: 'app/forms/views/form-layout-modal.html',
            controller: 'ModalDemoCtrl' 
        });

        modalInstance.result.then(function () {
            $log.info('Modal closed at: ' + new Date());

        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });


    };

    $scope.registration = {};

    $scope.$watch('registration.date', function(changed){
        console.log('registration model changed', $scope.registration)
    })


});

"use strict";

angular.module('app.forms').controller('FormPluginsCtrl', function($scope, $log){

	$scope.editableOptions =  {
		mode: 'popup',
		disabled: false
	};

	$scope.toggleInline = function() {
		if($scope.editableOptions.mode == 'popup') {
			$scope.editableOptions.mode = 'inline';
		}
		else {
			$scope.editableOptions.mode = 'popup'
		}
	};

	$scope.toggleDisabled = function() {
		$scope.editableOptions.disabled = !$scope.editableOptions.disabled;
	};

});
"use strict";


angular.module('app.forms').controller('FormWizardCtrl', function($scope){

    $scope.wizard1CompleteCallback = function(wizardData){
        console.log('wizard1CompleteCallback', wizardData);
        $.smallBox({
            title: "Congratulations! Smart wizard finished",
            content: "<i class='fa fa-clock-o'></i> <i>1 seconds ago...</i>",
            color: "#5F895F",
            iconSmall: "fa fa-check bounce animated",
            timeout: 4000
        });
    };

    $scope.wizard2CompleteCallback = function(wizardData){
        console.log('wizard2CompleteCallback', wizardData);
        $.smallBox({
            title: "Congratulations! Smart fuekux wizard finished",
            content: "<i class='fa fa-clock-o'></i> <i>1 seconds ago...</i>",
            color: "#5F895F",
            iconSmall: "fa fa-check bounce animated",
            timeout: 4000
        });

    };

});
"use strict";

angular.module('app.forms').controller('FormXeditableCtrl', function($scope, $log){

    $scope.username = 'superuser';
    $scope.firstname = null;
    $scope.sex = 'not selected';
    $scope.group = "Admin";
    $scope.vacation = "25.02.2013";
    $scope.combodate = "15/05/1984";
    $scope.event = null;
    $scope.comments = 'awesome user!';
    $scope.state2 = 'California';
    $scope.fruits = 'peach<br/>apple';
    

    $scope.fruits_data = [
        {value: 'banana', text: 'banana'},
        {value: 'peach', text: 'peach'},
        {value: 'apple', text: 'apple'},
        {value: 'watermelon', text: 'watermelon'},
        {value: 'orange', text: 'orange'}]
    ;


    $scope.genders =  [
        {value: 'not selected', text: 'not selected'},
        {value: 'Male', text: 'Male'},
        {value: 'Female', text: 'Female'}
    ];

    $scope.groups =  [
        {value: 'Guest', text: 'Guest'},
        {value: 'Service', text: 'Service'},
        {value: 'Customer', text: 'Customer'},
        {value: 'Operator', text: 'Operator'},
        {value: 'Support', text: 'Support'},
        {value: 'Admin', text: 'Admin'}
    ]; 

});
"use strict";


angular.module('app.forms').controller('ImageEditorCtrl', function ($scope) {

    // api tab
    $scope.apiDemoSelection = [100, 100, 400, 300];

    $scope.apiDemoOptions = {
        allowSelect: true,
        allowResize: true,
        allowMove: true,
        animate: false
    };

    $scope.apiRandomSelection = function () {
        $scope.apiDemoOptions.animate = false;
        $scope.apiDemoSelection = [
            Math.round(Math.random() * 600),
            Math.round(Math.random() * 400),
            Math.round(Math.random() * 600),
            Math.round(Math.random() * 400)
        ]
    };

    $scope.apiRandomAnimation = function () {
        $scope.apiDemoOptions.animate = true;
        $scope.apiDemoSelection = [
            Math.round(Math.random() * 600),
            Math.round(Math.random() * 400),
            Math.round(Math.random() * 600),
            Math.round(Math.random() * 400)
        ]
    };

    $scope.apiReleaseSelection = function () {
        $scope.apiDemoOptions.animate = true;
        $scope.apiDemoSelection = 'release';
    };


    $scope.apiToggleDisable = function () {
        $scope.apiDemoOptions.disabled = !$scope.apiDemoOptions.disabled;
    };

    $scope.apiToggleDestroy = function () {
        $scope.apiDemoOptions.destroyed = !$scope.apiDemoOptions.destroyed;
    };

    $scope.apiDemoShowAspect = false;
    $scope.apiDemoToggleAspect = function () {
        $scope.apiDemoShowAspect = !$scope.apiDemoShowAspect;
        if ($scope.apiDemoShowAspect)
            $scope.apiDemoOptions.aspectRatio = 4 / 3;
        else
            $scope.apiDemoOptions.aspectRatio = 0;
    };

    $scope.apiDemoShowSizeRestrict = false;
    $scope.apiDemoToggleSizeRestrict = function () {
        $scope.apiDemoShowSizeRestrict = !$scope.apiDemoShowSizeRestrict;
        if ($scope.apiDemoShowSizeRestrict) {
            $scope.apiDemoOptions.minSizeWidth = 80;
            $scope.apiDemoOptions.minSizeHeight = 80;
            $scope.apiDemoOptions.maxSizeWidth = 350;
            $scope.apiDemoOptions.maxSizeHeight = 350;
        } else {
            $scope.apiDemoOptions.minSizeWidth = 0;
            $scope.apiDemoOptions.minSizeHeight = 0;
            $scope.apiDemoOptions.maxSizeWidth = 0;
            $scope.apiDemoOptions.maxSizeHeight = 0;
        }

    };


    $scope.setApiDemoImage = function (image) {
        $scope.apiDemoImage = image;
        $scope.apiDemoOptions.src = image.src;
        $scope.apiDemoOptions.bgOpacity = image.bgOpacity;
        $scope.apiDemoOptions.outerImage = image.outerImage;
        $scope.apiRandomAnimation();
    };

    $scope.apiDemoImages = [
        {
            name: 'Lego',
            src: 'styles/img/superbox/superbox-full-24.jpg',
            bgOpacity: .6
        },
        {
            name: 'Breakdance',
            src: 'styles/img/superbox/superbox-full-7.jpg',
            bgOpacity: .6
        },
        {
            name: 'Dragon Fly',
            src: 'styles/img/superbox/superbox-full-20.jpg',
            bgOpacity: 1,
            outerImage: 'styles/img/superbox/superbox-full-20-bw.jpg'
        }
    ];

    $scope.apiDemoImage = $scope.apiDemoImages[1];

    // animations tab
    $scope.animationsDemoOptions = {
        bgOpacity: undefined,
        bgColor: undefined,
        bgFade: true,
        shade: false,
        animate: true
    };
    $scope.animationsDemoSelection = undefined;
    $scope.selections = {
        1: [217, 122, 382, 284],
        2: [20, 20, 580, 380],
        3: [24, 24, 176, 376],
        4: [347, 165, 550, 355],
        5: [136, 55, 472, 183],
        Release: 'release'
    };

    $scope.opacities = {
        Low: .2,
        Mid: .5,
        High: .8,
        Full: 1
    };

    $scope.colors = {
        R: '#900',
        B: '#4BB6F0',
        Y: '#F0B207',
        G: '#46B81C',
        W: 'white',
        K: 'black'
    };


    // styling tab

    $scope.styles = [
        {
            name: 'jcrop-light',
            bgFade: true,
            animate: true,
            selection: [130, 65, 130 + 350, 65 + 285],
            bgColor: 'white',
            bgOpacity: 0.5
        },
        {
            name: 'jcrop-dark',
            bgFade: true,
            animate: true,
            selection: [130, 65, 130 + 350, 65 + 285],
            bgColor: 'black',
            bgOpacity: 0.4
        },
        {
            name: 'jcrop-normal',
            bgFade: true,
            animate: true,
            selection: [130, 65, 130 + 350, 65 + 285],
            bgColor: 'black',
            bgOpacity: 0.6
        }
    ];

    $scope.demoStyle = $scope.styles[0]
});
'use strict'

angular.module('app.forms').controller('ModalDemoCtrl', function($scope, $modalInstance){
    $scope.closeModal = function(){
        $modalInstance.dismiss('cancel');
    }
});
"use strict";	

angular.module('app.healthcheck').controller("HealthCheckCtrl", function ($http, APP_CONFIG, $scope) {
	var url = APP_CONFIG.ebaasRootUrl + "/api/health";

    $scope.loading = true;
    $http.get(url)
        .success(function (data) {
            $scope.results = data;
            $scope.loading = false;
        })
        .error(function (err) {
            $scope.results = null;
            $scope.loading = false;
        });
});
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


'use strict';

angular.module('app.fulltextsearch').factory('searchContext', function () {

    var SearchContextModel = {
            searchText: undefined
        };

    return SearchContextModel;
});

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
"use strict";

angular.module('app.homepage').directive('myActivitiesDropdownToggle', function ($log) {

	var link = function($scope,$element, attrs){
		var ajax_dropdown = null;

		$element.on('click',function(){
			var badge = $(this).find('.badge');

			if (badge.hasClass('bg-color-red')) {

				badge.removeClass('bg-color-red').text(0);

			}

			ajax_dropdown = $(this).next('.ajax-dropdown');

			if (!ajax_dropdown.is(':visible')) {

				ajax_dropdown.fadeIn(150);

				$(this).addClass('active');

			}
			 else {
				
				ajax_dropdown.fadeOut(150);
				
				$(this).removeClass('active');

			}

		})

		$(document).mouseup(function(e) {
			if (ajax_dropdown && !ajax_dropdown.is(e.target) && ajax_dropdown.has(e.target).length === 0) {
				ajax_dropdown.fadeOut(150);
				$element.removeClass('active');
			}
		});
	}
	
	return{
		restrict:'EA',
		link:link
	}
});
"use strict";

angular.module('app.homepage').factory('myActivityService', function ($http, $rootScope, $log, APP_CONFIG) {

    var MessageModel = {
        count: 0,
        items : []
    };

    function getActivities(callback) {

        $http.get(APP_CONFIG.apiRootUrl + '/activities/activity.json').success(function (data) {

            callback(data);

        }).error(function () {

            $log.log('Error');
            callback([]);

        });
    }

	function getTasks(callback) {

	    var url = APP_CONFIG.ebaasRootUrl + "/api/tasks/" + encodeURIComponent(APP_CONFIG.dbschema);
	    $http.get(url).success(function (data) {

	        callback(data);

	    }).error(function () {

	        $log.log('Error');
	        callback([]);
	    });
	}

	function getActivitiesByType(type, callback){

	    if (type === "msgs") {
	        $http.get(APP_CONFIG.ebaasRootUrl + '/api/messages').success(function (data) {

	            MessageModel.count = data.length;
	            callback(data);
	        }).error(function () {

	            $log.log('Error');
	            callback([]);
	        });
	    }
	    else
	    {
	        callback([]);
	    }
	}

	function createActivitiesByType(type, msg, callback) {

	    if (type === "msgs") {
	        var groupName = msg.dbschema + "-" + msg.dbclass + "-" + msg.oid;
	        $http.post(APP_CONFIG.ebaasRootUrl + '/api/messages/' + encodeURIComponent(groupName), msg).success(function (data) {
	            callback(data);
	        }).error(function () {
	            $log.log('Error');
	            callback([]);
	        });
	    }
	    else {
	        callback([]);
	    }
	}

	function addActivityByType(type, activity)
	{
	    if (type === "msgs") {
	        MessageModel.count++;
	        $rootScope.$apply(); // this is outside of angularjs, so need to digest
	        $.smallBox({
	            title: $rootScope.getWord('NewMessage'),
	            content: "<i class='fa fa-info'></i> <i>" + activity.Subject + "</i>",
	            color: "#5F895F",
	            iconSmall: "fa fa-check bounce animated",
	            timeout: 8000
	        });
	    }
	    else if (type === "tasks") {
	        MessageModel.count++;
	        $rootScope.$apply(); // this is outside of angularjs, so need to digest
	        $.smallBox({
	            title: $rootScope.getWord('NewTask'),
	            content: "<i class='fa fa-info'></i> <i>" + activity.Subject + "</i>",
	            color: "#5F895F",
	            iconSmall: "fa fa-check bounce animated",
	            timeout: 8000
	        });
	    }
	}

	function removeActivitiesByType(type, oid, callback) {

	    if (type === "msgs") {
	        $http.delete(APP_CONFIG.ebaasRootUrl + '/api/messages/' + oid).success(function (data) {

	            if (MessageModel.count > 0) {
	                MessageModel.count--;
	            }

	            callback(data);

	        }).error(function () {

	            $log.log('Error');
	            callback([]);

	        });
	    }
	    else {
	        callback([]);
	    }
	}

	function removeAllActivitiesByType(type, callback) {

	    if (type === "msgs") {
	        $http.delete(APP_CONFIG.ebaasRootUrl + '/api/messages').success(function () {

	            MessageModel.count = 0;

	            callback();

	        }).error(function () {

	            $log.log('Error');
	            callback();

	        });
	    }
	    else {
	        callback();
	    }
	}
	
	return {
	    MessageModel: function()
	    {
	        return MessageModel;
	    },
	    get: function (callback) {
	        getActivities(callback);
	    },
		getTasks:function(callback) {
		    getTasks(callback);
		},
		getbytype:function(type,callback){
			getActivitiesByType(type, callback);
		},
		create: function(type, activity, callback) {
		    createActivitiesByType(type, activity, callback);
		},
		add: function(type, activity)
		{
		    addActivityByType(type, activity);
		},
		remove: function(type, oid, callback) {
		    removeActivitiesByType(type, oid, callback);
		},
		removeAll: function(type, callback) {
		    removeAllActivitiesByType(type, callback);
		}
	}
});
"use strict";


angular.module('app.homepage').controller('mainChartsCtrl', function ($scope, $http, APP_CONFIG, promisedParams) {

    $scope.pageparams = promisedParams.data;
});
"use strict";


angular.module('app.homepage').controller('mainMenuCtrl', function ($scope, $rootScope, $state, promisedMenuItems) {

    $scope.dbschema = undefined;
    $scope.dbclass = undefined;

    var foundFullTextSearch = false;
    var menuItems = new Array();
    _.forEach(promisedMenuItems.data.items, function (item) {
        if (item.visible) {
            if (item.sref) {
                if (!item.desc)
                    item.desc = "col-xs-4 col-sm-3 col-md-2 page-darkblue";

                if (!item.icon)
                    item.icon = "fa fa-cog";

                // do not show home menu item on the home page
                if (item.name.toUpperCase() != "HOME") {
                    menuItems.push(item);
                }
                else {
                    $scope.dbschema = item.schema;
                    $scope.dbclass = item.class;
                }
            }
            else if (item.items) {
                _.forEach(item.items, function (child) {
                    if (!child.desc)
                        child.desc = "col-xs-4 col-sm-3 col-md-2 page-gray";

                    if (!child.icon)
                        child.icon = "fa fa-cog";

                    menuItems.push(child);
                })
            }
        }

        // find the first level menu item with name FULLTEXTSEARCH
        if (item.name.toUpperCase() === "FULLTEXTSEARCH") {
            var parameters = ["ID", "title", "itemClass", "packetClass",
                "taskNodeAttribute", "itemNodeAttribute", "packetNodeAttribute",
                "taskTemplate", "itemTemplate", "packetTemplate"];

            $rootScope.fullText = new Object();
            $rootScope.fullText.schema = item.schema;
            $rootScope.fullText.class = item.class;
            $rootScope.fullText.sref = item.sref;
            if (item.parameters) {
                for (var i = 0; i < parameters.length; i++) {
                    var parameter = parameters[i];
                    if (item.parameters.hasOwnProperty(parameter)) {
                        $rootScope.fullText[parameter] = item.parameters[parameter];
                    }
                    else {
                        console.error("The FullTextSearh menu item is missing " + parameter + " parameter");
                    }
                }              
            }
            else {
                console.error("The FullTextSearh menu item is missing parameters");
            };

            foundFullTextSearch = true;
        }
    });

    if (!foundFullTextSearch) {
        console.error("Unable to find a first level menu item with name FullTextSearch");
    }

    $scope.menuItems = menuItems;

    $scope.options = {
        visible: 5,
        perspective: 35,
        startSlide: 0,
        border: 3,
        dir: 'ltr',
        clicking: true,
        width: 360,
        height: 270,
        space: 220,
        autoRotationSpeed: 10000,
        loop: true
    };

    $scope.selectedClick = selectedClick;
    $scope.slideChanged = slideChanged;
    $scope.beforeChange = beforeChange;
    $scope.lastSlide = lastSlide;


    function lastSlide(index) {
        //$log.log('Last Slide Selected callback triggered. \n == Slide index is: ' + index + ' ==');
    }

    function beforeChange(index) {
        //$log.log('Before Slide Change callback triggered. \n == Slide index is: ' + index + ' ==');
    }

    function selectedClick(index) {
        if (index >= 0) {
            var post = $scope.slides[index];
            $state.go('app.bulletinboard.view', {schema: $scope.dbschema, class: $scope.dbclass, oid: post.obj_id });
        }
    }

    function slideChanged(index) {
        //$log.log('Slide Changed callback triggered. \n == Slide index is: ' + index + ' ==');
    }
});
"use strict";	

angular.module('app.homepage').controller("myTasksController", function ActivitiesCtrl($scope, APP_CONFIG, myActivityService) {

	// Getting different type of activites
	myActivityService.getTasks(function(data){
		$scope.tasks = data;
	});

	$scope.getDBSchema = function () {
	    return APP_CONFIG.dbschema;
	}
});
"use strict";

angular.module("app.hub").factory("hubService", function($http, $q, localStorageService, APP_CONFIG, User) {

    var hubServiceFactory = {};

    var _connect = function(schema, callback) {
        // establish signalr connection
        var hub = $.connection.messageHub; // create a proxy to signalr hub on web server

        // Create a function that the hub can call to broadcast messages.
        hub.client.addMessage = function (type, message) {
            if (callback)
            {
                callback(type, message);
            }
        };

        $.connection.hub.stop();

        $.connection.hub.qs = { 'user': User.userName, 'schema': schema }; // user name as part of query string of signalr connection
 
        $.connection.hub.start(); // connect to signalr hub

        $.connection.hub.error(function (error) {
            console.log('SignalR error: ' + error)
        });
    };

    var _dicconnect = function () {
        $.connection.hub.stop();
    };

    var _addToGroup = function (group) {

        var hub = $.connection.messageHub; // create a proxy to signalr hub on web server

        hub.server.addToGroup(group);
    };

    var _removeFromGroup = function (group, callback) {

        var hub = $.connection.messageHub; // create a proxy to signalr hub on web server

        hub.server.removeFromGroup(group).done(function () {
            if (callback)
            {
                callback();
            }
        });
    };

    var _getUserGroups = function (callback) {

        var hub = $.connection.messageHub; // create a proxy to signalr hub on web server

        hub.server.getUserGroups().done(function (groups) {
            callback(groups);
        });
    };

    var _isUserInGroup = function (group, callback) {

        var hub = $.connection.messageHub; // create a proxy to signalr hub on web server

        hub.server.isUserInGroup(group).done(function (status) {
            callback(status);
        });
    };

    hubServiceFactory.connect = _connect;
    hubServiceFactory.disconnect = _dicconnect;
    hubServiceFactory.addToGroup = _addToGroup;
    hubServiceFactory.removeFromGroup = _removeFromGroup;
    hubServiceFactory.getUserGroups = _getUserGroups;
    hubServiceFactory.isUserInGroup = _isUserInGroup;

    return hubServiceFactory;
});
"use strict";

angular.module('app.layout').controller('helpViewerCtlr', function ($rootScope, $scope, $stateParams, $http, APP_CONFIG) {

    $scope.hash = undefined;
    $scope.helpDoc = undefined;
    if ($stateParams.hash) {
        $scope.hash = $stateParams.hash;
    }

    if ($scope.hash) {
        if (!$scope.helpDoc) {
            $http.get(APP_CONFIG.ebaasRootUrl + "/api/sitemap/help/" + $scope.hash)
             .success(function (helpDoc) {
                 $scope.helpDoc = helpDoc;

                 $scope.pdfUrl = "helps/" + $scope.helpDoc;
             });
        }
        else
        {

            $scope.pdfUrl = "helps/" + $scope.helpDoc;
        }
    }
});

"use strict";

angular.module('app.layout').controller('layoutCtrl', function ($rootScope, $scope) {
    $scope.IntroOptions = {
        steps: [
            {
                element: '#logo',
                intro: $rootScope.getWord("ReturnToHomeIntro"),
                position: 'bottom'
            },
            {
                element: '#activity',
                intro: $rootScope.getWord("ViewActivitiesIntro"),
                position: 'bottom'
            },
            {
                element: '#fullscreen',
                intro: $rootScope.getWord("FullScreenIntro"),
                position: 'bottom'
            },
            {
                element: '#logout',
                intro: $rootScope.getWord("LogoutIntro"),
                position: 'bottom'
            },
            {
                element: '#hide-menu',
                intro: $rootScope.getWord("HideMenuIntro"),
                position: 'bottom'
            },
            {
                element: '#my-login-info',
                intro: $rootScope.getWord("MyLoginInfoIntro"),
                position: 'bottom'
            },
            {
                element: '#reset-settings',
                intro: $rootScope.getWord("ResetSettingsIntro"),
                position: 'bottom'
            },
            {
                element: '#demo-setting',
                intro: $rootScope.getWord("DemoSettingIntro"),
                position: 'left'
            },
            {
                element: '#sidemenu',
                intro: $rootScope.getWord("SidemenuIntro"),
                position: 'right'
            },
            {
                element: '#minimize-sidemenu',
                intro: $rootScope.getWord("MinimizeSidemenuIntro"),
                position: 'right'
            },
            {
                element: '#return-home',
                intro: $rootScope.getWord("ReturnToHomeIntro"),
                position: 'bottom'
            },
            {
                element: '#content',
                intro: $rootScope.getWord("ContentIntro"),
                position: 'center'
            }
        ],
        nextLabel: $rootScope.getWord("NextStep"),
        prevLabel: $rootScope.getWord("PreviousStep"),
        skipLabel: $rootScope.getWord("IntroSkip"),
        doneLabel: $rootScope.getWord("IntroComplete"),
    }
});

"use strict";

angular.module('app').controller("LanguagesCtrl",  function LanguagesCtrl($scope, $rootScope, $log, Language){




    $scope.selectLanguage = function(language){
        $rootScope.currentLanguage = language;
        
        Language.getLang(language.key,function(data){

            $rootScope.lang = data;
            
        });
    }

 

});
"use strict";

angular.module('app').factory('Language', function($http, APP_CONFIG){

	function getLanguage(key, callback) {

		$http.get(APP_CONFIG.apiRootUrl + '/langs/' + key + '.json').success(function(data){

			callback(data);
			
		}).error(function(){

			$log.log('Error');
			callback([]);

		});

	}

	function getLanguages(callback) {

		$http.get(APP_CONFIG.apiRootUrl + '/languages.json').success(function(data){

			callback(data);
			
		}).error(function(){

			$log.log('Error');
			callback([]);

		});

	}

	return {
		getLang: function(type, callback) {
			getLanguage(type, callback);
		},
		getLanguages:function(callback){
			getLanguages(callback);
		}
	}

});
"use strict";

angular.module('app').directive('languageSelector', function(Language){
    return {
        restrict: "EA",
        replace: true,
        templateUrl: "app/layout/language/language-selector.tpl.html",
        scope: true
    }
});
"use strict";

angular.module('app').directive('toggleShortcut', function($log,$timeout) {

	var initDomEvents = function($element){

		var shortcut_dropdown = $('#shortcut');

		$element.on('click',function(){
		
			if (shortcut_dropdown.is(":visible")) {
				shortcut_buttons_hide();
			} else {
				shortcut_buttons_show();
			}

		})

		shortcut_dropdown.find('a').click(function(e) {
			e.preventDefault();
			window.location = $(this).attr('href');
			setTimeout(shortcut_buttons_hide, 300);
		});

		

		// SHORTCUT buttons goes away if mouse is clicked outside of the area
		$(document).mouseup(function(e) {
			if (shortcut_dropdown && !shortcut_dropdown.is(e.target) && shortcut_dropdown.has(e.target).length === 0) {
				shortcut_buttons_hide();
			}
		});

		// SHORTCUT ANIMATE HIDE
		function shortcut_buttons_hide() {
			shortcut_dropdown.animate({
				height : "hide"
			}, 300, "easeOutCirc");
			$('body').removeClass('shortcut-on');

		}

		// SHORTCUT ANIMATE SHOW
		function shortcut_buttons_show() {
			shortcut_dropdown.animate({
				height : "show"
			}, 200, "easeOutCirc");
			$('body').addClass('shortcut-on');
		}
	}

	var link = function($scope,$element){
		$timeout(function(){
			initDomEvents($element);
		});
	}

	return{
		restrict:'EA',
		link:link
	}
})
'use strict';

angular.module('app.logs').controller('changeLogCtrl', function ($scope, $rootScope, APP_CONFIG, logManager, $stateParams) {

    var vm = this;
    vm.title = 'Log Viewer';

    vm.getWord = getWord;

    logManager.params.dbschema = $stateParams.logschema;
    logManager.params.dbclass = $stateParams.logclass;
    logManager.params.oid = $stateParams.logoid;
    logManager.params.property = $stateParams.logproperty;

    activate();

    function activate() {
        logManager.load(function (logs) {
            console.log(logs);
            vm.logs = logs;
        });
    }

    function getWord(key) {
        return $rootScope.getWord(key);
    }
});

'use strict';

angular.module('app.logs').controller('changeLogViewerCtrl', function ($scope, $rootScope, APP_CONFIG, $stateParams, $modalInstance) {

    $scope.dbschema = $stateParams.logschema;
    $scope.dbclass = $stateParams.logclass;
    $scope.oid = $stateParams.logoid;
    $scope.property = $stateParams.logproperty;


    $scope.closeModal = function () {
        $modalInstance.dismiss("dismiss");
    };
});

'use strict';

angular.module('app.logs').directive('changelog', function () {
    return {
        restrict: 'E',
        templateUrl: 'app/logs/views/change-log.html',
        replace: true,
        scope: {},
        controllerAs: 'vm',
        controller: 'changeLogCtrl',
        link: function (scope, element, attributes) {
        }
    }
});
'use strict';

angular.module('app.logs').factory('logManager', function ($q, $http, APP_CONFIG) {

    var service = {
        logs: [],
        load: load,
        status: {
            uploading: false
        },
        params: {
            dbschema: "",
            dbclass: "",
            oid: "",
            property: ""
        }
    };

    return service;

    function load(callback) {
        service.logs.length = 0;

        var url = APP_CONFIG.ebaasRootUrl + "/api/log/" + encodeURIComponent(service.params.dbschema) + "/" + service.params.dbclass + "/" + service.params.oid + "/" + service.params.property;

        $http.get(url).success(function (data) {

            callback(convertLogs(data));

        }).error(function () {

        });
    }

    function convertLogs(logRecordCollection) {

        var logs = [];

        if (logRecordCollection) {

            for (var i = 0; i < logRecordCollection.length; i++) {
                var logRecord = logRecordCollection[i];

                var log = {};

                switch (logRecord.actionType)
                {
                    case 1:
                        log.type = "Create";
                        break;

                    case 2:
                        log.type = "Modify";
                        break
                }
                
                log.user = logRecord.userDisplayText;
                log.time = logRecord.actionTime;
                log.content = logRecord.actionData;

                logs.push(log);
            }
        }

        return logs;
    }
});
'use strict';

angular.module('app.mldashboard').controller('MLDashboardLayoutCtrl', function ($http, APP_CONFIG, $scope, $state, $stateParams, propmisedParams) {

    var params = propmisedParams.data;

    // url to get tree model
    var url = APP_CONFIG.ebaasRootUrl + "/api/dnn/projects";

    $http.get(url).then(function (res) {

        var projectData = res.data;

        if (projectData) {

            $scope.modelTree = createProjectTree(projectData);
        }
    });

    $scope.OpenModelDashboard = function(project, model) {
        $state.go('app.mldashboard.modeldashboard', { project: project, model: model});
    }

    var addModelItems = function (parentItem, project, models)
    {
        var model, modelItem;

        if (models) {
            for (var i = 0; i < models.length; i += 1) {
                model = models[i];
                modelItem = {};
                modelItem.title = model;
                modelItem.content = "<span class='label label-info'><a class=\"station-a\" href=\"javascript:angular.element(document.getElementById('modelTree')).scope().OpenModelDashboard('" + project + "', '" + model + "');\">" + model + "</a></span>";

                parentItem.children.push(modelItem);
            }
        }
    }

    function createProjectTree(projects) {
        var roots = [];

        // add models to the project
        asyncLoop({
            length: projects.length,
            functionToLoop: function (loop, i) {
                $http.get(APP_CONFIG.ebaasRootUrl + "/api/dnn/models/" + projects[i])
                     .success(function (data) {

                         var project = projects[i];
                         var projectItem = {};
                         projectItem.title = project;
                         projectItem.content = "<span><i class=\"fa fa-lg fa-plus-circle\"></i> " + project + "</span>";
                         projectItem.children = [];
                         projectItem.expanded = true;
                         roots.push(projectItem);

                         addModelItems(projectItem, project, data);

                         loop();
                     });
            },
            callback: function () {
            }
        });

        return roots;
    }

    var asyncLoop = function (o) {
        var i = -1;

        var loop = function () {
            i++;
            if (i == o.length) {
                o.callback();
                return;
            }

            o.functionToLoop(loop, i);
        }

        loop(); // init
    }
})
'use strict';

angular.module('app.mldashboard').controller('MLModelDashboardCtrl', function ($controller, $rootScope, $scope, $http, APP_CONFIG, $state, $stateParams) {

    $scope.project = $stateParams.project;
    $scope.model = $stateParams.model;

    $scope.inputFields = [];
    $scope.outputFields = [];


    $scope.loading = false;
   
    // url to get model info
    var url = APP_CONFIG.ebaasRootUrl + "/api/dnn/model/" + $scope.project + "/" + $scope.model;

    $http.get(url).then(function (res) {
        $scope.modelInfo = res.data;

        var inputField, outputField;

        for (var i = 0; i < $scope.modelInfo.inputVariables.length; i += 1)
        {
            var inputVar = $scope.modelInfo.inputVariables[i];

            for (var j = 0; j < inputVar.dimension; j += 1)
            {
                inputField = {};
                inputField.Name = inputVar.name;
                inputField.Label = inputVar.name + "-" + j;
                inputField.DataType = inputVar.dataType;
                inputField.Value = "";

                $scope.inputFields.push(inputField);
            }
        }

        for (var i = 0; i < $scope.modelInfo.outputVariables.length; i += 1) {
            var outputVar = $scope.modelInfo.outputVariables[i];

            outputField = {};
            outputField.Label = outputVar.name;
            outputField.Name = outputVar.name;
            outputField.DataType = outputVar.dataType;
            outputField.Value = "";

            $scope.outputFields.push(outputField);
        }
    });

    $scope.submitModelForm = function()
    {
        $scope.loading = true;
        var features = GetFeatures();
        var labels = GetLabels();
  
        // url to get model info
        var url = APP_CONFIG.ebaasRootUrl + "/api/dnn/classify/" + $scope.project + "/" + $scope.model + "/" + features + "/" + labels;

        $http.get(url).then(function (res) {
            var outputs = res.data;

            for (var i = 0; i < $scope.outputFields.length; i += 1) {
                var outputField = $scope.outputFields[i];
                if (outputs[outputField.Name])
                {
                    outputField.Value = outputs[outputField.Name];
                }
            }

            $scope.loading = false;
        })
    }


    function GetFeatures()
    {
        var features = "";
        for (var i = 0; i < $scope.inputFields.length; i += 1) {
            var inputField = $scope.inputFields[i];
            if (inputField.Name == "features") {
                if (features)
                {
                    features += ";";
                }

                features += inputField.Value;
            }
        }

        return features;
    }

    function GetLabels() {
        var labels = "";
        for (var i = 0; i < $scope.inputFields.length; i += 1) {
            var inputField = $scope.inputFields[i];
            if (inputField.Name == "labels") {
                if (labels) {
                    labels += ";";
                }

                labels += inputField.Value;
            }
        }

        return labels;
    }
});
'use strict';

angular.module('app.newtask').controller('CreateTaskCtrl', function ($controller, $http, $scope, APP_CONFIG, $state, $rootScope, $stateParams, propmisedParams) {

    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.class;

    var params = propmisedParams.data;
    $scope.itemClass = params['itemClass'];
    $scope.packetClass = params['packetClass'];
    $scope.taskNodeAttribute = params['taskNodeAttribute'];
    $scope.itemNodeAttribute = params['itemNodeAttribute'];
    $scope.packetNodeAttribute = params['packetNodeAttribute'];
    $scope.taskTemplate = params['taskTemplate'];
    $scope.itemTemplate = params['itemTemplate'];
    $scope.packetTemplate = params['packetTemplate'];

    $scope.template = params['taskTemplate'];

    $scope.formId = "CreateNewTaskForm"; // this will be posted as headers to the server to identify the form(optional)

    $scope.submitFormCallback = function (data) {
        if (data) {
            var params = new Object();
            params.schema = $scope.dbschema;
            params.class = $scope.dbclass;
            params.oid = data.instance.obj_id;
            params.itemClass = $scope.itemClass;
            params.packetClass = $scope.packetClass;
            params.taskNodeAttribute = $scope.taskNodeAttribute;
            params.itemNodeAttribute = $scope.itemNodeAttribute;
            params.packetNodeAttribute = $scope.packetNodeAttribute;
            params.taskTemplate = $scope.taskTemplate;
            params.itemTemplate = $scope.itemTemplate;
            params.packetTemplate = $scope.packetTemplate;
            params.activeTabId = "tasktab";
            $state.go("app.taskviewer.details", params, { reload: true });
        }
    };

    $scope.onTaskSubmit = function () {
        $scope.submitForm($scope.submitFormCallback);
    };

    angular.extend(this, $controller('ebaasFormBaseCtrl', { $rootScope: $rootScope, $scope: $scope, $http: $http, APP_CONFIG: APP_CONFIG, $stateParams: $stateParams }));

});
'use strict';

angular.module('app.smartforms').controller('ebaasFormBaseCtrl', function ($rootScope, $scope, $http, APP_CONFIG, $state, $stateParams, MetaDataCache) {
    
    $scope.submitted = false;
    $scope.message = "";
    $scope.loading = false;
    $scope.hasError = false;

    if ($scope.dbclass) {
        var metadata = MetaDataCache.getClassView($scope.dbschema, $scope.dbclass, "full");
        if (metadata) {
            // get from cache
            $scope.caption = metadata.title;
        }
        else {
            $http.get(APP_CONFIG.ebaasRootUrl + "/api/metadata/view/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "?view=full")
                    .success(function (data) {
                        $scope.caption = data.title;
                        MetaDataCache.setClassView($scope.dbschema, $scope.dbclass, "full", data)
                    });
        }
    }

    loadFormData();

    var asyncLoop = function (o) {
        var i = -1;

        var loop = function () {
            i++;
            if (i == o.length) {
                o.callback();
                return;
            }

            o.functionToLoop(loop, i);
        }

        loop(); // init
    }

    $scope.updateListOptions = function (propertyNames, filterValue) {
        // getting options from api one by one in asunc mode
        asyncLoop({
            length: propertyNames.length,
            functionToLoop: function (loop, i) {
                var url = APP_CONFIG.ebaasRootUrl + "/api/form/listoptions/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + propertyNames[i] + "/" + encodeURIComponent(filterValue);
                $http.get(url).success(function (data) {

                    $scope.model[propertyNames[i]].options = data; // set the new options

                    loop();
                });
            },
            callback: function () {

            }
        })
    }

    $scope.getSuggestionsForFormField = function (propertyName, typedText) {
        if (typedText && typedText.length > 1) {
            // get filtered list of values
            var url = APP_CONFIG.ebaasRootUrl + "/api/form/suggestions/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + propertyName + "/" + encodeURIComponent(typedText);
            var promise = $http.get(url).then(function (response) {
                return response.data;
            });

            return promise;
        }
        else {
            return [];
        }
    }

    $scope.onSuggestionSelectForFormField = function ($item, $model, $label, textField) {
        $scope.model[textField] = $label;
        $scope.reloadInstance(textField);
    }

    $scope.reloadInstance = function(propertyName)
    {
        // something important changed in the instance, reload it from db
        var url;
        if ($scope.formAttribute) {
            if ($scope.oid)
                // get data for a form template which is an attribute value of the instance
                url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + $scope.oid + "/" + propertyName + "/refresh" + "?formAttribute=" + $scope.formAttribute;
            else
                url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + propertyName + "/refresh" + "?formAttribute=" + $scope.formAttribute;
        }
        else if ($scope.template) {
            if ($scope.oid)
                // get data for a template-based custom form
                url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + $scope.oid + "/" + propertyName + "/refresh" + "?template=" + encodeURIComponent($scope.template); // trailing slash to allow template name contans dot
            else
                url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + propertyName + "/refresh" + "?template=" + encodeURIComponent($scope.template); // trailing slash to allow template name contans dot
        }
        else {
            if ($scope.oid)
                // get a detailed instance
                url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + $scope.oid + "/" + propertyName + "/refresh" + "?formformat=true";
            else
                url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + "/" + propertyName + "/refresh" + "?formformat=true";
        }

        $http.post(url, $scope.model)
          .success(function (data) {
              $scope.model = data;
          });
    }

    $scope.getDuplicateDataUrl = function (oid, deepclone)
    {
        var url = undefined;
        if (oid) {
            if ($scope.formAttribute) {
                // get data for a form template which is an attribute value of the instance
                url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + oid + "/clone" + "?formAttribute=" + $scope.formAttribute + "&deep=" + deepclone;
            }
            else if ($scope.template) {
                // get data for a template-based custom form
                url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + oid + "/clone" + "?template=" + encodeURIComponent($scope.template) + "&deep=" + deepclone;
            }
            else {
                // get a detailed instance
                url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + oid + "/clone" + "?formformat=true" + "&deep=" + deepclone;
            }
        }

        return url;
    }

    $scope.submitForm = function(callbackMethod)
    {
        // validate the form data using server-side rules
        serverSideValidate(function(isValid, msg)
        {
           submitFormData(callbackMethod, isValid, msg);
        })
    }

    $scope.onSubmit = function () {
        // validate the form data using server-side rules
        serverSideValidate(function (isValid, msg) {
           submitFormData(null, isValid, msg);
        })
    };

    $scope.reloadForm = function()
    {
        loadFormData();
    }

    function loadFormData()
    {
        var url;

        if ($scope.oid) {
            if ($scope.formAttribute) {
                // get data for a form template which is an attribute value of the instance
                url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + $scope.oid + "?formAttribute=" + $scope.formAttribute;
            }
            else if ($scope.template) {
                // get data for a template-based custom form
                url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + $scope.oid + "?template=" + encodeURIComponent($scope.template); // trailing slash to allow template name contans dot
            }
            else {
                // get a detailed instance
                url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + $scope.oid + "?formformat=true";
            }

            if (url) {
                $http.get(url)
                    .success(function (data) {
                        $scope.model = data;
                    });
            }
        }
        else {
            if ($scope.cloneid && $scope.cloneclass) {
                // get a cloned instance as initial values for the new instance, clone id and clone class are set by sub controller
                url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.cloneclass + "/" + $scope.cloneid + "?formformat=true";
            }
            else if ($scope.template) {
                // get data for a template-based custom form
                url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/new?template=" + encodeURIComponent($scope.template); // tralling slash allowing template name conatain dot
            }
            else {
                // get a default form
                url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/new";
            }

            if (url) {
                $http.get(url)
                    .success(function (data) {
                        $scope.model = data;
                    });
            }
        }
    }

    var serverSideValidate = function(callbackFunc)
    {
        var url = APP_CONFIG.ebaasRootUrl + "/api/form/validate/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass;

        $http.post(url, $scope.model)
            .success(function (msg) {
                if (msg) {
                    callbackFunc(false, msg);
                }
                else {
                    callbackFunc(true, "");
                }
            });
    }

    var submitFormData = function (callbackMethod, isValid, errMsg)
    {
        var url;

        $scope.submitted = true;

        //if ($scope.ebaasform.$valid && $scope.ebaasform.$dirty) {
        if (isValid) {
            if ($scope.ebaasform.$valid) { // we don't check dirty since nested form may have row deleted which doesn't flag as dirty
                if ($scope.formAttribute && $scope.oid) {
                    // get data for a form template which is an attribute value of the instance
                    url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + $scope.oid + "?formAttribute=" + $scope.formAttribute;
                }
                else if ($scope.template) {
                    if ($scope.oid) {
                        // update data for a template-based custom form
                        url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + $scope.oid + "?template=" + encodeURIComponent($scope.template);
                    }
                    else {
                        // add a data for a template-based custom form
                        url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "?template=" + encodeURIComponent($scope.template);
                    }
                }
                else {
                    if ($scope.oid) {
                        // update data for a default form
                        url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + $scope.oid;
                    }
                    else {
                        // add data for a default form
                        url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass;
                    }
                }

                $scope.loading = true;
                $http.post(url, $scope.model, {
                    headers: {
                        'formId': $scope.formId,
                        'taskId': $scope.taskId,
                        'actionId': $scope.actionId
                    }
                })
                .success(function (data) {
                    $scope.message = $rootScope.getWord('Submitted');
                    $scope.loading = false;
                    $scope.hasError = false;
                    if (!$scope.oid && data.obj_id)
                    {
                        // get oid from the newly created instance
                        $scope.oid = data.obj_id;

                        // broadcast the obj_id of the new instance, since the attachment directive need to become editable
                        $rootScope.$broadcast('instanceCreated', { oid: $scope.oid });
                    }

                    if (callbackMethod) {
                        callbackMethod({ 'instance': data });
                    }
                })
                .error(function (err) {
                    $scope.message = err.message;
                    $scope.hasError = true;
                    $scope.loading = false;
                });
            }
            else {
                $scope.message = $rootScope.getWord('ValidationError');
            }
        }
        else
        {
            $scope.hasError = true;
            $scope.message = errMsg;
            /*
            BootstrapDialog.show({
                title: $rootScope.getWord("Error Dialog"),
                type: BootstrapDialog.TYPE_DANGER,
                message: errMsg,
                buttons: [{
                    label: $rootScope.getWord("Cancel"),
                    action: function (dialog) {
                        dialog.close();
                    }
                }]
            });
            */

            if (callbackMethod) {
                callbackMethod({ 'instance': undefined });
            }
        }
    }

    var deepValue = function (obj, path) {
        for (var i = 0, path = path.split('.'), len = path.length; i < len; i++) {
            obj = obj[path[i]];
        };
        return obj;
    };

    $scope.removeArrayRow = function (path, index)
    {
        var array = deepValue($scope.model, path);
        if (array) {
            array.splice(index, 1);
        }
    }

    $scope.addEmptyArrayRow = function(path)
    {
        var array = deepValue($scope.model, path);
        if (array) {
            var newObject = {};
            array.push(newObject);
        }
    }

    $scope.imageChangeTime = new Date().getTime();

    $scope.getImageUrl = function(property)
    {
        if ($scope.model &&
            $scope.model[property]) {
            return "styles/custom/images/" + $scope.model[property] + '?' + $scope.imageChangeTime;
        }
        else {
            return "styles/img/blank.jpg";
        }
    }

    $scope.ClearImage = function(property)
    {
        if ($scope.model &&
            $scope.model[property]) {
            // delete the image on server
            var url = APP_CONFIG.ebaasRootUrl + "/api/images/images/" + $scope.model[property];
            $http.delete(url).success(function (data) {
                $scope.model[property] = "";
            });
        }
    }

    $scope.getImageId = function(property)
    {
        if ($scope.model && $scope.model["obj_id"])
        {
            return "img-" + $scope.dbschema + "-" + $scope.dbclass + "-" + property + "-" + $scope.model["obj_id"] + ".png";
        }
        else
        {
            return "";
        }
    }

    $scope.showImageEditButtons = function()
    {
        if ($scope.model && $scope.model["obj_id"]) {
            return true;
        }
        else
        {
            return false;
        }
    }

    $scope.copyArrayRow = function (path, index) {

        var array = deepValue($scope.model, path);

        if (array) {
            var oldObject = array[index];
            var newObject = jQuery.extend(true, {}, oldObject);
            newObject.obj_id = "";
            array.push(newObject);
        }
    }

    $rootScope.$on('modalClosed', function (event, data) {
        if (data)
        {
            if (data.modal === "pickPrimaryKey")
            {
                $scope.model[data.property] = data.value;

                if (data.callback && data.callback === "true")
                    // pk change need to call the server-end callback function
                    $scope.reloadInstance(data.property);
            }
            else if (data.modal === "uploadImage") {
                $scope.model[data.property] = data.value;
                $scope.imageChangeTime = new Date().getTime();
            }
            else if (data.modal === "viewManyToMany") {
                loadFormData(); // reload the form data
            }
        }
    });

    $scope.getSelectedText = function getSelectedText(elementId) {
        var elt = document.getElementById(elementId);

        if (elt.selectedIndex == -1)
            return null;

        return elt.options[elt.selectedIndex].text;
    }

    $scope.getCssClasses = function (ngModelController) {
        return {
            'has-error': ngModelController && ngModelController.$invalid && (ngModelController.$dirty || $scope.submitted),
            'has-success': ngModelController && ngModelController.$valid && (ngModelController.$dirty || $scope.submitted)
        };
    };

    $scope.getFormCssClasses = function (formModelController) {
        return {
            'alert alert-danger': formModelController.$invalid || $scope.hasError,
            'alert alert-success': formModelController.$valid && !$scope.hasError
        };
    };

    $scope.showError = function (formModelController, ngModelController, error) {
        return (formModelController.$dirty || $scope.submitted) && formModelController.$error[error];
 
    };

    $scope.canSave = function () {
        //return $scope.ebaasform.$dirty && $scope.ebaasform.$valid;
        return true;
    };

    $scope.goBack = function () {
        if ($scope.submitted) {
            $state.go($scope.parentStateName, {}, { reload: true });
        }
        else {
            $state.go($scope.parentStateName);
        }
    }

    $scope.duplicate = function () {
        BootstrapDialog.show({
            title: $rootScope.getWord("Confirm Dialog"),
            type: BootstrapDialog.TYPE_WARNING,
            message: $rootScope.getWord("Confirm Duplicate Form"),
            buttons: [{
                label: $rootScope.getWord("Confirm"),
                action: function (dialog) {
                    dialog.close();
                    // duplicateFunc defined in sub controller to duplicate the current data instance
                    if ($scope.duplicateFunc)
                    {
                        $scope.duplicateFunc();
                    }
                }
            }, {
                label: $rootScope.getWord("Cancel"),
                action: function (dialog) {
                    dialog.close();
                }
            }]
        });
    }

    $scope.allowSubmit = function()
    {
        if (($scope.preview && $scope.preview === true) ||
            ($scope.readonly && $scope.readonly === true) ||
            ($scope.model && $scope.model.allowWrite === false) ||
            (!$scope.oid && $scope.model && $scope.model.allowCreate === false))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    $scope.allowDuplicate = function () {
        if ($scope.duplicateBtn && $scope.duplicateBtn === true && $scope.model && $scope.model.allowCreate === true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    $scope.options = {
        height: 250,
        focus: true,
        toolbar: [
                ['edit', ['undo', 'redo']],
                ['headline', ['style']],
                ['style', ['bold', 'italic', 'underline', 'superscript', 'subscript', 'clear']],
                ['fontface', ['fontname']],
                ['textsize', ['fontsize']],
                ['fontclr', ['color']],
                ['alignment', ['ul', 'ol', 'paragraph', 'lineheight']],
                ['table', ['table']],
                ['insert', ['link','picture']],
                ['view', ['codeview']]
        ]
    };
});

'use strict';

angular.module('app.smartforms').controller('ebaasFormCtrl', function ($controller, $rootScope, $scope, $http, APP_CONFIG, $state, $stateParams, parentStateName) {

    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.class;
    $scope.oid = $stateParams.oid;
    $scope.template = $stateParams.template;
    $scope.formAttribute = $stateParams.formAttribute;
    $scope.parentStateName = parentStateName;
    if ($stateParams.readonly && $stateParams.readonly === "true") {
        $scope.readonly = true;
    }
    else {
        $scope.readonly = false;
    }

    angular.extend(this, $controller('ebaasFormBaseCtrl', { $rootScope: $rootScope, $scope: $scope, $http: $http, APP_CONFIG: APP_CONFIG}));
});

'use strict';

angular.module('app.smartforms').controller('ebaasFormModalCtrl', function ($controller, $rootScope, $scope, $http, APP_CONFIG, $stateParams, $modalInstance, $state) {
 
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.class;
    $scope.oid = $stateParams.oid;
    $scope.template = $stateParams.template;
    $scope.formAttribute = $stateParams.formAttribute;
    $scope.preview = false;
    
    if ($stateParams.previewid)
    {
        $scope.preview = true;
        $scope.previewid = $stateParams.previewid;
    }
    else
    {
        $scope.previewid = undefined;
    }

    if ($stateParams.readonly && $stateParams.readonly === "true") {
        $scope.readonly = true;
    }
    else {
        $scope.readonly = false;
    }

    if ($stateParams.duplicate && $stateParams.duplicate === "true") {
        $scope.duplicateBtn = true;
    }
    else {
        $scope.duplicateBtn = false;
    }

    if ($stateParams.cmd) {
        $scope.cmdName = $stateParams.cmd;
    }
    else {
        $scope.cmdName = undefined;
    }

    if ($stateParams.sref) {
        $scope.sref = $stateParams.sref;
    }
    else {
        $scope.sref = undefined;
    }

    angular.extend(this, $controller('ebaasFormBaseCtrl', { $rootScope: $rootScope, $scope: $scope, $http: $http, APP_CONFIG: APP_CONFIG}));

    $scope.closeModal = function () {
        if ($scope.submitted)
            $modalInstance.close("update");
        else
            $modalInstance.dismiss("dismiss");
    };

    $scope.submitModalForm = function () {
        $scope.submitForm(function (result) {
            $scope.reloadForm();
        });
    }

    $scope.duplicateFunc = function()
    {
        if ($scope.cmdName && $scope.sref) {
            $scope.loading = true;
            // get custom commands, execute one of them
            var url = APP_CONFIG.ebaasRootUrl + "/api/sitemap/commands/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass;
            var cmdName;

            $http.get(url).success(function (commands) {
                // custom commands
                var cmdInfo;
                var item;
                var found = undefined;
                for (var cmd in commands) {
                    if (commands.hasOwnProperty(cmd)) {
                        cmdInfo = commands[cmd];
                        if (cmdInfo.name == $scope.cmdName) {
                            found = cmdInfo;
                            break;
                        }
                    }
                }

                if (found) {
                    url = $scope.getDuplicateDataUrl($scope.oid, true);

                    $http.get(url)
                       .success(function (data) {
                           //console.log(data);
                           var clonedObjId = data["obj_id"];

                           $scope.loading = false;

                           $state.go($scope.sref, { schema: $scope.dbschema, class: $scope.dbclass, oid: clonedObjId, hash: found.parameters.hash });
                       });

                }
                else {
                    console.debug("Unable to find a command with name " + $scope.cmdName);
                }
            })
        }
        else
        {
            $scope.loading = true;
            // get a shallow copy of instance and display it in the same modal window
            url = $scope.getDuplicateDataUrl($scope.oid, false);

            $http.get(url)
               .success(function (data) {
                   $scope.loading = false;
                   $scope.oid = data["obj_id"];
                   $scope.model = data;
                   $scope.submitted = true;
                   $scope.message = $rootScope.getWord("Instance Duplicated");
                   $scope.hasError = false;
               });
        }
    }
});

'use strict';

angular.module('app.smartforms').controller('pickPrimaryKeyCtrl', function ($scope, $controller, $rootScope, $http, APP_CONFIG, $stateParams, $modalInstance) {
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.pkclass;
    $scope.property = $stateParams.property;
    $scope.filter = unescape($stateParams.filter);
    $scope.callback = $stateParams.callback;

    angular.extend(this, $controller('dataGridBaseCtrl', { $scope: $scope, $rootScope: $rootScope, $http: $http, APP_CONFIG: APP_CONFIG}));

    $scope.gridInstance = null;
    $scope.dataGridSettings = {
        dataSource: {
            store: $scope.customStore
        },
        columnAutoWidth: true,
        sorting: {
            mode: "multiple"
        },
        editing: {
            allowAdding: false,
            allowUpdating: false,
            allowDeleting: false
        },
        grouping: {
            autoExpandAll: false
        },
        pager: {
            visible: true,
            showPageSizeSelector: true,
            showInfo: true
        },
        filterRow: {
            visible: true,
            applyFilter: "auto"
        },
        searchPanel: { visible: true },
        selection: { mode: 'single' },
        remoteOperations: true,
        bindingOptions: {
            columns: 'columns'
        },
        headerFilter: {
            visible: true
        },
        rowAlternationEnabled: true,
        onInitialized: function (e) {
            $scope.gridInstance = e.component;
        },
        onRowClick: function (e) {
            if (e.rowType == "data")
            {
                var url = APP_CONFIG.ebaasRootUrl + "/api/form/primarykey/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + encodeURIComponent(e.key);

                $http.get(url).success(function (data) {
                    // data is primary key value(s). If primary key consists of more than one property, primary key values are separated by &
                    $modalInstance.close({"modal": "pickPrimaryKey", "property" : $scope.property, "value" : data, "callback": $scope.callback});
                });
            }
        }
    };

    function unescape(str)
    {
        return str.replace(/%/g, "'");
    }
});

'use strict';

angular.module('app.smartforms').controller('relatedFormModalCtrl', function ($controller, $rootScope, $scope, $http, APP_CONFIG, $stateParams, $modalInstance, $state) {
 
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.rclass;
    $scope.oid = $stateParams.roid;
    $scope.template = $stateParams.rtemplate;
    $scope.formAttribute = $stateParams.rformAttribute;
    $scope.masterClass = ($stateParams.masterclass) ? $stateParams.masterclass : $stateParams.class;
    $scope.readonly = false;

    if ($stateParams.oid || $stateParams.masteroid) {
        $scope.masterOid = ($stateParams.masteroid) ? $stateParams.masteroid : $stateParams.oid;
    }
            
    if ($stateParams.readonly && $stateParams.readonly === "true")
    {
        $scope.readonly = true;
    }

    angular.extend(this, $controller('ebaasFormBaseCtrl', { $rootScope: $rootScope, $scope: $scope, $http: $http, APP_CONFIG: APP_CONFIG}));

    $scope.closeModal = function () {
        $rootScope.$broadcast('relatedModalFormClosed', { masterOid: $scope.masterOid });
        if ($scope.submitted)
            $modalInstance.close("update");
        else
            $modalInstance.dismiss("dismiss");
    };

    $scope.submitRelatedForm = function()
    {
        $scope.submitForm(function (result) {
            if ($scope.masterOid) {
                // link the related instance to the master instance
                $http.post(APP_CONFIG.ebaasRootUrl + "/api/relationship/" + encodeURIComponent($scope.dbschema) + "/" + $scope.masterClass + "/" + $scope.masterOid + "/" + $scope.dbclass + "/" + result.instance.obj_id)
                  .success(function (data) {
                      
                  });
            }
        });
    }
});

"use strict";

angular.module('app.smartforms').controller('uploadImageCtrl', function ($scope, $http, $stateParams, $modalInstance, APP_CONFIG, Upload, $timeout) {

    $scope.isSubmitted = false;

    // upload on file select or drop
    $scope.uploadFile = function (file) {
        $scope.loading = true;

        var fileId = $stateParams.imageid;

        file = Upload.rename(file, fileId); // rename the file using image id

        var uploadUrl = APP_CONFIG.ebaasRootUrl + "/api/images/images";

        file.upload = Upload.upload({
            url: uploadUrl,
            data: {file: file },
        });

        file.upload.onAfterAddingFile = function (item) {
            var fileExtension = '.' + item.file.name.split('.').pop();

            console.debug(fileExtension);

            item.file.name = $stateParams.imageid;

            console.debug(item.file.name);
        };

        file.upload.then(function (resp) {
            $timeout(function () {
                file.result = resp.data;
                $scope.loading = false;
                $scope.isSubmitted = true;
            });
        }, function (resp) {
            if (resp.status > 0)
                $scope.errorMsg = resp.status + ': ' + resp.data.message;
            $scope.loading = false;
        }, function (evt) {
            file.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
        });
    };

    $scope.closeModal = function () {
        if ($scope.isSubmitted) {
            $modalInstance.close({ "modal": "uploadImage", "property": $stateParams.property, "value": $stateParams.imageid });
        }
        else {
            $modalInstance.dismiss("dismiss");
        }
    };
});
'use strict';

angular.module('app.smartforms').controller('viewManyToManyCtrl', function ($scope, $controller, $rootScope, $http, APP_CONFIG, $stateParams, $modalInstance) {
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.relatedclass;

    angular.extend(this, $controller('dataGridBaseCtrl', { $scope: $scope, $rootScope: $rootScope, $http: $http, APP_CONFIG: APP_CONFIG }));

    $scope.gridInstance = null;
    $scope.existingKeys = null;
    $scope.currentKeys = null;
    $scope.isUpdated = false;
    $scope.loading = false;
    $scope.dataGridSettings = {
        dataSource: {
            store: $scope.customStore
        },
        columnAutoWidth: true,
        sorting: {
            mode: "multiple"
        },
        editing: {
            allowAdding: false,
            allowUpdating: false,
            allowDeleting: false
        },
        grouping: {
            autoExpandAll: false
        },
        pager: {
            visible: true,
            showPageSizeSelector: true,
            showInfo: true
        },
        filterRow: {
            visible: true,
            applyFilter: "auto"
        },
        searchPanel: { visible: false },
        selection: { mode: 'multiple'},
        remoteOperations: true,
        bindingOptions: {
            columns: 'columns'
        },
        headerFilter: {
            visible: true
        },
        rowAlternationEnabled: true,
        onInitialized: function (e) {
            $scope.gridInstance = e.component;
        },
        onContentReady: function (e) {
            selectGridRows();
        },
        onSelectionChanged: function(e)
        {
            changeSelections(e.currentSelectedRowKeys, e.currentDeselectedRowKeys);
        }
    };

    var asyncLoop = function(o)
    {
        var i = -1;

        var loop = function() {
            i++;
            if (i == o.length)
            {
                o.callback();
                return;
            }

            o.functionToLoop(loop, i);
        }

        loop(); // init
    }

    var selectGridRows = function()
    {
        if ($scope.currentKeys) {
            var keys = $scope.currentKeys;
            var indexes = [];
            for (var i = 0; i < keys.length; i++) {
                var index = $scope.gridInstance.getRowIndexByKey(keys[i]);

                if (index >= 0)
                    indexes.push(index);
            }

            $scope.gridInstance.selectRowsByIndexes(indexes, true);
        }
        else {
            if ($stateParams.masterid) {
                $http.get(APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($stateParams.schema) + "/" + $stateParams.masterclass + "/" + $stateParams.masterid + "/" + $stateParams.relatedclass)
                    .success(function (data) {
                        var keys = new Array();
                        if (data) {
                            for (var i = 0; i < data.length; i++) {
                                keys.push(data[i].obj_id);
                            }

                            if (keys.length > 0) {
                                // set the existing selections of rows
                                $scope.gridInstance.selectRows(keys, true);
                            }
                        }

                        $scope.existingKeys = keys; // keep the existing keys
                        // initialize the current keys
                        $scope.currentKeys = [];
                        for (var i = 0; i < keys.length; i++)
                        {
                            $scope.currentKeys.push(keys[i]);
                        }
                    });
            }
        }
    }

    // keep the current keys in sync with grid row selections
    var changeSelections = function (selectedKeys, deselectedKeys)
    {
        var addedKeys = new Array();
        var removedKeys = new Array();
        var found;

        // find the newly selected keys
        if (selectedKeys && $scope.currentKeys) {
            for (var i = 0; i < selectedKeys.length; i++) {
                found = false;

                var index = $scope.gridInstance.getRowIndexByKey(selectedKeys[i]);
                if (index > -1) {
                    for (var j = 0; j < $scope.currentKeys.length; j++) {
                        if (selectedKeys[i] === $scope.currentKeys[j]) {
                            found = true;
                            break;
                        }
                    }
                }

                if (!found) {
                    addedKeys.push(selectedKeys[i]);
                }
            }
        }

        if (deselectedKeys && $scope.currentKeys) {
            // find the unselected keys
            for (var i = 0; i < $scope.currentKeys.length; i++) {
                found = false;

                for (var j = 0; j < deselectedKeys.length; j++) {
                    var index = $scope.gridInstance.getRowIndexByKey(deselectedKeys[j]);
                    if (index > -1) {
                        if ($scope.currentKeys[i] === deselectedKeys[j]) {
                            found = true;
                            break;
                        }
                    }
                }

                if (found) {
                    removedKeys.push($scope.currentKeys[i]);
                }
            }
        }

        for (var i = 0; i < addedKeys.length; i++) {
            $scope.currentKeys.push(addedKeys[i]);
        }

        for (var i = 0; i < removedKeys.length; i++) {
            var index = $scope.currentKeys.indexOf(removedKeys[i]);
            if (index > -1)
                $scope.currentKeys.splice(index, 1);
        }

        //console.log("after current keys = " + $scope.currentKeys);
    }

    $scope.saveSelection = function () {
        var addedKeys = new Array();
        var removedKeys = new Array();
        var found;

        // find the added selections
        for (var i = 0; i < $scope.currentKeys.length; i++)
        {
            found = false;

            for (var j = 0; j < $scope.existingKeys.length; j++)
            {
                if ($scope.currentKeys[i] === $scope.existingKeys[j])
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                addedKeys.push($scope.currentKeys[i]);
            }
        }

        // find the removed selections
        for (var i = 0; i < $scope.existingKeys.length; i++) {
            found = false;

            for (var j = 0; j < $scope.currentKeys.length; j++) {
                if ($scope.existingKeys[i] === $scope.currentKeys[j]) {
                    found = true;
                    break;
                }
            }

            if (!found) {
                removedKeys.push($scope.existingKeys[i]);
            }
        }

        if (addedKeys.length > 0)
        {
            $scope.loading = true;

            // add relationhsips to the db
            asyncLoop({
                length: addedKeys.length,
                functionToLoop : function(loop, i)
                {
                    if ($stateParams.masterid) {
                        $http.post(APP_CONFIG.ebaasRootUrl + "/api/relationship/" + encodeURIComponent($stateParams.schema) + "/" + $stateParams.masterclass + "/" + $stateParams.masterid + "/" + $stateParams.relatedclass + "/" + encodeURIComponent(addedKeys[i]))
                             .success(function (data) {
                                 loop();
                             });
                    }
                },
                callback: function () {
                    $scope.loading = false;
                    $scope.isUpdated = true;
                }
            })
        }

        if (removedKeys.length > 0) {
            $scope.loading = true;
            // delete relationhsips from the db
            asyncLoop({
                length: removedKeys.length,
                functionToLoop: function (loop, i) {
                    $http.delete(APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($stateParams.schema) + "/" + $stateParams.masterclass + "/" + $stateParams.masterid + "/" + $stateParams.relatedclass + "/" + encodeURIComponent(removedKeys[i]))
                         .success(function (data) {
                             loop();
                         });
                },
                callback: function () {
                    $scope.existingKeys = [];
                    for (var i = 0; i < $scope.currentKeys.length; i++)
                        $scope.existingKeys.push($scope.currentKeys[i]);
                    $scope.loading = false;
                    $scope.isUpdated = true;
                }
            })
        }
   
    };

    $scope.goBack = function () {
        if ($scope.isUpdated) {
            $modalInstance.close({ "modal": "viewManyToMany"});
        }
        else {
            $modalInstance.dismiss("dismiss");
        }
    };
});

'use strict';

angular.module('app.smartforms').directive('buttonSpinner', function ($compile) {
    return {
        restrict: 'A',
        scope: {
            spinning: '=buttonSpinner',
            spinningIcon: '@?',
            buttonPrepend: '@?',
            buttonAppend: '@?'
        },
        transclude: true,
        template: 
        "<span ng-if=\"!!buttonPrepend\" ng-hide=\"spinning\"><i class=\"{{ buttonPrepend }}\"></i>&nbsp;</span>" +
        "<span ng-if=\"!!buttonPrepend\" ng-show=\"spinning\"><i class=\"{{ !!spinningIcon ? spinningIcon : 'fa fa-spinner fa-spin' }}\"></i>&nbsp;</span>" +
        "<ng-transclude></ng-transclude>" +
        "<span ng-if=\"!!buttonAppend\" ng-hide=\"spinning\">&nbsp;<i class=\"{{ buttonAppend }}\"></i></span>" +
        "<span ng-if=\"!buttonPrepend\" ng-show=\"spinning\">&nbsp;<i class=\"{{ !!spinningIcon ? spinningIcon : 'fa fa-spinner fa-spin' }}\"></i></span>"
    }
});
'use strict';

angular.module('app.smartforms').directive('dynamic', function ($compile) {
    return {
        restrict: 'A',
        replace: true,
        link: function (scope, ele, attrs) {
            scope.$watch(attrs.dynamic, function (html) {
                ele.html(html);
                $compile(ele.contents())(scope);
            });
        }
    };
});
'use strict';

angular.module('app.smartforms').directive('ebaasFormTemplate', function ($templateRequest, $compile, $sce, APP_CONFIG, $stateParams) {
    return {
        restrict: "E",
        scope: true,
        link: function (scope, element, attrs) {
            var schema = scope.$eval(attrs.dbschema);
            var cls = scope.$eval(attrs.dbclass);
            var oid = scope.$eval(attrs.oid);
            var template = scope.$eval(attrs.template);
            var formAttribute = scope.$eval(attrs.formattribute);
            var previewid = scope.$eval(attrs.previewid);
            var taskId = undefined;
            var readOnly = false;
            if (attrs.readonly)
            {
                readOnly = scope.$eval(attrs.readonly);
            }

            if (attrs.taskid) {
                taskId = scope.$eval(attrs.taskid);
            }

            var url = undefined;
            if (cls) {
                if (formAttribute && oid)
                {
                    // get custom form from an property value
                    url = APP_CONFIG.ebaasRootUrl + "/api/form/template/" + encodeURIComponent(schema) + "/" + cls + "/" + oid + "?templateSource=property&property=" + formAttribute + "&readOnly=" + readOnly;
                }
                else if (template) {
                    if (oid) {
                        // get custom form
                        url = APP_CONFIG.ebaasRootUrl + "/api/form/template/" + encodeURIComponent(schema) + "/" + cls + "/" + oid + "?templateSource=file&template=" + encodeURIComponent(template) + "&readOnly=" + readOnly;
                    }
                    else {
                        url = APP_CONFIG.ebaasRootUrl + "/api/form/template/" + encodeURIComponent(schema) + "/" + cls + "?templateSource=file&template=" + encodeURIComponent(template) + "&readOnly=" + readOnly;
                    }
                }
                else {
                    if (oid) {
                        // get a default form
                        url = APP_CONFIG.ebaasRootUrl + "/api/form/template/" + encodeURIComponent(schema) + "/" + cls + "/" + oid + "?readonly=" + readOnly;
                    }
                    else {
                        url = APP_CONFIG.ebaasRootUrl + "/api/form/template/" + encodeURIComponent(schema) + "/" + cls + "?readOnly=" + readOnly;
                    }
                }

                if (previewid)
                {
                    url += "&previewid=" + previewid;
                }

                if (taskId)
                {
                    url += "&taskId=" + taskId;
                }
            }
            
            if (url) {
                // Load the html through $templateRequest
                $templateRequest($sce.trustAsResourceUrl(url)).then(function (html) {
                    // Convert the html to an actual DOM node
                    
                    html = html.slice(1, html.length); // remove double quote at start and end
                    html = html.replace(/\\/g, "");

                    //console.log(html);
                    var template = angular.element($.trim(html)); // remove spaces

                    // Append it to the directive element
                    element.append(template);
                    // And let Angular $compile it
                    $compile(template)(scope);
                });
            }
        }
    }
});

'use strict';

angular.module('app.smartforms').directive('compile', function ($compile) {
    return function (scope, element, attrs) {
        var ensureCompileRunsOnce = scope.$watch(
          function (scope) {
              // watch the 'compile' expression for changes
              return scope.$eval(attrs.compile);
          },
          function (value) {
              // when the 'compile' expression changes
              // assign it into the current DOM
              element.html(value);

              // compile the new DOM and link it to the current
              // scope.
              // NOTE: we only compile .childNodes so that
              // we don't get into infinite loop compiling ourselves
              $compile(element.contents())(scope);

              // Use Angular's un-watch feature to ensure compilation only happens once.
              ensureCompileRunsOnce();
          }
      );
    };
});

'use strict';

angular.module('app.smarttables').controller('dataGridBaseCtrl', function ($scope, $rootScope, $state, $http, APP_CONFIG, $timeout, MetaDataCache, ngProgressFactory, searchContext) {

    // parameters to be provided by sub controllers
    $scope.dbschema;
    $scope.dbclass;
    $scope.view;
    $scope.tree;
    $scope.node;
    $scope.oid;
    $scope.filter;
    $scope.isrelated;
    $scope.relatedclass;
    $scope.relatedview;
    $scope.isfulltextsearch;

    var url;
    if ($scope.isrelated && $scope.isrelated === true)
    {
        // get meta data for a related class
        if ($scope.relatedview) {
            url = APP_CONFIG.ebaasRootUrl + "/api/metadata/view/" + encodeURIComponent($scope.dbschema) + "/" + $scope.relatedclass + "?view=" + $scope.relatedview;
        }
        else {
            url = APP_CONFIG.ebaasRootUrl + "/api/metadata/view/" + encodeURIComponent($scope.dbschema) + "/" + $scope.relatedclass;
        }
    }
    else
    {
        // get meta data for a master class
        if ($scope.view) {
            url = APP_CONFIG.ebaasRootUrl + "/api/metadata/view/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "?view=" + $scope.view;
        }
        else {
            url = APP_CONFIG.ebaasRootUrl + "/api/metadata/view/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass;
        }
    }

    //console.debug("meta data url = " + url);

    $http.get(url).success(function (data) {

        var column;
        var columns = new Array();

        // data is a JSON Schema for the class
        var properties = data.properties; // data.properies contains infos of each property of the schema

        $scope.selectColumns = "";

        var propertyInfo;
        for (var property in properties)
        {
            if (properties.hasOwnProperty(property))
            {
                propertyInfo = properties[property];

                column = new Object();
                column.dataField = property;
                column.caption = propertyInfo["title"];
                column.dataType = convertDataType(propertyInfo);
                if (column.dataType === "date")
                    column.calculateFilterExpression = function (filterValue, selectedFilterOperation) {
                        if (selectedFilterOperation === "=")
                        {
                            return [this.dataField, selectedFilterOperation || '=', dateFormat(filterValue)];
                        }
                        else
                            return this.defaultCalculateFilterExpression(filterValue, selectedFilterOperation);
                    };

                if ($scope.selectColumns === "")
                {
                    $scope.selectColumns = column.dataField + " AS [" + column.caption + "]";
                }
                else
                {
                    $scope.selectColumns += ", " + column.dataField + " AS [" + column.caption + "]";
                }

                if (propertyInfo["readonly"] === true)
                {
                    column.allowFiltering = false;
                    column.allowSorting = false;
                }
  
                if (propertyInfo["hidden"] === true)
                {
                    column.visible = false;
                }

                if (propertyInfo["type"] === "object") {

                    column.allowSearch = true;
                }
                else {
                    column.allowSearch = false;
                }

                if (column.dataType === "number")
                {
                    if (propertyInfo["format"] && propertyInfo["format"] === "progress") {
                        column.cellTemplate = function (container, options) {
                            $("<div><div class='progress-bar' aria-valuemin='0' aria-valuemax='100' aria-valuenow='" + options.value + "' style='width:" + options.value + "%'>" + options.value + "%</div></div>")
                              .attr("class", "progress")
                              .appendTo(container);
                        }
                    }
                }

                if (column.dataType === "string") {
                    if (propertyInfo["format"] && propertyInfo["format"] === "text") {
                        // long text, set the column width
                        column.width = "150";
                    }
                }

                if (propertyInfo["format"] == "date")
                {
                    column.format = "yyyy-MM-dd";
                }

                if (propertyInfo["format"] == "datetime") {
                    column.format = "yyyy-MM-dd HH:mm:ss";
                }

                if (propertyInfo.enum)
                {
                    if (propertyInfo.enum.length > 0)
                    {
                        // remove the first item which is for "unknown", wo don't need this filter value
                        propertyInfo.enum.splice(0, 1);
                    }
                    column.lookup = { dataSource: propertyInfo.enum };

                    if (propertyInfo["format"] && propertyInfo["format"] === "icon") {
                        column.cellTemplate = function (container, options) {
                            var src = "styles/custom/icons/" + options.value;
                            container.addClass("img-container");
                            $("<img />")
                                .attr("src", src)
                                .appendTo(container);
                        }
                    }
                }
                else if (propertyInfo["format"] && propertyInfo["format"] === "image")
                {
                    var thumbWidth = propertyInfo["minLength"];
                    var thumHeight = propertyInfo["maxLength"];
                    column.cellTemplate = function (container, options) {
                        var src = "styles/img/empty.jpg";
                        if (options.value)
                        {
                            src = "styles/custom/images/" + options.value;
                        }
                            
                        container.addClass("img-container");
                        $("<img />")
                            .attr("src", src)
                            .attr("width", thumbWidth)
                            .attr("height", thumHeight)
                            .appendTo(container);
                    }

                    column.allowFiltering = false;
                    column.allowSorting = false;
                }
                else if (column.dataType === "string") {
                    //column.width = "150";
                }

                columns.push(column);
            }
        }

        // Append a command column
        if ($scope.inlineCmds) {
            column = new Object();
            column.dataField = "obj_id";
            column.caption = "#";
            column.cellTemplate = "cellTemplate";
            columns.push(column);
        }

        $scope.columns = columns;
        if (!$scope.caption) {
            $scope.caption = data.title;
        }
    });

    function dateFormat(filterValue) {
        var dateString = new Date(filterValue).toLocaleDateString("en-US");

        return dateString;
    }

    function convertDataType(propertyInfo)
    {
        var dtype = "string";
        var dataFromat = undefined;
        var dataType = "string";

        if (propertyInfo["type"])
        {
            dtype = propertyInfo["type"];
        }

        if (propertyInfo["format"])
        {
            dataFromat = propertyInfo["format"];
        }

        switch (dtype)
        {
            case "integer":
                dataType = "number";
                break;

            case "string":

                if (dataFromat === "date") {
                    dataType = "date";
                }
                else if (dataFromat === "datetime")
                {
                    dataType = "date";
                }
                break;
        }

        return dataType;
    }

    function appendExpr(filterExpr, op, binaryExpr)
    {
        if (filterExpr) {
            var filter = JSON.parse(filterExpr);

            if (filterExpr[0] instanceof Array) {
                // filterExp is array of multipe binary exp
                filter.push(op, binaryExpr);

                return JSON.stringify(filter);
            }
            else {
                // filterExpr is a single binary expr
                var array = [];

                array.push(filter, op, binaryExpr);

                return JSON.stringify(array);
            }
        }
        else
        {
            return JSON.stringify(binaryExpr);
        }
    }

    var dbImpl = {
        key: 'obj_id',
        load: function (loadOptions) {
            if (!$scope.isfulltextsearch)
                return loadFromDB(loadOptions);
            else
                return loadFromSearchEngine(loadOptions);
        },
        totalCount: function (loadOptions) {
            if (!$scope.isfulltextsearch)
                return getCountFromDB(loadOptions);
            else
                return getCountFromSearchEngine(loadOptions);
        },
        remove: function (key) {
            var def = $.Deferred();

            var url;

            if ($scope.isrelated && $scope.isrelated === true) {
                url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.relatedclass + "/" + key;
            }
            else {
                url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + key;
            }

            $http.delete(url).success(function (result) {
                def.resolve(result);
            })
            .error(function (err) {
                def.reject(err["message"] + ":" + err["exceptionMessage"]);
            });

            return def.promise();
        }

    };

    $scope.customStore = new DevExpress.data.CustomStore(dbImpl);

    var asyncLoop = function (o) {
        var i = -1;

        var loop = function () {
            i++;
            if (i == o.length) {
                o.callback();
                return;
            }

            o.functionToLoop(loop, i);
        }

        loop(); // init
    }

    $scope.progressbar = ngProgressFactory.createInstance();

    $scope.export = function () {
        if ($scope.totalCount < 1000 ||
            confirm(String.format($rootScope.getWord("Export All"), $scope.totalCount))) {
            var items = new Array();

            $scope.progressbar.start();
            asyncLoop({
                length: Math.ceil($scope.totalCount / $scope.pageSize),
                functionToLoop: function (loop, i) {
                    var url = $scope.url;

                    var startIndex = url.indexOf("from=");
                    var endIndex = url.indexOf("&size=");
                    var rest = url.substring(endIndex);
                    if (startIndex != -1) {
                        url = url.substring(0, startIndex);
                        url += "from=" + i * $scope.pageSize;
                        url += rest;
                    }
                    else {
                        var pos = url.indexOf("?");
                        if (pos != -1) {
                            url += "&";
                        }
                        else {
                            url += "?";
                        }
                        url += "from=" + i * $scope.pageSize + "&size=" + $scope.pageSize;
                    }

                    $http.get(url).success(function (data) {
                        if (items.length === 0) {
                            items = data;
                        }
                        else {
                            items = items.concat(data);
                        }

                        loop();
                    }).error(function () {

                        $scope.progressbar.reset();
                    });
                },
                callback: function () {
                    var sql = 'SELECT ' + $scope.selectColumns + ' INTO XLSX("data.xlsx",{headers:true}) FROM ?';
 
                    alasql(sql, [items]);

                    $timeout($scope.progressbar.complete(), 1000);
                }
            })
        }
    };

    $scope.import = function () {
        if ($scope.isrelated && $scope.isrelated === true) {
            $state.go(".importdata", { schema: $scope.dbschema, class: $scope.dbclass, oid: $scope.oid, relatedclass: $scope.relatedclass }, { location: false, notify: false });
        }
        else {
            $state.go(".importdata", { schema: $scope.dbschema, class: $scope.dbclass }, { location: false, notify: false });
        }
    }

    function loadFromDB(loadOptions)
    {
        var url;
        var params = "";

        if ($scope.isrelated && $scope.isrelated === true) {
            if ($scope.oid) {
                if ($scope.relatedview) {
                    // get data for a related class using a view
                    url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + $scope.oid + "/" + $scope.relatedclass;
                    params = "view=" + $scope.relatedview;
                }
                else {
                    // get data for a related class using default view
                    url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + $scope.oid + "/" + $scope.relatedclass;
                }
            }
        }
        else {
            // get data for a master class
            url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass;
        }

        if (url) {

            if ($scope.tree &&
                $scope.node) {
                // search for a tree node
                params = "tree=" + $scope.tree + "&node=" + $scope.node;
            }
            else if ($scope.view) {
                // search for a view
                params = "view=" + $scope.view;
            }

            $scope.pageSize = 20;
            if (loadOptions.skip) {
                var range = "from=" + loadOptions.skip + "&size=" + loadOptions.take;
                $scope.pageSize = loadOptions.take;
                if (params === "") {
                    params = range;
                }
                else {
                    params += "&" + range;
                }
            }

            if (loadOptions.sort != null) {
                var sortExpr = "sortfield=" + loadOptions.sort[0].selector + "&sortreverse=" + loadOptions.sort[0].desc;
                if (params === "") {
                    params = sortExpr;
                }
                else {
                    params += "&" + sortExpr;
                }
            }

            // keyword search text
            var searchText = $('#gridContainer').dxDataGrid('instance')._options.searchPanel.text;

            var filter = undefined;
            if (loadOptions.filter) {
                var expr = JSON.stringify(loadOptions.filter);

                //Date.prototype.toJSON = function () { return moment(this).format(); };

                /*
                if (searchText)
                {
                    expr = appendExpr(expr, "and", ['keywords', 'contains', searchText]);
                }*/

                expr = encodeURIComponent(expr);

                filter = "filter=" + expr;
            }
            else if ($scope.filter) {
                var expr = $scope.filter;
                if (searchText) {
                    expr = appendExpr(expr, "and", ['keywords', 'contains', searchText]);
                }

                expr = encodeURIComponent(expr);

                filter = "filter=" + expr;
            }
            else if (searchText) {
                filter = "filter=['keywords', 'contains','" + encodeURIComponent(searchText) + "']";
            }

            if (filter) {
                if (params === "") {
                    params = filter;
                }
                else {
                    params += "&" + filter;
                }
            }

            if (params) {
                url += "?" + params;
            }

            var def = $.Deferred();

            $scope.url = url;

            $http.get(url).success(function (result) {
                def.resolve(result);

            }).error(function (err) {
                def.reject(err);
            });

            return def.promise();
        }
    }

    function getCountFromDB(loadOptions)
    {
        var def = $.Deferred();

        var url;

        if ($scope.isrelated && $scope.isrelated === true) {
            if ($scope.oid) {
                if ($scope.relatedview) {
                    // get total count for a related class using a view
                    url = APP_CONFIG.ebaasRootUrl + "/api/count/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + $scope.oid + "/" + $scope.relatedclass + "?view=" + $scope.relatedview;
                }
                else {
                    // get total count for a related class using default view
                    url = APP_CONFIG.ebaasRootUrl + "/api/count/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + $scope.oid + "/" + $scope.relatedclass
                }
            }
        }
        else {
            // get total count for master class
            if ($scope.tree && $scope.node) {
                url = APP_CONFIG.ebaasRootUrl + "/api/count/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "?tree=" + $scope.tree + "&node=" + $scope.node;
            }
            else if ($scope.view) {
                url = APP_CONFIG.ebaasRootUrl + "/api/count/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "?view=" + $scope.view;
            }
            else {
                url = APP_CONFIG.ebaasRootUrl + "/api/count/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass;
            }
        }

        // keyword search text
        var searchText = $('#gridContainer').dxDataGrid('instance')._options.searchPanel.text;

        var filter = undefined;
        if (loadOptions.filter) {
            var expr = JSON.stringify(loadOptions.filter)

            /*
            if (searchText) {
                expr = appendExpr(expr, "and", ['keywords', 'contains', searchText]);
            }*/

            expr = encodeURIComponent(expr);

            filter = "filter=" + expr;
        }
        else if ($scope.filter) {
            var expr = $scope.filter;
            if (searchText) {
                expr = appendExpr(expr, "and", ['keywords', 'contains', searchText]);
            }

            expr = encodeURIComponent(expr);

            filter = "filter=" + expr;
        }
        else if (searchText) {
            filter = "filter=['keywords', 'contains','" + encodeURIComponent(searchText) + "']";
        }

        if (filter) {
            var pos = url.indexOf("?");
            if (pos != -1) {
                url += "&";
            }
            else {
                url += "?";
            }
            url += filter;
        }

        if (url) {
            $http.get(url).success(function (result) {

                $scope.totalCount = result;

                // keep the count, view and filter in the cache, the report generation needs them
                var key = $scope.dbschema + $scope.dbclass + "TotalCount";
                MetaDataCache.setNamedData(key, result);

                key = $scope.dbschema + $scope.dbclass + "View";
                MetaDataCache.setNamedData(key, $scope.view);

                key = $scope.dbschema + $scope.dbclass + "Filter";
                MetaDataCache.setNamedData(key, filter);

                def.resolve(result);
            })
               .error(function (err) {
                   def.reject(err);
               });

            return def.promise();
        }
        else {
            return 0;
        }
    }

    function loadFromSearchEngine(loadOptions) {
        var url;
        var params = "";

        // get data for a master class
        url = APP_CONFIG.ebaasRootUrl + "/api/search/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass;

        if (url) {

            if ($scope.tree &&
                $scope.node) {
                // search for a tree node
                params = "tree=" + $scope.tree + "&node=" + $scope.node;
            }
            else if ($scope.view) {
                // search for a view
                params = "view=" + $scope.view;
            }

            $scope.pageSize = 20;
            if (loadOptions.skip) {
                var range = "from=" + loadOptions.skip + "&size=" + loadOptions.take;
                $scope.pageSize = loadOptions.take;
                if (params === "") {
                    params = range;
                }
                else {
                    params += "&" + range;
                }
            }

            if (loadOptions.sort != null) {
                var sortExpr = "sortfield=" + loadOptions.sort[0].selector + "&sortreverse=" + loadOptions.sort[0].desc;
                if (params === "") {
                    params = sortExpr;
                }
                else {
                    params += "&" + sortExpr;
                }
            }

            // full text search text
            var searchText = searchContext.typedText;

            var filter = undefined;
          
            if (searchText) {
                filter = "searchtext=" + encodeURIComponent(searchText);
            }

            if (filter) {
                if (params === "") {
                    params = filter;
                }
                else {
                    params += "&" + filter;
                }
            }

            if (params) {
                url += "?" + params;
            }

            var def = $.Deferred();

            $scope.url = url;

            $http.get(url).success(function (result) {
                def.resolve(result);

            }).error(function (err) {
                def.reject(err);
            });

            return def.promise();
        }
    }

    function getCountFromSearchEngine(loadOptions) {
        var def = $.Deferred();

        var url = APP_CONFIG.ebaasRootUrl + "/api/search/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/count";

        // keyword search text
        var searchText = searchContext.searchText;

        var filter = undefined;
       
        if (searchText) {
            filter = "filter=['keywords', 'contains','" + encodeURIComponent(searchText) + "']";
        }

        if (filter) {
            var pos = url.indexOf("?");
            if (pos != -1) {
                url += "&";
            }
            else {
                url += "?";
            }
            url += filter;
        }

        if (url) {
            $http.get(url).success(function (result) {

                $scope.totalCount = result;

                // keep the count, view and filter in the cache, the report generation needs them
                var key = $scope.dbschema + $scope.dbclass + "TotalCount";
                MetaDataCache.setNamedData(key, result);

                key = $scope.dbschema + $scope.dbclass + "View";
                MetaDataCache.setNamedData(key, $scope.view);

                key = $scope.dbschema + $scope.dbclass + "Filter";
                MetaDataCache.setNamedData(key, filter);

                def.resolve(result);
            })
               .error(function (err) {
                   def.reject(err);
               });

            return def.promise();
        }
        else {
            return 0;
        }
    }
});
'use strict';

angular.module('app.smarttables').controller('dataGridCtrl', function ($scope, $controller, $rootScope, $http, APP_CONFIG, $stateParams, $state, propmisedParams, hubService) {
   
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.class;
    $scope.oid = $stateParams.oid;

    var params = propmisedParams.data;

    if (params) {
        $scope.view = params['dataView'];
        $scope.formTemplate = params['formTemplate'];
    }
    else
    {
        $scope.view = undefined;
        $scope.formTemplate = undefined;
    }

    if ($stateParams.insert && $stateParams.insert === "false") {
        $scope.add = false;
    }
    else {
        $scope.add = true;
    }

    if ($stateParams.attachment && $stateParams.attachment === "false") {
        $scope.attachment = false;
    }
    else {
        $scope.attachment = true;
    }
 
    if ($stateParams.export && $stateParams.export === "true")
    {
        $scope.exportData = true;
    }
    else
    {
        $scope.exportData = false;
    }

    if ($stateParams.import && $stateParams.import === "true") {
        $scope.importData = true;
    }
    else {
        $scope.importData = false;
    }

    if ($stateParams.track && $stateParams.track === "true") {
        $scope.track = true;
    }
    else {
        $scope.track = false;
    }

    if ($stateParams.search && $stateParams.search === "fulltext")
    {
        $scope.isfulltextsearch = true;
    }
    else
    {
        $scope.isfulltextsearch = false;
    }

    angular.extend(this, $controller('dataGridBaseCtrl', { $scope: $scope, $rootScope: $rootScope, $http: $http, APP_CONFIG: APP_CONFIG}));

    $scope.openModal = function() {
        $state.go('.modalform', { schema: $scope.dbschema, class: $scope.dbclass, template: $scope.formTemplate }, { location: false, notify: false });
    };

    $scope.GetCommands = function (rowIndex, data)
    {
        var items = new Array();

        var url = APP_CONFIG.ebaasRootUrl + "/api/sitemap/commands/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + data.obj_id;

        $http.get(url).success(function (commands) {

            // custom commands
            $scope.commands = commands;
            var cmdInfo;
            var item;
            for (var cmd in commands) {
                if (commands.hasOwnProperty(cmd)) {
                    cmdInfo = commands[cmd];
                    item = new Object();
                    item.text = cmdInfo.title;
                    item.css = "btn btn-primary btn-md btn-nav";
                    item.track = false;
                    if (cmdInfo.icon) {
                        item.icon = cmdInfo.icon;
                    }
                    else
                    {
                        item.icon = "fa fa-lg fa-tasks";
                    }

                    item.onItemClick = function (text) {
                        gotoState(text, $scope.dbschema, data.type, data.obj_id, !data.allowWrite)
                    }

                    items.push(item);

                    if (cmdInfo.baseUrl && !APP_CONFIG.hashedBaseUrls[cmdInfo.hash]) {
                        APP_CONFIG.hashedBaseUrls[cmdInfo.hash] = cmdInfo.baseUrl;
                    }
                }
            }

            // add standard commands
            if (data.allowWrite && $stateParams.edit !== "false") {
                items.push({
                    text: $rootScope.getWord('Edit'),
                    icon: "fa fa-lg fa-edit",
                    css: "btn btn-default btn-md",
                    track : false,
                    onItemClick: function () {
                        $state.go('.modalform', { schema: $scope.dbschema, class: data.type, oid: data.obj_id, template: $scope.formTemplate, duplicate: "true" }, { location: false, notify: false });
                    }
                });
            }

            /*
            if (data.allowCreate && $stateParams.insert !== "false") {
                items.push({
                    text: $rootScope.getWord('Add'),
                    icon: "fa fa-lg fa-plus-square",
                    css: "btn btn-default btn-md btn-nav",
                    track : false,
                    onItemClick: function () {
                        $state.go('.modalform', { schema: $scope.dbschema, class: data.type }, { location: false, notify: false });
                    }
                });
            }
            */

            if (data.allowDelete && $stateParams.delete !== "false") {
                items.push({
                    text: $rootScope.getWord('Delete'),
                    icon: "fa fa-lg fa-times",
                    css: "btn btn-default btn-md",
                    track: false,
                    onItemClick: function () {
                        $scope.gridInstance.deleteRow(rowIndex);
                    }
                });
            }

            if ($scope.track) {
                var groupName = $scope.dbschema + "-" + data.type + "-" + data.obj_id;
                var isTracking = false;
                hubService.isUserInGroup(groupName, function (status) {
                    isTracking = status;

                    items.push({
                        text: $rootScope.getWord('Track Status'),
                        icon: "fa fa-lg fa-file-archive-o",
                        css: "btn btn-default btn-md",
                        track: true,
                        trackStatus: isTracking,
                        onItemClick: function () {

                            if (isTracking) {
                                hubService.removeFromGroup(groupName); // hubService removes the current user from the group
                            }
                            else {
                                hubService.addToGroup(groupName); // hubService adds the current user to the group
                            }
                        }
                    });
                });
            }
        });
        return items;
    }

    var gotoState = function (title, dbschema, dbclass, oid, readonly)
    {
        var commands = $scope.commands;
        var url = undefined;
        var cmdUrl = undefined;
        var params = undefined;
        var cmdInfo;
        for (var cmd in commands) {
            if (commands.hasOwnProperty(cmd)) {
                cmdInfo = commands[cmd];
                if (cmdInfo.title === title) {
                    url = cmdInfo.url;
                    cmdUrl = cmdInfo.url;
                    params = new Object();
                    params.schema = dbschema;
                    params.class = dbclass;
                    params.oid = oid;
                    params.readonly = readonly;
                    params.cmdHash = cmdInfo.hash;
   
                    // add command's parameters to the state parameters
                    if (cmdInfo.parameters) {
                        for (var key in cmdInfo.parameters) {
                            if (cmdInfo.parameters.hasOwnProperty(key)) {
                                params[key] = cmdInfo.parameters[key];
                            }
                        }
                    };

                    break;
                }
            }
        }

        if (url) {

            if (cmdUrl === ".modalform") {
                $state.go(url, params, { location: false, notify: false });
            }
            else {
                $state.go(url, params);
            }
        }
    }

    $scope.gridInstance = null;
    $scope.dataGridSettings = {
        dataSource : {
            store: $scope.customStore
        },
        columnAutoWidth: true,
        height: $rootScope.isChrome() === true ? '750px' : undefined,
        sorting: {
            mode: "multiple"
        },
        searchPanel: {
            visible: $stateParams.search && $stateParams.search === "true"? true: false,
            width: 300,
            placeholder: $rootScope.getWord("Keyword Search")
        },
        editing: {
            allowAdding: false,
            allowUpdating: false,
            allowDeleting: false
        },
        grouping: {
            autoExpandAll: false
        },
        pager: {
            visible: true,
            showPageSizeSelector: false,
            showInfo: true
        },
        filterRow: {
            visible: $scope.isfulltextsearch ? false : true,
            applyFilter: "auto"
        },
        selection: { mode: 'single' },
        remoteOperations: true,
        bindingOptions: {
            columns: 'columns',
            'scrolling.showScrollbar': 'never'
        },
        headerFilter: {
            visible: true
        },
        rowAlternationEnabled: true,
        masterDetail: {
            enabled: true,
            template: "detail"
        },
        onRowClick: function(e)
        {
            if (e.rowType === "data") {
                var isExpanded = $scope.gridInstance.isRowExpanded(e.key);
                $scope.gridInstance.collapseAll(-1); // collaspsed all
                if (!isExpanded)
                    {
                    $scope.gridInstance.expandRow(e.key);
                    }
            }
        },
        onInitialized: function (e) {
            $scope.gridInstance = e.component;
        }
    };

    $rootScope.$on('modalClosed', function (event, data) {
        if ($scope.gridInstance && data === "update")
            $scope.gridInstance.refresh();
    });

    $scope.downloadReports = function () {
        $state.go(".downloadreports", { schema: $scope.dbschema, class: $scope.dbclass }, { location: false, notify: false });
    }
});
'use strict';

angular.module('app.smarttables').controller('relatedDataGridCtrl', function ($scope, $controller, $rootScope, $http, APP_CONFIG, $stateParams, $state, promiseParentClassInfo) {
   
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.class;
    $scope.oid = $stateParams.oid;
    $scope.isrelated = true;
    $scope.relatedclass = $stateParams.relatedclass;

    if ($stateParams.exportrelated && $stateParams.exportrelated === "true") {
        $scope.exportData = true;
    }
    else {
        $scope.exportData = false;
    }

    if ($stateParams.importrelated && $stateParams.importrelated === "true") {
        $scope.importData = true;
    }
    else {
        $scope.importData = false;
    }

    if ($stateParams.insertrelated && $stateParams.insertrelated === "false") {
        $scope.add = false;
    }
    else {
        $scope.add = true;
    }

    if ($stateParams.editrelated && $stateParams.editrelated === "false") {
        $scope.edit = false;
    }
    else {
        $scope.edit = true;
    }

    if ($stateParams.attachmentrelated && $stateParams.attachmentrelated === "false") {
        $scope.attachment = false;
    }
    else {
        $scope.attachment = true;
    }

    if ($stateParams.deleterelated && $stateParams.deleterelated === "false") {
        $scope.delete = false;
    }
    else {
        $scope.delete = true;
    }

    $scope.parentClassInfo = promiseParentClassInfo.data;

    angular.extend(this, $controller('dataGridBaseCtrl', { $scope: $scope, $rootScope: $rootScope, $http: $http, APP_CONFIG: APP_CONFIG}));

    $scope.openModal = function () {
        $state.go('.relatedform', { schema: $scope.dbschema, rclass: $scope.relatedclass}, { location: false, notify: false });
    };

    $scope.GetCommands = function (rowIndex, data)
    {
        var items = [];

        var url = APP_CONFIG.ebaasRootUrl + "/api/sitemap/commands/" + encodeURIComponent($scope.dbschema) + "/" + $scope.relatedclass + "/" + data.obj_id;

        $http.get(url).success(function (commands) {

            // custom commands
            $scope.commands = commands;
            var cmdInfo;
            var item;
            for (var cmd in commands) {
                if (commands.hasOwnProperty(cmd)) {
                    cmdInfo = commands[cmd];
                    item = new Object();
                    item.text = cmdInfo.title;
                    item.css = "btn btn-primary btn-md btn-nav";
                    item.track = false;
                    if (cmdInfo.icon) {
                        item.icon = cmdInfo.icon;
                    }
                    else {
                        item.icon = "fa fa-lg fa-tasks";
                    }

                    item.onItemClick = function (text) {
                        gotoState(text, $scope.dbschema, data.type, data.obj_id, !data.allowWrite)
                    }

                    items.push(item);

                    if (cmdInfo.baseUrl && !APP_CONFIG.hashedBaseUrls[cmdInfo.hash]) {
                        APP_CONFIG.hashedBaseUrls[cmdInfo.hash] = cmdInfo.baseUrl;
                    }
                }
            }

            // add standard commands
            if (data.allowWrite && $scope.edit) {
                items.push({
                    text: $rootScope.getWord('Edit'),
                    icon: "fa fa-lg fa-edit",
                    css: "btn btn-default btn-md btn-nav",
                    onItemClick: function () {
                        $state.go('.relatedform', { schema: $scope.dbschema, rclass: data.type, roid: data.obj_id }, { location: false, notify: false });
                    }
                });
            }

            if (data.allowDelete && $scope.delete) {
                items.push({
                    text: $rootScope.getWord('Delete'),
                    icon: "fa fa-lg fa-times",
                    css: "btn btn-default btn-md btn-nav",
                    onItemClick: function () {
                        $scope.gridInstance.deleteRow(rowIndex);
                    }
                });
            }
        });
        
        return items;
    }

    var gotoState = function (title, dbschema, dbclass, oid, readonly)
    {
        var commands = $scope.commands;
        var url = undefined;
        var cmdUrl = undefined;
        var params = undefined;
        var cmdInfo;
        for (var cmd in commands) {
            if (commands.hasOwnProperty(cmd)) {
                cmdInfo = commands[cmd];
                if (cmdInfo.title === title) {
                    url = cmdInfo.url;
                    cmdUrl = cmdInfo.url;
                    params = new Object();
                    params.schema = dbschema;
                    params.class = dbclass;
                    params.oid = oid;
                    params.readonly = readonly;
                    params.cmdHash = cmdInfo.hash;

                    // add command's parameters to the state parameters
                    if (cmdInfo.parameters) {
                        for (var key in cmdInfo.parameters) {
                            if (cmdInfo.parameters.hasOwnProperty(key)) {
                                params[key] = cmdInfo.parameters[key];
                            }
                        }
                    };

                    break;
                }
            }
        }

        if (url) {

            if (cmdUrl === ".modalform" || cmdUrl === ".report") {
                $state.go(url, params, { location: false, notify: false });
            }
            else {
                $state.go(url, params);
            }
        }
    }

    $scope.gridInstance = null;
    $scope.dataGridSettings = {
        dataSource: {
            store: $scope.customStore
        },
        columnAutoWidth: true,
        height: $rootScope.isChrome() === true ? '750px' : undefined,
        sorting: {
            mode: "multiple"
        },
        searchPanel: {
            visible: false,
            highlightSearchText: false
        },
        editing: {
            allowAdding: false,
            allowUpdating: false,
            allowDeleting: false
        },
        grouping: {
            autoExpandAll: false
        },
        pager: {
            visible: true,
            showPageSizeSelector: false,
            showInfo: true
        },
        filterRow: {
            visible: true,
            applyFilter: "auto"
        },
        selection: { mode: 'single' },
        remoteOperations: true,
        bindingOptions: {
            columns: 'columns'
        },
        headerFilter: {
            visible: true
        },
        rowAlternationEnabled: true,
        masterDetail: {
            enabled: true,
            template: "detail"
        },
        onRowClick: function (e) {
            if (e.rowType === "data") {
                var isExpanded = $scope.gridInstance.isRowExpanded(e.key);
                $scope.gridInstance.collapseAll(-1); // collaspsed all
                if (!isExpanded) {
                    $scope.gridInstance.expandRow(e.key);
                }
            }
        },
        onInitialized: function (e) {
            $scope.gridInstance = e.component;
        }
    };

    $scope.parentClassTitle = function () {
        // return class title
        return $scope.parentClassInfo.title;
    }

    $scope.goBack = function () {
        history.back(1);
    }

    $rootScope.$on('modalClosed', function (event, data) {
        if ($scope.gridInstance && data === "update")
            $scope.gridInstance.refresh();
    });
});
"use strict";

angular.module("app.smarttables").factory("MetaDataCache", function () {

    var cache = new Object();

    function _getClassView(dbschema, dbclass, view) {
        var data = undefined;
        var key = dbschema + "_" + dbclass + "_" + view;
        if (cache[key])
        {
            data = cache[key];
        }

        return data;
    }

    function _setClassView(dbschema, dbclass, view, data) {
        var key = dbschema + "_" + dbclass + "_" + view;
        cache[key] = data;
    }

    function _getNamedData(dataName) {
        var data = undefined;
        if (cache[dataName]) {
            data = cache[dataName];
        }

        return data;
    }

    function _setNamedData(dataName, data) {
        cache[dataName] = data;
    }

    return {
        getClassView: _getClassView,
        setClassView: _setClassView,
        getNamedData : _getNamedData,
        setNamedData : _setNamedData
    };
});
'use strict';

angular.module('app.smartreports').controller('downloadReportCtrl', function ($controller, $rootScope, $scope, $http, APP_CONFIG, $stateParams, $modalInstance, fileManager) {
 
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.class;
    $scope.oid = $stateParams.oid;
    $scope.template = $stateParams.template;
    $scope.templateAttribute = $stateParams.templateAttribute;

    $scope.baseUrl = APP_CONFIG.ebaasRootUrl;
    if (APP_CONFIG.hashedBaseUrls[$stateParams.cmdHash])
    {
        $scope.baseUrl = APP_CONFIG.hashedBaseUrls[$stateParams.cmdHash];
    }
   
    $scope.loading = false;

    $scope.closeModal = function () {
        $modalInstance.dismiss("dismiss");
    };

    $scope.download = function() {

        var getFileUrl = undefined;
 
        if ($scope.templateAttribute)
        {
            getFileUrl = $scope.baseUrl + "/api/report/" + $scope.dbschema + "/" + $scope.dbclass + "/" + $scope.oid + "?templateSource=property&property=" + $scope.templateAttribute;

        }
        else if ($scope.template) {
            getFileUrl = $scope.baseUrl + "/api/report/" + $scope.dbschema + "/" + $scope.dbclass + "/" + $scope.oid + "?templateSource=file&template=" + encodeURIComponent($scope.template);
        }

        if (getFileUrl) {
            $scope.loading = true;

            fileManager.performDownload(getFileUrl, function () {
                $scope.loading = false;
            });
        }
        else
        {
            BootstrapDialog.show({
                title: $rootScope.getWord("Info Dialog"),
                type: BootstrapDialog.TYPE_DANGER,
                message: "template or property parameter not defined",
                buttons: [{
                    label: $rootScope.getWord("Cancel"),
                    action: function (dialog) {
                        dialog.close();
                    }
                }]
            });
            
        }
    }
});

'use strict';

angular.module('app.stations').controller('StationDashboardCtrl', function ($controller, $rootScope, $scope, $http, APP_CONFIG, $state, $stateParams, TestStations, promisedSettings, $interval) {

    // Live Feeds Widget Data And Display Controls
    // Live Stats Tab
    var index = $stateParams.index;
    var testStationName = TestStations.params['testStationName'];

    $scope.CurrentStationName = TestStations.stations[index][testStationName];
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.class;
    $scope.template = TestStations.params['testStationForm'];
    $scope.oid = $stateParams.oid;
    $scope.showMonitor = true;
  
    if (TestStations.params['monitor'] && TestStations.params['monitor'] === "hidden")
    {
        $scope.showMonitor = false;
    }

    var defaultSettings = {
        ID: "0",
        LineXName1: "X",
        LineXMin1: "0",
        LineXMax1: "100",
        LineXValue1: "0",
        LineYName1: "Y",
        LineYMin1: "0",
        LineYMax1: "100",
        LineYValue1: "0",
        PieName1: "",
        PieName2: "",
        PieName3: "",
        PieName4: "",
        PieUnit1: "",
        PieUnit2: "",
        PieUnit3: "",
        PieUnit4: "",
        PieValue1: "",
        PieValue2: "",
        PieValue3: "",
        PieValue4: "",
        PiePercent1: "0",
        PiePercent2: "0",
        PiePercent3: "0",
        PiePercent4: "0",
        ProgressName1: "",
        ProgressName2: "",
        ProgressName3: "",
        ProgressName4: "",
        ProgressUnit1: "",
        ProgressUnit2: "",
        ProgressUnit3: "",
        ProgressUnit4: "",
        ProgressValue1: "",
        ProgressValue2: "",
        ProgressValue3: "",
        ProgressValue4: "",
        ProgressPercent1: "0",
        ProgressPercent2: "0",
        ProgressPercent3: "0",
        ProgressPercent4: "0"
    };

    if ($scope.showMonitor && promisedSettings)
    {
        var settingsWrapper = promisedSettings.data
        var settings = findObjectByName(settingsWrapper, "Settings")
        if (settings)
        {
            $scope.settings = settings;
        }
        else
        {
            $scope.settings = defaultSettings;
        }
    }
    else
    {
        $scope.settings = defaultSettings;
    }

    function findObjectByName(obj, name) {
        for (var i in obj) {
            if (obj.hasOwnProperty(i)) {
                if (i === name)
                {
                    return obj[i];
                }
                else if (obj[i] instanceof Object)
                {
                    return findObjectByName(obj[i], name);
                }
            }
        }
        return null;
    };

    angular.extend(this, $controller('ebaasFormBaseCtrl', { $rootScope: $rootScope, $scope: $scope, $http: $http, APP_CONFIG: APP_CONFIG }));

    $scope.autoUpdate = false;
    var data = [];
    var totalPoints = 100;
    var timerInterval = 1000;
    var currentTime = new Date().getTime();

    var updateInterval = undefined;
    $scope.$watch('autoUpdate', function (autoUpdate) {

        if (autoUpdate) {
            updateInterval = $interval(function () {

                GetData(true);

                $.plot($("#updating-chart"), [data], $scope.liveStatsOptions)
            }, timerInterval)
        } else {
            $interval.cancel(updateInterval);
        }
    });

    // cancel the auto updating when exiting
    $scope.$on("$destroy", function (event) {
        if (updateInterval) {
            $interval.cancel(updateInterval);
        }
    });

    if ($scope.showMonitor) {

        GetData(false);

        $scope.liveStats = [data];
    }
    else
    {
        $scope.liveStats = [];
    }

    function GetData(fromDB) {
        data.shift(); //to remove first item of array
      
        var temp, y;
        if (fromDB && $stateParams.xmlschema)
        {
            var url = APP_CONFIG.ebaasRootUrl + "/api/data/extract/" + encodeURIComponent($stateParams.schema) + "/" + $stateParams.class + "/" + $stateParams.oid + "/" + $stateParams.xmlschema;

            $http.get(url).success(function (res) {
                var settings = findObjectByName(res, "Settings")
                if (settings) {
                    y = settings.LineYValue1;
                    $scope.settings = settings;
                }
                else
                {
                    y = 3.0; // fake data
                }

                temp = [currentTime += timerInterval, y]; //data format [x, y]

                data.push(temp);
            });
        }
        else
        {
            while (data.length < totalPoints) {
                // fake data
                 y = 2.0;
                //y = Math.random() * 100;

                temp = [currentTime += timerInterval, y]; //data format [x, y]

                data.push(temp);
            }
        }
    }

    if ($scope.showMonitor) {
        $scope.liveStatsOptions = {
            yaxis: {
                min: $scope.settings.LineYMin1,
                max: $scope.settings.LineYMax1,
                tickFormatter: function (v, axis) {
                    if (v % 10 == 0) {
                        return v;
                    } else {
                        return "";
                    }
                }
            },
            xaxis: {
                mode: "time",
                tickSize: [2, "second"],
                tickFormatter: function (v, axis) {
                    var date = new Date(v);

                    if (date.getSeconds() % 20 == 0) {
                        var hours = date.getHours() < 10 ? "0" + date.getHours() : date.getHours();
                        var minutes = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();
                        var seconds = date.getSeconds() < 10 ? "0" + date.getSeconds() : date.getSeconds();

                        return hours + ":" + minutes + ":" + seconds;
                    } else {
                        return "";
                    }
                }
            },
            colors: ['rgb(87, 136, 156)'],
            series: {
                lines: {
                    lineWidth: 1,
                    fill: true,
                    fillColor: {
                        colors: [
                            {
                                opacity: 0.4
                            },
                            {
                                opacity: 0
                            }
                        ]
                    },
                    steps: false
                }
            }
        };
    }

    // station scheduler code
    var appointmentClass = TestStations.params['appointmentClass'];
    var appointmentToStation = TestStations.params['appointmentToStation'];
    $scope.stationpk = TestStations.stations[index]["obj_pk"];

    $scope.schedulerOptions = {
        dataSource: new DevExpress.data.DataSource({
            store: new DevExpress.data.CustomStore({
                key: 'obj_id',
                load: function (options) {
                    var def = $.Deferred();

                    var filter = createFilter(options);
                    var url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + $scope.oid + "/" + appointmentClass + "?from=0&size=500&filter=" + JSON.stringify(filter);
                    $http.get(url).success(function (result) {
                        def.resolve(result);
                    }).error(function (err) {
                        def.reject(err);
                    });

                    return def.promise();
                },
                byKey: function (key) {

                    var def = $.Deferred();

                    var url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + appointmentClass + "/" + key;

                    $http.get(url).success(function (result) {
                        def.resolve(result);
                    }).error(function (err) {
                        def.reject(err);
                    });

                    return def.promise();
                },
                insert: function (data) {

                    var def = $.Deferred();

                    //scheduler.server.saveAppointment(data.text, data.startDate, data.endDate);
                    var converted = convertModel(data)

                    // associated new appointment with the station by primary key
                    converted[appointmentToStation] = $scope.stationpk;

                    var url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + appointmentClass;

                    $http.post(url, converted)
                     .success(function (result) {
                         def.resolve(result);
                     }).error(function (err) {
                         def.reject(err);
                     });

                    return def.promise();
                },
                remove: function (key) {
                    var def = $.Deferred();

                    var url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + appointmentClass + "/" + key;

                    $http.delete(url).success(function (result) {
                        def.resolve(result);
                    })
                    .error(function (err) {
                        def.reject(err);
                    });

                    return def.promise();
                },
                update: function (key, data) {
                    var def = $.Deferred();

                    var converted = convertModel(data);

                    var url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + appointmentClass + "/" + key;

                    $http.post(url, converted)
                     .success(function (result) {
                         def.resolve(result);
                     }).error(function (err) {
                         def.reject(err);
                     });

                    return def.promise();
                }
            }),
            map: function (item) {
                var appointment = {
                    text: item.Subject,
                    description: item.Description,
                    startDate: createDate(item.StartTime),
                    endDate: createDate(item.EndTime),
                    color: "#ff00aa",
                    obj_id: item.obj_id
                };

                //console.debug("appointment=" + JSON.stringify(appointment));
                return appointment;
            }
        }),
        views: ["month", "week", "day"],
        currentView: "month",
        currentDate: new Date(),
        firstDayOfWeek: 0,
        startDayHour: 8,
        endDayHour: 19,
        width: "100%",
        height: 600,
        onInitialized: function (e) {
            $scope.scheduleInstance = e.component;
        }
    };

    function createDate(str1) {
        // str1 format should be yyyy-mm-ddThh:mm:SS.
        var dt1 = parseInt(str1.substring(8, 10));
        var mon1 = parseInt(str1.substring(5, 7));
        var yr1 = parseInt(str1.substring(0, 4));
        var date1 = new Date(yr1, mon1 - 1, dt1);
        return date1;
    }

    function createFilter(options) {
        var filter = new Array();

        var composite = new Array();

        var expr = new Array();
        // StartTime >= intervalStartDay
        expr.push("StartTime");
        expr.push(">=");
        expr.push(options.dxScheduler.startDate);

        composite.push(expr);

        composite.push("and");

        expr = new Array();
        expr.push("StartTime");
        expr.push("<");
        expr.push(options.dxScheduler.endDate);

        composite.push(expr);

        filter.push(composite);

        filter.push("or");

        composite = new Array();
        expr = new Array();
        expr.push("EndTime");
        expr.push(">=");
        expr.push(options.dxScheduler.startDate);

        composite.push(expr);

        composite.push("and");

        expr = new Array();
        expr.push("EndTime");
        expr.push("<");
        expr.push(options.dxScheduler.endDate);

        composite.push(expr);

        filter.push(composite);

        return filter;
    }

    function convertModel(data) {
        var converted = new Object();

        converted.StartTime = data.startDate;
        converted.EndTime = data.endDate;
        converted.Subject = data.text;
        converted.Description = data.description;

        return converted;
    }

    var timezoneParseNameResults = /\((.*)\)/.exec(new Date().toString());

    if (timezoneParseNameResults && timezoneParseNameResults.length > 0) {
        $scope.timezone = timezoneParseNameResults[1];
    } else {
        var timezoneOffset = new Date().getTimezoneOffset();
        $scope.timezone = "UTC" + (timezoneOffset < 0 ? "+" : "") + timezoneOffset / (-60) + " time zone";
    }
});
'use strict';

angular.module('app.stations').controller('StationSchedulerCtrl', function ($scope, $http, $stateParams, APP_CONFIG, TestStations) {

    var index = $stateParams.index;
    var testStationName = TestStations.params['testStationName'];
    var appointmentClass = TestStations.params['appointmentClass'];
    var appointmentToStation = TestStations.params['appointmentToStation'];

    $scope.CurrentStationName = TestStations.stations[index][testStationName];
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.class;
    $scope.oid = TestStations.stations[index]["obj_id"];
    $scope.stationpk = TestStations.stations[index]["obj_pk"];

    $scope.schedulerOptions = {
        dataSource: new DevExpress.data.DataSource({
            store: new DevExpress.data.CustomStore({
                key: 'obj_id',
                load: function (options) {
                    var def = $.Deferred();

                    var filter = createFilter(options);
                    var url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "/" + $scope.oid + "/" + appointmentClass + "?from=0&size=500&filter=" + JSON.stringify(filter);

                    $http.get(url).success(function (result) {
                        def.resolve(result);
                    }).error(function (err) {
                        def.reject(err);
                    });

                    return def.promise();
                },
                byKey: function (key) {

                    var def = $.Deferred();

                    var url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + appointmentClass + "/" + key;

                    $http.get(url).success(function (result) {
                        def.resolve(result);
                    }).error(function (err) {
                        def.reject(err);
                    });

                    return def.promise();
                },
                insert: function (data) {

                    var def = $.Deferred();

                    //scheduler.server.saveAppointment(data.text, data.startDate, data.endDate);
                    var converted = convertModel(data)
                    
                    // associated new appointment with the station by primary key
                    converted[appointmentToStation] = $scope.stationpk;

                    var url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + appointmentClass;

                    $http.post(url, converted)
                     .success(function (result) {
                         def.resolve(result);
                     }).error(function (err) {
                         def.reject(err);
                     });

                    return def.promise();
                },
                remove: function (key) {
                    var def = $.Deferred();

                    var url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + appointmentClass + "/" + key;

                    $http.delete(url).success(function (result) {
                        def.resolve(result);
                    })
                    .error(function (err) {
                        def.reject(err);
                    });

                    return def.promise();
                },
                update: function (key, data) {
                    var def = $.Deferred();

                    var converted = convertModel(data);

                    var url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + appointmentClass + "/" + key;

                    $http.post(url, converted)
                     .success(function (result) {
                         def.resolve(result);
                     }).error(function (err) {
                         def.reject(err);
                     });

                    return def.promise();
                }
            }),
            map: function (item) {
                var appointment = {
                    text: item.Subject,
                    description: item.Description,
                    startDate: createDate(item.StartTime),
                    endDate: createDate(item.EndTime),
                    color: "#ff00aa",
                    obj_id: item.obj_id
                };

                //console.debug("appointment=" + JSON.stringify(appointment));
                return appointment;
            }
        }),
        views: ["month", "week", "day"],
        currentView: "month",
        currentDate: new Date(2016, 3, 25),
        firstDayOfWeek: 0,
        startDayHour: 8,
        endDayHour: 19,
        width: "100%",
        height: 600,
        onInitialized: function (e) {
            $scope.scheduleInstance = e.component;
        }
    };

    function createDate(str1) {
        // str1 format should be yyyy-mm-ddThh:mm:SS.
        var dt1 = parseInt(str1.substring(8, 10));
        var mon1 = parseInt(str1.substring(5, 7));
        var yr1 = parseInt(str1.substring(0, 4));
        var date1 = new Date(yr1, mon1 - 1, dt1);
        return date1;
    }

    function createFilter(options) {
        var filter = new Array();

        var composite = new Array();

        var expr = new Array();
        // StartTime >= intervalStartDay
        expr.push("StartTime");
        expr.push(">=");
        expr.push(options.dxScheduler.startDate);

        composite.push(expr);

        composite.push("and");

        expr = new Array();
        expr.push("StartTime");
        expr.push("<");
        expr.push(options.dxScheduler.endDate);

        composite.push(expr);

        filter.push(composite);

        filter.push("or");

        composite = new Array();
        expr = new Array();
        expr.push("EndTime");
        expr.push(">=");
        expr.push(options.dxScheduler.startDate);

        composite.push(expr);

        composite.push("and");

        expr = new Array();
        expr.push("EndTime");
        expr.push("<");
        expr.push(options.dxScheduler.endDate);

        composite.push(expr);

        filter.push(composite);

        return filter;
    }

    function convertModel(data)
    {
        var converted = new Object();

        converted.StartTime = data.startDate;
        converted.EndTime = data.endDate;
        converted.Subject = data.text;
        converted.Description = data.description;
        
        return converted;
    }

    var timezoneParseNameResults = /\((.*)\)/.exec(new Date().toString());

    if (timezoneParseNameResults && timezoneParseNameResults.length > 0) {
        $scope.timezone = timezoneParseNameResults[1];
    } else {
        var timezoneOffset = new Date().getTimezoneOffset();
        $scope.timezone = "UTC" + (timezoneOffset < 0 ? "+" : "") + timezoneOffset / (-60) + " time zone";
    }
});
'use strict';

angular.module('app.stations').controller('StationsLayoutCtrl', function ($http, APP_CONFIG, $scope, $rootScope, $state, $stateParams, stationParams, TestStations, MetaDataCache) {

    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.class;
 
    TestStations.params = stationParams.data;

    var testStationGroup = TestStations.params['testStationGroup'];
    var testStationName = TestStations.params['testStationName'];
    var testStationStatus = TestStations.params['testStationStatus'];

    if (MetaDataCache.getNamedData("stationTree")) {
        $scope.stationTree = MetaDataCache.getNamedData("stationTree");

        if (TestStations.stations.length > 0) {
            // show the dashboard of the first station by default
            var oid = TestStations.stations[0].obj_id;
            $state.go('app.stations.dashboard', { schema: $stateParams.schema, class: $stateParams.class, oid: oid, xmlschema: TestStations.params['xmlSchemaName'], index: 0 });
        }
    }
    else {
        // url to get station tree model, assuming no more than 1000 stations
        var url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass + "?view=full&from=0&size=1000";

        $http.get(url).then(function (res) {

            TestStations.stations = res.data;

            $scope.stationTree = CreateTree(testStationGroup,
                testStationName,
                testStationStatus,
                $scope.dbclass);

            MetaDataCache.setNamedData("stationTree", $scope.stationTree);

            if (TestStations.stations.length > 0) {
                // show the dashboard of the first station by default
                var oid = TestStations.stations[0].obj_id;
                $state.go('app.stations.dashboard', { schema: $stateParams.schema, class: $stateParams.class, oid: oid, xmlschema: TestStations.params['xmlSchemaName'], index: 0 });
            }
        });
    }

    function GetNodeClass(status)
    {
        var nodeClass;
      
        switch (status)
        {
            case $rootScope.getWord("Available"):
                nodeClass = "label label-primary";
                break;

            case $rootScope.getWord("Occupied"):
                nodeClass = "label label-success";
                break;

            case $rootScope.getWord("Under Maintenance"):
                nodeClass = "label label-warning";
                break;

            case $rootScope.getWord("Not In Service"):
                nodeClass = "label label-danger";
                break;

            default:
                nodeClass = "label label-info";

                break;
        }

        return nodeClass;
    }

    function GetNodeIcon(status) {
        var nodeIcon;
        switch (status) {
            case $rootScope.getWord("Available"):
                nodeIcon = "fa fa-check-square-o";
                break;

            case $rootScope.getWord("Occupied"):
                nodeIcon = "fa fa-spinner fa-spin";
                break;

            case $rootScope.getWord("Under Maintenance"):
                nodeIcon = "fa fa-exclamation-circle";
                break;

            case $rootScope.getWord("Not In Service"):
                nodeIcon = "fa fa-warning";
                break;

            default:
                nodeIcon = "fa  fa-question-circle";

                break;
        }

        return nodeIcon;
    }

    function CreateTree(groupName, testStationName, testStationStatus, testStationClass) {
        var treeNodes = new Array();

        var stations = TestStations.stations;

        var groupNames = new Array();
        var found;
        var hasGroups = false;
        var menuItem;
        // create folder items
        if (groupName) {
            for (var i = 0; i < stations.length; i++) {
                var gn = stations[i][groupName];

                found = false;
                for (var j = 0; j < groupNames.length; j++) {
                    if (groupNames[j] === gn) {
                        found = true;
                        break;
                    }
                }

                if (!found) {
                    groupNames.push(gn);
                }
            }

            if (groupNames.length > 0) {
                hasGroups = true;

                // add gorup names as first level menus, they are not clickable
                for (var i = 0; i < groupNames.length; i++) {
                    menuItem = new Object();
                    menuItem.content = "<span><i class=\"fa fa-lg fa-plus-circle\"></i>" + groupNames[i] + "</span>";
                    menuItem.name = groupNames[i];
                    if (i === 0) {
                        menuItem.expanded = true;
                    }
                    else {
                        menuItem.expanded = true;
                    }
                    menuItem.children = new Array();

                    treeNodes.push(menuItem);
                }
            }
        }

        // create item category menu items
        for (var i = 0; i < stations.length; i++) {
            stations[i].checked = false;
            var status = stations[i][testStationStatus];
            if (!status)
            {
                status = "Unknown";
            }
            if (hasGroups) {
                for (var j = 0; j < treeNodes.length; j++) {
                    if (treeNodes[j].name === stations[i][groupName]) {
                        menuItem = new Object();
       
                        menuItem.content = "<span class=\"" + GetNodeClass(status) + "\"><i class=\"" + GetNodeIcon(stations[i][testStationStatus]) + "\" style=\"color:white\"></i>&nbsp;<a class=\"station-a\" href=\"javascript:angular.element(document.getElementById('StationsLayoutCtrl')).scope().OpenDashboard(" + i + ");\">" + stations[i][testStationName] + " (" + status + ")" + "</a></span>";
                        treeNodes[j].children.push(menuItem);
                        break;
                    }
                }
            }
            else {
                menuItem = new Object();
                menuItem.content = "<span class=\"" + GetNodeClass(status) + "\"><i class=\"" + GetNodeIcon(stations[i][testStationStatus]) + "\" style=\"color:white\"></i>&nbsp;<a class=\"station-a\" href=\"javascript:angular.element(document.getElementById('StationsLayoutCtrl')).scope().OpenDashboard(" + i + ");\">" + stations[i][testStationName] + " (" + status + ")" + "</a></span>";

                treeNodes.push(menuItem);
            }
        }

        return treeNodes;
    };

    $scope.OpenDashboard = function OpenDashboard(index) {
        var oid = TestStations.stations[index].obj_id;
        $state.go('app.stations.dashboard', { schema: $stateParams.schema, class: $stateParams.class, oid: oid, xmlschema: TestStations.params['xmlSchemaName'], index: index });
    }
});


'use strict';

angular.module('app.stations').factory('TestStations', function () {

    var TestStationsModel = {
        params: undefined,
        stations: undefined,
        error: "",
        init: function()
        {
            this.params = undefined;
            this.stations = undefined;
            this.error = "";
        }
    };

    return TestStationsModel;
});

/// <reference path='./DlhSoft.Kanban.Angular.Components.ts'/>
var KanbanBoard = DlhSoft.Controls.KanbanBoard;


var nextIteration = { groups: [], items: [] };
angular.module('app.taskkanban').controller('KanbanMainCtrl', function ($http, APP_CONFIG, $scope, $stateParams, $state, kanbanModel, propmisedParams, hubService) {
    // Bind data to the user interface.
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.class;

    var params = propmisedParams.data;
    $scope.itemClass = params['itemClass'];
    $scope.packetClass = params['packetClass'];
    $scope.taskNodeAttribute = params['taskNodeAttribute'];
    $scope.itemNodeAttribute = params['itemNodeAttribute'];
    $scope.packetNodeAttribute = params['packetNodeAttribute'];
    $scope.taskTemplate = params['taskTemplate'];
    $scope.itemTemplate = params['itemTemplate'];
    $scope.packetTemplate = params['packetTemplate'];

    $scope.stateAttribute = params['stateAttribute'];
    $scope.stateMapping = params['stateMapping'];

    $scope.groups = kanbanModel.data.groups;
    $scope.states = kanbanModel.data.states;
    $scope.items = kanbanModel.data.items;

    //console.debug("kanban model = " + JSON.stringify(kanbanModel.data));

    $scope.reload = function(pageIndex)
    {
        var params = new Object();
       
        $state.go($state.current, params, { reload: true }); //second parameter is for $stateParams
    }

    $scope.trackStatusChanged = function (group)
    {
        var groupName = $scope.dbschema + "-" + $scope.dbclass + "-" + group.objId;
        hubService.removeFromGroup(groupName, function () {
            // refresh
            $scope.reload($scope.pageIndex);
        }); // hubService removes the current user from the group
    }

    $scope.gotoItemDetail = function (itemName)
    {
        for (var i = 0; i < $scope.items.length; i++) {
            var item = $scope.items[i];
            if (item.name == itemName) {
                var params = new Object();
                params.schema = $scope.dbschema;
                params.class = $scope.dbclass;
                params.oid = item.objId;
                params.itemClass = $scope.itemClass;
                params.packetClass = $scope.packetClass;
                params.taskNodeAttribute = $scope.taskNodeAttribute;
                params.itemNodeAttribute = $scope.itemNodeAttribute;
                params.packetNodeAttribute = $scope.packetNodeAttribute;
                params.taskTemplate = $scope.taskTemplate;
                params.itemTemplate = $scope.itemTemplate;
                params.packetTemplate = $scope.packetTemplate;
                params.activeTabId = "tasktab";
                $state.go("app.taskviewer.details", params, { reload: true });
            }
        }
    }

    // Handle changes.
    $scope.onItemStateChanged = function (item, boardState) {
        var url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($scope.dbschema) + "/" + encodeURIComponent($scope.dbclass) + "/" + item.objId + "?formformat=false";

        var model = new Object();
        var actualState = GetActualState(boardState.name);
        if (!actualState)
            return;

        model[$scope.stateAttribute] = actualState;

        $scope.loading = true;
        $http.post(url, model)
            .success(function (data) {
                $scope.loading = false;
            })
            .error(function (err) {
                console.debug("error=" + JSON.stringify(err));
                $scope.loading = false;
            });
    };
    $scope.onItemGroupChanged = function (item, group) {
        //console.log('Group of ' + item.name + ' was changed to: ' + group.name);
    };
    // Move items to the next iteration.
    $scope.nextIteration = nextIteration;
    $scope.moveItemToNextIteration = function (type, index) {
        if (type === DlhSoft.Controls.KanbanBoard.types.group) {
            // Move an entire group (story) and all its items.
            var group = groups[index];
            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                if (item.group === group) {
                    items.splice(i--, 1);
                    nextIteration.items.push(item);
                }
            }
            groups.splice(index, 1);
            if (nextIteration.groups.indexOf(group) < 0)
                nextIteration.groups.push(group);
            console.log('Group ' + group.name + ' and its items were moved to next iteration.');
        }
        else {
            // Move a single item, and copy the group (story) if needed.
            var item = items[index];
            items.splice(index, 1);
            nextIteration.items.push(item);
            var group = item.group;
            if (nextIteration.groups.indexOf(group) < 0)
                nextIteration.groups.push(group);
            console.log('Item ' + item.name + ' was moved to next iteration.');
        }
    };

    function GetActualState(displayState) {
        var states = $scope.stateMapping.split(";");
        var actualState = undefined;

        for (var i = 0; i < states.length; ++i) {
            var state = states[i];
            var keyValue = state.split(":");
            var key = keyValue[0];
            var value = keyValue[1];
            if (key === displayState) {
                var actualStates = value.split(",");
                if (actualStates.length > 0) {
                    // use the first state as default
                    actualState = actualStates[0];
                    break;
                }
            }
        }

        return actualState;
    }
});
//# sourceMappingURL=app.js.map
'use strict';

angular.module('app.taskkanban').directive('ngEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    scope.$eval(attrs.ngEnter, { 'event': event });
                });

                event.preventDefault();
            }
        });
    };
});

'use strict';

angular.module('app.taskviewer').controller('ItemFormCtrl', function ($controller, $rootScope, $scope, $http, APP_CONFIG, $state, $stateParams, taskService, $document, $window) {

    // override the dbclass and oid with test item class and test item oid
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.itemClass;
    $scope.oid = $stateParams.itemOid;
    $scope.template = $stateParams.itemTemplate;
    $rootScope.hasItemOid = taskService.hasValue($stateParams.itemOid);

    if ($stateParams.activeTabId == "itemtab") {
        var itemTabElement = $document[0].getElementById('itemtab');
        if (itemTabElement) {
            itemTabElement.click();
        }
    }

    angular.extend(this, $controller('ebaasFormBaseCtrl', { $rootScope: $rootScope, $scope: $scope, $http: $http, APP_CONFIG: APP_CONFIG }));

    $scope.editForm = function () {
        $scope.$broadcast('editParentNodeEvent', {
            parentClass: $scope.dbclass,
            parentObjId: $scope.oid,
            childNodeType: "ItemNode"
        });
    };

    $rootScope.$on('relatedModalFormClosed', function (event, args) {
        $state.reload();
    });

    $scope.$on('addChildNodeEvent', function (e, args) {
        if (args.childNodeType === "ItemNode") {
            $state.go('.relatedform', { schema: $scope.dbschema, masterclass: args.parentClass, masteroid: args.parentObjId, rclass: args.childClass, rtemplate: $scope.template }, { location: false, notify: false });
        }
    });

    $scope.$on('editParentNodeEvent', function (e, args) {
        if (args.childNodeType === "ItemNode") {
            $state.go('.relatedform', { schema: $scope.dbschema, rclass: args.parentClass, roid: args.parentObjId, rtemplate: $scope.template }, { location: false, notify: false });
        }
    });
});
'use strict';

angular.module('app.taskviewer').controller('PacketFormCtrl', function ($controller, $state, $stateParams, $rootScope, $scope, $http, APP_CONFIG, taskService, $document) {
    // override the dbclass and oid with packet class and packet oid
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.packetClass;
    $scope.oid = $stateParams.packetOid;
    $scope.template = $stateParams.packetTemplate;
    $scope.prefix = $stateParams.packetPrefix;

    $rootScope.hasPacketOid = taskService.hasValue($stateParams.packetOid);

    if ($stateParams.activeTabId == "packettab") {
        var itemTabElement = $document[0].getElementById('packettab');
        if (itemTabElement) {
            itemTabElement.click();
        }
    }

    angular.extend(this, $controller('ebaasFormBaseCtrl', { $rootScope: $rootScope, $scope: $scope, $http: $http, APP_CONFIG: APP_CONFIG }));

    $scope.editForm = function () {
        $scope.$broadcast('editParentNodeEvent', {
            parentClass: $scope.dbclass,
            parentObjId: $scope.oid,
            childNodeType: "PacketNode"
        });
    };

    $scope.$on('addChildNodeEvent', function (e, args) {
        if (args.childNodeType === "PacketNode") {
            $state.go('.relatedform', { schema: $scope.dbschema, masterclass: args.parentClass, masteroid: args.parentObjId, rclass: args.childClass, rtemplate: $scope.template }, { location: false, notify: false });
        }
    });

    $scope.$on('editParentNodeEvent', function (e, args) {
        if (args.childNodeType === "PacketNode") {
            $state.go('.relatedform', { schema: $scope.dbschema, rclass: args.parentClass, roid: args.parentObjId, rtemplate: $scope.template }, { location: false, notify: false });
        }
    });
});
'use strict';

angular.module('app.taskviewer').controller('TaskFormCtrl', function ($stateParams, $state, $controller, $rootScope, $scope, $http, APP_CONFIG, $document) {
    if (!$stateParams.activeTabId || $stateParams.activeTabId == "tasktab") {
        var itemTabElement = $document[0].getElementById('tasktab');
        if (itemTabElement) {
            itemTabElement.click();
        }
    }

    $scope.template = $stateParams.taskTemplate;

    angular.extend(this, $controller('ebaasFormBaseCtrl', { $rootScope: $rootScope, $scope: $scope, $http: $http, APP_CONFIG: APP_CONFIG }));

    $scope.editForm = function () {
        $scope.$broadcast('editParentNodeEvent', {
            parentClass: $scope.dbclass,
            parentObjId: $scope.oid,
            childNodeType: "TaskNode"
        });
    };

    $scope.$on('editParentNodeEvent', function (e, args) {
        if (args.childNodeType === "TaskNode") {
            $state.go('.modalform', { schema: $scope.dbschema, class: args.parentClass, oid: args.parentObjId, template: $scope.template }, { location: false, notify: false });
        }
    });
});
"use strict";

angular.module('app.taskviewer').factory('taskService', function ($http, $q, APP_CONFIG) {

    var createTaskTree = function (treeData) {
        var node, rootMenuItem, roots = [];
        var params = {};
        params.index = 0;
        node = treeData;

        var rootMenuItem = {};
        rootMenuItem.content = "<span class='label label-info'><i class=\"fa fa-lg fa-plus-circle\"></i>&nbsp;&nbsp;<a class=\"station-a\" href=\"javascript:angular.element(document.getElementById('taskDataTree')).scope().GoToTaskInfoView('" + node.ClassName + "', '" + node.ID + "', '" + escapePath(node.Prefix) + "');\">" + node.Name + "</a></span>";
        rootMenuItem.index = params.index;
        rootMenuItem.children = [];
        rootMenuItem.expanded = true;
        roots.push(rootMenuItem);

        addChildMenuItems(rootMenuItem, treeData.Children, params);

        return roots;
    };

    var addChildMenuItems = function (parentItem, nodes, params) {
        var node, menuItem;
        if (nodes != null) {
            for (var i = 0; i < nodes.length; i += 1) {
                node = nodes[i];
                params.index++;
                menuItem = {};
                menuItem.children = [];
                menuItem.index = params.index;

                if (node.Children.length > 0) {
                    menuItem.expanded = true;
                    menuItem.content = "<span class='label label-info'><i class=\"fa fa-lg fa-plus-circle\"></i>&nbsp;&nbsp;<a class=\"station-a\" href=\"javascript:angular.element(document.getElementById('taskDataTree')).scope().GoToTaskInfoView('" + node.ClassName + "', '" + node.ID + "', '" + escapePath(node.Prefix) + "');\">" + node.Name + "</a></span>";
                } else {
                    menuItem.content = "<span class='label label-info'><a class=\"station-a\" href=\"javascript:angular.element(document.getElementById('taskDataTree')).scope().GoToTaskInfoView('" + node.ClassName + "', '" + node.ID + "', '" + escapePath(node.Prefix) + "');\">" + node.Name + "</a></span>";
                }

                parentItem.children.push(menuItem);

                addChildMenuItems(menuItem, node.Children, params);
            }
        }
    }

    var escapePath = function (path) {
        if (path) {
            path = path.replaceAll("\\", "\\\\");
        }

        return path;
    }

    var flattenTreeNodes = function (treeNode) {
        var nodes = [];
        var params = {};
        params.index = 0;
        var node = {};
        node.index = params.index;
        node.isTaskNode = true;
        node.className = treeNode.ClassName;
        node.childClass = treeNode.ChildClass;
        node.objId = treeNode.ID;
        node.allowCreate = (treeNode.Children == undefined || treeNode.Children.length == 0) ? true : treeNode.Children[0].AllowCreate;
        node.allowEdit = treeNode.AllowWrite;
        node.allowDelete = treeNode.AllowDelete;
        nodes.push(node);

        flatternChildNodes(nodes, treeNode.Children, params);

        return nodes;
    }

    var flatternChildNodes = function (nodes, childTreeNodes, params) {
        var childTreeNode, node;
        if (childTreeNodes != null) {
            for (var i = 0; i < childTreeNodes.length; i += 1) {
                childTreeNode = childTreeNodes[i];
                params.index++;
                node = {};
                node.index = params.index;
                if (childTreeNode.Type === "TestItem") {
                    node.isItemNode = true;
                }
                else if (childTreeNode.Type === "TestPacket") {
                    node.isPacketNode = true;
                }
                node.className = childTreeNode.ClassName;
                node.childClass = childTreeNode.ChildClass;
                node.objId = childTreeNode.ID;
                node.allowCreate = (childTreeNode.Children == undefined || childTreeNode.Children.length == 0) ? true : childTreeNode.Children[0].AllowCreate;
                node.allowEdit = childTreeNode.AllowWrite;
                node.allowDelete = childTreeNode.AllowDelete;

                nodes.push(node);

                flatternChildNodes(nodes, childTreeNode.Children, params);
            }
        }
    }

    function getTaskTree(parameters, callback) {

        // url to get task instance
        var url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent(parameters.schema) + "/" + parameters.taskClass + "/" + parameters.taskOid + "/custom/GetTaskTree";
        var urlWithParams = url + "?itemClass=" + parameters.itemClass;
        urlWithParams += "&packetClass=" + parameters.packetClass;
        urlWithParams += "&taskNodeAttribute=" + parameters.taskNodeAttribute;
        urlWithParams += "&itemNodeAttribute=" + parameters.itemNodeAttribute;
        urlWithParams += "&packetNodeAttribute=" + parameters.packetNodeAttribute;
        $http.get(urlWithParams).success(function (data) {
            var treeData = data;
            if (callback != null) {
                callback(createTaskTree(treeData),
                    flattenTreeNodes(treeData),
                );
            }
        }).error(function () {
            callback(undefined);
        });
    }

    function deleteTreeNode(parameters, callback) {
        var url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent(parameters.schema) + "/" + parameters.class + "/" + parameters.oid;

        $http.delete(url)
            .success(function (data) {
                callback(parameters.nodeObjId);
            })
            .error(function () {
                callback(undefined);
            });
    }

    function hasValue(val) {
        if (val == null || val == undefined || val == "") {
            return false;
        }
        else {
            return true;
        }
    }
	
	return {
        getTaskTree: function (parameters, callback) {
            return getTaskTree(parameters, callback);
        },
        deleteTreeNode: function (parameters, callback) {
            return deleteTreeNode(parameters, callback);
        },
        hasValue: function (val) {
            return hasValue(val);
        }
	}
});
'use strict';

angular.module('app.taskviewer').controller('TaskViewerLayoutCtrl', function ($rootScope, $scope, $state, $stateParams, taskService, MetaDataCache) {

    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.class;
    $scope.oid = $stateParams.oid;
    $scope.formAttribute = undefined;
    $scope.itemClass = $stateParams.itemClass;
    $scope.itemTemplate = $stateParams.itemTemplate;
    $scope.itemOid = $stateParams.itemOid;
    $scope.packetClass = $stateParams.packetClass;
    $scope.packetOid = $stateParams.packetOid;
    $scope.packetTemplate = $stateParams.packetTemplate;
    $scope.packetPrefix = $stateParams.packetPrefix;
    $scope.taskNodeAttribute = $stateParams.taskNodeAttribute;
    $scope.taskTemplate = $stateParams.taskTemplate;
    $scope.itemNodeAttribute = $stateParams.itemNodeAttribute;
    $scope.packetNodeAttribute = $stateParams.packetNodeAttribute;
    $scope.packetPrefixAttribute = $stateParams.packetPrefixAttribute;

    $rootScope.IsReloaded = false;

    var parameters = {};
    parameters.schema = $stateParams.schema;
    parameters.class = $stateParams.class;
    parameters.oid = $stateParams.oid;
    parameters.taskClass = $stateParams.class;
    parameters.taskOid = $stateParams.oid;
    parameters.taskTemplate = $stateParams.taskTemplate;
    parameters.taskNodeAttribute = $stateParams.taskNodeAttribute;
    parameters.itemClass = $stateParams.itemClass;
    parameters.itemTemplate = $stateParams.itemTemplate;
    parameters.itemNodeAttribute = $stateParams.itemNodeAttribute;
    parameters.packetClass = $stateParams.packetClass;
    parameters.packetNodeAttribute = $stateParams.packetNodeAttribute;
    parameters.packetTemplate = $stateParams.packetTemplate;
    parameters.packetPrefixAttribute = $stateParams.packetPrefixAttribute;

    var treeName = $stateParams.schema + $stateParams.class + $stateParams.oid;
    if (MetaDataCache.getNamedData(treeName)) {
        $scope.taskDataTree = MetaDataCache.getNamedData(treeName);
        $scope.nodes = MetaDataCache.getNamedData(treeName + "-nodes");
    }
    else {
        taskService.getTaskTree(parameters, function (taskTree, flatternNodes) {
            $scope.taskDataTree = taskTree;
            $scope.nodes = flatternNodes;
            MetaDataCache.setNamedData(treeName, taskTree);
            MetaDataCache.setNamedData(treeName + "-nodes", $scope.nodes);
        });
    }

    $state.go("app.taskviewer.details", parameters);

    $scope.GoToTaskInfoView = function GoToTaskInfoView(nodeClass, nodeOid, nodePrefix) {
        var params = new Object();
        params.schema = $scope.dbschema;
        params.class = $scope.dbclass;
        params.oid = $scope.oid;
        params.itemClass = $scope.itemClass;
        params.packetClass = $scope.packetClass;
        params.taskNodeAttribute = $scope.taskNodeAttribute;
        params.itemNodeAttribute = $scope.itemNodeAttribute;
        params.packetNodeAttribute = $scope.packetNodeAttribute;
        params.packetPrefixAttribute = $scope.packetPrefixAttribute;
        params.taskTemplate = $scope.taskTemplate;
        params.itemTemplate = $scope.itemTemplate;
        params.packetTemplate = $scope.packetTemplate;

        if (nodeClass == $scope.itemClass) {
            params.itemOid = nodeOid;
            params.packetOid = null;
            params.packetPrefix = "";
            params.activeTabId = "itemtab";
            $scope.itemOid = nodeOid;
            $scope.packetOid = null;
            $state.go("app.taskviewer.details", params, { reload: true });
        }
        else if (nodeClass == $scope.packetClass) {
            params.packetOid = nodeOid;
            $scope.packetOid = nodeOid;
            $scope.packetPrefix = nodePrefix;
            params.packetPrefix = nodePrefix;
            params.activeTabId = "packettab";
            $state.go("app.taskviewer.details", params, { reload: true });
        }
        else {
            params.itemOid = "";
            params.packetOid = "";
            $scope.itemOid = "";
            $scope.packetOid = "";
            params.activeTabId = "tasktab";
            params.packetPrefix = "";
            $state.go("app.taskviewer.details", params, { reload: true });
        }
    }

    $scope.Refresh = function (reloadTree) {
        if ($rootScope.RefreshTaskTree) {
            var treeName = $stateParams.schema + $stateParams.class + $stateParams.oid;
            MetaDataCache.setNamedData(treeName, null);
            MetaDataCache.setNamedData(treeName + "-nodes", null);
            $rootScope.RefreshTaskTree = false;
        }

        // Hack as workaround to the issue with $state.reload() not working
        $scope.$$postDigest(function () {
            angular.element('#reloadTaskViewer').trigger('click');
        });
    };

    $scope.addNode = function (parentClass, parentObjId, childClass, childNodeType) {
        $scope.$broadcast('addChildNodeEvent', {
            parentClass: parentClass,
            parentObjId: parentObjId,
            childClass: childClass,
            childNodeType: childNodeType
        });

        $rootScope.RefreshTaskTree = true;
    };

    $scope.editNode = function (parentClass, parentObjId, childClass, childObjId, childNodeType) {
        $scope.$broadcast('editParentNodeEvent', {
            parentClass: parentClass,
            parentObjId: parentObjId,
            childClass: childClass,
            childObjId: childObjId,
            childNodeType: childNodeType
        });
    };

    $scope.deleteNode = function (parentClass, parentObjId, childClass, childObjId, childNodeType) {
        $scope.$broadcast('deleteParentNodeEvent', {
            parentClass: parentClass,
            parentObjId: parentObjId,
            childClass: childClass,
            childObjId: childObjId,
            childNodeType: childNodeType
        });
    };

    $rootScope.$on('modalClosed', function (event, data) {
        if (data === "update" && !$rootScope.IsReloaded) {
            event.preventDefault();
            event.stopPropagation();
            $rootScope.IsReloaded = true;
            $scope.Refresh();
        }
    });

    $scope.$on('deleteParentNodeEvent', function (event, args) {
        var result = confirm($rootScope.getWord("Confirm Delete Test Item"));
        if (result) {
            var params = new Object();
            params.schema = $scope.dbschema;
            params.class = args.parentClass;
            params.oid = args.parentObjId;
            taskService.deleteTreeNode(params, function (result) {
                $rootScope.RefreshTaskTree = true;
                $scope.Refresh();
            });
        }
    });
});
'use strict';

angular.module('app.taskviewer')
    .factory('ContextMenuService', function () {
        return {
            element: null,
            menuElement: null
        };
    })
    .directive('contextMenu', [
        '$document',
        'ContextMenuService',
        function ($document, ContextMenuService) {
            return {
                restrict: 'A',
                scope: {
                    'callback': '&contextMenu',
                    'disabled': '&contextMenuDisabled',
                    'closeCallback': '&contextMenuClose',
                    'marginBottom': '@contextMenuMarginBottom'
                },
                link: function ($scope, $element, $attrs) {
                    var opened = false;

                    function open(event, menuElement) {
                        menuElement.addClass('open');

                        var doc = $document[0].documentElement;
                        var docLeft = (window.pageXOffset || doc.scrollLeft) -
                            (doc.clientLeft || 0),
                            docTop = (window.pageYOffset || doc.scrollTop) -
                                (doc.clientTop || 0),
                            elementWidth = menuElement[0].scrollWidth,
                            elementHeight = menuElement[0].scrollHeight;
                        var pageX;
                        var pageY;
                        // browser compatibility fix for the click location
                        if (event.pageX || event.pageY) {
                            // use pageX and pageY when available (modern browsers)
                            pageX = event.pageX;
                            pageY = event.pageY;
                        } else {
                            // calculate pageX and pageY when they do not exist
                            // (IE8 and generated events in later versions of IE)
                            var docBody = $document[0].body;
                            pageX = event.clientX + docBody.scrollLeft + doc.scrollLeft;
                            pageY = event.clientY + docBody.scrollTop + doc.scrollTop;
                        }
                        var docWidth = doc.clientWidth + docLeft,
                            docHeight = doc.clientHeight + docTop,
                            totalWidth = elementWidth + pageX,
                            totalHeight = elementHeight + pageY,
                            left = Math.max(pageX - docLeft, 0),
                            top = Math.max(pageY - docTop, 0);

                        if (totalWidth > docWidth) {
                            left = left - (totalWidth - docWidth);
                        }

                        if (totalHeight > docHeight) {
                            var marginBottom = $scope.marginBottom || 0;
                            top = top - (totalHeight - docHeight) - marginBottom;
                        }

                        menuElement.css('top', top + 'px');
                        menuElement.css('left', left + 'px');
                        opened = true;
                    }

                    function close(menuElement) {
                        menuElement.removeClass('open');

                        if (opened) {
                            $scope.closeCallback();
                        }

                        opened = false;
                    }

                    $element.bind('contextmenu', function (event) {
                        if (!$scope.disabled()) {
                            if (ContextMenuService.menuElement !== null) {
                                close(ContextMenuService.menuElement);
                            }
                            ContextMenuService.menuElement = angular.element(
                                document.getElementById($attrs.target)
                            );
                            ContextMenuService.element = event.target;

                            event.preventDefault();
                            event.stopPropagation();
                            $scope.$apply(function () {
                                $scope.callback({ $event: event });
                            });
                            $scope.$apply(function () {
                                open(event, ContextMenuService.menuElement);
                            });
                        }
                    });

                    function handleKeyUpEvent(event) {
                        if (opened && event.keyCode === 27) {
                            $scope.$apply(function () {
                                close(ContextMenuService.menuElement);
                            });
                        }
                    }

                    function handleClickEvent(event) {
                        if (opened &&
                            (event.button !== 2 ||
                                event.target !== ContextMenuService.element)) {
                            $scope.$apply(function () {
                                close(ContextMenuService.menuElement);
                            });
                        }
                    }

                    $document.bind('keyup', handleKeyUpEvent);
                    // Firefox treats a right-click as a click and a contextmenu event
                    // while other browsers just treat it as a contextmenu event
                    $document.bind('click', handleClickEvent);
                    $document.bind('contextmenu', handleClickEvent);

                    $scope.$on('$destroy', function () {
                        $document.unbind('keyup', handleKeyUpEvent);
                        $document.unbind('click', handleClickEvent);
                        $document.unbind('contextmenu', handleClickEvent);
                    });
                }
            };
        }
    ]);

'use strict';

angular.module('app.taskviewer').directive('taskTreeviewContent', function ($compile) {
    return {
        restrict: 'E',
        link: function (scope, element) {
            var $content = $(scope.item.content);

            function handleExpanded(){
                $content.find('>i')
                    .toggleClass('fa-plus-circle', !scope.item.expanded)
                    .toggleClass('fa-minus-circle', !!scope.item.expanded)

            }


            if (scope.item.children && scope.item.children.length) {
                $content.on('click', function(){
                    scope.$apply(function(){
                        scope.item.expanded = !scope.item.expanded;
                        handleExpanded();
                    });


                });
                handleExpanded();
            }

            element.replaceWith($content);


        }
    }
});

angular.module('app.taskviewer').directive('taskTreeview', function ($compile, $sce) {
    return {
        restrict: 'A',
        scope: {
            'items': '='
        },
        template: '<li ng-class="{parent_li: item.children.length}" ng-repeat="item in items" role="treeitem">' +
            '<div context-menu data-target="menu-{{ item.index }}">' +
            '<task-treeview-content></task-treeview-content>' +
            '</div>' +
            '<ul ng-if="item.children.length" task-treeview ng-show="item.expanded"  items="item.children" role="group" class="smart-treeview-group" ></ul>' +
            '</li>',
        compile: function (element) {
            // Break the recursion loop by removing the contents
            var contents = element.contents().remove();
            var compiledContents;
            return {
                post: function (scope, element) {
                    // Compile the contents
                    if (!compiledContents) {
                        compiledContents = $compile(contents);
                    }
                    // Re-add the compiled contents to the element
                    compiledContents(scope, function (clone) {
                        element.append(clone);
                    });
                }
            };
        }
    };
});
"use strict";

angular.module('app.ui').controller('GeneralElementsCtrl', function ($scope) {
    /*
     * Smart Notifications
     */
    $scope.eg1 = function () {

        $.bigBox({
            title: "Big Information box",
            content: "This message will dissapear in 6 seconds!",
            color: "#C46A69",
            //timeout: 6000,
            icon: "fa fa-warning shake animated",
            number: "1",
            timeout: 6000
        });
    };

    $scope.eg2 = function () {

        $.bigBox({
            title: "Big Information box",
            content: "Lorem ipsum dolor sit amet, test consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam",
            color: "#3276B1",
            //timeout: 8000,
            icon: "fa fa-bell swing animated",
            number: "2"
        });

    };

    $scope.eg3 = function () {

        $.bigBox({
            title: "Shield is up and running!",
            content: "Lorem ipsum dolor sit amet, test consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam",
            color: "#C79121",
            //timeout: 8000,
            icon: "fa fa-shield fadeInLeft animated",
            number: "3"
        });

    };

    $scope.eg4 = function () {

        $.bigBox({
            title: "Success Message Example",
            content: "Lorem ipsum dolor sit amet, test consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam",
            color: "#739E73",
            //timeout: 8000,
            icon: "fa fa-check",
            number: "4"
        }, function () {
            $scope.closedthis();
        });

    };


    $scope.eg5 = function() {

        $.smallBox({
            title: "Ding Dong!",
            content: "Someone's at the door...shall one get it sir? <p class='text-align-right'><a href-void class='btn btn-primary btn-sm'>Yes</a> <a href-void class='btn btn-danger btn-sm'>No</a></p>",
            color: "#296191",
            //timeout: 8000,
            icon: "fa fa-bell swing animated"
        });
    };


    $scope.eg6 = function() {

        $.smallBox({
            title: "Big Information box",
            content: "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam",
            color: "#5384AF",
            //timeout: 8000,
            icon: "fa fa-bell"
        });

    };

    $scope.eg7 = function() {

        $.smallBox({
            title: "James Simmons liked your comment",
            content: "<i class='fa fa-clock-o'></i> <i>2 seconds ago...</i>",
            color: "#296191",
            iconSmall: "fa fa-thumbs-up bounce animated",
            timeout: 4000
        });

    };

    $scope.closedthis = function() {
        $.smallBox({
            title: "Great! You just closed that last alert!",
            content: "This message will be gone in 5 seconds!",
            color: "#739E73",
            iconSmall: "fa fa-cloud",
            timeout: 5000
        });
    };

    /*
     * SmartAlerts
     */
    // With Callback
    $scope.smartModEg1 =  function () {
        $.SmartMessageBox({
            title: "Smart Alert!",
            content: "This is a confirmation box. Can be programmed for button callback",
            buttons: '[No][Yes]'
        }, function (ButtonPressed) {
            if (ButtonPressed === "Yes") {

                $.smallBox({
                    title: "Callback function",
                    content: "<i class='fa fa-clock-o'></i> <i>You pressed Yes...</i>",
                    color: "#659265",
                    iconSmall: "fa fa-check fa-2x fadeInRight animated",
                    timeout: 4000
                });
            }
            if (ButtonPressed === "No") {
                $.smallBox({
                    title: "Callback function",
                    content: "<i class='fa fa-clock-o'></i> <i>You pressed No...</i>",
                    color: "#C46A69",
                    iconSmall: "fa fa-times fa-2x fadeInRight animated",
                    timeout: 4000
                });
            }

        });
    };

    // With Input
    $scope.smartModEg2 =  function () {
        $.SmartMessageBox({
            title: "Smart Alert: Input",
            content: "Please enter your user name",
            buttons: "[Accept]",
            input: "text",
            placeholder: "Enter your user name"
        }, function (ButtonPress, Value) {
            alert(ButtonPress + " " + Value);
        });
    };

    // With Buttons
    $scope.smartModEg3 =  function () {
        $.SmartMessageBox({
            title: "Smart Notification: Buttons",
            content: "Lots of buttons to go...",
            buttons: '[Need?][You][Do][Buttons][Many][How]'
        });

    }
    // With Select
    $scope.smartModEg4 =  function () {
        $.SmartMessageBox({
            title: "Smart Alert: Select",
            content: "You can even create a group of options.",
            buttons: "[Done]",
            input: "select",
            options: "[Costa Rica][United States][Autralia][Spain]"
        }, function (ButtonPress, Value) {
            alert(ButtonPress + " " + Value);
        });

    };

    // With Login
    $scope.smartModEg5 =  function () {

        $.SmartMessageBox({
            title: "Login form",
            content: "Please enter your user name",
            buttons: "[Cancel][Accept]",
            input: "text",
            placeholder: "Enter your user name"
        }, function (ButtonPress, Value) {
            if (ButtonPress == "Cancel") {
                alert("Why did you cancel that? :(");
                return 0;
            }

            var Value1 = Value.toUpperCase();
            var ValueOriginal = Value;
            $.SmartMessageBox({
                title: "Hey! <strong>" + Value1 + ",</strong>",
                content: "And now please provide your password:",
                buttons: "[Login]",
                input: "password",
                placeholder: "Password"
            }, function (ButtonPress, Value) {
                alert("Username: " + ValueOriginal + " and your password is: " + Value);
            });
        });

    };
});

"use strict";


angular.module('app.ui').controller('JquiCtrl', function ($scope) {
    $scope.demoAutocompleteWords = [
        "ActionScript",
        "AppleScript",
        "Asp",
        "BASIC",
        "C",
        "C++",
        "Clojure",
        "COBOL",
        "ColdFusion",
        "Erlang",
        "Fortran",
        "Groovy",
        "Haskell",
        "Java",
        "JavaScript",
        "Lisp",
        "Perl",
        "PHP",
        "Python",
        "Ruby",
        "Scala",
        "Scheme"];


    $scope.demoAjaxAutocomplete = '';


    $scope.modalDemo1 = function(){
        console.log('modalDemo1');
    }

    $scope.modalDemo2 = function(){
        console.log('modalDemo2');
    }


});
"use strict";


angular.module('app.ui').controller('TreeviewCtrl', function ($scope) {
    $scope.demoTree1 = [
        {"content": "<span><i class=\"fa fa-lg fa-calendar\"></i> 2013, Week 2</span>", "expanded": true, "children": [
            {"content": "<span class=\"label label-success\"><i class=\"fa fa-lg fa-plus-circle\"></i> Monday, January 7: 8.00 hours</span>", "expanded": true, "children": [
                {"content": "<span><i class=\"fa fa-clock-o\"></i> 8.00</span> &ndash; <a> Changed CSS to accomodate...</a>"}
            ]},
            {"content": "<span><i class=\"fa fa-clock-o\"></i> 8.00</span> &ndash; <a> Changed CSS to accomodate...</a>"},
            {"content": "<span class=\"label label-success\"><i class=\"fa fa-lg fa-minus-circle\"></i> Tuesday, January 8: 8.00 hours</span>", "expanded": true, "children": [
                {"content": "<span><i class=\"fa fa-clock-o\"></i> 6.00</span> &ndash; <a> Altered code...</a>"},
                {"content": "<span><i class=\"fa fa-clock-o\"></i> 2.00</span> &ndash; <a> Simplified our approach to...</a>"}
            ]},
            {"content": "<span><i class=\"fa fa-clock-o\"></i> 6.00</span> &ndash; <a> Altered code...</a>"},
            {"content": "<span><i class=\"fa fa-clock-o\"></i> 2.00</span> &ndash; <a> Simplified our approach to...</a>"},
            {"content": "<span class=\"label label-warning\"><i class=\"fa fa-lg fa-minus-circle\"></i> Wednesday, January 9: 6.00 hours</span>", "children": [
                {"content": "<span><i class=\"fa fa-clock-o\"></i> 3.00</span> &ndash; <a> Fixed bug caused by...</a>"},
                {"content": "<span><i class=\"fa fa-clock-o\"></i> 3.00</span> &ndash; <a> Comitting latest code to Git...</a>"}
            ]},
            {"content": "<span><i class=\"fa fa-clock-o\"></i> 3.00</span> &ndash; <a> Fixed bug caused by...</a>"},
            {"content": "<span><i class=\"fa fa-clock-o\"></i> 3.00</span> &ndash; <a> Comitting latest code to Git...</a>"},
            {"content": "<span class=\"label label-danger\"><i class=\"fa fa-lg fa-minus-circle\"></i> Wednesday, January 9: 4.00 hours</span>", "children": [
                {"content": "<span><i class=\"fa fa-clock-o\"></i> 2.00</span> &ndash; <a> Create component that...</a>"}
            ]},
            {"content": "<span><i class=\"fa fa-clock-o\"></i> 2.00</span> &ndash; <a> Create component that...</a>"}
        ]},
        {"content": "<span><i class=\"fa fa-lg fa-calendar\"></i> 2013, Week 3</span>", "children": [
            {"content": "<span class=\"label label-success\"><i class=\"fa fa-lg fa-minus-circle\"></i> Monday, January 14: 8.00 hours</span>", "children": [
                {"content": "<span><i class=\"fa fa-clock-o\"></i> 7.75</span> &ndash; <a> Writing documentation...</a>"},
                {"content": "<span><i class=\"fa fa-clock-o\"></i> 0.25</span> &ndash; <a> Reverting code back to...</a>"}
            ]},
            {"content": "<span><i class=\"fa fa-clock-o\"></i> 7.75</span> &ndash; <a> Writing documentation...</a>"},
            {"content": "<span><i class=\"fa fa-clock-o\"></i> 0.25</span> &ndash; <a> Reverting code back to...</a>"}
        ]}
    ]

    $scope.demoTree2 = [
        {"content": "<span><i class=\"fa fa-lg fa-folder-open\"></i> Parent</span>", "expanded": true, "children": [
            {"content": "<span><i class=\"fa fa-lg fa-plus-circle\"></i> Administrators</span>", "expanded": true, "children": [
                {"content": "<span> <label class=\"checkbox inline-block\"><input type=\"checkbox\" name=\"checkbox-inline\"><i></i>Michael.Jackson</label> </span>"},
                {"content": "<span> <label class=\"checkbox inline-block\"><input type=\"checkbox\" checked=\"checked\" name=\"checkbox-inline\"><i></i>Sunny.Ahmed</label> </span>"},
                {"content": "<span> <label class=\"checkbox inline-block\"><input type=\"checkbox\" checked=\"checked\" name=\"checkbox-inline\"><i></i>Jackie.Chan</label> </span>"}
            ]},
            {"content": "<span> <label class=\"checkbox inline-block\"><input type=\"checkbox\" name=\"checkbox-inline\"><i></i>Michael.Jackson</label> </span>"},
            {"content": "<span> <label class=\"checkbox inline-block\"><input type=\"checkbox\" checked=\"checked\" name=\"checkbox-inline\"><i></i>Sunny.Ahmed</label> </span>"},
            {"content": "<span> <label class=\"checkbox inline-block\"><input type=\"checkbox\" checked=\"checked\" name=\"checkbox-inline\"><i></i>Jackie.Chan</label> </span>"},
            {"content": "<span><i class=\"fa fa-lg fa-minus-circle\"></i> Child</span>", "expanded": true, "children": [
                {"content": "<span><i class=\"icon-leaf\"></i> Grand Child</span>"},
                {"content": "<span><i class=\"icon-leaf\"></i> Grand Child</span>"},
                {"content": "<span><i class=\"fa fa-lg fa-plus-circle\"></i> Grand Child</span>",  "children": [
                    {"content": "<span><i class=\"fa fa-lg fa-plus-circle\"></i> Great Grand Child</span>", "children": [
                        {"content": "<span><i class=\"icon-leaf\"></i> Great great Grand Child</span>"},
                        {"content": "<span><i class=\"icon-leaf\"></i> Great great Grand Child</span>"}
                    ]},
                    {"content": "<span><i class=\"icon-leaf\"></i> Great great Grand Child</span>"},
                    {"content": "<span><i class=\"icon-leaf\"></i> Great great Grand Child</span>"},
                    {"content": "<span><i class=\"icon-leaf\"></i> Great Grand Child</span>"},
                    {"content": "<span><i class=\"icon-leaf\"></i> Great Grand Child</span>"}
                ]},
                {"content": "<span><i class=\"fa fa-lg fa-plus-circle\"></i> Great Grand Child</span>", "children": [
                    {"content": "<span><i class=\"icon-leaf\"></i> Great great Grand Child</span>"},
                    {"content": "<span><i class=\"icon-leaf\"></i> Great great Grand Child</span>"}
                ]},
                {"content": "<span><i class=\"icon-leaf\"></i> Great great Grand Child</span>"},
                {"content": "<span><i class=\"icon-leaf\"></i> Great great Grand Child</span>"},
                {"content": "<span><i class=\"icon-leaf\"></i> Great Grand Child</span>"},
                {"content": "<span><i class=\"icon-leaf\"></i> Great Grand Child</span>"}
            ]},
            {"content": "<span><i class=\"icon-leaf\"></i> Grand Child</span>"},
            {"content": "<span><i class=\"icon-leaf\"></i> Grand Child</span>"},
            {"content": "<span><i class=\"fa fa-lg fa-plus-circle\"></i> Grand Child</span>", "children": [
                {"content": "<span><i class=\"fa fa-lg fa-plus-circle\"></i> Great Grand Child</span>", "children": [
                    {"content": "<span><i class=\"icon-leaf\"></i> Great great Grand Child</span>"},
                    {"content": "<span><i class=\"icon-leaf\"></i> Great great Grand Child</span>"}
                ]},
                {"content": "<span><i class=\"icon-leaf\"></i> Great great Grand Child</span>"},
                {"content": "<span><i class=\"icon-leaf\"></i> Great great Grand Child</span>"},
                {"content": "<span><i class=\"icon-leaf\"></i> Great Grand Child</span>"},
                {"content": "<span><i class=\"icon-leaf\"></i> Great Grand Child</span>"}
            ]},
            {"content": "<span><i class=\"fa fa-lg fa-plus-circle\"></i> Great Grand Child</span>", "children": [
                {"content": "<span><i class=\"icon-leaf\"></i> Great great Grand Child</span>"},
                {"content": "<span><i class=\"icon-leaf\"></i> Great great Grand Child</span>"}
            ]},
            {"content": "<span><i class=\"icon-leaf\"></i> Great great Grand Child</span>"},
            {"content": "<span><i class=\"icon-leaf\"></i> Great great Grand Child</span>"},
            {"content": "<span><i class=\"icon-leaf\"></i> Great Grand Child</span>"},
            {"content": "<span><i class=\"icon-leaf\"></i> Great Grand Child</span>"}
        ]},
        {"content": "<span><i class=\"fa fa-lg fa-folder-open\"></i> Parent2</span>", "children": [
            {"content": "<span><i class=\"icon-leaf\"></i> Child</span>"}
        ]}
    ]
});
'use strict';

angular.module('app.ui').directive('smartClassFilter', function () {
    return {
        restrict: 'A',
        scope: {
            model: '=',
            displayElements: '@',
            filterElements: '@'
        },
        link: function (scope, element) {
            scope.$watch('model', function (model) {
                if (angular.isString(model)) {
                    var search = model.trim();
                    if (search) {
                        angular.element(scope.displayElements, element).hide();

                        angular.element(scope.filterElements, element)
                            .filter(function () {
                                var r = new RegExp(search, 'i');
                                return r.test($(this).attr('class') + $(this).attr('alt'))
                            })
                            .closest(scope.displayElements).show();
                    } else {
                        angular.element(scope.displayElements, element).show();
                    }
                }
            })
        }
    }
});

"use strict";

angular.module('app.ui').directive('smartHtmlPopoverPopup', function () {
    return {
        restrict: "EA",
        replace: true,
        scope: { title: "@", content: "@", placement: "@", animation: "&", isOpen: "&" },
        template: '<div class="popover {{placement}}" ng-class="{ in: isOpen(), fade: animation() }"><div class="arrow"></div><div class="popover-inner"><h3 class="popover-title" bind-html-unsafe="title" ng-show="title"></h3><div class="popover-content" bind-html-unsafe="content"></div></div></div>'
    };
});
angular.module('app.ui').directive('smartHtmlPopover',function ($tooltip) {
    return $tooltip("smartHtmlPopover", "popover", "click");
});
'use strict';

angular.module('app.ui').directive('smartJquiAccordion', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attributes) {

            element.accordion({
                autoHeight : false,
                heightStyle : "content",
                collapsible : true,
                animate : 300,
                icons: {
                    header: "fa fa-plus",    // custom icon class
                    activeHeader: "fa fa-minus" // custom icon class
                },
                header : "h4"
            })
        }
    }
});

'use strict';

angular.module('app.ui').directive('smartJquiAjaxAutocomplete', function () {
    return {
        restrict: 'A',
        scope: {
            ngModel: '='
        },
        link: function (scope, element, attributes) {
            function split(val) {
                return val.split(/,\s*/);
            }

            function extractLast(term) {
                return split(term).pop();
            }

            function extractFirst(term) {
                return split(term)[0];
            }


            element.autocomplete({
                source: function (request, response) {
                    jQuery.getJSON(
                            "http://gd.geobytes.com/AutoCompleteCity?callback=?&q=" + extractLast(request.term),
                        function (data) {
                            response(data);
                        }
                    );
                },
                minLength: 3,
                select: function (event, ui) {
                    var selectedObj = ui.item,
                    placeName = selectedObj.value;
                    if (typeof placeName == "undefined") placeName = element.val();

                    if (placeName) {
                        var terms = split(element.val());
                        // remove the current input
                        terms.pop();
                        // add the selected item (city only)
                        terms.push(extractFirst(placeName));
                        // add placeholder to get the comma-and-space at the end
                        terms.push("");

                        scope.$apply(function(){
                            scope.ngModel = terms.join(", ")
                        });
                    }

                    return false;
                },
                focus: function() {
                    // prevent value inserted on focus
                    return false;
                },
                delay: 100
            });
        }
    }
});
'use strict';

angular.module('app.ui').directive('smartJquiAutocomplete', function () {
    return {
        restrict: 'A',
        scope: {
            'source': '='
        },
        link: function (scope, element, attributes) {

            element.autocomplete({
                source: scope.source
            });
        }
    }
});
'use strict';

/*
 * CONVERT DIALOG TITLE TO HTML
 * REF: http://stackoverflow.com/questions/14488774/using-html-in-a-dialogs-title-in-jquery-ui-1-10
 */
$.widget("ui.dialog", $.extend({}, $.ui.dialog.prototype, {
    _title: function (title) {
        if (!this.options.title) {
            title.html("&#160;");
        } else {
            title.html(this.options.title);
        }
    }
}));


angular.module('app.ui').directive('smartJquiDialog', function () {

    var optionAttributes = ['autoOpen', 'modal', 'width', 'resizable'];

    var defaults = {
        width: Math.min($(window).width() * .7, 600),
        autoOpen: false,
        resizable: false
    };


    return {
        restrict: 'A',
        link: function (scope, element, attributes) {

            var title = element.find('[data-dialog-title]').remove().html();

            var options = _.clone(defaults);

            optionAttributes.forEach(function (option) {
                if (element.data(option)) {
                    options[option] = element.data(option);
                }
            });

            var buttons = element.find('[data-dialog-buttons]').remove()
                .find('button').map(function (idx, button) {
                    return {
                        class: button.className,
                        html: button.innerHTML,
                        click: function () {
                            if ($(button).data('action'))
                                scope.$eval($(button).data('action'));
                            element.dialog("close");
                        }
                    }
                });

            element.dialog(_.extend({
                title: title,
                buttons: buttons
            }, options));

        }
    }
});
'use strict';

//    $.widget("ui.dialog", $.extend({}, $.ui.dialog.prototype, {
//        _title: function (title) {
//            if (!this.options.title) {
//                title.html("&#160;");
//            } else {
//                title.html(this.options.title);
//            }
//        }
//    }));


angular.module('app.ui').directive('smartJquiDialogLauncher', function () {
    return {
        restrict: 'A',
        compile: function (element, attributes) {
            element.removeAttr('smart-jqui-dialog-launcher data-smart-jqui-dialog-launcher');
            element.on('click', function (e) {
                $(attributes.smartJquiDialogLauncher).dialog('open');
                e.preventDefault();
            })
        }
    }
});
'use strict';

angular.module('app.ui').directive('smartJquiDynamicTabs', function ($timeout) {

	
	function addDomEvents(element){

		$('#tabs2').tabs();

		var tabTitle = $("#tab_title"), tabContent = $("#tab_content"), tabTemplate = "<li style='position:relative;'> <span class='air air-top-left delete-tab' style='top:7px; left:7px;'><button class='btn btn-xs font-xs btn-default hover-transparent'><i class='fa fa-times'></i></button></span></span><a href='#{href}'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; #{label}</a></li>", tabCounter = 2;

		var tabs = $('#tabs2').tabs();

		// modal dialog init: custom buttons and a "close" callback reseting the form inside
		var dialog = $("#addtab").dialog({
			autoOpen : false,
			width : 600,
			resizable : false,
			modal : true,
			buttons : [{
			html : "<i class='fa fa-times'></i>&nbsp; Cancel",
			"class" : "btn btn-default",
			click : function() {
			$(this).dialog("close");

		}
		}, {

			html : "<i class='fa fa-plus'></i>&nbsp; Add",
			"class" : "btn btn-danger",
			click : function() {
				addTab();
				$(this).dialog("close");
			}
		}]
		});

		// addTab form: calls addTab function on submit and closes the dialog
		var form = dialog.find("form").submit(function(event) {
			addTab();
			dialog.dialog("close");
			event.preventDefault();
		});

		// actual addTab function: adds new tab using the input from the form above
		function addTab() {
			var label = tabTitle.val() || "Tab " + tabCounter, id = "tabs-" + tabCounter, li = $(tabTemplate.replace(/#\{href\}/g, "#" + id).replace(/#\{label\}/g, label)), tabContentHtml = tabContent.val() || "Tab " + tabCounter + " content.";

			tabs.find(".ui-tabs-nav").append(li);
			tabs.append("<div id='" + id + "'><p>" + tabContentHtml + "</p></div>");
			tabs.tabs("refresh");
			tabCounter++;

			// clear fields
			$("#tab_title").val("");
			$("#tab_content").val("");
		}

		// addTab button: just opens the dialog
		$("#add_tab").button().click(function() {
			dialog.dialog("open");
		});

		// close icon: removing the tab on click
		$("#tabs2").on("click", 'span.delete-tab', function() {

			var panelId = $(this).closest("li").remove().attr("aria-controls");
			$("#" + panelId).remove();
			tabs.tabs("refresh");

		});

	}

	function link(element){

		$timeout(function(){
			addDomEvents(element);
		});

	}


    return {
        restrict: 'A',
        link: link
    }
});

'use strict';

angular.module('app.ui').directive('smartJquiMenu', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attributes) {

            element.menu();
        }
    }
});
'use strict';

angular.module('app.ui').directive('smartJquiTabs', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attributes) {

            element.tabs();
        }
    }
});
'use strict';

angular.module('app.ui').directive('smartNestable', function () {
    return {
        restrict: 'A',
        scope: {
            group: '@',
            output: '='
        },
        link: function (scope, element, attributes) {
            var options = {};
            if(scope.group){
                options.group = scope.group;
            }
            element.nestable(options);
            if(attributes.output){
                element.on('change', function(){
                    scope.$apply(function(){
                        scope.output = element.nestable('serialize');
                    });
                });
                scope.output = element.nestable('serialize');
            }

        }
    }
});
'use strict';

angular.module('app.ui').directive('smartProgressbar', function (lazyScript) {
    return {
        restrict: 'A',
        compile: function (tElement, tAttributes) {
        	lazyScript.register('bootstrap-progressbar').then(function(){
        		tElement.removeAttr('smart-progressbar data-smart-progressbar');
        		tElement.progressbar({
        		    display_text : 'fill'
        		})
        	})

        }
    }
});
'use strict';

angular.module('app.ui').directive('smartRideCarousel', function () {
    return {
        restrict: 'A',
        compile: function (tElement, tAttributes) {
            tElement.removeAttr('smart-ride-carousel data-smart-ride-carousel');
            tElement.carousel(tElement.data());
        }
    }
});
'use strict';

angular.module('app.ui').directive('smartSuperBox', function () {
    return {
        restrict: 'A',
        compile: function (tElement, tAttributes) {

            tElement.removeAttr('smart-super-box data-smart-super-box');

            tElement.SuperBox();
        }
    }
});
'use strict';

angular.module('app.ui').directive('smartTreeviewContent', function ($compile) {
    return {
        restrict: 'E',
        link: function (scope, element) {
            var $content = $(scope.item.content);

            function handleExpanded(){
                $content.find('>i')
                    .toggleClass('fa-plus-circle', !scope.item.expanded)
                    .toggleClass('fa-minus-circle', !!scope.item.expanded)

            }


            if (scope.item.children && scope.item.children.length) {
                $content.on('click', function(){
                    scope.$apply(function(){
                        scope.item.expanded = !scope.item.expanded;
                        handleExpanded();
                    });


                });
                handleExpanded();
            }

            element.replaceWith($content);


        }
    }
});

angular.module('app.ui').directive('smartTreeview', function ($compile, $sce) {
    return {
        restrict: 'A',
        scope: {
            'items': '='
        },
        template: '<li ng-class="{parent_li: item.children.length}" ng-repeat="item in items" role="treeitem">' +
            '<smart-treeview-content></smart-treeview-content>' +
            '<ul ng-if="item.children.length" smart-treeview ng-show="item.expanded"  items="item.children" role="group" class="smart-treeview-group" ></ul>' +
            '</li>',
        compile: function (element) {
            // Break the recursion loop by removing the contents
            var contents = element.contents().remove();
            var compiledContents;
            return {
                post: function (scope, element) {
                    // Compile the contents
                    if (!compiledContents) {
                        compiledContents = $compile(contents);
                    }
                    // Re-add the compiled contents to the element
                    compiledContents(scope, function (clone) {
                        element.append(clone);
                    });
                }
            };
        }
    };
});
"use strict";

angular.module('app.user').controller('ChangePasswordController', function ($http, $scope, $rootScope, User, APP_CONFIG) {
        $scope.savedSuccessfully = false;
        $scope.message = "";

        $scope.registration = {
            userName: User.userName,
            password: "",
            newPassword: "",
            confirmNewPassword: ""
        };

        $scope.save = function () {
 
            if ($scope.registration.newPassword != $scope.registration.confirmNewPassword)
            {
                $scope.savedSuccessfully = false;
                $scope.message = $rootScope.getWord("ConfirmPasswordIncorrect");
            }
            else if ($scope.registration.newPassword.length < 3)
            {
                $scope.savedSuccessfully = false;
                $scope.message = $rootScope.getWord("NewPasswordInvalid");
            }
            else
            {
                var model = {};
                model.oldPassword = $scope.registration.password;
                model.newPassword = $scope.registration.newPassword;
                model.confirmPassword = $scope.registration.confirmNewPassword;
                $http.post(APP_CONFIG.ebaasRootUrl + '/api/accounts/ChangePassword', model).success(function (data) {
                    $scope.message = $rootScope.getWord("PasswordUpdated");
                    $scope.savedSuccessfully = true;
                })
                .error(function (err) {
                    console.log(err);
                    $scope.message = err.message;
                    $scope.savedSuccessfully = false;
                });
            }
        };

    });
"use strict";

angular.module('app.user').controller('EditProfileController', function ($http, $scope, $rootScope, User, APP_CONFIG) {
        $scope.savedSuccessfully = false;
        $scope.message = "";

        $scope.profile = User;

        $scope.save = function () {

            User.save(function () {
                $scope.message = $rootScope.getWord("ProfileUpdated");
                $scope.savedSuccessfully = true;
            })
        }

    });
'use strict';

angular.module('app.userdirectory').controller('assignRolesCtrl', function ($scope, $controller, $rootScope, $http, APP_CONFIG, $stateParams, $modalInstance) {
    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = "Role";
    $scope.masterclass = $stateParams.class;
    $scope.masterid = $stateParams.oid;
    $scope.roletype = $stateParams.roletype;

    if ($stateParams.dataview)
    {
        $scope.view = $stateParams.dataview;
    }
    else
    {
        $scope.view = undefined;
    }

    $scope.filter = "['RType', '=', '" + $scope.roletype + "']";

    angular.extend(this, $controller('dataGridBaseCtrl', { $scope: $scope, $rootScope: $rootScope, $http: $http, APP_CONFIG: APP_CONFIG }));

    $scope.gridInstance = null;
    $scope.existingKeys = null;
    $scope.currentKeys = null;
    $scope.isUpdated = false;
    $scope.loading = false;
    $scope.dataGridSettings = {
        dataSource: {
            store: $scope.customStore
        },
        columnAutoWidth: true,
        sorting: {
            mode: "multiple"
        },
        editing: {
            allowAdding: false,
            allowUpdating: false,
            allowDeleting: false
        },
        grouping: {
            autoExpandAll: false
        },
        pager: {
            visible: true,
            showPageSizeSelector: true,
            showInfo: true
        },
        filterRow: {
            visible: true,
            applyFilter: "auto"
        },
        searchPanel: { visible: false },
        selection: { mode: 'multiple' },
        remoteOperations: true,
        bindingOptions: {
            columns: 'columns'
        },
        headerFilter: {
            visible: true
        },
        rowAlternationEnabled: true,
        onInitialized: function (e) {
            $scope.gridInstance = e.component;
        },
        onContentReady: function (e) {
            selectGridRows();
        },
        onSelectionChanged: function (e) {
            changeSelections(e.currentSelectedRowKeys, e.currentDeselectedRowKeys);
        }
    };

    var asyncLoop = function(o)
    {
        var i = -1;

        var loop = function() {
            i++;
            if (i == o.length)
            {
                o.callback();
                return;
            }

            o.functionToLoop(loop, i);
        }

        loop(); // init
    }

    var selectGridRows = function()
    {
        if ($scope.currentKeys) {
            var keys = $scope.currentKeys;
            var indexes = [];
            for (var i = 0; i < keys.length; i++) {
                var index = $scope.gridInstance.getRowIndexByKey(keys[i]);

                if (index >= 0)
                    indexes.push(index);
            }

            $scope.gridInstance.selectRowsByIndexes(indexes, true);
        }
        else {
            if ($scope.masterid) {
                $http.get(APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($stateParams.schema) + "/" + $scope.masterclass + "/" + $scope.masterid + "/" + $scope.dbclass)
                    .success(function (data) {
                        var keys = new Array();
                        if (data) {
                            for (var i = 0; i < data.length; i++) {
                                keys.push(data[i].obj_id);
                            }

                            if (keys.length > 0) {
                                // set the existing selections of rows
                                $scope.gridInstance.selectRows(keys, false);
                            }
                        }

                        $scope.existingKeys = keys; // keep the existing keys
                        // initialize the current keys
                        $scope.currentKeys = [];
                        for (var i = 0; i < keys.length; i++) {
                            $scope.currentKeys.push(keys[i]);
                        }
                    });
            }
        }
    }

    // keep the current keys in sync with grid row selections
    var changeSelections = function (selectedKeys, deselectedKeys) {
        var addedKeys = new Array();
        var removedKeys = new Array();
        var found;

        // find the newly selected keys
        if (selectedKeys && $scope.currentKeys) {
            for (var i = 0; i < selectedKeys.length; i++) {
                found = false;

                var index = $scope.gridInstance.getRowIndexByKey(selectedKeys[i]);
                if (index > -1) {
                    for (var j = 0; j < $scope.currentKeys.length; j++) {
                        if (selectedKeys[i] === $scope.currentKeys[j]) {
                            found = true;
                            break;
                        }
                    }
                }

                if (!found) {
                    addedKeys.push(selectedKeys[i]);
                }
            }
        }

        if (deselectedKeys && $scope.currentKeys) {
            // find the unselected keys
            for (var i = 0; i < $scope.currentKeys.length; i++) {
                found = false;

                for (var j = 0; j < deselectedKeys.length; j++) {
                    var index = $scope.gridInstance.getRowIndexByKey(deselectedKeys[j]);
                    if (index > -1) {
                        if ($scope.currentKeys[i] === deselectedKeys[j]) {
                            found = true;
                            break;
                        }
                    }
                }

                if (found) {
                    removedKeys.push($scope.currentKeys[i]);
                }
            }
        }

        for (var i = 0; i < addedKeys.length; i++) {
            $scope.currentKeys.push(addedKeys[i]);
        }

        for (var i = 0; i < removedKeys.length; i++) {
            var index = $scope.currentKeys.indexOf(removedKeys[i]);
            if (index > -1)
                $scope.currentKeys.splice(index, 1);
        }

        //console.log("after current keys = " + $scope.currentKeys);
    }

    $scope.saveSelection = function () {
        var addedKeys = new Array();
        var removedKeys = new Array();
        var found;

        // find the added selections
        for (var i = 0; i < $scope.currentKeys.length; i++) {
            found = false;

            for (var j = 0; j < $scope.existingKeys.length; j++) {
                if ($scope.currentKeys[i] === $scope.existingKeys[j]) {
                    found = true;
                    break;
                }
            }

            if (!found) {
                addedKeys.push($scope.currentKeys[i]);
            }
        }

        // find the removed selections
        for (var i = 0; i < $scope.existingKeys.length; i++) {
            found = false;

            for (var j = 0; j < $scope.currentKeys.length; j++) {
                if ($scope.existingKeys[i] === $scope.currentKeys[j]) {
                    found = true;
                    break;
                }
            }

            if (!found) {
                removedKeys.push($scope.existingKeys[i]);
            }
        }

        if (addedKeys.length > 0) {
            $scope.loading = true;

            // add relationhsips to the db
            asyncLoop({
                length: addedKeys.length,
                functionToLoop: function (loop, i) {
                    if ($scope.masterid) {
                        $http.post(APP_CONFIG.ebaasRootUrl + "/api/relationship/" + encodeURIComponent($stateParams.schema) + "/" + $scope.masterclass + "/" + $scope.masterid + "/" + $scope.dbclass + "/" + encodeURIComponent(addedKeys[i]))
                             .success(function (data) {
                                 loop();
                             });
                    }
                },
                callback: function () {
                    $scope.loading = false;
                    $scope.isUpdated = true;
                }
            })
        }

        if (removedKeys.length > 0) {
            $scope.loading = true;
            // delete relationhsips from the db
            asyncLoop({
                length: removedKeys.length,
                functionToLoop: function (loop, i) {
                    $http.delete(APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent($stateParams.schema) + "/" + $scope.masterclass + "/" + $scope.masterid + "/" + $scope.dbclass + "/" + encodeURIComponent(removedKeys[i]))
                         .success(function (data) {
                             loop();
                         });
                },
                callback: function () {
                    $scope.existingKeys = [];
                    for (var i = 0; i < $scope.currentKeys.length; i++)
                        $scope.existingKeys.push($scope.currentKeys[i]);
                    $scope.loading = false;
                    $scope.isUpdated = true;
                }
            })
        }

    };

    $scope.goBack = function () {
        if ($scope.isUpdated) {
            $modalInstance.close({ "modal": "viewManyToMany"});
        }
        else {
            $modalInstance.dismiss("dismiss");
        }
    };
});

'use strict';

angular.module('app.userdirectory').controller('RoleListViewCtrl', function ($controller, $rootScope, $scope, $http, APP_CONFIG, $state, $stateParams, TestStations, $interval, userService) {

    $scope.dbschema = $stateParams.schema;
    $scope.dbclass = $stateParams.baseclass;
    $scope.roletype = $stateParams.roletype;

    $scope.filter = "['RType', '=', '" + $scope.roletype + "']";

    if ($stateParams.insert && $stateParams.insert === "false") {
        $scope.add = false;
    }
    else {
        $scope.add = true;
    }

    if ($stateParams.export && $stateParams.export === "true") {
        $scope.exportData = true;
    }
    else {
        $scope.exportData = false;
    }

    if ($stateParams.import && $stateParams.import === "true") {
        $scope.importData = true;
    }
    else {
        $scope.importData = false;
    }

    angular.extend(this, $controller('dataGridBaseCtrl', { $scope: $scope, $rootScope: $rootScope, $http: $http, APP_CONFIG: APP_CONFIG }));

    $scope.openModal = function () {
        $state.go('.modalform', { schema: $scope.dbschema, class: $scope.dbclass }, { location: false, notify: false });
    };

    $scope.GetCommands = function (rowIndex, data) {
        var items = new Array();

        var url = APP_CONFIG.ebaasRootUrl + "/api/sitemap/commands/" + encodeURIComponent($scope.dbschema) + "/" + $scope.dbclass;

        $http.get(url).success(function (commands) {

            // custom commands
            $scope.commands = commands;
            var cmdInfo;
            var item;
            for (var cmd in commands) {
                if (commands.hasOwnProperty(cmd)) {
                    cmdInfo = commands[cmd];
                    item = new Object();
                    item.text = cmdInfo.title;
                    item.css = "btn btn-primary btn-md btn-nav";
                    if (cmdInfo.icon) {
                        item.icon = cmdInfo.icon;
                    }
                    else {
                        item.icon = "fa fa-lg fa-tasks";
                    }

                    item.onItemClick = function (text) {
                        gotoState(text, $scope.dbschema, data.type, data.obj_id)
                    }

                    items.push(item);

                    if (cmdInfo.baseUrl && !APP_CONFIG.hashedBaseUrls[cmdInfo.hash]) {
                        APP_CONFIG.hashedBaseUrls[cmdInfo.hash] = cmdInfo.baseUrl;
                    }
                }
            }

            if (data.allowWrite && $stateParams.edit !== "false") {
                items.push({
                    text: $rootScope.getWord('Edit'),
                    icon: "fa fa-lg fa-edit",
                    css: "btn btn-default btn-md btn-nav",
                    onItemClick: function () {
                        $state.go('.modalform', { schema: $scope.dbschema, class: data.type, oid: data.obj_id }, { location: false, notify: false });
                    }
                });
            }

            if (data.allowDelete && $stateParams.delete !== "false") {
                items.push({
                    text: $rootScope.getWord('Delete'),
                    icon: "fa fa-lg fa-times",
                    css: "btn btn-default btn-md btn-nav",
                    onItemClick: function () {
                        $scope.gridInstance.deleteRow(rowIndex);
                    }
                });
            }
        });
        return items;
    }

    var gotoState = function (title, dbschema, dbclass, oid) {
        var commands = $scope.commands;
        var url = undefined;
        var cmdUrl = undefined;
        var params = undefined;
        var cmdInfo;
        for (var cmd in commands) {
            if (commands.hasOwnProperty(cmd)) {
                cmdInfo = commands[cmd];
                if (cmdInfo.title === title) {
                    url = cmdInfo.url;
                    cmdUrl = cmdInfo.url;
                    params = new Object();
                    params.schema = dbschema;
                    params.class = dbclass;
                    params.oid = oid;
                    params.cmdHash = cmdInfo.hash;

                    // add command's parameters to the state parameters
                    if (cmdInfo.parameters) {
                        for (var key in cmdInfo.parameters) {
                            if (cmdInfo.parameters.hasOwnProperty(key)) {
                                params[key] = cmdInfo.parameters[key];
                            }
                        }
                    };

                    break;
                }
            }
        }

        if (url) {
            try
            {
                if (cmdUrl === ".modalform") {
                    $state.go(url, params, { location: false, notify: false });
                }
                else {
                    $state.go(url, params);
                }
            }
            catch (err)
            {
                BootstrapDialog.show({
                    title: $rootScope.getWord("Info Dialog"),
                    type: BootstrapDialog.TYPE_INFO,
                    message: $rootScope.getWord("Invalid Command"),
                    buttons: [{
                        label: $rootScope.getWord("Cancel"),
                        action: function (dialog) {
                            dialog.close();
                        }
                    }]
                });
            }
        }
    }

    $scope.gridInstance = null;
    $scope.dataGridSettings = {
        dataSource: {
            store: $scope.customStore
        },
        columnAutoWidth: true,
        height: $rootScope.isChrome() === true ? '750px' : undefined,
        sorting: {
            mode: "multiple"
        },
        searchPanel: {
            visible: $stateParams.search && $stateParams.search === "true"? true: false,
            width: 300,
            placeholder: $rootScope.getWord("Keyword Search")
        },
        editing: {
            allowAdding: false,
            allowUpdating: false,
            allowDeleting: false
        },
        grouping: {
            autoExpandAll: false
        },
        pager: {
            visible: true,
            showPageSizeSelector: true,
            showInfo: true
        },
        filterRow: {
            visible: true,
            applyFilter: "auto"
        },
        selection: { mode: 'single' },
        remoteOperations: true,
        bindingOptions: {
            columns: 'columns'
        },
        headerFilter: {
            visible: true
        },
        rowAlternationEnabled: true,
        masterDetail: {
            enabled: true,
            template: "detail"
        },
        onRowClick: function (e) {
            if (e.rowType === "data") {
                var isExpanded = $scope.gridInstance.isRowExpanded(e.key);
                $scope.gridInstance.collapseAll(-1); // collaspsed all
                if (!isExpanded) {
                    $scope.gridInstance.expandRow(e.key);
                }
            }
        },
        onInitialized: function (e) {
            $scope.gridInstance = e.component;
        },
        onRowPrepared: function (e) {
        }
    };

    $rootScope.$on('modalClosed', function (event, data) {
        if ($scope.gridInstance && data === "update")
            $scope.gridInstance.refresh();
    });
});
'use strict';

angular.module('app.userdirectory').controller('UserDirectoryLayoutCtrl', function ($http, APP_CONFIG, $scope, $rootScope, $state, $stateParams, userService, propmisedParams) {

    $scope.dbschema = $stateParams.schema;
    $scope.userclass = $stateParams.class;
    $scope.roleclass = "Role";

    // Getting unit tree
    $scope.unitObjId = $stateParams.unitObjId;

    var params = propmisedParams.data;

    $scope.view = params['dataView'];
    $scope.formTemplate = params['formTemplate'];

    userService.getUnitTree($stateParams.schema, $scope.roleclass, $scope.unitObjId, function (data) {
        $scope.unitTree = data;
    });

    // Getting function list
    userService.getFunctions($stateParams.schema, $scope.roleclass, function (data) {
        // add "Every One" as the first function
        var role = {};
        role.Text = $rootScope.getWord("Everyone");
        role.obj_id = undefined;
        if (data)
        {
            data.unshift(role);
        }
        else
        {
            data = [];
            data.push(role);
        }
 
        $scope.functions = data;
    });

    $state.go('app.userdirectory.usertable', { schema: $scope.dbschema, baseclass: $scope.roleclass, baseoid: undefined, relatedclass: $scope.userclass, view: $scope.view, formtemplate: $scope.formTemplate });

    $scope.functionObjId = undefined;
    
    $scope.getFunctionUsers = function (functionObjId) {
        $scope.functionObjId = functionObjId;
        $state.go('app.userdirectory.usertable', { schema: $scope.dbschema, baseclass: $scope.roleclass, baseoid: functionObjId, relatedclass: $scope.userclass, view: $scope.view, formtemplate: $scope.formTemplate });
    }

    $scope.getUnitUsers = function (unitObjId) {
        $scope.unitObjId = unitObjId;
        $state.go('app.userdirectory.usertable', { schema: $scope.dbschema, baseclass: $scope.roleclass, baseoid: unitObjId, relatedclass: $scope.userclass, view: $scope.view, formtemplate: $scope.formTemplate });
    }

    $scope.openFunctions = function () {
        $state.go('app.userdirectory.roletable', { schema: $scope.dbschema, baseclass: $scope.roleclass, roletype: "Function" });
    }

    $scope.openUnits = function () {
        $state.go('app.userdirectory.roletable', { schema: $scope.dbschema, baseclass: $scope.roleclass, roletype: "Unit" });
    }

    $scope.refresh = function () {
        $state.reload();
    }
});
'use strict';

angular.module('app.userdirectory').controller('UserListCtrl', function ($scope, $rootScope, $state, $stateParams, APP_CONFIG, userService, promisedUsers) {

    $scope.dbschema = $stateParams.schema;
    $scope.userclass = $stateParams.class;
    $scope.roleclass = "Role";

    $scope.getWord = function(key)
    {
        return key;
    }

    $scope.tableOptions = {
        "data": userService.convertUsers(promisedUsers.data),
        //            "bDestroy": true,
        "iDisplayLength": 20,
        "columns": [
            {
                "class": 'details-control',
                "orderable": false,
                "data": null,
                "defaultContent": ''
            },
            { "data": "FullName" },
            { "data": "ID" },
            { "data": "PhoneNumber" },
            { "data": "Email" }
        ],
        "order": [[1, 'asc']]
    }

    // Getting unit tree
    $scope.unitObjId = $stateParams.unitObjId;

    userService.getUnitTree($stateParams.schema, $scope.roleclass, $scope.unitObjId, function (data) {
        $scope.unitTree = data;

    });

    // Getting function list
    userService.getFunctions($stateParams.schema, $scope.roleclass, function (data) {
       
        $scope.functions = data;

    });

    $scope.functionObjId = $stateParams.functionObjId;
    $scope.getFunctionUsers = function (functionObjId) {
        var params = new Object();

        params.functionObjId = functionObjId;
        params.unitObjId = undefined;
        $scope.functionObjId = functionObjId;
        $scope.unitObjId = undefined;

        $state.go($state.current, params, { reload: true }); //second parameter is for $stateParams
    }

    $scope.getUnitUsers = function(unitObjId)
    {
        var params = new Object();

        params.unitObjId = unitObjId;
        params.functionObjId = undefined;
        $scope.unitObjId = unitObjId;
        $scope.functionObjId = undefined;

        $state.go($state.current, params, { reload: true }); //second parameter is for $stateParams
    }
});
'use strict';

angular.module('app.userdirectory').controller('UserListViewCtrl', function ($controller, $rootScope, $scope, $http, APP_CONFIG, $state, $stateParams, TestStations, $interval, userService) {

    $scope.dbschema = $stateParams.schema;
    $scope.userclass = $stateParams.relatedclass;
    
    $scope.formTemplate = $stateParams.formtemplate;

    if ($stateParams.baseoid) {
        $scope.dbclass = $stateParams.baseclass;
        $scope.relatedclass = $stateParams.relatedclass;
        $scope.relatedview = $stateParams.view;
        $scope.oid = $stateParams.baseoid;
        
        $scope.isrelated = true;
    }
    else
    {
        // get all users
        $scope.dbclass = $stateParams.relatedclass;
        $scope.view = $stateParams.view;
    }

    if ($stateParams.insert && $stateParams.insert === "false") {
        $scope.add = false;
    }
    else {
        $scope.add = true;
    }

    if ($stateParams.export && $stateParams.export === "true") {
        $scope.exportData = true;
    }
    else {
        $scope.exportData = false;
    }

    if ($stateParams.import && $stateParams.import === "true") {
        $scope.importData = true;
    }
    else {
        $scope.importData = false;
    }

    angular.extend(this, $controller('dataGridBaseCtrl', { $scope: $scope, $rootScope: $rootScope, $http: $http, APP_CONFIG: APP_CONFIG }));

    $scope.openModal = function () {
        $state.go('.modalform', { schema: $scope.dbschema, class: $scope.userclass, template: $scope.formTemplate }, { location: false, notify: false });
    };

    $scope.GetCommands = function (rowIndex, data) {
        var items = new Array();

        var url = APP_CONFIG.ebaasRootUrl + "/api/sitemap/commands/" + encodeURIComponent($scope.dbschema) + "/" + $scope.userclass;

        $http.get(url).success(function (commands) {

            // custom commands
            $scope.commands = commands;
            var cmdInfo;
            var item;
            for (var cmd in commands) {
                if (commands.hasOwnProperty(cmd)) {
                    cmdInfo = commands[cmd];
                    item = new Object();
                    item.text = cmdInfo.title;
                    item.css = "btn btn-primary btn-md btn-nav";
                    if (cmdInfo.icon) {
                        item.icon = cmdInfo.icon;
                    }
                    else {
                        item.icon = "fa fa-lg fa-tasks";
                    }

                    item.onItemClick = function (text) {
                        gotoState(text, $scope.dbschema, data.type, data.obj_id)
                    }

                    items.push(item);

                    if (cmdInfo.baseUrl && !APP_CONFIG.hashedBaseUrls[cmdInfo.hash]) {
                        APP_CONFIG.hashedBaseUrls[cmdInfo.hash] = cmdInfo.baseUrl;
                    }
                }
            }

            if (data.allowWrite && $stateParams.edit !== "false") {
                items.push({
                    text: $rootScope.getWord('Edit'),
                    icon: "fa fa-lg fa-edit",
                    css: "btn btn-default btn-md btn-nav",
                    onItemClick: function () {
                        $state.go('.modalform', { schema: $scope.dbschema, class: data.type, oid: data.obj_id, template: $scope.formTemplate }, { location: false, notify: false });
                    }
                });
            }

            if (data.allowDelete && $stateParams.delete !== "false") {
                items.push({
                    text: $rootScope.getWord('Delete'),
                    icon: "fa fa-lg fa-times",
                    css: "btn btn-default btn-md btn-nav",
                    onItemClick: function () {
                        $scope.gridInstance.deleteRow(rowIndex);
                    }
                });
            }
        });
        return items;
    }

    var gotoState = function (title, dbschema, dbclass, oid) {
        var commands = $scope.commands;
        var url = undefined;
        var cmdUrl = undefined;
        var params = undefined;
        var cmdInfo;
        for (var cmd in commands) {
            if (commands.hasOwnProperty(cmd)) {
                cmdInfo = commands[cmd];
                if (cmdInfo.title === title) {
                    url = cmdInfo.url;
                    cmdUrl = cmdInfo.url;
                    params = new Object();
                    params.schema = dbschema;
                    params.class = dbclass;
                    params.oid = oid;
                    params.cmdHash = cmdInfo.hash;

                    // add command's parameters to the state parameters
                    if (cmdInfo.parameters) {
                        for (var key in cmdInfo.parameters) {
                            if (cmdInfo.parameters.hasOwnProperty(key)) {
                                params[key] = cmdInfo.parameters[key];
                            }
                        }
                    };

                    break;
                }
            }
        }

        if (url) {
            try
            {
                if (cmdUrl === ".modalform") {
                    $state.go(url, params, { location: false, notify: false });
                }
                else {
                    $state.go(url, params);
                }
            }
            catch (err)
            {
                BootstrapDialog.show({
                    title: $rootScope.getWord("Info Dialog"),
                    type: BootstrapDialog.TYPE_INFO,
                    message: $rootScope.getWord("Invalid Command"),
                    buttons: [{
                        label: $rootScope.getWord("Cancel"),
                        action: function (dialog) {
                            dialog.close();
                        }
                    }]
                });
            }
        }
    }

    $scope.gridInstance = null;
    $scope.dataGridSettings = {
        dataSource: {
            store: $scope.customStore
        },
        columnAutoWidth: true,
        height: $rootScope.isChrome() === true ? '750px' : undefined,
        sorting: {
            mode: "multiple"
        },
        searchPanel: {
            visible: $stateParams.search && $stateParams.search === "true"? true: false,
            width: 300,
            placeholder: $rootScope.getWord("Keyword Search")
        },
        editing: {
            allowAdding: false,
            allowUpdating: false,
            allowDeleting: false
        },
        grouping: {
            autoExpandAll: false
        },
        pager: {
            visible: true,
            showPageSizeSelector: true,
            showInfo: true
        },
        filterRow: {
            visible: true,
            applyFilter: "auto"
        },
        selection: { mode: 'single' },
        remoteOperations: true,
        bindingOptions: {
            columns: 'columns'
        },
        headerFilter: {
            visible: true
        },
        rowAlternationEnabled: true,
        masterDetail: {
            enabled: true,
            template: "detail"
        },
        onRowClick: function (e) {
            if (e.rowType === "data") {
                var isExpanded = $scope.gridInstance.isRowExpanded(e.key);
                $scope.gridInstance.collapseAll(-1); // collaspsed all
                if (!isExpanded) {
                    $scope.gridInstance.expandRow(e.key);
                }
            }
        },
        onInitialized: function (e) {
            $scope.gridInstance = e.component;
        },
        onRowPrepared: function (e) {
        }
    };

    $rootScope.$on('modalClosed', function (event, data) {
        if ($scope.gridInstance && data === "update")
            $scope.gridInstance.refresh();
    });
});
"use strict";

angular.module('app.userdirectory').factory('userService', function ($http, APP_CONFIG) {

    var convertUsers = function (userCollection) {

        var id = "ID";
        var firstName = "FirstName";
        var lastName = "LastName";
        var phoneNumber = "PhoneNumber";
        var email = "Email";
        var picture = "Picture";
        var division = "Department";
        var address = "Location";
        var displayText = "DisplayText";

        var users = new Array();

        if (userCollection) {

            for (var i = 0; i < userCollection.length; i++) {
                var user = userCollection[i];

                var userItem = new Object();

                userItem.obj_id = user["obj_id"];
                userItem.ID = user[id];
                if (user[displayText]) {
                    userItem.FullName = user[displayText];
                }
                else
                {
                    userItem.FullName = user[lastName] + user[firstName];
                }
                userItem.PhoneNumber = user[phoneNumber];
                userItem.Email = user[email];
                userItem.Division = user[division];
                userItem.Address = user[address];
                if (user[picture]) {
                    userItem.Picture = APP_CONFIG.avatarsUrl + userItem.ID + ".png";
                }
                else
                {
                    userItem.Picture = APP_CONFIG.avatarsUrl + "male.png";
                }

                users.push(userItem);
            }
        }

        return users;
    }

    var createUnitTree = function (nodes, currentNodeObjId) {
        var map = {}, node, menuItem, parentItem, roots = [], menuItems = [];
        for (var i = 0; i < nodes.length; i += 1) {
            node = nodes[i];
            
            menuItem = new Object();
            menuItem.name = node.Name;
            menuItem.text = node.Text;
            menuItem.children = new Array();
            map[menuItem.name] = i; // use map to look-up the parents
            menuItems.push(menuItem);
        }

        for (var i = 0; i < nodes.length; i += 1) {
            node = nodes[i];
            menuItem = menuItems[i];

            if (node.parentRole != "") {
                parentItem = menuItems[map[node.parentRole]];
                parentItem.children.push(menuItem);
                menuItem.parentItem = parentItem;
            } else {
                //menuItem.expanded = true;
                menuItem.parentItem = undefined;
                roots.push(menuItem);
            }
        }

        for (var i = 0; i < nodes.length; i += 1) {
            node = nodes[i];
            menuItem = menuItems[i];

            if (menuItem.children.length > 0) {
                menuItem.content = "<span><i class=\"fa fa-lg fa-plus-circle\"></i> " + node.Text + "</span>";
                //menuItem.content = "<span class='label label-info'><i class=\"fa fa-lg fa-plus-circle\"></i>&nbsp;&nbsp;<a class=\"station-a\" href=\"javascript:angular.element(document.getElementById('unitTreeContainer')).scope().getUnitUsers(" + node.obj_id + ");\">" + node.Text + "</a></span>";
            } else {
                if (currentNodeObjId && currentNodeObjId === node.obj_id)
                {
                    menuItem.content = "<span class='label label-warning'><a class=\"station-a\" href=\"javascript:angular.element(document.getElementById('unitTreeContainer')).scope().getUnitUsers(" + node.obj_id + ");\">" + node.Text + "</a></span>";

                    var thisItem = menuItem;
                    while (thisItem.parentItem) {
                        thisItem = thisItem.parentItem;
                        thisItem.expanded = true;
                    }
                }
                else
                {
                    menuItem.content = "<span class='label label-info'><a class=\"station-a\" href=\"javascript:angular.element(document.getElementById('unitTreeContainer')).scope().getUnitUsers(" + node.obj_id + ");\">" + node.Text + "</a></span>";
                }
            }
        }

        return roots;
    };

    function getAllUsers(dbschema, userclass, pageIndex, callback) {
	    
	    var pageSize = 20;
	    var url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent(dbschema) + "/" + userclass + "?view=full&size=" + pageSize;
	    if (pageIndex) {
	        var from = pageIndex * pageSize;
	        url += "&from=" + from;
	    }

        $http.get(url).success(function (data) {
	        callback(convertUsers(data));
				
		}).error(function(){
		    callback([]);

		});
    }

    function getUnitTree(dbschema, roleclass, currentNodeId, callback) {

        var pageSize = 200;
        var url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent(dbschema) + "/" + roleclass + "?view=full&size=" + pageSize + "&filter=['RType', '=', 'Unit']";

        $http.get(url).success(function (data) {
            callback(createUnitTree(data, currentNodeId));

        }).error(function () {
            callback(undefined);

        });
    }

    function getFunctions(dbschema, roleclass, callback) {

        var pageSize = 200;
        var url = APP_CONFIG.ebaasRootUrl + "/api/data/" + encodeURIComponent(dbschema) + "/" + roleclass + "?view=full&size=" + pageSize + "&filter=['RType', '=', 'Function']";

        $http.get(url).success(function (data) {
            callback(data);

        }).error(function () {
            callback(undefined);

        });
    }
	
	return {
	    getAllUsers: function (dbschema, userclass, pageIndex, callback) {
	        getAllUsers(dbschema, userclass, pageIndex, callback);
	    },
	    convertUsers: function (userCollection) {
	        return convertUsers(userCollection);
	    },
	    getUnitTree: function (dbschema, roleclass, currentNodeId, callback) {
	        return getUnitTree(dbschema, roleclass, currentNodeId, callback);
	    },
	    getFunctions: function (dbschema, roleclass, callback) {
	        return getFunctions(dbschema, roleclass, callback);
	    }
	}
});
"use strict";

angular.module('app.auth').directive('facebookSignin', function ($rootScope, ezfb) {
    return {
        replace: true,
        restrict: 'E',
        template: '<a class="btn btn-block btn-social btn-facebook"><i class="fa fa-facebook"></i> Sign in with Facebook</a>',
        link: function(scope, element){
            element.on('click', function(){
                ezfb.login(function (res) {
                    if (res.authResponse) {
                        $rootScope.$broadcast('event:facebook-signin-success', res.authResponse);
                    }
                }, {scope: 'public_profile'});
            })

        }
    }
});
"use strict";

angular.module('app.auth').directive('googleSignin', function ($rootScope, GooglePlus) {
    return {
        restrict: 'E',
        template: '<a class="g-signin btn btn-block btn-social btn-google-plus"><i class="fa fa-google-plus"></i> Sign in with Google</a>',
        replace: true,
        link: function (scope, element) {
            element.on('click', function(){
                GooglePlus.login().then(function (authResult) {
                    $rootScope.$broadcast('event:google-plus-signin-success', authResult);

                }, function (err) {
                    $rootScope.$broadcast('event:google-plus-signin-failure', err);

                });
            })
        }
    };
});

'use strict';

angular.module('app.chat').factory('ChatApi', function ($q, $rootScope, User, $http, APP_CONFIG) {
    var dfd = $q.defer();
    var _user;
    var ChatSrv = {
        initialized: dfd.promise,
        users: [],
        messages: [],
        statuses: ['Online', 'Busy', 'Away', 'Log Off'],
        status: 'Online',
        setUser: function (user) {
            if (ChatSrv.users.indexOf(_user) != -1)
                ChatSrv.users.splice(ChatSrv.users.indexOf(_user), 1);
            _user = user;
            ChatSrv.users.push(_user);
        },
        sendMessage: function (text) {
            var message = {
                user: _user,
                body: text,
                date: new Date()
            };
            this.messages.push(message);
        }
    };


    $http.get(APP_CONFIG.apiRootUrl + '/chat.json').then(function(res){
        ChatSrv.messages = res.data.messages;
        ChatSrv.users = res.data.users;
        dfd.resolve();
    });

    ChatSrv.initialized.then(function () {

        User.initialized.then(function () {
            ChatSrv.setUser({
                username: User.userName,
                picture: User.picture,
                status: ChatSrv.status
            });
        });

        $rootScope.$watch(function () {
            return User.userName
        }, function (name, oldName) {
            if (name != oldName) {
                ChatSrv.setUser({
                    username: User.username,
                    picture: User.picture,
                    status: ChatSrv.status
                });
            }
        });
    });


    return ChatSrv;

});
(function() {
        
   'use strict';

    /*
    * SMARTCHAT PLUGIN ARRAYS & CONFIG
    * Dependency: js/plugin/moment/moment.min.js 
    *             js/plugin/cssemotions/jquery.cssemoticons.min.js 
    *             js/smart-chat-ui/smart.chat.ui.js
    * (DO NOT CHANGE) 
    */ 
        var boxList = [],
        showList = [],
        nameList = [],
        idList = [];
    /*
    * Width of the chat boxes, and the gap inbetween in pixel (minus padding)
    */ 
        var chatbox_config = {
            width: 200,
            gap: 35,
            offset: 0
        };



    /*
    * SMART CHAT ENGINE
    * Copyright (c) 2013 Wen Pu
    * Modified by MyOrange
    * All modifications made are hereby copyright (c) 2014-2015 MyOrange
    */

    // TODO: implement destroy()
    (function($) {
    $.widget("ui.chatbox", {
        options: {
            id: null, //id for the DOM element
            title: null, // title of the chatbox
            user: null, // can be anything associated with this chatbox
            hidden: false,
            offset: 0, // relative to right edge of the browser window
            width: 300, // width of the chatbox
            status: 'online', //
            alertmsg: null,
            alertshow: null,
            messageSent: function(id, user, msg) {
                // override this
                this.boxManager.addMsg(user.first_name, msg);
            },
            boxClosed: function(id) {
            }, // called when the close icon is clicked
            boxManager: {
                // thanks to the widget factory facility
                // similar to http://alexsexton.com/?p=51
                init: function(elem) {
                    this.elem = elem;
                },
                addMsg: function(peer, msg) {
                    var self = this;
                    var box = self.elem.uiChatboxLog;
                    var e = document.createElement('div');
                    box.append(e);
                    $(e).hide();

                    var systemMessage = false;

                    if (peer) {
                        var peerName = document.createElement("b");
                        $(peerName).text(peer + ": ");
                        e.appendChild(peerName);
                    } else {
                        systemMessage = true;
                    }

                    var msgElement = document.createElement(
                        systemMessage ? "i" : "span");
                    $(msgElement).text(msg);
                    e.appendChild(msgElement);
                    $(e).addClass("ui-chatbox-msg");
                    $(e).css("maxWidth", $(box).width());
                    $(e).fadeIn();
                    //$(e).prop( 'title', moment().calendar() ); // add dep: moment.js
                    $(e).find("span").emoticonize(); // add dep: jquery.cssemoticons.js
                    self._scrollToBottom();

                    if (!self.elem.uiChatboxTitlebar.hasClass("ui-state-focus")
                        && !self.highlightLock) {
                        self.highlightLock = true;
                        self.highlightBox();
                    }
                },
                highlightBox: function() {
                    var self = this;
                    self.elem.uiChatboxTitlebar.effect("highlight", {}, 300);
                    self.elem.uiChatbox.effect("bounce", {times: 2}, 300, function() {
                        self.highlightLock = false;
                        self._scrollToBottom();
                    });
                },
                toggleBox: function() {
                    this.elem.uiChatbox.toggle();
                },
                _scrollToBottom: function() {
                    var box = this.elem.uiChatboxLog;
                    box.scrollTop(box.get(0).scrollHeight);
                }
            }
        },
        toggleContent: function(event) {
            this.uiChatboxContent.toggle();
            if (this.uiChatboxContent.is(":visible")) {
                this.uiChatboxInputBox.focus();
            }
        },
        widget: function() {
            return this.uiChatbox
        },
        _create: function() {
            var self = this,
            options = self.options,
            title = options.title || "No Title",
            // chatbox
            uiChatbox = (self.uiChatbox = $('<div></div>'))
                .appendTo(document.body)
                .addClass('ui-widget ' +
                          //'ui-corner-top ' +
                          'ui-chatbox'
                         )
                .attr('outline', 0)
                .focusin(function() {
                    // ui-state-highlight is not really helpful here
                    //self.uiChatbox.removeClass('ui-state-highlight');
                    self.uiChatboxTitlebar.addClass('ui-state-focus');
                })
                .focusout(function() {
                    self.uiChatboxTitlebar.removeClass('ui-state-focus');
                }),
            // titlebar
            uiChatboxTitlebar = (self.uiChatboxTitlebar = $('<div></div>'))
                .addClass('ui-widget-header ' +
                          //'ui-corner-top ' +
                          'ui-chatbox-titlebar ' +
                          self.options.status +
                          ' ui-dialog-header' // take advantage of dialog header style
                         )
                .click(function(event) {
                    self.toggleContent(event);
                })
                .appendTo(uiChatbox),
            uiChatboxTitle = (self.uiChatboxTitle = $('<span></span>'))
                .html(title)
                .appendTo(uiChatboxTitlebar),
            uiChatboxTitlebarClose = (self.uiChatboxTitlebarClose = $('<a href="#" rel="tooltip" data-placement="top" data-original-title="Hide"></a>'))
                .addClass(//'ui-corner-all ' +
                          'ui-chatbox-icon '
                         )
                .attr('role', 'button')
                .hover(function() { uiChatboxTitlebarClose.addClass('ui-state-hover'); },
                       function() { uiChatboxTitlebarClose.removeClass('ui-state-hover'); })
                .click(function(event) {
                    uiChatbox.hide();
                    self.options.boxClosed(self.options.id);
                    return false;
                })
                .appendTo(uiChatboxTitlebar),
            uiChatboxTitlebarCloseText = $('<i></i>')
                .addClass('fa ' +
                          'fa-times')
                .appendTo(uiChatboxTitlebarClose),
            uiChatboxTitlebarMinimize = (self.uiChatboxTitlebarMinimize = $('<a href="#" rel="tooltip" data-placement="top" data-original-title="Minimize"></a>'))
                .addClass(//'ui-corner-all ' +
                          'ui-chatbox-icon'
                         )
                .attr('role', 'button')
                .hover(function() { uiChatboxTitlebarMinimize.addClass('ui-state-hover'); },
                       function() { uiChatboxTitlebarMinimize.removeClass('ui-state-hover'); })
                .click(function(event) {
                    self.toggleContent(event);
                    return false;
                })
                .appendTo(uiChatboxTitlebar),
            uiChatboxTitlebarMinimizeText = $('<i></i>')
                .addClass('fa ' +
                          'fa-minus')
                .appendTo(uiChatboxTitlebarMinimize),
            // content
            uiChatboxContent = (self.uiChatboxContent = $('<div class="'+ self.options.alertshow +'"><span class="alert-msg">'+ self.options.alertmsg + '</span></div>'))
                .addClass('ui-widget-content ' +
                          'ui-chatbox-content '
                         )
                .appendTo(uiChatbox),
            uiChatboxLog = (self.uiChatboxLog = self.element)
                .addClass('ui-widget-content ' +
                          'ui-chatbox-log ' +
                          'custom-scroll'
                         )
                .appendTo(uiChatboxContent),
            uiChatboxInput = (self.uiChatboxInput = $('<div></div>'))
                .addClass('ui-widget-content ' +
                          'ui-chatbox-input'
                         )
                .click(function(event) {
                    // anything?
                })
                .appendTo(uiChatboxContent),
            uiChatboxInputBox = (self.uiChatboxInputBox = $('<textarea></textarea>'))
                .addClass('ui-widget-content ' +
                          'ui-chatbox-input-box '
                         )
                .appendTo(uiChatboxInput)
                .keydown(function(event) {
                    if (event.keyCode && event.keyCode == $.ui.keyCode.ENTER) {
                        var msg = $.trim($(this).val());
                        if (msg.length > 0) {
                            self.options.messageSent(self.options.id, self.options.user, msg);
                        }
                        $(this).val('');
                        return false;
                    }
                })
                .focusin(function() {
                    uiChatboxInputBox.addClass('ui-chatbox-input-focus');
                    var box = $(this).parent().prev();
                    box.scrollTop(box.get(0).scrollHeight);
                })
                .focusout(function() {
                    uiChatboxInputBox.removeClass('ui-chatbox-input-focus');
                });

            // disable selection
            uiChatboxTitlebar.find('*').add(uiChatboxTitlebar).disableSelection();

            // switch focus to input box when whatever clicked
            uiChatboxContent.children().click(function() {
                // click on any children, set focus on input box
                self.uiChatboxInputBox.focus();
            });

            self._setWidth(self.options.width);
            self._position(self.options.offset);

            self.options.boxManager.init(self);

            if (!self.options.hidden) {
                uiChatbox.show();
            }
            
            $(".ui-chatbox [rel=tooltip]").tooltip();
            //console.log("tooltip created");
        },
        _setOption: function(option, value) {
            if (value != null) {
                switch (option) {
                case "hidden":
                    if (value)
                        this.uiChatbox.hide();
                    else
                        this.uiChatbox.show();
                    break;
                case "offset":
                    this._position(value);
                    break;
                case "width":
                    this._setWidth(value);
                    break;
                }
            }
            $.Widget.prototype._setOption.apply(this, arguments);
        },
        _setWidth: function(width) {
            this.uiChatbox.width((width + 28) + "px");
            //this.uiChatboxTitlebar.width((width + 28) + "px");
            //this.uiChatboxLog.width(width + "px");
           // this.uiChatboxInput.css("maxWidth", width + "px");
            // padding:2, boarder:2, margin:5
            this.uiChatboxInputBox.css("width", (width + 18) + "px");
        },
        _position: function(offset) {
            this.uiChatbox.css("right", offset);
        }
    });
    }(jQuery));


    /*
    * jQuery CSSEmoticons plugin 0.2.9
    *
    * Copyright (c) 2010 Steve Schwartz (JangoSteve)
    *
    * Dual licensed under the MIT and GPL licenses:
    *   http://www.opensource.org/licenses/mit-license.php
    *   http://www.gnu.org/licenses/gpl.html
    *
    * Date: Sun Oct 22 1:00:00 2010 -0500
    */
    (function($) {
    $.fn.emoticonize = function(options) {

    var opts = $.extend({}, $.fn.emoticonize.defaults, options);

    var escapeCharacters = [ ")", "(", "*", "[", "]", "{", "}", "|", "^", "<", ">", "\\", "?", "+", "=", "." ];

    var threeCharacterEmoticons = [
        // really weird bug if you have :{ and then have :{) in the same container anywhere *after* :{ then :{ doesn't get matched, e.g. :] :{ :) :{) :) :-) will match everything except :{
        //  But if you take out the :{) or even just move :{ to the right of :{) then everything works fine. This has something to do with the preMatch string below I think, because
        //  it'll work again if you set preMatch equal to '()'
        //  So for now, we'll just remove :{) from the emoticons, because who actually uses this mustache man anyway?
      // ":{)",
      ":-)", ":o)", ":c)", ":^)", ":-D", ":-(", ":-9", ";-)", ":-P", ":-p", ":-Þ", ":-b", ":-O", ":-/", ":-X", ":-#", ":'(", "B-)", "8-)", ";*(", ":-*", ":-\\",
      "?-)", // <== This is my own invention, it's a smiling pirate (with an eye-patch)!
      // and the twoCharacterEmoticons from below, but with a space inserted
      ": )", ": ]", "= ]", "= )", "8 )", ": }", ": D", "8 D", "X D", "x D", "= D", ": (", ": [", ": {", "= (", "; )", "; ]", "; D", ": P", ": p", "= P", "= p", ": b", ": Þ", ": O", "8 O", ": /", "= /", ": S", ": #", ": X", "B )", ": |", ": \\", "= \\", ": *", ": &gt;", ": &lt;"//, "* )"
    ];

    var twoCharacterEmoticons = [ // separate these out so that we can add a letter-spacing between the characters for better proportions
      ":)", ":]", "=]", "=)", "8)", ":}", ":D", ":(", ":[", ":{", "=(", ";)", ";]", ";D", ":P", ":p", "=P", "=p", ":b", ":Þ", ":O", ":/", "=/", ":S", ":#", ":X", "B)", ":|", ":\\", "=\\", ":*", ":&gt;", ":&lt;"//, "*)"
    ];

    var specialEmoticons = { // emoticons to be treated with a special class, hash specifies the additional class to add, along with standard css-emoticon class
      "&gt;:)": { cssClass: "red-emoticon small-emoticon spaced-emoticon" },
      "&gt;;)": { cssClass: "red-emoticon small-emoticon spaced-emoticon"},
      "&gt;:(": { cssClass: "red-emoticon small-emoticon spaced-emoticon" },
      "&gt;: )": { cssClass: "red-emoticon small-emoticon" },
      "&gt;; )": { cssClass: "red-emoticon small-emoticon"},
      "&gt;: (": { cssClass: "red-emoticon small-emoticon" },
      ";(":     { cssClass: "red-emoticon spaced-emoticon" },
      "&lt;3":  { cssClass: "pink-emoticon counter-rotated" },
      "O_O":    { cssClass: "no-rotate" },
      "o_o":    { cssClass: "no-rotate" },
      "0_o":    { cssClass: "no-rotate" },
      "O_o":    { cssClass: "no-rotate" },
      "T_T":    { cssClass: "no-rotate" },
      "^_^":    { cssClass: "no-rotate" },
      "O:)":    { cssClass: "small-emoticon spaced-emoticon" },
      "O: )":   { cssClass: "small-emoticon" },
      "8D":     { cssClass: "small-emoticon spaced-emoticon" },
      "XD":     { cssClass: "small-emoticon spaced-emoticon" },
      "xD":     { cssClass: "small-emoticon spaced-emoticon" },
      "=D":     { cssClass: "small-emoticon spaced-emoticon" },
      "8O":     { cssClass: "small-emoticon spaced-emoticon" },
      "[+=..]":  { cssClass: "no-rotate nintendo-controller" }
      //"OwO":  { cssClass: "no-rotate" }, // these emoticons overflow and look weird even if they're made even smaller, could probably fix this with some more css trickery
      //"O-O":  { cssClass: "no-rotate" },
      //"O=)":    { cssClass: "small-emoticon" } 
    }

    var specialRegex = new RegExp( '(\\' + escapeCharacters.join('|\\') + ')', 'g' );
    // One of these characters must be present before the matched emoticon, or the matched emoticon must be the first character in the container HTML
    //  This is to ensure that the characters in the middle of HTML properties or URLs are not matched as emoticons
    //  Below matches ^ (first character in container HTML), \s (whitespace like space or tab), or \0 (NULL character)
    // (<\\S+.*>) matches <\\S+.*> (matches an HTML tag like <span> or <div>), but haven't quite gotten it working yet, need to push this fix now
    var preMatch = '(^|[\\s\\0])';

    for ( var i=threeCharacterEmoticons.length-1; i>=0; --i ){
      threeCharacterEmoticons[i] = threeCharacterEmoticons[i].replace(specialRegex,'\\$1');
      threeCharacterEmoticons[i] = new RegExp( preMatch+'(' + threeCharacterEmoticons[i] + ')', 'g' );
    }

    for ( var i=twoCharacterEmoticons.length-1; i>=0; --i ){
      twoCharacterEmoticons[i] = twoCharacterEmoticons[i].replace(specialRegex,'\\$1');
      twoCharacterEmoticons[i] = new RegExp( preMatch+'(' + twoCharacterEmoticons[i] + ')', 'g' );
    }

    for ( var emoticon in specialEmoticons ){
      specialEmoticons[emoticon].regexp = emoticon.replace(specialRegex,'\\$1');
      specialEmoticons[emoticon].regexp = new RegExp( preMatch+'(' + specialEmoticons[emoticon].regexp + ')', 'g' );
    }

    var exclude = 'span.css-emoticon';
    if(opts.exclude){ exclude += ','+opts.exclude; }
    var excludeArray = exclude.split(',')

    return this.not(exclude).each(function() {
      var container = $(this);
      var cssClass = 'css-emoticon'
      if(opts.animate){ cssClass += ' un-transformed-emoticon animated-emoticon'; }
      
      for( var emoticon in specialEmoticons ){
        var specialCssClass = cssClass + " " + specialEmoticons[emoticon].cssClass;
        container.html(container.html().replace(specialEmoticons[emoticon].regexp,"$1<span class='" + specialCssClass + "'>$2</span>"));
      }
      $(threeCharacterEmoticons).each(function(){
        container.html(container.html().replace(this,"$1<span class='" + cssClass + "'>$2</span>"));
      });                                                          
      $(twoCharacterEmoticons).each(function(){                    
        container.html(container.html().replace(this,"$1<span class='" + cssClass + " spaced-emoticon'>$2</span>"));
      });
      // fix emoticons that got matched more then once (where one emoticon is a subset of another emoticon), and thus got nested spans
      $.each(excludeArray,function(index,item){
        container.find($.trim(item)+" span.css-emoticon").each(function(){
          $(this).replaceWith($(this).text());
        });
      });
      if(opts.animate){
        setTimeout(function(){$('.un-transformed-emoticon').removeClass('un-transformed-emoticon');}, opts.delay);
      }
    });
    }

    $.fn.unemoticonize = function(options) {
    var opts = $.extend({}, $.fn.emoticonize.defaults, options);
    return this.each(function() {
      var container = $(this);
      container.find('span.css-emoticon').each(function(){
        // add delay equal to animate speed if animate is not false
        var span = $(this);
        if(opts.animate){
          span.addClass('un-transformed-emoticon');
          setTimeout(function(){span.replaceWith(span.text());}, opts.delay); 
        }else{
          span.replaceWith(span.text());
        }
      });
    });
    }

    $.fn.emoticonize.defaults = {animate: true, delay: 500, exclude: 'pre,code,.no-emoticons'}
    })(jQuery);

    var chatboxManager = function () {
        
    var init = function (options) {
        $.extend(chatbox_config, options)
    };


    var delBox = function (id) {
        // TODO
    };

    var getNextOffset = function () {
        return (chatbox_config.width + chatbox_config.gap) * showList.length;
    };

    var boxClosedCallback = function (id) {
        // close button in the titlebar is clicked
        var idx = showList.indexOf(id);
        if (idx != -1) {
            showList.splice(idx, 1);
            var diff = chatbox_config.width + chatbox_config.gap;
            for (var i = idx; i < showList.length; i++) {
                chatbox_config.offset = $("#" + showList[i]).chatbox("option", "offset");
                $("#" + showList[i]).chatbox("option", "offset", chatbox_config.offset - diff);
            }
        } else {
            alert("NOTE: Id missing from array: " + id);
        }
    };

    // caller should guarantee the uniqueness of id
    var addBox = function (id, user, name) {
        var idx1 = showList.indexOf(id);
        var idx2 = boxList.indexOf(id);
        if (idx1 != -1) {
            // found one in show box, do nothing
        } else if (idx2 != -1) {
            // exists, but hidden
            // show it and put it back to showList
            $("#" + id).chatbox("option", "offset", getNextOffset());
            var manager = $("#" + id).chatbox("option", "boxManager");
            manager.toggleBox();
            showList.push(id);
        } else {
            var el = document.createElement('div');
            el.setAttribute('id', id);
            $(el).chatbox({
                id: id,
                user: user,
                title: '<i title="' + user.status + '"></i>' + user.first_name + " " + user.last_name,
                hidden: false,
                offset: getNextOffset(),
                width: chatbox_config.width,
                status: user.status,
                alertmsg: user.alertmsg,
                alertshow: user.alertshow,
                messageSent: dispatch,
                boxClosed: boxClosedCallback
            });
            boxList.push(id);
            showList.push(id);
            nameList.push(user.first_name);
        }
    };

    var messageSentCallback = function (id, user, msg) {
        var idx = boxList.indexOf(id);
        chatbox_config.messageSent(nameList[idx], msg);
    };

    // not used in demo
    var dispatch = function (id, user, msg) {
        //$("#log").append("<i>" + moment().calendar() + "</i> you said to <b>" + user.first_name + " " + user.last_name + ":</b> " + msg + "<br/>");
        if ($('#chatlog').length){
            $("#chatlog").append("You said to <b>" + user.first_name + " " + user.last_name + ":</b> " + msg + "<br/>").effect("highlight", {}, 500);;
        }
        $("#" + id).chatbox("option", "boxManager").addMsg("Me", msg);
    }

    return {
        init: init,
        addBox: addBox,
        delBox: delBox,
        dispatch: dispatch
    };
    }();

    var link = function (scope, element, attributes) {

        $('a[data-chat-id]').click(function (event, ui) {
            if(!$(this).hasClass('offline')){

                var $this = $(this),
                    temp_chat_id = $this.attr("data-chat-id"),
                    fname = $this.attr("data-chat-fname"),
                    lname = $this.attr("data-chat-lname"),
                    status = $this.attr("data-chat-status") || "online",
                    alertmsg = $this.attr("data-chat-alertmsg"),
                    alertshow =  $this.attr("data-chat-alertshow") || false;


                chatboxManager.addBox(temp_chat_id, {
                    // dest:"dest" + counter, 
                    // not used in demo
                    title: "username" + temp_chat_id,
                    first_name: fname,
                    last_name: lname,
                    status: status,
                    alertmsg: alertmsg,
                    alertshow: alertshow
                    //you can add your own options too
                });
            }

            event.preventDefault();

        });

    }

    angular.module('app.chat').directive('asideChatWidget', function (ChatApi) {
        return {
            restrict: 'A',
            replace: true,
            templateUrl: 'app/dashboard/chat/directives/aside-chat-widget.tpl.html',
            link: link
        }
    });

})(); 
"use strict";

angular.module('app.chat').directive('chatUsers', function(ChatApi){
    return {
        restrict: 'E',
        replace: true,
        templateUrl: 'app/dashboard/chat/directives/chat-users.tpl.html',
        scope: true,
        link: function(scope, element){
            scope.open = false;
            scope.openToggle = function(){
                scope.open = !scope.open;
            };

            scope.chatUserFilter = '';

            ChatApi.initialized.then(function () {
                scope.chatUsers = ChatApi.users;
            });
        }
    }
});

"use strict";

angular.module('app.chat').directive('chatWidget', function (ChatApi) {
    return {
        replace: true,
        restrict: 'E',
        templateUrl: 'app/dashboard/chat/directives/chat-widget.tpl.html',
        scope: {},
        link: function (scope, element) {
            scope.newMessage = '';

            scope.sendMessage = function () {
                ChatApi.sendMessage(scope.newMessage);
                scope.newMessage = '';
            };

            scope.messageTo = function(user){
                scope.newMessage += (user.username + ', ');
            };

            ChatApi.initialized.then(function () {
                scope.chatMessages = ChatApi.messages;
            });
            scope.$watch(function () {
                return ChatApi.messages.length
            }, function (count) {
                if (count){
                    var $body = $('.chat-body', element);
                    $body.animate({scrollTop: $body[0].scrollHeight});
                }
            })
        }
    }
});
"use strict";

 angular.module('app').directive('todoList', function ($timeout, Todo) {

    return {
        restrict: 'E',
        replace: true,
        templateUrl: 'app/dashboard/todo/directives/todo-list.tpl.html',
        scope: {
            todos: '='
        },
        link: function (scope, element, attributes) {
            scope.title = attributes.title
            scope.icon = attributes.icon
            scope.state = attributes.state
            scope.filter = {
                state: scope.state
            }

            element.find('.todo').sortable({
                handle: '.handle',
                connectWith: ".todo",
                receive: function (event, ui) {

                    console.log(ui.item.scope().todo,scope.state)
                    var todo = ui.item.scope().todo;
                    var state = scope.state
                    // // console.log(ui.item, todo, state)
                    // // console.log(state, todo)
                    if (todo && state) {
                        todo.setState(state);
                         // ui.sender.sortable("cancel");
                        // scope.$apply();
                    } else {
                        console.log('Wat', todo, state);
                    }
                    
                }
            }).disableSelection();

        }
    }
});
"use strict";

angular.module('app').factory('Todo', function (Restangular, APP_CONFIG) {


    Restangular.extendModel(APP_CONFIG.apiRootUrl + '/todos.json', function(todo) {
        todo.toggle = function(){
            if (!todo.completedAt) {
                todo.state = 'Completed';
                todo.completedAt = JSON.stringify(new Date());
            } else {
                todo.state = 'Critical';
                todo.completedAt = null;
            }
            // return this.$update();
        };

        todo.setState = function(state){
            todo.state = state;
            if (state == 'Completed') {
                todo.completedAt = JSON.stringify(new Date());
            } else {
                todo.completedAt = null;
            }
            // return this.$update();
        };

        return todo;
      });

    return Restangular.all(APP_CONFIG.apiRootUrl + '/todos.json')
});
'use strict';

angular.module('app.homepage').directive('demoBarChart', function ($http, APP_CONFIG) {
    return {
        restrict: 'A',
        link: function (scope, element, attributes) {

            var barOptions = {
                //Boolean - Whether the scale should start at zero, or an order of magnitude down from the lowest value
                scaleBeginAtZero : true,
                //Boolean - Whether grid lines are shown across the chart
                scaleShowGridLines : true,
                //String - Colour of the grid lines
                scaleGridLineColor : "rgba(0,0,0,.05)",
                //Number - Width of the grid lines
                scaleGridLineWidth : 1,
                //Boolean - If there is a stroke on each bar
                barShowStroke : true,
                //Number - Pixel width of the bar stroke
                barStrokeWidth : 1,
                //Number - Spacing between each of the X value sets
                barValueSpacing : 5,
                //Number - Spacing between data sets within X values
                barDatasetSpacing : 1,
                //Boolean - Re-draw chart on page resize
                responsive: true,
                //String - A legend template
                legendTemplate : "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].lineColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>"
            }

            var barChartUrl = scope.pageparams["barChart"];

            if (barChartUrl) {
                $http.get(APP_CONFIG.ebaasRootUrl + encodeURIComponent(barChartUrl))
                    .success(function (res) {
                        scope.barChartTitle = res.title;
                        var ctx = element[0].getContext("2d");
                        new Chart(ctx).Bar(res.chart, barOptions);
                    })
            }

        }
    }
});
'use strict';

angular.module('app.homepage').directive('demoDoughnutChart', function ($http, APP_CONFIG) {
    return {
        restrict: 'A',
        link: function (scope, element, attributes) {
            var doughnutOptions = {
                //Boolean - Whether we should show a stroke on each segment
                segmentShowStroke : true,
                //String - The colour of each segment stroke
                segmentStrokeColor : "#fff",
                //Number - The width of each segment stroke
                segmentStrokeWidth : 2,
                //Number - The percentage of the chart that we cut out of the middle
                percentageInnerCutout : 50, // This is 0 for Pie charts
                //Number - Amount of animation steps
                animationSteps : 100,
                //String - Animation easing effect
                animationEasing : "easeOutBounce",
                //Boolean - Whether we animate the rotation of the Doughnut
                animateRotate : true,
                //Boolean - Whether we animate scaling the Doughnut from the centre
                animateScale : false,
                //Boolean - Re-draw chart on page resize
                responsive: true,
                //String - A legend template
                legendTemplate : "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<segments.length; i++){%><li><span style=\"background-color:<%=segments[i].fillColor%>\"></span><%if(segments[i].label){%><%=segments[i].label%><%}%></li><%}%></ul>"
            };


            var doughnutChartUrl = scope.pageparams["doughnutChart"];

            if (doughnutChartUrl) {
                $http.get(APP_CONFIG.ebaasRootUrl + encodeURIComponent(doughnutChartUrl))
                    .success(function (res) {
                        scope.doughnutChartTitle = res.title;
                        // render chart
                        var ctx = element[0].getContext("2d");
                        new Chart(ctx).Doughnut(res.chart, doughnutOptions);
                    })
            }
        }}
});
'use strict';

angular.module('app.homepage').directive('demoLineChart', function ($http, APP_CONFIG) {
    return {
        restrict: 'A',
        link: function (scope, element, attributes) {

            // LINE CHART
            // ref: http://www.chartjs.org/docs/#line-chart-introduction
            var lineOptions = {
                ///Boolean - Whether grid lines are shown across the chart
                scaleShowGridLines : true,
                //String - Colour of the grid lines
                scaleGridLineColor : "rgba(0,0,0,.05)",
                //Number - Width of the grid lines
                scaleGridLineWidth : 1,
                //Boolean - Whether the line is curved between points
                bezierCurve : true,
                //Number - Tension of the bezier curve between points
                bezierCurveTension : 0.4,
                //Boolean - Whether to show a dot for each point
                pointDot : true,
                //Number - Radius of each point dot in pixels
                pointDotRadius : 4,
                //Number - Pixel width of point dot stroke
                pointDotStrokeWidth : 1,
                //Number - amount extra to add to the radius to cater for hit detection outside the drawn point
                pointHitDetectionRadius : 20,
                //Boolean - Whether to show a stroke for datasets
                datasetStroke : true,
                //Number - Pixel width of dataset stroke
                datasetStrokeWidth : 2,
                //Boolean - Whether to fill the dataset with a colour
                datasetFill : true,
                //Boolean - Re-draw chart on page resize
                responsive: true,
                //String - A legend template
                legendTemplate : "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].lineColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>"
            };

            var lineChartUrl = scope.pageparams["lineChart"];
            
            if (lineChartUrl) {
                $http.get(APP_CONFIG.ebaasRootUrl + encodeURIComponent(lineChartUrl))
                    .success(function (res) {
                        scope.lineChartTitle = res.title;
                        var ctx = element[0].getContext("2d");
                        var myNewChart = new Chart(ctx).Line(res.chart, lineOptions);
                    })
            }
        }
    }
});
'use strict';

angular.module('app.homepage').directive('demoPieChart', function ($http, APP_CONFIG) {
    return {
        restrict: 'A',
        link: function (scope, element, attributes) {
            var pieOptions = {
                //Boolean - Whether we should show a stroke on each segment
                segmentShowStroke: true,
                //String - The colour of each segment stroke
                segmentStrokeColor: "#fff",
                //Number - The width of each segment stroke
                segmentStrokeWidth: 2,
                //Number - Amount of animation steps
                animationSteps: 100,
                //String - types of animation
                animationEasing: "easeOutBounce",
                //Boolean - Whether we animate the rotation of the Doughnut
                animateRotate: true,
                //Boolean - Whether we animate scaling the Doughnut from the centre
                animateScale: false,
                //Boolean - Re-draw chart on page resize
                responsive: true,
                //String - A legend template
                legendTemplate : "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<segments.length; i++){%><li><span style=\"background-color:<%=segments[i].fillColor%>\"></span><%if(segments[i].label){%><%=segments[i].label%><%}%></li><%}%></ul>"
            };


            var pieChartUrl = scope.pageparams["pieChart"];

            if (pieChartUrl) {
                $http.get(APP_CONFIG.ebaasRootUrl + encodeURIComponent(pieChartUrl))
                    .success(function (res) {
                        scope.pieChartTitle = res.title;
                        // render chart
                        var ctx = element[0].getContext("2d");
                        var myNewChart = new Chart(ctx).Pie(res.chart, pieOptions);
                    })
            }
        }}
});
'use strict';

angular.module('app.homepage').directive('demoPolarChart', function ($http, APP_CONFIG) {
    return {
        restrict: 'A',
        link: function (scope, element, attributes) {
            var polarOptions = {
                //Boolean - Show a backdrop to the scale label
                scaleShowLabelBackdrop : true,
                //String - The colour of the label backdrop
                scaleBackdropColor : "rgba(255,255,255,0.75)",
                // Boolean - Whether the scale should begin at zero
                scaleBeginAtZero : true,
                //Number - The backdrop padding above & below the label in pixels
                scaleBackdropPaddingY : 2,
                //Number - The backdrop padding to the side of the label in pixels
                scaleBackdropPaddingX : 2,
                //Boolean - Show line for each value in the scale
                scaleShowLine : true,
                //Boolean - Stroke a line around each segment in the chart
                segmentShowStroke : true,
                //String - The colour of the stroke on each segement.
                segmentStrokeColor : "#fff",
                //Number - The width of the stroke value in pixels
                segmentStrokeWidth : 2,
                //Number - Amount of animation steps
                animationSteps : 100,
                //String - Animation easing effect.
                animationEasing : "easeOutBounce",
                //Boolean - Whether to animate the rotation of the chart
                animateRotate : true,
                //Boolean - Whether to animate scaling the chart from the centre
                animateScale : false,
                //Boolean - Re-draw chart on page resize
                responsive: true,
                //String - A legend template
                legendTemplate : "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<segments.length; i++){%><li><span style=\"background-color:<%=segments[i].fillColor%>\"></span><%if(segments[i].label){%><%=segments[i].label%><%}%></li><%}%></ul>"
            };

            var polarChartUrl = scope.pageparams["polarChart"];

            if (polarChartUrl) {
                $http.get(APP_CONFIG.ebaasRootUrl + encodeURIComponent(polarChartUrl))
                    .success(function (res) {
                        scope.polarChartTitle = res.title;
                        // render chart
                        var ctx = element[0].getContext("2d");
                        new Chart(ctx).PolarArea(res.chart, polarOptions);
                    })
            }
        }}
});
'use strict';

angular.module('app.homepage').directive('demoRadarChart', function ($http, APP_CONFIG) {
    return {
        restrict: 'A',
        link: function (scope, element, attributes) {

            var radarOptions = {
                //Boolean - Whether to show lines for each scale point
                scaleShowLine : true,
                //Boolean - Whether we show the angle lines out of the radar
                angleShowLineOut : true,
                //Boolean - Whether to show labels on the scale
                scaleShowLabels : false,
                // Boolean - Whether the scale should begin at zero
                scaleBeginAtZero : true,
                //String - Colour of the angle line
                angleLineColor : "rgba(0,0,0,.1)",
                //Number - Pixel width of the angle line
                angleLineWidth : 1,
                //String - Point label font declaration
                pointLabelFontFamily : "'Arial'",
                //String - Point label font weight
                pointLabelFontStyle : "normal",
                //Number - Point label font size in pixels
                pointLabelFontSize : 10,
                //String - Point label font colour
                pointLabelFontColor : "#666",
                //Boolean - Whether to show a dot for each point
                pointDot : true,
                //Number - Radius of each point dot in pixels
                pointDotRadius : 3,
                //Number - Pixel width of point dot stroke
                pointDotStrokeWidth : 1,
                //Number - amount extra to add to the radius to cater for hit detection outside the drawn point
                pointHitDetectionRadius : 20,
                //Boolean - Whether to show a stroke for datasets
                datasetStroke : true,
                //Number - Pixel width of dataset stroke
                datasetStrokeWidth : 2,
                //Boolean - Whether to fill the dataset with a colour
                datasetFill : true,
                //Boolean - Re-draw chart on page resize
                responsive: true,
                //String - A legend template
                legendTemplate : "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].lineColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>"
            }

            var radarChartUrl = scope.pageparams["radarChart"];

            if (radarChartUrl) {
                $http.get(APP_CONFIG.ebaasRootUrl + encodeURIComponent(radarChartUrl))
                    .success(function (res) {
                        scope.radarChartTitle = res.title;
                        // render chart
                        var ctx = element[0].getContext("2d");
                        var myNewChart = new Chart(ctx).Radar(res.chart, radarOptions);
                    })
            }
        }}
});
"use strict";

angular.module('SmartAdmin.Layout').directive('fullScreen', function(){
    return {
        restrict: 'A',
        link: function(scope, element){
            var $body = $('body');
            var toggleFullSceen = function(e){
                if (!$body.hasClass("full-screen")) {
                    $body.addClass("full-screen");
                    if (document.documentElement.requestFullscreen) {
                        document.documentElement.requestFullscreen();
                    } else if (document.documentElement.mozRequestFullScreen) {
                        document.documentElement.mozRequestFullScreen();
                    } else if (document.documentElement.webkitRequestFullscreen) {
                        document.documentElement.webkitRequestFullscreen();
                    } else if (document.documentElement.msRequestFullscreen) {
                        document.documentElement.msRequestFullscreen();
                    }
                } else {
                    $body.removeClass("full-screen");
                    if (document.exitFullscreen) {
                        document.exitFullscreen();
                    } else if (document.mozCancelFullScreen) {
                        document.mozCancelFullScreen();
                    } else if (document.webkitExitFullscreen) {
                        document.webkitExitFullscreen();
                    }
                }
            };

            element.on('click', toggleFullSceen);

        }
    }
});
"use strict";

angular.module('SmartAdmin.Layout').directive('minifyMenu', function(){
    return {
        restrict: 'A',
        link: function(scope, element){
                var $body = $('body');
            var minifyMenu = function() {
                if (!$body.hasClass("menu-on-top")) {
                    $body.toggleClass("minified");
                    $body.removeClass("hidden-menu");
                    $('html').removeClass("hidden-menu-mobile-lock");
                }
            };

            element.on('click', minifyMenu);
        }
    }
})
'use strict';

angular.module('SmartAdmin.Layout').directive('reloadState', function ($rootScope) {
    return {
        restrict: 'A',
        compile: function (tElement, tAttributes) {
            tElement.removeAttr('reload-state data-reload-state');
            tElement.on('click', function (e) {
                $rootScope.$state.transitionTo($rootScope.$state.current, $rootScope.$stateParams, {
                    reload: true,
                    inherit: false,
                    notify: true
                });
                e.preventDefault();
            })
        }
    }
});

"use strict";

angular.module('SmartAdmin.Layout').directive('resetWidgets', function($state){

    return {
        restrict: 'A',
        link: function(scope, element){
            element.on('click', function(){
                $.SmartMessageBox({
                    title : "<i class='fa fa-refresh' style='color:green'></i> Clear Local Storage",
                    content : "Would you like to RESET all your saved widgets and clear LocalStorage?1",
                    buttons : '[No][Yes]'
                }, function(ButtonPressed) {
                    if (ButtonPressed == "Yes" && localStorage) {
                        localStorage.clear();
                        location.reload()
                    }
                });

            });
        }
    }

});

'use strict';

angular.module('SmartAdmin.Layout').directive('searchMobile', function () {
    return {
        restrict: 'A',
        compile: function (element, attributes) {
            element.removeAttr('search-mobile data-search-mobile');

            element.on('click', function (e) {
                $('body').addClass('search-mobile');
                e.preventDefault();
            });

            $('#cancel-search-js').on('click', function (e) {
                $('body').removeClass('search-mobile');
                e.preventDefault();
            });
        }
    }
});
"use strict";

angular.module('SmartAdmin.Layout').directive('toggleMenu', function(){
    return {
        restrict: 'A',
        link: function(scope, element){
            var $body = $('body');

            var toggleMenu = function(){
                if (!$body.hasClass("menu-on-top")){
                    $('html').toggleClass("hidden-menu-mobile-lock");
                    $body.toggleClass("hidden-menu");
                    $body.removeClass("minified");
                } else if ( $body.hasClass("menu-on-top") && $body.hasClass("mobile-view-activated") ) {
                    $('html').toggleClass("hidden-menu-mobile-lock");
                    $body.toggleClass("hidden-menu");
                    $body.removeClass("minified");
                }
            };

            element.on('click', toggleMenu);

            scope.$on('requestToggleMenu', function(){
                toggleMenu();
            });
        }
    }
});
'use strict';

angular.module('SmartAdmin.Layout').directive('bigBreadcrumbs', function () {
    return {
        restrict: 'EA',
        replace: true,
        template: '<div><div class="pull-left"><h3 id="return-home" ui-sref="app.homepage.mainmenu" class="page-title txt-color-blueDark"><i class="pointer-icon fa-fw fa fa-home"></i></h3></div><div ng-show="false" class="pull-right hidden-mobile"><a ui-sref=".help" class="btn btn-info btn-sm"><i class="fa fa-question-circle fa-lg"></i>&nbsp;&nbsp;{{getWord("Help")}}</a></div></div>',
        scope: {
            items: '=',
            icon: '@'
        },
        controller: 'bigBreadcrumbsCtlr',
        link: function (scope, element) {
            var first = _.first(scope.items);

            //var icon = scope.icon || 'home';
            element.find('h3').append(first);
            _.rest(scope.items).forEach(function (item) {
                element.find('h3').append(' <span>> ' + item + '</span>')
            });
        }
    }
});

'use strict';

angular.module('SmartAdmin.Layout').controller('bigBreadcrumbsCtlr', function ($rootScope, $scope, $stateParams, $http, APP_CONFIG) {

    $scope.helpUrl = undefined;
    if ($stateParams.hash)
    {
        $scope.hasHashCode = true;
    }
    else
    {
        $scope.hasHashCode = false;
    }

    $scope.getWord = function(key)
    {
        return $rootScope.getWord(key);
    }

});

'use strict';

angular.module('SmartAdmin.Layout').directive('dismisser', function () {
    return {
        restrict: 'A',
        compile: function (element) {
            element.removeAttr('dismisser data-dissmiser')
            var closer = '<button class="close">&times;</button>';
            element.prepend(closer);
            element.on('click', '>button.close', function(){
                element.fadeOut('fast',function(){ $(this).remove(); });

            })
        }
    }
});
'use strict';

angular.module('SmartAdmin.Layout').directive('hrefVoid', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attributes) {
            element.attr('href','#');
            element.on('click', function(e){
                e.preventDefault();
                e.stopPropagation();
            })
        }
    }
});
'use strict';

/*
* Directive for toggling a ng-model with a button
* Source: https://gist.github.com/aeife/9374784
*/

angular.module('SmartAdmin.Layout').directive('radioToggle', function ($log) {
    return {
        scope: {
            model: "=ngModel",
            value: "@value"
        },
        link: function(scope, element, attrs) {

            element.parent().on('click', function() {
                scope.model = scope.value;
                scope.$apply();
            });
        }
    }
});
/**
 * DETECT MOBILE DEVICES
 * Description: Detects mobile device - if any of the listed device is
 *
 * detected class is inserted to <tElement>.
 *
 *  (so far this is covering most hand held devices)
 */
'use strict';

angular.module('SmartAdmin.Layout').directive('smartDeviceDetect', function () {
    return {
        restrict: 'A',
        compile: function (tElement, tAttributes) {
            tElement.removeAttr('smart-device-detect data-smart-device-detect');

            var isMobile = (/iphone|ipad|ipod|android|blackberry|mini|windows\sce|palm/i.test(navigator.userAgent.toLowerCase()));
            
            tElement.toggleClass('desktop-detected', !isMobile);
            tElement.toggleClass('mobile-detected', isMobile);


        }
    }
});
/**
 *
 * Description: Directive utilizes FastClick library.
 *
 *
 * FastClick is a simple, easy-to-use library for eliminating the
 * 300ms delay between a physical tap and the firing of a click event on mobile browsers.
 * FastClick doesn't attach any listeners on desktop browsers.
 * @link: https://github.com/ftlabs/fastclick
 *
 * On mobile devices 'needsclick' class is attached to <tElement>
 *
 */


'use strict';

angular.module('SmartAdmin.Layout').directive('smartFastClick', function () {
    return {
        restrict: 'A',
        compile: function (tElement, tAttributes) {
            tElement.removeAttr('smart-fast-click data-smart-fast-click');

            FastClick.attach(tElement);

            if(!FastClick.notNeeded())
                tElement.addClass('needsclick')
        }
    }
});

'use strict';

angular.module('SmartAdmin.Layout').directive('smartFitAppView', function ($rootScope, SmartCss) {
    return {
        restrict: 'A',
        compile: function (element, attributes) {
            element.removeAttr('smart-fit-app-view data-smart-fit-app-view leading-y data-leading-y');

            var leadingY = attributes.leadingY ? parseInt(attributes.leadingY) : 0;

            var selector = attributes.smartFitAppView;

            if(SmartCss.appViewSize && SmartCss.appViewSize.height){
                var height =  SmartCss.appViewSize.height - leadingY < 252 ? 252 :  SmartCss.appViewSize.height - leadingY;
                SmartCss.add(selector, 'height', height+'px');
            }

            var listenerDestroy = $rootScope.$on('$smartContentResize', function (event, data) {
                var height = data.height - leadingY < 252 ? 252 : data.height - leadingY;
                SmartCss.add(selector, 'height', height+'px');
            });

            element.on('$destroy', function () {
                listenerDestroy();
                SmartCss.remove(selector, 'height');
            });


        }
    }
});

"use strict";

angular.module('SmartAdmin.Layout').directive('smartInclude', function () {
        return {
            replace: true,
            restrict: 'A',
            templateUrl: function (element, attr) {
                return attr.smartInclude;
            },
            compile: function(element){
                element[0].className = element[0].className.replace(/placeholder[^\s]+/g, '');
            }
        };
    }
);


'use strict';

angular.module('SmartAdmin.Layout').directive('smartLayout', function ($rootScope, $timeout, $interval, $q, $location, SmartCss, authService) {
    
    var _debug = 0;

    function getDocHeight() {
        var D = document;
        return Math.max(
            D.body.scrollHeight, D.documentElement.scrollHeight,
            D.body.offsetHeight, D.documentElement.offsetHeight,
            D.body.clientHeight, D.documentElement.clientHeight
        );
    }

    var initialized = false, 
           initializedResolver = $q.defer();
    initializedResolver.promise.then(function () {
        initialized = true;
    });

    var $window = $(window),
        $document = $(document),
        $html = $('html'),
        $body = $('body'),
        $navigation ,
        $menu,
        $ribbon,
        $footer,
        $contentAnimContainer;


    (function cacheElements() {
        $navigation = $('#header');
        $menu = $('#left-panel');
        $ribbon = $('#ribbon');
        $footer = $('.page-footer');
        if (_.every([$navigation, $menu, $ribbon, $footer], function ($it) {
            return angular.isNumber($it.height())
        })) {
            initializedResolver.resolve();
        } else {
            $timeout(cacheElements, 100);
        }
    })();


    return {
        priority: 2014,
        restrict: 'A',
        compile: function (tElement, tAttributes) {
            tElement.removeAttr('smart-layout data-smart-layout');

            var appViewHeight = 0 ,
                appViewWidth = 0,
                calcWidth,
                calcHeight,
                deltaX,
                deltaY;

            var forceResizeTrigger = false;

            function resizeListener() {

//                    full window height appHeight = Math.max($menu.outerHeight() - 10, getDocHeight() - 10);

                var menuHeight = $body.hasClass('menu-on-top') && $menu.is(':visible') ? $menu.height() : 0;
                var menuWidth = !$body.hasClass('menu-on-top') && $menu.is(':visible') ? $menu.width() + $menu.offset().left : 0;

                var $content = $('#content');
                var contentXPad = $content.outerWidth(true) - $content.width();
                var contentYPad = $content.outerHeight(true) - $content.height();


                calcWidth = $window.width() - menuWidth - contentXPad;
                calcHeight = $window.height() - menuHeight - contentYPad - $navigation.height() - $ribbon.height() - $footer.height();

                deltaX = appViewWidth - calcWidth;
                deltaY = appViewHeight - calcHeight;
                if (Math.abs(deltaX) || Math.abs(deltaY) || forceResizeTrigger) {

                    //console.log('exec', calcWidth, calcHeight);
                    $rootScope.$broadcast('$smartContentResize', {
                        width: calcWidth,
                        height: calcHeight,
                        deltaX: deltaX,
                        deltaY: deltaY
                    });
                    appViewWidth = calcWidth;
                    appViewHeight = calcHeight;
                    forceResizeTrigger = false;
                }
            }


            var looping = false;
            $interval(function () {
                if (looping) loop();
            }, 300);

            var debouncedRun = _.debounce(function () {
                run(300)
            }, 300);

            function run(delay) {
                initializedResolver.promise.then(function () {
                    attachOnResize(delay);
                });
            }

            run(10);

            function detachOnResize() {
                looping = false;
            }

            function attachOnResize(delay) {
                $timeout(function () {
                    looping = true;
                }, delay);
            }

            function loop() {
                $body.toggleClass('mobile-view-activated', $window.width() < 979);

                if ($window.width() < 979)
                    $body.removeClass('minified');

                resizeListener();
            }

            function handleHtmlId(toState) {
                if (toState.data && toState.data.htmlId) $html.attr('id', toState.data.htmlId);
                else $html.removeAttr('id');
            }

            // Added
            function handleLogin(toState, toParams) {
                if (toState.authenticate && !authService.authentication.isAuth) {
                    $rootScope.returnToState = toState.url;
                    authService.logOut();
                    $location.path('/login');
                }
            }

            $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
                //console.log(1, '$stateChangeStart', event, toState, toState.authenticate, toParams, fromState, fromParams);
 
                handleHtmlId(toState);
                detachOnResize();

                handleLogin(toState, toParams);
            });

            // initialized with 1 cause we came here with one $viewContentLoading request
            var viewContentLoading = 1;
            $rootScope.$on('$viewContentLoading', function (event, viewConfig) {
                //console.log(2, '$viewContentLoading', event, viewConfig);
                viewContentLoading++;
            });

            $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
                //console.log(3, '$stateChangeSuccess', event, toState, toParams, fromState, fromParams);
                forceResizeTrigger = true;
            });

            $rootScope.$on('$viewContentLoaded', function (event) {
                //console.log(4, '$viewContentLoaded', event);
                viewContentLoading--;

                if (viewContentLoading == 0 && initialized) {
                    debouncedRun();
                }
            });
        }
    }
});



'use strict';

angular.module('SmartAdmin.Layout').directive('smartPageTitle', function ($rootScope, $timeout) {
    return {
        restrict: 'A',
        compile: function (element, attributes) {
            element.removeAttr('smart-page-title data-smart-page-title');

            var defaultTitle = attributes.smartPageTitle;
            var listener = function(event, toState, toParams, fromState, fromParams) {
                var title = defaultTitle;
                if (toState.data && toState.data.title) title = toState.data.title + ' | ' + title;
                // Set asynchronously so page changes before title does
                $timeout(function() {
                    $('html head title').text(title);
                });
            };

            $rootScope.$on('$stateChangeStart', listener);

        }
    }
});
'use strict';

angular.module('SmartAdmin.Layout').directive('smartRouterAnimationWrap', function ($rootScope,$timeout) {
    return {
        restrict: 'A',
        compile: function (element, attributes) {
            element.removeAttr('smart-router-animation-wrap data-smart-router-animation-wrap wrap-for data-wrap-for');

            element.addClass('router-animation-container');


            var $loader = $('<div class="router-animation-loader"><i class="fa fa-gear fa-4x fa-spin"></i></div>')
                .css({
                    position: 'absolute',
                    top: 50,
                    left: 10
                }).hide().appendTo(element);


            var animateElementSelector = attributes.wrapFor;
            var viewsToMatch = attributes.smartRouterAnimationWrap.split(/\s/);

            var needRunContentViewAnimEnd = false;
            function contentViewAnimStart() {
                needRunContentViewAnimEnd = true;
                element.css({
                    height: element.height() + 'px',
                    overflow: 'hidden'
                }).addClass('active');
                $loader.fadeIn();

                $(animateElementSelector).addClass('animated faster fadeOutDown');
            }

            function contentViewAnimEnd() {
                if(needRunContentViewAnimEnd){
                    element.css({
                        height: 'auto',
                        overflow: 'visible'
                    }).removeClass('active');
                    

                    $(animateElementSelector).addClass('animated faster fadeInUp');

                    needRunContentViewAnimEnd = false;

                    $timeout(function(){
                        $(animateElementSelector).removeClass('animated');
                    },10);
                }
                $loader.fadeOut();
            }


            var destroyForStart = $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
                if (toState.data.animation !== false) {
                    var isAnimRequired = _.any(viewsToMatch, function (view) {
                        return _.has(toState.views, view) || _.has(fromState.views, view);
                    });
                    if (isAnimRequired) {
                        contentViewAnimStart()
                    }
                }
            });

            var destroyForEnd = $rootScope.$on('$viewContentLoaded', function (event) {
                contentViewAnimEnd();
            });

            element.on('$destroy', function(){
                destroyForStart();
                destroyForEnd();

            });



        }
    }
});
angular.module('SmartAdmin.Layout').directive('speechRecognition', function ($log) {
	'use strict';

	$.root_ = $('body');
	var root, commands;

    root = window;
    window.appConfig = window.appConfig || {};

	if (appConfig.voice_command) {
		commands = appConfig.commands;
	}


	/*
	 * SMART VOICE
	 * Author: MyOrange | @bootstraphunt
	 * http://www.myorange.ca
	 */

	var SpeechRecognition = root.SpeechRecognition || root.webkitSpeechRecognition || root.mozSpeechRecognition || root.msSpeechRecognition || root.oSpeechRecognition;

// ref: http://updates.html5rocks.com/2013/01/Voice-Driven-Web-Apps-Introduction-to-the-Web-Speech-API


// function
	$.speechApp = (function(speech) {

		speech.start = function() {

			// Add our commands to smartSpeechRecognition
			smartSpeechRecognition.addCommands(commands);

			if (smartSpeechRecognition) {
				// activate plugin
				smartSpeechRecognition.start();
				// add btn class
				$.root_.addClass("voice-command-active");
				// play sound
				$.speechApp.playON();
				// set localStorage when switch is on manually
				if (appConfig.voice_localStorage) {
					localStorage.setItem('sm-setautovoice', 'true');
				}

			} else {
				// if plugin not found
				alert("speech plugin not loaded");
			}

		};
		speech.stop = function() {

			if (smartSpeechRecognition) {
				// deactivate plugin
				smartSpeechRecognition.abort();
				// remove btn class
				$.root_.removeClass("voice-command-active");
				// sound
				$.speechApp.playOFF();
				// del localStorage when switch if off manually
				if (appConfig.voice_localStorage) {
					localStorage.setItem('sm-setautovoice', 'false');
				}
				// remove popover if visible
				if ($('#speech-btn .popover').is(':visible')) {
					$('#speech-btn .popover').fadeOut(250);
				}
			}

		};

		// play sound
		speech.playON = function() {

			var audioElement = document.createElement('audio');

			if (navigator.userAgent.match('Firefox/'))
				audioElement.setAttribute('src', appConfig.sound_path + 'voice_on' + ".ogg");
			else
				audioElement.setAttribute('src', appConfig.sound_path + 'voice_on' + ".mp3");

			//$.get();
			audioElement.addEventListener("load", function() {
				audioElement.play();
			}, true);

			if (appConfig.sound_on) {
				audioElement.pause();
				audioElement.play();
			}
		};

		speech.playOFF = function() {

			var audioElement = document.createElement('audio');

			if (navigator.userAgent.match('Firefox/'))
				audioElement.setAttribute('src', appConfig.sound_path + 'voice_off' + ".ogg");
			else
				audioElement.setAttribute('src', appConfig.sound_path + 'voice_off' + ".mp3");

			$.get();
			audioElement.addEventListener("load", function() {
				audioElement.play();
			}, true);

			if (appConfig.sound_on) {
				audioElement.pause();
				audioElement.play();
			}
		};

		speech.playConfirmation = function() {

			var audioElement = document.createElement('audio');

			if (navigator.userAgent.match('Firefox/'))
				audioElement.setAttribute('src', appConfig.sound_path + 'voice_alert' + ".ogg");
			else
				audioElement.setAttribute('src', appConfig.sound_path + 'voice_alert' + ".mp3");

			$.get();
			audioElement.addEventListener("load", function() {
				audioElement.play();
			}, true);

			if (appConfig.sound_on) {
				audioElement.pause();
				audioElement.play();
			}
		};

		return speech;

	})({});



	/*
	 * SPEECH RECOGNITION ENGINE
	 * Copyright (c) 2013 Tal Ater
	 * Modified by MyOrange
	 * All modifications made are hereby copyright (c) 2014 MyOrange
	 */

	(function(undefined) {"use strict";

		// Check browser support
		// This is done as early as possible, to make it as fast as possible for unsupported browsers
		if (!SpeechRecognition) {
			root.smartSpeechRecognition = null;
			return undefined;
		}

		var commandsList = [], recognition, callbacks = {
				start : [],
				error : [],
				end : [],
				result : [],
				resultMatch : [],
				resultNoMatch : [],
				errorNetwork : [],
				errorPermissionBlocked : [],
				errorPermissionDenied : []
			}, autoRestart, lastStartedAt = 0,
		//debugState = false, // decleared in app.appConfig.js
		//appConfig.debugStyle = 'font-weight: bold; color: #00f;', // decleared in app.appConfig.js

		// The command matching code is a modified version of Backbone.Router by Jeremy Ashkenas, under the MIT license.
			optionalParam = /\s*\((.*?)\)\s*/g, optionalRegex = /(\(\?:[^)]+\))\?/g, namedParam = /(\(\?)?:\w+/g, splatParam = /\*\w+/g, escapeRegExp = /[\-{}\[\]+?.,\\\^$|#]/g, commandToRegExp = function(command) {
				command = command.replace(escapeRegExp, '\\$&').replace(optionalParam, '(?:$1)?').replace(namedParam, function(match, optional) {
					return optional ? match : '([^\\s]+)';
				}).replace(splatParam, '(.*?)').replace(optionalRegex, '\\s*$1?\\s*');
				return new RegExp('^' + command + '$', 'i');
			};

		// This method receives an array of callbacks to iterate over, and invokes each of them
		var invokeCallbacks = function(callbacks) {
			callbacks.forEach(function(callback) {
				callback.callback.apply(callback.context);
			});
		};

		var initIfNeeded = function() {
			if (!isInitialized()) {
				root.smartSpeechRecognition.init({}, false);
			}
		};

		var isInitialized = function() {
			return recognition !== undefined;
		};

		root.smartSpeechRecognition = {
			// Initialize smartSpeechRecognition with a list of commands to recognize.
			// e.g. smartSpeechRecognition.init({'hello :name': helloFunction})
			// smartSpeechRecognition understands commands with named variables, splats, and optional words.
			init : function(commands, resetCommands) {

				// resetCommands defaults to true
				if (resetCommands === undefined) {
					resetCommands = true;
				} else {
					resetCommands = !!resetCommands;
				}

				// Abort previous instances of recognition already running
				if (recognition && recognition.abort) {
					recognition.abort();
				}

				// initiate SpeechRecognition
				recognition = new SpeechRecognition();

				// Set the max number of alternative transcripts to try and match with a command
				recognition.maxAlternatives = 5;
				recognition.continuous = true;
				// Sets the language to the default 'en-US'. This can be changed with smartSpeechRecognition.setLanguage()
				recognition.lang = appConfig.voice_command_lang || 'en-US';

				recognition.onstart = function() {
					invokeCallbacks(callbacks.start);
					//debugState
					if (appConfig.debugState) {
						root.console.log('%c ✔ SUCCESS: User allowed access the microphone service to start ', appConfig.debugStyle_success);
						root.console.log('Language setting is set to: ' + recognition.lang, appConfig.debugStyle);
					}
					$.root_.removeClass("service-not-allowed");
					$.root_.addClass("service-allowed");
				};

				recognition.onerror = function(event) {
					invokeCallbacks(callbacks.error);
					switch (event.error) {
						case 'network':
							invokeCallbacks(callbacks.errorNetwork);
							break;
						case 'not-allowed':
						case 'service-not-allowed':
							// if permission to use the mic is denied, turn off auto-restart
							autoRestart = false;
							$.root_.removeClass("service-allowed");
							$.root_.addClass("service-not-allowed");
							//debugState
							if (appConfig.debugState) {
								root.console.log('%c WARNING: Microphone was not detected (either user denied access or it is not installed properly) ', appConfig.debugStyle_warning);
							}
							// determine if permission was denied by user or automatically.
							if (new Date().getTime() - lastStartedAt < 200) {
								invokeCallbacks(callbacks.errorPermissionBlocked);
							} else {
								invokeCallbacks(callbacks.errorPermissionDenied);
								//console.log("You need your mic to be active")
							}
							break;
					}
				};

				recognition.onend = function() {
					invokeCallbacks(callbacks.end);
					// smartSpeechRecognition will auto restart if it is closed automatically and not by user action.
					if (autoRestart) {
						// play nicely with the browser, and never restart smartSpeechRecognition automatically more than once per second
						var timeSinceLastStart = new Date().getTime() - lastStartedAt;
						if (timeSinceLastStart < 1000) {
							setTimeout(root.smartSpeechRecognition.start, 1000 - timeSinceLastStart);
						} else {
							root.smartSpeechRecognition.start();
						}
					}
				};

				recognition.onresult = function(event) {
					invokeCallbacks(callbacks.result);

					var results = event.results[event.resultIndex], commandText;

					// go over each of the 5 results and alternative results received (we've set maxAlternatives to 5 above)
					for (var i = 0; i < results.length; i++) {
						// the text recognized
						commandText = results[i].transcript.trim();
						if (appConfig.debugState) {
							root.console.log('Speech recognized: %c' + commandText, appConfig.debugStyle);
						}

						// try and match recognized text to one of the commands on the list
						for (var j = 0, l = commandsList.length; j < l; j++) {
							var result = commandsList[j].command.exec(commandText);
							if (result) {
								var parameters = result.slice(1);
								if (appConfig.debugState) {
									root.console.log('command matched: %c' + commandsList[j].originalPhrase, appConfig.debugStyle);
									if (parameters.length) {
										root.console.log('with parameters', parameters);
									}
								}
								// execute the matched command
								commandsList[j].callback.apply(this, parameters);
								invokeCallbacks(callbacks.resultMatch);

								// for commands "sound on", "stop" and "mute" do not play sound or display message
								//var myMatchedCommand = commandsList[j].originalPhrase;

								var ignoreCallsFor = ["sound on", "mute", "stop"];

								if (ignoreCallsFor.indexOf(commandsList[j].originalPhrase) < 0) {
									// play sound when match found
									console.log(2);
									$.smallBox({
										title : (commandsList[j].originalPhrase),
										content : "loading...",
										color : "#333",
										sound_file : 'voice_alert',
										timeout : 2000
									});

									if ($('#speech-btn .popover').is(':visible')) {
										$('#speech-btn .popover').fadeOut(250);
									}
								}// end if

								return true;
							}
						} // end for
					}// end for

					invokeCallbacks(callbacks.resultNoMatch);
					//console.log("no match found for: " + commandText)
					$.smallBox({
						title : "Error: <strong>" + ' " ' + commandText + ' " ' + "</strong> no match found!",
						content : "Please speak clearly into the microphone",
						color : "#a90329",
						timeout : 5000,
						icon : "fa fa-microphone"
					});
					if ($('#speech-btn .popover').is(':visible')) {
						$('#speech-btn .popover').fadeOut(250);
					}
					return false;
				};

				// build commands list
				if (resetCommands) {
					commandsList = [];
				}
				if (commands.length) {
					this.addCommands(commands);
				}
			},

			// Start listening (asking for permission first, if needed).
			// Call this after you've initialized smartSpeechRecognition with commands.
			// Receives an optional options object:
			// { autoRestart: true }
			start : function(options) {
				initIfNeeded();
				options = options || {};
				if (options.autoRestart !== undefined) {
					autoRestart = !!options.autoRestart;
				} else {
					autoRestart = true;
				}
				lastStartedAt = new Date().getTime();
				recognition.start();
			},

			// abort the listening session (aka stop)
			abort : function() {
				autoRestart = false;
				if (isInitialized) {
					recognition.abort();
				}
			},

			// Turn on output of debug messages to the console. Ugly, but super-handy!
			debug : function(newState) {
				if (arguments.length > 0) {
					appConfig.debugState = !!newState;
				} else {
					appConfig.debugState = true;
				}
			},

			// Set the language the user will speak in. If not called, defaults to 'en-US'.
			// e.g. 'fr-FR' (French-France), 'es-CR' (Español-Costa Rica)
			setLanguage : function(language) {
				initIfNeeded();
				recognition.lang = language;
			},

			// Add additional commands that smartSpeechRecognition will respond to. Similar in syntax to smartSpeechRecognition.init()
			addCommands : function(commands) {
				var cb, command;

				initIfNeeded();

				for (var phrase in commands) {
					if (commands.hasOwnProperty(phrase)) {
						cb = root[commands[phrase]] || commands[phrase];
						if ( typeof cb !== 'function') {
							continue;
						}
						//convert command to regex
						command = commandToRegExp(phrase);

						commandsList.push({
							command : command,
							callback : cb,
							originalPhrase : phrase
						});
					}
				}
				if (appConfig.debugState) {
					root.console.log('Commands successfully loaded: %c' + commandsList.length, appConfig.debugStyle);
				}
			},

			// Remove existing commands. Called with a single phrase, array of phrases, or methodically. Pass no params to remove all commands.
			removeCommands : function(commandsToRemove) {
				if (commandsToRemove === undefined) {
					commandsList = [];
					return;
				}
				commandsToRemove = Array.isArray(commandsToRemove) ? commandsToRemove : [commandsToRemove];
				commandsList = commandsList.filter(function(command) {
					for (var i = 0; i < commandsToRemove.length; i++) {
						if (commandsToRemove[i] === command.originalPhrase) {
							return false;
						}
					}
					return true;
				});
			},

			// Lets the user add a callback of one of 9 types:
			// start, error, end, result, resultMatch, resultNoMatch, errorNetwork, errorPermissionBlocked, errorPermissionDenied
			// Can also optionally receive a context for the callback function as the third argument
			addCallback : function(type, callback, context) {
				if (callbacks[type] === undefined) {
					return;
				}
				var cb = root[callback] || callback;
				if ( typeof cb !== 'function') {
					return;
				}
				callbacks[type].push({
					callback : cb,
					context : context || this
				});
			}
		};

	}).call(this);

	var autoStart = function() {

		smartSpeechRecognition.addCommands(commands);

		if (smartSpeechRecognition) {
			// activate plugin
			smartSpeechRecognition.start();
			// add btn class
			$.root_.addClass("voice-command-active");
			// set localStorage when switch is on manually
			if (appConfig.voice_localStorage) {
				localStorage.setItem('sm-setautovoice', 'true');
			}

		} else {
			// if plugin not found
			alert("speech plugin not loaded");
		}
	}
// if already running with localstorage
	if (SpeechRecognition && appConfig.voice_command && localStorage.getItem('sm-setautovoice') == 'true') {
		autoStart();
	}

// auto start
	if (SpeechRecognition && appConfig.voice_command_auto && appConfig.voice_command) {
		autoStart();
	}


	var link = function(scope, element) {


		if (SpeechRecognition && appConfig.voice_command) {

			// create dynamic modal instance
			var modal = $('<div class="modal fade" id="voiceModal" tabindex="-1" role="dialog" aria-labelledby="remoteModalLabel" aria-hidden="true"><div class="modal-dialog"><div class="modal-content"></div></div></div>');
			// attach to body
			modal.appendTo("body");

			element.on("click", function(e) {

            	if ($.root_.hasClass("voice-command-active")) {
					$.speechApp.stop();
					//$('#speech-btn > span > a > i').removeClass().addClass('fa fa-microphone-slash');
				} else {
					$.speechApp.start();
					//add popover
					$('#speech-btn .popover').fadeIn(350);
					//$('#speech-btn > span > a > i').removeClass().addClass('fa fa-microphone')

				}

				e.preventDefault();

            });

			//remove popover
			$(document).mouseup(function(e) {
				if (!$('#speech-btn .popover').is(e.target) && $('#speech-btn .popover').has(e.target).length === 0) {
					$('#speech-btn .popover').fadeOut(250);
				}
			});


			$("#speech-help-btn").on("click", function() {
				commands.help();
			});

		}
		else {
			$("#speech-btn").addClass("display-none");
		}


	}



    return {
        restrict: 'AE',
        link: link
    }
});

'use strict';

angular.module('SmartAdmin.Layout').directive('stateBreadcrumbs', function ($rootScope, $state) {


    return {
        restrict: 'EA',
        replace: true,
        template: '<ol class="breadcrumb"><li>Home</li></ol>',
        link: function (scope, element) {

            function setBreadcrumbs(breadcrumbs) {
                var html = '<li>Home</li>';
                angular.forEach(breadcrumbs, function (crumb) {
                    html += '<li>' + crumb + '</li>'
                });
                element.html(html)
            }

            function fetchBreadcrumbs(stateName, breadcrunbs) {

                var state = $state.get(stateName);

                if (state && state.data && state.data.title && breadcrunbs.indexOf(state.data.title) == -1) {
                    breadcrunbs.unshift(state.data.title)
                }

                var parentName = stateName.replace(/.?\w+$/, '');
                if (parentName) {
                    return fetchBreadcrumbs(parentName, breadcrunbs);
                } else {
                    return breadcrunbs;
                }
            }

            function processState(state) {
                var breadcrumbs;
                if (state.data && state.data.breadcrumbs) {
                    breadcrumbs = state.data.breadcrumbs;
                } else {
                    breadcrumbs = fetchBreadcrumbs(state.name, []);
                }
                setBreadcrumbs(breadcrumbs);
            }

            processState($state.current);

            $rootScope.$on('$stateChangeStart', function (event, state) {
                processState(state);
            })
        }
    }
});
'use strict';

angular.module('SmartAdmin.Layout').factory('lazyScript', function($q, $http){
    var scripts = null;
    var initialized = false;
    var initializingPromise = null;

    function init(){        
        if(!initialized){
            if(!initializingPromise){
                initializingPromise = $http.get('app.scripts.json').then(function(res){
                    scripts = res.data
                    initialized = true;
                });     
            }
            return initializingPromise;
               
        } else {
            return $q.resolve();
        }
    }

    var cache = {};

    function isPending(scriptName){
        return (cache.hasOwnProperty(scriptName) && cache[scriptName].promise && cache[scriptName].promise.$$state.pending)
    }

    function isRegistered(scriptName){
        if(cache.hasOwnProperty(scriptName)){
            return true;
        } else {
            return (scripts.prebuild.indexOf(scriptName) > -1);
        }
    }
    function loadScript(scriptName){
        if(!cache[scriptName]){
            cache[scriptName] = $q.defer();
            var el = document.createElement( 'script' );
            el.onload = function(script){
                console.log('script is lazy loaded:', scriptName)
                cache[scriptName].resolve(scriptName);
            };
            el.src = scripts.paths[scriptName];
            var x = document.getElementsByTagName('script')[0];
            x.parentNode.insertBefore(el, x);
            
        }
        return cache[scriptName].promise;

    }

    function register(scriptName){
        if(isPending(scriptName)){
            return cache[scriptName].promise
        }
        if(isRegistered(scriptName)){
            return $q.resolve(scriptName);
        } else {
            var dfd = $q.defer();
            if(scripts.shim.hasOwnProperty(scriptName) && scripts.shim[scriptName].deps){
                var depsPromises = [];
                angular.forEach(scripts.shim[scriptName].deps, function(dep){

                    depsPromises.push(register(dep))
                    
                })
                $q.all(depsPromises).then(function(){
                    loadScript(scriptName).then(function(){
                        dfd.resolve(scriptName);
                    })
                })

            } else {
                
                loadScript(scriptName).then(function(){
                    dfd.resolve(scriptName);
                })
                 
            }
            return dfd.promise; 

        }
    }
    return {
        register: function (scripts) {
            
            var dfd = $q.defer();
            init().then(function(){
                var promises = [];
                if (angular.isString(scripts)) 
                    scripts = [scripts];    

                angular.forEach(scripts, function(script){
                    promises.push(register(script));
                })

                $q.all(promises).then(function(resolves){
                    dfd.resolve(resolves);
                })
            })
            return dfd.promise;

        }
    };
});
'use strict';

angular.module('SmartAdmin.Layout').factory('SmartCss', function ($rootScope, $timeout) {

    var sheet = (function () {
        // Create the <style> tag
        var style = document.createElement("style");

        // Add a media (and/or media query) here if you'd like!
        // style.setAttribute("media", "screen")
        // style.setAttribute("media", "@media only screen and (max-width : 1024px)")

        // WebKit hack :(
        style.appendChild(document.createTextNode(""));

        // Add the <style> element to the page
        document.head.appendChild(style);

        return style.sheet;
    })();

    var _styles = {};


    var SmartCss = {
        writeRule: function(selector){
            SmartCss.deleteRuleFor(selector);
            if(_.has(_styles, selector)){
                var css = selector + '{ ' + _.map(_styles[selector], function(v, k){
                    return  k + ':' +  v + ';'
                }).join(' ') +'}';
                sheet.insertRule(css, _.size(_styles) - 1);
            }
        },
        add: function (selector, property, value, delay) {
            if(!_.has(_styles, selector))
                _styles[selector] = {};

            if(value == undefined || value == null || value == '')
                delete _styles[selector][property];
            else
                _styles[selector][property] = value;


            if(_.keys(_styles[selector]).length == 0)
                delete _styles[selector];

            if(!delay)
                delay = 0;
            $timeout(function(){
                SmartCss.writeRule(selector);
            }, delay);

        },
        remove: function(selector, property, delay){
            SmartCss.add(selector, property, null, delay);
        },
        deleteRuleFor: function (selector) {
            _(sheet.rules).forEach(function (rule, idx) {
                if (rule.selectorText == selector) {
                    sheet.deleteRule(idx);
                }
            });
        },
        appViewSize: null
    };

    $rootScope.$on('$smartContentResize', function (event, data) {
        SmartCss.appViewSize = data;
    });

    return SmartCss;

});




"use strict";


angular.module('SmartAdmin.Forms').directive('bootstrapAttributeForm', function(){

    return {
        restrict: 'E',
        replace: true,
        templateUrl: 'app/_common/forms/directives/bootstrap-validation/bootstrap-attribute-form.tpl.html',
        link: function(scope, form){
            form.bootstrapValidator();


        }

    }
});
"use strict";


angular.module('SmartAdmin.Forms').directive('bootstrapButtonGroupForm', function(){

    return {
        restrict: 'E',
        replace: true,
        templateUrl: 'app/_common/forms/directives/bootstrap-validation/bootstrap-button-group-form.tpl.html',
        link: function(scope, form){
            form.bootstrapValidator({
                excluded : ':disabled',
                feedbackIcons : {
                    valid : 'glyphicon glyphicon-ok',
                    invalid : 'glyphicon glyphicon-remove',
                    validating : 'glyphicon glyphicon-refresh'
                },
                fields : {
                    gender : {
                        validators : {
                            notEmpty : {
                                message : 'The gender is required'
                            }
                        }
                    },
                    'languages[]' : {
                        validators : {
                            choice : {
                                min : 1,
                                max : 2,
                                message : 'Please choose 1 - 2 languages you can speak'
                            }
                        }
                    }
                }
            });


        }

    }
});
"use strict";


angular.module('SmartAdmin.Forms').directive('bootstrapContactForm', function(){

    return {
        restrict: 'E',
        replace: true,
        templateUrl: 'app/_common/forms/directives/bootstrap-validation/bootstrap-contact-form.tpl.html',
        link: function(scope, form){
            form.bootstrapValidator({
                container : '#messages',
                feedbackIcons : {
                    valid : 'glyphicon glyphicon-ok',
                    invalid : 'glyphicon glyphicon-remove',
                    validating : 'glyphicon glyphicon-refresh'
                },
                fields : {
                    fullName : {
                        validators : {
                            notEmpty : {
                                message : 'The full name is required and cannot be empty'
                            }
                        }
                    },
                    email : {
                        validators : {
                            notEmpty : {
                                message : 'The email address is required and cannot be empty'
                            },
                            emailAddress : {
                                message : 'The email address is not valid'
                            }
                        }
                    },
                    title : {
                        validators : {
                            notEmpty : {
                                message : 'The title is required and cannot be empty'
                            },
                            stringLength : {
                                max : 100,
                                message : 'The title must be less than 100 characters long'
                            }
                        }
                    },
                    content : {
                        validators : {
                            notEmpty : {
                                message : 'The content is required and cannot be empty'
                            },
                            stringLength : {
                                max : 500,
                                message : 'The content must be less than 500 characters long'
                            }
                        }
                    }
                }
            });

        }

    }

});
"use strict";


angular.module('SmartAdmin.Forms').directive('bootstrapMovieForm', function(){

    return {
        restrict: 'E',
        replace: true,
        templateUrl: 'app/_common/forms/directives/bootstrap-validation/bootstrap-movie-form.tpl.html',
        link: function(scope, form){
            form.bootstrapValidator({
                feedbackIcons : {
                    valid : 'glyphicon glyphicon-ok',
                    invalid : 'glyphicon glyphicon-remove',
                    validating : 'glyphicon glyphicon-refresh'
                },
                fields : {
                    title : {
                        group : '.col-md-8',
                        validators : {
                            notEmpty : {
                                message : 'The title is required'
                            },
                            stringLength : {
                                max : 200,
                                message : 'The title must be less than 200 characters long'
                            }
                        }
                    },
                    genre : {
                        group : '.col-md-4',
                        validators : {
                            notEmpty : {
                                message : 'The genre is required'
                            }
                        }
                    },
                    director : {
                        group : '.col-md-4',
                        validators : {
                            notEmpty : {
                                message : 'The director name is required'
                            },
                            stringLength : {
                                max : 80,
                                message : 'The director name must be less than 80 characters long'
                            }
                        }
                    },
                    writer : {
                        group : '.col-md-4',
                        validators : {
                            notEmpty : {
                                message : 'The writer name is required'
                            },
                            stringLength : {
                                max : 80,
                                message : 'The writer name must be less than 80 characters long'
                            }
                        }
                    },
                    producer : {
                        group : '.col-md-4',
                        validators : {
                            notEmpty : {
                                message : 'The producer name is required'
                            },
                            stringLength : {
                                max : 80,
                                message : 'The producer name must be less than 80 characters long'
                            }
                        }
                    },
                    website : {
                        group : '.col-md-6',
                        validators : {
                            notEmpty : {
                                message : 'The website address is required'
                            },
                            uri : {
                                message : 'The website address is not valid'
                            }
                        }
                    },
                    trailer : {
                        group : '.col-md-6',
                        validators : {
                            notEmpty : {
                                message : 'The trailer link is required'
                            },
                            uri : {
                                message : 'The trailer link is not valid'
                            }
                        }
                    },
                    review : {
                        // The group will be set as default (.form-group)
                        validators : {
                            stringLength : {
                                max : 500,
                                message : 'The review must be less than 500 characters long'
                            }
                        }
                    },
                    rating : {
                        // The group will be set as default (.form-group)
                        validators : {
                            notEmpty : {
                                message : 'The rating is required'
                            }
                        }
                    }
                }
            });

        }

    }

});
"use strict";


angular.module('SmartAdmin.Forms').directive('bootstrapProductForm', function(){

    return {
        restrict: 'E',
        replace: true,
        templateUrl: 'app/_common/forms/directives/bootstrap-validation/bootstrap-product-form.tpl.html',
        link: function(scope, form){
            form.bootstrapValidator({
                feedbackIcons : {
                    valid : 'glyphicon glyphicon-ok',
                    invalid : 'glyphicon glyphicon-remove',
                    validating : 'glyphicon glyphicon-refresh'
                },
                fields : {
                    price : {
                        validators : {
                            notEmpty : {
                                message : 'The price is required'
                            },
                            numeric : {
                                message : 'The price must be a number'
                            }
                        }
                    },
                    amount : {
                        validators : {
                            notEmpty : {
                                message : 'The amount is required'
                            },
                            numeric : {
                                message : 'The amount must be a number'
                            }
                        }
                    },
                    color : {
                        validators : {
                            notEmpty : {
                                message : 'The color is required'
                            }
                        }
                    },
                    size : {
                        validators : {
                            notEmpty : {
                                message : 'The size is required'
                            }
                        }
                    }
                }
            });
        }

    }
});
"use strict";


angular.module('SmartAdmin.Forms').directive('bootstrapProfileForm', function(){

    return {
        restrict: 'E',
        replace: true,
        templateUrl: 'app/_common/forms/directives/bootstrap-validation/bootstrap-profile-form.tpl.html',
        link: function(scope, form){
           form.bootstrapValidator({
                feedbackIcons : {
                    valid : 'glyphicon glyphicon-ok',
                    invalid : 'glyphicon glyphicon-remove',
                    validating : 'glyphicon glyphicon-refresh'
                },
                fields : {
                    email : {
                        validators : {
                            notEmpty : {
                                message : 'The email address is required'
                            },
                            emailAddress : {
                                message : 'The email address is not valid'
                            }
                        }
                    },
                    password : {
                        validators : {
                            notEmpty : {
                                message : 'The password is required'
                            }
                        }
                    }
                }
            });
        }

    }

});
"use strict";


angular.module('SmartAdmin.Forms').directive('bootstrapTogglingForm', function(){

    return {
        restrict: 'E',
        replace: true,
        templateUrl: 'app/_common/forms/directives/bootstrap-validation/bootstrap-toggling-form.tpl.html',
        link: function(scope, form){
            form.bootstrapValidator({
                feedbackIcons : {
                    valid : 'glyphicon glyphicon-ok',
                    invalid : 'glyphicon glyphicon-remove',
                    validating : 'glyphicon glyphicon-refresh'
                },
                fields : {
                    firstName : {
                        validators : {
                            notEmpty : {
                                message : 'The first name is required'
                            }
                        }
                    },
                    lastName : {
                        validators : {
                            notEmpty : {
                                message : 'The last name is required'
                            }
                        }
                    },
                    company : {
                        validators : {
                            notEmpty : {
                                message : 'The company name is required'
                            }
                        }
                    },
                    // These fields will be validated when being visible
                    job : {
                        validators : {
                            notEmpty : {
                                message : 'The job title is required'
                            }
                        }
                    },
                    department : {
                        validators : {
                            notEmpty : {
                                message : 'The department name is required'
                            }
                        }
                    },
                    mobilePhone : {
                        validators : {
                            notEmpty : {
                                message : 'The mobile phone number is required'
                            },
                            digits : {
                                message : 'The mobile phone number is not valid'
                            }
                        }
                    },
                    // These fields will be validated when being visible
                    homePhone : {
                        validators : {
                            digits : {
                                message : 'The home phone number is not valid'
                            }
                        }
                    },
                    officePhone : {
                        validators : {
                            digits : {
                                message : 'The office phone number is not valid'
                            }
                        }
                    }
                }
            }).find('button[data-toggle]').on('click', function() {
                var $target = $($(this).attr('data-toggle'));
                // Show or hide the additional fields
                // They will or will not be validated based on their visibilities
                $target.toggle();
                if (!$target.is(':visible')) {
                    // Enable the submit buttons in case additional fields are not valid
                    form.data('bootstrapValidator').disableSubmitButtons(false);
                }
            });
        }

    }



});
'use strict';

angular.module('SmartAdmin.Forms').directive('smartCheckoutForm', function (formsCommon, lazyScript) {
    return {
        restrict: 'A',
        link: function (scope, form) {
           lazyScript.register('jquery-validation').then(function(){

               scope.countries = formsCommon.countries;

               form.validate(angular.extend({
                    // Rules for form validation
                    rules : {
                        fname : {
                            required : true
                        },
                        lname : {
                            required : true
                        },
                        email : {
                            required : true,
                            email : true
                        },
                        phone : {
                            required : true
                        },
                        country : {
                            required : true
                        },
                        city : {
                            required : true
                        },
                        code : {
                            required : true,
                            digits : true
                        },
                        address : {
                            required : true
                        },
                        name : {
                            required : true
                        },
                        card : {
                            required : true,
                            creditcard : true
                        },
                        cvv : {
                            required : true,
                            digits : true
                        },
                        month : {
                            required : true
                        },
                        year : {
                            required : true,
                            digits : true
                        }
                    },

                    // Messages for form validation
                    messages : {
                        fname : {
                            required : 'Please enter your first name'
                        },
                        lname : {
                            required : 'Please enter your last name'
                        },
                        email : {
                            required : 'Please enter your email address',
                            email : 'Please enter a VALID email address'
                        },
                        phone : {
                            required : 'Please enter your phone number'
                        },
                        country : {
                            required : 'Please select your country'
                        },
                        city : {
                            required : 'Please enter your city'
                        },
                        code : {
                            required : 'Please enter code',
                            digits : 'Digits only please'
                        },
                        address : {
                            required : 'Please enter your full address'
                        },
                        name : {
                            required : 'Please enter name on your card'
                        },
                        card : {
                            required : 'Please enter your card number'
                        },
                        cvv : {
                            required : 'Enter CVV2',
                            digits : 'Digits only'
                        },
                        month : {
                            required : 'Select month'
                        },
                        year : {
                            required : 'Enter year',
                            digits : 'Digits only please'
                        }
                    }
                }, formsCommon.validateOptions));
            });
        }
    }
});
'use strict';

angular.module('SmartAdmin.Forms').directive('smartCommentForm', function (formsCommon, lazyScript) {
    return {
        restrict: 'A',
        link: function (scope, form) {
            lazyScript.register('jquery-validation').then(function(){
                form.validate(angular.extend({
                    // Rules for form validation
                    rules : {
                        name : {
                            required : true
                        },
                        email : {
                            required : true,
                            email : true
                        },
                        url : {
                            url : true
                        },
                        comment : {
                            required : true
                        }
                    },

                    // Messages for form validation
                    messages : {
                        name : {
                            required : 'Enter your name',
                        },
                        email : {
                            required : 'Enter your email address',
                            email : 'Enter a VALID email'
                        },
                        url : {
                            email : 'Enter a VALID url'
                        },
                        comment : {
                            required : 'Please enter your comment'
                        }
                    },

                    // Ajax form submition
                    submitHandler : function() {
                        form.ajaxSubmit({
                            success : function() {
                                form.addClass('submited');
                            }
                        });
                    }

                }, formsCommon.validateOptions));
            });

        }
    }
});

'use strict';

angular.module('SmartAdmin.Forms').directive('smartContactsForm', function (formsCommon, lazyScript) {
    return {
        restrict: 'A',
        link: function (scope, form) {
            lazyScript.register('jquery-validation').then(function(){
                form.validate(angular.extend({
                    // Rules for form validation
                    rules : {
                        name : {
                            required : true
                        },
                        email : {
                            required : true,
                            email : true
                        },
                        message : {
                            required : true,
                            minlength : 10
                        }
                    },

                    // Messages for form validation
                    messages : {
                        name : {
                            required : 'Please enter your name'
                        },
                        email : {
                            required : 'Please enter your email address',
                            email : 'Please enter a VALID email address'
                        },
                        message : {
                            required : 'Please enter your message'
                        }
                    },

                    // Ajax form submition
                    submitHandler : function() {
                        form.ajaxSubmit({
                            success : function() {
                                form.addClass('submited');
                            }
                        });
                    }
                }, formsCommon.validateOptions));
            });
        }
    }
});
'use strict';

angular.module('SmartAdmin.Forms').directive('smartOrderForm', function (formsCommon, lazyScript) {
    return {
        restrict: 'E',
        link: function (scope, form) {
            lazyScript.register('jquery-validation').then(function(){
                form.validate(angular.extend({
                    // Rules for form validation
                    rules : {
                        name : {
                            required : true
                        },
                        email : {
                            required : true,
                            email : true
                        },
                        phone : {
                            required : true
                        },
                        interested : {
                            required : true
                        },
                        budget : {
                            required : true
                        }
                    },

                    // Messages for form validation
                    messages : {
                        name : {
                            required : 'Please enter your name'
                        },
                        email : {
                            required : 'Please enter your email address',
                            email : 'Please enter a VALID email address'
                        },
                        phone : {
                            required : 'Please enter your phone number'
                        },
                        interested : {
                            required : 'Please select interested service'
                        },
                        budget : {
                            required : 'Please select your budget'
                        }
                    },

                }, formsCommon.validateOptions));
            });

        }
    }
});
'use strict';

angular.module('SmartAdmin.Forms').directive('smartRegistrationForm', function (formsCommon, lazyScript) {
    return {
        restrict: 'A',
        link: function (scope, form, attributes) {
            lazyScript.register('jquery-validation').then(function(){
                form.validate(angular.extend({

                    // Rules for form validation
                    rules: {
                        username: {
                            required: true
                        },
                        email: {
                            required: true,
                            email: true
                        },
                        password: {
                            required: true,
                            minlength: 3,
                            maxlength: 20
                        },
                        passwordConfirm: {
                            required: true,
                            minlength: 3,
                            maxlength: 20,
                            equalTo: '#password'
                        },
                        firstname: {
                            required: true
                        },
                        lastname: {
                            required: true
                        },
                        gender: {
                            required: true
                        },
                        terms: {
                            required: true
                        }
                    },

                    // Messages for form validation
                    messages: {
                        login: {
                            required: 'Please enter your login'
                        },
                        email: {
                            required: 'Please enter your email address',
                            email: 'Please enter a VALID email address'
                        },
                        password: {
                            required: 'Please enter your password'
                        },
                        passwordConfirm: {
                            required: 'Please enter your password one more time',
                            equalTo: 'Please enter the same password as above'
                        },
                        firstname: {
                            required: 'Please select your first name'
                        },
                        lastname: {
                            required: 'Please select your last name'
                        },
                        gender: {
                            required: 'Please select your gender'
                        },
                        terms: {
                            required: 'You must agree with Terms and Conditions'
                        }
                    }

                }, formsCommon.validateOptions));
            });
        }
    }
});
'use strict';

angular.module('SmartAdmin.Forms').directive('smartReviewForm', function (formsCommon, lazyScript) {
    return {
        restrict: 'E',
        link: function (scope, form) {
            lazyScript.register('jquery-validation').then(function(){

                form.validate(angular.extend({
                    // Rules for form validation
                    rules : {
                        name : {
                            required : true
                        },
                        email : {
                            required : true,
                            email : true
                        },
                        review : {
                            required : true,
                            minlength : 20
                        },
                        quality : {
                            required : true
                        },
                        reliability : {
                            required : true
                        },
                        overall : {
                            required : true
                        }
                    },

                    // Messages for form validation
                    messages : {
                        name : {
                            required : 'Please enter your name'
                        },
                        email : {
                            required : 'Please enter your email address',
                            email : '<i class="fa fa-warning"></i><strong>Please enter a VALID email addres</strong>'
                        },
                        review : {
                            required : 'Please enter your review'
                        },
                        quality : {
                            required : 'Please rate quality of the product'
                        },
                        reliability : {
                            required : 'Please rate reliability of the product'
                        },
                        overall : {
                            required : 'Please rate the product'
                        }
                    }

                }, formsCommon.validateOptions));
            });
        }
    }
});
'use strict';

angular.module('SmartAdmin.Forms').directive('smartJcrop', function ($q) {
    return {
        restrict: 'A',
        scope: {
            coords: '=',
            options: '=',
            selection: '='
        },
        link: function (scope, element, attributes) {
            var jcropApi, imageWidth, imageHeight, imageLoaded = $q.defer();

            var listeners = {
                onSelectHandlers: [],
                onChangeHandlers: [],
                onSelect: function (c) {
                    angular.forEach(listeners.onSelectHandlers, function (handler) {
                        handler.call(jcropApi, c)
                    })
                },
                onChange: function (c) {
                    angular.forEach(listeners.onChangeHandlers, function (handler) {
                        handler.call(jcropApi, c)
                    })
                }
            };

            if (attributes.coords) {
                var coordsUpdate = function (c) {
                    scope.$apply(function () {
                        scope.coords = c;
                    });
                };
                listeners.onSelectHandlers.push(coordsUpdate);
                listeners.onChangeHandlers.push(coordsUpdate);
            }

            var $previewPane = $(attributes.smartJcropPreview),
                $previewContainer = $previewPane.find('.preview-container'),
                $previewImg = $previewPane.find('img');

            if ($previewPane.length && $previewImg.length) {
                var previewUpdate = function (coords) {
                    if (parseInt(coords.w) > 0) {
                        var rx = $previewContainer.width() / coords.w;
                        var ry = $previewContainer.height() / coords.h;

                        $previewImg.css({
                            width: Math.round(rx * imageWidth) + 'px',
                            height: Math.round(ry * imageHeight) + 'px',
                            marginLeft: '-' + Math.round(rx * coords.x) + 'px',
                            marginTop: '-' + Math.round(ry * coords.y) + 'px'
                        });
                    }
                };
                listeners.onSelectHandlers.push(previewUpdate);
                listeners.onChangeHandlers.push(previewUpdate);
            }


            var options = {
                onSelect: listeners.onSelect,
                onChange: listeners.onChange
            };

            if ($previewContainer.length) {
                options.aspectRatio = $previewContainer.width() / $previewContainer.height()
            }

            if (attributes.selection) {
                scope.$watch('selection', function (newVal, oldVal) {
                    if (newVal != oldVal) {
                        var rectangle = newVal == 'release' ? [imageWidth / 2, imageHeight / 2, imageWidth / 2, imageHeight / 2] : newVal;

                        var callback = newVal == 'release' ? function () {
                            jcropApi.release();
                        } : angular.noop;

                        imageLoaded.promise.then(function () {
                            if (scope.options && scope.options.animate) {
                                jcropApi.animateTo(rectangle, callback);
                            } else {
                                jcropApi.setSelect(rectangle);
                            }
                        });
                    }
                });
            }

            if (attributes.options) {

                var optionNames = [
                    'bgOpacity', 'bgColor', 'bgFade', 'shade', 'outerImage',
                    'allowSelect', 'allowMove', 'allowResize',
                    'aspectRatio'
                ];

                angular.forEach(optionNames, function (name) {
                    if (scope.options[name])
                        options[name] = scope.options[name]

                    scope.$watch('options.' + name, function (newVal, oldVal) {
                        if (newVal != oldVal) {
                            imageLoaded.promise.then(function () {
                                var update = {};
                                update[name] = newVal;
                                jcropApi.setOptions(update);
                            });
                        }
                    });

                });


                scope.$watch('options.disabled', function (newVal, oldVal) {
                    if (newVal != oldVal) {
                        if (newVal) {
                            jcropApi.disable();
                        } else {
                            jcropApi.enable();
                        }
                    }
                });

                scope.$watch('options.destroyed', function (newVal, oldVal) {
                    if (newVal != oldVal) {
                        if (newVal) {
                            jcropApi.destroy();
                        } else {
                            _init();
                        }
                    }
                });

                scope.$watch('options.src', function (newVal, oldVal) {
                    imageLoaded = $q.defer();
                    if (newVal != oldVal) {
                        jcropApi.setImage(scope.options.src, function () {
                            imageLoaded.resolve();
                        });
                    }
                });

                var updateSize = function(){
                    jcropApi.setOptions({
                        minSize: [scope.options.minSizeWidth, scope.options.minSizeHeight],
                        maxSize: [scope.options.maxSizeWidth, scope.options.maxSizeHeight]
                    });
                };

                scope.$watch('options.minSizeWidth', function (newVal, oldVal) {
                    if (newVal != oldVal) updateSize();
                });
                scope.$watch('options.minSizeHeight', function (newVal, oldVal) {
                    if (newVal != oldVal) updateSize();
                });
                scope.$watch('options.maxSizeWidth', function (newVal, oldVal) {
                    if (newVal != oldVal) updateSize();
                });
                scope.$watch('options.maxSizeHeight', function (newVal, oldVal) {
                    if (newVal != oldVal) updateSize();
                });
            }

            var _init = function () {
                element.Jcrop(options, function () {
                    jcropApi = this;
                    // Use the API to get the real image size
                    var bounds = this.getBounds();
                    imageWidth = bounds[0];
                    imageHeight = bounds[1];

                    if (attributes.selection && angular.isArray(scope.selection)) {
                        if (scope.options && scope.options.animate) {
                            jcropApi.animateTo(scope.selection);
                        } else {
                            jcropApi.setSelect(scope.selection);
                        }
                    }
                    imageLoaded.resolve();
                });
            };

            _init()


        }
    }
});
'use strict';

angular.module('SmartAdmin.Forms').directive('smartClockpicker', function () {
    return {
        restrict: 'A',
        compile: function (tElement, tAttributes) {
            tElement.removeAttr('smart-clockpicker data-smart-clockpicker');

            var options = {
                placement: 'top',
                donetext: 'Done'
            }

            tElement.clockpicker(options);
        }
    }
});

'use strict';

angular.module('SmartAdmin.Forms').directive('smartColorpicker', function () {
    return {
        restrict: 'A',
        compile: function (tElement, tAttributes) {
            tElement.removeAttr('smart-colorpicker data-smart-colorpicker');


            var aOptions = _.pick(tAttributes, ['']);

            var options = _.extend(aOptions, {});

            tElement.colorpicker(options);
        }
    }
});
"use strict";

angular.module('SmartAdmin.Forms').directive('smartDatepicker', function () {
    return {
        restrict: 'A',
        compile: function (element, attributes) {
            element.removeAttr('smartDatepicker');

            var onSelectCallbacks = [];
            if (attributes.minRestrict) {
                onSelectCallbacks.push(function (selectedDate) {
                    $(attributes.minRestrict).datepicker('option', 'minDate', selectedDate);
                });
            }
            if (attributes.maxRestrict) {
                onSelectCallbacks.push(function (selectedDate) {
                    $(attributes.maxRestrict).datepicker('option', 'maxDate', selectedDate);
                });
            }

            //Let others know about changes to the data field
            onSelectCallbacks.push(function (selectedDate) {
                //CVB - 07/14/2015 - Update the scope with the selected value
                element.triggerHandler("change");

                //CVB - 07/17/2015 - Update Bootstrap Validator
                var form = element.closest('form');

                if(typeof form.bootstrapValidator == 'function')
                    form.bootstrapValidator('revalidateField', element.attr('name'));
            });

            var options = {
                prevText: '<i class="fa fa-chevron-left"></i>',
                nextText: '<i class="fa fa-chevron-right"></i>',
                onSelect: function (selectedDate) {
                    angular.forEach(onSelectCallbacks, function (callback) {
                        callback.call(this, selectedDate)
                    })
                }
            };


            if (attributes.numberOfMonths) options.numberOfMonths = parseInt(attributes.numberOfMonths);

            if (attributes.dateFormat) options.dateFormat = attributes.dateFormat;

            if (attributes.defaultDate) options.defaultDate = attributes.defaultDate;

            if (attributes.changeMonth) options.changeMonth = attributes.changeMonth == "true";


            element.datepicker(options)
        }
    }
});
'use strict';

angular
  .module('SmartAdmin.Forms', [])

  .provider('datetimepicker', function () {
      var default_options = {};

      this.setOptions = function (options) {
          default_options = options;
      };

      this.$get = function () {
          return {
              getOptions: function () {
                  return default_options;
              }
          };
      };
  })

  .directive('datetimepicker', [
    '$timeout',
    'datetimepicker',
    function ($timeout,
              datetimepicker) {
        var default_options = datetimepicker.getOptions();

        return {
            require: '?ngModel',
            restrict: 'AE',
            scope: {
                datetimepickerOptions: '@'
            },
            link: function ($scope, $element, $attrs, controller) {
                var passed_in_options = $scope.$eval($attrs.datetimepickerOptions);
                var options = jQuery.extend({}, default_options, passed_in_options);

                $element.on('dp.change', function (ev) {
                    $timeout(function () {
                        var dtp = $element.data("DateTimePicker");
                        controller.$setViewValue(ev.target.value);
                    });
                });

                function setPickerValue() {
                    var result = null;
                    if (!!controller && !!controller.$viewValue) {
                        result = controller.$viewValue;
                    }
                    var dtp = $element.data("DateTimePicker");
                    dtp.date(result);
                }

                controller.$render = function (value) {
                    setPickerValue();
                };

                $element.datetimepicker(options);

                setPickerValue();
            }
        };
    }
  ]);
'use strict';

angular.module('SmartAdmin.Forms').directive('smartDuallistbox', function () {
    return {
        restrict: 'A',
        compile: function (tElement, tAttributes) {
            tElement.removeAttr('smart-duallistbox data-smart-duallistbox');


            var aOptions = _.pick(tAttributes, ['nonSelectedFilter']);

            var options = _.extend(aOptions, {
                nonSelectedListLabel: 'Non-selected',
                selectedListLabel: 'Selected',
                preserveSelectionOnMove: 'moved',
                moveOnSelect: false
            });

            tElement.bootstrapDualListbox(options);
        }
    }
});

'use strict';

angular.module('SmartAdmin.Forms').directive('smartIonslider', function (lazyScript) {
    return {
        restrict: 'A',
        compile: function (element, attributes) {
            element.removeAttr('smart-ionslider data-smart-ionslider');

        	lazyScript.register('ionslider').then(function(){
            	element.ionRangeSlider();
        	});
        }
    }
});
'use strict';

angular.module('SmartAdmin.Forms').directive('smartKnob', function () {
    return {
        restrict: 'A',
        compile: function (tElement, tAttributes) {
            tElement.removeAttr('smart-knob data-smart-knob');

            tElement.knob();
        }
    }
});
"use strict";

angular.module('SmartAdmin.Forms').directive('smartMaskedInput', function(lazyScript){
    return {
        restrict: 'A',
        compile: function(tElement, tAttributes){
            tElement.removeAttr('smart-masked-input data-smart-masked-input');

        	lazyScript.register('jquery-maskedinput').then(function(){

	            var options = {};
	            if(tAttributes.maskPlaceholder) options.placeholder =  tAttributes.maskPlaceholder;
	            tElement.mask(tAttributes.smartMaskedInput, options);
        	})	            
        }
    }
});
'use strict';

angular.module('SmartAdmin.Forms').directive('smartNouislider', function ($parse, lazyScript) {
    return {
        restrict: 'A',
        compile: function (tElement, tAttributes) {
            lazyScript.register('nouislider').then(function(){
                tElement.removeAttr('smart-nouislider data-smart-nouislider');

                tElement.addClass('noUiSlider');

                var options = {
                    range: {
                        min: tAttributes.rangeMin ? parseInt(tAttributes.rangeMin) : 0,
                        max: tAttributes.rangeMax ? parseInt(tAttributes.rangeMax) : 1000
                    },
                    start: $parse(tAttributes.start)()
                };

                if (tAttributes.step) options.step =  parseInt(tAttributes.step);

                if(tAttributes.connect) options.connect = tAttributes.connect == 'true' ? true : tAttributes.connect;

                tElement.noUiSlider(options);

                if(tAttributes.update) tElement.on('slide', function(){
                    $(tAttributes.update).text(JSON.stringify(tElement.val()));
                });                
            })
        }
    }
});
'use strict'

angular.module('SmartAdmin.Forms').directive('smartSelect2', function (lazyScript) {
    return {
        restrict: 'A',
        compile: function (element, attributes) {
            element.hide().removeAttr('smart-select2 data-smart-select2');
        	lazyScript.register('select2').then(function(){
	            element.show().select2();
        	})
        }
    }
});
'use strict'

angular.module('SmartAdmin.Forms').directive('smartSpinner', function () {
    return {
        restrict: 'A',
        compile: function (tElement, tAttributes) {
            tElement.removeAttr('smart-spinner');

            var options = {};
            if(tAttributes.smartSpinner == 'deicimal'){
                options = {
                    step: 0.01,
                    numberFormat: "n"
                };
            }else if(tAttributes.smartSpinner == 'currency'){
                options = {
                    min: 5,
                    max: 2500,
                    step: 25,
                    start: 1000,
                    numberFormat: "C"
                };
            }

            tElement.spinner(options);
        }
    }
});
'use strict';

angular.module('SmartAdmin.Forms').directive('smartTagsinput', function () {
    return {
        restrict: 'A',
        compile: function (tElement, tAttributes) {
            tElement.removeAttr('smart-tagsinput data-smart-tagsinput');
            tElement.tagsinput();
        }
    }
});
'use strict';

angular.module('SmartAdmin.Forms').directive('smartTimepicker', function () {
    return {
        restrict: 'A',
        compile: function (tElement, tAttributes) {
            tElement.removeAttr('smart-timepicker data-smart-timepicker');
            tElement.timepicker();
        }
    }
});

'use strict';

angular.module('SmartAdmin.Forms').directive('smartUislider', function ($parse, lazyScript) {
    return {
        restrict: 'A',
        compile: function (tElement, tAttributes) {

            tElement.removeAttr('smart-uislider data-smart-uislider');

            lazyScript.register('bootstrap-slider').then(function(){
			    tElement.bootstrapSlider();

			    $(tElement.data('bootstrapSlider').sliderElem).prepend(tElement);      	
            })

        }
    }
});
"use strict";

angular.module('SmartAdmin.Forms').directive('smartXeditable', function($timeout, $log){

	function link (scope, element, attrs, ngModel) {

        var defaults = {
            // display: function(value, srcData) {
            //     ngModel.$setViewValue(value);
            //     // scope.$apply();
            // }
        };

        var inited = false;

        var initXeditable = function() {

            var options = scope.options || {};
    		var initOptions = angular.extend(defaults, options);

            // $log.log(initOptions);
            element.editable('destroy');
            element.editable(initOptions);
        }

        scope.$watch("options", function(newValue) {

            if(!newValue) {
                return false;
            }

            initXeditable();

            // $log.log("Options changed...");

        }, true);

    }

    return {
    	restrict: 'A',
    	require: "ngModel",
        scope: {
            options: "="
        },
    	link: link 

    }
});
'use strict';

angular.module('SmartAdmin.Forms').directive('smartCkEditor', function () {
    return {
        restrict: 'A',
        compile: function ( tElement) {
            tElement.removeAttr('smart-ck-editor data-smart-ck-editor');

            CKEDITOR.replace( tElement.attr('name'), { height: '380px', startupFocus : true} );
        }
    }
});
'use strict';

angular.module('SmartAdmin.Forms').directive('smartDestroySummernote', function () {
    return {
        restrict: 'A',
        compile: function (tElement, tAttributes) {
            tElement.removeAttr('smart-destroy-summernote data-smart-destroy-summernote')
            tElement.on('click', function() {
                angular.element(tAttributes.smartDestroySummernote).destroy();
            })
        }
    }
});

'use strict';

angular.module('SmartAdmin.Forms').directive('smartEditSummernote', function () {
    return {
        restrict: 'A',
        compile: function (tElement, tAttributes) {
            tElement.removeAttr('smart-edit-summernote data-smart-edit-summernote');
            tElement.on('click', function(){
                angular.element(tAttributes.smartEditSummernote).summernote({
                    focus : true
                });  
            });
        }
    }
});

'use strict';

angular.module('SmartAdmin.Forms').directive('smartMarkdownEditor', function () {
    return {
        restrict: 'A',
        compile: function (element, attributes) {
            element.removeAttr('smart-markdown-editor data-smart-markdown-editor')

            var options = {
                autofocus:false,
                savable:true,
                fullscreen: {
                    enable: false
                }
            };

            if(attributes.height){
                options.height = parseInt(attributes.height);
            }

            element.markdown(options);
        }
    }
});

'use strict';

angular.module('SmartAdmin.Forms').directive('smartSummernoteEditor', function (lazyScript) {
    return {
        restrict: 'A',
        compile: function (tElement, tAttributes) {
            tElement.removeAttr('smart-summernote-editor data-smart-summernote-editor');

            var options = {
                focus : true,
                tabsize : 2
            };

            if(tAttributes.height){
                options.height = tAttributes.height;
            }

            lazyScript.register('summernote').then(function(){
                tElement.summernote(options);
            });
        }
    }
});
'use strict';

angular.module('SmartAdmin.Forms').directive('smartDropzone', function () {
    return {
        restrict: 'A',
        compile: function (tElement, tAttributes) {
            tElement.removeAttr('file-dropzone data-file-dropzone');

            tElement.dropzone({
                addRemoveLinks : true,
                maxFilesize: 0.5,
                dictDefaultMessage: '<span class="text-center"><span class="font-lg visible-xs-block visible-sm-block visible-lg-block"><span class="font-lg"><i class="fa fa-caret-right text-danger"></i> Drop files <span class="font-xs">to upload</span></span><span>&nbsp&nbsp<h4 class="display-inline"> (Or Click)</h4></span>',
                dictResponseError: 'Error uploading file!'
            });
        }
    }
});

'use strict';

angular.module('SmartAdmin.Forms').directive('smartFueluxWizard', function () {
    return {
        restrict: 'A',
        scope: {
            smartWizardCallback: '&'
        },
        link: function (scope, element, attributes) {

            var wizard = element.wizard();

            var $form = element.find('form');

            wizard.on('actionclicked.fu.wizard', function(e, data){
                if ($form.data('validator')) {
                    if (!$form.valid()) {
                        $form.data('validator').focusInvalid();
                        e.preventDefault();
                    }
                }
            });

            wizard.on('finished.fu.wizard', function (e, data) {
                var formData = {};
                _.each($form.serializeArray(), function(field){
                    formData[field.name] = field.value
                });
                if(typeof scope.smartWizardCallback() === 'function'){
                    scope.smartWizardCallback()(formData)
                }
            });
        }
    }
});
'use strict';

angular.module('SmartAdmin.Forms').directive('smartWizard', function () {
    return {
        restrict: 'A',
        scope: {
            'smartWizardCallback': '&'
        },
        link: function (scope, element, attributes) {

            var stepsCount = $('[data-smart-wizard-tab]').length;

            var currentStep = 1;

            var validSteps = [];

            var $form = element.closest('form');

            var $prev = $('[data-smart-wizard-prev]', element);

            var $next = $('[data-smart-wizard-next]', element);

            function setStep(step) {
                currentStep = step;
                $('[data-smart-wizard-pane=' + step + ']', element).addClass('active').siblings('[data-smart-wizard-pane]').removeClass('active');
                $('[data-smart-wizard-tab=' + step + ']', element).addClass('active').siblings('[data-smart-wizard-tab]').removeClass('active');

                $prev.toggleClass('disabled', step == 1)
            }


            element.on('click', '[data-smart-wizard-tab]', function (e) {
                setStep(parseInt($(this).data('smartWizardTab')));
                e.preventDefault();
            });

            $next.on('click', function (e) {
                if ($form.data('validator')) {
                    if (!$form.valid()) {
                        validSteps = _.without(validSteps, currentStep);
                        $form.data('validator').focusInvalid();
                        return false;
                    } else {
                        validSteps = _.without(validSteps, currentStep);
                        validSteps.push(currentStep);
                        element.find('[data-smart-wizard-tab=' + currentStep + ']')
                            .addClass('complete')
                            .find('.step')
                            .html('<i class="fa fa-check"></i>');
                    }
                }
                if (currentStep < stepsCount) {
                    setStep(currentStep + 1);
                } else {
                    if (validSteps.length < stepsCount) {
                        var steps = _.range(1, stepsCount + 1)

                        _(steps).forEach(function (num) {
                            if (validSteps.indexOf(num) == -1) {
                                console.log(num);
                                setStep(num);
                                return false;
                            }
                        })
                    } else {
                        var data = {};
                        _.each($form.serializeArray(), function(field){
                            data[field.name] = field.value
                        });
                        if(typeof  scope.smartWizardCallback() === 'function'){
                            scope.smartWizardCallback()(data)
                        }
                    }
                }

                e.preventDefault();
            });

            $prev.on('click', function (e) {
                if (!$prev.hasClass('disabled') && currentStep > 0) {
                    setStep(currentStep - 1);
                }
                e.preventDefault();
            });


            setStep(currentStep);

        }
    }
});
'use strict';

angular.module('SmartAdmin.Forms').directive('smartValidateForm', function (formsCommon) {
    return {
        restrict: 'A',
        link: function (scope, form, attributes) {

            var validateOptions = {
                rules: {},
                messages: {},
                highlight: function (element) {
                    $(element).closest('.form-group').removeClass('has-success').addClass('has-error');
                },
                unhighlight: function (element) {
                    $(element).closest('.form-group').removeClass('has-error').addClass('has-success');
                },
                errorElement: 'span',
                errorClass: 'help-block',
                errorPlacement: function (error, element) {
                    if (element.parent('.input-group').length) {
                        error.insertAfter(element.parent());
                    } else {
                        error.insertAfter(element);
                    }
                }
            };
            form.find('[data-smart-validate-input], [smart-validate-input]').each(function () {
                var $input = $(this), fieldName = $input.attr('name');

                validateOptions.rules[fieldName] = {};

                if ($input.data('required') != undefined) {
                    validateOptions.rules[fieldName].required = true;
                }
                if ($input.data('email') != undefined) {
                    validateOptions.rules[fieldName].email = true;
                }

                if ($input.data('maxlength') != undefined) {
                    validateOptions.rules[fieldName].maxlength = $input.data('maxlength');
                }

                if ($input.data('minlength') != undefined) {
                    validateOptions.rules[fieldName].minlength = $input.data('minlength');
                }

                if($input.data('message')){
                    validateOptions.messages[fieldName] = $input.data('message');
                } else {
                    angular.forEach($input.data(), function(value, key){
                        if(key.search(/message/)== 0){
                            if(!validateOptions.messages[fieldName])
                                validateOptions.messages[fieldName] = {};

                            var messageKey = key.toLowerCase().replace(/^message/,'')
                            validateOptions.messages[fieldName][messageKey] = value;
                        }
                    });
                }
            });


            form.validate(validateOptions);

        }
    }
});

'use strict';

angular.module('SmartAdmin.Layout').directive('demoStates', function ($rootScope) {
    return {
        restrict: 'EA',
        replace: true,
        templateUrl: 'app/_common/layout/directives/demo/demo-states.tpl.html',
        scope: true,
        link: function (scope, element, attributes) {
            element.parent().css({
                position: 'relative'
            });

            element.on('click', '#demo-setting', function () {
                element.toggleClass('activate')
            })
        },
        controller: function ($scope) {
            var $root = $('body');

            $scope.$watch('fixedHeader', function (fixedHeader) {
                localStorage.setItem('sm-fixed-header', fixedHeader);
                $root.toggleClass('fixed-header', fixedHeader);
                if (fixedHeader == false) {
                    $scope.fixedRibbon = false;
                    $scope.fixedNavigation = false;
                }
            });


            $scope.$watch('fixedNavigation', function (fixedNavigation) {
                localStorage.setItem('sm-fixed-navigation', fixedNavigation);
                $root.toggleClass('fixed-navigation', fixedNavigation);
                if (fixedNavigation) {
                    $scope.insideContainer = false;
                    $scope.fixedHeader = true;
                } else {
                    $scope.fixedRibbon = false;
                }
            });


            $scope.$watch('fixedRibbon', function (fixedRibbon) {
                localStorage.setItem('sm-fixed-ribbon', fixedRibbon);
                $root.toggleClass('fixed-ribbon', fixedRibbon);
                if (fixedRibbon) {
                    $scope.fixedHeader = true;
                    $scope.fixedNavigation = true;
                    $scope.insideContainer = false;
                }
            });

            $scope.$watch('fixedPageFooter', function (fixedPageFooter) {
                localStorage.setItem('sm-fixed-page-footer', fixedPageFooter);
                $root.toggleClass('fixed-page-footer', fixedPageFooter);
            });

            $scope.$watch('insideContainer', function (insideContainer) {
                localStorage.setItem('sm-inside-container', insideContainer);
                $root.toggleClass('container', insideContainer);
                if (insideContainer) {
                    $scope.fixedRibbon = false;
                    $scope.fixedNavigation = false;
                }
            });

            $scope.$watch('rtl', function (rtl) {
                localStorage.setItem('sm-rtl', rtl);
                $root.toggleClass('smart-rtl', rtl);
            });

            $scope.$watch('menuOnTop', function (menuOnTop) {
                $rootScope.$broadcast('$smartLayoutMenuOnTop', menuOnTop);
                localStorage.setItem('sm-menu-on-top', menuOnTop);
                $root.toggleClass('menu-on-top', menuOnTop);

                if(menuOnTop)$root.removeClass('minified');
            });

            $scope.$watch('colorblindFriendly', function (colorblindFriendly) {
                localStorage.setItem('sm-colorblind-friendly', colorblindFriendly);
                $root.toggleClass('colorblind-friendly', colorblindFriendly);
            });


            $scope.fixedHeader = localStorage.getItem('sm-fixed-header') == 'true';
            $scope.fixedNavigation = localStorage.getItem('sm-fixed-navigation') == 'true';
            $scope.fixedRibbon = localStorage.getItem('sm-fixed-ribbon') == 'true';
            $scope.fixedPageFooter = localStorage.getItem('sm-fixed-page-footer') == 'true';
            $scope.insideContainer = localStorage.getItem('sm-inside-container') == 'true';
            $scope.rtl = localStorage.getItem('sm-rtl') == 'true';
            $scope.menuOnTop = localStorage.getItem('sm-menu-on-top') == 'true' || $root.hasClass('menu-on-top');
            $scope.colorblindFriendly = localStorage.getItem('sm-colorblind-friendly') == 'true';


            $scope.skins = appConfig.skins;

            $scope.smartSkin = localStorage.getItem('sm-skin') || appConfig.smartSkin;


            $scope.setSkin = function (skin) {
                $scope.smartSkin = skin.name;
                $root.removeClass(_.pluck($scope.skins, 'name').join(' '));
                $root.addClass(skin.name);
                localStorage.setItem('sm-skin', skin.name);
                $("#logo img").attr('src', skin.logo);
            };


            if($scope.smartSkin != "smart-style-0"){
                $scope.setSkin(_.find($scope.skins, {name: $scope.smartSkin}))
            }


            $scope.factoryReset = function () {
                $.SmartMessageBox({
                    title: "<i class='fa fa-refresh' style='color:green'></i> Clear Local Storage",
                    content: "Would you like to RESET all your saved widgets and clear LocalStorage?1",
                    buttons: '[No][Yes]'
                }, function (ButtonPressed) {
                    if (ButtonPressed == "Yes" && localStorage) {
                        localStorage.clear();
                        location.reload()
                    }
                });
            }
        }
    }
});
"use strict";

(function ($) {

    $.fn.smartCollapseToggle = function () {

        return this.each(function () {

            var $body = $('body');
            var $this = $(this);

            // only if not  'menu-on-top'
            if ($body.hasClass('menu-on-top')) {


            } else {

                $body.hasClass('mobile-view-activated')

                // toggle open
                $this.toggleClass('open');

                // for minified menu collapse only second level
                if ($body.hasClass('minified')) {
                    if ($this.closest('nav ul ul').length) {
                        $this.find('>a .collapse-sign .fa').toggleClass('fa-minus-square-o fa-plus-square-o');
                        $this.find('ul:first').slideToggle(appConfig.menu_speed || 200);
                    }
                } else {
                    // toggle expand item
                    $this.find('>a .collapse-sign .fa').toggleClass('fa-minus-square-o fa-plus-square-o');
                    $this.find('ul:first').slideToggle(appConfig.menu_speed || 200);
                }
            }
        });
    };
})(jQuery);

angular.module('SmartAdmin.Layout').directive('smartMenu', function ($state, $rootScope) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var $body = $('body');

            var $collapsible = element.find('li[data-menu-collapse]');

            var bindEvents = function(){
                $collapsible.each(function (idx, li) {
                    var $li = $(li);
                    $li
                        .on('click', '>a', function (e) {

                            // collapse all open siblings
                            $li.siblings('.open').smartCollapseToggle();

                            // toggle element
                            $li.smartCollapseToggle();

                            // add active marker to collapsed element if it has active childs
                            if (!$li.hasClass('open') && $li.find('li.active').length > 0) {
                                $li.addClass('active')
                            }

                            e.preventDefault();
                        })
                        .find('>a').append('<b class="collapse-sign"><em class="fa fa-plus-square-o"></em></b>');

                    // initialization toggle
                    if ($li.find('li.active').length) {
                        $li.smartCollapseToggle();
                        $li.find('li.active').parents('li').addClass('active');
                    }
                });
            }
            bindEvents();


            // click on route link
            element.on('click', 'a[data-ui-sref]', function (e) {
                // collapse all siblings to element parents and remove active markers
                $(this)
                    .parents('li').addClass('active')
                    .each(function () {
                        $(this).siblings('li.open').smartCollapseToggle();
                        $(this).siblings('li').removeClass('active')
                    });

                if ($body.hasClass('mobile-view-activated')) {
                    $rootScope.$broadcast('requestToggleMenu');
                }
            });


            scope.$on('$smartLayoutMenuOnTop', function (event, menuOnTop) {
                if (menuOnTop) {
                    $collapsible.filter('.open').smartCollapseToggle();
                }
            });
        }
    }
});
(function(){
    "use strict";

    angular.module('SmartAdmin.Layout').directive('smartMenuItems', function ($http, $rootScope, $compile, APP_CONFIG) {
    return {
        restrict: 'A',
        compile: function (element, attrs) {
            

            function createItem(item, parent, level){
                var li = $('<li />' ,{'ui-sref-active': "active"})
                var a = $('<a />');
                var i = $('<i />');

                li.append(a);

                if (item.sref) {
                    a.attr('ui-sref', item.sref);
                    // add this option to reload state when clicked

                    a.attr("ui-sref-opts", "{reload: true}");
                }
                if(item.href)
                    a.attr('href', item.href);
                if(item.icon){
                    i.attr('class', item.icon);
                    a.append(i);
                }
                if(item.title){
                    a.attr('title', item.title);
                    if(level == 1){ 
                        a.append(' <span class="menu-item-parent">' + item.title + '</span>');
                    } else {
                        a.append(' ' + item.title);

                    }

                    // add a badge to the app.tasks.list item
                    if (item.sref) {
                        var myTaskSref = "app.myspace";
                        if (item.sref.substring(0, myTaskSref.length) === myTaskSref) {
                            a.append('<span class="badge pull-right inbox-badge">{{getTotalCount()}}</span>');
                        }
                    }
                }

                if(item.items){
                    var ul = $('<ul />');
                    li.append(ul);
                    li.attr('data-menu-collapse', '');
                    _.forEach(item.items, function(child) {
                        createItem(child, ul, level+1);
                    })
                } 

                parent.append(li); 
            }

            $http.get(APP_CONFIG.ebaasRootUrl + attrs.smartMenuItems).then(function (res) {

                var ul = $('<ul />', {
                    'smart-menu': ''
                })
                _.forEach(res.data.items, function (item) {
                    if (item.visible) {
                        createItem(item, ul, 1);
                    }
                })
                
                var $scope = $rootScope.$new();
                var html = $('<div>').append(ul).html(); 
                var linkingFunction = $compile(html);
                
                var _element = linkingFunction($scope);

                element.replaceWith(_element);                
            })
        }
    }
});
})();
/**
 * Jarvis Widget Directive
 *
 *    colorbutton="false"
 *    editbutton="false"
      togglebutton="false"
       deletebutton="false"
        fullscreenbutton="false"
        custombutton="false"
        collapsed="true"
          sortable="false"
 *
 *
 */
"use strict";

angular.module('SmartAdmin.Layout').directive('jarvisWidget', function($rootScope){
    return {
        restrict: "A",
        compile: function(element, attributes){
            if(element.data('widget-color'))
                element.addClass('jarviswidget-color-' + element.data('widget-color'));


            element.find('.widget-body').prepend('<div class="jarviswidget-editbox"><input class="form-control" type="text"></div>');

            element.addClass('jarviswidget jarviswidget-sortable');
            $rootScope.$emit('jarvisWidgetAdded', element )

        }
    }
});
 "use strict";
 
 angular.module('SmartAdmin.Layout').directive('widgetGrid', function ($rootScope, $compile, $q, $state, $timeout) {

    var jarvisWidgetsDefaults = {
        grid: 'article',
        widgets: '.jarviswidget',
        localStorage: true,
        deleteSettingsKey: '#deletesettingskey-options',
        settingsKeyLabel: 'Reset settings?',
        deletePositionKey: '#deletepositionkey-options',
        positionKeyLabel: 'Reset position?',
        sortable: true,
        buttonsHidden: false,
        // toggle button
        toggleButton: true,
        toggleClass: 'fa fa-minus | fa fa-plus',
        toggleSpeed: 200,
        onToggle: function () {
        },
        // delete btn
        deleteButton: true,
        deleteMsg: 'Warning: This action cannot be undone!',
        deleteClass: 'fa fa-times',
        deleteSpeed: 200,
        onDelete: function () {
        },
        // edit btn
        editButton: true,
        editPlaceholder: '.jarviswidget-editbox',
        editClass: 'fa fa-cog | fa fa-save',
        editSpeed: 200,
        onEdit: function () {
        },
        // color button
        colorButton: true,
        // full screen
        fullscreenButton: true,
        fullscreenClass: 'fa fa-expand | fa fa-compress',
        fullscreenDiff: 3,
        onFullscreen: function () {
        },
        // custom btn
        customButton: false,
        customClass: 'folder-10 | next-10',
        customStart: function () {
            alert('Hello you, this is a custom button...');
        },
        customEnd: function () {
            alert('bye, till next time...');
        },
        // order
        buttonOrder: '%refresh% %custom% %edit% %toggle% %fullscreen% %delete%',
        opacity: 1.0,
        dragHandle: '> header',
        placeholderClass: 'jarviswidget-placeholder',
        indicator: true,
        indicatorTime: 600,
        ajax: true,
        timestampPlaceholder: '.jarviswidget-timestamp',
        timestampFormat: 'Last update: %m%/%d%/%y% %h%:%i%:%s%',
        refreshButton: true,
        refreshButtonClass: 'fa fa-refresh',
        labelError: 'Sorry but there was a error:',
        labelUpdated: 'Last Update:',
        labelRefresh: 'Refresh',
        labelDelete: 'Delete widget:',
        afterLoad: function () {
        },
        rtl: false, // best not to toggle this!
        onChange: function () {

        },
        onSave: function () {

        },
        ajaxnav: true

    }

    var dispatchedWidgetIds = [];
    var setupWaiting = false;

    var debug = 1;

    var setupWidgets = function (element, widgetIds) {

        if (!setupWaiting) {

            if(_.intersection(widgetIds, dispatchedWidgetIds).length != widgetIds.length){

                dispatchedWidgetIds = _.union(widgetIds, dispatchedWidgetIds);

//                    console.log('setupWidgets', debug++);

                element.data('jarvisWidgets') && element.data('jarvisWidgets').destroy();
                element.jarvisWidgets(jarvisWidgetsDefaults);
                initDropdowns(widgetIds);
            }

        } else {
            if (!setupWaiting) {
                setupWaiting = true;
                $timeout(function () {
                    setupWaiting = false;
                    setupWidgets(element, widgetIds)
                }, 200);
            }
        }

    };

    var destroyWidgets = function(element, widgetIds){
        element.data('jarvisWidgets') && element.data('jarvisWidgets').destroy();
        dispatchedWidgetIds = _.xor(dispatchedWidgetIds, widgetIds);
    };

    var initDropdowns = function (widgetIds) {
        angular.forEach(widgetIds, function (wid) {
            $('#' + wid + ' [data-toggle="dropdown"]').each(function () {
                var $parent = $(this).parent();
                // $(this).removeAttr('data-toggle');
                if (!$parent.attr('dropdown')) {
                    $(this).removeAttr('href');
                    $parent.attr('dropdown', '');
                    var compiled = $compile($parent)($parent.scope())
                    $parent.replaceWith(compiled);
                }
            })
        });
    };

    var jarvisWidgetAddedOff,
        $viewContentLoadedOff,
        $stateChangeStartOff;

    return {
        restrict: 'A',
        compile: function(element){

            element.removeAttr('widget-grid data-widget-grid');

            var widgetIds = [];

            $viewContentLoadedOff = $rootScope.$on('$viewContentLoaded', function (event, data) {
                $timeout(function () {
                    setupWidgets(element, widgetIds)
                }, 100);
            });


            $stateChangeStartOff = $rootScope.$on('$stateChangeStart',
                function(event, toState, toParams, fromState, fromParams){
                    jarvisWidgetAddedOff();
                    $viewContentLoadedOff();
                    $stateChangeStartOff();
                    destroyWidgets(element, widgetIds)
                });

            jarvisWidgetAddedOff = $rootScope.$on('jarvisWidgetAdded', function (event, widget) {
                if (widgetIds.indexOf(widget.attr('id')) == -1) {
                    widgetIds.push(widget.attr('id'));
                    $timeout(function () {
                        setupWidgets(element, widgetIds)
                    }, 100);
                }
//                    console.log('jarvisWidgetAdded', widget.attr('id'));
            });

        }
    }
});
