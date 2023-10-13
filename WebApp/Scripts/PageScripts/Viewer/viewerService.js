

app.service('ViewerService', function ($http) {
    this.getAllRequests = function (criteria) {
        var promise = $http({
            url: '/Viewer/ListAllRequests',
            method: 'POST',
            data: criteria
        });
        return promise;
    };
});