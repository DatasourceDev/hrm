$(document).ready(function () {
   $('<input/>').attr({ type: 'hidden', name: 'unsaved', id: 'unsaved' }).val(false).appendTo('#content');

   $(".chosen-select").chosen({ disable_search_threshold: 10 });
   $('form').on("submit", function (event) {
      try {

         $("#unsaved").val(false);
         if ($(this).get(0).getAttribute('target') == "_blank")
            return;
         if ($(this).valid())
            ShowMask();
      }
      catch (err) {

      }
   });
   $('button').on("click", function (event) {

   });

   $('a').on("click", function (event) {
      if ($(this).get(0).getAttribute('data-toggle') != null)
         return;
      if ($(this).get(0).getAttribute('data-modal') != null)
         return;
      if ($(this).hasClass("chosen-single") == true)
         return;
      if (event.isDefaultPrevented() == true)
         return;
      if ($(this).get(0).href == null || $(this).get(0).href == '')
         return;
      if ($(this).get(0).href.indexOf('#') != -1)
         return;
      if ($(this).get(0).getAttribute('target') == "_blank")
         return;
      var unsaved = $("#unsaved").val();
      if (unsaved == "true") {
         if (!$(this).hasClass('btn btn-default')) { //btn type a href : Cancel ,Reject  
            if (!confirm("You have unsaved changes on this page. Do you want to leave this page and discard your changes or stay on this page?")) {
               //$(":input[type=submit]:visible:first").focus();
               $(":input[type=text]:visible:first").focus();
               CloseMask();
               return false;
            }
         }
      }
      ShowMask(true);
   });
   InitInputChange();
});


function InitInputChange() {
   $(":input").change(function () {
      var path = window.location.pathname;
      if (path.indexOf("/Expenses/Application") >= 0) { //Expenses only
         $("#unsaved").val(true);
      }
      else if (path.indexOf("/TsEx/TsExInfo") >= 0) {
         $("#unsaved").val(true);
      } else {
         $("#unsaved").val(false);
      }
   });
}

function HandleBackFunctionality() {
   if (window.event) {
      if (window.event.clientX < 40 && window.event.clientY < 0) {
         alert("Browser back button is clicked...");
      }
      else {
         alert("Browser refresh button is clicked...");
      }
   }
   else {
      if (event.currentTarget.performance.navigation.type == 1) {
         alert("Browser refresh button is clicked...");
      }
      if (event.currentTarget.performance.navigation.type == 2) {
         alert("Browser back button is clicked...");
      }
   }
}

function ShowMask(dohide) {
   doloadmask();
   if (dohide == true)
      setTimeout(checkloading(), 5000);

}
function checkloading() {
   //CloseMask();
}

function doloadmask() {
   CloseMask();
   if ($('#loading-bg').get(0) == null) {
      var modal = $('<div id="loading-bg" onclick="loadingbg_onclick()" />');
      modal.addClass("modal-loading");
      $('body').append(modal);
      var loading = $(".loading");
      loading.show();
      var top = Math.max($(window).height() / 2 - loading[0].offsetHeight / 2, 0);
      var left = Math.max($(window).width() / 2 - loading[0].offsetWidth / 2, 0);
      loading.css({ top: top, left: left });
   }
}

function CloseMask() {
   $("#loading-bg").remove();
   var loading = $(".loading");
   loading.hide();
}

function loadingbg_onclick() {
   //CloseMask();
}

function SaveBg(bg) {
   ShowMask();
   $.ajax({
      type: 'POST',
      url: '../Account/SaveBg',
      contentType: 'application/json; charset=utf-8',
      data: JSON.stringify({
         bg: bg
      }),
      dataType: 'json',
      success: function (data) {
         CloseMask();
      }
   });
}

//function SaveSkipTutorial(IsSkip) {
//    ShowMask();
//    $.ajax({
//        type: 'POST',
//        url: '../Account/SaveSkipTutorial',
//        contentType: 'application/json; charset=utf-8',
//        data: JSON.stringify({
//            IsSkip: IsSkip
//        }),
//        dataType: 'json',
//        success: function (data) {
//            CloseMask();
//        }
//    });
//}

