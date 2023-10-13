app.service('ExpenseTypeManager', function ($http) {
    this.create = function (expenseType) {
        var promise = $http({
            url: '/Settings/CreateExpenseType',
            data: expenseType,
            method: 'POST'
        });
        return promise;
    };

    this.update = function (expenseType) {
        var promise = $http({
            url: '/Settings/UpdateExpenseType',
            data: expenseType,
            method: 'POST'
        });
        return promise;
    };

    this.enable = function (expenseType) {
        var promise = $http({
            url: '/Settings/EnableExpenseType',
            data: expenseType,
            method: 'POST'
        });
        return promise;
    };

    this.disable = function (expenseType) {
        var promise = $http({
            url: '/Settings/DisableExpenseType',
            data: expenseType,
            method: 'POST'
        });
        return promise;
    };
});