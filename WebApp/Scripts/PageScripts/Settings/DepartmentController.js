

app.controller('DepartmentController', function ($scope, $uibModal, GeneralService, RequestService, DepartmentManager) {
    $scope.loadDepartments = function () {
        GeneralService.getAllDepartments().then(function (response) {
            $scope.Departments = response.data;
        });
    }
        $scope.loadPayingEntities = function () {
            RequestService.getInitialData().then(function (response) {
                $scope.PayingEntities = response.data.PayingEntities;
            });     
          }
 
    $scope.initialize = function () {
      
        $scope.Departments = [];
        $scope.loadDepartments();
        $scope.PayingEntities = [];
        $scope.loadPayingEntities();
        
       
    }
   

    $scope.enable = function (Department) {
        DepartmentManager.enable(Department).then(function (response) {
            $scope.loadDepartments();
        });
    }

    $scope.disable = function (Department) {
        DepartmentManager.disable(Department).then(function (response) {
            $scope.loadDepartments();
        });
    }



    $scope.newDepartmentDialog = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/Department/NewDepartment.html',
            controller: 'DepartmentPopupController',
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

    $scope.editCountryDialog = function (Department) {
        $scope.editDepartment = {
            Id: Department.Id,
            Name: Department.Name,
            Code: Department.Code,
            Description: Department.Description,
            PayingEntitiesId: Department.PayingEntitiesId,
            PayingEntities: Department.PayingEntities,
        };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/Department/EditDepartment.html',
            controller: 'DepartmentPopupController',
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

//app.controller('DepartmentPopupController', function ($scope, $uibModalInstance, DepartmentManager) {
//    $scope.reset = function (newDepartment) {
//        debugger;
       
//    }
//    $scope.close = function () {
//        $uibModalInstance.close('close');
//    };
//    $scope.update = function (editDepartment) {
//        debugger;
        
//    }
//});

app.controller("DepartmentPopupController", function ($scope, $uibModalInstance, DepartmentManager) {
    $scope.create = function (newDepartment) {
        var varid = newDepartment.Id.Id;
        var varPayingEntities = newDepartment.Id.Name;
        newDepartment.PayingEntitiesId = varid;
        newDepartment.PayingEntities = varPayingEntities;
        DepartmentManager.createDepartment(newDepartment).then(function (response) {
            $uibModalInstance.close(response.data);
        });
    };
    $scope.update = function (editDepartment) {

        var varid = editDepartment.PayingEntitiesId.Id;
        var varPayingEntities = editDepartment.PayingEntitiesId.Name;
        editDepartment.PayingEntitiesId = varid;
        editDepartment.PayingEntities = varPayingEntities;
        DepartmentManager.update(editDepartment).then(function (response) {
            $uibModalInstance.close(response.data);
        });
    };
    $scope.close = function () {
        $uibModalInstance.close('close')
    }
});

