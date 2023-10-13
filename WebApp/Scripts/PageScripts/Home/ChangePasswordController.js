

app.directive('passwordVerify', function () {
    return {
        restrict: 'A', // only activate on element attribute
        require: '?ngModel', // get a hold of NgModelController
        link: function (scope, elem, attrs, ngModel) {
            if (!ngModel) return; // do nothing if no ng-model

            // watch own value and re-validate on change
            scope.$watch(attrs.ngModel, function () {
                validate();
            });
            // observe the other value and re-validate on change
            attrs.$observe('passwordVerify', function (val) {
                validate();
            });
            var validate = function () {
                // values
                var val1 = ngModel.$viewValue;
                var val2 = attrs.passwordVerify;
                
                //var validity = (val1 && val1.length >= ngModel.$$attr.passwordMinLength)
                //                && (val2 && val2.length >= ngModel.$$attr.passwordMinLength)
                //                && (val1 === val2);
                // set validity
                ngModel.$setValidity('passwordVerify', val1===val2);
            };
        }
    }
});

app.service("ChangePasswordSvc", function ($http) {
    this.changePassword = function (model) {
        var promise = $http({
            url: '/Account/ChangePassword',
            method: 'POST',
            data: model
        });
        return promise;
    }
});

app.controller("ChangePasswordController", function ($scope, ChangePasswordSvc) {
    $scope.changePassword = function (model) {
        ChangePasswordSvc.changePassword(model).then(function (response) {
            if (response.data.ResponseType === "success") {
                showSuccess(response.data.Message);
            } else if(response.data.ResponseType === "error") {
                showError(response.data.Message);
                debugger;
            }
        });
    }
});