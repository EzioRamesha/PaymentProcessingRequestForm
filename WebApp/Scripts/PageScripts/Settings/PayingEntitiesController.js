/// <reference path="../../angular.js" />

//app.service('PayingEntityManager', function ($http, Upload) {
//    this.create = function (payingEntity) {
//        var promise = Upload.upload({
//            url: '/Settings/CreatePayingEntity',
//            method: 'POST',
//            data: payingEntity
//        });
//        return promise;
//    };
//    this.update = function (payingEntity) {
//        var promise = Upload.upload({
//            url: '/Settings/UpdatePayingEntity',
//            method: 'POST',
//            data: payingEntity
//        });
//        return promise;
//    };

//    this.enable = function (payingEntity) {
//        var promise = $http({
//            url: '/Settings/EnablePayingEntity',
//            method: 'POST',
//            data: payingEntity
//        });
//        return promise;
//    };

//    this.disable = function (payingEntity) {
//        var promise = $http({
//            url: '/Settings/DisablePayingEntity',
//            method: 'POST',
//            data: payingEntity
//        });
//        return promise;
//    };
//});

app.controller('PayingEntityController', function ($scope, $uibModal, PayingEntityManager, GeneralService) {

    var searchCriteria = {
        Pager: {
            ItemsPerPage: 20,
            CurrentPage: 1
        }
    };

    $scope.refresh = function (Pager) {
        searchCriteria.Pager = Pager;
        GeneralService.getAllPayingEntities(searchCriteria).then(function (response) {
            $scope.PayingEntities = response.data.PayingEntities;
            $scope.Pager = response.data.Pager;
        });
    }
    $scope.Pager = searchCriteria.Pager;
    $scope.initialize = function () {
        $scope.PayingEntities = [];
        $scope.refresh(searchCriteria.Pager);
    }

    $scope.newPayingEntityDialog = function () {
        $scope.newPayingEntity = null;
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/PayingEntity/NewPayingEntity.html',
            controller: 'NewPayingEntityController',
            backdrop: 'static',
            scope: $scope
        }).result.then(function (response) {
            if (response.data.ResponseType === "success") {
                $scope.initialize();
                showSuccess(response.data.Message);
            } else if (response.data.ResponseType === "error") {
                showError(response.data.Message);
            }
        });
    }

    $scope.editPayingEntityDialog = function (editPayingEntity) {
        $scope.editPayingEntity = {
            Id: editPayingEntity.Id,
            Name: editPayingEntity.Name,
            Abbreviation: editPayingEntity.Abbreviation,
            Description: editPayingEntity.Description,
            Logo: null
        };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/PayingEntity/EditPayingEntity.html',
            controller: 'EditPayingEntityController',
            backdrop: 'static',
            scope: $scope
        }).result.then(function (response) {
            if (response.data.ResponseType === "success") {
                $scope.initialize();
                showSuccess(response.data.Message);
            } else if (response.data.ResponseType === "error") {
                showError(response.data.Message);
            }
        });
    }

    $scope.enable = function (existingPayingEntity) {
        PayingEntityManager.enable(existingPayingEntity).then(function (response) {
            $scope.initialize();
        });
    }
    $scope.disable = function (existingPayingEntity) {
        PayingEntityManager.disable(existingPayingEntity).then(function (response) {
            $scope.initialize();
        });
    };


    $scope.editRange = function (existingPayingEntity) {
        PayingEntityManager.getExistingRanges(existingPayingEntity.Id).then(function (response) {
            if (response.data.ResponseType === "success") {
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: '/Templates/PayingEntity/EditAmountRange.html',
                    controller: 'PayingEntityAmountRangeController',
                    backdrop: 'static',
                    size: 'lg',
                    resolve: {
                        AmountRangeConfig: function () { return response.data.Data; }
                    }
                }).result.then(function (response) {
                    if (response !== null && response !== undefined && response.data !== null && response.data !== undefined) {
                        if (response.data.ResponseType === 'success') {
                            $scope.initialize();
                            showSuccess(response.data.Message);
                        } else if (response.data.ResponseType === 'error') {
                            showError(response.data.Message);
                        }
                    }
                    else if(response===null || response===undefined) {
                        showError('Something went wrong while processing your request.');
                        ////TODO: continue in the EditPayingEntityController for the range.
                    }
                });
            }
        });
    };
});

