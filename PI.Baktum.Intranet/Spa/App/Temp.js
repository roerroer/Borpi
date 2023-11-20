yumKaaxApp.filter("dateformat", function () {
    var re = /\\\/Date\(([0-9]*)\)\\\//;
    return function (x) {
        var m = x.match(re);
        if (m) return new Date(parseInt(m[1]));
        else return null;
    };
});

yumKaaxApp.run(["$rootScope", "$location", "authService",
    function ($rootScope, $location, authService) {

    $rootScope.$on("$routeChangeSuccess", function (userInfo) {
        if ($location.path().indexOf("olvide-clave") >= 0)
            return;

        if (authService.getUserInfo() === null || authService.getUserInfo() === undefined) {
            $location.path("/login");
        }
    });

    $rootScope.$on("$routeChangeError", function (event, current, previous, eventObj) {
        if (eventObj.authenticated === false) {
            $location.path("/login");
        }
    });
}]);

/*
 * Interceptor
 * */
yumKaaxApp.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('sessionInjector');
}]);


Array.prototype.remove = function () {
    var what, a = arguments, L = a.length, ax;
    while (L && this.length) {
        what = a[--L];
        while ((ax = this.indexOf(what)) !== -1) {
            this.splice(ax, 1);
        }
    }
    return this;
};
