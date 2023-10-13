app.service('PaymentMethodManager', function ($http) {
    this.create = function (paymentMethod) {
        var promise = $http({
            url: '/Settings/CreatePaymentMethod',
            data: paymentMethod,
            method: 'POST'
        });
        return promise;
    };

    this.update = function (paymentMethod) {
        var promise = $http({
            url: '/Settings/UpdatePaymentMethod',
            data: paymentMethod,
            method: 'POST'
        });
        return promise;
    };

    this.enable = function (paymentMethod) {
        var promise = $http({
            url: '/Settings/EnablePaymentMethod',
            data: paymentMethod,
            method: 'POST'
        });
        return promise;
    };

    this.disable = function (paymentMethod) {
        var promise = $http({
            url: '/Settings/DisablePaymentMethod',
            data: paymentMethod,
            method: 'POST'
        });
        return promise;
    };
});