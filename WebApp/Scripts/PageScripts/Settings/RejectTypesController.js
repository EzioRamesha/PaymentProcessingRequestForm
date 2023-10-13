
app.service('RejectTypeService', function ($http, Upload) {
   

    this.CreateClosePPRFReasonTypes = function (model) {
        var promise = $http({
            url: '/Settings/CreateClosePPRFReasonTypes',
            data: model,
            method: 'POST'
        });
        return promise;
    };

    this.CreateCancelPPRFReasonTypes = function (model) {
        var promise = $http({
            url: '/Settings/CreateCancelPPRFReasonTypes',
            data: model,
            method: 'POST'
        });
        return promise;
    };

    this.CreateRejectType = function (model) {
        var promise = $http({
            url: '/Settings/CreateRejectType',
            data: model,
            method: 'POST'
        });
        return promise;
    };


    this.ClosePPRFReasonTypesupdate = function (model) {
        var promise = $http({
            url: '/Settings/ClosePPRFReasonTypesupdate',
            data: model,
            method: 'POST'
        });
        return promise;
    };

    this.CancelPPRFReasonTypesupdate = function (model) {
        var promise = $http({
            url: '/Settings/CancelPPRFReasonTypesupdate',
            data: model,
            method: 'POST'
        });
        return promise;
    };

    this.RejectTypeupdate = function (model) {
        var promise = $http({
            url: '/Settings/RejectTypeupdate',
            data: model,
            method: 'POST'
        });
        return promise;
    };

    this.RejectTypeEnable = function (model) {
        var promise = $http({
            url: '/Settings/EnableRejectType',
            method: 'POST',
            data: model
        });
        return promise;
    };

    this.RejectTypeDisable = function (model) {
        var promise = $http({
            url: '/Settings/DisableRejectType',
            method: 'POST',
            data: model
        });
        return promise;
    };

    this.CancelPPRFReasonTypesEnable = function (model) {
        var promise = $http({
            url: '/Settings/CancelPPRFReasonTypesEnable',
            method: 'POST',
            data: model
        });
        return promise;
    };

    this.CancelPPRFReasonTypesDisable = function (model) {
        var promise = $http({
            url: '/Settings/CancelPPRFReasonTypesDisable',
            method: 'POST',
            data: model
        });
        return promise;
    };
    this.ClosePPRFReasonTypesEnable = function (model) {
        var promise = $http({
            url: '/Settings/ClosePPRFReasonTypesEnable',
            method: 'POST',
            data: model
        });
        return promise;
    };

    this.ClosePPRFReasonTypesDisable = function (model) {
        var promise = $http({
            url: '/Settings/ClosePPRFReasonTypesDisable',
            method: 'POST',
            data: model
        });
        return promise;
    };

});


