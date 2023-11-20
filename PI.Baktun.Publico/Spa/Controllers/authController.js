
yumKaaxControllers.controller
    (
    "authCtrl",
    [
        "$scope", "$location", "authService", "feedbackSrvc",
        function ($scope, $location, authService, feedbackSrvc) {
            $scope.userIdentity = null;

            $scope.login = function () {
                authService.login($scope.userName, $scope.password)
                    .then(function (result) {
                        $scope.userIdentity = result;
                        $location.path('/home');
                    }, function (error) {
                        feedbackSrvc.showAlertInfo("Invalid credentials**");
                    });
            };

            $scope.cancel = function () {
                $scope.userName = "";
                $scope.password = "";
            };
        }
    ]
    );