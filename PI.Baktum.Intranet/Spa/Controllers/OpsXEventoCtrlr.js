
///---------------
/// OxECtrlr
///---------------
yumKaaxControllers.controller("OxECtrlr",
    ["$scope", "$http", "$q", "$routeParams", "$timeout", "$route", "authService", "bouncer", "feedbackSrvc",
        function ($scope, $http, $q, $routeParams, $timeout, $route, authService, bouncer, feedbackSrvc) {
        $scope.$route = $route;
        $scope.OpsXEventos = [];
        $scope.totalItems = 0;
        $scope.pageSize = 10;
        $scope.opcion = {};
        $scope.Opciones = [];
        $scope.Estatuses = [];
        $scope.eModuloId = 0;

        if ($routeParams.Id) {
            $scope.eventoId = $routeParams.Id;
            getEntity($routeParams.Id);
        }

        function getEntity(Id) {

            $http.get(authService.api() + '/Admin/Eventos/Index?id=' + Id)
                .then(function (result) {
                    $scope.eventoName = result.data.Result.Descripcion;
                    $scope.eModuloId = result.data.Result.ModuloId;
                    console.log('<Eventos>');

                    //Get them all page=0
                    $http.get('/Admin/Opciones/GetPage?page=0' + "&pageSize=0" + "&moduloId=" + $scope.eModuloId)
                    .then(function (result) {                        
                        $scope.Opciones = result.data.Result.DataSet;
                        console.log('opciones');
                    });

                    //Get them all page=0
                    $http.get('/Admin/Estatus/GetPage?page=0' + "&pageSize=0" + "&moduloId=" + $scope.eModuloId)
                    .then(function (result) {
                        $scope.Estatuses = result.data.Result.DataSet;
                        console.log('Estatuses');
                    });

                });

            getResultsPage(1, $scope.pageSize);

        };

        $scope.isUnchanged = function (ops) {
            return angular.equals(ops, $scope.master);
        };

        $scope.pagination = {
            current: 1
        };

        $scope.pageChanged = function (newPage) {
            getResultsPage(newPage, $scope.pageSize);
        };

        $scope.$watch('textSearch', function (newVal, oldVal) {
            if (newVal !== oldVal) {
                $scope.filterFunction(newVal, $scope.pageNumber, $scope.pageSize);
            }
        }, true);

        $scope.filterFunction = function (element, pageNumber, pageSize) {
            $http.get(authService.api() + '/Admin/OpsXEvento/GetPageFilter?filter=' + element + '&page=' + pageNumber + "&pageSize=" + pageSize + "&eventoId=" + $scope.eventoId)
                .then(function (result) {
                    $scope.OpsXEventos = result.data.Result.DataSet;
                    $scope.totalItems = result.data.TotalItems;
                    console.log(JSON.stringify(result));
                });
        };

        function getResultsPage(pageNumber, pageSize) {
            if ($scope.textSearch)
                $scope.filterFunction($scope.textSearch, pageNumber, pageSize);
            else
                $http.get(authService.api() + '/Admin/OpsXEvento/GetPage?page=' + pageNumber + "&pageSize=" + pageSize + "&eventoId=" + $scope.eventoId)
                .then(function (result) {
                    $scope.OpsXEventos = result.data.Result.DataSet;
                    $scope.totalItems = result.data.Result.TotalItems;
                    //console.log(JSON.stringify(result));
                });
        }

        $scope.update = function (ops) {
            alert($scope.eModuloId);
            alert($scope.eventoId);
            alert(ops);
            alert(JSON.stringify(ops));
            $http.post(authService.api() + "/Admin/OpsXEvento/save", { model: ops })
                .then(
                    function (result) {
                        if (result.data.Succeeded) {
                            feedbackSrvc.showAlertInfo("ops ha sido grabado correctamente...");
                        }
                        else {
                            feedbackSrvc.showAlertInfo("Error al intentar grabar el ops...consulte al admin del sistema");
                        }
                    },
                    function (error) {
                        feedbackSrvc.handleError(error);
                    }
                );
        };

    }
    ]); //End OxECtrlr
