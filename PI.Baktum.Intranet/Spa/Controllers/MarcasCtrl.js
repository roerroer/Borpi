
///---------------
/// Marcas
///---------------
yumKaaxControllers.controller("MarcasCtrl",
    ["$scope", "$http", "$q", "$routeParams", "$timeout", "$route", "$location", "authService", "listService", "dateHelper", "bouncer", "feedbackSrvc",
        function ($scope, $http, $q, $routeParams, $timeout, $route, $location, authService, listService, dateHelper, bouncer, feedbackSrvc) {
    $scope.config = { _expediente: "./Spa/Views/eMarcas/MarcasExpediente.html", _modulo: "Marcas/Signos distintivos" };
    
    $scope.$route = $route;
    $scope.model = {};
    $scope.opciones = listService.getMarcasmnu();
    $scope.Estatuses = [];
    $scope.dateHelper = dateHelper;

    function getEstatus() {
        $http.get(authService.api() + '/Admin/Estatus/GetPage?page=1&pageSize=200&moduloId=1')
        .then(function (result) {
            $scope.Estatuses = result.data.Result.DataSet;
        });
    }

    function fixModelDates() {
        $scope.model.Expediente.FechaDeSolicitud = dateHelper.parseStrDate($scope.model.Expediente.FechaDeSolicitud);
        $scope.model.Expediente.FechaDeEstatus = dateHelper.parseStrDate($scope.model.Expediente.FechaDeEstatus);
        $scope.model.Marca.FechaDeTraslado = dateHelper.parseStrDate($scope.model.Marca.FechaDeTraslado);
        //saveOrgModel();
    }

    function getDocto(action, queryString) {
        var service = '/Marca/';

        $http.get(authService.api() + service + action + queryString)
        .then(function (result) {
            $scope.model = result.data.Result.documento;
            console.log(result.data.Result.documento);
        });
    }

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

    function getEntityById(Id) {
        getDocto('ExpedienteId','?id=' + Id);
    }

    $scope.getExpediente = function getEntityByNumero(expediente) {
        if (!$routeParams.numero) {
            $location.path("/Marcas/" + expediente);
        }
        else {
            getDocto('Expediente', '?numero=' + expediente);
            $location.path("/Marcas/" + expediente, false);
        }

        
    };

    $scope.getNew = function () {
        $scope.model = {
            Expediente: { Id: -1, TipoDeRegistroId: 0, Numero: '', FechaDeSolicitud: undefined, DoctosAdjuntos: '0,0,0,0,0,0,0,0' },
            Marca: { Denominacion: '' }, Prioridades: []
        };
    };


    function refreshExpediente() {
        $scope.getExpediente($routeParams.numero)
        $scope._resolucion = { FechaResolucion: new Date() };
        //showLeftBox();
        $scope.res = './Spa/Views/empty.html';
    }

    function createResolucion(service, resolucion) {
        $http.post(authService.api() + service, { tramite: resolucion })
            .then(
                function (result) {
                    listService.printDoc(result);
                    refreshExpediente();
                },
                function (error) {
                    feedbackSrvc.handleError(error);
                }
            );
    }

    $scope.myfunction = function (res) {//Test
        var jsondoc = JSON.stringify(res);

        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        };

        createResolucion("/Marca/Requerimiento_De_Examen_De_Forma", resolucion);
    };


    $scope._MAR400SaveNPrint = function (res) {
        var jsondoc = JSON.stringify(res);

        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            AmparaProductos: res.AmparaProductos,
            AmparaServicios: res.AmparaServicios,
            TipoDeMarca: res.TipoDeMarca,
            MarcaAColor: res.MarcaAColor,
            AdminUnico: res.AdminUnico,
            RepresentanteLegal: res.RepresentanteLegal,
            MandatarioEspecial: res.MandatarioEspecial,
            Apoderado: res.Apoderado,
            Gerente: res.Gerente,
            Otro: res.Otro,
            EnCalidadDe: res.EnCalidadDe,
            Mandatario: res.Mandatario,
            Lugar: res.Lugar,
            DireccionEntidad: res.DireccionEntidad,
            Observaciones: res.Observaciones,
            JSONDOC: jsondoc
        };

        createResolucion("/Marca/Edicto", resolucion);
    };
    

    $scope._mar810SaveNPrint = function (res) {
        var jsondoc = JSON.stringify(res);

        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            EstatusId: res.EstatusId.Id,
            UpdatesEstatus: res.UpdatesEstatus,
            Titulo: res.Titulo,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        };

        //DA130To131 ->ConLugar
        $http.post(authService.api() + "/Marca/ResolucionCustomizada", { tramite: resolucion })
            .then(
                function (result) {
                    listService.printDoc(result);
                    refreshExpediente();
                },
                function (error) {
                    feedbackSrvc.handleError(error);
                }
            );
    }

    // cambiar estatus
    $scope._MAR900SaveNPrint = function (res) {
        var jsondoc = JSON.stringify(res);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            EstatusId: res.EstatusId.Id,
            JSONDOC: jsondoc
        }

        //DA130To131 ->ConLugar
        $http.post(authService.api() + "/Marca/CambiarEstatus", { tramite: resolucion })
            .then(
                function (result) {
                    listService.printDoc(result);
                    refreshExpediente();
                },
                function (error) {
                    feedbackSrvc.handleError(error);
                }
            );
    };
    

    //calendar flag controllers
    $scope.fds = {};
    $scope.ppf = {};
    $scope.saf = {};
    $scope.fresolucion = {};
    $scope.fv = {};
    
    $scope.open = function ($event, wtf) {
        $event.preventDefault();
        $event.stopPropagation();
        if (wtf == 'fds')
            $scope.fds.open = true;
        else if (wtf == 'ppf')
            $scope.ppf.open = true;
        else if (wtf == 'saf')
            $scope.saf.open = true;
        else if (wtf == 'fresolucion')
            $scope.fresolucion.open = true;
        else if (wtf == 'fv')
            $scope.fv.open = true;
    };

            $scope.getPaisCode = function (paisId) {
                var pais = $scope.paises.firstOrDefault(function (p, i) { return p[i].Id == paisId });
                return pais.Codigo;
            };

    $scope.titular = {};
    $scope.selectTitular = function (tm) {
        $scope.titular = tm;
    };

    $scope.titular = { id: 0, DireccionParaNotificacion: '', PaisId: 0, Nombre: '' };
    $scope.onSelectTitular = function ($item, $model, $label) {
        $scope.titular.Id = $item.Id;
        $scope.titular.PaisId = $item.PaisId;
        $scope.titular.DireccionParaNotificacion = $item.Direccion;
        $scope.titular.DireccionParaUbicacion = $item.Direccion;
    };

    $scope.searchTitulares = function (textToSearch) {
        var service = '/Marca/SearchTitular';
        return $http.get(authService.api() + service + '?textToSearch=' + textToSearch)
            .then(
                function (result) { return result.data.Result.DataSet; },
                function (Error) { return []; }
            );
    };

    $scope.isMnuEnabled = function (opcion) {
        var _disabled = "";
        try {
            if (opcion._estatus.length == 0) {
                _disabled = "";
            }
            else {
                //_disabled = opcion._estatus.indexOf($scope.model.Expediente.EstatusId) == -1 ? "disabled" : "";
                _disabled = bouncer.isMenuDisabledForMarcas(opcion, $scope.model);
            }
        }
        catch (err) {
            _disabled = "disabled";
        }
        return _disabled;
    };

    $scope.Resolucion = function (opcion) {
        //console.log('opcion:' + opcion)
        var view = opcion.view;

        //if (opcion._estatus.length != 0 && (view == '' || opcion._estatus.indexOf($scope.model.Expediente.EstatusId) == -1)) {
        if (bouncer.isUnauthorizedActionForMarcas(opcion, view, $scope.model)) {
            $scope.res = './Spa/Views/empty.html';
            $scope.$apply();
        }
        else {
            $scope.res = './Spa/Views/eMarcas/' + view;
            $scope.evento = opcion.opcion;
        }
        $scope._resolucion = {resId:""};
    };

            function _ctrlInit() {
                $scope.paises = listService.getLists().paises;
                console.log($scope.paises);
                $scope.getNew();
                getEstatus();
                if ($routeParams.Id) {
                    getEntityById($routeParams.Id);
                }
                else if ($routeParams.numero) {
                    $scope.getExpediente($routeParams.numero);
                }
            }


    authService.custodian(function () {
        _ctrlInit();
    });
    

    $scope.rollbackResolution = function () {
        $location.path("/Marcas/" + $routeParams.numero);
        $scope.Resolucion({_estatus:[0]});//reset resolution
        showLeftBox();
    };

    /*
     * Resoluciones
     */
    $scope.Gestor_Oficioso = function (res) {
        var jsondoc = JSON.stringify(res);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        };
        createResolucion("/Marca/Gestor_Oficioso", resolucion);
    };
    
    $scope.Requerimientos = function (res) {
        var jsondoc = JSON.stringify(res);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        };
        createResolucion("/Marca/Requerimientos", resolucion);
    };
    
    $scope.Objeciones_Forma = function (res) {
        var jsondoc = JSON.stringify(res);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        };
        createResolucion("/Marca/Objeciones_Forma", resolucion);
    };
    
    $scope.Objeciones_Fondo = function (res) {
        var jsondoc = JSON.stringify(res);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        };
        createResolucion("/Marca/Objeciones_Fondo", resolucion);
    };
    
    $scope.Edictos = function (res) {
        var jsondoc = JSON.stringify(res);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        };
        createResolucion("/Marca/Edictos", resolucion);
    };
    
    $scope.Orden_de_Pago = function (res) {
        var jsondoc = JSON.stringify(res);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        };
        createResolucion("/Marca/Orden_de_Pago", resolucion);
    };
    
    $scope.Titulo = function (res) {
        var jsondoc = JSON.stringify(res);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        };
        createResolucion("/Marca/Titulo", resolucion);
    };
    
    $scope.Reposicion_de_edicto = function (res) {
        var jsondoc = JSON.stringify(res);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        };
        createResolucion("/Marca/Reposicion_de_edicto", resolucion);
    };
    
    $scope.Enmienda = function (res) {
        var jsondoc = JSON.stringify(res);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        };
        createResolucion("/Marca/Enmienda", resolucion);
    };
    
    $scope.Cancelacion = function (res) {
        var jsondoc = JSON.stringify(res);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        };
        createResolucion("/Marca/Cancelacion", resolucion);
    };
    
    $scope.Traspaso = function (res) {
        var jsondoc = JSON.stringify(res);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        };
        createResolucion("/Marca/Traspaso", resolucion);
    };
    
    $scope.Division_Registro = function (res) {
        var jsondoc = JSON.stringify(res);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        };
        createResolucion("/Marca/Division_Registro", resolucion);
    };
    
    $scope.Abandono = function (res) {
        var jsondoc = JSON.stringify(res);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        };
        createResolucion("/Marca/Abandono", resolucion);
    };
    
    $scope.Desistimiento = function (res) {
        var jsondoc = JSON.stringify(res);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        };
        createResolucion("/Marca/Desistimiento", resolucion);
    };
    
    $scope.Rechazo_por_objecion = function (res) {
        var jsondoc = JSON.stringify(res);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        };
        createResolucion("/Marca/Rechazo_por_objecion", resolucion);
    };
    
    $scope.Revocatoria_de_Oficio = function (res) {
        var jsondoc = JSON.stringify(res);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        };
        createResolucion("/Marca/Revocatoria_de_Oficio", resolucion);
    };
    
    $scope.Resolucion_Customizada = function (res) {
        var jsondoc = JSON.stringify(res);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        };
        createResolucion("/Marca/Resolucion_Customizada", resolucion);
    };
    
    $scope.Errores_Materiales = function (res) {
        var jsondoc = JSON.stringify(res);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        };
        createResolucion("/Marca/Errores_Materiales", resolucion);
    };
    
    $scope.Cambiar_Estatus = function (res) {
        var jsondoc = JSON.stringify(res);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        };
        createResolucion("/Marca/Cambiar_Estatus", resolucion);
    };
    
    $scope.Notificaciones = function (res) {
        var jsondoc = JSON.stringify(res);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        };
        createResolucion("/Marca/Notificaciones", resolucion);
    };
}]);