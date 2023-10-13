app.service('PayingEntityManager', function ($http, Upload) {
    this.create = function (payingEntity) {
        var promise = Upload.upload({
            url: '/Settings/CreatePayingEntity',
            method: 'POST',
            data: payingEntity
        });
        return promise;
    };
    this.update = function (payingEntity) {
        var promise = Upload.upload({
            url: '/Settings/UpdatePayingEntity',
            method: 'POST',
            data: payingEntity
        });
        return promise;
    };

    this.enable = function (payingEntity) {
        var promise = $http({
            url: '/Settings/EnablePayingEntity',
            method: 'POST',
            data: payingEntity
        });
        return promise;
    };

    this.disable = function (payingEntity) {
        var promise = $http({
            url: '/Settings/DisablePayingEntity',
            method: 'POST',
            data: payingEntity
        });
        return promise;
    };

    this.getExistingRanges = function (payingEntityId) {
        var promise = $http({
            url: '/Settings/ManageEntityAmountRange',
            method: 'GET',
            params: { payingEntityId: payingEntityId }
        });
        return promise;
    };

    this.SavePayingEntityAmountRange = function (range) {
        var promise = $http({
            url: '/Settings/AddEntityAmountRange',
            method: 'POST',
            data: range
        });
        return promise;
    };

    this.SavePayingEntityAmountRangeEmails = function (item) {
        var promise = $http({
            url: '/Settings/AddEntityRangeEmail',
            method: 'POST',
            data: {
                rangeId: item.Id,
                Emails: item.EmailAddresses
            }
        });
        return promise;
    };
    this.getRangeDetails = function (rangeId) {
        var promise = $http({
            url: '/Settings/GetRangeDetails',
            method: 'GET',
            params: { rangeId: rangeId }
        });
        return promise;
    };

    this.RemoveEmailFromRange = function (email, rangeId) {
        var promise = $http({
            url: '/Settings/DeleteEmailFromRange',
            method: 'DELETE',
            params: {
                rangeId: rangeId,
                email: email
            }
        });
        return promise;
    }
});