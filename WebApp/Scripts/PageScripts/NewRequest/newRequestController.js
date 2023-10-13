app.controller('newRequest', function ($scope, RequestService, PayeeManager, $uibModal, GeneralService) {
    $scope.mask = function (value) {
        return mask(value);
    };
    $scope.unmask = function (value) {
        unmask(value);
    };

    $scope.popup1 = {
        opened: false
    };
    $scope.showDatePicker1 = function () {
        $scope.popup1.opened = true;
    };
    $scope.popup2 = {
        opened: false
    };
    $scope.showDatePicker2 = function () {
        $scope.popup2.opened = true;
    };
    $scope.popup3 = {
        opened: false
    };
    $scope.showDatePicker3 = function () {
        $scope.popup3.opened = true;
    };
    $scope.popup4 = {
        opened: false
    };
    $scope.showDatePicker4 = function () {
        $scope.popup4.opened = true;
    };

    $scope.editCurrency = function (value) {
        if (!$scope.isUndefinedOrNull($scope.request.CurrencyType)) {
            $scope.currencyEditable = value;
            if (!value) {
                $scope.currencyChanged();
            }
        } else {
            alert("Please select a currency first!");
        }
    }
   

    function reset() {
        $scope.request = {
            GoodsAndServices: [{}],
            Approvers: [{}],
            FileAttachmentsDetails: [],
            Files: [],
            CurrencyType: undefined,
            Month: $scope.Months[new Date().getMonth()].Value,
            //Year: $scope.yearRange.end - 1
            Year: new Date().getFullYear()
        };
        var currentDate = new Date();
        $scope.request.PPRFDate = new Date();
        $scope.request.DueDate = currentDate.addDays(7);
    }
    var DepartmentsAccountsDefaultData = '';
    $scope.initialize = function () {
        $scope.dateOptions = {
            //dateDisabled: disabled,
            formatYear: 'yy',
            maxDate: new Date(2099, 12, 23),
            minDate: new Date(),
            startingDay: 1
        };

        var months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
        RequestService.getInitialData().then(function (response) {
            $scope.PayingEntities = response.data.PayingEntities;
            $scope.Countries = response.data.Countries;
            $scope.Months = [];
            angular.forEach(months, function (month, index) {
                $scope.Months.push({ Name: month, Value: index + 1 });
            });
            $scope.editValueEuro = $scope.editValueUSD = "0.00000";
            $scope.yearRange = {
                start: 2015,
                end: 2031
                //end: new Date().getFullYear() + 1
            };

            $scope.RestrictPayeeOption = [];
            $scope.RestrictPayeeOption.push({ Name: "Yes", Value: "Y" });
            $scope.RestrictPayeeOption.push({ Name: "No", Value: "N" });

            $scope.Departments = response.data.Departments;
           // $scope.DepartmentsAccounts = response.data.DepartmentsAccounts;
            DepartmentsAccountsDefaultData = response.data.DepartmentsAccounts;
            $scope.FrequencyTypes = response.data.FrequencyTypes;
            $scope.PaymentMethods = response.data.PaymentMethods;
            $scope.ExpenseTypes = response.data.ExpenseTypes;
            $scope.Payees = response.data.Payees;
            $scope.CurrencyTypes = response.data.CurrencyTypes;
            $scope.TaxTypes = response.data.TaxTypes;
            $scope.Operators = response.data.Operators;
            $scope.BudgetOrders = response.data.BudgetOrders;
            $scope.PayeeAccounts = [];
            reset();
        });
    };

    $scope.createRequest = function (request) {
        var canCreateRequest = true;

        if (request.Payee === undefined || request.PayeeBank === undefined) {
            showError("Please complete Payee information");
            canCreateRequest = false;
        };

        if (canCreateRequest) {
            if (request.DocumentType === "BDG") {
                if (request.BudgetValidFrom === undefined || request.BudgetValidFrom === "" || request.BudgetValidTo === undefined || request.BudgetValidTo === "") {
                    showError("Please fill up Budget validity period");
                    canCreateRequest = false;
                }
            };
        };

        if (canCreateRequest) {
            $.each(request.GoodsAndServices, function (idx, obj) {
                obj.Amount = unmask(obj.AmountDisp);
            });
            $.each(request.Approvers, function (idx, obj) {
                obj.SequenceNo = idx + 1;
            });
            request.USDExRate = $scope.editValueUSD;
            request.EuroExRate = $scope.editValueEuro;

            if (request.TaxAmount2Disp !== undefined && request.TaxAmount2Disp !== null)
                request.TaxAmount2 = unmask(request.TaxAmount2Disp);
            if (request.TaxAmount3Disp !== undefined && request.TaxAmount3Disp !== null)
                request.TaxAmount3 = unmask(request.TaxAmount3Disp);

            var canProceed = true;

            if (request.DocumentType === "PFB") {
                var remainingBudget = request.BudgetApprovedAmtUSD - request.BudgetSpentAmtUSD - unmask(request.TotalUSD);

                if (remainingBudget < 0) {
                    if (confirm("Are you sure to proceed, as the spending is beyond the approved Budget?")) {
                        canProceed = true;
                    }
                    else {
                        canProceed = false;
                    };
                }
            }

            if (canProceed) {
                
                RequestService.createRequest(request).then(function (response) {
                    if (response !== undefined) {
                        if (response.data.ResponseType === "success") {
                            //reset();
                            $scope.initialize();
                            showSuccess(response.data.Message);
                            //window.location.href = "/";
                        }
                        else if (response.data.ResponseType === "error")
                            showError(response.data.Message);
                    }
                });
            }
        }  
    };

   


    $scope.SaveDraft = function (request) {
        var canCreateRequest = true;

        if (request.Payee === undefined || request.PayeeBank === undefined) {
            showError("Please complete Payee information");
            canCreateRequest = false;
        };

        if (canCreateRequest) {
            if (request.DocumentType === "BDG") {
                if (request.BudgetValidFrom === undefined || request.BudgetValidFrom === "" || request.BudgetValidTo === undefined || request.BudgetValidTo === "") {
                    showError("Please fill up Budget validity period");
                    canCreateRequest = false;
                }
            };
        };

        if (canCreateRequest) {
            $.each(request.GoodsAndServices, function (idx, obj) {
                obj.Amount = unmask(obj.AmountDisp);
            });
            $.each(request.Approvers, function (idx, obj) {
                obj.SequenceNo = idx + 1;
            });
            request.USDExRate = $scope.editValueUSD;
            request.EuroExRate = $scope.editValueEuro;

            if (request.TaxAmount2Disp !== undefined && request.TaxAmount2Disp !== null)
                request.TaxAmount2 = unmask(request.TaxAmount2Disp);
            if (request.TaxAmount3Disp !== undefined && request.TaxAmount3Disp !== null)
                request.TaxAmount3 = unmask(request.TaxAmount3Disp);

            var canProceed = true;

            if (request.DocumentType === "PFB") {
                var remainingBudget = request.BudgetApprovedAmtUSD - request.BudgetSpentAmtUSD - unmask(request.TotalUSD);

                if (remainingBudget < 0) {
                    if (confirm("Are you sure to proceed, as the spending is beyond the approved Budget?")) {
                        canProceed = true;
                    }
                    else {
                        canProceed = false;
                    };
                }
            }

            if (canProceed) {
                RequestService.SaveDraft(request).then(function (response) {
                    if (response !== undefined) {
                        if (response.data.ResponseType === "success") {
                            //reset();
                            $scope.initialize();
                            showSuccess(response.data.Message);
                            //window.location.href = "/";
                        }
                        else if (response.data.ResponseType === "error")
                            showError(response.data.Message);
                    }
                });
            }
        }
    };

    $scope.setFieldValuesForBudget = function (selecteditem) {
        $scope.budgetOrderRetrictPayee = selecteditem.RestrictedPayeeOnly;
        $scope.request.BudgetApprovedAmt = selecteditem.BudgetApprovedAmt;
        $scope.request.BudgetApprovedAmtUSD = selecteditem.BudgetApprovedAmtUSD;
        $scope.request.BudgetApprovedAmtEuro = selecteditem.BudgetApprovedAmtEuro;

        RequestService.getRequestDetails(selecteditem.BudgetPPRFId).then(function (response) {
            if (response !== undefined) {
                $scope.request.BudgetApprovedAmtDesc = response.data.CurrencyType.Code + " " + selecteditem.BudgetApprovedAmt.toFixed(2);
            };
        });

        RequestService.getBudgetSpendingInfo(selecteditem.BudgetPPRFId).then(function (response) {
            if (response !== undefined) {
                $scope.request.BudgetSpentAmtUSD = response.data.TotalSpentAmountUSD;
                $scope.request.BudgetSpentAmtEuro = response.data.TotalSpentAmountEuro;
                $scope.request.BudgetSpentAmtUSDDesc = "USD " + response.data.TotalSpentAmountUSD.toFixed(2);
            };
        });

        RequestService.getBudgetPayeeData(selecteditem.PayeeBankAccountDetailId).then(function (response) {
            if (response !== undefined) {
                $scope.request.Payee = response.data;

                var index = -1

                $scope.Payees.some(function (obj, i) {
                    return obj.Id === response.data.Id ? index = i : false;
                });



                $scope.request.Payee = $scope.Payees[index];

                //$scope.Payees = response.data;
            };
        });
        //$scope.request.PayeeBank = selecteditem.PayeeBankAccountDetailId;
    };

    $scope.setBudgetOrderList = function (pprfDate) {
        RequestService.getBudgetOrderListByDate(pprfDate).then(function (response) {
            if (response !== undefined) {
                $scope.BudgetOrders = response.data;
            }
        });
    };


    $scope.TempCheckBoxValue = function () {
        alert("Here");
    };


    $scope.validate = function ($files, $invalidFiles) {
        if ($scope.request === undefined) {
            $scope.request = { FileAttachmentsDetails: [] };
        } else if ($scope.request.FileAttachmentsDetails === undefined) {
            $scope.request.FileAttachmentsDetails = [];
            $scope.request.Files = [];
        }
        if ($files) {
            if ($files.length + $scope.request.FileAttachmentsDetails.length <= 5) {
                $.each($files, function (index, file) {
                    $scope.request.FileAttachmentsDetails.push({
                        Name: file.name,
                        Index: index
                    });
                    $scope.request.Files.push(file);
                });
                //$files = null;
            }
        }
    };

    $scope.removeFile = function (file) {
        var index = $scope.request.FileAttachmentsDetails.indexOf(file);
        if (index > -1) {
            $scope.request.FileAttachmentsDetails.splice(index, 1);
            $scope.request.Files.splice(index, 1);
        }
    };


    $scope.addService = function () {
        $scope.request.GoodsAndServices.push({});
    };
    $scope.addService1 = function () {
        $scope.formDetails.GoodsAndServices.push({});
    };
    $scope.removeService = function (service) {
        var index = $scope.request.GoodsAndServices.indexOf(service);
        if (index > -1 && $scope.request.GoodsAndServices.length > 0)
            $scope.request.GoodsAndServices.splice(index, 1);
    };
    $scope.removeService1 = function (service) {
        var index = $scope.formDetails.GoodsAndServices.indexOf(service);
        if (index > -1 && $scope.formDetails.GoodsAndServices.length > 0)
            $scope.formDetails.GoodsAndServices.splice(index, 1);
    };
    $scope.currencyChanged = function () {
        $scope.editValueUSD = mask($scope.request.CurrencyType.USDValue, 5);
        $scope.editValueEuro = mask($scope.request.CurrencyType.EuroValue, 5);
        $scope.dataChanged();
    }

    $scope.isUndefinedOrNull = function (val) {
        return angular.isUndefined(val) || val === null;
    }

    $scope.addApprover = function () {
        if ($scope.request.Approvers.length < 10)
            $scope.request.Approvers.push({});
    };
    $scope.removeApprover = function (approver) {
        if ($scope.request.Approvers.length > 1) {
            $scope.request.Approvers.splice($scope.request.Approvers.length - 1, 1);
        } else {
            alert("You need at least 1 approver.");
        }
    };

    $scope.approverSelected = function (newApprover, index) {
        x = $.grep($scope.request.Approvers, function (obj, idx) {
            return obj.Email === newApprover.Email;
        });
        if (x.length > 0) {
            alert("Approver already added.");
            this.operator = null;
        }
        else
            $scope.request.Approvers[index] = newApprover;
    };

    $scope.$watch('request.PayingEntity', function () {

        if ($scope.request !== undefined && $scope.request !== null)
            PayeeManager.getDepartmentDetails($scope.request.PayingEntity).then(function (response) {

                $scope.Departments = response.data;
            });
    });

    $scope.$watch('request.Department', function () {
       
        if ($scope.request !== undefined && $scope.request !== null)
            
            GeneralService.getDepartmentsAccount($scope.request.Department).then(function (response) {

                $scope.DepartmentsAccounts = response.data;
        });
            //if ($scope.request.Department.Name != 'Marketing') {
            //    $scope.DepartmentsAccounts = '';
            //}
            //else {
            //    $scope.DepartmentsAccounts = DepartmentsAccountsDefaultData;
            //}
    });


    $scope.$watch('request.Payee', function () {
      
        if ($scope.request !== undefined && $scope.request !== null)
            PayeeManager.getBankAccounts($scope.request.Payee).then(function (response) {
              
                $scope.PayeeAccounts = response.data;
            });
    });

    $scope.$watch('request.PPRFDate', function () {
        var minDate = new Date().addDays(7);
        if ($scope.request !== undefined) {
            minDate = $scope.request.PPRFDate.addDays(7);
            if ($scope.request.DueDate < minDate)
                $scope.request.DueDate = minDate;
        }
        $scope.dueDateOptions = {
            //dateDisabled: disabled,
            formatYear: 'yy',
            maxDate: new Date(2099, 12, 31),
            minDate: minDate,
            startingDay: 1
        };
    });

    $scope.addPayeeDialog = function () {
        $scope.newPayee = null;
        GeneralService.GetActiveCountries().then(function (response) {
            $scope.Countries = response.data;
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: '/Templates/Payee/PayeePopup.html',
                controller: 'PayeePopupController',
                backdrop: 'static',
                scope: $scope
            }).result.then(function (data) {
                if (data !== undefined && data !== null && data.ResponseType && data.ResponseType === "success") {
                    showSuccess(response.Message);
                    GeneralService.GetActivePayees().then(function (response) {
                        $scope.Payees = response.data;
                    });
                } else {
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
                        if (data.ResponseType === "success")
                            PayeeManager.getBankAccounts(payee).then(function (response) {
                                $scope.PayeeAccounts = response.data;
                            });
                        else
                            showError(data.Message);
                    }
                });
            });
        } else {
            alert("No payee selected.");
        }
    }

    function compute() {
        var currencyFactorUSD = 1, currencyFactorEuro = 1;
        if (!($scope.request.CurrencyType === undefined || $scope.request.CurrencyType === null)) {
            //currencyFactorUSD = $scope.request.CurrencyType.USDValue;
            //currencyFactorEuro = $scope.request.CurrencyType.EuroValue;
            currencyFactorUSD = unmask($scope.editValueUSD);
            currencyFactorEuro = unmask($scope.editValueEuro);
        }
        $scope.request.SubTotal = 0;
        $scope.request.TaxAmount = 0;
        for (i = 0; i < $scope.request.GoodsAndServices.length; i++) {
            $scope.request.SubTotal = +$scope.request.SubTotal + +unmask($scope.request.GoodsAndServices[i].AmountDisp);
            $scope.request.TaxAmount = +$scope.request.TaxAmount + +unmask($scope.request.GoodsAndServices[i].AmountDisp) *
                                        (($scope.request.GoodsAndServices[i].TaxType === null || $scope.request.GoodsAndServices[i].TaxType === undefined) ? 0 : $scope.request.GoodsAndServices[i].TaxType.PercentageValue / 100);

            $scope.request.GoodsAndServices[i].AmountUSDDisp = mask(unmask($scope.request.GoodsAndServices[i].AmountDisp) * currencyFactorUSD);
            $scope.request.GoodsAndServices[i].AmountEuroDisp = mask(unmask($scope.request.GoodsAndServices[i].AmountDisp) * currencyFactorEuro);
        }
        $scope.request.Total = $scope.request.SubTotal +
                                    ($scope.request.TaxAmount === undefined ? 0 : +$scope.request.TaxAmount) +
                                    (unmask($scope.request.TaxAmount2Disp) === undefined ? 0 : +unmask($scope.request.TaxAmount2Disp)) +
                                    (unmask($scope.request.TaxAmount3Disp) === undefined ? 0 : +unmask($scope.request.TaxAmount3Disp));

        $scope.request.TotalUSD = mask($scope.request.Total * currencyFactorUSD);
        $scope.request.TotalEuro = mask($scope.request.Total * currencyFactorEuro);
        $scope.request.Total = mask($scope.request.Total);


        $scope.request.SubTotalUSD = mask($scope.request.SubTotal * currencyFactorUSD);
        $scope.request.SubTotalEuro = mask($scope.request.SubTotal * currencyFactorEuro);
        $scope.request.SubTotal = mask($scope.request.SubTotal);



        $scope.request.TaxAmountUSD = mask($scope.request.TaxAmount * currencyFactorUSD);
        $scope.request.TaxAmountEuro = mask($scope.request.TaxAmount * currencyFactorEuro);
        $scope.request.TaxAmount = mask($scope.request.TaxAmount);


        $scope.request.TaxAmount2USD = mask(unmask($scope.request.TaxAmount2Disp) * currencyFactorUSD);
        $scope.request.TaxAmount2Euro = mask(unmask($scope.request.TaxAmount2Disp) * currencyFactorEuro);

        $scope.request.TaxAmount3USD = mask(unmask($scope.request.TaxAmount3Disp) * currencyFactorUSD);
        $scope.request.TaxAmount3Euro = mask(unmask($scope.request.TaxAmount3Disp) * currencyFactorEuro);
    }
    $scope.dataChanged = function () {
        compute();
    };


    function EditPricecompute() {
        var currencyFactorUSD = 1, currencyFactorEuro = 1;
        if (!($scope.request.CurrencyType === undefined || $scope.request.CurrencyType === null)) {
            //currencyFactorUSD = $scope.request.CurrencyType.USDValue;
            //currencyFactorEuro = $scope.request.CurrencyType.EuroValue;
            currencyFactorUSD = unmask($scope.editValueUSD);
            currencyFactorEuro = unmask($scope.editValueEuro);
        }
        $scope.request.SubTotal = 0;
        $scope.request.TaxAmount = 0;
        for (i = 0; i < $scope.formDetails.GoodsAndServices.length; i++) {
            $scope.request.SubTotal = +$scope.request.SubTotal + +unmask($scope.formDetails.GoodsAndServices[i].AmountDisp);
            $scope.request.TaxAmount = +$scope.request.TaxAmount + +unmask($scope.formDetails.GoodsAndServices[i].AmountDisp) *
                (($scope.formDetails.GoodsAndServices[i].TaxType === null || $scope.formDetails.GoodsAndServices[i].TaxType === undefined) ? 0 : $scope.formDetails.GoodsAndServices[i].TaxType.PercentageValue / 100);

            $scope.formDetails.GoodsAndServices[i].AmountUSDDisp = mask(unmask($scope.formDetails.GoodsAndServices[i].AmountDisp) * currencyFactorUSD);
            $scope.formDetails.GoodsAndServices[i].AmountEuroDisp = mask(unmask($scope.formDetails.GoodsAndServices[i].AmountDisp) * currencyFactorEuro);
        }
        $scope.request.Total = $scope.request.SubTotal +
            ($scope.request.TaxAmount === undefined ? 0 : +$scope.request.TaxAmount) +
            (unmask($scope.request.TaxAmount2Disp) === undefined ? 0 : +unmask($scope.request.TaxAmount2Disp)) +
            (unmask($scope.request.TaxAmount3Disp) === undefined ? 0 : +unmask($scope.request.TaxAmount3Disp));

        $scope.request.TotalUSD = mask($scope.request.Total * currencyFactorUSD);
        $scope.request.TotalEuro = mask($scope.request.Total * currencyFactorEuro);
        $scope.request.Total = mask($scope.request.Total);


        $scope.request.SubTotalUSD = mask($scope.request.SubTotal * currencyFactorUSD);
        $scope.request.SubTotalEuro = mask($scope.request.SubTotal * currencyFactorEuro);
        $scope.request.SubTotal = mask($scope.request.SubTotal);



        $scope.request.TaxAmountUSD = mask($scope.request.TaxAmount * currencyFactorUSD);
        $scope.request.TaxAmountEuro = mask($scope.request.TaxAmount * currencyFactorEuro);
        $scope.request.TaxAmount = mask($scope.request.TaxAmount);


        $scope.request.TaxAmount2USD = mask(unmask($scope.request.TaxAmount2Disp) * currencyFactorUSD);
        $scope.request.TaxAmount2Euro = mask(unmask($scope.request.TaxAmount2Disp) * currencyFactorEuro);

        $scope.request.TaxAmount3USD = mask(unmask($scope.request.TaxAmount3Disp) * currencyFactorUSD);
        $scope.request.TaxAmount3Euro = mask(unmask($scope.request.TaxAmount3Disp) * currencyFactorEuro);
    }
    $scope.EditdataChanged = function () {
        EditPricecompute();
    };
});