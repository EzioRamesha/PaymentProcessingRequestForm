app.service('RequestService', function ($http, Upload) {
    this.createRequest = function (request) {
        var promise = Upload.upload({
            url: '/Pprf/SaveNewRequest',
            method: 'POST',
            data: request
        });
        return promise;
    };

    this.SaveDraft = function (request) {
        var promise = Upload.upload({
            url: '/Pprf/SaveDraft',
            method: 'POST',
            data: request
        });
        return promise;
    };

    this.DraftUpdate = function (request, Id) {
        request.RequestId = Id;
        var promise = Upload.upload({
            url: '/Pprf/DraftUpdate',
            method: 'POST',
            data: request
            //data: { model:request, requestId: Id}
        });
        return promise;
    };

    this.getInitialData = function () {
        var promise = $http({
            url: '/Pprf/GetInitialData',
            method: 'GET'
        });
        return promise;
    };
    this.getCancelPPRFReasonTypes = function () {
        var promise = $http({
            url: '/Settings/GetCancelPPRFReasonTypes',
            method: 'GET'
        });
        return promise;
    };

    this.getClosePPRFReasonTypes = function () {
        var promise = $http({
            url: '/Settings/GetClosePPRFReasonTypes',
            method: 'GET'
        });
        return promise;
    };

    this.getRejectReasonTypes = function () {
        var promise = $http({
            url: '/Settings/GetRejectTypes',
            method: 'GET'
        });
        return promise;
    };
    this.getAllDepartments = function () {
        var promise = $http({
            url: '/Settings/GetDepartments',
            method: 'GET'
        });
        return promise;
    };
    this.getBudgetPayeeData = function (payeeAccountId) {
        var promise = $http({
            url: '/Pprf/GetBudgetPayeeData',
            method: 'GET',
            params: {PayeeBankAccountDetailId: payeeAccountId}
        });
        return promise;
    };

    this.getBudgetOrderListByDate = function (pprfDate) {
        var promise = $http({
            url: '/Pprf/GetBurgetOrderListByDate',
            method: 'GET',
            params: { PPRFDate: pprfDate }
        });
        return promise;
    };

    this.getBudgetSpendingInfo = function (budgetrequestId) {
        var promise = $http({
            url: '/Pprf/GetBudgetSpendingInfo',
            method: 'GET',
            params: { BudgetrequestId: budgetrequestId }
        });
        return promise;
    }

    this.getOriginatorRequestDetails = function (requestId) {
        var promise = $http({
            url: '/Pprf/GetOriginatorRequestById',
            method: 'GET',
            params: {requestId: requestId}
        });
        return promise;
    };

    this.getRequestDetails = function (requestId) {
        var promise = $http({
            url: '/Pprf/GetRequestById',
            method: 'GET',
            params: {requestId: requestId}
        });
        return promise;
    };

    this.getApproverRequestDetails = function (requestId) {
        var promise = $http({
            url: '/Pprf/GetApproverRequestById',
            method: 'GET',
            params: {requestId: requestId}
        });
        return promise;
    };

    this.getClosedRequestDetails = function (requestId) {
        var promise = $http({
            url: '/Pprf/GetClosedRequestDetails',
            method: 'GET',
            params: { requestId: request }
        });
        return promise;
    };

    this.getApprovedRequestDetails = function (requestId) {
        var promise = $http({
            url: '/Pprf/GetApprovedRequestDetails',
            method: 'GET',
            params: { requestId: requestId }
        });
        return promise;
    }; 
    
    this.askClarifications = function (request) {
        var promise = $http({
            url: '/Pprf/AskClarification',
            method: 'POST',
            data: request
        });
        return promise;
    };
    this.giveClarification = function (request) {
        var promise = $http({
            url: '/Pprf/GiveClarification',
            method: 'POST',
            data: {
                RequestId: request.Id,
                Clarifications: request.Clarifications
            }
        });
        return promise;
    }
    this.approve = function (pendingRequest) {
        var promise = $http({
            url: '/Pprf/ApproveRequest',
            method: 'POST',
            data: pendingRequest
        });
        return promise;
    };
    this.reject = function (pendingRequest, reason) {
        var promise = $http({
            url: '/Pprf/RejectRequest',
            method: 'POST',
            data: {
                requestForm: pendingRequest,
                reason: reason
            }
        });
        return promise;
    };
    this.resendEmail = function (pendingRequest, varRemark, varUrjent) {

        var promise = $http({
            url: '/Pprf/ReSendApproverEmail',
            method: 'POST',
            data: { requestId: pendingRequest.Id, Remark: varRemark, Urjent: varUrjent }
        });
        return promise;
    };
    this.getOperators = function () {
        var promise = $http({
            url: '/Pprf/GetOperators',
            method: 'GET'
        });
        return promise;
    };
    this.update = function (requestId, newApproversList) {
        var promise = $http({
            url: '/Pprf/UpdatePPRF',
            method: 'POST',
            data: {
                RequestId: requestId,
                Approvers: newApproversList
            }
        });
        return promise;
    };

    this.askQuestion = function (approverQuestion) {
        var promise = $http({
            url: '/Pprf/AskQuestion',
            method: 'POST',
            data: approverQuestion
        });
        return promise;
    };

    this.saveAnswer = function (approverAnswer) {
        var promise = $http({
            url: '/Pprf/SaveAnswer',
            method: 'POST',
            data: approverAnswer
        });
        return promise;
    };

    this.getBankAccounts = function (payee) {
        var promise = $http({
            url: '/Settings/GetPayeeAccountDetails',
            method: 'POST',
            data: payee
        });
        return promise;
    }

    this.getRequestQueries = function (request) {
        var promise = $http({
            url: '/Pprf/GetRequestQueries',
            method: 'GET',
            params: request
        });
        return promise;
    };

    this.cancelRequest = function (request, CancelRemark) {
        var promise = $http({
            url: '/Pprf/CancelRequest',
            method: 'POST',
            data: {
                RequestId: request.RequestId, CloseReason: CancelRemark.Remark, CloseRemark: CancelRemark.CancelPPRFReasonType.Description
            }
        });
        return promise;
    };  
});