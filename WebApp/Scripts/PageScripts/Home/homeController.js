/// <reference path="../NewRequest/RequestService.js" />
/// <reference path="../../angular-ui/ui-bootstrap-tpls.min.js" />
var varrequestForms = '';
var varcancelRequests = '';
app.service('HomeService', function ($http) {
    this.getRequestsPendingApproval = function (searchCriteria) {
        var promise = $http({
            url: '/Home/FormsPendingApproval',
            method: 'POST',
            data: searchCriteria
        });
        return promise;
    }
    this.getOriginatorRequestForms = function (searchCriteria) {
        var promise = $http({
            url: '/Home/FormsForOriginator',
            method: 'POST',
            data: searchCriteria
        });
        return promise;
    };
});

var months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
app.controller('homeController', function ($scope, $uibModal, HomeService, RequestService) {
    $scope.months = months;

    $scope.refreshOriginatorForms = function (searchCriteria) {
        HomeService.getOriginatorRequestForms(searchCriteria).then(function (response) {
            $scope.OriginatorFormsSearchCriteria.Pager = response.data.Pager;
            $scope.pendingRaisedRequests = response.data.RequestForms;
        });
    }

    $scope.refreshApproverForms = function (searchCriteria) {
        HomeService.getRequestsPendingApproval(searchCriteria).then(function (response) {
            $scope.RequestFormsSearchCriteria.Pager = response.data.Pager;
            $scope.pendingApprovals = response.data.RequestForms;
        });
    }
    var originatorSearchCriteria = {
        Pager: {
            ItemsPerPage: 10,
            CurrentPage: 1
        }
    };
    var approverSearchCriteria = {
        Pager: {
            ItemsPerPage: 10,
            CurrentPage: 1
        }
    }
    $scope.OriginatorFormsSearchCriteria = originatorSearchCriteria;
    $scope.RequestFormsSearchCriteria = approverSearchCriteria;
    $scope.initialize = function () {
        $scope.refreshApproverForms($scope.RequestFormsSearchCriteria);
        $scope.refreshOriginatorForms($scope.OriginatorFormsSearchCriteria);

        $scope.CancelPPRFReasonTypes = [];
        $scope.loadCancelPPRFReasonTypes();
    };

    $scope.viewRequestDetails = function (requestForm) {
        RequestService.getApproverRequestDetails(requestForm.Id).then(function (response) {
            $scope.formDetails = response.data;
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: '/Templates/Pprf/viewPprf.html',
                controller: 'ViewController',
                backdrop: 'static',
                scope: $scope,
                windowClass: 'app-modal-window',
            });
        });
    };

    $scope.viewQueries = function (requestForm) {
        RequestService.getRequestQueries(requestForm).then(function (response) {
            if (response.data.ResponseType === "success") {
                $scope.Queries = response.data.Data;
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: '/Templates/Pprf/pprfQueries.html',
                    controller: 'ViewQuestionsController',
                    backdrop: 'static',
                    scope: $scope,
                    windowClass: 'app-modal-window',
                });
            } else {
                showError(response.data.Message);
            }
        });
    };


    $scope.askClarifications = function (requestForm) {
        RequestService.askClarifications(requestForm).then(function (response) {
            $scope.initialize();
        });
    };
    $scope.giveClarification = function (requestForm) {
        requestForm.EditClarification = !requestForm.EditClarification;
    };
    $scope.closeClarification = function (requestForm) {
        requestForm.EditClarification = false;
    };
    $scope.saveClarification = function (requestForm) {
        RequestService.giveClarification(requestForm).then(function (response) {
            $scope.initialize();
        });
    };


    

    $scope.showAskQuestion = function (requestForm) {
        requestForm.editQuestion = true;
    };
    $scope.closeQuestionBox = function (requestForm) {
        requestForm.editQuestion = false;
    };
    $scope.askQuestion = function (requestForm) {
        var approverQuestion = {
            Question: requestForm.Question,
            RequestId: requestForm.Id
        };
        RequestService.askQuestion(approverQuestion).then(function (response) {
            if (response.data.ResponseType === 'success') {
                showSuccess(response.data.Message);
                $scope.initialize();
            }
            else
                showError(response.data.Message);
        });
    };
    $scope.showQuestion = function (requestForm) {
        requestForm.EditAnswer = true;
    };
    $scope.saveAnswer = function (question) {
        var approverAnswer = {
            QuestionId: question.Id,
            Answer: question.Answer
        };
        RequestService.saveAnswer(approverAnswer).then(function (response) {
            if (response.data.ResponseType === 'success') {
                showSuccess(response.data.Message);
                $scope.initialize();
            }
            else
                showError(response.data.Message);
        });
    };
    $scope.closeAnswerBox = function (requestForm) {
        requestForm.EditAnswer = false;
    };


    $scope.loadCancelPPRFReasonTypes = function () {
        RequestService.getCancelPPRFReasonTypes().then(function (response) {
            $scope.CancelPPRFReasonTypes = response.data;
        });
    }
   


   


    //$scope.resendEmail = function (requestForm) {

        
    //    if (confirm("Are you sure you want to re-send the email?"))
    //        $("#exampleModal").modal('show');
    //        RequestService.resendEmail(requestForm).then(function (response) {
    //            if (response.data.ResponseType === 'success')
    //                showSuccess(response.data.Message);
    //            else showError(response.data.Message);
    //        });
    //};
   
    $scope.SendEmailDialog = function (requestForm) {
        varrequestForms = requestForm;
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/PendingRaisedRequestReEmail/ResendEmail.html',
            controller: 'ResendEmailPopupController',
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



   

    $scope.viewRaisedRequestDetails = function (requestForm) {
        RequestService.getOriginatorRequestDetails(requestForm.Id).then(function (response) {
            $scope.formDetails = response.data;

            var pprfTemplate = "";

            if ($scope.formDetails.DocumentType === 'PFB') {
                pprfTemplate = '/Templates/Pprf/viewOriginatorPprf_PFB.html';
            }
            else if ($scope.formDetails.DocumentType === 'BDG') {
                pprfTemplate = '/Templates/Pprf/viewOriginatorPprf_BDG.html';
            }
            else {
                pprfTemplate = '/Templates/Pprf/viewOriginatorPprf.html';
            }

            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: pprfTemplate,
                controller: 'ViewController',
                backdrop: 'static',
                scope: $scope,
                windowClass: 'app-modal-window',
            });
        });
    };

    $scope.viewDraftRaisedRequestDetails = function (requestForm) {
        RequestService.getOriginatorRequestDetails(requestForm.Id).then(function (response) {
            $scope.formDetails = response.data;

            var pprfTemplate = "";

            if ($scope.formDetails.DocumentType === 'PFB') {
                pprfTemplate = '/Templates/Pprf/viewOriginatorPprf_PFB.html';
            }
            else if ($scope.formDetails.DocumentType === 'BDG') {
                pprfTemplate = '/Templates/Pprf/viewOriginatorPprf_BDG.html';
            }
            else {
                pprfTemplate = '/Templates/Pprf/viewDraftOriginatorPprf.html';
            }

            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: pprfTemplate,
                controller: 'ViewController',
                backdrop: 'static',
                scope: $scope,
                windowClass: 'app-modal-window',
            });
        });
    };


    $scope.approve = function (requestForm) {
        RequestService.approve(requestForm).then(function (response) {
            $scope.initialize($scope.RequestFormsSearchCriteria);
        });
    };

    $scope.reject = function (requestForm) {
        $scope.selectedForm = requestForm;
        $scope.reason = '';
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/Pprf/rejectReason.html',
            controller: 'RequestRejectController',
            backdrop: 'static',
            scope: $scope
        });
    };

    $scope.actions = {
        templateUrl: 'myPopoverTemplate.html',
        placement: 'top-right',
        trigger: 'outsideClick',
        cclass: 'popover-actions'
    }
   
    
});

