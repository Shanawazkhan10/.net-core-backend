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

namespace UserDataMicroservice.Controllers
{
    [Route("v1/api/user/master/")]
    [ApiController]

    public class UserMasterController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IEncryptor _encryptor;

        public UserMasterController(ILoggerManager logger, IUserRepository userRepository, IJwtBuilder jwtBuilder, IEncryptor encryptor)
        {
            _logger = logger;
            _userRepository = userRepository;
            _jwtBuilder = jwtBuilder;
            _encryptor = encryptor;
        }

        // Common Function
        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public string Get_Current_DateTime()    // Gives current datetime.  Should be modified to find IST
        {
            var Cur_DateTime = DateTime.Now.AddMinutes(210).ToString("yyyy-MM-dd HH:mm:ss");     //yyyy-mm-dd hh:mm:ss(24h) // Switzerland time (210)
            return Cur_DateTime;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public string Get_Normalised_Message(string Input_Msg)  // This function converts Error message with special characters to normal message
        {
            string Input_Msg_1 = Input_Msg.Replace("\r\n", " ");
            string Input_Msg_2 = Input_Msg_1.Replace(@"""", @"\""");
            string Output_Msg = Input_Msg_2.Replace(@"'", @"\""");
            return Output_Msg;
        }

        public class Return_Master
        {
            public string Res_Output { get; set; }
        }

        #region Application
        // Application related API
        // API to Read All Application
        public class Input_Param_Application_Read_All
        {
            public string Org_Id { get; set; }
            public string Application_Name { get; set; }
        }

        [HttpPost("Read_All_Application", Name = "Read_All_Application")]
        public IActionResult Read_All_Application([FromBody] Input_Param_Application_Read_All Application_Data)
        {
            try
            {
                if (Application_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Application_Data.Org_Id;
                var Application_Name = Application_Data.Application_Name;
                var Application_Id = "";
                var Method_Name = "Get";

                using (var context = new User_RepositoryContext())
                {
                    var Return_Master_List = context.Application_List.FromSqlInterpolated($"CALL dbo.USP_Config_Application ({Method_Name},{Org_Id},{Application_Id},{Application_Name},{1},{0},{null},{""},{null},{""},{""});").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        var Return_Master_String = JsonConvert.SerializeObject(Return_Master_List);

                        // Generate Output String 
                        var Return_Master = new Return_Master();
                        Return_Master.Res_Output = "{\"Response\": \"OK\",\"Response_Data\": " + Return_Master_String + "}";
                        return Ok(Return_Master);
                    }
                    else
                    {
                        // Generate Output String
                        var Return_Master = new Return_Master();
                        Return_Master.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ0\", \"Error_Msg\": \"No record found\"}";
                        return Ok(Return_Master);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                string Err_msg_1 = ex.Message.Replace(@"""", @"\""");
                string Err_msg = Err_msg_1.Replace(@"'", @"\""");
                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ1\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        [HttpPost("Read_Config_Application", Name = "Read_Config_Application")]
        public IActionResult Read_Config_Application([FromBody] Input_Param_Application_Read_All Config_Application_Data)
        {
            try
            {
                if (Config_Application_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Config_Application_Data.Org_Id;
                var Config_Application_Name = Config_Application_Data.Application_Name;
                var Config_Application_Id = "";
                var Method_Name = "Get_All";

                using (var context = new User_RepositoryContext())
                {
                    var Return_Master_List = context.Application_List.FromSqlInterpolated($"CALL dbo.USP_Config_Application ({Method_Name},{Org_Id},{Config_Application_Id},{Config_Application_Name},{1},{0},{null},{""},{null},{""},{""});").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        var Return_Master_String = JsonConvert.SerializeObject(Return_Master_List);

                        // Generate Output String 
                        var Return_Master = new Return_Master();
                        Return_Master.Res_Output = "{\"Response\": \"OK\",\"Response_Data\": " + Return_Master_String + "}";
                        return Ok(Return_Master);
                    }
                    else
                    {
                        // Generate Output String
                        var Return_Master = new Return_Master();
                        Return_Master.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ0\", \"Error_Msg\": \"No record found\"}";
                        return Ok(Return_Master);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                string Err_msg_1 = ex.Message.Replace(@"""", @"\""");
                string Err_msg = Err_msg_1.Replace(@"'", @"\""");
                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ1\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        // API to Add or Remove Application
        public class Input_Param_Add_Remove_Application
        {
            public string Org_Id { get; set; }
            public string Application_Id { get; set; }
            public int Is_Active { get; set; }
            public string LastEdited_By { get; set; }
        }

        [HttpPost("Add_Remove_Application", Name = "Add_Remove_Application")]
        public IActionResult Add_Remove_Application([FromBody] Input_Param_Add_Remove_Application Application_Data)
        {
            try
            {
                if (Application_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Application_Data.Org_Id;
                var Application_Id = Application_Data.Application_Id;
                var Application_Name = "";
                int Is_Deleted = 0;
                var LastEdited_By = Application_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();
                int Is_Active = Application_Data.Is_Active;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = "";
                if (Application_Data.Is_Active == 1)
                {
                    Method_Name = "Create";
                }
                else
                {
                    Method_Name = "Delete";
                }


                using (var context = new User_RepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Config_Application ({Method_Name},{Org_Id},{Application_Id},{Application_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By},{""});").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        if (Return_Master_List[0].Result_Id == 1)
                        {
                            // Generate Output String 
                            var Return_Master = new Return_Master();
                            Return_Master.Res_Output = "{\"Response\": \"OK\"}";
                            return Ok(Return_Master);
                        }
                        else if (Return_Master_List[0].Result_Id == -1)
                        {
                            // Generate Output String 
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ2\", \"Error_Msg\": \"Entry in use.  Can't block\"}";
                            return StatusCode(500, Return_Error);
                        }
                        else
                        {
                            // Generate Output String
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ3\", \"Error_Msg\": \"Record not blocked\"}";
                            return StatusCode(500, Return_Error);
                        }

                    }
                    else
                    {
                        // Generate Output String
                        var Return_Error = new Return_Master();
                        Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"No record found\"}";
                        return StatusCode(500, Return_Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                string Err_msg_1 = ex.Message.Replace(@"""", @"\""");
                string Err_msg = Err_msg_1.Replace(@"'", @"\""");
                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        // API to Read One Application
        public class Input_Param_Application_Read_One
        {
            public string Org_Id { get; set; }
            public string Application_Id { get; set; }
        }

        [HttpPost("Read_One_Application", Name = "Read_One_Application")]
        public IActionResult Read_One_Application([FromBody] Input_Param_Application_Read_One Application_Data)
        {
            try
            {
                if (Application_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Application_Data.Org_Id;
                var Application_Id = Application_Data.Application_Id;
                var Application_Name = "";
                var Method_Name = "Get_One";

                using (var context = new User_RepositoryContext())
                {
                    var Return_Master_List = context.Application_List.FromSqlInterpolated($"CALL dbo.USP_Config_Application ({Method_Name},{Org_Id},{Application_Id},{Application_Name},{1},{0},{null},{""},{null},{""},{""});").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        var Return_Master_String = JsonConvert.SerializeObject(Return_Master_List);

                        // Generate Output String 
                        var Return_Master = new Return_Master();
                        Return_Master.Res_Output = "{\"Response\": \"OK\",\"Response_Data\": " + Return_Master_String + "}";
                        return Ok(Return_Master);
                    }
                    else
                    {
                        // Generate Output String
                        var Return_Error = new Return_Master();
                        Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ0\", \"Error_Msg\": \"No record found\"}";
                        return StatusCode(500, Return_Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                string Err_msg_1 = ex.Message.Replace(@"""", @"\""");
                string Err_msg = Err_msg_1.Replace(@"'", @"\""");
                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ1\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        // API to Update Application
        public class Input_Param_Application_Update
        {
            public string Org_Id { get; set; }
            public string Application_Id { get; set; }
            public string Application_Name { get; set; }
            public int Is_Active { get; set; }
            public string LastEdited_By { get; set; }
        }

        [HttpPost("Update_Application", Name = "Update_Application")]
        public IActionResult Update_Application([FromBody] Input_Param_Application_Update Application_Data)
        {
            try
            {
                if (Application_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Application_Data.Org_Id;
                var Application_Id = Application_Data.Application_Id;
                var Application_Name = Application_Data.Application_Name.Trim();
                int Is_Active = Application_Data.Is_Active;
                var LastEdited_By = Application_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();


                int Is_Deleted = 0;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = "Update";

                using (var context = new User_RepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Config_Application ({Method_Name},{Org_Id},{Application_Id},{Application_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By},{""});").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        if (Return_Master_List[0].Result_Id == 1)
                        {
                            // Generate Output String 
                            var Return_Master = new Return_Master();
                            Return_Master.Res_Output = "{\"Response\": \"OK\"}";
                            return Ok(Return_Master);
                        }
                        else if (Return_Master_List[0].Result_Id == -1)
                        {
                            // Generate Output String 
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ2\", \"Error_Msg\": \"Duplicate record exists\"}";
                            return StatusCode(500, Return_Error);
                        }
                        else
                        {
                            // Generate Output String
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ3\", \"Error_Msg\": \"Record not saved\"}";
                            return StatusCode(500, Return_Error);
                        }

                    }
                    else
                    {
                        // Generate Output String
                        var Return_Error = new Return_Master();
                        Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"No record found\"}";
                        return StatusCode(500, Return_Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                string Err_msg_1 = ex.Message.Replace(@"""", @"\""");
                string Err_msg = Err_msg_1.Replace(@"'", @"\""");
                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        // API to Read All Config Options
        public class Input_Param_Config_Read_All
        {
            public string Org_Id { get; set; }
        }

        [HttpPost("Read_All_Config", Name = "Read_All_Config")]
        public IActionResult Read_All_Config([FromBody] Input_Param_Config_Read_All Application_Data)
        {
            try
            {
                if (Application_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Application_Data.Org_Id;
                var Method_Name = "Get_Config";

                using (var context = new User_RepositoryContext())
                {
                    var Return_Master_List = context.Application_List.FromSqlInterpolated($"CALL dbo.USP_Config_Application ({Method_Name},{Org_Id},{""},{""},{1},{0},{null},{""},{null},{""},{""});").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        var Return_Master_String = JsonConvert.SerializeObject(Return_Master_List);

                        // Generate Output String 
                        var Return_Master = new Return_Master();
                        Return_Master.Res_Output = "{\"Response\": \"OK\",\"Response_Data\": " + Return_Master_String + "}";
                        return Ok(Return_Master);
                    }
                    else
                    {
                        // Generate Output String
                        var Return_Master = new Return_Master();
                        Return_Master.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ0\", \"Error_Msg\": \"No record found\"}";
                        return Ok(Return_Master);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                string Err_msg_1 = ex.Message.Replace(@"""", @"\""");
                string Err_msg = Err_msg_1.Replace(@"'", @"\""");
                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ1\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        // API to Update Config
        public class Input_Param_Update_Config
        {
            public string Org_Id { get; set; }
            public string Config_String { get; set; }
            public string LastEdited_By { get; set; }
            public string Method_Name { get; set; }
        }

        [HttpPost("Update_Config", Name = "Update_Config")]
        public IActionResult Update_Config([FromBody] Input_Param_Update_Config Application_Data)
        {
            try
            {
                if (Application_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Application_Data.Org_Id;
                var Application_Id = "";
                var Application_Name = "";
                int Is_Deleted = 0;
                var LastEdited_By = Application_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();
                int Is_Active = 1;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = Application_Data.Method_Name;
                var Config_String = Application_Data.Config_String;

                using (var context = new User_RepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Config_Application ({Method_Name},{Org_Id},{Application_Id},{Application_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By},{Config_String});").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        if (Return_Master_List[0].Result_Id == 1)
                        {
                            // Generate Output String 
                            var Return_Master = new Return_Master();
                            Return_Master.Res_Output = "{\"Response\": \"OK\"}";
                            return Ok(Return_Master);
                        }
                        else
                        {
                            // Generate Output String
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ3\", \"Error_Msg\": \"Record not saved\"}";
                            return StatusCode(500, Return_Error);
                        }

                    }
                    else
                    {
                        // Generate Output String
                        var Return_Error = new Return_Master();
                        Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"No record found\"}";
                        return StatusCode(500, Return_Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                string Err_msg_1 = ex.Message.Replace(@"""", @"\""");
                string Err_msg = Err_msg_1.Replace(@"'", @"\""");
                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }


        #endregion

        #region Role
        // Role related API
        // API to Read All Role
        public class Input_Param_Role_Read_All
        {
            public string Org_Id { get; set; }
            public string Application_Id { get; set; }
            public string Role_Name { get; set; }
        }

        [HttpPost("Read_All_Role", Name = "Read_All_Role")]
        public IActionResult Read_All_Role([FromBody] Input_Param_Role_Read_All Role_Data)
        {
            try
            {
                if (Role_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Role_Data.Org_Id;
                var Application_Id = Role_Data.Application_Id;
                var Role_Name = Role_Data.Role_Name;
                var Role_Id = "";
                var Method_Name = "Get";

                using (var context = new User_RepositoryContext())
                {
                    var Return_Master_List = context.Role_List.FromSqlInterpolated($"CALL dbo.USP_Master_Role ({Method_Name},{Org_Id},{Application_Id},{Role_Id},{Role_Name},{1},{0},{null},{""},{null},{""},{""});").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        var Return_Master_String = JsonConvert.SerializeObject(Return_Master_List);

                        // Generate Output String 
                        var Return_Master = new Return_Master();
                        Return_Master.Res_Output = "{\"Response\": \"OK\",\"Response_Data\": " + Return_Master_String + "}";
                        return Ok(Return_Master);
                    }
                    else
                    {
                        // Generate Output String
                        var Return_Master = new Return_Master();
                        Return_Master.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ0\", \"Error_Msg\": \"No record found\"}";
                        return Ok(Return_Master);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                string Err_msg = Get_Normalised_Message(ex.Message);
                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ1\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        // API to Create Role
        public class Input_Param_Role_Create
        {
            public string Org_Id { get; set; }
            public string Application_Id { get; set; }
            public string Role_Name { get; set; }
            public int Is_Active { get; set; }
            public string Created_By { get; set; }
        }

        [HttpPost("Create_Role", Name = "Create_Role")]
        public IActionResult Create_Role([FromBody] Input_Param_Role_Create Role_Data)
        {
            try
            {
                if (Role_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Role_Data.Org_Id;
                var Application_Id = Role_Data.Application_Id;
                var Role_Name = Role_Data.Role_Name.Trim();
                var Created_By = Role_Data.Created_By;
                var Created_On = Get_Current_DateTime();
                var Role_Id = "";
                int Is_Active = Role_Data.Is_Active;
                int Is_Deleted = 0;
                var Method_Name = "Create";

                using (var context = new User_RepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Master_Role ({Method_Name},{Org_Id},{Application_Id},{Role_Id},{Role_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{null},{""},{""});").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        if (Return_Master_List[0].Result_Id == 1)
                        {
                            // Generate Output String 
                            var Return_Master = new Return_Master();
                            Return_Master.Res_Output = "{\"Response\": \"OK\"}";
                            return Ok(Return_Master);
                        }
                        else if (Return_Master_List[0].Result_Id == -1)
                        {
                            // Generate Output String 
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ2\", \"Error_Msg\": \"Duplicate record exists\"}";
                            return StatusCode(500, Return_Error);
                        }
                        else
                        {
                            // Generate Output String
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ3\", \"Error_Msg\": \"Record not saved\"}";
                            return StatusCode(500, Return_Error);
                        }

                    }
                    else
                    {
                        // Generate Output String
                        var Return_Error = new Return_Master();
                        Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"No record found\"}";
                        return StatusCode(500, Return_Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                string Err_msg = Get_Normalised_Message(ex.Message);
                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        // API to Read One Role
        public class Input_Param_Role_Read_One
        {
            public string Org_Id { get; set; }
            public string Application_Id { get; set; }
            public string Role_Id { get; set; }
        }

        [HttpPost("Read_One_Role", Name = "Read_One_Role")]
        public IActionResult Read_One_Role([FromBody] Input_Param_Role_Read_One Role_Data)
        {
            try
            {
                if (Role_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Role_Data.Org_Id;
                var Application_Id = Role_Data.Application_Id;
                var Role_Id = Role_Data.Role_Id;
                var Role_Name = "";
                var Method_Name = "Get_One";

                using (var context = new User_RepositoryContext())
                {
                    var Return_Master_List = context.Role_List.FromSqlInterpolated($"CALL dbo.USP_Master_Role ({Method_Name},{Org_Id},{Application_Id},{Role_Id},{Role_Name},{1},{0},{null},{""},{null},{""},{""});").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        var Return_Master_String = JsonConvert.SerializeObject(Return_Master_List);

                        // Generate Output String 
                        var Return_Master = new Return_Master();
                        Return_Master.Res_Output = "{\"Response\": \"OK\",\"Response_Data\": " + Return_Master_String + "}";
                        return Ok(Return_Master);
                    }
                    else
                    {
                        // Generate Output String
                        var Return_Error = new Return_Master();
                        Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ0\", \"Error_Msg\": \"No record found\"}";
                        return StatusCode(500, Return_Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                string Err_msg = Get_Normalised_Message(ex.Message);

                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ1\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        // API to Update Role
        public class Input_Param_Role_Update
        {
            public string Org_Id { get; set; }
            public string Application_Id { get; set; }
            public string Role_Id { get; set; }
            public string Role_Name { get; set; }
            public int Is_Active { get; set; }
            public string LastEdited_By { get; set; }
            public string Menu_List { get; set; }
        }

        [HttpPost("Update_Role", Name = "Update_Role")]
        public IActionResult Update_Role([FromBody] Input_Param_Role_Update Role_Data)
        {
            try
            {
                if (Role_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Role_Data.Org_Id;
                var Application_Id = Role_Data.Application_Id;
                var Role_Id = Role_Data.Role_Id;
                var Role_Name = Role_Data.Role_Name.Trim();
                int Is_Active = Role_Data.Is_Active;
                var LastEdited_By = Role_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();
                var Menu_List = Role_Data.Menu_List;

                int Is_Deleted = 0;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = "Update";

                using (var context = new User_RepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Master_Role ({Method_Name},{Org_Id},{Application_Id},{Role_Id},{Role_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By},{Menu_List} ;").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        if (Return_Master_List[0].Result_Id == 1)
                        {
                            // Generate Output String 
                            var Return_Master = new Return_Master();
                            Return_Master.Res_Output = "{\"Response\": \"OK\"}";
                            return Ok(Return_Master);
                        }
                        else if (Return_Master_List[0].Result_Id == -1)
                        {
                            // Generate Output String 
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ2\", \"Error_Msg\": \"Duplicate record exists\"}";
                            return StatusCode(500, Return_Error);
                        }
                        else
                        {
                            // Generate Output String
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ3\", \"Error_Msg\": \"Record not saved\"}";
                            return StatusCode(500, Return_Error);
                        }

                    }
                    else
                    {
                        // Generate Output String
                        var Return_Error = new Return_Master();
                        Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"No record found\"}";
                        return StatusCode(500, Return_Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                string Err_msg = Get_Normalised_Message(ex.Message);
                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        // API to Delete Role
        public class Input_Param_Role_Delete
        {
            public string Org_Id { get; set; }
            public string Application_Id { get; set; }
            public string Role_Id { get; set; }
            public int Is_Deleted { get; set; }
            public string LastEdited_By { get; set; }
        }

        [HttpPost("Delete_Role", Name = "Delete_Role")]
        public IActionResult Delete_Role([FromBody] Input_Param_Role_Delete Role_Data)
        {
            try
            {
                if (Role_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Role_Data.Org_Id;
                var Application_Id = Role_Data.Application_Id;
                var Role_Id = Role_Data.Role_Id;
                var Role_Name = "";
                int Is_Active = 0;
                var LastEdited_By = Role_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();
                int Is_Deleted = Role_Data.Is_Deleted;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = "Delete";

                using (var context = new User_RepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Master_Role ({Method_Name},{Org_Id},{Application_Id},{Role_Id},{Role_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By},{""} ;").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        if (Return_Master_List[0].Result_Id == 1)
                        {
                            // Generate Output String 
                            var Return_Master = new Return_Master();
                            Return_Master.Res_Output = "{\"Response\": \"OK\"}";
                            return Ok(Return_Master);
                        }
                        else if (Return_Master_List[0].Result_Id == -1)
                        {
                            // Generate Output String 
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ2\", \"Error_Msg\": \"Entry in use.  Can't block\"}";
                            return StatusCode(500, Return_Error);
                        }
                        else
                        {
                            // Generate Output String
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ3\", \"Error_Msg\": \"Record not blocked\"}";
                            return StatusCode(500, Return_Error);
                        }

                    }
                    else
                    {
                        // Generate Output String
                        var Return_Error = new Return_Master();
                        Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"No record found\"}";
                        return StatusCode(500, Return_Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                string Err_msg = Get_Normalised_Message(ex.Message);
                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        // API to Read Menu for Current Role

        [HttpPost("Read_One_Role_Menu", Name = "Read_One_Role_Menu")]
        public IActionResult Read_One_Role_Menu([FromBody] Input_Param_Role_Read_One Role_Data)
        {
            try
            {
                if (Role_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Role_Data.Org_Id;
                var Application_Id = Role_Data.Application_Id;
                var Role_Id = Role_Data.Role_Id;
                var Role_Name = "";
                var Method_Name = "Get_One_Role_Menu";

                using (var context = new User_RepositoryContext())
                {
                    var Return_Master_List = context.Role_Menu_List.FromSqlInterpolated($"CALL dbo.USP_Master_Role ({Method_Name},{Org_Id},{Application_Id},{Role_Id},{Role_Name},{1},{0},{null},{""},{null},{""},{""});").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        var Return_Master_String = JsonConvert.SerializeObject(Return_Master_List);

                        // Generate Output String 
                        var Return_Master = new Return_Master();
                        Return_Master.Res_Output = "{\"Response\": \"OK\",\"Response_Data\": " + Return_Master_String + "}";
                        return Ok(Return_Master);
                    }
                    else
                    {
                        // Generate Output String
                        var Return_Error = new Return_Master();
                        Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ0\", \"Error_Msg\": \"No record found\"}";
                        return StatusCode(500, Return_Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                string Err_msg = Get_Normalised_Message(ex.Message);

                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ1\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        #endregion

        #region User
        // User related API
        // API to Read All User
        public class Input_Param_User_Read_All
        {
            public string Org_Id { get; set; }
            public string Application_Id { get; set; }
            public string UserName_DisplayName { get; set; }
            public string User_Name { get; set; }
            public string Method_Name { get; set; }
        }

        [HttpPost("Read_All_User", Name = "Read_All_User")]
        public IActionResult Read_All_User([FromBody] Input_Param_User_Read_All User_Data)
        {
            try
            {
                if (User_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = User_Data.Org_Id;
                var Application_Id = User_Data.Application_Id;
                var User_Name = User_Data.User_Name;
                var UserName_DisplayName = User_Data.UserName_DisplayName;
                var User_Id = "";
                var Method_Name = User_Data.Method_Name; // Get, Get_By_StaffType

                using (var context = new User_RepositoryContext())
                {
                    var Return_Master_List = context.User_List.FromSqlInterpolated($"CALL dbo.USP_Master_User ({Method_Name},{Org_Id},{Application_Id},{User_Id},{User_Name},{1},{0},{null},{""},{null},{""},{""},{""},{""},{""},{""},{""},{""},{""},{""},{""},{UserName_DisplayName});").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        var Return_Master_String = JsonConvert.SerializeObject(Return_Master_List);

                        // Generate Output String 
                        var Return_Master = new Return_Master();
                        Return_Master.Res_Output = "{\"Response\": \"OK\",\"Response_Data\": " + Return_Master_String + "}";
                        return Ok(Return_Master);
                    }
                    else
                    {
                        // Generate Output String
                        var Return_Master = new Return_Master();
                        Return_Master.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ0\", \"Error_Msg\": \"No record found\"}";
                        return Ok(Return_Master);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                string Err_msg = Get_Normalised_Message(ex.Message);
                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ1\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        // API to Create User
        public class Input_Param_User_Create
        {
            public string Org_Id { get; set; }
            public string Application_Id { get; set; }
            public string User_Name { get; set; }
            public string Password { get; set; }
            public string Display_Name { get; set; }
            public string Role_Id { get; set; }
            public int Is_Active { get; set; }
            public string Created_By { get; set; }
            public string SignUpOption_Id { get; set; }
            public string User_Extra_Details { get; set; }
            public string StaffType_Id { get; set; }
            public string AccessCode_Id { get; set; }
            public string Allowed_IP_Address { get; set; }
            public string Allow_Multi_Login { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
        }

        [HttpPost("Create_User", Name = "Create_User")]
        public IActionResult Create_User([FromBody] Input_Param_User_Create User_Data)
        {
            try
            {
                if (User_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = User_Data.Org_Id;
                var Application_Id = User_Data.Application_Id;
                var User_Name = User_Data.User_Name.Trim();
                var Password = User_Data.Password;
                var Display_Name = User_Data.Display_Name.Trim();
                var Role_Id = User_Data.Role_Id;
                var Created_By = User_Data.Created_By;
                var Created_On = Get_Current_DateTime();
                var User_Id = "";
                int Is_Active = User_Data.Is_Active;
                int Is_Deleted = 0;
                var Method_Name = "Create";
                var LastEdited_By = "";
                var LastEdited_On = "";
                var SignUpOption_Id = User_Data.SignUpOption_Id;
                var User_Extra_Details = User_Data.User_Extra_Details;
                var StaffType_Id = User_Data.StaffType_Id;
                var AccessCode_Id = User_Data.AccessCode_Id;
                var Allowed_IP_Address = User_Data.Allowed_IP_Address;
                var Allow_Multi_Login = User_Data.Allow_Multi_Login;
                var Email = User_Data.Email;
                var Phone = User_Data.Phone;

                using (var context = new User_RepositoryContext())
                {
                    var Return_Master_List = context.New_User_List.FromSqlInterpolated($"CALL dbo.USP_Master_User ({Method_Name},{Org_Id},{Application_Id},{User_Id},{User_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By},{Role_Id},{Password},{Display_Name},{SignUpOption_Id},{User_Extra_Details},{StaffType_Id},{AccessCode_Id},{Allowed_IP_Address},{Email},{Phone},{""},{""}, {Allow_Multi_Login});").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        if (Return_Master_List[0].Result_Id == 1)
                        {
                            // Generate Output String 
                            var Return_Master = new Return_Master();
                            Return_Master.Res_Output = "{\"Response\": \"OK\", \"User_Id\": \"" + Return_Master_List[0].Result_Extra_Key + "\"}";
                            return Ok(Return_Master);
                        }
                        else if (Return_Master_List[0].Result_Id == -1)
                        {
                            // Generate Output String 
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ2\", \"Error_Msg\": \"Duplicate record exists\"}";
                            return StatusCode(409, Return_Error);
                        }
                        else
                        {
                            // Generate Output String
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ3\", \"Error_Msg\": \"Record not saved\"}";
                            return StatusCode(500, Return_Error);
                        }

                    }
                    else
                    {
                        // Generate Output String
                        var Return_Error = new Return_Master();
                        Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"No record found\"}";
                        return StatusCode(500, Return_Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                string Err_msg = Get_Normalised_Message(ex.Message);
                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        // API to Read One User
        public class Input_Param_User_Read_One
        {
            public string Org_Id { get; set; }
            public string Application_Id { get; set; }
            public string User_Id { get; set; }
        }

        [HttpPost("Read_One_User", Name = "Read_One_User")]
        public IActionResult Read_One_User([FromBody] Input_Param_User_Read_One User_Data)
        {
            try
            {
                if (User_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = User_Data.Org_Id;
                var Application_Id = User_Data.Application_Id;
                var User_Id = User_Data.User_Id;
                var User_Name = "";
                var Email = "";
                var Phone = "";
                var Method_Name = "Get_One";

                using (var context = new User_RepositoryContext())
                {
                    var Return_Master_List = context.User_List.FromSqlInterpolated($"CALL dbo.USP_Master_User ({Method_Name},{Org_Id},{Application_Id},{User_Id},{User_Name},{1},{0},{null},{""},{null},{""},{""},{""},{""},{""},{""},{""},{""},{""},{""},{""});").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        var Return_Master_String = JsonConvert.SerializeObject(Return_Master_List);

                        // Generate Output String 
                        var Return_Master = new Return_Master();
                        Return_Master.Res_Output = "{\"Response\": \"OK\",\"Response_Data\": " + Return_Master_String + "}";
                        return Ok(Return_Master);
                    }
                    else
                    {
                        // Generate Output String
                        var Return_Error = new Return_Master();
                        Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ0\", \"Error_Msg\": \"No record found\"}";
                        return StatusCode(500, Return_Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                string Err_msg = Get_Normalised_Message(ex.Message);

                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ1\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        // API to Read One User Application Role
        public class Input_Param_User_Application_Role_Read_One
        {
            public string Org_Id { get; set; }
            public string User_Id { get; set; }
        }

        [HttpPost("Read_One_User_Application_Role", Name = "Read_One_User_Application_Role")]
        public IActionResult Read_One_User_Application_Role([FromBody] Input_Param_User_Application_Role_Read_One User_Data)
        {
            try
            {
                if (User_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = User_Data.Org_Id;
                var Application_Id = "";
                var User_Id = User_Data.User_Id;
                var User_Name = "";
                var Method_Name = "Get_One_User_App_Role";

                using (var context = new User_RepositoryContext())
                {
                    var Return_Master_List = context.User_Application_Role_List.FromSqlInterpolated($"CALL dbo.USP_Master_User ({Method_Name},{Org_Id},{Application_Id},{User_Id},{User_Name},{1},{0},{null},{""},{null},{""},{""},{""},{""},{""},{""},{""},{""},{""});").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        var Return_Master_String = JsonConvert.SerializeObject(Return_Master_List);

                        // Generate Output String 
                        var Return_Master = new Return_Master();
                        Return_Master.Res_Output = "{\"Response\": \"OK\",\"Response_Data\": " + Return_Master_String + "}";
                        return Ok(Return_Master);
                    }
                    else
                    {
                        // Generate Output String
                        var Return_Error = new Return_Master();
                        Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ0\", \"Error_Msg\": \"No record found\"}";
                        return StatusCode(500, Return_Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                string Err_msg = Get_Normalised_Message(ex.Message);

                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ1\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        // API to Update User
        public class Input_Param_User_Update
        {
            public string Org_Id { get; set; }
            public string Application_Id { get; set; }
            public string User_Id { get; set; }
            public string Display_Name { get; set; }
            public string Role_Id { get; set; }
            public string User_Name { get; set; }
            public int Is_Active { get; set; }
            public string LastEdited_By { get; set; }
            public string SignUpOption_Id { get; set; }
            public string User_Extra_Details { get; set; }
            public string StaffType_Id { get; set; }
            public string AccessCode_Id { get; set; }
            public string Allowed_IP_Address { get; set; }
            public string Allow_Multi_Login { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
        }

        [HttpPost("Update_User", Name = "Update_User")]
        public IActionResult Update_User([FromBody] Input_Param_User_Update User_Data)
        {
            try
            {
                if (User_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = User_Data.Org_Id;
                var Application_Id = User_Data.Application_Id;
                var User_Id = User_Data.User_Id;
                var User_Name = User_Data.User_Name.Trim();
                int Is_Active = User_Data.Is_Active;
                var LastEdited_By = User_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();
                var Display_Name = User_Data.Display_Name.Trim();
                var SignUpOption_Id = User_Data.SignUpOption_Id;
                var User_Extra_Details = User_Data.User_Extra_Details;
                var StaffType_Id = User_Data.StaffType_Id;
                var Role_Id = User_Data.Role_Id;
                var AccessCode_Id = User_Data.AccessCode_Id;
                var Allowed_IP_Address = User_Data.Allowed_IP_Address;
                var Allow_Multi_Login = User_Data.Allow_Multi_Login;
                var Password = "";
                var Email = User_Data.Email;
                var Phone = User_Data.Phone;


                int Is_Deleted = 0;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = "Update";

                using (var context = new User_RepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Master_User ({Method_Name},{Org_Id},{Application_Id},{User_Id},{User_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By},{Role_Id},{Password},{Display_Name},{SignUpOption_Id},{User_Extra_Details},{StaffType_Id},{AccessCode_Id},{Allowed_IP_Address},{Email},{Phone},{""},{""},{Allow_Multi_Login});").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        if (Return_Master_List[0].Result_Id == 1)
                        {
                            // Generate Output String 
                            var Return_Master = new Return_Master();
                            Return_Master.Res_Output = "{\"Response\": \"OK\"}";
                            return Ok(Return_Master);
                        }
                        else if (Return_Master_List[0].Result_Id == -1)
                        {
                            // Generate Output String 
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ2\", \"Error_Msg\": \"Duplicate record exists\"}";
                            return StatusCode(409, Return_Error);
                        }
                        else
                        {
                            // Generate Output String
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ3\", \"Error_Msg\": \"Record not saved\"}";
                            return StatusCode(500, Return_Error);
                        }

                    }
                    else
                    {
                        // Generate Output String
                        var Return_Error = new Return_Master();
                        Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"No record found\"}";
                        return StatusCode(500, Return_Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                string Err_msg = Get_Normalised_Message(ex.Message);
                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        // API to Delete User
        public class Input_Param_User_Delete
        {
            public string Org_Id { get; set; }
            public string Application_Id { get; set; }
            public string User_Id { get; set; }
            public int Is_Deleted { get; set; }
            public string LastEdited_By { get; set; }
        }

        [HttpPost("Delete_User", Name = "Delete_User")]
        public IActionResult Delete_User([FromBody] Input_Param_User_Delete User_Data)
        {
            try
            {
                if (User_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = User_Data.Org_Id;
                var Application_Id = User_Data.Application_Id;
                var User_Id = User_Data.User_Id;
                var User_Name = "";
                int Is_Active = 0;
                var LastEdited_By = User_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();
                int Is_Deleted = User_Data.Is_Deleted;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = "Delete";

                using (var context = new User_RepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Master_User ({Method_Name},{Org_Id},{Application_Id},{User_Id},{User_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By},{""},{""},{""},{""},{""},{""},{""},{""} ;").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        if (Return_Master_List[0].Result_Id == 1)
                        {
                            // Generate Output String 
                            var Return_Master = new Return_Master();
                            Return_Master.Res_Output = "{\"Response\": \"OK\"}";
                            return Ok(Return_Master);
                        }
                        else if (Return_Master_List[0].Result_Id == -1)
                        {
                            // Generate Output String 
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ2\", \"Error_Msg\": \"" + Return_Master_List[0].Result_Extra_Key + "\"}";
                            return StatusCode(500, Return_Error);
                        }
                        else
                        {
                            // Generate Output String
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ3\", \"Error_Msg\": \"Record not blocked\"}";
                            return StatusCode(500, Return_Error);
                        }

                    }
                    else
                    {
                        // Generate Output String
                        var Return_Error = new Return_Master();
                        Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"No record found\"}";
                        return StatusCode(500, Return_Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                string Err_msg = Get_Normalised_Message(ex.Message);
                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        // API to Reset User Password
        public class Input_Param_Reset_User_Password
        {
            public string Org_Id { get; set; }
            public string User_Id { get; set; }
            public string User_Name { get; set; }
            public string User_Password { get; set; }
            public string LastEdited_By { get; set; }
        }

        [HttpPost("Reset_User_Password", Name = "Reset_User_Password")]
        public IActionResult Reset_User_Password([FromBody] Input_Param_Reset_User_Password User_Data)
        {
            try
            {
                if (User_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = User_Data.Org_Id;
                var Application_Id = "";
                var User_Id = User_Data.User_Id;
                var User_Name = User_Data.User_Name;
                int Is_Active = 1;
                var LastEdited_By = User_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();
                var Display_Name = "";
                var Role_Id = "";
                var Password = User_Data.User_Password;

                int Is_Deleted = 0;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = "Reset_Password";

                using (var context = new User_RepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Master_User ({Method_Name},{Org_Id},{Application_Id},{User_Id},{User_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By},{Role_Id},{Password},{Display_Name},{""},{""},{""},{""},{""});").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        if (Return_Master_List[0].Result_Id == 1)
                        {
                            // Generate Output String 
                            var Return_Master = new Return_Master();
                            Return_Master.Res_Output = "{\"Response\": \"OK\"}";
                            return Ok(Return_Master);
                        }
                        else if (Return_Master_List[0].Result_Id == -1)
                        {
                            // Generate Output String 
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ2\", \"Error_Msg\": \"Duplicate record exists\"}";
                            return StatusCode(500, Return_Error);
                        }
                        else
                        {
                            // Generate Output String
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ3\", \"Error_Msg\": \"Record not saved\"}";
                            return StatusCode(500, Return_Error);
                        }

                    }
                    else
                    {
                        // Generate Output String
                        var Return_Error = new Return_Master();
                        Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"No record found\"}";
                        return StatusCode(500, Return_Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                string Err_msg = Get_Normalised_Message(ex.Message);
                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        // API to Update User_Role
        public class Input_Param_User_Role_Update
        {
            public string Org_Id { get; set; }
            public string Application_Id { get; set; }
            public string User_Id { get; set; }
            public string Role_Id { get; set; }
            public string LastEdited_By { get; set; }
            public string Method_Name { get; set; }
        }

        [HttpPost("Update_User_Role", Name = "Update_User_Role")]
        public IActionResult Update_User_Role([FromBody] Input_Param_User_Role_Update User_Data)
        {
            try
            {
                if (User_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = User_Data.Org_Id;
                var Application_Id = User_Data.Application_Id;
                var User_Id = User_Data.User_Id;
                var User_Name = "";
                int Is_Active = 1;
                var LastEdited_By = User_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();
                var Display_Name = "";
                var SignUpOption_Id = "";
                var User_Extra_Details = "";
                var StaffType_Id = "";
                var Role_Id = User_Data.Role_Id;
                var Password = "";

                int Is_Deleted = 0;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = User_Data.Method_Name;

                using (var context = new User_RepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Master_User ({Method_Name},{Org_Id},{Application_Id},{User_Id},{User_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By},{Role_Id},{Password},{Display_Name},{SignUpOption_Id},{User_Extra_Details},{StaffType_Id},{""},{""});").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        if (Return_Master_List[0].Result_Id == 1)
                        {
                            // Generate Output String 
                            var Return_Master = new Return_Master();
                            Return_Master.Res_Output = "{\"Response\": \"OK\"}";
                            return Ok(Return_Master);
                        }
                        else if (Return_Master_List[0].Result_Id == -1)
                        {
                            // Generate Output String 
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ2\", \"Error_Msg\": \"Duplicate record exists\"}";
                            return StatusCode(500, Return_Error);
                        }
                        else
                        {
                            // Generate Output String
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ3\", \"Error_Msg\": \"Record not saved\"}";
                            return StatusCode(500, Return_Error);
                        }

                    }
                    else
                    {
                        // Generate Output String
                        var Return_Error = new Return_Master();
                        Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"No record found\"}";
                        return StatusCode(500, Return_Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                string Err_msg = Get_Normalised_Message(ex.Message);
                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        public class Input_Param_User_Validate_Email_Mobile
        {
            public string Org_Id { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Method_Name { get; set; }
        }
        [HttpPost("Validate_Email_Phone", Name = "Validate_Email_Phone")]
        public IActionResult Validate_Email_Phone([FromBody] Input_Param_User_Validate_Email_Mobile User_Data)
        {
            try
            {
                if (User_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = User_Data.Org_Id;
                var Application_Id = "";
                var User_Id = "";
                var User_Name = "";
                int Is_Active = 1;
                var LastEdited_By = "";
                var LastEdited_On = Get_Current_DateTime();
                var Display_Name = "";
                var SignUpOption_Id = "";
                var User_Extra_Details = "";
                var StaffType_Id = "";
                var Role_Id = "";
                var Password = "";

                int Is_Deleted = 0;
                var Created_By = "";
                var Created_On = "";
                var Email = User_Data.Email;
                var Phone = User_Data.Phone;
                var Method_Name = User_Data.Method_Name;

                using (var context = new User_RepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Master_User ({Method_Name},{Org_Id},{Application_Id},{User_Id},{User_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By},{Role_Id},{Password},{Display_Name},{SignUpOption_Id},{User_Extra_Details},{StaffType_Id},{""},{""},{Email},{Phone});").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        if (Return_Master_List[0].Result_Id == 1)
                        {
                            // Generate Output String 
                            var Return_Master = new Return_Master();
                            Return_Master.Res_Output = "{\"Response\": \"OK\"}";
                            return Ok(Return_Master);
                        }
                        else if (Return_Master_List[0].Result_Id == -1)
                        {
                            // Generate Output String 
                            var Error_Output = Return_Master_List[0].Result_Description;
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ2\", \"Error_Msg\": \"" + Error_Output + "\"}";
                            return StatusCode(500, Return_Error);
                        }
                        else
                        {
                            // Generate Output String
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ3\", \"Error_Msg\": \"Record not saved\"}";
                            return StatusCode(500, Return_Error);
                        }

                    }
                    else
                    {
                        // Generate Output String
                        var Return_Error = new Return_Master();
                        Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"No record found\"}";
                        return StatusCode(500, Return_Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                string Err_msg = Get_Normalised_Message(ex.Message);
                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        #endregion

        #region Menu_Access
        // API to Insert Log Menu Access
        public class Input_Param_Insert_Log_Menu_Access
        {
            public string APPid { get; set; }
            public string UID { get; set; }
            public string Menuid { get; set; }
            public string IP { get; set; }
            public string session { get; set; }
        }

        [HttpPost("Insert_Log_Menu_Access", Name = "Insert_Log_Menu_Access")]
        public IActionResult Insert_Log_Menu_Access([FromBody] Input_Param_Insert_Log_Menu_Access User_Data)
        {
            try
            {
                if (User_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }
                var APPid = User_Data.APPid;
                var UID = User_Data.UID;
                var Menuid = User_Data.Menuid;
                var IP = User_Data.IP;
                var session = User_Data.session;

                using (var context = new User_RepositoryContext())
                {
                    var Return_Master_List = context.New_User_List.FromSqlInterpolated($"CALL dbo.UserMenuLog ({UID},{APPid},{Menuid},{IP},{session});").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        if (Return_Master_List[0].Result_Id == 1)
                        {
                            // Generate Output String 
                            var Return_Master = new Return_Master();
                            Return_Master.Res_Output = "{\"Response\": \"OK\", \"User_Id\": \"" + Return_Master_List[0].Result_Extra_Key + "\"}";
                            return Ok(Return_Master);
                        }
                        else if (Return_Master_List[0].Result_Id == -1)
                        {
                            // Generate Output String 
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ2\", \"Error_Msg\": \"Duplicate record exists\"}";
                            return StatusCode(409, Return_Error);
                        }
                        else
                        {
                            // Generate Output String
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ3\", \"Error_Msg\": \"Record not saved\"}";
                            return StatusCode(500, Return_Error);
                        }

                    }
                    else
                    {
                        // Generate Output String
                        var Return_Error = new Return_Master();
                        Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"No record found\"}";
                        return StatusCode(500, Return_Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User Authentication  : {ex.Message}");

                string Err_msg = Get_Normalised_Message(ex.Message);
                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }
        #endregion
    }
}
