// Based on Domian name decide Location from where 
// Web services will get served.

var Global_Var_API_Base_Url = "";  // API_Base_URL
var Global_Var_Org_Id_From_Base_Url = "";  // App_Key based on Base Url

function Get_App_Base_Url() {
    var Cur_Page_Host = window.location.protocol + "//" + window.location.host;
    switch (Cur_Page_Host) {
        case "http://127.0.0.1:5000": Global_Var_API_Base_Url = "http://127.0.0.1:5000";
            break;
        case "https://127.0.0.1:5001": Global_Var_API_Base_Url = "https://127.0.0.1:5001";
            break;
        case "http://localhost:44310": Global_Var_API_Base_Url = "http://localhost:44300";
            break;
        default:
            Global_Var_Media_Base_Url = Cur_Page_Host;

    }       
}