function Combo_Reload(id, type, param, param2) {
   ShowMask();
   $.ajax({
      type: 'POST',
      url: '../Combo/Reload',
      contentType: 'application/json; charset=utf-8',
      data: JSON.stringify({ type: type, param: param, param2: param2 }),
      dataType: 'json',
      success: function (data) {
         var result = '';
         for (var i = 0, iL = data.length; i < iL; i++) {
            result += '<option value="' + data[i].Value + '">' + data[i].Text + '</option>';
         }
         $('#' + id).html(result);
         $('#' + id).trigger("chosen:updated");
         CloseMask();
      }
   });
}

function logout() {
   window.location.href = '../Account/Logout';
}


function Input_Onblur(ctlname) {
   var val = $("#" + ctlname).val();
   if (val == null || val == '') {
      MandatoryColor(ctlname);
   }
   else {
      MandatoryRemoveColor(ctlname);
   }
}

//$(document).ajaxStart(function () {
//    if (dlgloading == true)
//    {
//        abort();
//        $(document).ajaxStop();
//    }
//    else {
//        //ShowMask();
//        dlgloading = true;
//    }

//});

//$(document).ajaxComplete(function () {
//    // CloseMask();
//    dlgloading = false;
//});
function doStuff() {
   $.ajax({
      type: "Get",
      url: '../Account/CheckLogin',
   }).done(function (data) {
      if (data <= 60) {
         startLogout(data);
      } else {
         setTimeout(doStuff, 180000); //every three mins
      }
   });

}

function startLogout(seconds) {
   if ($("#warning-timeout-dlg-bg").get(0) == null) {
      ShowWarningTimeoutDlg();
      setTimeout(doStuff, 180000);
   }
   else {
      //Goto session timeout page
      CloseWarningTimeoutDlg();
      $.ajax({
         type: "Get",
         url: '../Account/CheckLogin',
      }).done(function (data) {
         if (data <= 60) {
            window.location.href = '../Account/SessionLogout';
         }
      });

   }
}

function ContinousLogin() {
   $.ajax({
      type: "Get",
      url: '../Account/ContinousLogin',
   }).done(function () {
      CloseWarningTimeoutDlg();
   });

}

function ShowWarningTimeoutDlg() {
   setTimeout(function () {

      var modal = $('<div id="warning-timeout-dlg-bg"  />');
      modal.addClass("modal-warning-timeout-dlg");
      $('body').append(modal);
      var loading = $(".warning-timeout-dlg");
      loading.show();
      //var top = Math.max($(window).height() / 2 - loading[0].offsetHeight / 2, 0);
      //var left = Math.max($(window).width() / 2 - loading[0].offsetWidth / 2, 0);
      //loading.css({ top: top, left: left });
   }, 200);
}
function CloseWarningTimeoutDlg() {
   setTimeout(function () {
      $("#warning-timeout-dlg-bg").remove();
      var loading = $(".warning-timeout-dlg");
      loading.hide();
   }, 200);
}



function hexToR(h) { return parseInt((cutHex(h)).substring(0, 2), 16) }
function hexToG(h) { return parseInt((cutHex(h)).substring(2, 4), 16) }
function hexToB(h) { return parseInt((cutHex(h)).substring(4, 6), 16) }
function cutHex(h) { return (h.charAt(0) == "#") ? h.substring(1, 7) : h }

function convertHextoRgb(hex, optical) {
   var R = hexToR(hex);
   var G = hexToG(hex);

   var B = hexToB(hex);

   if (optical == null) {
      optical = 1;
   }
   var rgb = "rgba(" + R + ", " + G + ", " + B + ", " + optical + ")";
   return rgb;
}

