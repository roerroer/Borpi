/*
    page
*/

angular.module('authFactory', [])
    .factory('authService', ['$http', '$q', '$window', '$interval', '$timeout', function ($http, $q, $window, $interval, $timeout) {
        var userIdentity;
        var urlBase = '/eRegistroAPI/AuthPublic/';

        function login(userName, password) {
            var deferred = $q.defer();

            $http.post(urlBase + "Login", { userName: userName, password: password })
                .then(
                    function (result) {
                        if (result.data.Succeeded) {
                            userIdentity = {
                                accessToken: result.data.Result.token,
                                userName: result.data.Result.Nombre,
                                email: result.data.Result.Email
                                //Marcas: Marcas,
                                //MarcasAux: MarcasAux,
                                //Patentes: Patentes,
                                //DerechoDeAutor: DerechoDeAutor,
                                //Recepcion: Recepcion
                            };
                            //console.log('Public LogIn');
                            //console.log(JSON.stringify(userIdentity));
                            $.jrzStorage.local.setItem("appPublicUser", userIdentity)
                            deferred.resolve(userIdentity);
                        }
                        else {
                            deferred.reject("-Cuenta no existe en el sistema-");
                        }
                    },
                    function (error) {
                        deferred.reject(error);
                    }
                );
            return deferred.promise;
        }

        function logout() {
            var deferred = $q.defer();

            $http({
                method: "POST",
                url: urlBase + "logout",
                headers: {
                    "access_token": userIdentity.accessToken
                }
            }).then(function (result) {
                userIdentity = null;
                $.jrzStorage.local.removeItem("appPublicUser");
                deferred.resolve(result);
            }, function (error) {
                deferred.reject(error);
            });

            return deferred.promise;
        }

        function getUserInfo() {
            return userIdentity;
        }

        function getToken() {
            if (!userIdentity) console.log('no-token');
            return userIdentity ? userIdentity.accessToken : "no-token";
        }

        function api() {
            return '/eRegistroAPI';
        }

        function custodian(callback) {
            var job;
            var jobCompleted = undefined;
            function watch() {
                //console.log("INIT WORKFLOW");
                job = $interval(function () {
                    if (getUserInfo() !== null && !jobCompleted) {
                        console.log("custodian enabled");
                        $timeout(callback, 1000, false);
                        jobCompleted = true;
                    }
                    else if (jobCompleted) {
                        endWatch();
                    }
                }, 1000);
            }

            function endWatch() {
                if (angular.isDefined(job)) {
                    $interval.cancel(job);
                    job = undefined;
                }
            }

            if (callback)
                watch();
        }

        function init() {
            if ($.jrzStorage.local.exists("appPublicUser")) {
                userIdentity = $.jrzStorage.local.getItem("appPublicUser");
            }
        }
        init();

        return {
            login: login,
            logout: logout,
            getUserInfo: getUserInfo,
            getToken: getToken,
            api: api,
            custodian: custodian
        };
    }])
    .factory('listService', ['$http', '$q', 'authService', function ($http, $q, authService) {
        var lists;
        var urlBase = '/eRegistroAPI/Admin/Entities/';

        function fetchLists() {
            var deferred = $q.defer();

            $http.get(urlBase + 'Get?entity=all', { headers: { "access_token": authService.getToken() } })
                .then
                (
                function (result) {
                    lists = result.data.Result;
                    $.jrzStorage.local.setItem("lists", lists);
                    deferred.resolve(lists);
                    //console.log(JSON.stringify(lists.tipoDeRegistroDeMarcas));
                },
                function (error) {
                    deferred.reject(error);
                }
                );
            return deferred.promise;
        }

        function getLists() {
            return lists;
        }

        function reFetchLists() {
            fetchLists();
            return lists;
        }

        function init() {
            if ($.jrzStorage.local.exists("lists")) {
                lists = $.jrzStorage.local.getItem("lists");
            }
            else {
                fetchLists();
            }
        }

        init();

        return {
            getLists: getLists,
            reFetchLists: reFetchLists
        };
    }])
    .factory('classSrvc', ['$http', '$q', 'authService', function ($http, $q, authService) {
        var vienaList;

        function getNizaClass() {

            var niza = [];
            // create niza list   
            for (i = 0; i < 46; i++) {
                niza.push({ code: i, selected: false });
            }
            niza.push({ code: 99, selected: false });
            niza.push({ code: "All", selected: false });
            return niza;
        }

        function _fetchVienaClass() {
            var deferred = $q.defer();
            $http.get("/publico/content/json/viena.json")
                .then(
                    function (result) {
                        vienaList = result.data;
                        $.jrzStorage.local.setItem("vienaClass", vienaList)
                        deferred.resolve(vienaList);
                    },
                    function (error) {
                        deferred.reject(error);
                    }
                );
            return deferred.promise;
        }

        function getVienaClass() {
            if ($.jrzStorage.local.exists("vienaClass")) {
                vienaList = $.jrzStorage.local.getItem("vienaClass");
            }
            else {
                _fetchVienaClass();
            }
            return vienaList;
        }

        function _init() {
            console.log('classSrvc init...');
            _fetchVienaClass();
        }

        _init();


        function getGrupos() {
            return $http.get('/eRegistroAPI/Favoritos/GetGrupos', { headers: { "access_token": authService.getToken() } });
        };

        return {
            getNizaClass: getNizaClass,
            getVienaClass: getVienaClass,
            getGrupos: getGrupos
        };
    }])
    .factory('dateHelper', ['uibDateParser', function (uibDateParser) {
        // Disable weekend selection   
        var _sysdate = {};

        _sysdate.disabled = function (date, mode) {
            return mode === 'day' && (date.getDay() === 10 || date.getDay() === 6);
        };

        //calendar options
        _sysdate.dateOptions = {
            formatYear: 'yy',
            startingDay: 1,
            timezone: 'CST'
        };

        _sysdate.YearOptions = {
            formatYear: 'yyyy',
            startingDay: 1,
            minMode: 'year'
        };

        _sysdate.fixDate = function (x) {
            var date = new Date();
            if (x)
                date = new Date(parseInt(x.substr(6)));
            return date;
        };

        //calendar date format
        _sysdate.formats = ['MM/dd/yyyy hh:mm:ss', 'dd/MM/yyyy', 'dd.MM.yyyy', 'shortDate'];
        _sysdate.format = _sysdate.formats[1];

        _sysdate.minDate = function () {
            return new Date(1899, 1, 1);
        };

        _sysdate.maxDate = function () {
            return new Date((new Date()).setDate((new Date()).getDate() + 365));
        };

        _sysdate.parseStrDate = function (strDate) {
            if (strDate.length > 0)
                return uibDateParser.parse(strDate, "yyyy-MM-ddThh:mm:ss");
            else
                return null;
        };

        _sysdate.WeekendDisabled = function (date, mode) {
            return mode === 'day' && (date.getDay() === 10 || date.getDay() === 6);
        };

        return _sysdate;
    }])
    .factory('feedbackSrvc', ['$q', '$timeout', '$rootScope', '$location', function ($q, $timeout, $rootScope, $location) {
        var _service = {};
        var appPopupMsg = [];

        var pushMessage = function (message) {
            var ts = new Date();
            appPopupMsg.unshift(ts.getHours() + ":" + ts.getMinutes() + ":" + ts.getSeconds() + " " + message);
        };

        _service.flush = function () {
            appPopupMsg = [];
            $rootScope.$broadcast('feedbackOn', 'feedbackOn');
        };

        _service.handleError = function (error, message) {
            console.log(error);
            if (error && error.status === 401)
                message = 'El usuario no tiene permisos para realizar la operacion!';
            else if (error && error.Succeeded === false) 
                message = error.Errors ? error.Errors : 'El usuario no tiene permisos para realizar la operacion!';
            else if (!message)
                message = "El sistema ha encontrado un error inesperado, porfavor contacte al administrador del sistema!";

            pushMessage(message);
            $timeout(_service.flush, 10000, true);
        };

        _service.showAlertInfo = function (message, redirect) {
            pushMessage(message);
            $timeout(_service.flush, 10000, true);
            if (redirect)
                $timeout(function () { $location.path(redirect); }, 1400);
        };

        _service.isTriggered = function () {
            return appPopupMsg.length > 0;
        };

        _service.getFeedback = function () {
            return appPopupMsg;
        };

        return _service;
    }]);