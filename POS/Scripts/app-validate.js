function toNumber(num) {
    num = num.toString().replace(/\$|\,/g, '');
    return num;
}



function formatCurrency(num) {
    num = num.toString().replace(/\$|\,/g, '');
    if (isNaN(num)) {
        num = "0";
    }

    sign = (num == (num = Math.abs(num)));
    num = Math.floor(num * 100 + 0.50000000001);
    cents = num % 100;
    num = Math.floor(num / 100).toString();

    if (cents < 10) {
        cents = "0" + cents;
    }
    for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3) ; i++) {
        num = num.substring(0, num.length - (4 * i + 3)) + ',' + num.substring(num.length - (4 * i + 3));
    }

    return (((sign) ? '' : '-') + num + '.' + cents);
}

function formatNumber(num) {
    num = num.toString().replace(/\$|\,/g, '');
    if (isNaN(num)) {
        num = "0";
    }

    sign = (num == (num = Math.abs(num)));
    num = Math.floor(num * 100);
    cents = num % 100;
    num = Math.floor(num / 100).toString();

    if (cents < 10) {
        cents = "0" + cents;
    }

    for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3) ; i++) {
        num = num.substring(0, num.length - (4 * i + 3)) + ',' + num.substring(num.length - (4 * i + 3));
    }



    return (((sign) ? '' : '-') + num);
}


function formatDecimal(num) {
    num = num.toString().replace(/\$|\,/g, '');
    if (isNaN(num)) {
        num = "0";
    }

    sign = (num == (num = Math.abs(num)));
    num = Math.floor(num * 100 + 0.50000000001);
    cents = num % 100;
    num = Math.floor(num / 100).toString();

    if (cents < 10) {
        cents = "0" + cents;
    }

    return (((sign) ? '' : '-') + num + '.' + cents);
}
function ValidateArrayRequire(id, filed_name, require) {
    var name = id;
    if (id.indexOf('#') >= 0) {
        name = name.replace("#", '');
    }

    if (id.indexOf('#') < 0) {
        id = '#' + id;
    }

    var value = $("[name='" + name + "']");

    if ($(id + '_Err') != null) {
        $(id + '_Err').get(0).setAttribute("class", "field-validation-valid");
        if ($(id + '_Err').get(0).childNodes.length > 0) $(id + '_Err').get(0).removeChild($(id + '_Err').get(0).childNodes[0]);
        if (value == null) {
            $(id + '_Err').get(0).setAttribute("class", "field-validation-error");

            var spanErr = document.createElement("span");
            spanErr.textContent = "The " + filed_name + " field is required.";
            $(id + '_Err').get(0).appendChild(spanErr);
            return false;
        }
        else {
            var haschecked = false;
            for (var i = 0 ; i < value.length; i++) {
                if (value.get(i) != null && value.get(i).checked == true) {
                    haschecked = true;
                }
            }

            if (!haschecked) {
                $(id + '_Err').get(0).setAttribute("class", "field-validation-error");

                var spanErr = document.createElement("span");
                spanErr.textContent = "The " + filed_name + " field is required.";
                $(id + '_Err').get(0).appendChild(spanErr);
                return false;
            }


        }
    }


    return true;
}
function ValidateRequire(id, filed_name, require) {
    if (id.indexOf('#') < 0) {
        id = '#' + id;
    }

    var value = $(id).val();
    if ($(id + '_Err').get(0) != null) {
        $(id + '_Err').get(0).setAttribute("class", "field-validation-error");
        if ($(id + '_Err').get(0).childNodes.length > 0) $(id + '_Err').get(0).removeChild($(id + '_Err').get(0).childNodes[0]);

        if (require == true) {
            if (value == null || value == "") {

                var spanErr = document.createElement("span");
                spanErr.textContent = "The " + filed_name + " field is required.";
                $(id + '_Err').get(0).appendChild(spanErr);

                return false;
            }
            return true;
        }
    }
    return true;
}
function ValidateDecimal(id, filed_name, require) {
    if (id.indexOf('#') < 0) {
        id = '#' + id;
    }

    var value = $(id).val();
    var oldval = $(id).val();
    value = value.toString().replace(/\$|\,/g, '');
    if ($(id + '_Err') != null) {
        $(id + '_Err').get(0).setAttribute("class", "field-validation-valid");
        if ($(id + '_Err').get(0).childNodes.length > 0) $(id + '_Err').get(0).removeChild($(id + '_Err').get(0).childNodes[0]);

        if (require == true) {
            if (value == null || value == "" || value == 0) {
                $(id + '_Err').get(0).setAttribute("class", "field-validation-error");

                var spanErr = document.createElement("span");
                spanErr.textContent = "The " + filed_name + " field is required.";
                $(id + '_Err').get(0).appendChild(spanErr);
                MandatoryColor(id);
                return false;
            }
        }

        if (value == null || value == "") {
            $(id).val("0.00");
            ErrorRemoveColor(id);
            return true;
        }

        var dec = parseFloat(value).toFixed(2);
        if (isNaN(dec)) {
            $(id + '_Err').get(0).setAttribute("class", "field-validation-error");

            var spanErr = document.createElement("span");
            spanErr.textContent = "The value '" + value + "' is not valid for " + filed_name + " .";
            $(id + '_Err').get(0).appendChild(spanErr);
            ErrorColor(id);
            return false;
        }

        if (require == true & dec <= 0) {
            $(id + '_Err').get(0).setAttribute("class", "field-validation-error");

            var spanErr = document.createElement("span");
            spanErr.textContent = "The value '" + value + "' is not valid for " + filed_name + " .";
            $(id + '_Err').get(0).appendChild(spanErr);
            ErrorColor(id);
            return false;
        }

        if (oldval.indexOf(",") > 0) {
            dec = formatCurrency(dec);
        }

        $(id).val(dec);
        ErrorRemoveColor(id);
        MandatoryRemoveColor(id);


    }


    return true;
}

