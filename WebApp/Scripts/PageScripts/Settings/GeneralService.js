app.service("GeneralService", function ($http) {
    this.getAllUsers = function () {
        var promise = $http({
            url: '/Settings/GetUsers',
            method: 'GET',
        });
        return promise;
    };
    this.getAllDepartments = function () {
        var promise = $http({
            url: '/Settings/GetDepartments',
            method: 'GET'
        });
        return promise;
    };
    this.getDepartmentDetails = function (PayingEntity) {
        var promise = $http({
            url: '/Settings/GetDepartmentDetails',
            method: 'POST',
            data: PayingEntity
        });
        return promise;
    }
    this.getAllDepartmentsAccount = function () {
        var promise = $http({
            url: '/Settings/GetDepartmentsAccount',
            method: 'GET'
        });
        return promise;
    };

   
    this.getDepartmentsAccount = function (DepartmentId) {
        var promise = $http({
            url: '/Settings/GetDepartmentsAccountByDeptId',
            method: 'POST',
            data: DepartmentId
        });
        return promise;
    }
    this.createDepartments = function () {
        var promise = $http({
            url: '/Settings/CreateDepartment',
            method: 'GET'
        });
        return promise;
    };
    this.getAllCountries = function () {
        var promise = $http({
            url: '/Settings/GetCountries',
            method: 'GET'
        });
        return promise;
    };
    this.getAllCurrencies = function () {
        var promise = $http({
            url: '/Settings/GetCurrencies',
            method: 'GET'
        });
        return promise;
    };
    this.getAllPayingEntities = function (searchCriteria) {
        var promise = $http({
            url: '/Settings/GetPayingEntities',
            method: 'POST',
            data: searchCriteria
        });
        return promise;
    };
    this.getAllPayees = function (searchCriteria) {
        var promise = $http({
            url: '/Settings/GetPayees',
            method: 'POST',
            data: searchCriteria
        });
        return promise;
    };

    this.getAllPayeesByname = function (searchCriteria, PayeeName) {
        var promise = $http({
            url: '/Settings/GetPayeesByName',
            method: 'POST',
            data: { model: searchCriteria, PayeeName: PayeeName }
        });
        return promise;
    };
    this.GetActivePayees = function () {
        var promise = $http({
            url: '/Settings/GetActivePayees',
            method: 'GET',
        });
        return promise;
    };
    this.GetActiveCountries = function () {
        var promise = $http({
            url: '/Settings/GetActiveCountries',
            method: 'GET'
        });
        return promise;
    };

    this.getAllExpenseTypes = function () {
        var promise = $http({
            url: '/Settings/GetExpenseTypes',
            method: 'GET'
        });
        return promise;
    };

    this.getAllFrequencyTypes = function () {
        var promise = $http({
            url: '/Settings/GetFrequencyTypes',
            method: 'GET'
        });
        return promise;
    };
    this.getAllPaymentMethods = function () {
        var promise = $http({
            url: '/Settings/GetPaymentMethods',
            method: 'GET'
        });
        return promise;
    };
});