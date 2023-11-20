yumKaaxControllers.controller("mexpedienteCtrl",
    ["$scope", "authService", "listService", "$http", "$routeParams", "feedbackSrvc",
        function ($scope, authService, listService, $http, $routeParams, feedbackSrvc) {
        $scope.tipoDeRegistroDeMarcas = [];
        $scope.model = { EsFavorito: false };
        $scope.grupos = [];
        $scope.lists = {};

        function getExpediente(action, queryString) {
            var service = '/Marca/';

            $http.get(authService.api() + service + action + queryString,
                {
                    headers: { "access_token": authService.getToken() }
                })
                .then(function (result) {
                    $scope.model = result.data.Result.documento;
                    
                    console.log('Fetched!!');
                });
        }

        $scope.getExpedienteByNumero = function (expediente) {
            getExpediente('Expediente', '?numero=' + expediente);
        };

        $scope.getExpedienteByRegistro = function (tipoDeRegistroId, registro) {
            getExpediente('registro', '?tipoDeRegistroId=' + tipoDeRegistroId.Id + '&registro=' + registro);
        };

        $scope.tipoDeRegistroDeMarcas = [];

        function getTipoDeRegistro() {
            listService.reFetchLists();
            var lists = listService.getLists(); 
            $scope.tipoDeRegistroDeMarcas = lists.tipoDeRegistroDeMarcas;
        }

        $scope.resolvePais = function (paisId) {
            var result = !$scope.lists.paises ? [] : $scope.lists.paises.filter(function (p) { return p.Id == paisId; });
            return result.length > 0 ? result[0].Nombre : "";
        };

        if ($routeParams.Id) {
            getExpediente('ExpedienteId', '?Id=' + $routeParams.Id);
        }

        authService.custodian(function () {
            $scope.lists = listService.getLists();
            getTipoDeRegistro();
        });

    }]);

yumKaaxControllers.controller("mlogotiposCtrl",
    ["$scope", "$location", "authService", "classSrvc", "feedbackSrvc",
        function ($scope, $location, authService, classSrvc, feedbackSrvc) {
        $scope.userIdentity = authService.getUserInfo();
        $scope.niza = classSrvc.getNizaClass();
        $scope.nizaSel = { niza: {} };
        $scope.vienalstSel = [];
        $scope.vienalst = classSrvc.getVienaClass();

        $scope.selectViena = function (viena) {
            $scope.vienalstSel.push(viena);
            $scope.vienaSelected = null;
        };
    }]);

yumKaaxControllers.controller("mfoneticaCtrl",
    ["$scope", "$location", "authService", "classSrvc", "$http", "feedbackSrvc",
        function ($scope, $location, authService, classSrvc, $http, feedbackSrvc) {
        $scope.userIdentity = authService.getUserInfo();
        $scope.niza = classSrvc.getNizaClass();
        $scope.nizaSel = { niza: {} };
        $scope.tipoDeBusqueda = [{ Id: 1, Nombre: "Identica" }, { Id: 2, Nombre: "Fonetica" }];
        $scope.tipoDeBusquedaSel = $scope.tipoDeBusqueda[1];
        $scope.pageSize = 0;
        $scope.dataset = [];

        $scope.pageNumber = 1;

        $scope.pageChanged = function () {
            $scope.searchMarcas();
        };

        $scope.searchMarcas = function () {

            var csvClases = '';
            var nizaSel = $scope.nizaSel.niza;
            if (nizaSel['All']) {
                csvClases = '';
            }
            else {
                for (i = 0; i < 46; i++) {
                    if (nizaSel[i]) {
                        csvClases += i.toString() + ',';
                    }
                }
                if (nizaSel[99]) {
                    csvClases += '99,';
                }
            }
            if (csvClases.length > 1) {
                csvClases = csvClases.slice(0, -1);
            }

            var service = '/Marca/';

            $scope.getExpedienteUrl = function (e) {
                return "mexpediente/" + e.ExpedienteId;
            };

            //TODO: pagination missing!
            if (!$scope.tipoDeBusquedaSel || $scope.tipoDeBusquedaSel.Id == 2) {
                $http.get(authService.api() + service + 'BusquedaFonetica?pageNumber=' + $scope.pageNumber
                    + "&pageSize=" + $scope.pageSize
                    + "&textToSearch=" + $scope.textSearch.toUpperCase()
                    + "&csvClases=" + csvClases,
                    {
                        headers: { "access_token": authService.getToken() }
                    })
                    .then(function (result) {
                        $scope.dataset = result.data.Result.DataSet;
                        console.log(JSON.stringify(result.data.Result.DataSet[0]));
                        $scope.totalItems = result.data.Result.TotalItems;
                    });
            }
            else {
                $http.get(authService.api() + service + 'BusquedaIdentica?pageNumber=' + $scope.pageNumber
                    + "&pageSize=" + $scope.pageSize
                    + "&textToSearch=" + $scope.textSearch.toUpperCase()
                    + "&csvClases=" + csvClases,
                    {
                        headers: { "access_token": authService.getToken() }
                    })
                    .then(function (result) {
                        $scope.dataset = result.data.Result.DataSet;
                        console.log(JSON.stringify(result.data.Result.DataSet[0]));
                        $scope.totalItems = result.data.Result.TotalItems;
                    });
            }
        };

    }]);


