// Checked by Vivek - AdminPanel Sign-In Page
var App_Key = "A000OMNS";
var Spinner_HTML = "<span class=\"spinner-border spinner-border-sm\" role=\"status\" aria-hidden=\"true\"></span>";

// SignalR Initialisation and Connection        
//const connection = new signalR.HubConnectionBuilder()
//    .withUrl("http://192.168.99.164:44331/chatHub", {
//        skipNegotiation: true,
//        transport: signalR.HttpTransportType.WebSockets
//    })
//    .withAutomaticReconnect()
//    .configureLogging(signalR.LogLevel.Information)
//    .build();       // SignalR connection

$(document).ready(function () {
    $("#erroralert").hide();
    $("#lblusername").hide();
    $("#lblpassword").hide();

    //Copy client side IP Address in hindden field
    //$.getJSON("http://jsonip.com?callback=?", function (data) {
    //    $("#hdnClientIPAddres").val(data.ip);
    //});

    //$.getJSON("http://jsonip.com?callback=?", function (data) {
    //    var Key = "Report_Console_Ip_Address";

    //    var User_Report_Console_Ip_Address_Str = JSON.stringify(data.ip);
    //    var User_Report_Console_Ip_Address_Str_En = window.btoa(User_Report_Console_Ip_Address_Str);
    //    var Key_En = window.btoa(Key);
    //    window.localStorage.setItem(Key_En, User_Report_Console_Ip_Address_Str_En);   // Store encrypted values in Local storage

    //    $("#hdnClientIPAddres").val(data.ip);
    //    // console.log(data.ip);
    //});

    // Check if Report_Console_RememberMe_Details_Status exists in Localstorage.  If exists then proceed else make RememberMe Checkbox Uncheck
    var User_Report_Console_RememberMe_Details_En = window.localStorage.getItem(window.btoa("Report_Console_RememberMe_Details_Status"));
    if (User_Report_Console_RememberMe_Details_En != undefined && User_Report_Console_RememberMe_Details_En != "null") {
        var User_Report_Console_RememberMe_Details = JSON.parse(window.atob(User_Report_Console_RememberMe_Details_En));
    }


    if (User_Report_Console_RememberMe_Details != undefined && User_Report_Console_RememberMe_Details != "null") {
        Read_From_Cookies(User_Report_Console_RememberMe_Details);
    } 

    //OLD CODE TO READ FROM COOKIES
    //var remember = window.btoa('remember_Report_Console_Username_Password');
    //// var remember = $.cookie(RememberMe);
    ////var remember = "";
    
    //if (remember != undefined && remember != "" && remember != "null") {
    //    Read_From_Cookies();
    //}
    //OLD CODE TO READ FROM COOKIES

    Get_App_Base_Url();

    // Load Config Settings
    //App_Key = Get_QueryParams_Decrypt('App_Key', 'App_Key');
    //if (App_Key != "" && App_Key != null) {
    //    if (window.btoa(App_Key) == 'null') {
    //        App_Key = Global_Var_App_Key_From_Base_Url;
    //        Set_Shortcut(App_Key);
    //    } else {
    //        Set_Shortcut(App_Key);
    //    }
    //} else if (Global_Var_App_Key_From_Base_Url != "") {
    //    App_Key = Global_Var_App_Key_From_Base_Url;
    //    Set_Shortcut(App_Key);
    //}

    $(".toggle-password").click(function () {
        $(this).toggleClass("fa-eye fa-eye-slash");
        var input = $($(this).attr("toggle"));
        if (input.attr("type") == "password") {
            input.attr("type", "text");
        } else {
            input.attr("type", "password");
        }
    });

    $("#txtpassword").keyup(function (event) {
        if (event.keyCode === 13) {
            $("#btnsignin").click();
        }
    });

    $("#btnsignin").click(function () {
        $("#erroralert").hide();
        $("#lblmessage").empty();
        var uname = $('#txtusername').parsley();
        var password1 = $('#txtpassword').parsley();

        if (App_Key == "" || App_Key == null) {
            var Error_Msg = "App Key not found";
            $("#erroralert").show();
            $("#lblmessage").empty();
            $("#lblmessage").html(Error_Msg);
        }
        else {
            if (uname.isValid() && password1.isValid()) {
                var $this = $(this);
                var loadingText = Spinner_HTML +  ' Validating...';
                if ($(this).html() !== loadingText) {
                    $this.data('original-text', $(this).html());
                    $this.html(loadingText);
                }

                var username = document.getElementById('txtusername').value;
                var password = document.getElementById('txtpassword').value;

                // Validate if username and password are correct
                Check_User_Validity($this, username, password, App_Key, function () {
                    var loadingTextRedirecting = Spinner_HTML +  ' Logging In...';
                    if ($this.html() !== loadingTextRedirecting) {
                        $this.data('original-text', $this.html());
                        $this.html(loadingTextRedirecting);

                    }

                    Remember_Me(username, password);

                    var Version_No = "0.0.140321";   // The version number to be passed to workplace for version control.
                    var Default_Menu_Id = "M002";
                    setTimeout(function () {
                        window.location = 'workplace.html?CO=' + window.btoa(Default_Menu_Id) + '&VN=' + window.btoa(Version_No) ;
                        
                    }, 500);

                });

            }
            else {
                uname.validate();
                password1.validate();
            }

        }
        
    });

    //connection.onclose(async () => {
    //    await Connect_SignalR();
    //});

    //Connect_SignalR();  // Start SignalR connection

    document.getElementById("txtusername").focus();
});

