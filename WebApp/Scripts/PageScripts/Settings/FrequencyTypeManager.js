app.service('FrequencyTypeManager', function ($http) {
    this.create = function (frequencyType) {
        var promise = $http({
            url: '/Settings/CreateFrequencyType',
            data: frequencyType,
            method: 'POST'
        });
        return promise;
    };

    this.update = function (frequencyType) {
        var promise = $http({
            url: '/Settings/UpdateFrequencyType',
            data: frequencyType,
            method: 'POST'
        });
        return promise;
    };

    this.enable = function (frequencyType) {
        var promise = $http({
            url: '/Settings/EnableFrequencyType',
            data: frequencyType,
            method: 'POST'
        });
        return promise;
    };

    this.disable = function (frequencyType) {
        var promise = $http({
            url: '/Settings/DisableFrequencyType',
            data: frequencyType,
            method: 'POST'
        });
        return promise;
    };
});