yumKaaxControllers.controller("mpreIngresoCtrl",
    ["$scope", "$location", "authService", "$routeParams", "$timeout", "feedbackSrvc",
        function ($scope, $location, authService, $routeParams, $timeout, feedbackSrvc) {
            $scope.userIdentity = authService.getUserInfo();
            $scope.misExpedientes = [];
            $scope.totalItems = 0;
            $scope.pageSize = 10;
            $scope.expediente = {};

            if ($routeParams.Id) {
                //getEntity($routeParams.Id);
            }
            else {
                //getResultsPage(1, $scope.pageSize);
                $scope.misExpedientes = [
                    { Id: '100001', SignoDistintivo: 'Demo 1', TipoDeRegistro: 'Registro Inicial de Marca', EntidadSolicitante: 'Demo', FechaUltimaActualizacion: '05/01/20015' },
                    { Id: '100002', SignoDistintivo: "Demo 2", TipoDeRegistro: "Nombre Comercial", EntidadSolicitante: "Demo", FechaUltimaActualizacion: '05/01/20015' },
                    { Id: '100003', SignoDistintivo: "Demo 3", TipoDeRegistro: "Nombre Comercial", EntidadSolicitante: "Demo", FechaUltimaActualizacion: '05/01/20015' }
                ];
            }
        }]);

//*************************************************
// CONTROLLERS DE PATENTES
//*************************************************
yumKaaxControllers.controller("pexpedienteCtrl",
    ["$scope", "$location", "authService", "listService", "$http", "$routeParams", "feedbackSrvc",
        function ($scope, $location, authService, listService, $http, $routeParams, feedbackSrvc) {
            $scope.userIdentity = authService.getUserInfo();
            $scope.tipoDePatentes = [];
            $scope.model = {};
            $scope.service = '/Patente/';

            $scope.grupos = [];
            $scope.model.EsFavorito = false;
            $scope.lists = {};

            authService.custodian(function () {
                $scope.lists = listService.getLists();
                getTipoDeRegistro();
            });

            if ($routeParams.Id) {
                getExpediente('ExpedienteId', '?Id=' + $routeParams.Id);
            }

            function getExpediente(action, queryString) {
                var service = '/Patente/';

                $http.get(authService.api() + service + action + queryString,
                    {
                        headers: { "access_token": authService.getToken() }
                    })
                    .then(function (result) {
                        $scope.model = result.data.Result.documento;

                        for (var i = 0; i < $scope.tipoDePatentes.length; i++) {
                            if ($scope.tipoDePatentes[i].Id == $scope.model.Expediente.TipoDeRegistroId)
                                $scope.model.Expediente.TipoDeRegistroId = $scope.tipoDePatentes[i];
                        }

                        console.log('Fetched by expediente!!');
                    });
            }

            $scope.getExpedienteByNumero = function (tipoDeRegistroId, expediente) {
                getExpediente('Expediente', '?Numero=' + expediente + '&TipoDeRegistroId=' + tipoDeRegistroId.Id);
            };

            $scope.getExpedienteByRegistro = function (tipoDeRegistroId, registro) {
                getExpediente('Registro', '?Registro=' + registro + '&TipoDeRegistroId=' + tipoDeRegistroId.Id);
            };

            $scope.resolvePais = function (paisId) {
                var result = !$scope.lists.paises ? [] : $scope.lists.paises.filter(function (p) { return p.Id == paisId; });
                return result.length > 0 ? result[0].Nombre : "";
            };

            function getTipoDeRegistro() {
                listService.reFetchLists();
                var lists = listService.getLists(); // all the lists
                $scope.tipoDePatentes = lists.tipoDePatentes;
            }

        }]);