function ValidateNumber(id, filed_name, require, allowZero) {
    if (id.indexOf('#') < 0) {
        id = '#' + id;
    }

    var value = $(id).val();
    if ($(id + '_Err') != null) {
        $(id + '_Err').get(0).setAttribute("class", "field-validation-valid");
        if ($(id + '_Err').get(0).childNodes.length > 0) $(id + '_Err').get(0).removeChild($(id + '_Err').get(0).childNodes[0]);

        if (require == true) {
            if (value == 0 && allowZero == true) {
                return true;
            }
            else if (value == null || value == "") {
                $(id + '_Err').get(0).setAttribute("class", "field-validation-error");

                var spanErr = document.createElement("span");
                spanErr.textContent = "The " + filed_name + " field is required.";
                $(id + '_Err').get(0).appendChild(spanErr);
                MandatoryColor(id);
                return false;
            }
        }

        if (value == null || value == "") {
            $(id).val(0);
            ErrorRemoveColor(id);
            return true;
        }

        var dec = parseInt(value).toFixed(0);

        if (isNaN(dec)) {
            $(id + '_Err').get(0).setAttribute("class", "field-validation-error");

            var spanErr = document.createElement("span");
            spanErr.textContent = "The value '" + value + "' is not valid for " + filed_name + " .";
            $(id + '_Err').get(0).appendChild(spanErr);
            ErrorColor(id);
            return false;
        }

        if (require == true & dec <= 0) {
            $(id + '_Err').get(0).setAttribute("class", "field-validation-error");

            var spanErr = document.createElement("span");
            spanErr.textContent = "The value '" + value + "' is not valid for " + filed_name + " .";
            $(id + '_Err').get(0).appendChild(spanErr);
            ErrorColor(id);
            return false;
        }


        $(id).val(dec);
        ErrorRemoveColor(id);
        MandatoryRemoveColor(id);
    }


    return true;
}

function ValidateEmail(id, filed_name, require) {
    if (id.indexOf('#') < 0) {
        id = '#' + id;
    }
    var isvalid = false;
    var value = $(id).val();

    if ($(id + '_Err') != null) {
        $(id + '_Err').get(0).setAttribute("class", "field-validation-valid");
        if ($(id + '_Err').get(0).childNodes.length > 0) $(id + '_Err').get(0).removeChild($(id + '_Err').get(0).childNodes[0]);

        if (require == true) {
            if (value == null || value == "") {
                $(id + '_Err').get(0).setAttribute("class", "field-validation-error");

                var spanErr = document.createElement("span");
                spanErr.textContent = "The " + filed_name + " field is required.";
                $(id + '_Err').get(0).appendChild(spanErr);

                MandatoryColor(id);
                isvalid = false;
                return isvalid;
            }
            isvalid = true;
        } else if (require == false) {
            if (value == null || value == "") {
                ErrorRemoveColor(id);
                isvalid = true;
                return isvalid;
            }
        }

        if (isvalid) {
            var re = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i;

            if (re.test(value) == false) {
                $(id + '_Err').get(0).setAttribute("class", "field-validation-error");

                var spanErr = document.createElement("span");
                spanErr.textContent = "The value '" + value + "' is not a valid email.";
                $(id + '_Err').get(0).appendChild(spanErr);

                ErrorColor(id);
                isvalid = false;
            } else {
                MandatoryRemoveColor(id);
                ErrorRemoveColor(id);
            }
        }
    }

    return isvalid;
}

