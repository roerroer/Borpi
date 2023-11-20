///---------------
/// Agentes
///---------------
yumKaaxControllers.controller("AgentesCtrlr", ["$scope", "$http", "$q", "$routeParams", "$timeout", "$route", "authService", "feedbackSrvc",
    function ($scope, $http, $q, $routeParams, $timeout, $route, authService, feedbackSrvc) {
    $scope.$route = $route;
    $scope.Agentes = [];
    $scope.agente = {};
    
    $scope.totalItems = 0;
    $scope.currentPage = 1;
    $scope.maxSize = 5;
    $scope.pageSize = 10;

    if ($routeParams.Id) {
        getEntity($routeParams.Id);
    }
    else {
        getResultsPage(1, $scope.pageSize);
    }

    function getEntity(Id) {
        $http.get( authService.api() + '/Admin/Agentes/Index?id=' + Id)
        .then(function (result) {           
            //$scope.agente = result.data.Result;
            $scope.agente = result.data.Result;
            //console.log(JSON.stringify(result.data.Result));
        });
    };

    $scope.isUnchanged = function (agente) {
        return angular.equals(agente, $scope.master);
    };

    $scope.pagination = {
        current: 1
    };

    $scope.pageChanged = function () {
        getResultsPage($scope.currentPage, $scope.pageSize);
    };

    $scope.$watch('textSearch', function (newVal, oldVal) {
        if (newVal !== oldVal) {
            $scope.filterFunction(newVal, $scope.pageNumber, $scope.pageSize);
        }
    }, true);

    $scope.filterFunction = function (element, pageNumber, pageSize) {
        $http.get(authService.api() + '/Admin/Agentes/GetPageFilter?filter=' + element + '&page=' + pageNumber + "&pageSize=" + pageSize)
            .then(function (result) {
                $scope.Agentes = result.data.Result.DataSet;
                $scope.totalItems = result.data.TotalItems;
                console.log(JSON.stringify(result));
            });
    };

    function getResultsPage(pageNumber, pageSize) {
        if ($scope.textSearch)
            $scope.filterFunction($scope.textSearch, pageNumber, pageSize);
        else
            $http.get(authService.api() + '/Admin/Agentes/GetPage?page=' + pageNumber + "&pageSize=" + pageSize)
            .then(function (result) {
                $scope.Agentes = result.data.Result.DataSet;
                $scope.totalItems = result.data.Result.TotalItems;
                //console.log(JSON.stringify(result));
            });
    }

    $scope.update = function (agente) {
        $http.post(authService.api() + "/Admin/Agentes/save", { model: agente })
            .then(
                function (result) {
                    if (result.data.Succeeded) {
                        feedbackSrvc.handleError(null, "agente ha sido grabado correctamente...");
                    }
                    else {
                        feedbackSrvc.handleError(null);
                    }
                },
                function (error) {
                    feedbackSrvc.handleError(error);
                }
            );        
    };

}]); //End Agentes
