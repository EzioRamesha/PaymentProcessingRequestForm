app.service('DepartmentsAccountManager', function ($http) {
    this.createDepartmentsAccount = function (DepartmentsAccount) {
        var promise = $http({
            url: '/Settings/createDepartmentsAccount',
            method: 'POST',
            data: DepartmentsAccount
        });
        return promise;
    };

    this.update = function (DepartmentsAccount) {
        var promise = $http({
            url: '/Settings/UpdateDepartmentsAccount',
            data: DepartmentsAccount,
            method: 'POST'
        });
        return promise;
    };

    this.enable = function (DepartmentsAccount) {
        var promise = $http({
            url: '/Settings/EnableDepartmentsAccount',
            method: 'POST',
            data: DepartmentsAccount
        });
        return promise;
    };

    this.disable = function (DepartmentsAccount) {
        var promise = $http({
            url: '/Settings/DisableDepartmentsAccount',
            method: 'POST',
            data: DepartmentsAccount
        });
        return promise;
    };
});