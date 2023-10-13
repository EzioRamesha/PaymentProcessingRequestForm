/// <reference path="../../angular.js" />
/// <reference path="../Settings/PayingEntityManager.js" />
var varrequestForms = '';
var varNotesForms = '';
app.service("ApprovedRequestService", function ($http) {
    this.getApprovedRequestsForClosing = function (searchCriteria) {
        var promise = $http({
            url: '/Home/ApprovedFormsForClosure',
            method: 'POST',
            data: searchCriteria
        });
        return promise;
    };
    this.getClosedForms = function (searchCriteria) {
        var promise = $http({
            url: '/Home/GetClosedForms',
            method: 'POST',
            data: searchCriteria
        });
        return promise;
    };
    this.getAllForms = function (searchCriteria) {
        var promise = $http({
            url: '/Home/GetAllForms',
            method: 'POST',
            data: searchCriteria
        });
        return promise;
    };

    this.closeRequest = function (request,CloseRequestRemarks) {
        var promise = $http({
            url: '/Pprf/CloseRequest',
            method: 'POST',
            data: {
                requestForm: request, CloseReason: CloseRequestRemarks.CloseRequestReason.Description, CloseRemark: CloseRequestRemarks.Remark
               
                  }
        });
        return promise;
    };

    this.saveNotes = function (request, varNotes) {
        var promise = $http({
            url: '/Pprf/SaveNotesRequest',
            method: 'POST',
            data: {
                requestForm: request, Notes: varNotes
            }
        });
        return promise;
    };

    this.getClosedRequestDetails = function (requestId) {
        var promise = $http({
            url: '/Pprf/GetClosedRequestDetails',
            method: 'GET',
            params: { requestId: requestId }
        });
        return promise;
    };

    this.exportToExcel = function (searchCriteria) {
        var promise = $http({
            url: '/Home/ExportClosedForms',
            method: 'POST',
            data: searchCriteria
        });
        return promise;
    };


    this.getFilterCriteria = function () {
        var promise = $http({
            url: '/Home/FilterCriteria',
            method: 'GET'
        });
        return promise;
    }
});