app.controller('NewPayingEntityController', function ($scope, $uibModalInstance, PayingEntityManager) {
    $scope.create = function (newPayingEntity) {
        PayingEntityManager.create(newPayingEntity).then(function (response) {
            $uibModalInstance.close(response);
        });
    }
    $scope.reset = function () {
        $scope.newPayingEntity = null;
    }
    $scope.close = function () {
        $uibModalInstance.close('close');
    }
});


app.controller('EditPayingEntityController', function ($scope, $uibModalInstance, PayingEntityManager) {
    $scope.update = function (editPayingEntity) {
        PayingEntityManager.update(editPayingEntity).then(function (response) {
            $uibModalInstance.close(response);
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
        $scope.editPayingEntity = null;
    }
    $scope.close = function () {
        $uibModalInstance.close('close');
    }
});


app.controller('PayingEntityAmountRangeController', function ($scope, $uibModalInstance, PayingEntityManager, AmountRangeConfig) {
    //debugger;
    
    $scope.init = function () {
        $scope.AmountRangeConfigItems = AmountRangeConfig.AmountRangeConfig;
        $scope.PayingEntity = AmountRangeConfig.PayingEntity;
        $scope.range = {
            AmountFrom: '0.00',
            AmountTo: '0.00',
            PayingEntityId: AmountRangeConfig.PayingEntity.Id
        };
    };

    $scope.addRange = function (range) {
        PayingEntityManager
            .SavePayingEntityAmountRange(range)
            .then(function (response) {
                var success = false;
                if (response !== null && response !== undefined) {
                    if (response.data.ResponseType !== null && response.data.ResponseType !== undefined) {
                        if (response.data.ResponseType === 'success') {
                            PayingEntityManager
                                .getExistingRanges(range.PayingEntityId)
                                .then(function (response) {
                                    var success1 = false;
                                    if (response !== null && response !== undefined) {
                                        if (response.data.ResponseType !== null && response.data.ResponseType !== undefined) {
                                            if (response.data.ResponseType === "success") {
                                                $scope.AmountRangeConfigItems = response.data.Data.AmountRangeConfig;
                                                $scope.PayingEntity = response.data.Data.PayingEntity;
                                                success1 = true;
                                            }
                                        }
                                    }
                                    if (!success1) showError("Error refreshing data");
                                    success = success && success1;
                                });
                        }
                        else showError(response.data.Message);
                    }
                }
                if (success) showSuccess(response.data.Message);
                //else showError(response.data.Message);
            });
    };

    $scope.addEmails = function (item) {
        item.EmailAddresses = item.Emails.replace(/\s/g, '').split(',');
        PayingEntityManager
            .SavePayingEntityAmountRangeEmails(item)
            .then(function (response) {
                var success = false;
                if (response !== null && response !== undefined) {
                    if (response.data.ResponseType !== null && response.data.ResponseType !== undefined) {
                        if (response.data.ResponseType === "success") {
                            success = true;
                            PayingEntityManager
                                .getRangeDetails(item.Id)
                                .then(function (response) {
                                    var success1 = false;
                                    if (response !== null && response !== undefined) {
                                        if (response.data.ResponseType !== null && response.data.ResponseType !== undefined) {
                                            if (response.data.ResponseType === "success") {
                                                item.EmailAddresses = response.data.Data.EmailAddresses;
                                                item.Emails = null;
                                                success1 = true;
                                            }
                                        }
                                    }
                                    if (!success || !success1) showError("Error refreshing data");
                                    success = success && success1;
                                });
                        }
                        else showError(response.data.Message);
                    }
                }
                if (success) showSuccess(response.data.Message);
                //else showError(response.data.Message);
            });
    };

    $scope.deleteEmail = function (email, item) {
        var rangeId = item.Id;
        PayingEntityManager.RemoveEmailFromRange(email, rangeId)
            .then(function (response) {
                var success = false;
                if (response !== null && response !== undefined) {
                    if (response.data.ResponseType !== null && response.data.ResponseType !== undefined) {
                        if (response.data.ResponseType === "success") {
                            PayingEntityManager
                                .getRangeDetails(rangeId)
                                .then(function (response) {
                                    var success1 = false;
                                    if (response !== null && response !== undefined) {
                                        if (response.data.ResponseType !== null && response.data.ResponseType !== undefined) {
                                            if (response.data.ResponseType === "success") {
                                                item.EmailAddresses = response.data.Data.EmailAddresses;
                                                item.Emails = null;
                                            } else {
                                                alert("Something went wrong while refreshing the list");
                                            }
                                        }
                                    }
                                });
                        }
                        else {
                            showError(response.data.Message);
                        }
                    }
                }
            });
    }

    $scope.close = function () {
        $uibModalInstance.close('close');
    };
});