var yumKaaxApp = angular.module('yumKaaxApp',
    [
        'ngRoute',
        'gespiControllers',
        'authFactory',
        'grlDirectives',
        'ngAnimate',
        'ngSanitize',
        'ui.bootstrap',
        'ngSanitize',
        'ui.utils',
        'textAngular',
        'appfeedback'
    ]
);


yumKaaxApp.config(['$routeProvider', function ($routeProvider) {
    var urlBase = './Spa/Views/';

    $routeProvider
    .when('/Gaceta', {
    templateUrl: urlBase + 'Recepcion/GacetaPublicacion.html',
    controller: 'AvisosCtrlr'
    })
        .when('/Gaceta/:Id/:GacetaSeccionId', {
        templateUrl: urlBase + 'Recepcion/GacetaPublicacionAbc.html',
        controller: 'AvisosCtrlr'
    })
    .otherwise({
        redirectTo: ''
    });
}]);

