(function ($) {
    $.validator.unobtrusive.parseDynamicContent = function (selector) {
        //use the normal unobstrusive.parse method
        $.validator.unobtrusive.parse(selector);

        //get the relevant form
        var form = $(selector).first().closest('form');

        //get the collections of unobstrusive validators, and jquery validators
        //and compare the two
        var unobtrusiveValidation = form.data('unobtrusiveValidation');
        var validator = form.validate();

        if (unobtrusiveValidation !== undefined) {
            $.each(unobtrusiveValidation.options.rules, function (elname, elrules) {
                if (validator.settings.rules[elname] == undefined) {
                    var args = {};
                    $.extend(args, elrules);
                    args.messages = unobtrusiveValidation.options.messages[elname];
                    //edit:use quoted strings for the name selector
                    $("[name='" + elname + "']").rules("add", args);
                } else {
                    $.each(elrules, function (rulename, data) {
                        if (validator.settings.rules[elname][rulename] == undefined) {
                            var args = {};
                            args[rulename] = data;
                            args.messages = unobtrusiveValidation.options.messages[elname][rulename];
                            //edit:use quoted strings for the name selector
                            $("[name='" + elname + "']").rules("add", args);
                        }
                    });
                }
            });
        }
    }
})($);

(function ($) {
    $.fn.clearForm = function (options) {
        // This is the easiest way to have default options.
        var settings = $.extend({
            // These are the defaults.
            formId: this.closest('form')
        }, options);

        var $form = $(settings.formId);

        //reset jQuery Validate's internals
        $form.validate().resetForm();

        //reset unobtrusive validation summary, if it exists
        $form.find("[data-valmsg-summary=true]")
            .removeClass("validation-summary-errors")
            .addClass("validation-summary-valid")
            .find("ul").empty();

        //reset unobtrusive field level, if it exists
        $form.find("[data-valmsg-replace]")
            .removeClass("field-validation-error")
            .addClass("field-validation-valid")
            .empty();
        return $form;
    };
}(jQuery));