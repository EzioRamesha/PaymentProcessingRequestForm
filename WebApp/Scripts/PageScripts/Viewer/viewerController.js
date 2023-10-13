

app.controller('viewerController', function ($scope, $uibModal, ViewerService) {
    $scope.initialize = function () {
        $scope.getRequests();
    };

    $scope.getRequests = function () {
        ViewerService.getAllRequests().then(function (response) {
            if (response && response.data.ResponseType === 'success') {
                $scope.requests = response.data.Data.RequestForms;
            }
        });
    };

    //$scope.requestDetails = function () {
    //    ViewerService.getRequestDetails().then(function () {
    //        if (response && response.data.ResponseType === 'success') {

    //        }
    //    });
    //}
});