app.controller('RejectTypesController', function ($scope, $uibModal, RequestService, RejectTypeService) {
    $scope.initialize = function () {     
        $scope.ClosePPRFReasonTypes = [];
        $scope.loadClosePPRFReasonTypes();
        $scope.CancelPPRFReasonTypes = [];
        $scope.loadCancelPPRFReasonTypes();

        $scope.RejectTypes = [];
        $scope.loadRejectReasonTypes();
    }
    $scope.loadClosePPRFReasonTypes = function () {
        RequestService.getClosePPRFReasonTypes().then(function (response) {
            $scope.ClosePPRFReasonTypes = response.data;
        });
    }
    $scope.loadCancelPPRFReasonTypes = function () {
        RequestService.getCancelPPRFReasonTypes().then(function (response) {
            $scope.CancelPPRFReasonTypes = response.data;
        });
    }

    $scope.loadRejectReasonTypes = function () {
        RequestService.getRejectReasonTypes().then(function (response) {
            $scope.RejectTypes = response.data;
        });
    }

    $scope.RejectTypeEnable = function (RejectType) {
        RejectTypeService.RejectTypeEnable(RejectType).then(function (response) {
            $scope.initialize();
        });
    }

    $scope.RejectTypeDisable = function (RejectType) {
        RejectTypeService.RejectTypeDisable(RejectType).then(function (response) {
            $scope.initialize();
        });
    }



    $scope.CancelPPRFReasonTypesEnable = function (CancelPPRFReasonType) {
        RejectTypeService.CancelPPRFReasonTypesEnable(CancelPPRFReasonType).then(function (response) {
            $scope.initialize();
        });
    }

    $scope.CancelPPRFReasonTypesDisable = function (CancelPPRFReasonType) {
        RejectTypeService.CancelPPRFReasonTypesDisable(CancelPPRFReasonType).then(function (response) {
            $scope.initialize();
        });
    }

    $scope.ClosePPRFReasonTypesEnable = function (ClosePPRFReasonType) {
        RejectTypeService.ClosePPRFReasonTypesEnable(ClosePPRFReasonType).then(function (response) {
            $scope.initialize();
        });
    }

    $scope.ClosePPRFReasonTypesDisable = function (ClosePPRFReasonType) {
        RejectTypeService.ClosePPRFReasonTypesDisable(ClosePPRFReasonType).then(function (response) {
            $scope.initialize();
        });
    }



    $scope.ClosePPRFReasonTypeDialog = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/Reject Type/NewClosePPRFReasonTypes.html',
            controller: 'ClosePPRFReasonTypePopupController',
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

    $scope.CancelPPRFReasonTypeDialog = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/Reject Type/NewCancelPPRFReasonTypes.html',
            controller: 'CancelPPRFReasonTypePopupController',
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

    $scope.RejectReasonTypesDialog = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/Reject Type/NewRejectTypes.html',
            controller: 'RejectReasonTypesPopupController',
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

    $scope.editClosePPRFReasonTypesDialog = function (ClosePPRFReasonTypes) {
        $scope.editClosePPRFReasonTypes = {
            Id: ClosePPRFReasonTypes.Id,
            Description: ClosePPRFReasonTypes.Description,
        };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/Reject Type/EditClosePPRFReasonTypes.html',
            controller: 'ClosePPRFReasonTypePopupController',
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
    $scope.CancelPPRFReasonTypesDialog = function (CancelPPRFReasonTypes) {
        $scope.editCancelPPRFReasonTypes = {
            Id: CancelPPRFReasonTypes.Id,
            Description: CancelPPRFReasonTypes.Description,
        };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/Reject Type/EditCancelPPRFReasonTypes.html',
            controller: 'CancelPPRFReasonTypePopupController',
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
    $scope.EditRejectTypesDialog = function (EditRejectTypes) {
        $scope.editEditRejectTypes = {
            Id: EditRejectTypes.Id,
            Description: EditRejectTypes.Description,
        };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/Reject Type/EditRejectTypes.html',
            controller: 'RejectReasonTypesPopupController',
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

app.controller("ClosePPRFReasonTypePopupController", function ($scope, RejectTypeService, $uibModalInstance) {
    $scope.ClosePPRFReasonTypescreate = function (newClosePPRFReasonTypes) {
              
        RejectTypeService.CreateClosePPRFReasonTypes(newClosePPRFReasonTypes).then(function (response) {
            $uibModalInstance.close(response.data);
        });
    };
    $scope.editClosePPRFReasonTypesupdate = function (editClosePPRFReasonTypes) {       
        RejectTypeService.ClosePPRFReasonTypesupdate(editClosePPRFReasonTypes).then(function (response) {
            $uibModalInstance.close(response.data);
        });
    };
    $scope.editClosePPRFReasonTypesclose = function () {
        $uibModalInstance.close('close')
    }
    $scope.ClosePPRFReasonTypesclose = function () {
        $uibModalInstance.close('close')
    }
});
app.controller("CancelPPRFReasonTypePopupController", function ($scope, RejectTypeService, $uibModalInstance) {
    $scope.CancelPPRFReasonTypescreate = function (newCancelPPRFReasonTypes) {

        RejectTypeService.CreateCancelPPRFReasonTypes(newCancelPPRFReasonTypes).then(function (response) {
            $uibModalInstance.close(response.data);
        });
    };
    $scope.editCancelPPRFReasonTypesupdate = function (editDepartment) {

        RejectTypeService.CancelPPRFReasonTypesupdate(editDepartment).then(function (response) {
            $uibModalInstance.close(response.data);
        });
    };
    $scope.editCancelPPRFReasonTypesclose = function () {
        $uibModalInstance.close('close')
    }
    $scope.CancelPPRFReasonTypesclose = function () {
        $uibModalInstance.close('close')
    }
});
app.controller("RejectReasonTypesPopupController", function ($scope, RejectTypeService,$uibModalInstance) {
    $scope.RejectTypescreate = function (newDepartment) {

        RejectTypeService.CreateRejectType(newDepartment).then(function (response) {
            $uibModalInstance.close(response.data);
        });
    };
    $scope.editRejectTypesupdate = function (editDepartment) {

      
        RejectTypeService.RejectTypeupdate(editDepartment).then(function (response) {
            $uibModalInstance.close(response.data);
        });
    };
    $scope.editRejectTypesclose = function () {
        $uibModalInstance.close('close')
    }
    $scope.RejectTypesclose = function () {
        $uibModalInstance.close('close')
    }
});

