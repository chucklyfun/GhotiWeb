angular.module('routerApp').factory("Global", [function (angular) {
    var _this = this;
    _this._data = {
        user: window.user,
        authenticated: !!window.user
    };

    return _this._data;
}]);