//Store Username and Password in cookies if remember me is checked
function Remember_Me(UserName, Password) {
    var Key = "Report_Console_RememberMe_Details_Status";

    if ($("#chkRememberMe").prop("checked")) {

        const values = {
            Remember: true,
            username: UserName,
            password: Password
        };

        var User_Report_Console_RememberMe_Details_Str = JSON.stringify(values);
        var User_Report_Console_RememberMe_Details_Str_En = window.btoa(User_Report_Console_RememberMe_Details_Str);
        var Key_En = window.btoa(Key);
        window.localStorage.setItem(Key_En, User_Report_Console_RememberMe_Details_Str_En);   // Store encrypted values in Local storage

    }
    else {
        // reset localStorage values
        const values = {
            Remember: null,
            username: null,
            password: null
        };

        var User_Admin_RememberMe_Details_Str = JSON.stringify(values);
        var User_Admin_RememberMe_Details_Str_En = window.btoa(User_Admin_RememberMe_Details_Str);
        var Key_En = window.btoa(Key);
        window.localStorage.setItem(Key_En, User_Admin_RememberMe_Details_Str_En);   // Store encrypted values in Local storage

    }

    //OLD CODE TO SAVE IN COOKIES
    //var CookUName = window.btoa('Report_Console_username');
    //var CookUPassword = window.btoa('Report_Console_password');
    //var CookUStatus = window.btoa('remember_Report_Console_Username_Password');

    //if ($("#chkRememberMe").prop("checked"))
    //{
    //    var username = window.btoa(UserName);
    //    var password = window.btoa(Password);

    //    // set cookies to expire in 14 days
    //    $.cookie(CookUName, username, { expires: 14 });
    //    $.cookie(CookUPassword, password, { expires: 14 });
    //    $.cookie(CookUStatus, window.btoa(true), { expires: 14 });
    //}
    //else
    //{
    //    // reset cookies
    //    $.cookie(CookUName, null);
    //    $.cookie(CookUPassword, null);
    //    $.cookie(CookUStatus, null);
    //}
    //OLD CODE TO SAVE IN COOKIES
}

//Read From Cookies UserName and Password
function Read_From_Cookies(User_Report_Console_RememberMe_Details) {

    var remember = User_Report_Console_RememberMe_Details.Remember;
    if (remember) {
        var username = User_Report_Console_RememberMe_Details.username;
        var password = User_Report_Console_RememberMe_Details.password;

        // autofill the fields
        $('#txtusername').val(username);
        $('#txtpassword').val(password);
        $('#chkRememberMe').prop('checked', true);
    }
    else {
        // autofill the fields
        $('#txtusername').val("");
        $('#txtpassword').val("");
        $('#chkRememberMe').prop('checked', false);
    }

    //OLD CODE TO READ IN COOKIES
    //var CookieUsername = window.btoa('Report_Console_username');
    //var CookiePassword = window.btoa('Report_Console_password');
    //var CookieRememberStatus = window.btoa('remember_Report_Console_Username_Password');

    //var remember = window.atob($.cookie(CookieRememberStatus));
    //if (remember == 'true')
    //{
    //    var username = window.atob($.cookie(CookieUsername));
    //    var password = window.atob($.cookie(CookiePassword));

    //    // autofill the fields
    //    $('#txtusername').val(username);
    //    $('#txtpassword').val(password);
    //    $('#chkRememberMe').prop('checked', true);
    //}
    //else
    //{
    //    // autofill the fields
    //    $('#txtusername').val("");
    //    $('#txtpassword').val("");
    //    $('#chkRememberMe').prop('checked', false);
    //}
    //OLD CODE TO READ IN COOKIES
}

