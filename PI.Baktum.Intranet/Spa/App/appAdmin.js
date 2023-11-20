var yumKaaxApp = angular.module('yumKaaxApp',
    [
        'ngRoute',
        'gespiControllers',
        'pageFactory',
        'authFactory',
        'grlDirectives',
        'ngAnimate',
        'ui.bootstrap',
        'appfeedback'
    ]
);

yumKaaxApp.config(['$routeProvider', function ($routeProvider) {
    var urlBase = './Spa/Views/';

    $routeProvider
    .when('/Agentes', {
        templateUrl: urlBase + 'Admin/Agentes.html',
        controller: 'AgentesCtrlr'
    })
    .when('/Agentes/:Id', {
        templateUrl: urlBase + 'Admin/AgentesAbc.html',
        controller: 'AgentesCtrlr'
    })
    .when('/Paises', {
        templateUrl: urlBase + 'Admin/Paises.html',
        controller: 'PaisesCtrlr'
    })
    .when('/Paises/:Id', {
        templateUrl: urlBase + 'Admin/PaisesAbc.html',
        controller: 'PaisesCtrlr'
    })
    .when('/Estatus', {
        templateUrl: urlBase + 'Admin/Estatus.html',
        controller: 'EstatusesCtrlr'
    })
    .when('/Estatus/:Id', {
        templateUrl: urlBase + 'Admin/EstatusAbc.html',
        controller: 'EstatusesCtrlr'
    })
    .when('/Eventos', {
        templateUrl: urlBase + 'Admin/Eventos.html',
        controller: 'EventosCtrlr'
    })
    .when('/Eventos/:Id', {
        templateUrl: urlBase + 'Admin/EventosAbc.html',
        controller: 'EventosCtrlr'
    })
    .when('/OpsXEvento/:Id', {
        templateUrl: urlBase + 'Admin/OpsXEvento.html',
        controller: 'OxECtrlr'
    })    
    .when('/Leyes', {
        templateUrl: urlBase + 'Admin/Leyes.html',
        controller: 'LeyesCtrlr'
    })
    .when('/Niza', {
        templateUrl: urlBase + 'Admin/Niza.html',
        controller: 'NizaCtrlr'
    })
    .when('/Niza/:Id', {
        templateUrl: urlBase + 'Admin/NizaAbc.html',
        controller: 'NizaCtrlr'
    })
    .when('/Vienna', {
        templateUrl: urlBase + 'Admin/Vienna.html',
        controller: 'ViennaCtrlr'
    })
    .when('/Vienna/:Id', {
        templateUrl: urlBase + 'Admin/ViennaAbc.html',
        controller: 'ViennaCtrlr'
    })
    .when('/Roles', {
        templateUrl: urlBase + 'Admin/Roles.html',
        controller: 'RolesCtrlr'
    })
    .when('/Usuarios/:Id', {
        templateUrl: urlBase + 'Admin/UsuariosAbc.html',
        controller: 'UsuariosCtrlr'
    })
    .when('/Usuarios/ResetPW/:spk/:Id', {
        templateUrl: urlBase + 'Admin/UsuariosResetPW.html',
        controller: 'UsuariosCtrlr'
    })
    .when('/Usuarios/Permisos/:Id', {
        templateUrl: urlBase + 'Admin/UsuariosPerm.html',
        controller: 'UsuariosCtrlr'
    })
    .when('/Usuarios', {
        templateUrl: urlBase + 'Admin/Usuarios.html',
        controller: 'UsuariosCtrlr'
    })
    // Usuarios Publicos
    .when('/eConsulta/:Id', {
        templateUrl: urlBase + 'Admin/UsuariosPublicosAbc.html',
        controller: 'UsuariosPublicosCtrlr'
    })
    .when('/eConsulta/ResetPW/:spk/:Id', {
        templateUrl: urlBase + 'Admin/UsuariosPublicosResetPW.html',
        controller: 'UsuariosPublicosCtrlr'
    })
    .when('/eConsulta', {
        templateUrl: urlBase + 'Admin/UsuariosPublicos.html',
        controller: 'UsuariosPublicosCtrlr'
    })
    .otherwise({
        redirectTo: ''
    });
}]);
