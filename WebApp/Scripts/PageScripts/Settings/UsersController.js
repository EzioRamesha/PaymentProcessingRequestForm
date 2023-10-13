/// <reference path="../../angular.js" />

app.service("UserManager", function ($http) {
    this.create = function (user) {
        var promise = $http({
            url: '/Settings/CreateUser',
            method: 'POST',
            data: user
        });
        return promise;
    };

    this.update = function (user) {
        var promise = $http({
            url: '/Settings/UpdateUser',
            method: 'POST',
            data: user
        });
        return promise;
    }

    this.enable = function (user) {
        var promise = $http({
            url: '/Settings/EnableUser',
            method: 'POST',
            data: user
        });
        return promise;
    };

    this.disable = function (user) {
        var promise = $http({
            url: '/Settings/DisableUser',
            method: 'POST',
            data: user
        });
        return promise;
    };

    this.getUserGroups = function () {
        var promise = $http({
            url: '/Settings/GetActiveAppGroups',
            method: 'GET'
        });
        return promise;
    };
    this.getUserPermissions = function () {
        var promise = $http({
            url: '/Settings/GetActivePermissions',
            method: 'GET'
        });
        return promise;
    };
    this.getUserRoles = function () {
        var promise = $http({
            url: '/Settings/GetActiveRoles',
            method: 'GET'
        });
        return promise;
    };

    this.getUserByEmail = function (user) {
        var promise = $http({
            url: '/Settings/GetUserByEmail',
            method: 'POST',
            data: user
        });
        return promise;
    };

    this.resetPassword = function (userEmail) {
        var promise = $http({
            url: '/Settings/ResetPassword',
            method: 'POST',
            data: {userEmail: userEmail}
        });
        return promise;
    }
});

app.controller('UserController', function ($scope, $uibModal, GeneralService, UserManager, $filter) {
    $scope.initialize = function () {
        $scope.Users = [];
        GeneralService.getAllUsers().then(function (response) {
            $scope.Users = response.data.Data;
        });
    }

    $scope.newUserDialog = function () {
        GeneralService.getAllDepartments().then(function (response) {
            $scope.Departments = response.data;
        });
        UserManager.getUserGroups().then(function (response) {
            if ($scope.newUser === undefined)
                $scope.newUser = {};
            $scope.newUser.SelectedGroups = response.data;
        });
        UserManager.getUserRoles().then(function (response) {
            $scope.Roles = response.data;
            if ($scope.newUser === undefined)
                $scope.newUser = {};
            var role = $filter('filter')($scope.Roles, { Name: 'Operator' })[0];
            $scope.newUser.Role = role;
        });
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/User/NewUser.html',
            controller: 'NewUserController',
            backdrop: 'static',
            scope: $scope
        });
        modalInstance.result.then(function (response) {
            if (response === 'close') { }
            else if (response.data.ResponseType === "error") {
                showError(response.data.Message);
            } else if (response.data.ResponseType === "success") {
                $scope.initialize();
                showSuccess(response.data.Message);
            }
        });
    }

    $scope.editUserDialog = function (user) {
        if (user != undefined && user != null) {
            UserManager.getUserByEmail(user).then(function (response) {
                $scope.editUser = response.data;
                GeneralService.getAllDepartments().then(function (response) {
                    $scope.Departments = response.data;
                    $scope.editUser.Department = $.grep($scope.Departments, function (obj, idx) {
                        return obj.Id === user.Department.Id;
                    })[0];
                    if ($scope.editUser.SelectedGroups.length == 0) {
                        UserManager.getUserGroups().then(function (response) {
                            $scope.editUser.SelectedGroups = response.data;
                        });
                    }
                    if ($scope.editUser.Permissions.length == 0) {
                        UserManager.getUserPermissions().then(function (response) {
                            $scope.editUser.Permissions = response.data;
                        });
                    }
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: '/Templates/User/EditUser.html',
                        controller: 'EditUserController',
                        backdrop: 'static',
                        scope: $scope
                    });
                    modalInstance.result.then(function (response) {
                        if (response === 'close') { }
                        else if (response.data.ResponseType === "error") {
                            showError(response.data.Message);
                        } else {
                            $scope.initialize();
                            showSuccess(response.data.Message);
                        }
                    });
                });
            });

        }
    }

    $scope.enable = function (user) {
        UserManager.enable(user).then(function (response) {
            $scope.initialize();
        });
    };
    $scope.disable = function (user) {
        UserManager.disable(user).then(function (response) {
            $scope.initialize();
        });
    };

    $scope.resetPassword = function (userEmail) {
        UserManager.resetPassword(userEmail).then(function (response) {
            if (response.data.Type === 'success')
                alert('Successfully changed the password. New password is: ' + response.data.NewPassword);
            else showError('Something went wrong while processing your request.');
        });
    };
});

app.controller('NewUserController', function ($scope, $uibModalInstance, UserManager, $filter) {
    $scope.create = function (newUser) {
        UserManager.create(newUser).then(function (response) {
            $uibModalInstance.close(response);
        });
    };
    $scope.reset = function () {
        $scope.newUser = {};
        var role = $filter('filter')($scope.Roles, { Name: 'Operator' })[0];
        $scope.newUser.Role = role;
    };
    $scope.close = function () {
        $uibModalInstance.close('close');
    };
});


app.controller('EditUserController', function ($scope, $uibModalInstance, UserManager) {
    $scope.update = function (editUser) {
        UserManager.update(editUser).then(function (response) {
            $uibModalInstance.close(response);
        });
    };
    $scope.reset = function () {
        $scope.newUser = {};
    };
    $scope.close = function () {
        $uibModalInstance.close('close');
    };
});