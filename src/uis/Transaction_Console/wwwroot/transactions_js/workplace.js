var Global_Var_Org_Id = "";
var Global_Var_User_Id = "";
var Global_Var_App_Key = "";
var Global_Var_Is_Authorised = "";
var Global_Var_App_Id = "";
var Global_Var_Ip_Address = "";
var Global_Var_Login_Name = "";
var Global_Var_Is_New_User = "";
var LReport_Id = "";
var Last_Param_XML = "";
var Global_Var_Allow_Multi_Login = 0;
var Global_Var_Dynamic_Breadcrumb = "";

// SignalR Initialisation and Connection        
//const connection = new signalR.HubConnectionBuilder()
//    .withUrl("http://192.168.00.00:44331/chatHub", {
//        skipNegotiation: true,
//        transport: signalR.HttpTransportType.WebSockets
//    })
//    .withAutomaticReconnect()
//    .configureLogging(signalR.LogLevel.Information)
//    .build();       // SignalR connection



$(document).ready(function () {
    var pageid = 'M000';
    Get_App_Base_Url();

    Global_Var_App_Id = document.getElementById('txtApplication_Id').value;

    // Check if QueryParameter contains auto login code
    var App_Key = Get_QueryParams_Decrypt('App_Key', 'App_Key');
    var User_Id = Get_QueryParams_Decrypt('App_Key', 'User_Id');
    var Auth_Id = Get_QueryParams_Decrypt('App_Key', 'Auth_Id');
    var Source_Key = Get_QueryParams_Decrypt('App_Key', 'Source_Key');
    var Mode = Get_QueryParams_Decrypt('App_Key', 'Mode');

    // Check if User Details exists in Localstorage.  If exists then proceed else goto Sign-In page
    var User_Details_En = window.localStorage.getItem(window.btoa("User_Details"));

    if (User_Details_En != null) {
        var User_Details = JSON.parse(window.atob(User_Details_En));
    }
    //var User_Details = JSON.parse(window.atob(User_Details_En));

    if (User_Details == undefined || User_Details == null) {
        alert("Please login");
        window.location = './index.html';

        return;
    }

    // Global Variable Initialisation
    Global_Var_Org_Id = User_Details.Org_Id;
    Global_Var_User_Id = User_Details.User_Id;
    Global_Var_Is_Authorised = User_Details.Is_Authorised;
    Global_Var_App_Key = User_Details.App_Key;
    Global_Var_Shop_Name = User_Details.Shop_Name;
    Global_Var_Is_Marketplace = User_Details.Is_Marketplace;
    Global_Var_Login_Name = User_Details.Login_Name;

    //Style for DDL
    $("#ddlDownloadFileType").select2({
        dropdownParent: $('#Modal_DownLoadCenter')
    });

    // Display current user name
    var User_Name = User_Details.User_Name;
    $('#spanusername').html(User_Name.charAt(0).toUpperCase() + User_Name.slice(1));

    // Based on ContentOption load content of child page
    var ContentOption = window.atob(Get_QueryParams('CO'));

    // Display menu as per menu assigned to user
    var Menu_List = User_Details.Menu_List;
    var Menu_HTML = "";

    var Cur_Parent_Menu_Id = "0";
    for (var i = 0; i < Menu_List.length; ++i) {
        if (Menu_List[i].Parent_Menu_Id == Cur_Parent_Menu_Id) {
            // Add a new Node for ProductGroup
            var Menu_Id = Menu_List[i].Menu_Id;
            var Menu_Link = Menu_List[i].Menu_Link;
            var Menu_Name = Menu_List[i].Menu_Name;
            var Menu_Icon_Name = Menu_List[i].Menu_Icon_Name;

            if (Menu_Link == "" || Menu_Link == null) {
                Menu_Link = "?CO=" + window.btoa(Menu_Id) + "&VN=" + window.btoa(Global_Var_Release_No);   // Internal Link for Menu.  Based on this Content page will be loaded
            }
            var Menu_Parent_List = Cur_Parent_Menu_Id;
            var Menu_Parent_Name_List = Menu_Name

            // Get Submenu for this menu
            var SubMenu_HTML = "";
            Add_Menu_TreeNode(Menu_Id, Menu_List, Menu_Parent_List, Menu_Parent_Name_List, function (Tree_Ret_HTML) {
                var Output_Str = Tree_Ret_HTML;
                if (Output_Str != "") {
                    SubMenu_HTML = "<ul>" + Output_Str + "</ul>";
                }
            });

            // Generate html for current menu
            if (SubMenu_HTML != "") {
                Menu_Link = "#";    // If the current menu has sub menu then the link will be only #
            }

            var Cur_Menu_HTML = "<li id=\"mnu" + Menu_Id + "\" >";
            Cur_Menu_HTML += "<a href=\"" + Menu_Link + "\"  title=\"" + Menu_Name + "\"  data-filter-tags=\"" + Menu_Name + "\">";
            Cur_Menu_HTML += "<i class=\"fal " + Menu_Icon_Name + "\"></i>";
            Cur_Menu_HTML += "<span class=\"nav-link-text\">" + Menu_Name + "</span>";
            Cur_Menu_HTML += "</a>";

            Cur_Menu_HTML += SubMenu_HTML;  // Add submenu HTML

            Menu_HTML += Cur_Menu_HTML;

            // Close Activity Type
            Menu_HTML += "</li>";
        }
    };



    // Add Menu to Workplace
    var divMenu = document.getElementById("js-nav-menu");
    divMenu.innerHTML = Menu_HTML;

    // Read Display, Add, Modify and Delete option for the selected menu
    if (ContentOption != null && ContentOption != "") {
        var Page_Display_Flag = 0;
        var Page_Add_Flag = 0;
        var Page_Modify_Flag = 0;
        var Page_Delete_Flag = 0;
        var Page_External_URL = "";
        var Page_Menu_Name = "";
        var Page_Menu_Tooltip = "";
        var Page_Is_External_Page = 0;

        for (var i = 0; i < Menu_List.length; ++i) {
            if (ContentOption == Menu_List[i].Menu_Id) {
                Page_Display_Flag = Menu_List[i].Display_Flag;
                Page_Add_Flag = Menu_List[i].Add_Flag;
                Page_Modify_Flag = Menu_List[i].Modify_Flag;
                Page_Delete_Flag = Menu_List[i].Delete_Flag;
                Page_External_URL = Menu_List[i].External_Page_URL;
                Page_Menu_Name = Menu_List[i].Menu_Name;
                Page_Menu_Tooltip = Menu_List[i].Menu_Tooltip;
                Page_Is_External_Page = Menu_List[i].Is_External_Page;
                break;
            }
        }
    }

    // Generate Dynamic Breadcrumb
    var Open_Parent_MenuName = document.getElementById("mnu" + ContentOption).getAttribute("data-parent-menu-name");
    if (Open_Parent_MenuName != null && Open_Parent_MenuName != undefined) {
        var Parent_MenuName = Open_Parent_MenuName.split("%:%");
        var Breadcrumb_HTML = "<li class=\"breadcrumb-item\"><a href=\"javascript:void(0);\">Home</a></li>";

        for (var i = 0; i < Parent_MenuName.length; i++) {
            if (i + 1 == Parent_MenuName.length) {
                Breadcrumb_HTML += "<li class=\"breadcrumb-item active\">" + Parent_MenuName[i] + "</li>";
            } else {
                Breadcrumb_HTML += "<li class=\"breadcrumb-item\">" + Parent_MenuName[i] + "</li>";
            }

        }

        Global_Var_Dynamic_Breadcrumb = Breadcrumb_HTML;
    } else {

        Global_Var_Dynamic_Breadcrumb = "";
    }
    



        // Perform action based on Menu selected using ContentOption
        switch (ContentOption) {
            case "M001":
                Show_Employee_Page();
                //Get_Content_Page_Dashboard(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag);  // Dashboard
                break;
            case "M003": Get_Content_Page_Mydownloads(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag);  // My Downloads
                break;
            case "M004": Get_Content_Page_Role_Content_Type(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag);  // My Downloads
                break;

            case "M205": Get_Content_Page_Report_Banking_Check_Status_Payin_Status(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '9');
                LReport_Id = "9";
                break;
            case "M206": Get_Content_Page_Report_Banking_Check_Status_payout_Status(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '79');
                LReport_Id = "79";
                break;
            case "M207": Get_Content_Page_Report_Banking_punch_new_request_payout_status(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '11');
                LReport_Id = "11";
                break;
            case "M208": Get_Content_Page_Report_Banking_Reports_Cheque(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '1');
                LReport_Id = "1";
                break;
            case "M209": Get_Content_Page_Report_Banking_Reports_third_Party(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '6');
                LReport_Id = "6";
                break;
            case "M210": Get_Content_Page_Request_Funds_Payin_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '6');
                LReport_Id = "6";
                break;
            case "M211": Get_Content_Page_Report_Banking_Reports_Funds_Payin_Payout(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '78');
                LReport_Id = "78";
                break;
            case "M212": Get_Content_Page_Request_Third_Party_Module(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '9');
                LReport_Id = "9";
                break;

            case "M302": Get_Content_Page_Report_ACOP_FormStatus(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '3');  // Form Status Report
                LReport_Id = "3";
                break;
            case "M303": Get_Content_Page_Report_ACOP_Form_Status_Dispatch(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '4');  // Form Status Dispatch Report
                LReport_Id = "4";
                break;

            case "M402": Get_Content_Page_DP_Minor_Turns_Major_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '2');  // Minor Turns Major Report
                LReport_Id = "2";
                break;
            case "M403": Get_Content_Page_DP_Multi_Client_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '12');  // Multiple Client Report
                LReport_Id = "12";
                break;

            case "M404": Get_Content_Page_Report_DP_Dormant_Client_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '13');  // Dormant Client Report
                LReport_Id = "13";
                break;
            case "M405": Get_Content_Page_Report_DP_ISIN(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '20');  // ISIN Report
                LReport_Id = "20";
                break;
            case "M406": Get_Content_Page_Report_DP_Bill_Summary_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '14');
                LReport_Id = "14";
                break;
            case "M407": Get_Content_Page_Report_DP_Book_Issue_Register_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '19');
                LReport_Id = "19";
                break;
            case "M408": Get_Content_Page_Report_DP_No_Nominee(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '21');
                LReport_Id = "21";
                break;
            case "M409": Get_Content_Page_Report_DP_Used_Unused_Slips_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '40');  // Ledger Report
                LReport_Id = "40";
                break;
            case "M410": Get_Content_Page_Report_DP_Slip_Requisition_Status_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '41');  // Slip Requisition Status Report
                LReport_Id = "41";
                break;
            case "M411": Get_Content_Page_Report_DP_Client_Summary_Report_SSO(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '54');
                LReport_Id = "54";
                break;
            case "M412": Get_Content_Page_Report_DP_Client_Summary_Report_CMR_Report_SSO(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '55');
                LReport_Id = "55";
                break;
            case "M413": Get_Content_Page_DP_Pledge_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '58');
                LReport_Id = "58";
                break;
            case "M414": Get_Content_Page_Transaction_View_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '57');
                LReport_Id = "57";
                break;
            case "M415": Get_Content_Page_Report_DP_POA(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '56');
                LReport_Id = "56";
                break;
            case "M416": Get_Content_Page_Report_DP_Instruction_Verification_Status_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '61');
                LReport_Id = "61";
                break;
            case "M417": Get_Content_Page_Dis_Pledge_Reconcilation_Report_DP(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '59');
                LReport_Id = "59";
                break;
            case "M418": Get_Content_Page_Dis_Request_Reconcilation_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '60');
                LReport_Id = "60";
                break;
            case "M421": Get_Content_Page_Dis_Issuance_New_Clients(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '62');;
                break;
            case "M423": Get_Content_Page_Report_DP_Scheme_Modification_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '60');
                LReport_Id = "63";
                break;
            case "S001": Get_Content_Page_Mytask(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag);  // Setting
                break;
            case "S002": Get_Content_Page_Search(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag);  // Setting
                break;
            case "S003": Get_Content_Page_Mydownloads(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag);  // Setting
                break;
            case "S004": Get_Content_Page_Setting(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag);  // Setting
                break;
            case "M502": Get_Content_Page_settlement_calendar_report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag);
                break;
            case "M503": Get_Content_Page_settlement_client_settlement_report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag);
                break;
            case "M504": Get_Content_Page_Settlement_Closeout_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag);
                break;
            case "M505": Get_Content_Page_Settlement_Delivery_Position_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag);
                break;
            case "M506": Get_Content_Page_Debit_Stock_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag);
                break;
            case "M507": Get_Content_Page_Settlement_Buy_Sell_Shortage_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag);
                break;
            case "M508": Get_Content_Page_TS_EDP_Miscellaneous_6_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag);
                break;
            case "M509": Get_Content_Page_EDP_Miscellaneous_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag);
                break;
            case "M510": Get_Content_Page_TS_Edp_5_Sauda_Summary_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag);
                break;
            case "M511": Get_Content_Page_TS_EDP_Pledge_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag);
                break;
            case "M512": Get_Content_Page_EDP_Holding_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag);
                break;
            case "M513": Get_Content_Page_EDP_Miscellaneous_Combined_Stock_Details_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag);
                break;
            case "M514": Get_Content_Page_TS_Settlement_Auction_Bill(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag);
                break;
            case "M515": Get_Content_Page_TS_EDP_Miscellaneous_Open_Position_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag);  // Settlement Auction Bill 040221
                break;
            case "M516": Get_Content_Page_EDP_Brokerage_Turnover_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag);  // EDP Brokerage Turnover Report
                break;
            case "M517": Get_Content_Page_EDP_STT_CTT_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag);  // EDP STT/CTT Report Report
                break;
            case "M602": Get_Content_Page_Banking_Interest_Ledger_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '33');
                LReport_Id = "33";
                break;
            case "M603": Get_Content_Page_Report_DP_Ledger_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '32');
                LReport_Id = "32";
                break;
            case "M604": Get_Content_Page_BO_Multi_Client_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '12');
                LReport_Id = "12";
                break;
            case "M605": Get_Content_Page_BO_Collateral_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '70');
                LReport_Id = "70";
                break;
            case "M606": Get_Content_Page_BO_White_Paper_Holistic_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '75');
                LReport_Id = "75";
                break;
            case "M607": Get_Content_Page_BO_Operation_Ledger_Report(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '81');
                LReport_Id = "81";
                break;
            case "M702": Get_Content_Page_Report_MCORE_Certificate_Expiry(ContentOption, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, '62');
                LReport_Id = "62";
                break;
            default:
                if (Page_Is_External_Page == 1 && Page_External_URL != "") {
                    // Check if any external page is linked to the page.  If yes then call that
                    Open_Content_Page_External_URL(Page_External_URL);
                } else if (Page_Menu_Tooltip != "") {
                    // Check if this page is common report page without filters
                    Get_Content_Page_Report_Without_Fillters_Common(Page_Menu_Tooltip);
                } else {
                    // Show message to user that this menu is not configured in the system
                    Show_Error_Toastr("This menu is not configured in the system.");
                
            }
        }


    $("#btnlogout").click(function () {
        Confirm_Logout();
    });

    $("#btnlogouticon").click(function () {
        Confirm_Logout();
    });

    

    //// Set initial active toggle
    //$("[data-toggle='slide.'].is-expanded").parent().toggleClass('is-expanded');
    $("#js-nav-menu").navigationDestroy();
    $("#js-nav-menu").navigation({

    });

    // Set Current Menu as active
    var Open_MenuId = "#mnu" + ContentOption;    // Current Open Menu

    
    // Read all parents of current menu
    var Open_Parent_MenuId = document.getElementById("mnu" + ContentOption).getAttribute("data-parent-menu");
    //console.log(Open_Parent_MenuId);

    // Split using comma
    var Open_Parent_MenuId_Array = Open_Parent_MenuId.split(",");

    // Loop through Array Items
    for (var i = 0; i < Open_Parent_MenuId_Array.length; i++) {
        $("#mnu" + Open_Parent_MenuId_Array[i]).addClass('active open');
    }
    $(Open_MenuId).addClass('active');


});

