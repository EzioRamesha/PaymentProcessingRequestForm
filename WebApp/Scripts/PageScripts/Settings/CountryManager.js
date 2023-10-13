app.service('CountryManager', function ($http) {
    this.create = function (country) {
        var promise = $http({
            url: '/Settings/CreateCountry',
            method: 'POST',
            data: country
        });
        return promise;
    };

    this.enable = function (country) {
        var promise = $http({
            url: '/Settings/EnableCountry',
            method: 'POST',
            data: country
        });
        return promise;
    };

    this.disable = function (country) {
        var promise = $http({
            url: '/Settings/DisableCountry',
            method: 'POST',
            data: country
        });
        return promise;
    };
});