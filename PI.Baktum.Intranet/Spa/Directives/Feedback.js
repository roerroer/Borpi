angular.module('appfeedback', [])
    .directive('appFeedback', ['feedbackSrvc', function (feedbackSrvc) {
    return {
        link: function (scope, elm, attrs) {
            scope.feedbackLog = [];

            scope.isTriggered = function () {
                return feedbackSrvc.isTriggered();
            };

            scope.$watch(scope.isTriggered, function (isTriggered) {
                scope.feedbackLog = feedbackSrvc.getFeedback();
            });

            scope.$on('feedbackOn', function (event, args) {
                scope.feedbackLog = feedbackSrvc.getFeedback();
            });

            scope.closeAppPopup = function () {
                feedbackSrvc.flush();
            };
        },
        template:
            '<div class="message-popup-box box-feedback" style="right: 0px;" ng-show="feedbackLog.length>0">'+
            '<div class="popup-header box-header box-feedback">' +
            '<span class="glyphicon glyphicon-exclamation-sign" font-size 20px;></span> Aviso(s) del Sistema' +
            '<span class="glyphicon glyphicon-remove close-popup-box" ng-click="closeAppPopup()"></span>' +
            '</div>' +
            '<div class="popup-body">' +
            '<table class="table table-striped tbl-text-warns">' +
            '<tbody>' +
            '<tr ng-repeat="m in feedbackLog">' +
            '<td  ng-bind="m">' +
            '</td>' +
            '</tr>' +
            '</tbody>' +
            '</table>' +
            '</div>' +
            '</div>' 
    };
}]);
