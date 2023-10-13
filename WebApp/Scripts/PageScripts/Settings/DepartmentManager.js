app.service('DepartmentManager', function ($http) {
    this.createDepartment = function (Department) {
        var promise = $http({
            url: '/Settings/CreateDepartment',
            method: 'POST',
            data: Department
        });
        return promise;
    };

    this.update = function (Department) {
        var promise = $http({
            url: '/Settings/UpdateDepartment',
            data: Department,
            method: 'POST'
        });
        return promise;
    };

    this.enable = function (Department) {
        var promise = $http({
            url: '/Settings/EnableDepartment',
            method: 'POST',
            data: Department
        });
        return promise;
    };

    this.disable = function (Department) {
        var promise = $http({
            url: '/Settings/DisableDepartment',
            method: 'POST',
            data: Department
        });
        return promise;
    };
});