function ValidateDate(id, filed_name, require, allowFutureDate) {
    if (id.indexOf('#') < 0) {
        id = '#' + id;
    }
    var isvalid = true;
    var value = $(id).val();

    if ($(id + '_Err') != null) {
        $(id + '_Err').get(0).setAttribute("class", "field-validation-valid");
        if ($(id + '_Err').get(0).childNodes.length > 0) $(id + '_Err').get(0).removeChild($(id + '_Err').get(0).childNodes[0]);
        if (require == true) {
            if (value == null || value == "") {
                $(id + '_Err').get(0).setAttribute("class", "field-validation-error");

                var spanErr = document.createElement("span");
                spanErr.textContent = "The " + filed_name + " field is required.";
                $(id + '_Err').get(0).appendChild(spanErr);

                MandatoryColor(id);
                isvalid = false;
                return isvalid;
            }
        } else if (require == false) {
            if (value == null || value == "") {
                ErrorRemoveColor(id);
                isvalid = true;
                return isvalid;
            }
        }

        var data = value.split("/");
        // using ISO 8601 Date String (dd/MM/yyyy)
        var dateVal = Date.parse(data[2] + "-" + data[1] + "-" + data[0]);

        if (isNaN(dateVal)) {
            $(id + '_Err').get(0).setAttribute("class", "field-validation-error");
            var spanErr = document.createElement("span");
            spanErr.textContent = "The value '" + value + "' is not a valid date.";
            $(id + '_Err').get(0).appendChild(spanErr);
            ErrorColor(id);
            isvalid = false;
        } else {
            var month = parseInt(data[1]);
            var day = parseInt(data[0]);
            var year = parseInt(data[2]);

            if (day <= 0) {
                isvalid = false;
            } else if (month == 4 | month == 6 | month == 9 | month == 11) {
                if (day > 30) {
                    isvalid = false;
                }
            } else if (month == 1 | month == 3 | month == 5 | month == 7 | month == 8 | month == 10 | month == 12) {
                if (day > 31) {
                    isvalid = false;
                }
            } else if (month == 2) {
                if (isLeapYear(year)) {
                    if (day > 29) {
                        isvalid = false;
                    }
                } else {
                    if (day > 28) {
                        isvalid = false;
                    }
                }
            }

            if (!isvalid) {
                $(id + '_Err').get(0).setAttribute("class", "field-validation-error");
                var spanErr = document.createElement("span");
                spanErr.textContent = "The value '" + value + "' is not a valid date.";
                $(id + '_Err').get(0).appendChild(spanErr);
                ErrorColor(id);
                return isvalid;
            } else {
                if (allowFutureDate == false) {
                    if (isFutureDate(dateVal)) {
                        $(id + '_Err').get(0).setAttribute("class", "field-validation-error");
                        var spanErr = document.createElement("span");
                        spanErr.textContent = "Future date is not allowed.";
                        $(id + '_Err').get(0).appendChild(spanErr);
                        ErrorColor(id);
                        return isvalid;
                    }
                }
                MandatoryRemoveColor(id);
                ErrorRemoveColor(id);
            }
        }
    }

    return isvalid;
}



function ClearValidate(id) {
    if (id.indexOf('#') < 0) {
        id = '#' + id;
    }
    $(id + "_Err").get(0).setAttribute("class", "field-validation-valid")
    if ($(id + '_Err').get(0).childNodes.length > 0) $(id + '_Err').get(0).removeChild($(id + '_Err').get(0).childNodes[0]);
}

function ToDisplayDate(d) {
    try {
        function pad(s) { return (s < 10) ? '0' + s : s; }

        if (!isNaN(d.getDate()) & !isNaN(d.getMonth()) & !isNaN(d.getFullYear())) {
            return [pad(d.getDate()), pad(d.getMonth()), d.getFullYear()].join('/');
        }
    }
    catch (err) {

    }
    return '';
}

function ToDate(d) {
    try {
        var dsplit = d.split('/');
        if (dsplit.length == 3) {
            var sdate = new Date(dsplit[2], dsplit[1], dsplit[0]);
            return sdate;
        }
    }
    catch (err) {

    }
    return;
}



function DaySpan(d1, d2) {
    try {
        var date1 = ToDate(d1);
        var date2 = ToDate(d2);
        var diff = new Date(date2.getTime() - date1.getTime());

        return diff.getUTCDate() - 1;
    }
    catch (err) {
        return 0;
    }

}



function getChar(event) {
    if (event.which == null) {
        return String.fromCharCode(event.keyCode) // IE
    } else if (event.which != 0 && event.charCode != 0) {
        return String.fromCharCode(event.which)   // the rest
    } else {
        return null // special key
    }
}

function isNumeric(val) {
    return val !== "NaN" && (+val) + '' === val + ''
}

function isLeapYear(year) {
    return ((year % 4 == 0) && (year % 100 != 0)) || (year % 400 == 0);
}

function isFutureDate(idate) {
    var today = new Date().getTime();
    return (today - idate) < 0 ? true : false;
}


function isDecimal(num) {

    if (num.indexOf(".") >= 0) {
        // have point
        var havedot = num.substring(num.indexOf("."));
        if (havedot.length > 2) {

            var digit = num.substring(num.indexOf(".") + 1);
            digit = parseInt(digit);
            if (!isNaN(digit) && digit > 0)
            {
                return true;
            }
        }
    }
    return false;
}