
app.controller("ExpenseTypeController", function ($scope, $uibModal, ExpenseTypeManager, GeneralService) {
    $scope.initialize = function () {
        $scope.getExpenseTypes();
    };

    $scope.getExpenseTypes = function () {
        GeneralService.getAllExpenseTypes().then(function (response) {
            $scope.ExpenseTypes = response.data;
        });
    };

    $scope.enable = function (expenseType) {
        ExpenseTypeManager.enable(expenseType).then(function (response) {
            if (response.data)
                $scope.getExpenseTypes();
        });
    };

    $scope.disable = function (expenseType) {
        ExpenseTypeManager.disable(expenseType).then(function (response) {
            if (response.data)
                $scope.getExpenseTypes();
        });
    };


    $scope.showNewExpenseTypeDialog = function () {
        $scope.newExpenseType = null;
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/ExpenseType/NewExpenseType.html',
            controller: 'ExpenseTypePopupController',
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

    $scope.showEditExpenseTypeDialog = function (expenseType) {
        $scope.editExpenseType = {
            Id: expenseType.Id,
            Name: expenseType.Name,
            Description: expenseType.Description
        };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/ExpenseType/EditExpenseType.html',
            controller: 'ExpenseTypePopupController',
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


app.controller("ExpenseTypePopupController", function ($scope, $uibModalInstance, ExpenseTypeManager) {
    $scope.create = function (expenseType) {
        ExpenseTypeManager.create(expenseType).then(function (response) {
            $uibModalInstance.close(response.data);
        });
    };
    $scope.update = function (expenseType) {
        ExpenseTypeManager.update(expenseType).then(function (response) {
            $uibModalInstance.close(response.data);
        });
    };
    $scope.close = function () {
        $uibModalInstance.close('close');
    }
});