yumKaaxControllers.controller("pbusquedadCtrl",
    ["$scope", "authService", "$http", "listService", "feedbackSrvc",
        function ($scope, authService, $http, listService, feedbackSrvc) {
            $scope.userIdentity = authService.getUserInfo();
            $scope.pageNumber = 0;
            $scope.pageSize = 0;
            $scope.dataset = [];
            $scope.tipoDePatentes = [];
            $scope.lists = {};

            authService.custodian(function () {
                $scope.lists = listService.getLists();
                getTipoDeRegistro();
            });

            function getTipoDeRegistro() {
                listService.reFetchLists();
                var lists = listService.getLists(); // all the lists
                $scope.tipoDePatentes = lists.tipoDePatentes;
            }

            $scope.searchPatentes = function () {
                var service = '/Patente/';

                $scope.getExpedienteUrl = function (e) {
                    return "pexpediente/" + e.ExpedienteId;
                };
                var tipoDePatente = null;
                if ($scope.tipoDeRegistroId) {
                    tipoDePatente = $scope.tipoDeRegistroId.Id;
                }

                $http.get(authService.api() + service + 'BusquedaPatentesDsc?pageNumber=' + $scope.pageNumber
                    + "&pageSize=" + $scope.pageSize
                    + "&textToSearch=" + $scope.textSearch.toUpperCase()
                    + "&tipoDeRegistro=" + tipoDePatente,
                    {
                        headers: { "access_token": authService.getToken() }
                    })
                    .then(function (result) {
                        $scope.dataset = result.data.Result.DataSet;
                        $scope.totalItems = result.data.Result.DataSet.length;//tbfix
                    });
            };

        }]);


/*
 *FAVORITOS - GOES EVERYWHERE
 */
yumKaaxControllers.controller("favoritosCtrl",
    ["$scope", "$location", "authService", "classSrvc", "$http", "$timeout", "feedbackSrvc",
        function ($scope, $location, authService, classSrvc, $http, $timeout, feedbackSrvc) {

            function Init() {
                classSrvc.getGrupos()
                    .success(function (result) {
                        $scope.grupos = result.Result.DataSet;
                        if ($scope.grupos.length === 0) {
                            $scope.grupos.push({ Id: 0, Nombre: "Mis Expedientes" });
                        }
                    })
                    .error(function (error) {
                        $scope.grupos = [{ id: 0, Nombre: 'Todos' }];
                    });
            }

            authService.custodian(function () {
                Init();
            });

            $scope.addFavorito = function addFavorito(expediente, grupoId) {
                $http.post(authService.api() + "/Favoritos/AddFavorito", { expediente: expediente, grupoId: grupoId }, { headers: { "access_token": authService.getToken() } })
                    .then(
                        function (result) {
                            if (result.data.Succeeded) {
                                $scope.model.EsFavorito = true;
                            }
                            else {
                                feedbackSrvc.showAlertInfo("Error agregando expediente a mi listado de expedientes...");
                            }
                        },
                        function (error) {
                            feedbackSrvc.handleError(error, "Error agregando expediente a mi listado de expedientes...");
                        }
                    );
            };

            $scope.esFavoritoBtn = function esFavoritoBtn() {
                if ($scope.model.EsFavorito)
                    return 'btn btn-primary btn-xs dropdown-toggle';
                else
                    return 'btn btn-default btn-xs dropdown-toggle';
            };

            $scope.esFavoritoIcon = function esFavoritoIcon() {
                if ($scope.model.EsFavorito)
                    return 'glyphicon glyphicon-saved';
                else
                    return 'glyphicon glyphicon-pushpin';
            };
        }]);



/*
 * CONTROLLERS DE DERECHO DE AUTOR
 */
