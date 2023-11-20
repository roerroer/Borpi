yumKaaxControllers.controller("authCtrl",
    ["$scope", "$location", "feedbackSrvc", "authService",
        function ($scope, $location, feedbackSrvc, authService) {
            $scope.userIdentity = null;

            $scope.login = function () {
                authService.login($scope.userName, $scope.password)
                    .then(function (result) {
                        $scope.userIdentity = result;
                        $location.path("/");
                    }, function (error) {
                        feedbackSrvc.showAlertInfo("Invalid credentials**");
                        console.log(error);
                    });
            };

            $scope.cancel = function () {
                $scope.userName = "";
                $scope.password = "";
            };
        }
    ]);