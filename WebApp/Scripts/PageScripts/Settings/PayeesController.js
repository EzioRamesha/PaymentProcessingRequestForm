

app.controller('PayeeController', function ($scope, $uibModal, GeneralService, PayeeManager) {
    var searchCriteria = {
        Pager: {
            ItemsPerPage: 20,
            CurrentPage: 1
        }
    };
    $scope.refresh = function (Pager) {
        if (Pager === null)
        {
            Pager: {
                CurrentPage : 1
                ItemsPerPage: 20
                TotalItems : 1
            }
        }
        searchCriteria.Pager = Pager;
        GeneralService.getAllPayees(searchCriteria).then(function (response) {
            $scope.Payees = response.data.Payees;
            $scope.Pager = response.data.Pager;
        });
        GeneralService.GetActivePayees().then(function (response) {
            $scope.FilterPayees = response.data;
        });

       
    }
    
    $scope.filter = function (Pager, Payee)
    {
        if (Pager === null) {
            Pager: {
                CurrentPage: 1
                ItemsPerPage: 20
                TotalItems: 1
            }
        }
        var PayeeName = Payee.PayeeId;
        searchCriteria.Pager = Pager;
        GeneralService.getAllPayeesByname(searchCriteria, PayeeName).then(function (response) {
            $scope.Payees = response.data.Payees;
            $scope.Pager = response.data.Pager;
        });
        GeneralService.GetActivePayees().then(function (response) {
            $scope.FilterPayees = response.data;
        });
    };
   

    $scope.Pager = searchCriteria.Pager;
    $scope.initialize = function () {
        $scope.Payees = [];
        $scope.refresh(searchCriteria.Pager);
    }
    $scope.enable = function (payee) {
        PayeeManager.enable(payee).then(function (resposne) {
            $scope.initialize();
        });
    };
    $scope.disable = function (payee) {
        PayeeManager.disable(payee).then(function (resposne) {
            $scope.initialize();
        });
    };

    $scope.payeeDialog = function (payee) {
        if (payee)
            $scope.payee = {
                Id: payee.Id,
                Name: payee.Name,
                Phone: payee.Phone,
                Fax: payee.Fax,
                AddressLine1: payee.AddressLine1,
                AddressLine2: payee.AddressLine2,
                AddressLine3: payee.AddressLine3,
                CountryId: payee.CountryId,
                HotelName: payee.HotelName
            };
        else
            $scope.payee = null;
        GeneralService.GetActiveCountries().then(function (response) {
            $scope.Countries = response.data;
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: '/Templates/Payee/PayeePopup.html',
                controller: 'PayeePopupController',
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
        });
    };

    $scope.payeeBankDetailsDialog = function (payee) {
        if (payee !== undefined && payee !== null) {
            PayeeManager.getBankAccounts(payee).then(function (response) {
                $scope.payee = {
                    Id: payee.Id,
                    Name: payee.Name,
                    AccountDetails: response.data
                };
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: '/Templates/Payee/PayeeBankDetails.html',
                    controller: 'PayeeBankDetailsPopupController',
                    backdrop: 'static',
                    windowClass: 'app-modal-window',
                    scope: $scope
                });
                modalInstance.result.then(function (data) {
                    if (data !== undefined && data !== null) {
                        if (data.ResponseType === "success") {
                            $scope.initialize();
                        }
                    }
                });
            });
        }
    }
});

//app.controller('PayeeBankDetailsController', function ($scope, $uibModalInstance, PayeeManager) {
//    $scope.AddPayeeBank = $scope.UpdatePayeeBank = function (account) {
//        var bankDetails = {
//            Id: $scope.payee.Id,
//            AccountDetails: account
//        };
//        PayeeManager.savePayeeBank(bankDetails).then(function (response) {
//            if (response.data.ResponseType === 'success') {
//                PayeeManager.getBankAccounts($scope.payee).then(function (response) {
//                    $scope.payee.AccountDetails = response.data;
//                    $scope.account = null;
//                });
//            }
//        });
//    };

//    $scope.RemovePayeeBank = function (account) {
//        var bankDetails = {
//            Id: $scope.payee.Id,
//            AccountDetails: account
//        };
//        PayeeManager.removePayeeBank(bankDetails).then(function (response) {
//            if (response.data.ResponseType === 'success') {
//                $scope.remove(account);
//            }
//        });
//    };


//    //$scope.AddToList = function (account) {
//    //    if (account !== undefined && account !== null) {
//    //        account.TempId = Date.now();
//    //        $scope.payee.AccountDetails.push(account);
//    //        $scope.account = null;
//    //    }
//    //}
//    //$scope.remove = function (account) {
//    //    var index = $scope.payee.AccountDetails.indexOf(account);
//    //    if (index > -1 && !account.Locked)
//    //        $scope.payee.AccountDetails.splice(index, 1);
//    //}
//    $scope.editAccount = function (account) {
//        account.TempId = account.TempId === undefined || account.TempId === null ? Date.now() : account.TempId;
//        $scope.account = {
//            Id: account.Id,
//            TempId: account.TempId,
//            BankName: account.BankName,
//            AccountName: account.AccountName,
//            AccountNumber: account.AccountNumber,
//            IFSC: account.IFSC,
//            IBAN: account.IBAN,
//            Swift: account.Swift,
//            AccountType: account.AccountType,
//            BankAddress: account.BankAddress
//        }
//        account.Locked = true;
//        $scope.IsEdit = true;
//    }

//    //$scope.UpdateAccount = function (account) {
//    //    for (var i = 0; i < $scope.payee.AccountDetails.length; i++) {
//    //        if ($scope.payee.AccountDetails[i].TempId === account.TempId) {
//    //            $scope.payee.AccountDetails[i] = account;
//    //            break;
//    //        }
//    //    }
//    //    $scope.reset();
//    //}

//    $scope.reset = function () {
//        var obj = $.grep($scope.payee.AccountDetails, function (obj, idx) {
//            return obj.TempId === $scope.account.TempId;
//        });
//        if (obj.length > 0)
//            obj[0].Locked = false;
//        $scope.account = null;
//        $scope.IsEdit = false;
//    }

//    //$scope.save = function (payee) {
//    //    PayeeManager.updateBankAccountDetails(payee).then(function (response) {
//    //        $uibModalInstance.close(response.data);
//    //    });
//    //}

//    $scope.close = function () {
//        $uibModalInstance.close(null);
//    }
//});