yumKaaxControllers.controller("dexpedienteCtrl",
    ["$scope", "$location", "authService", "listService", "$http", "$timeout", "$routeParams", "feedbackSrvc",
        function ($scope, $location, authService, listService, $http, $timeout, $routeParams, feedbackSrvc) {
            $scope.userIdentity = authService.getUserInfo();
            $scope.model = {};
            $scope.grupos = [];
            $scope.model.EsFavorito = false;
            $scope.lists = {};

            authService.custodian(function () {
                $scope.lists = listService.getLists();
            });
            $scope.resolvePais = function (paisId) {
                var result = !$scope.lists.paises ? [] : $scope.lists.paises.filter(function (p) { return p.Id == paisId; });
                return result.length > 0 ? result[0].Nombre : "";
            };
            if ($routeParams.Id) {
                getExpediente('ExpedienteId', '?Id=' + $routeParams.Id);
            }

            function getExpediente(action, queryString) {
                var service = '/DAutor/';

                $http.get(authService.api() + service + action + queryString,
                    {
                        headers: { "access_token": authService.getToken() }
                    })
                    .then(function (result) {
                        $scope.model = result.data.Result.documento;
                        //console.log(JSON.stringify($scope.opciones));
                        console.log('Fetched!!');
                    });
            }

            $scope.getExpedienteByNumero = function (expediente) {
                getExpediente('Expediente', '?Numero=' + expediente);
            };

            $scope.getExpedienteByRegistro = function (registro) {
                getExpediente('Registro', '?Registro=' + registro);
            };

        }]);


yumKaaxControllers.controller("gacetaCtrl",
    ["$scope", "authService", "$http", "listService", "dateHelper", "feedbackSrvc", "$sce",
        function ($scope, authService, $http, listService, dateHelper, feedbackSrvc, $sce) {
            $scope.Publicaciones = [];
            $scope.pageNumber = 1;
            $scope.totalItems = 0;
            $scope.pageSize = 100;
            $scope.lists = {};
            $scope.secciones = [];
            $scope.seccionGaceta = [];
            $scope.seccion = '/gaceta';
            // Disable weekend selection
            $scope.disabled = dateHelper.disabled;

            $scope.minDate = dateHelper.minDate();
            $scope.maxDate = dateHelper.maxDate();

            $scope.status = {
                opened: false
            };

            $scope.open = function ($event) {
                $scope.status.opened = true;
            };

            //calendar options
            $scope.dateOptions = dateHelper.dateOptions;

            //calendar date format
            $scope.formats = dateHelper.formats;
            $scope.format = $scope.formats[1];

            authService.custodian(function () {
                $scope.lists = listService.getLists();
                $scope.secciones = $scope.lists.seccionesGaceta;
                $scope.seccionGaceta = $scope.secciones[0];
                getResultsPage(1, $scope.pageSize);
            });

            $scope.resolvePais = function (paisId) {
                var result = !$scope.lists.paises ? [] : $scope.lists.paises.filter(function (p) { return p.Id === paisId; });
                return result.length > 0 ? result[0].Nombre : "";
            };

            $scope.onSeccionChange = function () {
                console.log($scope.seccionGaceta);
                if ($scope.seccionGaceta.Id == 1)
                    $scope.seccion = '/gaceta';
                else if ($scope.seccionGaceta.Id == 2)
                    $scope.seccion = '/gacetapatentesedictos';
                else if ($scope.seccionGaceta.Id == 3)
                    $scope.seccion = '/GacetaMarcasAnotaciones';
                else if ($scope.seccionGaceta.Id == 4)
                    $scope.seccion = '/GacetaGrl';
                $scope.pageNumber = 1;
                $scope.Publicaciones = [];
                getResultsPage(1, $scope.pageSize);
            };

            $scope.pageChanged = function () {
                if ($scope.fechaSelected && Object.prototype.toString.call($scope.fechaSelected) === "[object Date]") {
                    var dateFilter = $scope.fechaSelected.getDate() + "/" + ($scope.fechaSelected.getMonth() + 1) + "/" + $scope.fechaSelected.getFullYear();
                    $scope.filterFunction(dateFilter, $scope.pageNumber, $scope.pageSize);
                }
                else if ($scope.textSearch && $scope.textSearch !== '')
                    $scope.filterFunction($scope.textSearch, $scope.pageNumber, $scope.pageSize);
                else
                    getResultsPage($scope.pageNumber, $scope.pageSize);
            };

            $scope.$watch('textSearch', function (newVal, oldVal) {
                if (newVal !== oldVal) {
                    if (newVal == '')
                        getResultsPage($scope.pageNumber, $scope.pageSize);
                    else
                        $scope.filterFunction(newVal, $scope.pageNumber, $scope.pageSize);
                }
            }, true);

            $scope.$watch('fechaSelected', function (newVal, oldVal) {                
                if (newVal !== oldVal) {
                    if (newVal !== null && Object.prototype.toString.call(newVal) === "[object Date]")
                        $scope.filterFunction(newVal.getDate() + "/" + (newVal.getMonth() + 1) + "/" + newVal.getFullYear(), $scope.pageNumber, $scope.pageSize);
                    else
                        getResultsPage($scope.pageNumber, $scope.pageSize);
                }
            }, true);


            $scope.filterFunction = function (element, pageNumber, pageSize) {
                $http.get(authService.api() + $scope.seccion + '/GetPageFilter?filter=' + element + '&page=' + pageNumber + "&pageSize=" + pageSize,
                    {
                        headers: { "access_token": authService.getToken() }
                    })
                    .then(function (result) {
                        //$scope.Publicaciones = result.data.Result.DataSet;
                        setFlag(result.data.Result.DataSet);
                        $scope.totalItems = result.data.Result.TotalItems;
                    });
            };

            function getResultsPage(pageNumber, pageSize) {

                if ($scope.textSearch)
                    $scope.filterFunction($scope.textSearch, pageNumber, pageSize);
                else
                    $http.get(authService.api() + $scope.seccion + '/GetPage?page=' + pageNumber + "&pageSize=" + pageSize,
                        {
                            headers: { "access_token": authService.getToken() }
                        })
                        .then(function (result) {
                            setFlag(result.data.Result.DataSet);
                            $scope.totalItems = result.data.Result.TotalItems;
                        });
            }

            function setFlag(publicaciones) {
                if ($scope.seccion === '/gaceta') {
                    for (var i = 0; i < publicaciones.length; i++) {
                        if (publicaciones[i].logotipo.toLowerCase().indexOf("mp3") > -1)
                            publicaciones[i].format = 'mp3';
                        else
                            publicaciones[i].format = '';
                    }
                }
                for (var i = 0; i < publicaciones.length; i++) {
                    publicaciones[i].HTMLDOC = publicaciones[i].HTMLDOC ? $sce.trustAsHtml(publicaciones[i].HTMLDOC) : publicaciones[i].HTMLDOC;
                }
                
                $scope.Publicaciones = publicaciones;
            }
        }]);


