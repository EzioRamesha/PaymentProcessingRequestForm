/// <reference path="../angular.js" />


var app = angular.module("app", ['ui.bootstrap', 'ngFileUpload', 'ngAnimate']);


app.factory('httpInterceptor', function ($rootScope) {
    var loadingCount = 0;
    return {
        request: function (config) {
            var form = $('form[name="__AjaxAntiForgeryForm"]');
            var token = $('input[name="__RequestVerificationToken"]', form).val();
            //config.data.__RequestVerificationToken = token;
            //var token = $('input[name="antiForgeryToken"]', form).val();
            config.headers.RequestVerificationToken = token;
            if (++loadingCount === 1) $('#loading').css('display', 'block');
            return config;
        },
        response: function (response) {
            if (--loadingCount === 0) $('#loading').css('display', 'none');
            return response;
        },
        responseError: function (response) {
            if (--loadingCount === 0) $('#loading').css('display', 'none');
            //debugger;
            return response;
        }
    }
}).config(function ($httpProvider) {
    $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
    $httpProvider.interceptors.push('httpInterceptor');
});

app.directive('multipleEmails', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, ctrl) {
            ctrl.$parsers.unshift(function (viewValue) {

                var emails = viewValue.split(',');
                // define single email validator here
                var re = /\S+@\S+\.\S+/;

                // angular.foreach(emails, function() {
                var validityArr = emails.map(function (str) {
                    return re.test(str.trim());
                }); // sample return is [true, true, true, false, false, false]
                console.log(emails, validityArr);
                var atLeastOneInvalid = false;
                angular.forEach(validityArr, function (value) {
                    if (value === false)
                        atLeastOneInvalid = true;
                });
                if (!atLeastOneInvalid) {
                    // ^ all I need is to call the angular email checker here, I think.
                    ctrl.$setValidity('multipleEmails', true);
                    return viewValue;
                } else {
                    ctrl.$setValidity('multipleEmails', false);
                    return undefined;
                }
                // })
            });
        }
    };
});

app.directive('mmCurrency', function () {
    return {
        replace: false,
        restrict: 'A',
        transclude: false,
        scope: {
            mmPrecision: '@'
        },
        link: function (scope, element, attrs) {
            var cPrecision = parseInt(scope.mmPrecision);
            if (isNaN(cPrecision))
                cPrecision = 2;
            element.maskMoney({ precision: cPrecision, allowZero: true, allowNegative: true, decimal:'.' });
            element.addClass('text-right');
        }
    };
});

app.directive('uiMaskmoney', function ($timeout, $locale) {
    return {
        restrict: 'A',
        require: 'ngModel',
        scope: {
            model: '=ngModel',
            mmOptions: '=?',
            prefix: '@',
            suffix: '@',
            affixesStay: '=',
            thousands: '@',
            decimal: '@',
            precision: '=',
            allowZero: '=',
            allowNegative: '='
        },
        link: function (scope, el, attr, ctrl) {

            scope.$watch(checkOptions, init, true);

            scope.$watch(attr.ngModel, eventHandler, true);
            //el.on('keyup', eventHandler); //change to $watch or $observe

            function checkOptions() {
                return scope.mmOptions;
            }

            function checkModel() {
                return scope.model;
            }

            //this parser will unformat the string for the model behind the scenes
            function parser() {
                //return scope.model;
                return $(el).maskMoney('unmasked')[0];
            }

            ctrl.$parsers.push(parser);

            ctrl.$formatters.push(function (value) {
                $timeout(function () {
                    init();
                });
                var x = parseFloat(value).toFixed(scope.precision);
                return x;
            });

            function eventHandler() {
                $timeout(function () {
                    scope.$apply(function () {
                        ctrl.$setViewValue($(el).val());
                    });
                });
            }

            function init(options) {
                $timeout(function () {
                    var elOptions = {
                        prefix: scope.prefix || '',
                        suffix: scope.suffix || '',
                        affixesStay: scope.affixesStay,
                        thousands: scope.thousands || $locale.NUMBER_FORMATS.GROUP_SEP,
                        decimal: scope.decimal || $locale.NUMBER_FORMATS.DECIMAL_SEP,
                        //thousands: scope.thousands || '.',
                        //decimal: scope.decimal || ',',
                        precision: scope.precision || 2,
                        allowZero: scope.allowZero,
                        allowNegative: scope.allowNegative
                    };

                    if (!scope.mmOptions) {
                        scope.mmOptions = {};
                    }

                    for (var elOption in elOptions) {
                        if (elOptions[elOption]) {
                            scope.mmOptions[elOption] = elOptions[elOption];
                        }
                    }

                    $(el).maskMoney(scope.mmOptions);
                    $(el).maskMoney('mask');
                    eventHandler();

                }, 0);

                $timeout(function () {
                    scope.$apply(function () {
                        ctrl.$setViewValue($(el).val());
                    });
                });

            }
        }
    };
});

app.filter('range', function () {
    return function (input, min, max) {
        min = parseInt(min); //Make string input int
        max = parseInt(max);
        for (var i = min; i < max; i++)
            input.push(i);
        return input;
    };
});

app.filter('groupFilter', function () {
    return function (input, scope, value) {
        var x = [];
        if (input !== undefined && input !== null) {
            x = $.grep(input, function (obj, idx) {
                return obj.GroupName === value;
            });
        }
        if (x.length > 0) return x[0].Approvers;
        return [];
    };
});