function InitDatepicker() {
   //initialize datepicker

   $('.input-datepicker').datetimepicker({
      icons: {
         time: "fa fa-clock-o",
         date: "fa fa-calendar",
         up: "fa fa-arrow-up",
         down: "fa fa-arrow-down"
      },
      format: 'DD/MM/YYYY',
      useCurrent: false,
      pickTime: false
   });

   //$(".input-datepicker").datetimepicker();
   //$(".input-datepicker").datetimepicker("option", "dateFormat", "dd/mm/yy");

   $('.input-datepicker').on("dp.show", function (e) {

      for (var i = 0; i < $('.bootstrap-datetimepicker-widget').length  ; i++) {
         var d = $('.bootstrap-datetimepicker-widget').get(i);
         if (d != null) {
            if (d.style.display != null && d.style.display != "" && d.style.display != "none") {
               var top = parseInt(d.style.top.replace("px", ""));
               var newtop = top - 45;
               $('.bootstrap-datetimepicker-widget').css('top', newtop + 'px');
               break;
            }
         }
      }


   });


   //initialize datepicker
   $('.input-datepicker2').datetimepicker({
      icons: {
         time: "fa fa-clock-o",
         date: "fa fa-calendar",
         up: "fa fa-arrow-up",
         down: "fa fa-arrow-down"
      },
      format: 'DD/MM/YYYY',
      useCurrent: false,
      pickTime: false
   });

   $('.input-timepicker2').datetimepicker({
      icons: {
         time: "fa fa-clock-o",
         date: "fa fa-calendar",
         up: "fa fa-arrow-up",
         down: "fa fa-arrow-down"
      },
      format: 'HH:mm',
      pickDate: false,
      useCurrent: false
   });

   $('.input-timepicker').datetimepicker({
      icons: {
         time: "fa fa-clock-o",
         date: "fa fa-calendar",
         up: "fa fa-arrow-up",
         down: "fa fa-arrow-down"
      },
      format: 'HH:mm',
      pickDate: false,
      useCurrent: false
   });

   $('.input-timepicker').on("dp.show", function (e) {

      for (var i = 0; i < $('.bootstrap-datetimepicker-widget').length  ; i++) {
         var d = $('.bootstrap-datetimepicker-widget').get(i);
         if (d != null) {
            if (d.style.display != null && d.style.display != "" && d.style.display != "none") {
               var top = parseInt(d.style.top.replace("px", ""));
               var newtop = top - 45;
               $('.bootstrap-datetimepicker-widget').css('top', newtop + 'px');
               break;
            }
         }
      }


   });
}

//Using in promotion
function InitDatepickerByID(cid) {
   if (cid.indexOf('#') < 0) {
      cid = '#' + cid;
   }
   $(cid).datetimepicker({
      icons: {
         time: "fa fa-clock-o",
         date: "fa fa-calendar",
         up: "fa fa-arrow-up",
         down: "fa fa-arrow-down"
      },
      format: 'DD/MM/YYYY',
      useCurrent: false,
      pickTime: false
   });

   $(cid).on("dp.show", function (e) {
      for (var i = 0; i < $('.bootstrap-datetimepicker-widget').length  ; i++) {
         var d = $('.bootstrap-datetimepicker-widget').get(i);
         if (d != null) {
            if (d.style.display != null && d.style.display != "" && d.style.display != "none") {
               var top = parseInt(d.style.top.replace("px", ""));
               var newtop = top - 45;
               $('.bootstrap-datetimepicker-widget').css('top', newtop + 'px');
               break;
            }
         }
      }
   });
}

function Visible(cid) {
   $('#' + cid).hide();
}


