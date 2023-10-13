app.service('PayeeManager', function ($http) {
    this.create = function (payee) {
        var promise = $http({
            url: '/Settings/CreatePayee',
            method: 'POST',
            data: payee
        });
        return promise;
    };

    this.savePayeeBank = function (payee) {
        var promise = $http({
            url: '/Settings/AddOrUpdatePayeeBank',
            method: 'POST',
            data: payee
        });
        return promise;
    };
    this.removePayeeBank = function (payee) {
        var promise = $http({
            url: '/Settings/RemovePayeeBank',
            method: 'POST',
            data: payee
        });
        return promise;
    }

    this.update = function (payee) {
        var promise = $http({
            url: '/Settings/UpdatePayee',
            method: 'POST',
            data: payee
        });
        return promise;
    };
    this.getBankAccounts = function (payee) {
        var promise = $http({
            url: '/Settings/GetPayeeAccountDetails',
            method: 'POST',
            data: payee
        });
        return promise;
    }

    this.getDepartmentDetails = function (PayingEntity) {
        var promise = $http({
            url: '/Settings/GetDepartmentDetails',
            method: 'POST',
            data: PayingEntity
        });
        return promise;
    }

    this.updateBankAccountDetails = function (payeeDetails) {
        var promise = $http({
            url: '/Settings/UpdatePayeeBankDetails',
            method: 'POST',
            data: payeeDetails
        });
        return promise;
    };

    this.enable = function (payee) {
        var promise = $http({
            url: '/Settings/EnablePayee',
            method: 'POST',
            data: payee
        });
        return promise;
    };

    this.disable = function (payee) {
        var promise = $http({
            url: '/Settings/DisablePayee',
            method: 'POST',
            data: payee
        });
        return promise;
    };
});