function InitPage(module) {

    InitDatepicker();


    //var module = '@Model.result.tbActive';
    //if ('@string.IsNullOrEmpty(Model.result.tbActive)' == "True") {
    //    module = "product";
    //}
    var liID = "li-" + module;
    var tabID = module + "-tab";
    $("#" + liID).addClass("active");
    $("#" + tabID).addClass("active");


    // Add custom class to pagination div
    $.fn.dataTableExt.oStdClasses.sPaging = 'dataTables_paginate paging_bootstrap paging_custom';

    $('div.dataTables_filter input').addClass('form-control');
    $('div.dataTables_length select').addClass('form-control');

    /*************************************************/
    /************* DATATABLE *************/
    /*************************************************/

    /* Define two custom functions (asc and desc) for string sorting */
    jQuery.fn.dataTableExt.oSort['string-case-asc'] = function (x, y) {
        return ((x < y) ? -1 : ((x > y) ? 1 : 0));
    };

    jQuery.fn.dataTableExt.oSort['string-case-desc'] = function (x, y) {
        return ((x < y) ? 1 : ((x > y) ? -1 : 0));
    };


    /* Get the rows which are currently selected */
    function fnGetSelected(oTable01Local) {
        return oTable01Local.$('tr.row_selected');
    };

    /*************************************************/
    /************* DATATABLE *************/
    /*************************************************/

    // Add custom class to pagination div
    $.fn.dataTableExt.oStdClasses.sPaging = 'dataTables_paginate paging_bootstrap paging_custom';

    $('div.dataTables_filter input').addClass('form-control');
    $('div.dataTables_length select').addClass('form-control');

    /*************************************************/
    /************* Designation DATATABLE *************/
    /*************************************************/

    /* Define two custom functions (asc and desc) for string sorting */
    jQuery.fn.dataTableExt.oSort['string-case-asc'] = function (x, y) {
        return ((x < y) ? -1 : ((x > y) ? 1 : 0));
    };

    jQuery.fn.dataTableExt.oSort['string-case-desc'] = function (x, y) {
        return ((x < y) ? 1 : ((x > y) ? -1 : 0));
    };

    /* Add a click handler to the rows - this could be used as a callback */
    $("#productDataTable tbody tr").click(function (e) {
        if ($(this).hasClass('row_selected')) {
            $(this).removeClass('row_selected');
        }
        else {
            oTable01.$('tr.row_selected').removeClass('row_selected');
            $(this).addClass('row_selected');
        }
    });

    /* Add a click handler to the rows - this could be used as a callback */
    $("#categoryDataTable tbody tr").click(function (e) {
        if ($(this).hasClass('row_selected')) {
            $(this).removeClass('row_selected');
        }
        else {
            oTable01.$('tr.row_selected').removeClass('row_selected');
            $(this).addClass('row_selected');
        }
    });


    /* Build the DataTable with third column using our custom sort functions */
    var oTable01 = $('#productDataTable').dataTable({
        "sDom":
            "R<'row'<'col-md-6'l><'col-md-6'f>r>" +
            "t" +
            "<'row'<'col-md-4 sm-center'i><'col-md-4'><'col-md-4 text-right sm-center'p>>",
        "oLanguage": {
            "sSearch": ""
        },
        "aaSorting": [[0, 'asc']],
        "fnInitComplete": function (oSettings, json) {
            $('.dataTables_filter input').attr("placeholder", "Search");
        }
    });

    /* Build the DataTable with third column using our custom sort functions */
    var oTable01 = $('#categoryDataTable').dataTable({
        "sDom":
            "R<'row'<'col-md-6'l><'col-md-6'f>r>" +
            "t" +
            "<'row'<'col-md-4 sm-center'i><'col-md-4'><'col-md-4 text-right sm-center'p>>",
        "oLanguage": {
            "sSearch": ""
        },
        "aaSorting": [[0, 'asc']],
        "fnInitComplete": function (oSettings, json) {
            $('.dataTables_filter input').attr("placeholder", "Search");
        }
    });

    /* Get the rows which are currently selected */
    function fnGetSelected(oTable01Local) {
        return oTable01Local.$('tr.row_selected');
    };



    //initialize chosen
    $('.dataTables_length select').chosen({ disable_search_threshold: 10 });

    // Add custom class
    $('div.dataTables_filter input').addClass('form-control');
    $('div.dataTables_length select').addClass('form-control');

}

function InitDataTable(id) {

}

//function printBarcode(productCode, productName, price) {

    
//    $("#barcodeprint").barcode(productCode, "code128", { barWidth: 1.8, barHeight: 29 });
  
//    $("#itemnameprint").text(productName);
//    $("#itempriceprint").text('$ ' + price);
 
//    var contents = $("#barcodeSlipPrint").html();
    
//    var frame1 = $('<iframe />');
//    frame1[0].name = "frame1";
//    frame1.css({ "position": "absolute", "top": "-1000000px" });
//    $("body").append(frame1);
//    var frameDoc = frame1[0].contentWindow ? frame1[0].contentWindow : frame1[0].contentDocument.document ? frame1[0].contentDocument.document : frame1[0].contentDocument;
//    frameDoc.document.open();
//    //Create a new HTML document.
//    frameDoc.document.write('<html><head><title>Inventory</title>');
//    frameDoc.document.write('</head><body>');
//    frameDoc.document.write(contents);
//    frameDoc.document.write('</body></html>');
//    frameDoc.document.close();
//    setTimeout(function () {
//        window.frames["frame1"].focus();
//        window.frames["frame1"].print();
//        frame1.remove();
//    }, 500);

//};