function Disable(cid) {
   if (cid.indexOf('#') < 0)
      cid = '#' + cid;

   $(cid + ' .input-datepicker').attr("disabled", "disabled");
   if ($(cid + ' .input-datepicker') != null && $(cid + ' .input-datepicker').length > 0) {
      for (var i = 0 ; i < $(cid + ' .input-datepicker').length; i++) {
         var obj = $(cid + ' .input-datepicker').get(i);
         var hid = "<input type='hidden' name='" + obj.name + "' value='" + obj.value + "' />";
         $(cid).append(hid);
      }
   }
   $(cid + ' .input-datepicker2').attr("disabled", "disabled");
   if ($(cid + ' .input-datepicker2') != null && $(cid + ' .input-datepicker2').length > 0) {
      for (var i = 0 ; i < $(cid + ' .input-datepicker2').length; i++) {
         var obj = $(cid + ' .input-datepicker2').get(i);
         var hid = "<input type='hidden' name='" + obj.name + "' value='" + obj.value + "' />";
         $(cid).append(hid);
      }
   }

   $(cid + ' input').attr("readonly", "true");
   $(cid + ' input[type="file"]').attr("disabled", "disabled");
   $(cid + ' .uploader').attr("disabled", "disabled");
   $(cid + ' button').attr("disabled", "disabled");
   $(cid + ' textarea').attr("readonly", "true");

   if ($(cid + ' a') != null && $(cid + ' a').length > 0) {
      for (var i = 0 ; i < $(cid + ' a').length; i++) {
         var a = $(cid + ' a').get(i);
         if (a.className.indexOf("link-allow-click") < 0 && a.className.indexOf("chosen") < 0) {
            if (a.onclick != null) a.onclick = "return false;";
            if (a.href != null) a.href = "#";
         }

      }
   }

   $(cid + ' select').attr("readonly", "true");
   $(cid + ' select').prop('disabled', true);
   $(cid + ' select').trigger("chosen:updated");

   if ($(cid + ' select') != null && $(cid + ' select').length > 0) {
      for (var i = 0 ; i < $(cid + ' select').length; i++) {
         var select = $(cid + ' select').get(i);
         if (select != null) {
            var hid = "<input type='hidden' name='" + select.name + "' value='" + select.value + "' />";
            $(cid).append(hid);
         }

      }
   }


   $(cid + ' input[type=checkbox]').on("change", function (event) {
      var chk = this;
      if (chk.checked == true) {
         chk.checked = false;
      }
      else {
         chk.checked = true;
      }
   });


   if ($(cid + ' input[type=radio]') != null && $(cid + ' input[type=radio]').length > 0) {
      for (var i = 0 ; i < $(cid + ' input[type=radio]').length; i++) {
         var rdo = $(cid + ' input[type=radio]').get(i);
         if (rdo != null) {
            if (rdo.checked == true) {
               var hid = "<input type='hidden' name='" + rdo.name + "' value='" + rdo.value + "' />";
               $(cid).append(hid);
            }
         }

      }
   }

   $(cid + ' input[type=radio]').prop('disabled', true);

   if ($(cid).is("li")) {
      $(cid).addClass('disabled');
   }

   //var loadingchk = false;
   //$(cid + ' input[type=radio]').on("change", function (event) {
   //    var chk = this;
   //    var allradio = $('input[Name=' + chk.name + ']');
   //    if (allradio != null && loadingchk == false) {
   //        var checked = chk.checked;
   //        loadingchk = true;
   //        for (i = 0; i < allradio.length; i++) {
   //            if (chk == allradio[i]) {
   //                if (checked == true) {
   //                    chk.checked = false;
   //                }
   //                else {
   //                    chk.checked = true;
   //                }
   //            }
   //            else {
   //                if (checked == true) {
   //                    allradio[i].checked = true;
   //                }
   //                else {
   //                    allradio[i].checked = false;
   //                }
   //            }
   //        }
   //        loadingchk = false;
   //    }

   //});
}
function Enable(cid) {
   if (cid.indexOf('#') < 0)
      cid = '#' + cid;

   $(cid + ' input').removeAttr("readonly");
   $(cid + ' input[type="file"]').removeAttr("disabled");
   $(cid + ' .uploader').removeAttr("disabled");
   $(cid + ' button').removeAttr("disabled");
   $(cid + ' textarea').removeAttr("readonly");


   if ($(cid + ' select') != null && $(cid + ' select').length > 0) {
      for (var i = 0 ; i < $(cid + ' select').length; i++) {

         var select = $(cid + ' select').get(i);
         if (select != null) {
            var hid = $(cid + ' input[type="hidden"][name=' + select.name + ']');
            if (hid != null)
               hid.remove();
         }

      }
   }

   $(cid + ' select').removeAttr("readonly");
   $(cid + ' select').removeAttr('disabled');
   $(cid + ' select').trigger("chosen:updated");

   $(cid + ' .input-datepicker').removeAttr("disabled");
   if ($(cid + ' .input-datepicker') != null && $(cid + ' .input-datepicker').length > 0) {
      for (var i = 0 ; i < $(cid + ' .input-datepicker').length; i++) {
         var obj = $(cid + ' .input-datepicker').get(i);
         var hid = $(cid + ' input[type="hidden"][name=' + obj.name + ']');
         if (hid != null)
            hid.remove();
         InitDatepickerByID(obj.id);
      }
   }

   $(cid + ' .input-datepicker2').removeAttr("disabled");
   if ($(cid + ' .input-datepicker2') != null && $(cid + ' .input-datepicker').length > 0) {
      for (var i = 0 ; i < $(cid + ' .input-datepicker2').length; i++) {
         var obj = $(cid + ' .input-datepicker2').get(i);
         var hid = $(cid + ' input[type="hidden"][name=' + obj.name + ']');
         if (hid != null)
            hid.remove();
         InitDatepickerByID(obj.id);
      }
   }

   $(cid + ' input[type=radio]').removeAttr("disabled");
   if ($(cid + ' input[type=radio]') != null && $(cid + ' input[type=radio]').length > 0) {
      for (var i = 0 ; i < $(cid + ' input[type=radio]').length; i++) {
         var rdo = $(cid + ' input[type=radio]').get(i);
         if (rdo != null) {
            var hid = $(cid + ' input[type="hidden"][name=' + rdo.name + ']');
            if (hid != null)
               hid.remove();
         }
      }
   }

   if ($(cid).is("li")) {
      $(cid).removeClass('disabled');
   }
}