function Add_Menu_TreeNode(Parent_Menu_Id, Menu_List, Parent_List, Parent_Name_List, callback) {
    var Menu_HTML = "";

    for (var i = 0; i < Menu_List.length; i++) {

        if (Menu_List[i].Parent_Menu_Id == Parent_Menu_Id) {
            // Add a new Node for ProductGroup
            var Menu_Id = Menu_List[i].Menu_Id;
            var Menu_Link = Menu_List[i].Menu_Link;
            var Menu_Name = Menu_List[i].Menu_Name;
            var Menu_Icon_Name = Menu_List[i].Menu_Icon_Name;
            var Menu_Parent_List = Parent_List + "," + Parent_Menu_Id;
            var Menu_Parent_Name_List = Parent_Name_List + "%:%" + Menu_Name;

            if (Menu_Link == "" || Menu_Link == null) {
                Menu_Link = "?CO=" + window.btoa(Menu_Id) + "&VN=" + window.btoa(Global_Var_Release_No);   // Internal Link for Menu.  Based on this Content page will be loaded
            }

            var SubMenu_HTML = "";
            Add_Menu_TreeNode(Menu_Id, Menu_List, Menu_Parent_List, Menu_Parent_Name_List, function (Tree_Ret_HTML) {
                var Output_Str = Tree_Ret_HTML;
                if (Output_Str != "") {
                    SubMenu_HTML = "<ul>" + Output_Str + "</ul>";
                }
            });

            // Generate html for current menu
            if (SubMenu_HTML != "") {
                Menu_Link = "#";    // If the current menu has sub menu then the link will be only #
            }

            var Cur_Menu_HTML = "<li id=\"mnu" + Menu_Id + "\" data-parent-menu=\"" + Menu_Parent_List + "\" data-parent-menu-name=\"" + Menu_Parent_Name_List + "\"  >";
            Cur_Menu_HTML += "<a href=\"" + Menu_Link + "\" title=\"" + Menu_Name + "\" data-filter-tags=\"" + Menu_Name + "\" style=\"text-decoration:none !important; \">";
            if (Menu_Icon_Name != null && Menu_Icon_Name != "") {
                Cur_Menu_HTML += "<i class=\"fal " + Menu_Icon_Name + "\"></i>";
            }

            Cur_Menu_HTML += "<span class=\"nav-link-text\">" + Menu_Name + "</span>";
            Cur_Menu_HTML += "</a>";
            Cur_Menu_HTML += SubMenu_HTML;

            Menu_HTML += Cur_Menu_HTML;

            // Close menu
            Menu_HTML += "</li>";

        }
    };
    return callback(Menu_HTML);
}

/*Logout*/
function Confirm_Logout() {

    var btn = "button";
    swal({
        title: "Are you sure?",
        text: "Want to Logout..!",
        type: "warning",
        text: 'Want to Logout..! <br><button type="' + btn + '" id="btnNo" style="background:#c1c1c1;">No, cancel please!</button> ' +
            '<button type="' + btn + '" id="btnYes"  style="background:#DD6B55;">Yes, logout please!</button> ',
        html: true,
        showConfirmButton: false
    });
}

$(document).on('click', "#btnNo", function () {
    swal.close();
});

$(document).on('click', "#btnYes", function () {
    Logout_Application();
});
/*End Logout*/


