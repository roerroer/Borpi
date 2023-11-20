var yumKaaxControllers = angular.module('PublicControllers', []);

yumKaaxControllers.controller("HomeCtrl",
    ["$scope", "$location", "authService", "$timeout", "$http",
        function ($scope, $location, authService, $timeout, $http) {
            $scope.userIdentity = {};
            $scope.pageSize = 5;
            $scope.pageNumber = 1;
            $scope.totalItems = 0;
            $scope.pageSize = 5;

            $scope.eRpiAdminMod = "/Admin";
            $scope.eRpiMod = "/eRPI";
            $scope.eRPIRecepcionMod = "/Admin";
            $scope.eRPIConsulta = "/Admin";

            $scope.isAdmin = false;
            $scope.isLoggedIn = false;

            authService.custodian(function () {
                $scope.userIdentity = authService.getUserInfo();
                $timeout(function () { console.log(''); });
                $scope.getAvisos();
            });

            $scope.logout = function () {
                authService.logout()
                    .then(function (result) {
                        $scope.userIdentity = null;
                        $location.path("/login");
                    }, function (error) {
                        console.log(error);
                    });
            };

            $scope.getAvisos = function () {
                getAvisos($scope.pageNumber, $scope.pageSize);
            };

            function getAvisos(pageNumber, pageSize) {
                $http.get(authService.api() + '/Avisos/GetPage?page=' + pageNumber + "&pageSize=" + pageSize,
                    {
                        headers: { "access_token": "" }
                    })
                    .then(function (result) {
                        $scope.avisos = result.data.Result.DataSet;
                        $scope.totalItems = result.data.Result.TotalItems;
                    });
            }

            $scope.pageChanged = function () {
                getAvisos($scope.pageNumber, $scope.pageSize);
            };
        }]);


yumKaaxControllers.controller("navBarCtrl",
    ["$scope", "$location", "authService", "$http",
        function ($scope, $location, authService, $http) {
            $scope.preIngresoLink = "#";

            $http.get("/publico/content/json/pre-ingreso.json")
                .then(
                function (result) {
                    console.log(result);
                        $scope.preIngresoLink = result.data.preIngreso;
                    },
                    function (error) {
                        console.log(error);
                    }
            );

            authService.custodian(function () {
                $scope.userIdentity = authService.getUserInfo();
            });

    
    $scope.logout = function () {
        authService.logout()
            .then(function (result) {
                $scope.userIdentity = null;
                $location.path("/login");
            }, function (error) {
                console.log(error);
            });
    };
}]);
