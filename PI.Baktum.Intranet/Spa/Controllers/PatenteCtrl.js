﻿///---------------
/// Patente
///---------------
yumKaaxControllers.controller("PatenteCtrl",
    ["$scope", "$http", "$q", "$routeParams", "$timeout", "$route", "listService", "authService", "$location", "dateHelper", "bouncer", "feedbackSrvc",
        function ($scope, $http, $q, $routeParams, $timeout, $route, listService, authService, $location, dateHelper, bouncer, feedbackSrvc) {
            $scope.config = { _expediente: "./Spa/Views/ePatentes/PatentesExpediente.html", _modulo: "Patentes" };
            $scope.$route = $route;
            $scope.opciones = listService.getPatentesmnu();
            $scope.tipoDePatentes = [];
            $scope.paises = [];
            $scope.model = { Expediente: { Id: -1, TipoDeRegistroId: 0, Numero: '', FechaDeSolicitud: undefined }, Patente: { Descripcion: '' }, Titulares: [] };
            $scope.titular = { Id: -1, Direccion: '', PaisId: -1 };
            $scope.Estatuses = [];
            $scope.classPat = [];
            $scope.dateHelper = dateHelper;

            /*SYSTEM*/
            $scope.res = "";
            $scope.evento = "";
            $scope._resolucion = { FechaResolucion: new Date() };
            $scope.Estatuses = [];
            $scope.user = authService.getUserInfo();
            /*SYSTEM*/

            function getListas() {
                var lists = listService.getLists();
                $scope.tipoDePatentes = lists.tipoDePatentes;
                $scope.paises = lists.paises;
                $scope.classPat = listService.getClassificacionPat();
            }

            function getEstatus() {
                $http.get(authService.api() + '/Admin/Estatus/GetPage?page=1&pageSize=200&moduloId=2')
                    .then(function (result) {
                        $scope.Estatuses = result.data.Result.DataSet;
                    });
            }

            function _ctrlInit() {
                getListas();
                getEstatus();
            }

            function fixModelDates() {
                //console.log($scope.model);
                $scope.model.Expediente.FechaDeSolicitud = dateHelper.parseStrDate($scope.model.Expediente.FechaDeSolicitud);
                $scope.model.Expediente.FechaDeEstatus = dateHelper.parseStrDate($scope.model.Expediente.FechaDeEstatus);
                $scope.model.Patente.Fecha_Pct = dateHelper.parseStrDate($scope.model.Patente.Fecha_Pct);
                for (var i = 0; i < $scope.model.Prioridades.length; i++) {
                    $scope.model.Prioridades[i].Prioridad.Fecha = dateHelper.parseStrDate($scope.model.Prioridades[i].Prioridad.Fecha);
                }
                for (var i = 0; i < $scope.model.Cronologia.length; i++) {
                    $scope.model.Cronologia[i].Fecha = dateHelper.parseStrDate($scope.model.Cronologia[i].Fecha);
                }
                for (var i = 0; i < $scope.model.Anualidades.length; i++) {
                    $scope.model.Anualidades[i].FechaVencimiento = dateHelper.parseStrDate($scope.model.Anualidades[i].FechaVencimiento);
                    $scope.model.Anualidades[i].FechaRecibo = dateHelper.parseStrDate($scope.model.Anualidades[i].FechaRecibo);
                    $scope.model.Anualidades[i].FechaAnualidad = dateHelper.parseStrDate($scope.model.Anualidades[i].FechaAnualidad);
                }

            }

            function getDocto(action, queryString) {
                var service = '/Patente/';

                $http.get(authService.api() + service + action + queryString)
                    .then(
                        function (result) {
                            $scope.model = result.data.Result.documento;
                            console.log(JSON.stringify($scope.model));
                            fixModelDates();
                            console.log('Fetched by expediente!!');
                        },
                        function (error) {
                            feedbackSrvc.handleError(error, 'Expediente no encontrado - ' + ($routeParams.numero ? $routeParams.numero : ""));
                            var docType = $scope.model.Expediente.TipoDeRegistroId;
                            $scope.model = { Expediente: { TipoDeRegistroId: docType } };
                        }
                    );
            }

            function refreshExpediente() {
                $scope.getExpediente($scope.model.Expediente.TipoDeRegistroId, $scope.model.Expediente.Numero);
                $scope._resolucion = { FechaResolucion: new Date() };
                //showLeftBox();        
                $scope.res = './Spa/Views/empty.html';
            }


            function getEntityById(Id) {
                getDocto('ExpedienteId', '?Id=' + Id);
            }

            $scope.getExpediente2 = function getEntityByNumero(tipoDeRegistro, expediente) {
                if (!$routeParams.numero) {
                    $location.path("/Patentes/" + expediente + "/" + tipoDeRegistro);
                }
                else {
                    $scope.model.Expediente.TipoDeRegistroId = 'number:' + tipoDeRegistro;
                    $scope.model.Expediente.Numero = expediente;
                    getDocto('Expediente', '?numero=' + expediente + '&TipoDeRegistroId=' + tipoDeRegistro);
                    $location.path("/Patentes/" + expediente + "/" + tipoDeRegistro, false);
                }
            };

            $scope.getNew = function () {
                $scope.model = { Expediente: { Id: -1, TipoDeRegistroId: 0, Numero: '', FechaDeSolicitud: undefined }, Patente: { Descripcion: '' }, Titulares: [] };
            };

            $scope.titular = { id: 0, Direccion: '', PaisId: 0, Nombre: '' };
            $scope.onSelectTitular = function ($item, $model, $label) {
                $scope.titular.Id = $item.Id;
                $scope.titular.PaisId = $item.PaisId;
            };

            $scope.searchTitulares = function (textToSearch) {
                var service = '/PatTitular/Search';
                return $http.get(authService.api() + service + '?textToSearch=' + textToSearch)
                    .then(
                        function (result) { return result.data.Result.DataSet; },
                        function (error) { return []; }
                    );
            };

            $scope.onSelectAgente = function ($item, $model, $label) {
                $scope.model.Patente.AgenteId = $item.Id;
                $scope.model.Agente = $item.Nombre;
            };

            $scope.searchAgentes = function (textToSearch) {
                console.log('search agentes');
                var service = '/Admin/Agentes/Search';
                console.log(authService.api() + service + '?textToSearch=' + textToSearch);
                return $http.get(authService.api() + service + '?textToSearch=' + textToSearch)
                    .then(
                        function (result) { return result.data.Result.DataSet; },
                        function (error) { console.log('error search agentes'); return []; }
                    );
            };


            $scope.saveSolicitud = function (frmSol) {
                var service = '/Patente/';
                if (!frmSol.$valid)
                    return;

                $http.post(authService.api() + service + "SaveSolicitud", { model: $scope.model })
                    .then(
                        function (result) {
                            $scope.model = result.data.Result.documento;
                            //fixModelDates();
                            $scope.getExpediente2($routeParams.tipo, $routeParams.numero);
                            feedbackSrvc.showAlertInfo('Expediente actualizado - ' + $routeParams.tipo + '-' + $routeParams.numero);
                        },
                        function (error) {
                            feedbackSrvc.handleError(error);
                        }
                    );
            };

            $scope.saveTitulares = function (titular) {
                var service = '/Patente/';
                titular.id = undefined; //remove this weird field!
                var auditoria = JSON.stringify(titular);
                var extra = JSON.stringify({ ExpedienteId: $scope.model.Expediente.Id, Direccion: titular.Direccion });

                var genEntity = { Generic: titular, Auditoria: auditoria, jsExtra: extra };

                $http.post(authService.api() + service + "SaveTitulares", { model: genEntity })
                    .then(
                        function (result) {
                            console.log("response: " + JSON.stringify(result.data));
                            if (!$scope.model.Titulares)
                                $scope.model.Titulares = [];
                            $scope.model.Titulares.push(result.data.Result[0]);
                        },
                        function (error) {
                            feedbackSrvc.handleError(error);
                        }
                    );
            };

            $scope.deleteTitular = function (titular) {
                var service = '/Patente/';
                var auditoria = JSON.stringify(titular);
                $http.post(authService.api() + service + "DeleteTitular" + '?titularId=' + titular.Titular.Id + '&expedienteId=' + $scope.model.Expediente.Id, { historial: auditoria })
                    .then(
                        function (result) {
                            $scope.model.Titulares.remove(titular);
                        },
                        function (error) {
                            feedbackSrvc.handleError(error);
                        }
                    );
            };

            /*
             * Inventores
             */
            $scope.saveInventor = function (inventor) {

                console.log(JSON.stringify(inventor));
                var service = '/Inventores/';
                inventor.id = undefined; //remove this weird field!
                inventor.ExpedienteId = $scope.model.Expediente.Id;
                var auditoria = JSON.stringify(inventor);
                var genEntity = { Generic: inventor, Auditoria: auditoria };

                $http.post(authService.api() + service + "SaveInventor", { model: genEntity })
                    .then(
                        function (result) {
                            console.log("response: " + JSON.stringify(result.data));
                            if (!$scope.model.Inventores)
                                $scope.model.Inventores = [];
                            $scope.model.Inventores.push(result.data.Result);
                        },
                        function (error) {
                            console.log('Error:');
                            console.log(error);
                            feedbackSrvc.handleError(error);
                        }
                    );
            };

            $scope.deleteInventor = function (inventor) {
                var service = '/Inventores/';
                var auditoria = JSON.stringify(inventor);
                $http.post(authService.api() + service + "DeleteInventor" + '?inventorId=' + inventor.Id + '&expedienteId=' + $scope.model.Expediente.Id, { historial: auditoria })
                    .then(
                        function (result) {
                            //removeItemByhashKey($scope.model.Inventores, inventor.$$hashKey);
                            $scope.model.Inventores.remove(inventor);
                        },
                        function (error) {
                            feedbackSrvc.handleError(error);
                        }
                    );
            };

            function removeItemByhashKey(ngArray, key) {
                if (!ngArray)
                    return;

                for (var i = 0; i < ngArray.length; i++) {
                    if (ngArray[i].$$hashKey == key) {
                        ngArray.splice(i, 1);
                    }
                }
            };

            /*
             * Agente
             */
            $scope.saveAgente = function (agenteId) {
                var service = '/Patente/';
                var extra = JSON.stringify({ ExpedienteId: $scope.model.Expediente.Id, AgenteId: agenteId });
                var genEntity = { jsExtra: extra };

                $http.post(authService.api() + service + "SaveAgente", { model: genEntity })
                    .then(
                        function (result) {
                            feedbackSrvc.showAlertInfo('Agente ha sido grabado correctamente...');
                        },
                        function (error) {
                            feedbackSrvc.handleError(error);
                        }
                    );
            };

            /*
             * Save Resumen y Classificacion
             */
            $scope.saveResumenClasificacion = function (agenteId) {
                var service = '/Patente/';
                var extra = JSON.stringify({ ExpedienteId: $scope.model.Expediente.Id, Ipc1: $scope.model.Ipc1, Ipc2: $scope.model.Ipc2, Ipc3: $scope.model.Ipc3, Ipc4: $scope.model.Ipc4, Resumen: $scope.model.Patente.Resumen });

                var genEntity = { jsExtra: extra };

                $http.post(authService.api() + service + "saveResumenClasificacion", { model: genEntity })
                    .then(
                        function (result) {
                            feedbackSrvc.showAlertInfo('Resumen-Clasificacion ha sido grabado correctamente...');
                        },
                        function (error) {
                            feedbackSrvc.handleError(error);
                        }
                    );
            };

            /*
             * Save referencia
             */
            $scope.saveReferencia = function (ref) {

                console.log(JSON.stringify(ref));
                var service = '/Patente/';
                ref.ExpedienteId = $scope.model.Expediente.Id;
                var auditoria = JSON.stringify(ref);
                var genEntity = { Generic: ref, Auditoria: auditoria };

                $http.post(authService.api() + service + "SaveReferencia", { model: genEntity })
                    .then(
                        function (result) {
                            console.log("response: " + JSON.stringify(result.data));
                            if (!$scope.model.Prioridades)
                                $scope.model.Prioridades = [];
                            $scope.model.Prioridades.push(result.data.Result);
                        },
                        function (error) {
                            console.log('Error:');
                            console.log(error);
                            feedbackSrvc.handleError(error);
                        }
                    );
            };

            $scope.deleteReferencia = function (ref) {
                var service = '/Patente/';
                var auditoria = JSON.stringify(ref.Prioridad);
                var genEntity = { Generic: ref.Prioridad, Auditoria: auditoria };
                $http.post(authService.api() + service + "DeleteReferencia", { model: genEntity })
                    .then(
                        function (result) {
                            //removeItemByhashKey($scope.model.Inventores, inventor.$$hashKey);
                            $scope.model.Prioridades.remove(ref);
                        },
                        function (error) {
                            feedbackSrvc.handleError(error);
                        }
                    );
            };

            /*
             * Save Anualidad
             */
            $scope.saveAnualidad = function (anual) {
                alert('test');
                console.log(JSON.stringify(anual));
                var service = '/Patente/';
                anual.ExpedienteId = $scope.model.Expediente.Id;
                var auditoria = JSON.stringify(anual);
                var genEntity = { Generic: anual, Auditoria: auditoria };

                $http.post(authService.api() + service + "SaveAnualidad", { model: genEntity })
                    .then(
                        function (result) {
                            console.log("response: " + JSON.stringify(result.data));
                            if (!$scope.model.Anualidades)
                                $scope.model.Anualidades = [];
                            $scope.model.Anualidades.push(result.data.Result);
                        },
                        function (error) {
                            console.log('Error:');
                            console.log(error);
                            feedbackSrvc.handleError(error);
                        }
                    );
            };

            $scope.deleteAnualidad = function (anual) {
                var service = '/Patente/';
                var auditoria = JSON.stringify(anual);
                var genEntity = { Generic: anual, Auditoria: auditoria };
                $http.post(authService.api() + service + "DeleteAnualidad", { model: genEntity })
                    .then(
                        function (result) {
                            //removeItemByhashKey($scope.model.Inventores, inventor.$$hashKey);
                            $scope.model.Anualidades.remove(anual);
                        },
                        function (error) {
                            feedbackSrvc.handleError(error);
                        }
                    );
            };


            $scope._pat023SaveNPrint = function (res) {
                var jsondoc = JSON.stringify(res);

                var resolucion = {
                    ExpedienteId: $scope.model.Expediente.Id,
                    Fecha: res.FechaResolucion,
                    EstatusId: res.EstatusId.Id,
                    Titulo: res.Titulo,
                    JSONDOC: jsondoc,
                    HTMLDOC: res.htmlResol
                };

                createResolucion("/Patente/ResolucionCustomizada", resolucion);
            };

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
            };

            $scope.Requerimiento_De_Examen_De_Forma = function (res) {
                var jsondoc = JSON.stringify(res);

                var resolucion = {
                    ExpedienteId: $scope.model.Expediente.Id,
                    Fecha: res.FechaResolucion,
                    JSONDOC: jsondoc,
                    HTMLDOC: res.htmlResol
                };

                createResolucion("/Patente/Requerimiento_De_Examen_De_Forma", resolucion);
            };


            $scope.Gestor_Oficioso = function (res) {
                var jsondoc = JSON.stringify(res);

                var resolucion = {
                    ExpedienteId: $scope.model.Expediente.Id,
                    Fecha: res.FechaResolucion,
                    JSONDOC: jsondoc,
                    HTMLDOC: res.htmlResol
                };

                createResolucion("/Patente/Gestor_Oficioso", resolucion);
            };

            $scope.Admision_Tramite = function (res) {
                var jsondoc = JSON.stringify(res);

                var resolucion = {
                    ExpedienteId: $scope.model.Expediente.Id,
                    Fecha: res.FechaResolucion,
                    JSONDOC: jsondoc,
                    HTMLDOC: res.htmlResol
                };

                createResolucion("/Patente/Admision_Tramite", resolucion);

            };

            $scope.Edicto = function (res) {
                var jsondoc = JSON.stringify(res);

                var resolucion = {
                    ExpedienteId: $scope.model.Expediente.Id,
                    Fecha: res.FechaResolucion,
                    JSONDOC: jsondoc,
                    HTMLDOC: res.htmlResol
                };

                createResolucion("/Patente/Edicto", resolucion);

            };

            $scope.Publicaciones = function (res) {
                var jsondoc = JSON.stringify(res);

                var resolucion = {
                    ExpedienteId: $scope.model.Expediente.Id,
                    Fecha: res.FechaResolucion,
                    JSONDOC: jsondoc,
                    HTMLDOC: res.htmlResol
                };

                createResolucion("/Patente/Publicaciones", resolucion);

            };

            $scope.Admite_Observaciones = function (res) {
                var jsondoc = JSON.stringify(res);

                var resolucion = {
                    ExpedienteId: $scope.model.Expediente.Id,
                    Fecha: res.FechaResolucion,
                    JSONDOC: jsondoc,
                    HTMLDOC: res.htmlResol
                };

                createResolucion("/Patente/Admite_Observaciones", resolucion);

            };

            $scope.Orden_De_Pago = function (res) {
                var jsondoc = JSON.stringify(res);

                var resolucion = {
                    ExpedienteId: $scope.model.Expediente.Id,
                    Fecha: res.FechaResolucion,
                    JSONDOC: jsondoc,
                    HTMLDOC: res.htmlResol
                };

                createResolucion("/Patente/Orden_De_Pago", resolucion);

            };

            $scope.Pago_Examen = function (res) {
                var jsondoc = JSON.stringify(res);

                var resolucion = {
                    ExpedienteId: $scope.model.Expediente.Id,
                    Fecha: res.FechaResolucion,
                    JSONDOC: jsondoc,
                    HTMLDOC: res.htmlResol
                };

                createResolucion("/Patente/Pago_Examen", resolucion);

            };

            $scope.Requerimiento_Examen_De_Fondo_A = function (res) {
                var jsondoc = JSON.stringify(res);

                var resolucion = {
                    ExpedienteId: $scope.model.Expediente.Id,
                    Fecha: res.FechaResolucion,
                    JSONDOC: jsondoc,
                    HTMLDOC: res.htmlResol
                };

                createResolucion("/Patente/Requerimiento_Examen_De_Fondo_A", resolucion);

            };

            $scope.Requerimiento_Examen_de_Fondo_B = function (res) {
                var jsondoc = JSON.stringify(res);

                var resolucion = {
                    ExpedienteId: $scope.model.Expediente.Id,
                    Fecha: res.FechaResolucion,
                    JSONDOC: jsondoc,
                    HTMLDOC: res.htmlResol
                };

                createResolucion("/Patente/Requerimiento_Examen_de_Fondo_B", resolucion);

            };

            $scope.Reporte_De_Examen_De_Fondo = function (res) {
                var jsondoc = JSON.stringify(res);

                var resolucion = {
                    ExpedienteId: $scope.model.Expediente.Id,
                    Fecha: res.FechaResolucion,
                    JSONDOC: jsondoc,
                    HTMLDOC: res.htmlResol
                };

                createResolucion("/Patente/Reporte_De_Examen_De_Fondo", resolucion);

            };

            $scope.Informe_de_Busqueda = function (res) {
                var jsondoc = JSON.stringify(res);

                var resolucion = {
                    ExpedienteId: $scope.model.Expediente.Id,
                    Fecha: res.FechaResolucion,
                    JSONDOC: jsondoc,
                    HTMLDOC: res.htmlResol
                };

                createResolucion("/Patente/Informe_de_Busqueda", resolucion);

            };

            $scope.Razon_De_Abandono = function (res) {
                var jsondoc = JSON.stringify(res);

                var resolucion = {
                    ExpedienteId: $scope.model.Expediente.Id,
                    Fecha: res.FechaResolucion,
                    JSONDOC: jsondoc,
                    HTMLDOC: res.htmlResol
                };

                createResolucion("/Patente/Razon_De_Abandono", resolucion);

            };

            $scope.Desistimiento_Solicitud = function (res) {
                var jsondoc = JSON.stringify(res);

                var resolucion = {
                    ExpedienteId: $scope.model.Expediente.Id,
                    Fecha: res.FechaResolucion,
                    JSONDOC: jsondoc,
                    HTMLDOC: res.htmlResol
                };

                createResolucion("/Patente/Desistimiento_Solicitud", resolucion);

            };

            $scope.Titulo = function (res) {
                var jsondoc = JSON.stringify(res);

                var resolucion = {
                    ExpedienteId: $scope.model.Expediente.Id,
                    Fecha: res.FechaResolucion,
                    JSONDOC: jsondoc,
                    HTMLDOC: res.htmlResol
                };

                createResolucion("/Patente/Titulo", resolucion);

            };

            $scope.Notificacion = function (res) {
                var jsondoc = JSON.stringify(res);

                var resolucion = {
                    ExpedienteId: $scope.model.Expediente.Id,
                    Fecha: res.FechaResolucion,
                    JSONDOC: jsondoc,
                    HTMLDOC: res.htmlResol
                };

                createResolucion("/Patente/Notificacion", resolucion);

            };

            $scope.Traspaso = function (res) {
                var jsondoc = JSON.stringify(res);

                var resolucion = {
                    ExpedienteId: $scope.model.Expediente.Id,
                    Fecha: res.FechaResolucion,
                    JSONDOC: jsondoc,
                    HTMLDOC: res.htmlResol
                };

                createResolucion("/Patente/Traspaso", resolucion);

            };

            $scope.Cambio_De_Nombre = function (res) {
                var jsondoc = JSON.stringify(res);

                var resolucion = {
                    ExpedienteId: $scope.model.Expediente.Id,
                    Fecha: res.FechaResolucion,
                    JSONDOC: jsondoc,
                    HTMLDOC: res.htmlResol
                };

                createResolucion("/Patente/Cambio_De_Nombre", resolucion);

            };

            $scope.Titulo_Renovacion = function (res) {
                var jsondoc = JSON.stringify(res);

                var resolucion = {
                    ExpedienteId: $scope.model.Expediente.Id,
                    Fecha: res.FechaResolucion,
                    JSONDOC: jsondoc,
                    HTMLDOC: res.htmlResol
                };

                createResolucion("/Patente/Titulo_Renovacion", resolucion);

            };

            $scope.Reposicion_Titulo_Patente = function (res) {
                var jsondoc = JSON.stringify(res);

                var resolucion = {
                    ExpedienteId: $scope.model.Expediente.Id,
                    Fecha: res.FechaResolucion,
                    JSONDOC: jsondoc,
                    HTMLDOC: res.htmlResol
                };

                createResolucion("/Patente/Reposicion_Titulo_Patente", resolucion);

            };

            $scope.Certificaciones = function (res) {
                var jsondoc = JSON.stringify(res);

                var resolucion = {
                    ExpedienteId: $scope.model.Expediente.Id,
                    Fecha: res.FechaResolucion,
                    JSONDOC: jsondoc,
                    HTMLDOC: res.htmlResol
                };

                createResolucion("/Patente/Certificaciones", resolucion);

            };


            /*********************
            SYSTEM
            **********************/
            //calendar flag controllers
            $scope.fds = {};
            $scope.ppf = {};
            $scope.saf = {};
            $scope.fresolucion = {};
            $scope.fmem = {};
            $scope.fnoti = {};
            $scope.fecha2 = {};
            $scope.fecha3 = {};
            $scope.fref = {};
            $scope.fa = {};
            $scope.fv = {};
            $scope.fr = {};
            $scope.fpct = {};

            $scope.orightml = '<h2>Titulo del contenido de Resolución</h2><p>Este es un ejemplo de texto o guia para escribir una resolucion customizada</p>';
            $scope._resolucion.htmlResol = $scope.orightml;
            //$scope.disabled = false;

            // Disable weekend selection
            $scope.WeekendDisabled = dateHelper.WeekendDisabled();

            $scope.minDate = dateHelper.minDate();
            $scope.maxDate = dateHelper.maxDate();
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
                else if (wtf == 'fmem')
                    $scope.fmem.open = true;
                else if (wtf == 'fnoti')
                    $scope.fnoti.open = true;
                else if (wtf == 'fecha2')
                    $scope.fecha2.open = true;
                else if (wtf == 'fecha3')
                    $scope.fecha3.open = true;
                else if (wtf == 'fref')
                    $scope.fref.open = true;
                else if (wtf == 'fa')
                    $scope.fa.open = true;
                else if (wtf == 'fv')
                    $scope.fv.open = true;
                else if (wtf == 'fr')
                    $scope.fr.open = true;
                else if (wtf == 'fpct')
                    $scope.fpct.open = true;

            };

            //calendar options
            $scope.dateOptions = dateHelper.dateOptions;

            //calendar date format
            $scope.formats = dateHelper.formats;
            $scope.format = $scope.formats[1];

            $scope.isMnuEnabled = function (opcion) {
                var _disabled = "";
                try {
                    if (opcion._estatus.length == 0) {
                        _disabled = "";
                    }
                    else {
                        //Uncomment to validate status on each option
                        //_disabled = (opcion._estatus.indexOf($scope.model.Expediente.EstatusId) == -1) ? "disabled" : "";
                        _disabled = bouncer.isMenuDisabledForPatentes(opcion, $scope.model);
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
                console.log(view);
                //Uncomment to validate status on each option
                //if (opcion._estatus.length !== 0 && (view === '' || opcion._estatus.indexOf($scope.model.Expediente.EstatusId) === -1)) {
                if (bouncer.isUnauthorizedActionForPatentes(opcion, view, $scope.model)) {
                    console.log("production mode... status validation in place");
                    $scope.res = './Spa/Views/empty.html';
                    $scope.$apply();
                }
                else {
                    $scope.res = './Spa/Views/ePatentes/' + view;
                    $scope.evento = opcion.opcion;
                }
                $scope._resolucion = { resId: "", FechaResolucion: new Date() };
            };

            $scope.rollbackResolution = function () {
                if ($routeParams.numero)
                    $location.path("/Patentes/" + $routeParams.numero + "/" + $routeParams.tipo);
                else
                    $location.path("/Patentes");
                $scope.Resolucion({ _estatus: [0] });//reset resolution
                showLeftBox();
            };

            _ctrlInit();
            if ($routeParams.numero && $routeParams.tipo) {
                $scope.getExpediente2($routeParams.tipo, $routeParams.numero);
            }
            else if ($routeParams.numero) {
                getEntityById($routeParams.numero);
            }
        }]); 
