
///---------------
/// Usuarios Externos
///---------------
yumKaaxControllers.controller("UsuariosPublicosCtrlr",
    ["$scope", "$http", "$q", "$routeParams", "$timeout", "$location", "authService", "bouncer", "feedbackSrvc",
        function ($scope, $http, $q, $routeParams, $timeout, $location, authService, bouncer, feedbackSrvc) {
        $scope.usuarios = [];
        $scope.usuario = {}
        $scope.paises = [];

        $scope.totalItems = 0;
        $scope.currentPage = 1;
        $scope.maxSize = 5;
        $scope.pageSize = 10;

        if ($routeParams.spk && $routeParams.Id) {
            getUserWithSpk($routeParams.spk, $routeParams.Id);
        }
        if ($routeParams.Id) {
            getPaises();
            getUser($routeParams.Id);
        }
        else {
            getResultsPage(1, $scope.pageSize);
        }
        //alert('wtf usuarios');
        function getUserWithSpk(spk, userId) {
            $http.post(authService.api() + '/Admin/UsuariosPublicos/getwithspk', { Id: userId, Spk: spk }, { headers: { "access_token": spk } })
                .then(function (result) {
                    $scope.usuario = result.data.Result;
                    if (result.data.Succeeded) {
                        $scope.usuario = result.data.Result;
                    }
                    else {
                        feedbackSrvc.handleError(result.data);
                    }
                    console.log(JSON.stringify(result.data.Result));
                },
                function (error) {
                    feedbackSrvc.handleError(error);
                });
        };

        function getPaises() {
            $http.get(authService.api() + '/Admin/Entities/Get?entity=paises')
            .then(function (result) {
                $scope.paises = result.data.Result;
                console.log('paises');
                console.log(JSON.stringify(result.data.Result));
            });
        };

        function getUser(userId) {
            $http.get(authService.api() + '/Admin/UsuariosPublicos/Index?id=' + userId)
            .then(function (result) {

                $scope.usuario = result.data.Result;
                //console.log(JSON.stringify(result.data.Result));
            });
        };

        $scope.isUnchanged = function (usuario) {
            return angular.equals(usuario, $scope.master);
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
            $http.get(authService.api() + '/Admin/UsuariosPublicos/GetPageFilter?filter=' + element + '&page=' + pageNumber + "&pageSize=" + pageSize)
                .then(function (result) {
                    $scope.usuarios = result.data.Result.DataSet;
                    $scope.totalItems = result.data.TotalItems;
                    //`.log(JSON.stringify(result));
                });
        };

        function getResultsPage(pageNumber, pageSize) {
            if ($scope.textSearch)
                $scope.filterFunction($scope.textSearch, pageNumber, pageSize);
            else
                $http.get(authService.api() + '/Admin/UsuariosPublicos/GetPage?page=' + pageNumber + "&pageSize=" + pageSize)
                .then(function (result) {
                    $scope.usuarios = result.data.Result.DataSet;
                    $scope.totalItems = result.data.Result.TotalItems;
                    //console.log(JSON.stringify(result));
                });
        }

        $scope.update = function (usuario) {
            $http.post(authService.api() + "/Admin/UsuariosPublicos/save", { model: usuario })
                .then(
                    function (result) {
                        if (result.data.Succeeded) {
                            feedbackSrvc.showAlertInfo("Usuario ha sido grabado correctamente...");
                        }
                        else {
                            feedbackSrvc.showAlertInfo("Error al intentar grabar el Usuario...consulte al admin del sistema");
                        }
                    },
                    function (error) {
                        feedbackSrvc.handleError(error);
                    }
                );
        };

        $scope.setPW = function (usuario) {

            $http.post(authService.api() + "/Admin/UsuariosPublicos/ResetPW", { model: usuario })
                .then(
                    function (result) {
                        if (result.data.Succeeded) {
                            feedbackSrvc.showAlertInfo("Contrasena ha sido grabada correctamente...");
                        }
                        else {
                            feedbackSrvc.showAlertInfo("Error al intentar grabar contrasena...consulte al admin del sistema");
                        }
                    },
                    function (error) {
                        feedbackSrvc.handleError(error, "Error al intentar grabar contrasena...consulte al admin del sistema");
                    }
                );
        };

    }]);