function MandatoryColor(cid) {
   if (cid.indexOf('#') < 0) {
      cid = '#' + cid;
   }
   if ($(cid).get(0) != null) {
      if ($(cid).get(0).hasAttribute('readonly') == false & $(cid).get(0).hasAttribute('disabled') == false) {
         $(cid + '_chosen').addClass('mandatory-validation');
         $(cid).addClass('mandatory-validation');
      }

   }
}

function MandatoryRemoveColor(cid) {
   if (cid.indexOf('#') < 0) {
      cid = '#' + cid;
   }
   if ($(cid).get(0) != null) {
      $(cid + '_chosen').removeClass('mandatory-validation');
      $(cid).removeClass('mandatory-validation');

   }
}

function InitAmountControl(cid) {
   if (cid.indexOf('#') < 0) {
      cid = '#' + cid;
   }
   if ($(cid).get(0) != null) {
      if ($(cid).hasClass('text-right') == false) {
         $(cid).addClass('text-right');
      }
   }
}

function ErrorColor(cid) {
   if (cid.indexOf('#') < 0) {
      cid = '#' + cid;
   }
   if ($(cid).get(0) != null) {
      if ($(cid).get(0).hasAttribute('readonly') == false) {
         $(cid + '_chosen').addClass('input-validation-error');
         $(cid).addClass('input-validation-error');
         $(cid + '_chosen').removeClass('mandatory-validation');
         $(cid).removeClass('mandatory-validation');
      }
   }
}

function ErrorRemoveColor(cid) {
   if (cid.indexOf('#') < 0) {
      cid = '#' + cid;
   }
   if ($(cid).get(0) != null) {
      $(cid + '_chosen').removeClass('input-validation-error');
      $(cid).removeClass('input-validation-error');
   }
}

