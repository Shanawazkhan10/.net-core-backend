const report_get = {
    get: function (url, Param_XML, token, ControlName, callback, foObject = null) {
        var JQ_ControlName = "#" + ControlName; // Datatable Name

        if ($.fn.DataTable.isDataTable(JQ_ControlName)) {

            $(JQ_ControlName).DataTable().clear().destroy();
        }

        $(JQ_ControlName + ' thead').empty();
        $(JQ_ControlName + ' tbody').empty();
        $(JQ_ControlName).empty();

        var reqdata = {
            "Org_Id": window.Global_Var_Org_Id,
            "Rpt_Parameter": Param_XML
        };
        Last_Param_XML = Param_XML;

        // Call api to fetch data
        api_call.post(url, reqdata, token, function (res) {
            var res_Json = JSON.parse(res);
            const ret_data = JSON.parse(res_Json.res_Output);
            if (ret_data.Response == "Error") {
                // No records found

                //Show_Error_Toastr("Error: No record found");

                Common_Swal("Error: No record found");
                return callback(true);
            }
            else {   // Fill records in table
                //var Response_Data = ret_data.Response_Data;
                var Response_Data = ret_data;

                var Report_Data_Str = Response_Data[3]["OutPut"];   // Actual Data in JSON 
                var Report_Data_Json = JSON.parse(Report_Data_Str);

                var Report_Columns = [];
                var Report_Columns_Title_Str = Response_Data[2]["OutPut"];    // Column Title separated by comma
                // Split Column Title based on Comma
                var Report_Columns_Title_Array = Report_Columns_Title_Str.split(",");

                var Report_Columns_Name_Str = Response_Data[4]["OutPut"];    // Column Name separated by comma
                // Split Column Name based on Comma
                var Report_Columns_Name_Array = Report_Columns_Name_Str.split(",");

                var JumpToButton = "";
                //Jumpto Column
                if (ret_data.length > 5)
                {
                     JumpToButton = JSON.parse(Response_Data[6]["OutPut"]);
                }
                

                //Heading Name
                var Report_Heading_Str = Response_Data[1]["OutPut"];   // Heading Name

                // Added By Satyendra saroj
                var Report_Header_Str = Response_Data[0]["OutPut"];   // Header Data in JSON 
                var Report_Header_Json = JSON.parse(Report_Header_Str);
                var Hide_Columns = Report_Header_Json[0].ColHide;
                var ColBold = Report_Header_Json[0].ColBold;
                var ColFreeze = Report_Header_Json[0].ColFreeze;
                var ColFooter = Report_Header_Json[0].FooterNote;
                var ColAlign = Report_Header_Json[0].ColAlign;
                var ColNoWrap = Report_Header_Json[0].ColNoWrap;
                var ColAlign_Split = [];
                var RowColor = Report_Header_Json[0].RowColor;
                var RecordToDisplay = Report_Header_Json[0].RecordToDisplay;
                var Report_Id = Report_Header_Json[0].Rpt_SrNo;
                var Dfilename = Report_Header_Json[0].Dfilename;

                //File Format Name Changes
                var Query_String = window.location.search;
                var Url_Params = new URLSearchParams(Query_String);
                var Menu_Id = Url_Params.get('ContentOption');
                var File_Name = Report_Header_Json[0].ReportHead.replace(" ", "_");
                var date = new Date();
                //var File_Format_Name = 'Kwik_' + Menu_Id + '_' + date + '_' + File_Name.replace(" ", "_");
                var File_Format_Name = Dfilename;
                //File Format Name Changes

                //forloop to check and apply nowarp class
                var ColumnNumberOfNoWarp = [];
                if (typeof ColNoWrap != "" && typeof ColNoWrap != "undefined" && ColNoWrap != null) {
                    for (var i = 0; i < ColNoWrap.length; i++) {
                        if (ColNoWrap.charAt(i) == 'Y') {
                            ColumnNumberOfNoWarp.push(i);
                        }
                    }
                }
                //Column alignment and column bold changes
                var Report_ColAlign = [];
                for (var i = 0; i < ColAlign.length; i++) {
                    //This condition to add class for ColBold and ColAlign together.
                    var ClassBold = "";
                    //Condition to set bold class if present.
                    if (i == ColBold) {
                        ClassBold = "bold";
                    }

                    var ClassNowrap = "";
                    //Condition to set bold class if present.
                    if (i == ColBold) {
                        ClassBold = "bold";
                    }
                    //Condition to set Norap class if present.
                    if (ColumnNumberOfNoWarp.length > 0) {
                        //Check which colomn need nowarp class
                        if ($.inArray(i, ColumnNumberOfNoWarp) >= 0) {
                            ClassNowrap = "nowrap";
                        }
                    }

                    if (ColAlign.charAt(i) == 'L') { Report_ColAlign.push({ targets: i, className: 'left ' + ClassBold + ' ' + ClassNowrap + '' }); }
                    else if (ColAlign.charAt(i) == 'R') { Report_ColAlign.push({ targets: i, className: 'right ' + ClassBold + ' ' + ClassNowrap + '' }); }
                    else if (ColAlign.charAt(i) == 'C') { Report_ColAlign.push({ targets: i, className: 'center ' + ClassBold + ' ' + ClassNowrap + '' }); }
                }


                // Check if Title and Name array has same number of elements
                if (Report_Columns_Title_Array.length != Report_Columns_Name_Array.length) {
                    //$("#loader").hide();
                    //Show_Error_Toastr("Error: Mismatch in report column definition");

                    Common_Swal("Mismatch in report column definition");

                    return callback(true);
                }

                // Build Report columns
                for (i = 0; i < Report_Columns_Title_Array.length; i++) {
                    Report_Columns.push({ data: Report_Columns_Name_Array[i], title: Report_Columns_Title_Array[i] });
                }


                // Initialise datatable
                var table = $(JQ_ControlName).dataTable({
                    retrieve: true,
                    paging: true,
                    data: Report_Data_Json,
                    responsive: true,
                    bSort: true,
                    searching: true,
                    autoWidth: true,
                    orderCellsTop: true,
                    fixedHeader: true,
                    columns: Report_Columns,
                    lengthMenu: [[10, 20, 50, 100, -1], [10, 20, 50, 100, "All"]],
                    pagingType: "simple",
                    processing: true,
                    columnDefs: Report_ColAlign,
                    language: {
                        searchPlaceholder: 'Search...',
                        sSearch: '',
                        lengthMenu: '_MENU_ records/page',
                        loadingRecords: ' & nbsp; '
                    },
                    dom: "<'row mb-0'<'col-sm-12 col-md-12 d-flex align-items-center justify-content-start't>>" +
                        "<'row'<'col-sm-12'tr>>" +
                        "<'row'<'col-sm-12 col-md-5'<'jumpto'>><'col-sm-12 col-md-2'<'downloadcenter'>><'col-sm-12 col-md-2'l><'col-sm-12 col-md-1'i><'col-sm-12 col-md-2'p>>",
                    buttons: [
                        {
                            extend: 'collection',
                            text: 'Export',
                            autoClose: true,
                            buttons: [
                                { text: 'Excel', extend: 'excelHtml5', titleAttr: 'Generate Excel', filename: File_Format_Name },
                                { text: 'CSV', extend: 'csvHtml5', titleAttr: 'Generate CSV', filename: File_Format_Name }
                                //{ text: 'PDF', extend: 'pdfHtml5', titleAttr: 'Generate PDF', filename: File_Format_Name }
                            ],
                            className: 'form-control',
                            id: 'btnExport',
                            fade: true,
                        }
                    ],

                });

                // Hide columns
                //table.api().columns([0, 1, 2, 3, 4]).visible(false);

                if (Hide_Columns != 'N') {
                    var Columns = Hide_Columns;
                    table.api().columns([Columns]).visible(false);
                }

                //Add Filters in Datatable header
                if (Report_Id == "84" || Report_Id == "83") {
                    $('#' + ControlName + ' thead tr').clone(true).attr('id', 'divDatatableFilter' + Report_Id).appendTo('#' + ControlName + ' thead');
                    $('#' + ControlName + ' thead tr:eq(1) th').each(function (i) {
                        var title = $(this).text();
                        $(this).html('<input type="text" style="width: 100%;" placeholder="Search ' + title + '" />');

                        $('input', this).on('keyup change', function () {
                            if (table.api().column(i).search() !== this.value) {
                                table.api().column(i)
                                    .search(this.value)
                                    .draw();
                            }
                        });
                    }).removeClass('sorting_asc sorting');

                    $("#divDatatableFilter" + Report_Id).hide();
                }
                else
                {
                    $('#' + ControlName + ' thead tr').clone(true).attr('id', 'divDatatableFilter').appendTo('#' + ControlName + ' thead');
                    $('#' + ControlName + ' thead tr:eq(1) th').each(function (i) {
                        var title = $(this).text();
                        $(this).html('<input type="text" style="width: 100%;" placeholder="Search ' + title + '" />');

                        $('input', this).on('keyup change', function () {
                            if (table.api().column(i).search() !== this.value) {
                                table.api().column(i)
                                    .search(this.value)
                                    .draw();
                            }
                        });
                    }).removeClass('sorting_asc sorting');

                    $("#divDatatableFilter").hide();
                }
                //End Filters in Datatable header



                //Row Background Color
                //if (RowColor != '')
                //{
                //    var row = 1;
                //    var color = '#f6a828';
                //    $(JQ_ControlName + ' tr:nth-child(' + row + ')').css('background-color', color);
                //}
                //End Row Color

                //Report Heading Name
                if (foObject === null) {
                    $("#spnReportName").html(Report_Heading_Str);
                }
                else {
                    $("#" + foObject).html(Report_Heading_Str);
                }
                

                //Showing Report Id                  
                $('#lblReport_Id').text(Report_Id);

                var RedirectJumpToButton = "";
                if (JumpToButton.length > 0)
                {
                    if (JumpToButton.length > 3) {
                        RedirectJumpToButton += '<div class="row"><span class="bold ml-2 mt-1" style="padding:0px 0px;color: black;" id="lblJumpTo">Jump To: </span> </div>';
                        RedirectJumpToButton += '<div class="row">';
                        $.each(JumpToButton, function (index, value) {
                            if (index <= 5) {
                                RedirectJumpToButton += '<div class=\"ml-2 mt-1\">';
                                var Menu_Link = "?CO=" + window.btoa(value.Menu_id) + "&VN=" + window.btoa(Global_Var_Release_No);
                                RedirectJumpToButton += "<a class=\"btn btn-default btn-sm\" href=\"" + Menu_Link + "\" title=\"" + value.Menu_name + "\" data-filter-tags=\"" + value.Menu_name + " " + value.Menu_name + "\"><span class=\"nav-link-text\">" + value.Menu_name + "</span></a>";
                                RedirectJumpToButton += '</div>';
                            }
                        });
                        RedirectJumpToButton += '</div">';
                    }
                    else {
                        RedirectJumpToButton += '<div class="row"><span class="bold ml-2 mt-1" style="padding:0px 0px;color: black;" id="lblJumpTo">Jump To: </span>';
                        $.each(JumpToButton, function (index, value) {
                            if (index <= 5) {
                                RedirectJumpToButton += '<div class=\"ml-2\">';
                                var Menu_Link = "?CO=" + window.btoa(value.Menu_id) + "&VN=" + window.btoa(Global_Var_Release_No);
                                RedirectJumpToButton += "<a class=\"btn btn-default btn-sm\" href=\"" + Menu_Link + "\" title=\"" + value.Menu_name + "\" data-filter-tags=\"" + value.Menu_name + " " + value.Menu_name + "\"><span class=\"nav-link-text\">" + value.Menu_name + "</span></a>";
                                RedirectJumpToButton += '</div>';
                            }
                        });
                        RedirectJumpToButton += '</div">';
                    }
                }

                //Added by satyendra saroj to add Jump To and Download Filter option
                $("div.jumpto").html(RedirectJumpToButton);
                $("div.downloadcenter").html('<a class=\"btn btn-default btn-sm\" style="display:none;" title=\"Download Center\" id="divDownloadCenter_' + ControlName + '" href="javascript:void(0);" onclick="OpenDownLoadCenterPopUPModel()"><span class=\"nav-link-text\">Download Center</span></a>');
                //$(JQ_ControlName + "_info ").closest(".row").addClass("dataTables_right");

                //Pagnation changes
                if (Report_Data_Json == null || Report_Data_Json == '') {
                    $(JQ_ControlName + '_length').hide();
                    $(JQ_ControlName + '_info').hide();
                    $(JQ_ControlName + '_paginate').hide();
                    $("#filter-btn").hide();
                    $("#download-btn").hide();
                }
                else {
                    $(JQ_ControlName + '_length').show();
                    $(JQ_ControlName + '_info').show();
                    $(JQ_ControlName + '_paginate').show();
                    $("#filter-btn").show();
                    $("#download-btn").show();

                    //Footer 
                    $(JQ_ControlName).append(
                        $('<tfoot/>').append('<th colspan=' + Report_Columns_Name_Array.length + '>' + ColFooter + '</th>')
                    );
                }

                //Check Count of Data to show Download Center button
                if (typeof Report_Data_Json != "" && Report_Data_Json != undefined && Report_Data_Json != null) {
                    if (Report_Data_Json.length >= 500) {
                        $("#divDownloadCenter_" + ControlName + "").show();
                    } else {
                        $("#divDownloadCenter_" + ControlName + "").hide();
                    }
                }

                $('#download-btn').empty();
                var buttons = new $.fn.dataTable.Buttons(table, {
                    buttons: [
                        {
                            extend: 'collection',
                            text: '<i class="fal fa-download"/>',
                            autoClose: true,
                            buttons: [
                                { text: 'Excel', extend: 'excelHtml5', titleAttr: 'Generate Excel', filename: File_Format_Name },
                                { text: 'CSV', extend: 'csvHtml5', titleAttr: 'Generate CSV', filename: File_Format_Name }
                                //{ text: 'PDF', extend: 'pdfHtml5', titleAttr: 'Generate PDF', filename: File_Format_Name }
                            ],
                            className: 'btn btn-outline-warning btn-icon waves-effect waves-themed',
                            id: 'btnExport',
                            fade: true,
                        }
                    ]
                }).container().appendTo($('#download-btn'));

            }

            return callback(true);

        }, function (res) {
            var res_Json = JSON.parse(res);
            const ret_data = JSON.parse(res_Json.res_Output);
            Common_Swal(ret_data.Error_Msg);

            //Show_Error_Toastr("Error: " + res);
            //Common_Swal(res)

            return callback(true);
        });
    }
};


function Common_Swal(Err) {
    swal({
        title: "Error",
        text: "" + Err + "!",
        type: "warning",
        showCancelButton: false,
        //confirmButtonColor: "#DD6B55",
        confirmButtonText: "OK",
        closeOnConfirm: true,
        closeOnCancel: true,
        showCloseButton: true
    }, function (isConfirm) {
        if (isConfirm) {
        } else {
        }
    });
}