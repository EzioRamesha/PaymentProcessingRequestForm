
app.controller("FrequencyTypeController", function ($scope, $uibModal, FrequencyTypeManager, GeneralService) {
    $scope.initialize = function () {
        $scope.getFrequencyTypes();
    };

    $scope.getFrequencyTypes = function () {
        GeneralService.getAllFrequencyTypes().then(function (response) {
            $scope.FrequencyTypes = response.data;
        });
    };

    $scope.enable = function (frequencyType) {
        FrequencyTypeManager.enable(frequencyType).then(function (response) {
            if (response.data)
                $scope.getFrequencyTypes();
        });
    };

    $scope.disable = function (frequencyType) {
        FrequencyTypeManager.disable(frequencyType).then(function (response) {
            if (response.data)
                $scope.getFrequencyTypes();
        });
    };


    $scope.showNewFrequencyTypeDialog = function () {
        $scope.newFrequencyType = null;
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/FrequencyType/NewFrequencyType.html',
            controller: 'FrequencyTypePopupController',
            backdrop: 'static',
            scope: $scope
        }).result.then(function (response) {
            if (response.ResponseType)
                if (response.ResponseType === "success") {
                    $scope.initialize();
                    showSuccess(response.Message);
                } else if (response.ResponseType === "error") {
                    showError(response.Message);
                }
        });
    };

    $scope.showEditFrequencyTypeDialog = function (frequencyType) {
        $scope.editFrequencyType = {
            Id: frequencyType.Id,
            Name: frequencyType.Name,
            Description: frequencyType.Description
        };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/FrequencyType/EditFrequencyType.html',
            controller: 'FrequencyTypePopupController',
            backdrop: 'static',
            scope: $scope
        }).result.then(function (response) {
            if (response.ResponseType)
                if (response.ResponseType === "success") {
                    $scope.initialize();
                    showSuccess(response.Message);
                } else if (response.ResponseType === "error") {
                    showError(response.Message);
                }
        });
    };
});


app.controller("FrequencyTypePopupController", function ($scope, $uibModalInstance, FrequencyTypeManager) {
    $scope.create = function (frequencyType) {
        FrequencyTypeManager.create(frequencyType).then(function (response) {
            $uibModalInstance.close(response.data);
        });
    };
    $scope.update = function (frequencyType) {
        FrequencyTypeManager.update(frequencyType).then(function (response) {
            $uibModalInstance.close(response.data);
        });
    };
    $scope.close = function () {
        $uibModalInstance.close('close')
    }
});