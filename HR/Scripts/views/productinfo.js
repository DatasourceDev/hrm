function InitPage(module) {
    Assembly_Type_Onchange();

    InitDatepicker();


    //var module = '@Model.result.tbActive';
    //if ('@string.IsNullOrEmpty(Model.result.tbActive)' == "True") {
    //    module = "product";
    //}
    var liID = "li-" + module;
    var tabID = module + "-tab";
    $("#" + liID).addClass("active");
    $("#" + tabID).addClass("active");


    $('input[name=Assembly_Type]').click(function () {
        Assembly_Type_Onchange();
    });

    // ****************** Required tables of style changes ****************
    /* Add a click handler to the rows - this could be used as a callback */
    $("#orderHistDataTable tbody tr").click(function (e) {
        if ($(this).hasClass('row_selected')) {
            $(this).removeClass('row_selected');
        }
        else {
            oTable01.$('tr.row_selected').removeClass('row_selected');
            $(this).addClass('row_selected');
        }
    });

    $("#movementHistDataTable tbody tr").click(function (e) {
        if ($(this).hasClass('row_selected')) {
            $(this).removeClass('row_selected');
        }
        else {
            oTable01.$('tr.row_selected').removeClass('row_selected');
            $(this).addClass('row_selected');
        }
    });

    $("#vendorDataTable tbody tr").click(function (e) {
        if ($(this).hasClass('row_selected')) {
            $(this).removeClass('row_selected');
        }
        else {
            oTable01.$('tr.row_selected').removeClass('row_selected');
            $(this).addClass('row_selected');
        }
    });

    //           $("#itemdataTables tbody tr").click(function (e) {
    //       if ($(this).hasClass('row_selected')) {
    //           $(this).removeClass('row_selected');
    //       }
    //       else {
    //           oTable01.$('tr.row_selected').removeClass('row_selected');
    //           $(this).addClass('row_selected');
    //       }
    //   });
    //
    //   $("#bomdataTables tbody tr").click(function (e) {
    //       if ($(this).hasClass('row_selected')) {
    //           $(this).removeClass('row_selected');
    //       }
    //       else {
    //           oTable01.$('tr.row_selected').removeClass('row_selected');
    //           $(this).addClass('row_selected');
    //       }
    //   });
    //
    //   $("#kitdataTables tbody tr").click(function (e) {
    //       if ($(this).hasClass('row_selected')) {
    //           $(this).removeClass('row_selected');
    //       }
    //       else {
    //           oTable01.$('tr.row_selected').removeClass('row_selected');
    //           $(this).addClass('row_selected');
    //       }
    //   });
    //   

    $("#inventoryQoHDataTable tbody tr").click(function (e) {
        if ($(this).hasClass('row_selected')) {
            $(this).removeClass('row_selected');
        }
        else {
            oTable01.$('tr.row_selected').removeClass('row_selected');
            $(this).addClass('row_selected');
        }
    });
    //   
    //   $("#colordataTables tbody tr").click(function (e) {
    //       if ($(this).hasClass('row_selected')) {
    //           $(this).removeClass('row_selected');
    //       }
    //       else {
    //           oTable01.$('tr.row_selected').removeClass('row_selected');
    //           $(this).addClass('row_selected');
    //       }
    //   });
    //
    //   $("#sizedataTables tbody tr").click(function (e) {
    //       if ($(this).hasClass('row_selected')) {
    //           $(this).removeClass('row_selected');
    //       }
    //       else {
    //           oTable01.$('tr.row_selected').removeClass('row_selected');
    //           $(this).addClass('row_selected');
    //       }
    //   });
    //   

    /* Build the DataTable with third column using our custom sort functions */
    var oTable01 = $('#orderHistDataTable').dataTable({
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
    var oTable01 = $('#movementHistDataTable').dataTable({
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
    var oTable01 = $('#vendorDataTable').dataTable({
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

    //    
    //    /* Build the DataTable with third column using our custom sort functions */
    //    var oTable01 = $('#itemdataTables').dataTable({
    //        "sDom":
    //            "R<'row'<'col-md-6'l><'col-md-6'f>r>" +
    //            "t" +
    //            "<'row'<'col-md-4 sm-center'i><'col-md-4'><'col-md-4 text-right sm-center'p>>",
    //        "oLanguage": {
    //            "sSearch": ""
    //        },
    //        "aaSorting": [[0, 'asc']],
    //        "fnInitComplete": function (oSettings, json) {
    //            $('.dataTables_filter input').attr("placeholder", "Search");
    //        }
    //    });
    //
    //    /* Build the DataTable with third column using our custom sort functions */
    //    var oTable01 = $('#bomdataTables').dataTable({
    //        "sDom":
    //            "R<'row'<'col-md-6'l><'col-md-6'f>r>" +
    //            "t" +
    //            "<'row'<'col-md-4 sm-center'i><'col-md-4'><'col-md-4 text-right sm-center'p>>",
    //        "oLanguage": {
    //            "sSearch": ""
    //        },
    //        "aaSorting": [[0, 'asc']],
    //        "fnInitComplete": function (oSettings, json) {
    //            $('.dataTables_filter input').attr("placeholder", "Search");
    //        }
    //    });
    //
    //    /* Build the DataTable with third column using our custom sort functions */
    //    var oTable01 = $('#kitdataTables').dataTable({
    //        "sDom":
    //            "R<'row'<'col-md-6'l><'col-md-6'f>r>" +
    //            "t" +
    //            "<'row'<'col-md-4 sm-center'i><'col-md-4'><'col-md-4 text-right sm-center'p>>",
    //        "oLanguage": {
    //            "sSearch": ""
    //        },
    //        "aaSorting": [[0, 'asc']],
    //        "fnInitComplete": function (oSettings, json) {
    //            $('.dataTables_filter input').attr("placeholder", "Search");
    //        }
    //    });
    //    

    /* Build the DataTable with third column using our custom sort functions */
    var oTable01 = $('#inventoryQoHDataTable').dataTable({
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

    //   /* Build the DataTable with third column using our custom sort functions */
    //   
    //   var oTable01 = $('#colordataTables').dataTable({
    //       "sDom":
    //           "R<'row'<'col-md-6'l><'col-md-6'f>r>" +
    //           "t" +
    //           "<'row'<'col-md-4 sm-center'i><'col-md-4'><'col-md-4 text-right sm-center'p>>",
    //       "oLanguage": {
    //           "sSearch": ""
    //       },
    //       "aaSorting": [[0, 'asc']],
    //       "fnInitComplete": function (oSettings, json) {
    //           $('.dataTables_filter input').attr("placeholder", "Search");
    //       }
    //   });
    //
    //   /* Build the DataTable with third column using our custom sort functions */
    //   var oTable01 = $('#sizedataTables').dataTable({
    //       "sDom":
    //           "R<'row'<'col-md-6'l><'col-md-6'f>r>" +
    //           "t" +
    //           "<'row'<'col-md-4 sm-center'i><'col-md-4'><'col-md-4 text-right sm-center'p>>",
    //       "oLanguage": {
    //           "sSearch": ""
    //       },
    //       "aaSorting": [[0, 'asc']],
    //       "fnInitComplete": function (oSettings, json) {
    //           $('.dataTables_filter input').attr("placeholder", "Search");
    //       }
    //   });
    //   

    //initialize chosen
    $('.dataTables_length select').chosen({ disable_search_threshold: 10 });

    // Add custom class
    $('div.dataTables_filter input').addClass('form-control');
    $('div.dataTables_length select').addClass('form-control');

    $('.input-color').colorpicker();

}

function Assembly_Type_Onchange() {
    if ($('input[name=Assembly_Type]:checked').val() == '@ProductInfoType.Kit') {
        $('#aks').show();
        $('#abom').hide();
    } else if ($('input[name=Assembly_Type]:checked').val() == "@ProductInfoType.Bom") {
        $('#abom').removeAttr('style');
        $('#aks').hide();
    } else {
        $('#aks').hide();
        $('#abom').hide();
    }
}




