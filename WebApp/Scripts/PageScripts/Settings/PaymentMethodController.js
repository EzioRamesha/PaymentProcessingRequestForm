
app.controller("PaymentMethodController", function ($scope, $uibModal, PaymentMethodManager, GeneralService) {
    $scope.initialize = function () {
        $scope.getPaymentMethods();
    };

    $scope.getPaymentMethods = function () {
        GeneralService.getAllPaymentMethods().then(function (response) {
            $scope.PaymentMethods = response.data;
        });
    };

    $scope.enable = function (paymentMethod) {
        PaymentMethodManager.enable(paymentMethod).then(function (response) {
            if (response.data)
                $scope.getPaymentMethods();
        });
    };

    $scope.disable = function (paymentMethod) {
        PaymentMethodManager.disable(paymentMethod).then(function (response) {
            if (response.data)
                $scope.getPaymentMethods();
        });
    };


    $scope.showNewPaymentMethodDialog = function () {
        $scope.newPaymentMethod = null;
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/PaymentMethod/NewPaymentMethod.html',
            controller: 'PaymentMethodPopupController',
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

    $scope.showEditPaymentMethodDialog = function (paymentMethod) {
        $scope.editPaymentMethod = {
            Id: paymentMethod.Id,
            Name: paymentMethod.Name,
            Description: paymentMethod.Description
        };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/PaymentMethod/EditPaymentMethod.html',
            controller: 'PaymentMethodPopupController',
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


app.controller("PaymentMethodPopupController", function ($scope, $uibModalInstance, PaymentMethodManager) {
    $scope.create = function (paymentMethod) {
        PaymentMethodManager.create(paymentMethod).then(function (response) {
            $uibModalInstance.close(response.data);
        });
    };
    $scope.update = function (paymentMethod) {
        PaymentMethodManager.update(paymentMethod).then(function (response) {
            $uibModalInstance.close(response.data);
        });
    };
    $scope.close = function () {
        $uibModalInstance.close('close');
    }
});