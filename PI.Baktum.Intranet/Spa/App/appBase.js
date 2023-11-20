var yumKaaxApp = angular.module('yumKaaxApp',
    [
        'ngRoute',
        'gespiControllers',
        'authFactory',
        'grlDirectives',
        'ui.bootstrap',
        'appfeedback'
    ]
);

yumKaaxApp.config(['$routeProvider', function ($routeProvider) {
    var urlBase = './Spa/Views/';
    $routeProvider
    .when('/', {
        templateUrl: urlBase + 'Base/Home.html',
        controller: 'HomeCtrl'
    })
    .when('/login',
    {
        templateUrl: urlBase + 'Base/Login.html',
        controller: 'authCtrl'
    })
    .otherwise({
        redirectTo: ''
    });
}]);


