function isNotNullOrUndefined(data) {
    return data !== null && data !== undefined && data != "undefined";
}

function isNullOrUndefined(data) {
    return !isNotNullOrUndefined(data);
}