app.controller('PayeePopupController', function ($scope, $uibModalInstance, PayeeManager) {
    $scope.create = function (newPayee) {
        PayeeManager.create(newPayee).then(function (response) {
            $uibModalInstance.close(response.data);
            //$scope.initialize();
            //if (response.data.ResponseType === "success") {
            //    showSuccess(response.data.Message);
            //} else if (response.data.ResponseType === "error") {
            //    showError(response.data.Message);
            //}
            //$scope.close();
        });
    }
    $scope.update = function (updatePayee) {
        PayeeManager.update(updatePayee).then(function (response) {
            $uibModalInstance.close(response.data);
            //$scope.initialize();
            //if (response.data.ResponseType === "success") {
            //    showSuccess(response.data.Message);
            //} else if (response.data.ResponseType === "error") {
            //    showError(response.data.Message);
            //}
            //$scope.close();
        });
    }
    $scope.reset = function () {
        $scope.newPayee = null;
        $scope.editPayee = null;
    }
    $scope.close = function () {
        $uibModalInstance.close('close');
    }
});