var yumKaaxApp = angular.module('yumKaaxApp', [
    'ngRoute',
    'PublicControllers',
    'authFactory',
    'chieffancypants.loadingBar', 'ngAnimate'
    , 'ui.bootstrap'
    , 'ui.utils'
    , 'grlDirectives'
    , 'appfeedback'
]);

yumKaaxApp.config(["$routeProvider", function ($routeProvider) {

    var urlBase = './Spa/Views/';

    $routeProvider
        .when('/', {
            templateUrl: urlBase + 'Home.html',
            controller: 'HomeCtrl',
            resolve: {
                auth: ["$q", "authService",function ($q, authService) {
                    var userInfo = authService.getUserInfo();

                    if (userInfo) {
                        return $q.when(userInfo);
                    } else {
                        return $q.reject({ authenticated: false });
                    }
                }]
            }
        })
        .when('/home',
            {
                templateUrl: urlBase + 'home.html',
                controller: 'HomeCtrl'
            })
        .when('/leygacetaofficialrpi',
            {
                templateUrl: urlBase + 'gacetaley.html',
                controller: 'gacetaLeyCtrl'
            })
        .when('/gaceta',
            {
                templateUrl: urlBase + 'gaceta.html',
                controller: 'gacetaCtrl'
            })
        .when('/gacetasemanal',
            {
                templateUrl: urlBase + 'gacetasemanal.html',
                controller: 'gacetaSemanalCtrl'
            })
        .when('/gacetasemanal/:gaceta',
            {
                templateUrl: urlBase + 'gacetasemanal.html',
                controller: 'gacetaSemanalCtrl'
            })
        .when('/mregistro',
            {
                templateUrl: urlBase + 'mregistro.html',
                controller: 'mexpedienteCtrl'
            })
        .when('/mexpediente',
            {
                templateUrl: urlBase + 'mexpediente.html',
                controller: 'mexpedienteCtrl'
            })
        .when('/mexpediente/:Id',
            {
                templateUrl: urlBase + 'mexpediente.html',
                controller: 'mexpedienteCtrl'
            })

        .when('/mpreIngreso',
            {
                templateUrl: urlBase + 'mpreIngreso.html',
                controller: 'mpreIngresoCtrl'
            })
        .when('/mpreIngreso/:Id', {
            templateUrl: urlBase + 'mpreIngresoABC.html',
            controller: 'mpreIngresoCtrl'
        })
        .when('/mfonetica',
            {
                templateUrl: urlBase + 'mfonetica.html',
                controller: 'mfoneticaCtrl'
            })
        .when('/mlogotipos',
            {
                templateUrl: urlBase + 'mlogotipos.html',
                controller: 'mlogotiposCtrl'
            })

        .when('/pregistro',
            {
                templateUrl: urlBase + 'pregistro.html',
                controller: 'pexpedienteCtrl'
            })
        .when('/pexpediente',
            {
                templateUrl: urlBase + 'pexpediente.html',
                controller: 'pexpedienteCtrl'
            })
        .when('/pexpediente/:Id',
            {
                templateUrl: urlBase + 'pexpediente.html',
                controller: 'pexpedienteCtrl'
            })
        .when('/pbusquedad',
            {
                templateUrl: urlBase + 'pbusquedad.html',
                controller: 'pbusquedadCtrl'
            })

        .when('/dregistro',
            {
                templateUrl: urlBase + 'dregistro.html',
                controller: 'dexpedienteCtrl'
            })
        .when('/dexpediente',
            {
                templateUrl: urlBase + 'dexpediente.html',
                controller: 'dexpedienteCtrl'
            })
        .when('/dexpediente/:Id',
            {
                templateUrl: urlBase + 'dexpediente.html',
                controller: 'dexpedienteCtrl'
            })

        .when('/misexpedientes',
            {
                templateUrl: urlBase + 'misexpedientes.html',
                controller: 'misexpedientesCtrl'
            })

        .when('/rpilive',
            {
                templateUrl: urlBase + 'rpilive.html',
                controller: 'genericCtrl'
            })
        .when('/eTomo',
            {
                templateUrl: urlBase + 'eTomo.html',
                controller: 'genericCtrl'
            })
        .when('/econtacto',
            {
                templateUrl: urlBase + 'econtacto.html',
                controller: 'genericCtrl'
            })
        .when('/usuario',
            {
                templateUrl: urlBase + 'UsuariosPublicosAbc.html',
                controller: 'UsuariosPublicosCtrlr'
        })
        .when('/crear-cuenta',
            {
                templateUrl: urlBase + 'UsuariosPublicosA.html',
                controller: 'UsuarioCtrlr'
        })
        .when('/olvide-clave',
            {
                templateUrl: urlBase + 'UsuariosRecordarClave.html',
                controller: 'UsuarioCtrlr'
        })
        .when('/ResetPW/:miClave/:Id',
            {
                templateUrl: urlBase + 'UsuariosPublicosResetPW.html',
                controller: 'UsuariosPublicosCtrlr'
        })
        .when('/cambiarPW/:spk/:Id', {
            templateUrl: urlBase + 'UsuariosPublicosResetPW.html',
            controller: 'UsuariosPublicosCtrlr'
        })
        .when('/login',
            {
                templateUrl: urlBase + 'Login.html',
                controller: 'authCtrl'
            })
        .otherwise({
            redirectTo: ''
        });
}]);

yumKaaxApp.config(["cfpLoadingBarProvider", function (cfpLoadingBarProvider) {
    cfpLoadingBarProvider.includeSpinner = true;
}]);


yumKaaxApp.run(["$rootScope", "$location", "authService", function ($rootScope, $location, authService) {

    $rootScope.$on("$routeChangeSuccess", function (userInfo) {
        if ($location.path().indexOf("crear-cuenta") >= 0
            || $location.path().indexOf("cambiarPW") >= 0
            || $location.path().indexOf("olvide-clave") >= 0)
            return;

        if (authService.getUserInfo() === null || authService.getUserInfo() === undefined) {            
            $location.path("/login");
        }
    });

    $rootScope.$on("$routeChangeError", function (event, current, previous, eventObj) {
        if (eventObj.authenticated === false) {
            $location.path("/login");
        }
    });
}]);


yumKaaxApp.filter("dateformat", function () {
    var re = /\\\/Date\(([0-9]*)\)\\\//;
    return function (x) {
        var m = x.match(re);
        if (m) return new Date(parseInt(m[1]));
        else return null;
    };
});