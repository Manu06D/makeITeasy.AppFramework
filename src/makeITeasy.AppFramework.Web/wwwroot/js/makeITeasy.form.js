function formDataToJson(selector) {
    const formElement = getFormData(selector);
    return formToJson(formElement);
}

function getFormData(formSelector) {
    var myForm = $(formSelector);
    if (myForm.length == 0 || myForm.find('.form-control').length == 0 || !isFormValid(formSelector))
        return;

    const formElement = myForm[0];

    return formElement;
}

function formToJson(element) {
    const formDico = convertFormToDico(element);
    return convertToJson(formDico);
}

function isFormValid(elementSelector) {
    $.validator.unobtrusive.parseDynamicContent(elementSelector);
    return $(elementSelector).valid();
}

function convertFormToDico(formElement) {
    let bodyData = {};
    for (const pair of new FormData(formElement)) {
        if (pair[0] !== "__RequestVerificationToken" && pair[1] !== "") {
            var t = formElement.querySelector("[name='" + pair[0] + "']");
            if (t.hasAttribute('multiple')) {
                var tab = [];
                if (bodyData[pair[0]] != null) {
                    for (var i = 0; i < bodyData[pair[0]].length; ++i) {
                        tab.push(bodyData[pair[0]][i]);
                    }
                }
                tab.push(pair[1]);
                bodyData[pair[0]] = tab;
            }
            else {
                bodyData[pair[0]] = pair[1];
            }
        }
    }
    return bodyData;
}

function convertToJson(element) {
    return JSON.stringify(element);
}

function checkAndValidateForm(searchFormID, submitButton) {
    let formElement = document.forms[searchFormID.split("#")[1]];

    if (formElement !== undefined) {

        //let isAnyData = hasFormElementAnycontent(formElement);

        let isFormValid = validateFormBySelector(searchFormID);

        if (submitButton.length > 0) {
            submitButton.prop('disabled', !isFormValid);
        }
    }
}

//function hasFormElementAnycontent(searchFormElement) {
//    let isAnyData = false;
//    for (let element of searchFormElement.elements) {
//        if (element.attributes['type'] !== 'hidden' && element.type !== 'submit' && element.type !== 'checkbox') {
//            if (element.value.length > 0) {
//                isAnyData = true;
//                break;
//            }
//        }
//    }

//    return isAnyData;
//}

function validateFormBySelector(elementSelector) {
    var form = $(elementSelector);
    var validate = false;

    validate = isFormValid(form);

    if (validate) {
        form.clearForm();
    }

    return validate;
}

function initFormControls(searchForm, contentChangedKey, submitButton) {
    var $form = $(searchForm);

    $form
        .off('input')
        .on('input', function (e) {
            checkAndValidateForm(searchForm, submitButton);
            window[contentChangedKey] = true;
        });

    checkAndValidateForm(searchForm, submitButton);
}