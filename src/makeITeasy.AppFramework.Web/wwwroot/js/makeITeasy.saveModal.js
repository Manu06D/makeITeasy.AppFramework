class saveModal {
    constructor() { }

    initModal(dataToggle) {
        let element = $(dataToggle);
        this.startLoading(element);

        let url = element.attr('data-url');
        let target = element.attr('data-target');
        let method = this.getMethod(element);
        element.prop('disabled', true);

        $[method](url).done((data) => {
            element.prop('disabled', false);

            var modal = $(target).html(data)[0];
            var modalID = modal.id;

            let contentChangedKey = 'modal_' + modalID + '_contentChanged';
            window[contentChangedKey] = false;

            var submitButton = $('#' + modalID + ' button[data-action="save"]');

            //init form
            if ($("#" + modalID + " form").length > 0) {
                var form = $("#" + modalID + " form")[0];
                var formID = "#" + form.id;

                $(target).on('shown.bs.modal', function () {
                    initFormControls(formID, contentChangedKey, submitButton);
                });

                if (submitButton.length > 0) {
                    submitButton.bind("click", function () {
                        $('#modalAlert').hide();
                        $.ajax({
                            type: "POST",
                            url: url,
                            data: formToJson(form),
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data) {
                                element.trigger("saveDataEvent");
                                $(document).trigger("dataSavedEvent");
                                window['modal_' + modalID + '_contentChanged'] = false;
                                $(target + ' > .modal').modal('hide');
                            },
                            error: function (errMsg) {
                                console.log(errMsg);
                                $('#modalAlert').show();
                            }
                        });
                    });
                }
            }

            $(target + ' > .modal').modal('show');

            // Confirm message if content has changed
            $(target + ' > .modal').on('hide.bs.modal', function (e) {

                if (window['modal_' + modalID + '_contentChanged']) {
                    if (!confirm('Des modifications ont été détectées dans le formulaire. Souhaitez-vous les annuler ?')) {
                        e.preventDefault();
                    }
                }
            });

            this.stopLoading(element);
        });
    }



    startLoading(element) {
        this.oldText = element.html();
        element.html('<i class="fa fa-spin fa-spinner"></i>');
    }

    stopLoading(element) {
        element.html(this.oldText);
    }

    getMethod(element) {
        var method = element.attr('data-method');
        if (isNullOrUndefined(method)) {
            method = "get";
        }

        return method.toLowerCase();
    }
}