function CollapseHeader(show, btncollapse, section) {
   if (btncollapse == null) {
      btncollapse = "#" + "btncollapse";
   }
   if (section == null) {
      section = "#" + "secfilter";
   }
   if (btncollapse.indexOf('#') < 0) {
      btncollapse = '#' + btncollapse;
   }
   if (section.indexOf('#') < 0) {
      section = '#' + section;
   }
   if (show == null) {
      $(btncollapse).get(0).click();
   }
   else {
      if (show == true) {
         var haveminized = $(section).hasClass("minimized");
         if (haveminized == true) {
            if ($(btncollapse).get(0) != null) {
               $(btncollapse).get(0).click();
            }

         }
      }
      else {
         var haveminized = $(section).hasClass("minimized");
         if (haveminized == false) {
            if ($(btncollapse).get(0) != null) {
               $(btncollapse).get(0).click();
            }
         }
      }
   }

}

function InitCollapseHeader(show) {
   if ($("#" + "btncollapse").get(0) != null) CollapseHeader(show, "#" + "btncollapse", "#" + "secfilter");
   if ($("#" + "btncollapse1").get(0) != null) CollapseHeader(show, "#" + "btncollapse1", "#" + "secfilter1");
   if ($("#" + "btncollapse2").get(0) != null) CollapseHeader(show, "#" + "btncollapse2", "#" + "secfilter2");
   if ($("#" + "btncollapse3").get(0) != null) CollapseHeader(show, "#" + "btncollapse3", "#" + "secfilter3");
   if ($("#" + "btncollapse4").get(0) != null) CollapseHeader(show, "#" + "btncollapse4", "#" + "secfilter4");
   if ($("#" + "btncollapse5").get(0) != null) CollapseHeader(show, "#" + "btncollapse5", "#" + "secfilter5");
   if ($("#" + "btncollapse6").get(0) != null) CollapseHeader(show, "#" + "btncollapse6", "#" + "secfilter6");
}

function InitDatatableApplyLink(param) {
   var applyLink = '<div class="apply-row"> <select class="chosen-select form-control" name="' + param.name + '">';
   if (param.hasActive != null && param.hasActive == true) {
      applyLink = applyLink + '<option value="Active">Active</option>';
   }
   if (param.hasInactive != null && param.hasInactive == true) {
      applyLink = applyLink + '<option value="Inactive">Inactive</option>';
   }
   if (param.hasDel != null && param.hasDel == true) {
      applyLink = applyLink + '<option value="D">Delete</option>';
   }
   if (param.hasProcess != null && param.hasProcess == true) {
      applyLink = applyLink + '<option value="Process">Process Payroll</option>';
   }
   if (param.hasConfirm != null && param.hasConfirm == true) {
      applyLink = applyLink + '<option value="Comfirm">Confirm Payroll</option>';
   }
   applyLink = applyLink + '</select>';

   if (param.confirmAlert != null && param.confirmAlert == true) {
      if (param.tabAction != null && param.tabAction != '') {
         applyLink = applyLink + '&nbsp<button class="btn btn-default" type="submit" name="tabAction" value ="' + param.tabAction + '" style="padding-top:8px;padding-bottom:7px;" onclick="return Confirm_Alert(true);">Apply</button>';
      }
      else {
         applyLink = applyLink + '&nbsp<button class="btn btn-default" type="submit" style="padding-top:8px;padding-bottom:7px;" onclick="return Confirm_Alert(true);" >Apply</button>';
      }
   } else {
      if (param.tabAction != null && param.tabAction != '') {
         applyLink = applyLink + '&nbsp<button class="btn btn-default" type="submit" name="tabAction" value ="' + param.tabAction + '" style="padding-top:8px;padding-bottom:7px;">Apply</button>';
      }
      else {
         applyLink = applyLink + '&nbsp<button class="btn btn-default" type="submit" style="padding-top:8px;padding-bottom:7px;" >Apply</button>';
      }
   }


   applyLink = applyLink + '</div>';

   return applyLink;
}

