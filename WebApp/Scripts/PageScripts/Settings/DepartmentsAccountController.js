

app.controller('DepartmentsAccountController', function ($scope, $uibModal, GeneralService, RequestService, DepartmentsAccountManager) {
    $scope.loadDepartmentsAccounts = function () {
        GeneralService.getAllDepartmentsAccount().then(function (response) {
            $scope.DepartmentsAccounts = response.data;
        });
    }
    $scope.loadPayingEntities = function () {
        RequestService.getInitialData().then(function (response) {
            $scope.PayingEntities = response.data.PayingEntities;
        });
    }

    $scope.loadDepartments = function () {
        GeneralService.getAllDepartments().then(function (response) {
            $scope.Departments = response.data;
        });
    }
 
    $scope.initialize = function () {
      
        $scope.DepartmentsAccounts = [];
        $scope.loadDepartmentsAccounts();
        $scope.PayingEntities = [];
        $scope.loadPayingEntities();
        $scope.Departments = [];
        $scope.loadDepartments();

    }
   

    $scope.enable = function (DepartmentsAccount) {
        DepartmentsAccountManager.enable(DepartmentsAccount).then(function (response) {
            $scope.initialize();
        });
    }

    $scope.disable = function (DepartmentsAccount) {
        DepartmentsAccountManager.disable(DepartmentsAccount).then(function (response) {
            $scope.initialize();
        });
    }



    $scope.newDepartmentsAccountDialog = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/DepartmentsAccount/NewDepartmentsAccount.html',
            controller: 'DepartmentsAccountPopupController',
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

    $scope.editDepartmentsAccountDialog = function (DepartmentsAccount) {
        $scope.editDepartmentsAccount = {
            Id: DepartmentsAccount.Id,
            Name: DepartmentsAccount.Name,
            Code: DepartmentsAccount.Code,
            Description: DepartmentsAccount.Description,
            PayingEntitiesId: DepartmentsAccount.PayingEntitiesId,
            DepartmentId: DepartmentsAccount.DepartmentId,
            PayingEntities: DepartmentsAccount.PayingEntities,
            Department: DepartmentsAccount.Department,
        };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/DepartmentsAccount/EditDepartmentsAccount.html',
            controller: 'DepartmentsAccountPopupController',
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
app.controller("DepartmentsAccountPopupController", function ($scope, $uibModalInstance, GeneralService,DepartmentsAccountManager) {
    $scope.create = function (newDepartmentsAccounts) {
       // var varPayingEntitiesId = newDepartmentsAccounts.Id.Id;
        newDepartmentsAccounts.PayingEntitiesId = $scope.request.PayingEntity.Id;
        var varDepartmentId = $scope.request.DepartmentId.Id;
        newDepartmentsAccounts.DepartmentId = varDepartmentId;
        newDepartmentsAccounts.Department = $scope.request.DepartmentId.Name;
        newDepartmentsAccounts.PayingEntities = $scope.request.PayingEntity.Name;
        DepartmentsAccountManager.createDepartmentsAccount(newDepartmentsAccounts).then(function (response) {
            $uibModalInstance.close(response.data);
        });
    };
    $scope.update = function (editDepartmentsAccount) {

        //var varPayingEntitiesId = editDepartmentsAccount.Id.Id;
        editDepartmentsAccount.PayingEntitiesId = $scope.request.PayingEntityEdit.Id;
        var varDepartmentId = $scope.request.DepartmentId.Id;
        editDepartmentsAccount.DepartmentId = varDepartmentId;
        editDepartmentsAccount.Department = $scope.request.DepartmentId.Name;
        editDepartmentsAccount.PayingEntities = $scope.request.PayingEntityEdit.Name;
        DepartmentsAccountManager.update(editDepartmentsAccount).then(function (response) {
            $uibModalInstance.close(response.data);
        });
    };
    $scope.close = function () {
        $uibModalInstance.close('close')
    }
    
    $scope.$watch('request.PayingEntity', function () {

        if ($scope.request !== undefined && $scope.request !== null)
            GeneralService.getDepartmentDetails($scope.request.PayingEntity).then(function (response) {
               
                $scope.Departments = response.data;
            });
       
    });
    $scope.$watch('request.PayingEntityEdit', function () {

        if ($scope.request !== undefined && $scope.request !== null)
            GeneralService.getDepartmentDetails($scope.request.PayingEntityEdit).then(function (response) {

                $scope.Departments = response.data;
            });

    });
});