function Check_User_Validity($this, username, password, App_Key, callback) {
    $("#erroralert").hide();
    var url = Global_Var_API_Base_Url + "/api/validate/Get_User_Authentication";
    window.localStorage.removeItem("09aSw0tYah2754");
    var Ip_Address = "";

    if ($("#hdnClientIPAddres").val() != "") {
        Ip_Address = $("#hdnClientIPAddres").val();
    } else { }


    var reqdata = {
        "User_Name": username,
        "User_Password": password,
        "App_Key": App_Key,
        "Ip_Address": Ip_Address
    };

    api_call.post(url, reqdata, '', function (res) {
      
        var res_Json = JSON.parse(res);
        const ret_data = JSON.parse(res_Json.res_Output);
        console.log(ret_data);
        if (ret_data.Response == "Error") {
            var Error_Msg = ret_data.Error_Msg;
            $("#erroralert").show();
            $("#lblmessage").empty();
            $("#lblmessage").html(Error_Msg);
            $this.html($this.data('original-text'));
        }

        else {
            const auth = {
                "id": ret_data.User_Id,
                "token": ret_data.token
            };
            window.localStorage.setItem("09aSw0tYah2754", JSON.stringify(auth));

            var User_Id = ret_data.User_Id;
            var User_Name = ret_data.User_Display_Name;
            var Org_Id = ret_data.Org_Id;
            var Org_Name = ret_data.Org_Name;
            var Is_Authorised = ret_data.Is_Authorised;
            var Config_String = ret_data.Config_String;
            var Eva_Config_String = ret_data.Eva_Config_String;
            var Login_Name = ret_data.Login_Name;
            var Is_New_User = ret_data.Is_New_User;
            var Allow_Multi_Login = ret_data.Allow_Multi_Login;

            var loadingTextRole = Spinner_HTML +  ' Checking Role...';
            if ($this.html() !== loadingTextRole) {
                $this.data('original-text', $this.html());
                $this.html(loadingTextRole);
            }

            Get_Menu_List_By_User_Id(User_Id, Org_Id, function (Menu_List) {
                var Key = "User_Details";
                const values = {
                    User_Id: User_Id,
                    User_Name: User_Name,
                    Org_Id: Org_Id,
                    Org_Name: Org_Name,
                    Is_Authorised: Is_Authorised,
                    App_Key: App_Key,
                    Is_New_User: Is_New_User,
                    Menu_List: Menu_List,
                    Login_Name: Login_Name,
                    Allow_Multi_Login: Allow_Multi_Login
                };

                var User_Details_Str = JSON.stringify(values);
                var User_Details_Str_En = window.btoa(User_Details_Str);
                var Key_En = window.btoa(Key);
                window.localStorage.setItem(Key_En, User_Details_Str_En);   // Store encrypted values in Local storage

                Key_En = "CS" + Org_Id;
                window.localStorage.setItem(Key_En, JSON.stringify(Config_String)); // Store config String in Local storage

                Key_En = "ECS" + Org_Id;
                Eva_Config_String_Str = JSON.stringify(Eva_Config_String);
                Eva_Config_String_Str_En = window.btoa(Eva_Config_String_Str);
                window.localStorage.setItem(Key_En, Eva_Config_String_Str_En); // Store Eva config String in Local storage

                //Get_User_Notification(Org_Id, User_Id); // Read Notification for User and store in Local storage

                //Send_SignalR_Message(User_Id, "A000", User_Id, "LoggedIn_" + ret_data.token);   // Send Message to same user to logout

                return callback(true);
            });
        }


    }, function (res) {
        $("#splash1").hide();
        $("#successalert").hide();
        $("#erroralert").show();
        $("#erroralertvalue").empty();
        $("#erroralertvalue").html("Something went wrong");
        $this.html($this.data('original-text'));

    });

}

function Get_Menu_List_By_User_Id(User_Id, Org_Id, callback) {
    $("#splash1").show();

    var url = Global_Var_API_Base_Url + "/api/validate/Get_User_Menu";
    var reqdata = {
        "User_Id": User_Id,
        "Org_Id": Org_Id,
        "Application_Id": "AP00"
    };

    // api_call.post(url, reqdata, token, function (res) {}, function (res) {})

    api_call.post(url, reqdata, '', function (res) {
        var res_Json = JSON.parse(res);
        const ret_data = JSON.parse(res_Json.res_Output);
        if (ret_data.Response == "Error") {
            $("#erroralert").show();
            $("#lblmessage").empty();
            $("#lblmessage").html("No Role assigned. Contact Administrator");
        }
        else {
            var Menu_List = ret_data.Menu_List;
            callback(Menu_List);
        }
    }, function (res) {
            $("#splash1").hide();
            $("#successalert").hide();
            $("#erroralert").show();
            $("#erroralertvalue").empty();
            $("#erroralertvalue").html("Something went wrong");
    }); 

}

//async function Connect_SignalR() {
//    try {
//        Object.defineProperty(WebSocket, 'OPEN', { value: 1, });
//        await connection.start();
//    } catch (err) {
//        setTimeout(() => start(), 5000);
//    }
//};

//// SignalR Message Sender
//function Send_SignalR_Message(FromUser, Org_Id, User_Id, MessageInput) {
//    connection.invoke("SendMessage", FromUser, Org_Id, User_Id, MessageInput).catch(function (err) {
//        return console.error(err.toString());
//    });

//};

// SignalR Message Receiver
//connection.on("ReceiveMessage", function (FromUser, Org_Id, User_Id, Message) {
//    //console.log("Received: From_" + FromUser + ", To_" + ToUser + ", Message_" + Message);
//    console.log(Message);

//    //if (Org_Id == window.Global_Var_Org_Id && User_Id == window.Global_Var_Org_Id) {
//    //    console.log(Message);
//    //    //switch (Message) {
//    //    //    case "Update_Notification": Get_User_Notification(User_Id); // Update Notifications for the User
//    //    //        break;
//    //    //    default:
//    //    //}

//    //}
//});