yumKaaxControllers.controller("gacetaSemanalCtrl",
    ["$scope", "authService", "$timeout", "$routeParams", "listService", "dateHelper", "feedbackSrvc",
        function ($scope, authService, $timeout, $routeParams, listService, dateHelper, feedbackSrvc) {
            $scope.fechaSelected = new Date();
            $scope.seccion = '/gaceta';
            $scope.area = null;

            authService.custodian(function () {
                $scope.lists = listService.getLists();
                console.log($scope.lists.seccionesGaceta);
                $scope.area = $scope.lists.seccionesGaceta.filter(function(s) { return s.Id == $routeParams.gaceta; })[0];
                $timeout(function () { console.log($scope.area); });
            });

            if ($routeParams.gaceta == 1)
                $scope.seccion = '/gaceta';
            else if ($routeParams.gaceta == 2)
                $scope.seccion = '/gacetapatentesedictos';
            else if ($routeParams.gaceta == 3)
                $scope.seccion = '/GacetaMarcasAnotaciones';
            else if ($routeParams.gaceta == 4)
                $scope.seccion = '/GacetaGrl';

            // Disable weekend selection
            $scope.disabled = dateHelper.disabled;

            $scope.minDate = dateHelper.minDate();
            $scope.maxDate = dateHelper.maxDate();

            $scope.status = {
                opened: false
            };

            $scope.open = function ($event) {
                $scope.status.opened = true;
            };

            //calendar options
            $scope.dateOptions = dateHelper.dateOptions;

            //calendar date format
            $scope.formats = dateHelper.formats;
            $scope.format = $scope.formats[1];

        }]);


