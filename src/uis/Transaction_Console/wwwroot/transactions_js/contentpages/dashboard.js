var LPMF = 0;   // Local Page Modify Flag
var LPDF = 0;   // Local Page Delete Flag

/*var Global_Var_Login_Name;*/
function Get_Content_Page_Dashboard(Menu_Id, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag) {
    //alert(Menu_Id);

    var divContent = document.getElementById("ContentDiv");
    var Page_url = "../contentpages/dashboard.html" + "?v" + Global_Var_Release_No;

    $.get(Page_url, function (response) {
        divContent.innerHTML = response;
        

    });

}
