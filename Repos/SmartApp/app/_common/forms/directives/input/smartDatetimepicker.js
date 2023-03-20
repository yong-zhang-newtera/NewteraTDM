'use strict';

angular
  .module('SmartAdmin.Forms', [])

  .provider('datetimepicker', function () {
      var default_options = {};

      this.setOptions = function (options) {
          default_options = options;
      };

      this.$get = function () {
          return {
              getOptions: function () {
                  return default_options;
              }
          };
      };
  })

  .directive('datetimepicker', [
    '$timeout',
    'datetimepicker',
    function ($timeout,
              datetimepicker) {
        var default_options = datetimepicker.getOptions();

        return {
            require: '?ngModel',
            restrict: 'AE',
            scope: {
                datetimepickerOptions: '@'
            },
            link: function ($scope, $element, $attrs, controller) {
                var passed_in_options = $scope.$eval($attrs.datetimepickerOptions);
                var options = jQuery.extend({}, default_options, passed_in_options);

                $element.on('dp.change', function (ev) {
                    $timeout(function () {
                        var dtp = $element.data("DateTimePicker");
                        controller.$setViewValue(ev.target.value);
                    });
                });

                function setPickerValue() {
                    var result = null;
                    if (!!controller && !!controller.$viewValue) {
                        result = controller.$viewValue;
                    }
                    var dtp = $element.data("DateTimePicker");
                    dtp.date(result);
                }

                controller.$render = function (value) {
                    setPickerValue();
                };

                $element.datetimepicker(options);

                setPickerValue();
            }
        };
    }
  ]);