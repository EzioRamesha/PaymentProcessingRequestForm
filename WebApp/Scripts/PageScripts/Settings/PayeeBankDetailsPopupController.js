app.controller('PayeeBankDetailsPopupController', function ($scope, $uibModalInstance, PayeeManager) {
    $scope.AddPayeeBank = $scope.UpdatePayeeBank = function (account) {
        var bankDetails = {
            Id: $scope.payee.Id,
            AccountDetails: account
        };
        PayeeManager.savePayeeBank(bankDetails).then(function (response) {
            if (response.data.ResponseType === 'success') {
                PayeeManager.getBankAccounts($scope.payee).then(function (response) {
                    $scope.payee.AccountDetails = response.data;
                    $scope.account = null;
                });
            }
        });
    };

    $scope.RemovePayeeBank = function (account) {
        var bankDetails = {
            Id: $scope.payee.Id,
            AccountDetails: account
        };
        PayeeManager.removePayeeBank(bankDetails).then(function (response) {
            if (response.data.ResponseType === 'success') {
                $scope.remove(account);
            }
        });
    };


    //$scope.AddToList = function (account) {
    //    if (account !== undefined && account !== null) {
    //        account.TempId = Date.now();
    //        $scope.payee.AccountDetails.push(account);
    //        $scope.account = null;
    //    }
    //}
    $scope.remove = function (account) {
        var index = $scope.payee.AccountDetails.indexOf(account);
        if (index > -1 && !account.Locked)
            $scope.payee.AccountDetails.splice(index, 1);
    };
    $scope.editAccount = function (account) {
        account.TempId = account.TempId === undefined || account.TempId === null ? Date.now() : account.TempId;
        $scope.account = {
            Id: account.Id,
            TempId: account.TempId,
            BankName: account.BankName,
            AccountName: account.AccountName,
            AccountNumber: account.AccountNumber,
            IFSC: account.IFSC,
            IBAN: account.IBAN,
            Swift: account.Swift,
            AccountType: account.AccountType,
            BankAddress: account.BankAddress
        }
        account.Locked = true;
        $scope.IsEdit = true;
    }

    //$scope.UpdateAccount = function (account) {
    //    for (var i = 0; i < $scope.payee.AccountDetails.length; i++) {
    //        if ($scope.payee.AccountDetails[i].TempId === account.TempId) {
    //            $scope.payee.AccountDetails[i] = account;
    //            break;
    //        }
    //    }
    //    $scope.reset();
    //}

    $scope.reset = function () {
        var obj = $.grep($scope.payee.AccountDetails, function (obj, idx) {
            return obj.TempId === $scope.account.TempId;
        });
        if (obj.length > 0)
            obj[0].Locked = false;
        $scope.account = null;
        $scope.IsEdit = false;
    }

    //$scope.save = function (payee) {
    //    PayeeManager.updateBankAccountDetails(payee).then(function (response) {
    //        $uibModalInstance.close(response.data);
    //    });
    //}

    $scope.close = function () {
        $uibModalInstance.close(null);
    }
});