yumKaaxControllers.controller("misexpedientesCtrl",
    ["$scope", "$location", "authService", "$http", "classSrvc", "$timeout", "feedbackSrvc",
        function ($scope, $location, authService, $http, classSrvc, $timeout, feedbackSrvc) {
        $scope.userIdentity = authService.getUserInfo();
        $scope.grupos = [];
        $scope.misExpedientes = [];
        $scope.totalItems = 0;
        $scope.pageSize = 50;
        $scope.currentGrupo = null;
        $scope.addFormEnabled = false;

        function Init() {
            classSrvc.getGrupos()
                .success(function (result) {
                    $scope.grupos = result.Result.DataSet;
                })
                .error(function (error) {
                    $scope.grupos = [{ id: 0, Nombre: 'Mis Expedientes' }];
                });
            getExpedientes(null, 1, $scope.pageSize);
        }

        authService.custodian(function () {
            Init();
        });

        $scope.getExpedienteUrl = function (e) {
            if (!e)
                return "mexpediente";

            if (e.ModuloId == 1) {
                return "mexpediente/" + e.ExpedienteId;
            }
            else if (e.ModuloId == 2) {
                return "pexpediente/" + e.ExpedienteId;
            }
            else if (e.ModuloId == 3) {
                return "dexpediente/" + e.ExpedienteId;
            }
        };

        $scope.pageNumber = 1;
        $scope.pageChanged = function () {
            getExpedientes($scope.currentGrupo, $scope.pageNumber, $scope.pageSize);
        };

        $scope.getExpedientes = function (grupo) {
            getExpedientes(grupo, 1, $scope.pageSize);
        };

        function getExpedientes(grupo, pageNumber, pageSize) {
            $scope.currentGrupo = grupo;

            if ($scope.currentGrupo) {
                $http.get(authService.api() + '/Favoritos/GetFavoritosPageFilter?idGrupoFilter=' + $scope.currentGrupo + '&page=' + pageNumber + "&pageSize=" + pageSize,
                    {
                        headers: { "access_token": authService.getToken() }
                    })
                    .then(function (result) {
                        $scope.misExpedientes = result.data.Result.DataSet;
                        $scope.totalItems = result.data.Result.TotalItems;
                    });
            }
            else {
                $http.get(authService.api() + '/Favoritos/GetFavoritosPageFilter?idGrupoFilter=0&page=' + pageNumber + "&pageSize=" + pageSize,
                    {
                        headers: { "access_token": authService.getToken() }
                    })
                    .then(function (result) {

                        $scope.misExpedientes = result.data.Result.DataSet;
                        $scope.totalItems = result.data.Result.TotalItems;
                    });
            }
        }

        $scope.agregarGrupo = function () {
            $scope.addFormEnabled = true;
        };

        $scope.addGrupo = function (nombreDelGrupo) {
            if (!nombreDelGrupo)
                return;

            var grupo = { Id: 0, Nombre: nombreDelGrupo };

            $http.post(authService.api() + "/Favoritos/AddGrupo", grupo, { headers: { "access_token": authService.getToken() } })
                .then(
                    function (result) {
                        if (result.data.Succeeded) {
                            Init();
                            $scope.addFormEnabled = false;
                        }
                        else {
                            feedbackSrvc.handleError(error);
                        }
                    },
                    function (error) {
                        feedbackSrvc.handleError(error);
                    }   
                );
        };

    }]);


yumKaaxControllers.controller("UsuariosPublicosCtrlr",
    ["$scope", "$location", "$routeParams", "authService", "$http", "classSrvc", "$timeout", "feedbackSrvc",
        function ($scope, $location, $routeParams, authService, $http, classSrvc, $timeout, feedbackSrvc) {
            $scope.usuario = {};
            $scope.paises = [];

            function getPaises() {
                $http.get(authService.api() + '/Admin/Entities/Get?entity=paises',
                    {
                        headers: { "access_token": authService.getToken() }
                    })
                    .then(function (result) {
                        $scope.paises = result.data.Result;
                    });
            }

            function loadPerfil() { // the token 
                var service = "/Admin/UsuariosPublicos/";
                getPaises();
                $http.get(authService.api() + service + 'LoadMyPerfil',
                    {
                        headers: { "access_token": authService.getToken() }
                    })
                    .then(function (result) {
                        $scope.usuario = result.data.Result;
                        console.log(result.data.Result);
                    });

            }

            authService.custodian(function () {
                console.log($routeParams.spk);
                if ($routeParams.spk && $routeParams.Id) {
                    getUserWithSpk($routeParams.spk, $routeParams.Id);
                }
                else if ($routeParams.miClave && $routeParams.Id) {
                    loadPerfil();
                }
                else if (!$routeParams.spk && $routeParams.Id) {
                    loadPerfil();
                }
                else {
                    loadPerfil();
                }
            });
            

            $scope.isUnchanged = function (usuario) {
                return angular.equals(usuario, $scope.master);
            };

            $scope.update = function (usuario) {
                $http.post(authService.api() + "/Admin/UsuariosPublicos/save", { model: usuario }, { headers: { "access_token": authService.getToken() } })
                    .then(
                        function (result) {
                            if (result.data.Succeeded) {
                                feedbackSrvc.showAlertInfo("Usuario ha sido grabado correctamente...");
                            }
                            else {
                                feedbackSrvc.handleError(result.data);
                            }
                        },
                        function (error) {
                            feedbackSrvc.handleError(error);
                        }                        
                    );
            };

            $scope.setPW = function (usuario) {
                $http.post(authService.api() + "/Admin/UsuariosPublicos/ResetPW", { model: usuario }, { headers: { "access_token": $routeParams.spk ? $routeParams.spk : authService.getToken() } })
                    .then(
                        function (result) {
                            if (result.data.Succeeded) {
                                feedbackSrvc.showAlertInfo("Contrasena ha sido grabada correctamente...", $routeParams.spk ? '/login' : null);
                            }
                            else {
                                feedbackSrvc.handleError(result.data);
                            }
                        },
                        function (error) {
                            feedbackSrvc.handleError(error);
                        } 
                    );
            };

            function getUserWithSpk(spk, userId) {
                
                $http.post(authService.api() + '/Admin/UsuariosPublicos/GetWithSpk', { Id: userId, Spk: spk }, { headers: { "access_token": spk } })
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
            }
        }]);

