var Spinner_HTML = "<span class=\"spinner-border spinner-border-sm\" role=\"status\" aria-hidden=\"true\"></span>";
var Search_HTML = "<span class=\"fal fa-search mr-2\"></span>";


// Fill List of Country from database
function Get_Master_Country(ControlName, Default_Value) {
    var JQ_ControlName = "#" + ControlName;
    $(JQ_ControlName).empty().append('<option label="Select Country"></option>');

    var url = Global_Var_API_Base_Url + "/api/location/Read_All_Country";
    var reqdata = {
        "Org_Id": window.Global_Var_Org_Id,
        "Country_Name": ""
    };
    var stringReqdataupdate = JSON.stringify(reqdata);
    $.ajax({
        type: "POST",
        url: url,
        data: stringReqdataupdate,
        dataType: "json",
        contentType: "application/json",
        success: function (res) {
            const ret_data = JSON.parse(res.res_Output);
            if (ret_data.Response == "Error") {
                // No records found
            }
            else {   // Fill records in dropdown list
                var Response_Data = ret_data.Response_Data;
                $.each(ret_data.Response_Data, function (data, value) {
                    if (value.Is_Active == 1) {
                        $(JQ_ControlName).append($("<option></option>").val(value.Country_Id).html(value.Country_Name));
                    }
                });
                if (Default_Value != "") { $(JQ_ControlName).val(Default_Value); }
            }
            $("#splash1").hide();
            return;
        },
        error: function (res) {
            $("#splash1").hide();
            Show_Error_Toastr();
        }
    });
}


// Toggle Datatable filters
function Toggle_Datatable_Filter() {
    $("#divDatatableFilter").toggle();
}


//Back To Filter
function backReport() {
    $("#divProceed").hide();
    $("#divSearch").show();
}

function Show_Spinner(ControlName) {
    var loadingText = Spinner_HTML + ' Processing...';
    $("#" + ControlName).html(loadingText);
}

function Hide_Spinner(ControlName) {
    var SearchText = Search_HTML + ' Proceed';
    $("#" + ControlName).html(SearchText);
}

function ValidateSpecialChar(id, e) {
    e = e || window.event;
    var Regex = /[^\sa-z\d]/i,
        key = String.fromCharCode(e.keyCode || e.which);
    if (e.which !== 0 && e.charCode !== 0 && Regex.test(key)) {
        e.returnValue = false;
        if (e.preventDefault) {
            e.preventDefault();
        }
    }
}


function CollapsableChangesFilter(fnObject)
{
    if (fnObject.className == "btn btn-panel fal fa-plus")
    {
        $("#btnCollapseFilter").removeClass("btn btn-panel fal fa-plus");
        $("#btnCollapseFilter").addClass("btn btn-panel fal fa-minus");

    } else if (fnObject.className == "btn btn-panel fal fa-minus")
    {
        $("#btnCollapseFilter").removeClass("btn btn-panel fal fa-minus");
        $("#btnCollapseFilter").addClass("btn btn-panel fal fa-plus");
    }
    else if (fnObject.className == "btn btn-panel fal fa-expand")
    {
        $("#btnfullscreen").removeClass("btn btn-panel fal fa-expand");
        $("#btnfullscreen").addClass("btn btn-panel fal fa-compress-wide");

    } else if (fnObject.className == "btn btn-panel fal fa-compress-wide") {
        $("#btnfullscreen").removeClass("btn btn-panel fal fa-compress-wide");
        $("#btnfullscreen").addClass("btn btn-panel fal fa-expand");
    }
}
function CollapsableChangesReport(fnObject) {
    if (fnObject.className == "btn btn-panel fal fa-plus") {
        $("#btnCollapseReport").removeClass("btn btn-panel fal fa-plus");
        $("#btnCollapseReport").addClass("btn btn-panel fal fa-minus");

    } else if (fnObject.className == "btn btn-panel fal fa-minus") {
        $("#btnCollapseReport").removeClass("btn btn-panel fal fa-minus");
        $("#btnCollapseReport").addClass("btn btn-panel fal fa-plus");
    }
    else if (fnObject.className == "btn btn-panel fal fa-expand") {
        $("#btnfullscreenReport").removeClass("btn btn-panel fal fa-expand");
        $("#btnfullscreenReport").addClass("btn btn-panel fal fa-compress-wide");

    } else if (fnObject.className == "btn btn-panel fal fa-compress-wide") {
        $("#btnfullscreenReport").removeClass("btn btn-panel fal fa-compress-wide");
        $("#btnfullscreenReport").addClass("btn btn-panel fal fa-expand");
    }
}


