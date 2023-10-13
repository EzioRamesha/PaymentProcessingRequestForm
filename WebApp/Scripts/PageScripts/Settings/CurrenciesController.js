app.service('CurrencyManager', function ($http) {
    this.create = function (currency) {
        var promise = $http({
            url: '/Settings/CreateCurrency',
            method: 'POST',
            data: currency
        });
        return promise;
    };
    this.update = function (currency) {
        var promise = $http({
            url: '/Settings/UpdateCurrency',
            method: 'POST',
            data: currency
        });
        return promise;
    };

    this.enable = function (currency) {
        var promise = $http({
            url: '/Settings/EnableCurrency',
            method: 'POST',
            data: currency
        });
        return promise;
    };

    this.disable = function (currency) {
        var promise = $http({
            url: '/Settings/DisableCurrency',
            method: 'POST',
            data: currency
        });
        return promise;
    };
});


app.controller('CurrencyController', function ($scope, $uibModal, GeneralService, CurrencyManager) {

    $scope.mask = function (value, precision) {
        return mask(value, precision);
    };
    $scope.unmask = function (value) {
        return unmask(value);
    };
    $scope.loadCurrencies = function () {
        GeneralService.getAllCurrencies().then(function (response) {
            $scope.Currencies = response.data;
        });
    }



    $scope.initialize = function () {
        $scope.Currencies = [];
        $scope.loadCurrencies();
    }


    $scope.enable = function (currency) {
        CurrencyManager.enable().then(function (response) {
            $scope.loadCurrencies();
        });
    }
    $scope.disable = function (currency) {
        CurrencyManager.disable().then(function (response) {
            $scope.loadCurrencies();
        });
    }

    $scope.newCurrencyDialog = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/Currency/NewCurrency.html',
            controller: 'NewCurrencyController',
            backdrop: 'static',
            scope: $scope
        }).result.then(function (response) {
            if (response && response.data && response.data.ResponseType === 'error')
                showError(response.data.Message);
            else if (response && response.data && response.data.ResponseType === 'success')
                showSuccess(response.data.Message);
            $scope.loadCurrencies();
        });
    };

    $scope.edit = function (currency) {
        $scope.editCurrency = {
            Id: currency.Id,
            Name: currency.Name,
            Code: currency.Code,
            USDValue: currency.USDValue === 0 ? "0.00000" : mask(currency.USDValue, 5),
            EuroValue: currency.EuroValue === 0 ? "0.00000" : mask(currency.EuroValue, 5)
        };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/Currency/EditCurrency.html',
            controller: 'EditCurrencyController',
            backdrop: 'static',
            scope: $scope
        }).result.then(function (response) {
            if (response && response.data && response.data.ResponseType === 'error')
                showError(response.data.Message);
            else if (response && response.data && response.data.ResponseType === 'success')
                showSuccess(response.data.Message);
            $scope.loadCurrencies();
        });
    };
});

app.controller('NewCurrencyController', function ($scope, $uibModalInstance, CurrencyManager) {
    $scope.newCurrency = {
        USDValue: "0.000000",
        EuroValue: "0.000000"
    }
    $scope.saveCurrency = function (newCurrency) {
        CurrencyManager.create(newCurrency).then(function (response) {
            $uibModalInstance.close(response);
        });
    };
    $scope.reset = function () {
        $scope.newCurrency = null;
    };
    $scope.close = function () {
        $uibModalInstance.close('close');
    };
});

app.controller('EditCurrencyController', function ($scope, $uibModalInstance, CurrencyManager) {
    $scope.update = function (currency) {
        $scope.editCurrency = currency;
        CurrencyManager.update(currency).then(function (response) {
            $uibModalInstance.close(response);
        });
    };
    $scope.close = function () {
        $uibModalInstance.close('close');
    }
});