app.controller('ViewQuestionsController', function ($scope, $uibModalInstance, RequestService, $filter) {
    $scope.close = function () {
        $uibModalInstance.close();
    };
});


app.controller('ViewController', function ($scope, $uibModalInstance, RequestService, $filter, $uibModal) {
    $scope.months = months.slice();
    $scope.close = function () {
        $uibModalInstance.close();
    };
    $scope.printRequest = function (requestId) {
        window.open("/Pprf/PrintRequest/?requestId=" + requestId);
    };
    $scope.downloadFile = function (docID) {
        window.open("/Pprf/GetDocumentByDocId/?docID=" + docID);
    };

    $scope.cancel = function () {
        $scope.isEdit = false;
    };

    
    $scope.DraftCreateRequest = function (request, formDetails, editValueEuro, editValueUSD) {
        var canCreateRequest = true;
        request.GoodsAndServices = $scope.formDetails.GoodsAndServices;
       // request.Files = $scope.request.Files;
        

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
                if (obj.AmountDisp !== undefined) {
                    obj.Amount = unmask(obj.AmountDisp);
                }
               
            });
            $.each(request.Approvers, function (idx, obj) {
                obj.SequenceNo = idx + 1;
            });
            if (editValueUSD == 0.00000 | editValueUSD == undefined) {
                request.USDExRate = formDetails.USDExRate;
            }
            else {
                request.USDExRate = editValueUSD;
            }
            if (editValueEuro == 0.00000 | editValueEuro == undefined) {
                request.EuroExRate = formDetails.EuroExRate;
            }
            else {
                request.EuroExRate = editValueEuro;
            }
           
            //request.GoodsAndServices[0].Description = request.Descriptioni;
            if (request.TaxAmount2Disp !== undefined && request.TaxAmount2Disp !== null)
                request.TaxAmount2 = unmask(request.TaxAmount2Disp);
            if (request.TaxAmount3Disp !== undefined && request.TaxAmount3Disp !== null)
                request.TaxAmount3 = unmask(request.TaxAmount3Disp);

            var canProceed = true;



            if (canProceed) {
                if (request.Department === undefined) {

                    RequestService.getAllDepartments().then(function (response) {
                        var Departmentsii = [];
                        Departmentsii = response.data;
                        var i;
                        for (i = 0; i < Departmentsii.length; ++i) {

                            if (Departmentsii[i].Name === $scope.formDetails.DepartmentName) {
                                request.Department = Departmentsii[i];
                            }
                        }
                    });
                }
                if (request.Approvers.length === 1) {
                    request.Approvers = $scope.formDetails.Approvals;

                }
                RequestService.getInitialData().then(function (response) {
                    var Id = $scope.formDetails.RequestId;
                    if (request.DocumentType === undefined) {

                        request.DocumentType = $scope.formDetails.DocumentType;
                    }


                    if (request.FrequencyType === undefined) {

                        var FrequencyTypei = response.data.FrequencyTypes;
                        var i;
                        for (i = 0; i < FrequencyTypei.length; ++i) {

                            if (FrequencyTypei[i].Name === $scope.formDetails.FrequencyTypeName) {
                                request.FrequencyType = FrequencyTypei[i];
                            }
                        }


                    }
                    if (request.DepartmentsAccount === undefined) {

                        var DepartmentsAccountsi = response.data.DepartmentsAccounts;
                        var i;
                        for (i = 0; i < DepartmentsAccountsi.length; ++i) {

                            if (DepartmentsAccountsi[i].Name === $scope.formDetails.DepartmentsAccountName) {
                                request.DepartmentsAccount = DepartmentsAccountsi[i];
                            }
                        }


                    }
                    if (request.PaymentMethod === undefined) {

                        var PaymentMethodsi = response.data.PaymentMethods;
                        var i;
                        for (i = 0; i < PaymentMethodsi.length; ++i) {

                            if (PaymentMethodsi[i].Name === $scope.formDetails.PaymentMethodName) {
                                request.PaymentMethod = PaymentMethodsi[i];
                            }
                        }

                    }
                    if (request.ExpenseType === undefined) {

                        var ExpenseTypei = response.data.ExpenseTypes;
                        var i;
                        for (i = 0; i < ExpenseTypei.length; ++i) {

                            if (ExpenseTypei[i].Name === $scope.formDetails.ExpenseTypeName) {
                                request.ExpenseType = ExpenseTypei[i];
                            }
                        }


                    }
                    
                    if (request.CurrencyType === undefined) {

                        var CurrencyTypesi = response.data.CurrencyTypes;
                        var i;
                        for (i = 0; i < CurrencyTypesi.length; ++i) {

                            if (CurrencyTypesi[i].Name === $scope.formDetails.CurrencyType.Name) {
                                request.CurrencyType = CurrencyTypesi[i];
                            }
                        }

                    }
                    if (request.PayeeBankDetails === undefined) {
                        request.PayeeBankDetails = $scope.formDetails.PayeeBankDetails;
                    }

                    if (request.PayingEntity === undefined) {

                        $scope.PayingEntitiese = response.data.PayingEntities;

                        var payingenti = response.data.PayingEntities;
                        var i;
                        for (i = 0; i < payingenti.length; ++i) {

                            if (payingenti[i].Name === $scope.formDetails.PayingEntityName) {
                                request.PayingEntity = $scope.PayingEntitiese[i];
                            }
                        }

                    }
                    if (request.Originator === undefined) {

                        var Originatori = response.data.Operators[0].Approvers;
                        var i;
                        for (i = 0; i < Originatori.length; ++i) {
                            if (Originatori[i].UserName === $scope.formDetails.Originator.OriginatorName) {
                                request.Originator = Originatori[i];
                            }
                        }

                    }



                    if (request.Payee === undefined) {

                        var Payeei = response.data.Payees;
                        var i;
                        for (i = 0; i < Payeei.length; ++i) {
                            if (Payeei[i].Name === $scope.formDetails.Payee.Name) {
                                request.Payee = Payeei[i];
                            }
                        }

                    }
                    if (request.PayeeBank === undefined) {
                        RequestService.getBankAccounts(request.Payee).then(function (responses) {
                            request.PayeeBank = responses.data[0];
                            RequestService.DraftUpdate(request, Id).then(function (response) {
                                if (response !== undefined) {
                                    if (response.data.ResponseType === "success") {
                                        //reset();
                                        $scope.initialize();
                                        showSuccess(response.data.Message);
                                        $uibModalInstance.close();
                                    }
                                    else if (response.data.ResponseType === "error")
                                        showError(response.data.Message);
                                    $uibModalInstance.close();

                                }
                            });
                            RequestService.update($scope.formDetails.RequestId, $scope.editApprovals).then(function (response) {
                                var requestForm = {
                                    Id: $scope.formDetails.RequestId
                                };
                                RequestService.getOriginatorRequestDetails(requestForm.Id).then(function (response) {
                                    $scope.isEdit = false;
                                    $scope.formDetails = response.data;
                                });
                                if (response.data.ResponseType === 'success')
                                    showSuccess(response.data.Message);
                                else {
                                    showError(response.data.Message);
                                }
                            });
                        });

                    }
                    else {
                        RequestService.getBankAccounts(request.Payee).then(function (responses) {
                            request.PayeeBank = responses.data[0];
                            RequestService.DraftUpdate(request, Id).then(function (response) {
                                if (response !== undefined) {
                                    if (response.data.ResponseType === "success") {
                                        //reset();
                                        $scope.initialize();
                                        showSuccess(response.data.Message);
                                        $uibModalInstance.close();
                                    }
                                    else if (response.data.ResponseType === "error")
                                        showError(response.data.Message);
                                    $uibModalInstance.close();

                                }
                            });
                            RequestService.update($scope.formDetails.RequestId, $scope.editApprovals).then(function (response) {
                                var requestForm = {
                                    Id: $scope.formDetails.RequestId
                                };
                                RequestService.getOriginatorRequestDetails(requestForm.Id).then(function (response) {
                                    $scope.isEdit = false;
                                    $scope.formDetails = response.data;
                                });
                                if (response.data.ResponseType === 'success')
                                    showSuccess(response.data.Message);
                                else {
                                    showError(response.data.Message);
                                }
                            });
                        });
                    }


                });

            }
        }
    };
    $scope.edit = function () {
        $scope.editApprovals = [];
        
        if ($scope.Operators === undefined) {
            RequestService.getOperators().then(function (response) {
                $scope.Operators = response.data;
                $.each($scope.Operators, function (index, group) {
                    $.each(group.Approvers, function (index, approver) {
                        approver.ApproverType = group.GroupName;
                        approver.ApprovalStatus = 'Pending';
                    });

                    var type = $filter("filter")($scope.formDetails.Approvals, { ApproverType: group.GroupName })[0];
                    if (type !== undefined) {
                        var appro = $filter("filter")(group.Approvers, { Email: type.ApproverEmail })[0];
                        appro.ApprovalStatus = type.ApprovalStatus;
                        $scope.editApprovals.push(appro);
                    }
                });
            });
        } else {
            $.each($scope.Operators, function (index, group) {
                $.each(group.Approvers, function (index, approver) {
                    approver.ApproverType = group.GroupName;
                    approver.ApprovalStatus = 'Pending';
                });

                var type = $filter("filter")($scope.formDetails.Approvals, { ApproverType: group.GroupName })[0];
                if (type !== undefined) {
                    var appro = $filter("filter")(group.Approvers, { Email: type.ApproverEmail })[0];
                    appro.ApprovalStatus = type.ApprovalStatus;
                    $scope.editApprovals.push(appro);
                }
            });
        }
        RequestService.getInitialData().then(function (response) {
            $scope.Operators = response.data.Operators;
            $scope.PayingEntitiese = response.data.PayingEntities;
 
        });
        RequestService.getAllDepartments().then(function (response) {
            $scope.Departments = [];
            $scope.Departments = response.data;
        });
       
        //  $scope.editOriginator = [];
        //if ($scope.Originators === undefined) {
        //    RequestService.getOriginators().then(function (response) {
        //        $scope.Originators = response.data;
        //        $.each($scope.Originators, function (index, group) {
        //            $.each(group.Originators, function (index, Originator) {
        //                Originator.OriginatorType = group.GroupName;

        //            });

        //            var type = $filter("filter")($scope.formDetails.Originator, { OriginatorType: group.GroupName })[0];
        //            if (type !== undefined) {
        //                var appro = $filter("filter")(group.Originators, { Email: type.OriginatorEmail })[0];

        //                $scope.editOriginator.push(appro);
        //            }
        //        });
        //    });
        //} else {
        //    $.each($scope.Originators, function (index, group) {
        //        $.each(group.Originators, function (index, Originator) {
        //            Originator.OriginatorType = group.GroupName;
        //        });

        //        var type = $filter("filter")($scope.formDetails.Originator, { OriginatorType: group.GroupName })[0];
        //        if (type !== undefined) {
        //            var appro = $filter("filter")(group.Originators, { Email: type.OriginatorEmail })[0];

        //            $scope.editOriginator.push(appro);
        //        }
        //    });
        //}
     
        $scope.isEdit = true;
    };
    
    
    $scope.approverSelected1 = function (newApprover, index) {
        x = $.grep($scope.editApprovals, function (obj, idx) {
            return obj.Email === newApprover.Email;
        });
        if (x.length > 0) {
            alert("Approver already added.");
            this.operator = null;
        }
        else {
            $scope.editApprovals[index] = newApprover;
        }
    };


    $scope.OriginatorSelected = function (newApprover, index) {
        x = $.grep($scope.editOriginator, function (obj, idx) {
            return obj.Email === newApprover.Email;
        });
        if (x.length > 0) {
            alert("Originator already added.");
            this.operator = null;
        }
        else {
            $scope.editOriginator[index] = newApprover;
        }
    };

    $scope.addApprover1 = function () {
        if ($scope.editApprovals.length < 10)
            $scope.editApprovals.push({});
    };
    $scope.removeApprover = function () {
        if ($scope.editApprovals.length > 1)
            $scope.editApprovals.splice($scope.editApprovals.length - 1, 1);
        else
            alert("You need at least 1 approver.");
    };
    $scope.updateRequest = function () {
        var updatedApprovers = $filter("filter")($scope.editApprovals, { ApprovalStatus: 'Pending' });
        RequestService.update($scope.formDetails.RequestId, $scope.editApprovals).then(function (response) {
            var requestForm = {
                Id: $scope.formDetails.RequestId
            };
            RequestService.getOriginatorRequestDetails(requestForm.Id).then(function (response) {
                $scope.isEdit = false;
                $scope.formDetails = response.data;
            });
            if (response.data.ResponseType === 'success')
                showSuccess(response.data.Message);
            else {
                showError(response.data.Message);
            }
        });
    };

    //$scope.cancelRequest = function (request) {
    //    if (confirm("Are you sure you want to mark this PO/PPRF as cancelled?")) {
    //        RequestService.cancelRequest(request).then(function (response) {
    //            if (response.data.ResponseType === 'success') {
    //                showSuccess(response.data.Message);
    //                $scope.close();
    //                $scope.initialize();
    //            }
    //            else if (response.data.ResponseType === 'error') showError(response.data.Message);
    //        });
    //    }
    //};


    $scope.cancelRequestsDialog = function (request) {
        varcancelRequests = request;
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/Pprf/CancelRequest.html',
            controller: 'cancelRequestPopupController',
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




app.controller('RequestRejectController', function ($scope, $uibModalInstance, RequestService) {
    $scope.close = function () {
        $uibModalInstance.close();
    };
    $scope.submit = function (requestForm, reason) {
        RequestService.reject($scope.selectedForm, reason).then(function (response) {
            $scope.close();
            $scope.initialize($scope.RequestFormsSearchCriteria);
        });
    };
})

app.controller("ResendEmailPopupController", function ($scope, RequestService, $uibModalInstance) {
    $scope.Send = function (ReEmail) {
        if (ReEmail == null) {
            alert("Please Keyin the require field!!")
        }
        else { 
        var varRemark = ReEmail.Remark;
        var varUrjent = ReEmail.Checkbox;
        if (confirm("Are you sure you want to re-send the email?"))
            RequestService.resendEmail(varrequestForms, varRemark, varUrjent).then(function (response) {
                if (response.data.ResponseType === 'success')
                    showSuccess(response.data.Message);
                else showError(response.data.Message);
                $uibModalInstance.close('close');
            });
    }
    };
    $scope.close = function () {
        $uibModalInstance.close('close')
    }


});

app.controller("cancelRequestPopupController", function ($scope, RequestService, $uibModalInstance) {

    $scope.CancelPPRF = function (CancelRemark) {

        if (confirm("Are you sure you want to mark this PO/PPRF as cancelled?")) {
            RequestService.cancelRequest(varcancelRequests, CancelRemark).then(function (response) {
                if (response.data.ResponseType === 'success') {
                    showSuccess(response.data.Message);
                    $scope.close();
                    $scope.initialize();
                }
                else if (response.data.ResponseType === 'error') showError(response.data.Message);
            });
        }
    }
    $scope.close = function () {
        $uibModalInstance.close('close')
    }

});
