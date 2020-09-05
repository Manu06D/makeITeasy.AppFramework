function initDatatable(datatableID, searchFormID, columnRenderers) {

    tableModel = this['tableObject' + datatableID];
    var datatableSelector = "#" + datatableID;

    let options = buildOption(columnRenderers, tableModel);

    if (searchFormID != null) {

        options.ajax.data = function (d) {
            d.appJson = formDataToJson(searchFormID);
        };

        $(searchFormID).submit(function (event) {
            var myDataTable = $(datatableSelector).DataTable();

            if (isNotNullOrUndefined(myDataTable.settings()[0])) {
                if (myDataTable.settings()[0].oFeatures.bServerSide) {
                    myDataTable.ajax.reload();
                }
                else {
                    myDataTable.settings()[0].oFeatures.bServerSide = true;
                    myDataTable.ajax.reload();
                    myDataTable.settings()[0].oFeatures.bServerSide = false;
                }

                $(document).ajaxStop(function () {
                    $(datatableSelector).DataTable().columns.adjust();
                });
            }
        });
    }

    var table = $(datatableSelector).DataTable(options);


    table.on('xhr', function (e, settings, json) {
        if (tableModel.Options.ActivateDoubleClickOnRow == true) {
            $(datatableSelector + ' tbody').on('dblclick', 'tr', function () {
                var data = table.row(this);
                $(datatableSelector).trigger("dblClickEvent", [data.data(), data.id()]);
            });
        }

        $(datatableSelector).trigger("dblDataLoadEvent");
    });

    table.on('draw', function () {
        $(datatableSelector).trigger("dblDrawEvent");
    });
}

function datatableReload(datatableID) {
    tableModel = this['tableObject' + datatableID];
    var datatableSelector = "#" + datatableID;
    var myDataTable = $(datatableSelector).DataTable();
    myDataTable.ajax.reload();
}

function buildOption(renderColumns, tableModel) {

    var options = {
        "order": [[0, 'asc']],
        columns: [],
    };

    tableModel.Columns.forEach(element => options.columns.push(convertToDatableColumn(element, renderColumns)));

    options.ajax = {
        "url": tableModel.ApiUrl,
        "type": "POST",
        "datatype": "json",
        "error": function (xhr, error, thrown) {
        }
    }

    if (options.columns != undefined && options.columns.length > 0) {
        let columnRawId = options.columns.filter(function (column) { return column.isRowId; })[0] || null;
        if (columnRawId != null) {
            options.rowId = columnRawId.data;
        }
    }

    let mergedOptions = Object.assign(options, tableModel.Options);

    return mergedOptions;
}

function convertToDatableColumn(column, columnRenderers) {

    let dataObject = {
        "data": column.Name,
        "name": column.Title,
        "autoWidth": column.AutoWidth,
        "visible": column.Visible,
        "orderable": column.Orderable,
        "isRowId": column.IsRowId
    };

    if (columnRenderers != null) {
        template = columnRenderers[dataObject.data];
        if (template != null) {
            dataObject.render = template;
        }
    }

    return dataObject;
}