yumKaaxControllers.controller("UsuarioCtrlr",
    ["$scope", "$location", "$routeParams", "authService", "$http", "classSrvc", "$timeout", "feedbackSrvc",
        function ($scope, $location, $routeParams, authService, $http, classSrvc, $timeout, feedbackSrvc) {
            $scope.usuario = {
                Id: -1,
                Cuenta: undefined,
                Pwd: undefined,
                Nombre: undefined,
                Empresa: undefined,
                Direccion: undefined,
                Ciudad: undefined,
                EstadoProvincia: undefined,
                Telefonos: "",
                PaisId: undefined
            };

            $scope.paises = [];

            function getPaises() {
                $http.get(authService.api() + '/Admin/Entities/Get?entity=paises',
                    {
                        headers: { "access_token": "" }
                    })
                    .then(function (result) {
                        $scope.paises = result.data.Result;
                    });
            }
            authService.custodian(function () {
                getPaises();          
            });

            function userAlertInfo(msg, path) {
                feedbackSrvc.showAlertInfo(msg);
                if (path)
                    $timeout(function () {$location.path(path);}, 1400);
            }

            $scope.emailMe = function (usuario) {             
                $http.post(authService.api() + "/Admin/UsuariosPublicos/cambiarClave", { model: usuario }, { headers: { "access_token": "", "locPath": window.location } })
                    .then(
                        function (result) {
                            if (result.data.Succeeded) {
                                userAlertInfo("Un email ha sido enviado a su cuenta de acceso al sistema.", '/login');
                            }
                            else {
                                feedbackSrvc.handleError(result.data);
                            }
                        },
                        function (error) {
                            feedbackSrvc.handleError(error);
                        }
                    );
            };

            $scope.save = function (usuario) {
                $http.post(authService.api() + "/Admin/UsuariosPublicos/save", { model: usuario }, { headers: { "access_token": "" } })
                .then(
                    function (result) {
                        if (result.data.Succeeded) {
                            userAlertInfo("Usuario ha sido grabado correctamente...", '/login');
                        }
                        else {
                            feedbackSrvc.handleError(result.data);
                        }
                    },
                    function (error) {
                        feedbackSrvc.handleError(error);
                    } 
                );
            };        
        }]);

yumKaaxControllers.controller("genericCtrl",
    ["$scope", "$location", "authService", "$http", "classSrvc", "feedbackSrvc",
        function ($scope, $location, authService, $http, classSrvc, feedbackSrvc) {

    }]);


yumKaaxControllers.controller("avisosCtrlr",
    ["$scope", "authService", "$http", "listService", "feedbackSrvc",
        function ($scope, authService, $http, listService, feedbackSrvc) {

            $scope.pageNumber = 1;
            $scope.totalItems = 0;
            $scope.pageSize = 100;

        }]);


yumKaaxControllers.controller("gacetaLeyCtrl",
    ["$scope",
        function ($scope) {
        }]);