// Checked by Vivek
var LPMF = 0;   // Local Page Modify Flag
var LPDF = 0;   // Local Page Delete Flag


function Get_Content_User_Change_Password(Menu_Id, Page_Display_Flag, Page_Add_Flag, Page_Modify_Flag, Page_Delete_Flag, Report_ID) {


    // Check if current user has Display rights for this page
    if (Page_Display_Flag == 0) {
        alert("Please login");
        Logout_Application();
        return;
    }

    // Get HTML Code of Content Page and Load in ContentDiv
    var divContent = document.getElementById("ContentDiv");
    var Page_url = "./contentpages/user_change_password.html";

    $.get(Page_url, function (Page_Content) {
        divContent.innerHTML = Page_Content;
        $('#divPasswordInvalid').hide();
        $("#divAdd").show();
        $("#loader").hide();
        LPMF = Page_Modify_Flag;
        LPDF = Page_Delete_Flag;

        $('[data-toggle="tooltip"]').tooltip();

        $("#btnResetPassword").click(function () {
            Reset_User_Password();
            var $this = $(this);
            var loadingText = '<i class="fal fa-spinner fa-spin"></i>';
            if ($(this).html() !== loadingText) {
                $this.data('original-text', $(this).html());
                $this.html(loadingText);
            }
            setTimeout(function () {
                $this.html($this.data('original-text'));

            }, 1000);
        });
    });


    // Validate User before Password Reset
    function Validate_User_Before_Resest() {
        // Validate Password

        if ($("#txtNewPassword").val() == "") {

            $("#txtNewPassword").addClass('is-invalid');
            Show_Error_Toastr("Please Enter Password");
            return false;
        }
        else if ($("#txCnfPassword").val() == "") {
            $("#txCnfPassword").addClass('is-invalid');
            Show_Error_Toastr("Please Enter Confirm Password");
            return false;
        }
        else if ($("#txCnfPassword").val() == "" && $("#txCnfPassword").val() == "") {
            $("#txtNewPassword").addClass('is-invalid');
            $("#txCnfPassword").addClass('is-invalid');
        }
        else {
            $("#txtNewPassword").addClass('is-valid');
            $("#txCnfPassword").addClass('is-valid');
            return true;
        }
    }

    function ValidateMismatchPassword() {
        if (($('#txtNewPassword').val() != $('#txCnfPassword').val())) {
            $("#txtNewPassword").addClass('is-invalid');
            $("#txCnfPassword").addClass('is-invalid');
            Show_Error_Toastr("Password Mismatch!!");
            return false;
        } else {
            $("#txtNewPassword").addClass('is-valid');
            $("#txCnfPassword").removeClass('is-invalid');
            return true;
        }
    }
    // Reset Password  
    function Reset_User_Password() {

        var validationresult = Validate_User_Before_Resest();
        var ValidateMismatchPass = ValidateMismatchPassword();

        if (validationresult == true && ValidateMismatchPass == true) {
            var User_Password = document.getElementById('txtNewPassword').value;

            const auth = JSON.parse(window.localStorage.getItem("09aSw0tYah2754"));
            var url = Global_Var_API_Base_Url + "/api/user/Reset_User_Password";
            var reqdata = {
                "Org_Id": window.Global_Var_Org_Id,
                "User_Id": Global_Var_User_Id,
                "User_Name": Global_Var_Login_Name,
                "User_Password": User_Password,
                "LastEdited_By": window.Global_Var_User_Id
            };
            api_call.post(url, reqdata, auth.token, function (res) {
                // Show success Message
                //Clear_User_After_Password_Reset();
                //Show_Success_Toastr('Password updated successfully');
                Popup_Model_Logout();
                
            }, function (res) {
                var res_Json = JSON.parse(res);
                res_Json = JSON.parse(res_Json.res_Output);
                Show_Error_Toastr(res_Json.Error_Msg);
            });

        }
    }

    // Clear User form elements after password reset
    function Clear_User_After_Password_Reset() {
        $("#txtNewPassword").removeClass('is-invalid');
        $("#txtNewPassword").removeClass('is-valid');
    }

}
//Password Validation 
function PasswordValidator_1(txtPassword) {
    var val = txtPassword;
    var regularExpression = /^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%&]).*$/;
    if (regularExpression.test(val)) {
        $('#txtNewPassword').removeClass('is-invalid');
        $('#divPasswordInvalid').hide();
    }
    else {
        $('#divPasswordInvalid').show();
        $('#txtNewPassword').addClass('is-invalid');

    }
}

//Pop model popup
function Popup_Model_Logout() {
    var btn = "button";
    swal({
        title: "Password reset successfully",
        text: "Please login with new password",
        type: "success",
        text: 'Please login with new password <br>'+
            '<button type="' + btn + '" id="btnYes"  style="background:#DD6B55;">Ok</button> ',
        html: true,
        showConfirmButton: false
    });
   
}