function InitDatatable(id, addNewRowlink, applyLink, sortindex, sorttype, sortindex1, sorttype1, sortindex2, sorttype2) {
   if (id.indexOf("#") != 0) {
      id = "#" + id;
   }


   //check all checkboxes
   $('table thead input[type="checkbox"]').change(function () {
      var opts = $(this).parents('table').find('tbody input[type="checkbox"]');
      if (opts != null) {
         for (var i = 0; i < opts.length; i++) {
            var opt = opts.get(i);
            if (opt != null && opt.disabled == false)
               opt.checked = $(this).prop('checked');
         }
      }
   });


   // Add custom class to pagination div
   $.fn.dataTableExt.oStdClasses.sPaging = 'dataTables_paginate paging_bootstrap paging_custom';



   /* Define two custom functions (asc and desc) for string sorting */
   jQuery.fn.dataTableExt.oSort['string-case-asc'] = function (x, y) {
      return ((x < y) ? -1 : ((x > y) ? 1 : 0));
   };

   jQuery.fn.dataTableExt.oSort['string-case-desc'] = function (x, y) {
      return ((x < y) ? 1 : ((x > y) ? -1 : 0));
   };

   $(id + " tbody tr").click(function (e) {
      if ($(this).hasClass('row_selected')) {
         $(this).removeClass('row_selected');
      }
      else {
         oTable01.$('tr.row_selected').removeClass('row_selected');
         $(this).addClass('row_selected');
      }
   });
   var aasort = new Array();
   if (sortindex != null && sorttype != null) {
      aasort.push([sortindex, sorttype]);
   }
   if (sortindex1 != null && sorttype1 != null) {
      aasort.push([sortindex1, sorttype1]);
   }
   if (sortindex2 != null && sorttype2 != null) {
      aasort.push([sortindex2, sorttype2]);
   }
   var oTable01 = $(id).dataTable({
      "sDom":
       //"<'row'<'col-md-12 text-right sm-left'T C>r>" + "t" +
     "<'row'<'col-md-4 nopadding-left'l><'col-md-8 text-right sm-right nopadding-right'C T>r>" + "t" +
     "<'row'<'col-md-4 nopadding-left'><'col-md-4'><'col-md-4 text-right sm-right nopadding-right'f>r>" + "t" +
     "<'row'<'col-md-4 sm-center nopadding-left'i><'col-md-8 text-right sm-center nopadding-right'p>>",
      "oLanguage": {
         "sSearch": ""
      },
      "aaSorting": aasort,
      "oTableTools": {
         "sSwfPath": "http://app.sbsolutions.com.sg/SBSTmpAPI/assets/js/vendor/datatables/tabletools/swf/copy_csv_xls_pdf.swf",
         "aButtons": [
           "print",
 {
    "sExtends": "collection",
    "sButtonText": 'Export <span class="caret" />',
    "aButtons": ["xls", "pdf"]
 }
         ]
      },
      "fnInitComplete": function (oSettings, json) {
         $('.dataTables_filter input').attr("placeholder", "Search");
      },
      "oColVis": {
         "buttonText": '<i class="fa fa-eye"></i>'
      }
   });


   if (addNewRowlink != null && addNewRowlink != '') {
      $(id + '_wrapper').append(addNewRowlink);
   }
   if (applyLink != null && applyLink != '') {
      $(id + '_wrapper').append(applyLink);
      $(".chosen-select").chosen({ disable_search_threshold: 10 });
   }

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

function createCookie(name, value) {
   document.cookie = name + "=" + value + "; path=/";
}

function readCookie(name) {
   var nameEQ = name + "=";
   var ca = document.cookie.split(';');
   for (var i = 0; i < ca.length; i++) {
      var c = ca[i];
      while (c.charAt(0) == ' ') c = c.substring(1, c.length);
      if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
   }
   return null;
}

function eraseCookie(name) {
   createCookie(name, "", -1);
}