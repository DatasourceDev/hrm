function Combo_Reload(id, type, param, param2) {
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
        }
    });
}


$(function () {
    //$('.input-append').datepicker({ daysOfWeekDisabled: "0,6" });
    $('.input-append').datepicker({});



});
function Enable(cid) 
{
    $('#' + cid + ' .input-append').datepicker('remove');
    $('#' + cid + ' .input-append').datepicker({});

    if ($('#' + cid + ' input').hasClass('as-label') != true) {
        $('#' + cid + ' input').removeAttr("readonly");
    }

    $('#' + cid + ' input[type="file"]').removeAttr("disabled");
    $('#' + cid + ' .uploader').removeAttr("disabled");
    $('#' + cid + ' button').removeAttr("disabled");
    $('#' + cid + ' textarea').removeAttr("readonly");

    //$('#' + cid + ' a').attr("onclick", "return false;");
    //$('#' + cid + ' a').removeAttr("disabled");
    //$('#' + cid + ' a').attr("href", "#");

    $('#' + cid + ' select').removeAttr("readonly");
    $('#' + cid + ' option').removeAttr("readonly");

}

function Disable(cid) {
    $('#' + cid + ' .input-append').datepicker('remove');
    $('#' + cid + ' .input-append').datepicker({ daysOfWeekDisabled: "0,1,2,3,4,5,6" });
    $('#' + cid + ' input').attr("readonly", "true");
    $('#' + cid + ' input[type="file"]').attr("disabled", "disabled");
    $('#' + cid + ' .uploader').attr("disabled", "disabled");
    $('#' + cid + ' button').attr("disabled", "disabled");
    $('#' + cid + ' textarea').attr("readonly", "true");
   
    $('#' + cid + ' select').attr("readonly", "true");
    $('#' + cid + ' option').attr("readonly", "true");
    $('#' + cid + ' a').attr("onclick", "return false;");
    $('#' + cid + ' a').attr("disabled", "disabled");
    $('#' + cid + ' a').attr("href", "#");

    var selindex = null;
    $('#' + cid + ' select').on("click", function (event) {
        selindex = this.selectedIndex;
        this.onchange = null;
    });

    $('#' + cid + ' select').on("change", function (event) {
        if (selindex != null) {
            this.selectedIndex = selindex;
            this.onchange = null;
        }
    });

    $('#' + cid + ' input[type=checkbox]').on("change", function (event) {
        var chk = this;
        if (chk.checked == true) {
            chk.checked = false;
        }
        else {
            chk.checked = true;
        }
    });

    var loadingchk = false;
    $('#' + cid + ' input[type=radio]').on("change", function (event) {
        var chk = this;
        var allradio = $('input[Name=' + chk.name + ']');
        if (allradio != null && loadingchk == false) {
            var checked = chk.checked;
            loadingchk = true;
            for (i = 0; i < allradio.length; i++) {
                if (chk == allradio[i]) {
                    if (checked == true) {
                        chk.checked = false;
                    }
                    else {
                        chk.checked = true;
                    }
                }
                else {
                    if (checked == true) {
                        allradio[i].checked = true;
                    }
                    else {
                        allradio[i].checked = false;
                    }
                }
            }
            loadingchk = false;
        }

    });
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