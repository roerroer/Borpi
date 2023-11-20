var yumKaaxApp = angular.module('yumKaaxApp',
    [
        'ngRoute',
        'gespiControllers',
        'eRegistroAPI',
        'authFactory',
        'utilsFactory',
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
    .when('/', {
        templateUrl: urlBase + 'eMarcas/Home.html',
        controller: 'eRPICtrl'
    })
    .when('/login',//redirect to base !!
    {
        templateUrl: urlBase + 'Base/Login.html',
        controller: 'authCtrl'
    })
    .when('/Marcas',
    {
        //templateUrl: urlBase + 'eMarcas/Marcas.html',
        templateUrl: urlBase + 'shared/SolicitudTpl.html',
        controller: 'MarcasCtrl'
    })
    .when('/Marcas/:numero',
    {
        //templateUrl: urlBase + 'eMarcas/Marcas.html',
        templateUrl: urlBase + 'shared/SolicitudTpl.html',
        controller: 'MarcasCtrl'
    })
    .when('/Marcas/id/:id',
    {
        //templateUrl: urlBase + 'eMarcas/Marcas.html',
        templateUrl: urlBase + 'shared/SolicitudTpl.html',
        controller: 'MarcasCtrl'
    })
    .when('/Anotaciones',
    {
        //templateUrl: urlBase + 'eAnota/Anota.html',
        templateUrl: urlBase + 'shared/SolicitudTpl.html',
        controller: 'AnotacionesCtrl'
    })
    .when('/Anotaciones/:numero',
    {
        //templateUrl: urlBase + 'eAnota/Anota.html',
        templateUrl: urlBase + 'shared/SolicitudTpl.html',
        controller: 'AnotacionesCtrl'
    })
    .when('/Renovaciones',
    {
        templateUrl: urlBase + 'shared/SolicitudTpl.html',
        controller: 'RenovacionesCtrl'
    })
    .when('/Renovaciones/:numero',
    {
        templateUrl: urlBase + 'shared/SolicitudTpl.html',
        controller: 'RenovacionesCtrl'
    })
    .when('/BusquedasExt',
    {
        templateUrl: urlBase + 'eMarcas/busquedas-ext.html',
        controller: 'BusquedasExtCtrl'
    })
    .when('/Patentes',
    {
        //templateUrl: urlBase + 'ePatentes/Patentes.html',
        templateUrl: urlBase + 'shared/SolicitudTpl.html',
        controller: 'PatenteCtrl'
    })
    .when('/Patentes/:numero/:tipo', {
        //templateUrl: urlBase + 'ePatentes/Patentes.html',
        templateUrl: urlBase + 'shared/SolicitudTpl.html',
        controller: 'PatenteCtrl'
    })
    .when('/DA',
    {
        //templateUrl: urlBase + 'eDA/DA.html',
        templateUrl: urlBase + 'shared/SolicitudTpl.html',
        controller: 'DACtrl'
    })
    .when('/DA/:numero',
    {
        //templateUrl: urlBase + 'eDA/DA.html',
        templateUrl: urlBase + 'shared/SolicitudTpl.html',
        controller: 'DACtrl'
    })
    .otherwise({
        redirectTo: ''
    });
}]);