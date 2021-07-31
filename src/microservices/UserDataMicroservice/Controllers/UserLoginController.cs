using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Middleware;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UserDataMicroservice.Repository;
using UserDataMicroservice.Entities;
using System.Windows;
using System.Text;

namespace UserDataMicroservice.Controllers
{
    [Route("v1/api/user/login/")]
    [ApiController]

    public class UserLoginController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IEncryptor _encryptor;

        public UserLoginController(ILoggerManager logger, IUserRepository userRepository, IJwtBuilder jwtBuilder, IEncryptor encryptor)
        {
            _logger = logger;
            _userRepository = userRepository;
            _jwtBuilder = jwtBuilder;
            _encryptor = encryptor;
        }

        #region UserLogin
        public class Input_Param_User_Credentials
        {
            public string User_Name { get; set; }
            public string User_Password { get; set; }
            public string App_Key { get; set; }
            public string IP_Address { get; set; }
        }

        public class Return_User_Credentials
        {
            public string Res_Output { get; set; }
        }

        [HttpPost("Get_User_Authentication", Name = "Get_User_Authentication")]
        public IActionResult Get_User_Authentication([FromBody] Input_Param_User_Credentials User_Credentials)
        {
            try
            {
                if (User_Credentials == null)
                {
                    var Err_Msg_Str = "User Credentials is null";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var User_Name = User_Credentials.User_Name;
                var User_Password = User_Credentials.User_Password;
                var App_Key = User_Credentials.App_Key;
                var IP_Address = User_Credentials.IP_Address;
                var Method_Name = "Get";

                using (var context = new User_RepositoryContext())
                {
                    var UserDetails = context.User_Details.FromSqlInterpolated($"CALL USP_User_Login ({Method_Name},{User_Name},{User_Password},{App_Key});").ToList();
                    if (UserDetails.Count > 0)
                    {

                        var token = _jwtBuilder.GetToken(UserDetails[0].User_Id);
                        var User_Id = UserDetails[0].User_Id;
                        var User_Display_Name = UserDetails[0].User_Name;
                        var Org_Id = UserDetails[0].Org_Id;
                        var Org_Name = UserDetails[0].Org_Name;
                        var Is_Authorised = UserDetails[0].Is_Authorised;
                        var Config_String = UserDetails[0].Config_String;
                        var Eva_Config_String = UserDetails[0].Eva_Config_String;
                        var User_Email = UserDetails[0].User_Email;
                        var User_Mobile = UserDetails[0].User_Mobile;
                        var Gender_Id = UserDetails[0].Gender_Id;

                        _logger.LogInfo($"User details found.");

                        // Generate Output String
                        var Return_User = new Return_User_Credentials();
                        Return_User.Res_Output = "{\"Response\": \"OK\", \"token\": \"" + token + "\", \"User_Id\": \"" + User_Id + "\", \"User_Display_Name\": \"" + User_Display_Name + "\", \"Org_Id\": \"" + Org_Id + "\", \"Org_Name\": \"" + Org_Name + "\", \"Is_Authorised\": \"" + Is_Authorised + "\", \"Config_String\": " + Config_String + ", \"Eva_Config_String\": " + Eva_Config_String + ", \"User_Email\": \"" + User_Email + "\", \"User_Mobile\": \"" + User_Mobile + "\", \"Gender_Id\": \"" + Gender_Id + "\"}";
                        return Ok(Return_User);

                        

                    }
                    else
                    {
                        // Generate Output String
                        var Return_User = new Return_User_Credentials();
                        Return_User.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EL01\", \"Error_Msg\": \"Invalid Sign-in details\"}";
                        return Ok(Return_User);
                    }

                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                var Return_Error = new Return_User_Credentials();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EL02\", \"Error_Msg\": \"" + ex.Message + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        //API to get Menu List for user based on user_Id
        public class Input_Param_User_Menu
        {
            public string User_Id { get; set; }
            public string Org_Id { get; set; }
            public string Application_Id { get; set; }
        }

        public class Return_User_Menu
        {
            public string Res_Output { get; set; }
        }

        [HttpPost("Get_User_Menu", Name = "Get_User_Menu")]
        public IActionResult Get_User_Menu([FromBody] Input_Param_User_Menu User_Details)
        {
            try
            {
                if (User_Details == null)
                {
                    var Err_Msg_Str = "User details not found";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var User_Id = User_Details.User_Id;
                var Org_Id = User_Details.Org_Id;
                var Application_Id = User_Details.Application_Id;
                var Method_Name = "Get";

                using (var context = new User_RepositoryContext())
                {
                    var UserMenu = context.User_Menu.FromSqlInterpolated($"CALL USP_User_Menu ({Method_Name},{Org_Id},{User_Id},{Application_Id});").ToList();
                    if (UserMenu.Count > 0)
                    {
                        var Menu_List = JsonConvert.SerializeObject(UserMenu);

                        _logger.LogInfo($"Menu details found.");

                        // Generate Output String with Menu
                        var Return_Menu = new Return_User_Menu();
                        Return_Menu.Res_Output = "{\"Response\": \"OK\",\"Menu_List\": " + Menu_List + "}";
                        return Ok(Return_Menu);
                    }
                    else
                    {
                        // Generate Output String with blank menu
                        var Return_Menu = new Return_User_Menu();
                        Return_Menu.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EL03\", \"Error_Msg\": \"Menu not assigned to your account\"}";
                        return Ok(Return_Menu);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                var Return_Error = new Return_User_Menu();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EL04\", \"Error_Msg\": \"" + ex.Message + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        #endregion

        #region CheckValidEmailid

        //API to validate user login and password
        public class Input_Param_User_Emailid
        {
            public string Emailid { get; set; }
            public string App_Key { get; set; }
            public string IP_Address { get; set; }
        }

        public class Return_User_Emailid
        {
            public string Res_Output { get; set; }
        }

        [HttpPost("Check_Valid_Emailid", Name = "Check_Valid_Emailid")]
        public IActionResult Check_Valid_Emailid([FromBody] Input_Param_User_Emailid User_EmailId)
        {
            try 
            {
                if (User_EmailId == null)
                {
                    var Err_Msg_Str = "User Credentials is null";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Emailid = User_EmailId.Emailid;
                var App_Key = User_EmailId.App_Key;
                var IP_Address = User_EmailId.IP_Address;
                var Method_Name = "GetByEmailid";
                //var UserId = "";
                using (var context = new User_RepositoryContext())
                {
                    var UserDetails = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_User_PasswordValidation ({Method_Name},{Emailid},'1',{App_Key}, {IP_Address},'';").ToList();
                       if (UserDetails[0].Result_Id == 1)
                       {
                        var RequestedID = UserDetails[0].Result_Extra_Key;
                        var User_Id = UserDetails[0].Result_Description;

                        _logger.LogInfo($"User details found.");

                        var User_IdBytes = Encoding.UTF8.GetBytes(User_Id);
                        string encodedUser_Id = Convert.ToBase64String(User_IdBytes);
                        var RequestedIDBytes = Encoding.UTF8.GetBytes(RequestedID);
                        string encodedRequestedID = Convert.ToBase64String(RequestedIDBytes);
                        var Url = "http:localhost:44317/setPassword.html?id=" + encodedUser_Id + "&UmVxdWVzdElE=" + encodedRequestedID + "";

                        var Return_User = new Return_User_Emailid();
                        Return_User.Res_Output = "{\"Response\": \"success\", \"success_Msg\": \"" + encodedUser_Id + "\", \"RequestedID\": \"" + encodedRequestedID + "\"}";

                        return Ok(Return_User);
                    }
                    else
                    {
                        // Generate Output String
                        var Return_User = new Return_User_Emailid();
                        Return_User.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EL01\", \"Error_Msg\": \"Invalid Emailid\"}";
                        return Ok(Return_User);
                    }

                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                var Return_Error = new Return_User_Emailid();
                Return_Error.Res_Output = "{\"Response\": \"Invalid Emailid\", \"Error_No\": \"EL02\", \"Error_Msg\": \"" + ex.Message + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        #endregion

        #region GetUserDetails

        //API to validate user login and password
        public class Input_Param_Userdetails
        {
            public string UserId { get; set; }

            public string RequestID { get; set; }
            
            public string App_Key { get; set; }
            public string IP_Address { get; set; }
        }

        public class Return_UserDetails
        {
            public string Res_Output { get; set; }
        }

        [HttpPost("GetuserDetails", Name = "GetuserDetails")]
        public IActionResult GetuserDetails([FromBody] Input_Param_Userdetails User_Details)
        {
            try
            {
                if (User_Details == null)
                {
                    var Err_Msg_Str = "User Credentials is null";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var UserId = User_Details.UserId;
                var App_Key = User_Details.App_Key;
                var IP_Address = User_Details.IP_Address;
                var Method_Name = "GetByUserid";
                var RequestID = User_Details.RequestID;
                var Password = "";
                var Emailid = "";

                using (var context = new User_RepositoryContext())
                {
                    var UserDetails = context.User_Emailid.FromSqlInterpolated($"CALL dbo.USP_User_PasswordValidation ({Method_Name},{Emailid},{UserId},{App_Key}, {IP_Address},{Password},{RequestID});").ToList();
                    if (UserDetails.Count > 0)
                    {
                        var User_Id = UserDetails[0].User_Id;
                        var User_Email = UserDetails[0].User_Email;
                        var User_Name = UserDetails[0].User_Name;

                        _logger.LogInfo($"User details found.");

                        var Return_User = new Return_UserDetails();
                        Return_User.Res_Output = "{\"Response\": \"success\", \"UserName\": \"" + User_Name + "\", \"Error_Msg\": \"Invalid Emailid\"}";
                        return Ok(Return_User);
                    }
                    else
                    {
                        // Generate Output String
                        var Return_User = new Return_UserDetails();
                        Return_User.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EL01\", \"Error_Msg\": \"Invalid Emailid\"}";
                        return Ok(Return_User);
                    }

                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                var Return_Error = new Return_UserDetails();
                Return_Error.Res_Output = "{\"Response\": \"Invalid Emailid\", \"Error_No\": \"EL02\", \"Error_Msg\": \"" + ex.Message + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        #endregion

        #region UpdatePassword

        public class UserPassword
        {
            public string UserId { get; set; }
            public string Password { get; set; }
            public string App_Key { get; set; }
            public string IP_Address { get; set; }
        }

        public class Return_UserPassword
        {
            public string Res_Output { get; set; }
        }

        [HttpPost("UpdatePassword", Name = "UpdatePassword")]
        public IActionResult UpdatePassword([FromBody] UserPassword User_Detail)
        {
            try
            {
                if (User_Detail == null)
                {
                    var Err_Msg_Str = "User Credentials is null";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var UserId = User_Detail.UserId;
                var App_Key = User_Detail.App_Key;
                var IP_Address = User_Detail.IP_Address;
                var Password = User_Detail.Password;
                var Method_Name = "Update";
                var Emailid = "";
                var RequestID = "";

                using (var context = new User_RepositoryContext())
                {

                    var UserDetails = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_User_PasswordValidation ({Method_Name},{Emailid},{UserId},{App_Key}, {IP_Address},{Password},{RequestID});").ToList();
                    if (UserDetails[0].Result_Id == 1)
                    {
                        _logger.LogInfo($"User details Update.");

                        var Return_User = new Return_UserPassword();
                        Return_User.Res_Output = "{\"Response\": \"success\",\"Error_Msg\": \"Invalid Password\"}";
                        return Ok(Return_User);
                    }
                    else
                    {
                        // Generate Output String
                        var Return_User = new Return_UserPassword();
                        Return_User.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EL01\", \"Error_Msg\": \"Invalid Password\"}";
                        return Ok(Return_User);
                    }

                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                var Return_Error = new Return_UserPassword();
                Return_Error.Res_Output = "{\"Response\": \"Invalid Userid\", \"Error_No\": \"EL02\", \"Error_Msg\": \"" + ex.Message + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        #endregion


    }



}
