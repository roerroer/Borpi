/* rpi */
var yumKaaxControllers=angular.module("PublicControllers",[]);yumKaaxControllers.controller("HomeCtrl",["$scope","$location","authService","$timeout","$http",function(o,t,n,e,a){function i(e,t){a.get(n.api()+"/Avisos/GetPage?page="+e+"&pageSize="+t,{headers:{access_token:""}}).then(function(e){o.avisos=e.data.Result.DataSet,o.totalItems=e.data.Result.TotalItems})}o.userIdentity={},o.pageSize=5,o.pageNumber=1,o.totalItems=0,o.pageSize=5,o.eRpiAdminMod="/Admin",o.eRpiMod="/eRPI",o.eRPIRecepcionMod="/Admin",o.eRPIConsulta="/Admin",o.isAdmin=!1,o.isLoggedIn=!1,n.custodian(function(){o.userIdentity=n.getUserInfo(),e(function(){console.log("")}),o.getAvisos()}),o.logout=function(){n.logout().then(function(e){o.userIdentity=null,t.path("/login")},function(e){console.log(e)})},o.getAvisos=function(){i(o.pageNumber,o.pageSize)},o.pageChanged=function(){i(o.pageNumber,o.pageSize)}}]),yumKaaxControllers.controller("navBarCtrl",["$scope","$location","authService","$http",function(t,o,e,n){t.preIngresoLink="#",n.get("/publico/content/json/pre-ingreso.json").then(function(e){console.log(e),t.preIngresoLink=e.data.preIngreso},function(e){console.log(e)}),e.custodian(function(){t.userIdentity=e.getUserInfo()}),t.logout=function(){e.logout().then(function(e){t.userIdentity=null,o.path("/login")},function(e){console.log(e)})}}]),yumKaaxControllers.controller("authCtrl",["$scope","$location","authService","feedbackSrvc",function(t,o,e,n){t.userIdentity=null,t.login=function(){e.login(t.userName,t.password).then(function(e){t.userIdentity=e,o.path("/home")},function(e){n.showAlertInfo("Invalid credentials**")})},t.cancel=function(){t.userName="",t.password=""}}]),yumKaaxControllers.controller("mexpedienteCtrl",["$scope","authService","listService","$http","$routeParams","feedbackSrvc",function(o,n,t,a,e,i){function r(e,t){a.get(n.api()+"/Marca/"+e+t,{headers:{access_token:n.getToken()}}).then(function(e){o.model=e.data.Result.documento,console.log("Fetched!!")})}o.tipoDeRegistroDeMarcas=[],o.model={EsFavorito:!1},o.grupos=[],o.lists={},o.getExpedienteByNumero=function(e){r("Expediente","?numero="+e)},o.getExpedienteByRegistro=function(e,t){r("registro","?tipoDeRegistroId="+e.Id+"&registro="+t)},o.tipoDeRegistroDeMarcas=[],o.resolvePais=function(t){var e=o.lists.paises?o.lists.paises.filter(function(e){return e.Id==t}):[];return 0<e.length?e[0].Nombre:""},e.Id&&r("ExpedienteId","?Id="+e.Id),n.custodian(function(){o.lists=t.getLists(),function(){t.reFetchLists();var e=t.getLists();o.tipoDeRegistroDeMarcas=e.tipoDeRegistroDeMarcas}()})}]),yumKaaxControllers.controller("mlogotiposCtrl",["$scope","$location","authService","classSrvc","feedbackSrvc",function(t,e,o,n,a){t.userIdentity=o.getUserInfo(),t.niza=n.getNizaClass(),t.nizaSel={niza:{}},t.vienalstSel=[],t.vienalst=n.getVienaClass(),t.selectViena=function(e){t.vienalstSel.push(e),t.vienaSelected=null}}]),yumKaaxControllers.controller("mfoneticaCtrl",["$scope","$location","authService","classSrvc","$http","feedbackSrvc",function(n,e,a,t,r,o){n.userIdentity=a.getUserInfo(),n.niza=t.getNizaClass(),n.nizaSel={niza:{}},n.tipoDeBusqueda=[{Id:1,Nombre:"Identica"},{Id:2,Nombre:"Fonetica"}],n.tipoDeBusquedaSel=n.tipoDeBusqueda[1],n.pageSize=0,n.dataset=[],n.pageNumber=1,n.pageChanged=function(){n.searchMarcas()},n.searchMarcas=function(){var e="",t=n.nizaSel.niza;if(t.All)e="";else{for(i=0;i<46;i++)t[i]&&(e+=i.toString()+",");t[99]&&(e+="99,")}1<e.length&&(e=e.slice(0,-1));var o="/Marca/";n.getExpedienteUrl=function(e){return"mexpediente/"+e.ExpedienteId},n.tipoDeBusquedaSel&&2!=n.tipoDeBusquedaSel.Id?r.get(a.api()+o+"BusquedaIdentica?pageNumber="+n.pageNumber+"&pageSize="+n.pageSize+"&textToSearch="+n.textSearch.toUpperCase()+"&csvClases="+e,{headers:{access_token:a.getToken()}}).then(function(e){n.dataset=e.data.Result.DataSet,console.log(JSON.stringify(e.data.Result.DataSet[0])),n.totalItems=e.data.Result.TotalItems}):r.get(a.api()+o+"BusquedaFonetica?pageNumber="+n.pageNumber+"&pageSize="+n.pageSize+"&textToSearch="+n.textSearch.toUpperCase()+"&csvClases="+e,{headers:{access_token:a.getToken()}}).then(function(e){n.dataset=e.data.Result.DataSet,console.log(JSON.stringify(e.data.Result.DataSet[0])),n.totalItems=e.data.Result.TotalItems})}}]),yumKaaxControllers.controller("mpreIngresoCtrl",["$scope","$location","authService","$routeParams","$timeout","feedbackSrvc",function(e,t,o,n,a,i){e.userIdentity=o.getUserInfo(),e.misExpedientes=[],e.totalItems=0,e.pageSize=10,e.expediente={},n.Id||(e.misExpedientes=[{Id:"100001",SignoDistintivo:"Demo 1",TipoDeRegistro:"Registro Inicial de Marca",EntidadSolicitante:"Demo",FechaUltimaActualizacion:"05/01/20015"},{Id:"100002",SignoDistintivo:"Demo 2",TipoDeRegistro:"Nombre Comercial",EntidadSolicitante:"Demo",FechaUltimaActualizacion:"05/01/20015"},{Id:"100003",SignoDistintivo:"Demo 3",TipoDeRegistro:"Nombre Comercial",EntidadSolicitante:"Demo",FechaUltimaActualizacion:"05/01/20015"}])}]),yumKaaxControllers.controller("pexpedienteCtrl",["$scope","$location","authService","listService","$http","$routeParams","feedbackSrvc",function(o,e,n,t,a,i,r){function s(e,t){a.get(n.api()+"/Patente/"+e+t,{headers:{access_token:n.getToken()}}).then(function(e){o.model=e.data.Result.documento;for(var t=0;t<o.tipoDePatentes.length;t++)o.tipoDePatentes[t].Id==o.model.Expediente.TipoDeRegistroId&&(o.model.Expediente.TipoDeRegistroId=o.tipoDePatentes[t]);console.log("Fetched by expediente!!")})}o.userIdentity=n.getUserInfo(),o.tipoDePatentes=[],o.model={},o.service="/Patente/",o.grupos=[],o.model.EsFavorito=!1,o.lists={},n.custodian(function(){o.lists=t.getLists(),function(){t.reFetchLists();var e=t.getLists();o.tipoDePatentes=e.tipoDePatentes}()}),i.Id&&s("ExpedienteId","?Id="+i.Id),o.getExpedienteByNumero=function(e,t){s("Expediente","?Numero="+t+"&TipoDeRegistroId="+e.Id)},o.getExpedienteByRegistro=function(e,t){s("Registro","?Registro="+t+"&TipoDeRegistroId="+e.Id)},o.resolvePais=function(t){var e=o.lists.paises?o.lists.paises.filter(function(e){return e.Id==t}):[];return 0<e.length?e[0].Nombre:""}}]),yumKaaxControllers.controller("pbusquedadCtrl",["$scope","authService","$http","listService","feedbackSrvc",function(t,o,n,a,e){t.userIdentity=o.getUserInfo(),t.pageNumber=0,t.pageSize=0,t.dataset=[],t.tipoDePatentes=[],t.lists={},o.custodian(function(){t.lists=a.getLists(),function(){a.reFetchLists();var e=a.getLists();t.tipoDePatentes=e.tipoDePatentes}()}),t.searchPatentes=function(){t.getExpedienteUrl=function(e){return"pexpediente/"+e.ExpedienteId};var e=null;t.tipoDeRegistroId&&(e=t.tipoDeRegistroId.Id),n.get(o.api()+"/Patente/BusquedaPatentesDsc?pageNumber="+t.pageNumber+"&pageSize="+t.pageSize+"&textToSearch="+t.textSearch.toUpperCase()+"&tipoDeRegistro="+e,{headers:{access_token:o.getToken()}}).then(function(e){t.dataset=e.data.Result.DataSet,t.totalItems=e.data.Result.DataSet.length})}}]),yumKaaxControllers.controller("favoritosCtrl",["$scope","$location","authService","classSrvc","$http","$timeout","feedbackSrvc",function(o,e,n,t,a,i,r){n.custodian(function(){t.getGrupos().success(function(e){o.grupos=e.Result.DataSet,0===o.grupos.length&&o.grupos.push({Id:0,Nombre:"Mis Expedientes"})}).error(function(e){o.grupos=[{id:0,Nombre:"Todos"}]})}),o.addFavorito=function(e,t){a.post(n.api()+"/Favoritos/AddFavorito",{expediente:e,grupoId:t},{headers:{access_token:n.getToken()}}).then(function(e){e.data.Succeeded?o.model.EsFavorito=!0:r.showAlertInfo("Error agregando expediente a mi listado de expedientes...")},function(e){r.handleError(e,"Error agregando expediente a mi listado de expedientes...")})},o.esFavoritoBtn=function(){return o.model.EsFavorito?"btn btn-primary btn-xs dropdown-toggle":"btn btn-default btn-xs dropdown-toggle"},o.esFavoritoIcon=function(){return o.model.EsFavorito?"glyphicon glyphicon-saved":"glyphicon glyphicon-pushpin"}}]),yumKaaxControllers.controller("dexpedienteCtrl",["$scope","$location","authService","listService","$http","$timeout","$routeParams","feedbackSrvc",function(o,e,n,t,a,i,r,s){function c(e,t){a.get(n.api()+"/DAutor/"+e+t,{headers:{access_token:n.getToken()}}).then(function(e){o.model=e.data.Result.documento,console.log("Fetched!!")})}o.userIdentity=n.getUserInfo(),o.model={},o.grupos=[],o.model.EsFavorito=!1,o.lists={},n.custodian(function(){o.lists=t.getLists()}),o.resolvePais=function(t){var e=o.lists.paises?o.lists.paises.filter(function(e){return e.Id==t}):[];return 0<e.length?e[0].Nombre:""},r.Id&&c("ExpedienteId","?Id="+r.Id),o.getExpedienteByNumero=function(e){c("Expediente","?Numero="+e)},o.getExpedienteByRegistro=function(e){c("Registro","?Registro="+e)}}]),yumKaaxControllers.controller("gacetaCtrl",["$scope","authService","$http","listService","dateHelper","feedbackSrvc","$sce",function(n,a,i,e,t,o,r){function s(e,t){n.textSearch?n.filterFunction(n.textSearch,e,t):i.get(a.api()+n.seccion+"/GetPage?page="+e+"&pageSize="+t,{headers:{access_token:a.getToken()}}).then(function(e){c(e.data.Result.DataSet),n.totalItems=e.data.Result.TotalItems})}function c(e){if("/gaceta"===n.seccion)for(var t=0;t<e.length;t++)-1<e[t].logotipo.toLowerCase().indexOf("mp3")?e[t].format="mp3":e[t].format="";for(t=0;t<e.length;t++)e[t].HTMLDOC=e[t].HTMLDOC?r.trustAsHtml(e[t].HTMLDOC):e[t].HTMLDOC;n.Publicaciones=e}n.Publicaciones=[],n.pageNumber=1,n.totalItems=0,n.pageSize=100,n.lists={},n.secciones=[],n.seccionGaceta=[],n.seccion="/gaceta",n.disabled=t.disabled,n.minDate=t.minDate(),n.maxDate=t.maxDate(),n.status={opened:!1},n.open=function(e){n.status.opened=!0},n.dateOptions=t.dateOptions,n.formats=t.formats,n.format=n.formats[1],a.custodian(function(){n.lists=e.getLists(),n.secciones=n.lists.seccionesGaceta,n.seccionGaceta=n.secciones[0],s(1,n.pageSize)}),n.resolvePais=function(t){var e=n.lists.paises?n.lists.paises.filter(function(e){return e.Id===t}):[];return 0<e.length?e[0].Nombre:""},n.onSeccionChange=function(){console.log(n.seccionGaceta),1==n.seccionGaceta.Id?n.seccion="/gaceta":2==n.seccionGaceta.Id?n.seccion="/gacetapatentesedictos":3==n.seccionGaceta.Id?n.seccion="/GacetaMarcasAnotaciones":4==n.seccionGaceta.Id&&(n.seccion="/GacetaGrl"),n.pageNumber=1,n.Publicaciones=[],s(1,n.pageSize)},n.pageChanged=function(){if(n.fechaSelected&&"[object Date]"===Object.prototype.toString.call(n.fechaSelected)){var e=n.fechaSelected.getDate()+"/"+(n.fechaSelected.getMonth()+1)+"/"+n.fechaSelected.getFullYear();n.filterFunction(e,n.pageNumber,n.pageSize)}else n.textSearch&&""!==n.textSearch?n.filterFunction(n.textSearch,n.pageNumber,n.pageSize):s(n.pageNumber,n.pageSize)},n.$watch("textSearch",function(e,t){e!==t&&(""==e?s(n.pageNumber,n.pageSize):n.filterFunction(e,n.pageNumber,n.pageSize))},!0),n.$watch("fechaSelected",function(e,t){e!==t&&(null!==e&&"[object Date]"===Object.prototype.toString.call(e)?n.filterFunction(e.getDate()+"/"+(e.getMonth()+1)+"/"+e.getFullYear(),n.pageNumber,n.pageSize):s(n.pageNumber,n.pageSize))},!0),n.filterFunction=function(e,t,o){i.get(a.api()+n.seccion+"/GetPageFilter?filter="+e+"&page="+t+"&pageSize="+o,{headers:{access_token:a.getToken()}}).then(function(e){c(e.data.Result.DataSet),n.totalItems=e.data.Result.TotalItems})}}]),yumKaaxControllers.controller("gacetaSemanalCtrl",["$scope","authService","$timeout","$routeParams","listService","dateHelper","feedbackSrvc",function(t,e,o,n,a,i,r){t.fechaSelected=new Date,t.seccion="/gaceta",t.area=null,e.custodian(function(){t.lists=a.getLists(),console.log(t.lists.seccionesGaceta),t.area=t.lists.seccionesGaceta.filter(function(e){return e.Id==n.gaceta})[0],o(function(){console.log(t.area)})}),1==n.gaceta?t.seccion="/gaceta":2==n.gaceta?t.seccion="/gacetapatentesedictos":3==n.gaceta?t.seccion="/GacetaMarcasAnotaciones":4==n.gaceta&&(t.seccion="/GacetaGrl"),t.disabled=i.disabled,t.minDate=i.minDate(),t.maxDate=i.maxDate(),t.status={opened:!1},t.open=function(e){t.status.opened=!0},t.dateOptions=i.dateOptions,t.formats=i.formats,t.format=t.formats[1]}]),yumKaaxControllers.controller("misexpedientesCtrl",["$scope","$location","authService","$http","classSrvc","$timeout","feedbackSrvc",function(n,e,a,i,t,o,r){function s(){t.getGrupos().success(function(e){n.grupos=e.Result.DataSet}).error(function(e){n.grupos=[{id:0,Nombre:"Mis Expedientes"}]}),c(null,1,n.pageSize)}function c(e,t,o){n.currentGrupo=e,n.currentGrupo?i.get(a.api()+"/Favoritos/GetFavoritosPageFilter?idGrupoFilter="+n.currentGrupo+"&page="+t+"&pageSize="+o,{headers:{access_token:a.getToken()}}).then(function(e){n.misExpedientes=e.data.Result.DataSet,n.totalItems=e.data.Result.TotalItems}):i.get(a.api()+"/Favoritos/GetFavoritosPageFilter?idGrupoFilter=0&page="+t+"&pageSize="+o,{headers:{access_token:a.getToken()}}).then(function(e){n.misExpedientes=e.data.Result.DataSet,n.totalItems=e.data.Result.TotalItems})}n.userIdentity=a.getUserInfo(),n.grupos=[],n.misExpedientes=[],n.totalItems=0,n.pageSize=50,n.currentGrupo=null,n.addFormEnabled=!1,a.custodian(function(){s()}),n.getExpedienteUrl=function(e){return e?1==e.ModuloId?"mexpediente/"+e.ExpedienteId:2==e.ModuloId?"pexpediente/"+e.ExpedienteId:3==e.ModuloId?"dexpediente/"+e.ExpedienteId:void 0:"mexpediente"},n.pageNumber=1,n.pageChanged=function(){c(n.currentGrupo,n.pageNumber,n.pageSize)},n.getExpedientes=function(e){c(e,1,n.pageSize)},n.agregarGrupo=function(){n.addFormEnabled=!0},n.addGrupo=function(e){if(e){var t={Id:0,Nombre:e};i.post(a.api()+"/Favoritos/AddGrupo",t,{headers:{access_token:a.getToken()}}).then(function(e){e.data.Succeeded?(s(),n.addFormEnabled=!1):r.handleError(error)},function(e){r.handleError(e)})}}}]),yumKaaxControllers.controller("UsuariosPublicosCtrlr",["$scope","$location","$routeParams","authService","$http","classSrvc","$timeout","feedbackSrvc",function(o,e,n,a,i,t,r,s){function c(){i.get(a.api()+"/Admin/Entities/Get?entity=paises",{headers:{access_token:a.getToken()}}).then(function(e){o.paises=e.data.Result}),i.get(a.api()+"/Admin/UsuariosPublicos/LoadMyPerfil",{headers:{access_token:a.getToken()}}).then(function(e){o.usuario=e.data.Result,console.log(e.data.Result)})}o.usuario={},o.paises=[],a.custodian(function(){var e,t;console.log(n.spk),n.spk&&n.Id?(e=n.spk,t=n.Id,i.post(a.api()+"/Admin/UsuariosPublicos/GetWithSpk",{Id:t,Spk:e},{headers:{access_token:e}}).then(function(e){o.usuario=e.data.Result,e.data.Succeeded?o.usuario=e.data.Result:s.handleError(e.data),console.log(JSON.stringify(e.data.Result))},function(e){s.handleError(e)})):(n.miClave&&n.Id||n.spk||n.Id,c())}),o.isUnchanged=function(e){return angular.equals(e,o.master)},o.update=function(e){i.post(a.api()+"/Admin/UsuariosPublicos/save",{model:e},{headers:{access_token:a.getToken()}}).then(function(e){e.data.Succeeded?s.showAlertInfo("Usuario ha sido grabado correctamente..."):s.handleError(e.data)},function(e){s.handleError(e)})},o.setPW=function(e){i.post(a.api()+"/Admin/UsuariosPublicos/ResetPW",{model:e},{headers:{access_token:n.spk?n.spk:a.getToken()}}).then(function(e){e.data.Succeeded?s.showAlertInfo("Contrasena ha sido grabada correctamente...",n.spk?"/login":null):s.handleError(e.data)},function(e){s.handleError(e)})}}]),yumKaaxControllers.controller("UsuarioCtrlr",["$scope","$location","$routeParams","authService","$http","classSrvc","$timeout","feedbackSrvc",function(t,o,e,n,a,i,r,s){function c(e,t){s.showAlertInfo(e),t&&r(function(){o.path(t)},1400)}t.usuario={Id:-1,Cuenta:void 0,Pwd:void 0,Nombre:void 0,Empresa:void 0,Direccion:void 0,Ciudad:void 0,EstadoProvincia:void 0,Telefonos:"",PaisId:void 0},t.paises=[],n.custodian(function(){a.get(n.api()+"/Admin/Entities/Get?entity=paises",{headers:{access_token:""}}).then(function(e){t.paises=e.data.Result})}),t.emailMe=function(e){a.post(n.api()+"/Admin/UsuariosPublicos/cambiarClave",{model:e},{headers:{access_token:"",locPath:window.location}}).then(function(e){e.data.Succeeded?c("Un email ha sido enviado a su cuenta de acceso al sistema.","/login"):s.handleError(e.data)},function(e){s.handleError(e)})},t.save=function(e){a.post(n.api()+"/Admin/UsuariosPublicos/save",{model:e},{headers:{access_token:""}}).then(function(e){e.data.Succeeded?c("Usuario ha sido grabado correctamente...","/login"):s.handleError(e.data)},function(e){s.handleError(e)})}}]),yumKaaxControllers.controller("genericCtrl",["$scope","$location","authService","$http","classSrvc","feedbackSrvc",function(e,t,o,n,a,i){}]),yumKaaxControllers.controller("avisosCtrlr",["$scope","authService","$http","listService","feedbackSrvc",function(e,t,o,n,a){e.pageNumber=1,e.totalItems=0,e.pageSize=100}]),yumKaaxControllers.controller("gacetaLeyCtrl",["$scope",function(e){}]),angular.module("authFactory",[]).factory("authService",["$http","$q","$window","$interval","$timeout",function(n,a,e,i,r){var s,c="/eRegistroAPI/AuthPublic/";function l(){return s}return $.jrzStorage.local.exists("appPublicUser")&&(s=$.jrzStorage.local.getItem("appPublicUser")),{login:function(e,t){var o=a.defer();return n.post(c+"Login",{userName:e,password:t}).then(function(e){e.data.Succeeded?(s={accessToken:e.data.Result.token,userName:e.data.Result.Nombre,email:e.data.Result.Email},$.jrzStorage.local.setItem("appPublicUser",s),o.resolve(s)):o.reject("-Cuenta no existe en el sistema-")},function(e){o.reject(e)}),o.promise},logout:function(){var t=a.defer();return n({method:"POST",url:c+"logout",headers:{access_token:s.accessToken}}).then(function(e){s=null,$.jrzStorage.local.removeItem("appPublicUser"),t.resolve(e)},function(e){t.reject(e)}),t.promise},getUserInfo:l,getToken:function(){return s||console.log("no-token"),s?s.accessToken:"no-token"},api:function(){return"/eRegistroAPI"},custodian:function(e){var t,o=void 0;e&&(t=i(function(){null===l()||o?o&&angular.isDefined(t)&&(i.cancel(t),t=void 0):(console.log("custodian enabled"),r(e,1e3,!1),o=!0)},1e3))}}}]).factory("listService",["$http","$q","authService",function(e,o,n){var a,i="/eRegistroAPI/Admin/Entities/";function t(){var t=o.defer();return e.get(i+"Get?entity=all",{headers:{access_token:n.getToken()}}).then(function(e){a=e.data.Result,$.jrzStorage.local.setItem("lists",a),t.resolve(a)},function(e){t.reject(e)}),t.promise}return $.jrzStorage.local.exists("lists")?a=$.jrzStorage.local.getItem("lists"):t(),{getLists:function(){return a},reFetchLists:function(){return t(),a}}}]).factory("classSrvc",["$http","$q","authService",function(e,o,t){var n;function a(){var t=o.defer();return e.get("/publico/content/json/viena.json").then(function(e){n=e.data,$.jrzStorage.local.setItem("vienaClass",n),t.resolve(n)},function(e){t.reject(e)}),t.promise}return console.log("classSrvc init..."),a(),{getNizaClass:function(){var e=[];for(i=0;i<46;i++)e.push({code:i,selected:!1});return e.push({code:99,selected:!1}),e.push({code:"All",selected:!1}),e},getVienaClass:function(){return $.jrzStorage.local.exists("vienaClass")?n=$.jrzStorage.local.getItem("vienaClass"):a(),n},getGrupos:function(){return e.get("/eRegistroAPI/Favoritos/GetGrupos",{headers:{access_token:t.getToken()}})}}}]).factory("dateHelper",["uibDateParser",function(t){var e={disabled:function(e,t){return"day"===t&&(10===e.getDay()||6===e.getDay())},dateOptions:{formatYear:"yy",startingDay:1,timezone:"CST"},YearOptions:{formatYear:"yyyy",startingDay:1,minMode:"year"},fixDate:function(e){var t=new Date;return e&&(t=new Date(parseInt(e.substr(6)))),t},formats:["MM/dd/yyyy hh:mm:ss","dd/MM/yyyy","dd.MM.yyyy","shortDate"]};return e.format=e.formats[1],e.minDate=function(){return new Date(1899,1,1)},e.maxDate=function(){return new Date((new Date).setDate((new Date).getDate()+365))},e.parseStrDate=function(e){return 0<e.length?t.parse(e,"yyyy-MM-ddThh:mm:ss"):null},e.WeekendDisabled=function(e,t){return"day"===t&&(10===e.getDay()||6===e.getDay())},e}]).factory("feedbackSrvc",["$q","$timeout","$rootScope","$location",function(e,o,t,n){function a(e){var t=new Date;r.unshift(t.getHours()+":"+t.getMinutes()+":"+t.getSeconds()+" "+e)}var i={},r=[];return i.flush=function(){r=[],t.$broadcast("feedbackOn","feedbackOn")},i.handleError=function(e,t){console.log(e),t=e&&401===e.status?"El usuario no tiene permisos para realizar la operacion!":e&&!1===e.Succeeded?e.Errors?e.Errors:"El usuario no tiene permisos para realizar la operacion!":t||"El sistema ha encontrado un error inesperado, porfavor contacte al administrador del sistema!",a(t),o(i.flush,1e4,!0)},i.showAlertInfo=function(e,t){a(e),o(i.flush,1e4,!0),t&&o(function(){n.path(t)},1400)},i.isTriggered=function(){return 0<r.length},i.getFeedback=function(){return r},i}]);var yumKaaxApp=angular.module("yumKaaxApp",["ngRoute","PublicControllers","authFactory","chieffancypants.loadingBar","ngAnimate","ui.bootstrap","ui.utils","grlDirectives","appfeedback"]);yumKaaxApp.config(["$routeProvider",function(e){var t="./Spa/Views/";e.when("/",{templateUrl:t+"Home.html",controller:"HomeCtrl",resolve:{auth:["$q","authService",function(e,t){var o=t.getUserInfo();return o?e.when(o):e.reject({authenticated:!1})}]}}).when("/home",{templateUrl:t+"home.html",controller:"HomeCtrl"}).when("/leygacetaofficialrpi",{templateUrl:t+"gacetaley.html",controller:"gacetaLeyCtrl"}).when("/gaceta",{templateUrl:t+"gaceta.html",controller:"gacetaCtrl"}).when("/gacetasemanal",{templateUrl:t+"gacetasemanal.html",controller:"gacetaSemanalCtrl"}).when("/gacetasemanal/:gaceta",{templateUrl:t+"gacetasemanal.html",controller:"gacetaSemanalCtrl"}).when("/mregistro",{templateUrl:t+"mregistro.html",controller:"mexpedienteCtrl"}).when("/mexpediente",{templateUrl:t+"mexpediente.html",controller:"mexpedienteCtrl"}).when("/mexpediente/:Id",{templateUrl:t+"mexpediente.html",controller:"mexpedienteCtrl"}).when("/mpreIngreso",{templateUrl:t+"mpreIngreso.html",controller:"mpreIngresoCtrl"}).when("/mpreIngreso/:Id",{templateUrl:t+"mpreIngresoABC.html",controller:"mpreIngresoCtrl"}).when("/mfonetica",{templateUrl:t+"mfonetica.html",controller:"mfoneticaCtrl"}).when("/mlogotipos",{templateUrl:t+"mlogotipos.html",controller:"mlogotiposCtrl"}).when("/pregistro",{templateUrl:t+"pregistro.html",controller:"pexpedienteCtrl"}).when("/pexpediente",{templateUrl:t+"pexpediente.html",controller:"pexpedienteCtrl"}).when("/pexpediente/:Id",{templateUrl:t+"pexpediente.html",controller:"pexpedienteCtrl"}).when("/pbusquedad",{templateUrl:t+"pbusquedad.html",controller:"pbusquedadCtrl"}).when("/dregistro",{templateUrl:t+"dregistro.html",controller:"dexpedienteCtrl"}).when("/dexpediente",{templateUrl:t+"dexpediente.html",controller:"dexpedienteCtrl"}).when("/dexpediente/:Id",{templateUrl:t+"dexpediente.html",controller:"dexpedienteCtrl"}).when("/misexpedientes",{templateUrl:t+"misexpedientes.html",controller:"misexpedientesCtrl"}).when("/rpilive",{templateUrl:t+"rpilive.html",controller:"genericCtrl"}).when("/eTomo",{templateUrl:t+"eTomo.html",controller:"genericCtrl"}).when("/econtacto",{templateUrl:t+"econtacto.html",controller:"genericCtrl"}).when("/usuario",{templateUrl:t+"UsuariosPublicosAbc.html",controller:"UsuariosPublicosCtrlr"}).when("/crear-cuenta",{templateUrl:t+"UsuariosPublicosA.html",controller:"UsuarioCtrlr"}).when("/olvide-clave",{templateUrl:t+"UsuariosRecordarClave.html",controller:"UsuarioCtrlr"}).when("/ResetPW/:miClave/:Id",{templateUrl:t+"UsuariosPublicosResetPW.html",controller:"UsuariosPublicosCtrlr"}).when("/cambiarPW/:spk/:Id",{templateUrl:t+"UsuariosPublicosResetPW.html",controller:"UsuariosPublicosCtrlr"}).when("/login",{templateUrl:t+"Login.html",controller:"authCtrl"}).otherwise({redirectTo:""})}]),yumKaaxApp.config(["cfpLoadingBarProvider",function(e){e.includeSpinner=!0}]),yumKaaxApp.run(["$rootScope","$location","authService",function(e,a,t){e.$on("$routeChangeSuccess",function(e){0<=a.path().indexOf("crear-cuenta")||0<=a.path().indexOf("cambiarPW")||0<=a.path().indexOf("olvide-clave")||null!==t.getUserInfo()&&void 0!==t.getUserInfo()||a.path("/login")}),e.$on("$routeChangeError",function(e,t,o,n){!1===n.authenticated&&a.path("/login")})}]),yumKaaxApp.filter("dateformat",function(){var o=/\\\/Date\(([0-9]*)\)\\\//;return function(e){var t=e.match(o);return t?new Date(parseInt(t[1])):null}}),angular.module("grlDirectives",[]).directive("equals",function(){return{restrict:"A",require:"?ngModel",link:function(e,t,o,n){if(n){e.$watch(o.ngModel,function(){a()}),o.$observe("equals",function(e){a()});var a=function(){var e=n.$viewValue,t=o.equals;n.$setValidity("equals",!e||!t||e===t)}}}}}).directive("ngConfirmClick",function(){return{link:function($scope,element,attr){var msg=attr.ngConfirmClick||"Esta segur@? de continuar",clickAction=attr.confirmedClick,jsClick=attr.jsClick;element.bind("click",function(event){window.confirm(msg)&&($scope.$eval(clickAction),eval(jsClick))})}}}).directive("confirmAction",["$uibModal","$parse",function(a,i){return{restrict:"EA",link:function(t,e,o){if(o.continueWith){function n(){t.msg=o.msg?o.msg:"Continuar?",t.confirmButtonText=o.confirmButtonText?o.confirmButtonText:"Yes",t.cancelButtonText=o.cancelButtonText?o.cancelButtonText:"No"}e.click(function(){var e=i(o.continueWith);if((console.log(o.askWhen),o.askWhen)&&!i(o.askWhen)(t))return e(t),void t.$apply();a.open({size:"sm",template:'<div class="modal-header"><h3 class="modal-title">{{msg}}</h3></div><div class="modal-footer"><div class="row"> <div class="col-md-4"><button type="button" class="btn btn-primary btn-block" ng-click="$close(\'ok\')">{{ confirmButtonText }}</button></div> <div class="col-md-4"><button type="button" class="btn btn-primary btn-block" ng-click="$dismiss(\'cancel\')">{{ cancelButtonText }}</button></div></div></div>',controller:n,scope:t}).result.then(function(){e(t),t.$apply()})})}}}}]).directive("loadSpinner",["$http","$rootScope",function(n,a){return{link:function(e,t,o){a.spinnerActive=!1,e.isLoading=function(){return 0<n.pendingRequests.length},e.$watch(e.isLoading,function(e){(a.spinnerActive=e)?t.removeClass("ng-hide"):t.addClass("ng-hide")})},template:'<div id="appSpinner"><div class="kart-loader"><div class="sheath"><div class="segment"></div></div><div class="sheath"><div class="segment"></div></div><div class="sheath"><div class="segment"></div></div><div class="sheath"><div class="segment"></div></div><div class="sheath"><div class="segment"></div></div><div class="sheath"><div class="segment"></div></div><div class="sheath"><div class="segment"></div></div><div class="sheath"><div class="segment"></div></div><div class="sheath"><div class="segment"></div></div><div class="sheath"><div class="segment"></div></div><div class="sheath"><div class="segment"></div></div><div class="sheath"><div class="segment"></div></div></div></div>'}}]),angular.module("appfeedback",[]).directive("appFeedback",["feedbackSrvc",function(n){return{link:function(o,e,t){o.feedbackLog=[],o.isTriggered=function(){return n.isTriggered()},o.$watch(o.isTriggered,function(e){o.feedbackLog=n.getFeedback()}),o.$on("feedbackOn",function(e,t){o.feedbackLog=n.getFeedback()}),o.closeAppPopup=function(){n.flush()}},template:'<div class="message-popup-box box-feedback" style="right: 0px;" ng-show="feedbackLog.length>0"><div class="popup-header box-header box-feedback"><span class="glyphicon glyphicon-exclamation-sign" font-size 20px;></span> Aviso(s) del Sistema<span class="glyphicon glyphicon-remove close-popup-box" ng-click="closeAppPopup()"></span></div><div class="popup-body"><table class="table table-striped tbl-text-warns"><tbody><tr ng-repeat="m in feedbackLog"><td  ng-bind="m"></td></tr></tbody></table></div></div>'}}]);