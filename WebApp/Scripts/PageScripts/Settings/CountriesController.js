

app.controller('CountryController', function ($scope, $uibModal, GeneralService, CountryManager) {
    $scope.loadCountries = function () {
        GeneralService.getAllCountries().then(function (response) {
            $scope.Countries = response.data;
        });
    }

    $scope.initialize = function () {
        $scope.Countries = [];
        $scope.loadCountries();
    }


    $scope.enable = function (country) {
        CountryManager.enable(country).then(function (response) {
            $scope.loadCountries();
        });
    }

    $scope.disable = function (country) {
        CountryManager.disable(country).then(function (response) {
            $scope.loadCountries();
        });
    }



    $scope.newCountryDialog = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/Country/NewCountry.html',
            controller: 'CountryPopupController',
            backdrop: 'static',
            scope: $scope
        }).result.then(function (response) {
            if (response.ResponseType)
                if (response.ResponseType === 'success') {
                    $scope.initialize();
                    showSuccess(response.Message);
                }
                else if (response.ResponseType === 'error')
                    showError(response.Message)
        });
    };

    $scope.editCountryDialog = function (country) {
        $scope.editCountry = {
            Name: country.Name,
            Code: country.Code,
        };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/Country/EditCountry.html',
            controller: 'CountryPopupController',
            backdrop: 'static',
            scope: $scope
        }).result.then(function (response) {
            if (response.ResponseType)
                if (response.ResponseType === 'success') {
                    $scope.initialize();
                    showSuccess(response.Message);
                }
                else if (response.ResponseType === 'error')
                    showError(response.Message)
        });
    };
});

app.controller('CountryPopupController', function ($scope, $uibModalInstance, CountryManager) {
    $scope.reset = function (newCountry) {
        debugger;
        //CountryManager.create(newCountry).then(function (response) {
        //    $uibModalInstance.close(response.data);
        //});
    }
    $scope.close = function () {
        $uibModalInstance.close('close');
    };
    $scope.update = function (editCountry) {
        debugger;
        //CountryManager.update(editCountry).then(function (response) {
        //    $uibModalInstance.close(response.data);
        //});
    }
});

//app.controller('EditCountryController', function ($scope, $uibModalInstance) {
//    $scope.reset = function () {
//        $scope.newCountry = null;
//    }
//    $scope.close = function () {
//        $uibModalInstance.close('close');
//    }
//});