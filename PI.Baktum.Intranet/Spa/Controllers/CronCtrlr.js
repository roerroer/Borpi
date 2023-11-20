
///---------------
/// Marcas
///---------------
yumKaaxControllers.controller("CronCtrlr",
    ["$scope", "$http", "$q", "$routeParams", "$timeout", "$route", "$location", "authService", "listService", "bouncer", "feedbackSrvc", 
        function ($scope, $http, $q, $routeParams, $timeout, $route, $location, authService, listService, bouncer, feedbackSrvc) {
      
    //Place separate controller
    $scope.GetDOCResol = function (cronologiaId) {

        var service = '/Expediente/';
        $http.get(authService.api() + service + "GetDOCResol?cronologiaId=" + cronologiaId)
            .then(function (result) {
                if (result.data)
                    listService.printDoc(result);

                feedbackSrvc.showAlertInfo("Evento no contiene documento imprimible...");
                console.log(result);
            });
    };

}]); //End Paises



