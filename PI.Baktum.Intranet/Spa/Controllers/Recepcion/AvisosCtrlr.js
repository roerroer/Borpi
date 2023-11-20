///------------------------------------------------------------------------------------------
/// Gaceta - Avisos, pero tambien consulta de otros edictos/doctos publicados
///------------------------------------------------------------------------------------------
yumKaaxControllers.controller("AvisosCtrlr",
    ["$scope", "$http", "$q", "$routeParams", "$timeout", "$route", "dateHelper", "authService", "listService","feedbackSrvc",
        function ($scope, $http, $q, $routeParams, $timeout, $route, dateHelper, authService, listService, feedbackSrvc) {
            $scope.$route = $route;
            $scope.avisos = [];
            $scope.publicacion = {};
            $scope.seccionSelected = 4;
            $scope.totalItems = 0;
            $scope.currentPage = 1;
            $scope.maxSize = 5;
            $scope.pageSize = 10;
            $scope.metaData = {};

            // Disable weekend selection
            $scope.disabled = dateHelper.disabled;

            $scope.minDate = dateHelper.minDate();
            $scope.maxDate = dateHelper.maxDate();

            $scope.fp = { opened: false };
            $scope.fe = { opened: false };

            $scope.openFp = function ($event) {
                $scope.fp.opened = true;
            };

            $scope.openFe = function ($event) {
                $scope.fe.opened = true;
            };

            //calendar options
            $scope.dateOptions = dateHelper.dateOptions;

            //calendar date format
            $scope.formats = dateHelper.formats;
            $scope.format = $scope.formats[1];

            authService.custodian(function () {
                $scope.lists = listService.getLists();
                $scope.secciones = $scope.lists.seccionesGaceta;
                //$scope.publicacion.Id = $scope.lists.seccionesGaceta.filter(function (s) { return s.Id = 4; })[0];
                $scope.tiposDeRegistro = $scope.lists.tipoDeRegistroDeMarcas;

                if ($routeParams.Id) {
                    getEntity($routeParams.Id);
                }
                else {
                    getResultsPage(1, $scope.pageSize);
                }
            });

            $scope.onSeccionChange = function () {
                if ($scope.publicacion.GacetaSeccionId === 1)
                    $scope.tiposDeRegistro = $scope.lists.tipoDeRegistroDeMarcas;
                else if ($scope.publicacion.GacetaSeccionId === 2)
                    $scope.tiposDeRegistro = $scope.lists.tipoDePatentes;
                else if ($scope.publicacion.GacetaSeccionId === 3)
                    $scope.tiposDeRegistro = [{ Id: 35, Nombre: "Anotaciones" }];
                else if ($scope.publicacion.GacetaSeccionId === 4)
                    $scope.tiposDeRegistro = $scope.lists.tipoDeRegistroDeMarcas;
                else
                    $scope.tiposDeRegistro = [{ Id: 35, Nombre: "Anotaciones" }];
            };            

            function getEntity(Id) {
                if (Id > 0) {
                    $http.get(authService.api() + '/Admin/GacetaAbc/Index?id=' + Id)
                        .then(function (result) {
                            $scope.publicacion = result.data.Result;
                            $scope.metaData = $scope.publicacion.JSONDOC ? JSON.parse($scope.publicacion.JSONDOC) : {};
                            $scope.mMetaData = angular.copy($scope.metaData);
                            $scope.publicacion.FechaPublicacion = dateHelper.parseStrDate($scope.publicacion.FechaPublicacion);
                            $scope.publicacion.FechaEdicto = dateHelper.parseStrDate($scope.publicacion.FechaEdicto);

                            $scope.master = angular.copy($scope.publicacion);
                            
                            $scope.onSeccionChange();
                        });
                }
                else {
                    $scope.publicacion = {};
                    $scope.publicacion.Id = -1;                    
                    $scope.publicacion.GacetaSeccionId = $routeParams.GacetaSeccionId*1;
                    $scope.publicacion.TipoDeRegistroId = 0;
                    $scope.metaData = {};
                    $scope.mMetaData = angular.copy($scope.metaData);
                    $scope.master = angular.copy($scope.publicacion);
                    $timeout(function () {$scope.onSeccionChange();});
                }                
            }

            $scope.isUnchanged = function (publicacion, metaData) {
                return angular.equals(publicacion, $scope.master) && angular.equals(metaData, $scope.mMetaData);
            };

            $scope.pagination = {
                current: 1
            };

            $scope.pageChanged = function () {
                getResultsPage($scope.currentPage, $scope.pageSize);
            };

            $scope.getGacetaBySeccionSelected = function (element, pageNumber, pageSize) {
                $scope.currentPage = 1;
                getResultsPage($scope.currentPage, $scope.pageSize);
            };

            function getResultsPage(pageNumber, pageSize) {
                $http.get(authService.api() + '/Admin/GacetaAbc/GetPageBySeccion?seccionId=' + $scope.seccionSelected + '&page=' + pageNumber + "&pageSize=" + pageSize)
                        .then(function (result) {
                            $scope.avisos = result.data.Result.DataSet;
                            $scope.totalItems = result.data.Result.TotalItems;
                        });
            }

            $scope.update = function (publicacion) {
                publicacion.enabled = true;
                publicacion.JSONDOC = JSON.stringify($scope.metaData);
                $http.post(authService.api() + "/Admin/GacetaAbc/save", { model: publicacion })
                    .then(
                        function (result) {
                            if (result.data.Succeeded) {
                                feedbackSrvc.showAlertInfo("publicacion ha sido grabado correctamente...", "/Gaceta");
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

        }]);