var months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
app.controller('closeRequest', function ($scope, ApprovedRequestService, RequestService, $uibModal) {
    var searchCriteria = {
        Pager: {
            ItemsPerPage: 10,
            CurrentPage: 1
        },
        Filters: {
            PayingEntityId: '',
            Status: '',
            StartDate: '',
            EndDate: ''
        },
        Sort: {
            SortParameter: '',
            SortDirection: ''
        }
    };
    $scope.searchCriteria = searchCriteria;



    $scope.getApprovedForms = function (searchCriteria) {
        ApprovedRequestService.getApprovedRequestsForClosing(searchCriteria).then(function (response) {
            $scope.searchCriteria.Pager = response.data.Pager;
            $scope.approvedRequests = response.data.RequestForms;
        });
    };
    getClosedForms = function (searchCriteria) {
        ApprovedRequestService.getAllForms(searchCriteria).then(function (response) {
            $scope.closedRequestsSearchCriteria.Pager = response.data.Data.Pager;
            $scope.closedRequests = response.data.Data.RequestForms;
        });
    };

    $scope.initialize = function () {
        var closedRequestsSearchCriteria = {
            Pager: {
                ItemsPerPage: 10,
                CurrentPage: 1
            },
            Filters: {
                PayingEntityId: '',
                DepartmentId: '',
                PayeeId: '',
                PPRFNo: '',
                Status: '',
                StartDate: '',
                EndDate: ''
            },
            Sort: {
                SortParameter: '',
                SortDirection: ''
            }
        };
        var currentDate = new Date();
        var firstDay = new Date(
                            currentDate.getFullYear(),
                            currentDate.getMonth(), 1);
        closedRequestsSearchCriteria.Filters.StartDate = firstDay;

        var lastDay = new Date(
                            currentDate.getFullYear(),
                            currentDate.getMonth() + 1, 0);
        closedRequestsSearchCriteria.Filters.EndDate = lastDay;

        $scope.closedRequestsSearchCriteria = closedRequestsSearchCriteria;

        $scope.startDateOptions = {
            formatYear: 'yyyy',
            maxDate: new Date(2099, 12, 31),
            initDate: firstDay,
            startingDay: 1
        };

        $scope.endDateOptions = {
            formatYear: 'yyyy',
            maxDate: new Date(2099, 12, 31),
            initDate: lastDay,
            startingDay: 1
        };

        $scope.closedRequestsSearchCriteria.Sort.SortParameter = 'PprfDate';
        $scope.closedRequestsSearchCriteria.Sort.SortDirection = 'asc';

        ApprovedRequestService.getFilterCriteria().then(function (response) {
           
            $scope.Statuses = response.data.Statuses;
            $scope.PayingEntities = response.data.PayingEntities;
            $scope.Departments = response.data.Departments;
            $scope.Payees = response.data.Payees;
            
        });
        $scope.getApprovedForms($scope.searchCriteria);

        //$scope.filterClosedFormsData(closedRequestsSearchCriteria);
        $scope.ClosePPRFReasonTypes = [];
        $scope.loadClosePPRFReasonTypes();
    };

    $scope.loadClosePPRFReasonTypes = function () {
        RequestService.getClosePPRFReasonTypes().then(function (response) {
            $scope.ClosePPRFReasonTypes = response.data;
        });
    }


    //$scope.closeRequest = function (request) {
    //    ApprovedRequestService.closeRequest(request).then(function (response) {
    //        if (response.data.ResponseType === "success") {
    //            showSuccess(response.data.Message);
    //            $scope.initialize();
    //        } else if (response.data.ResponseType === "error")
    //            showError(response.data.Message);
    //    });
    //};

    $scope.closeRequestDialog = function (approvedRequest) {
        varrequestForms = approvedRequest;
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/CloseRequest/CloseRequest.html',
            controller: 'CloseRequestPopupController',
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

    $scope.NotesDialog = function (approvedRequest) {
        varNotesForms = approvedRequest;
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Templates/Notes/Notes.html',
            controller: 'NotesPopupController',
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

    $scope.viewApprovedRequestDetails = function (requestForm) {
        RequestService.getApprovedRequestDetails(requestForm.Id).then(function (response) {
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
    $scope.viewRequestDetails = function (requestForm) {
        RequestService.getRequestDetails(requestForm.Id).then(function (response) {
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
    $scope.viewDetailsClosedRequest = function (requestForm) {
        RequestService.getClosedRequestDetails(requestForm).then(function (response) {
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

    $scope.exportToExcel = function (closedRequestsSearchCriteria) {
        ApprovedRequestService.exportToExcel(closedRequestsSearchCriteria);
    }
    $scope.filterClosedFormsData = function (closedRequestsSearchCriteria) {
        getClosedForms(closedRequestsSearchCriteria);
    }


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

    $scope.$watch('closedRequestsSearchCriteria.Filters.StartDate', function () {
        if ($scope.closedRequestsSearchCriteria.Filters.StartDate) {
            var minDate = $scope.closedRequestsSearchCriteria.Filters.StartDate.addDays(1);
            if ($scope.closedRequestsSearchCriteria.Filters.EndDate < minDate) {
                $scope.closedRequestsSearchCriteria.Filters.EndDate = minDate;
            }
        }
    });
    $scope.$watch('closedRequestsSearchCriteria.Sort.SortDirection', function () {
        getClosedForms($scope.closedRequestsSearchCriteria);
    });

    //$(document).ready(function () {
    //    $('select').selectize({
    //        sortField: 'text'
    //    });
    //});
});


app.controller('ViewController', function ($scope, $uibModalInstance) {
    $scope.months = months.slice();
    $scope.close = function () {
        $uibModalInstance.close();
    };
    $scope.printRequest = function (requestId) {
        window.open("/Pprf/PrintRequest/?requestId=" + requestId);
    };
});

app.controller('CloseRequestPopupController', function ($scope, ApprovedRequestService, RequestService, $uibModalInstance) {
    $scope.Close = function (CloseRequestRemarks) {
        if (CloseRequestRemarks == null) {
            alert("Please Keyin the require field!!")
        }
        else {
            if (CloseRequestRemarks.CloseRequestRemark == 'Cancelled(duly approved).' | CloseRequestRemarks.CloseRequestRemark == 'Others.') {
                alert("Please Keyin the require field!!")
            }
            else {
                var varCloseRequestRemarks = CloseRequestRemarks.CloseRequestRemarks;
                var varRemark = CloseRequestRemarks.Remark;
                if (confirm("Are you sure you want to Close This Request?"))

                    ApprovedRequestService.closeRequest(varrequestForms,CloseRequestRemarks).then(function (response) {
                        if (response.data.ResponseType === "success") {
                            showSuccess(response.data.Message);
                            $scope.initialize();
                        } else if (response.data.ResponseType === "error")
                            showError(response.data.Message);
                    });
                $uibModalInstance.close('close')
            }
            
        }
    };
    $scope.Cancel = function () {
        $uibModalInstance.close('close')
    }


});

app.controller('NotesPopupController', function ($scope, ApprovedRequestService, $uibModalInstance) {
    $scope.Save = function (Notes) {
        if (Notes == null) {
            alert("Please Keyin the require field!!")
        }
        else {

            var varNotes = Notes.Remark;
         
            ApprovedRequestService.saveNotes(varNotesForms, varNotes).then(function (response) {
                    if (response.data.ResponseType === "success") {
                        showSuccess(response.data.Message);
                        $scope.initialize();
                    } else if (response.data.ResponseType === "error")
                        showError(response.data.Message);
                });
            $uibModalInstance.close('close')

        }
        
    };
    $scope.Cancel = function () {
        $uibModalInstance.close('close')
    }


});