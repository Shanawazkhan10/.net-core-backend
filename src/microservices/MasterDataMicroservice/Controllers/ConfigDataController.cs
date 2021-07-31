using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Middleware;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using MasterDataMicroservice.Repository;
using MasterDataMicroservice.Entities;


namespace MasterDataMicroservice.Controllers
{
    [Route("v1/api/master/config/")]
    [ApiController]

    public class ConfigDataController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IMasterRepository _userRepository;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IEncryptor _encryptor;

        public ConfigDataController(ILoggerManager logger, IMasterRepository userRepository, IJwtBuilder jwtBuilder, IEncryptor encryptor)
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

        #region Config_Title
        // Config_Title related API
        // API to Read All Config_Title
        public class Input_Param_Config_Title
        {
            public string Org_Id { get; set; }
            public string Config_Title_Name { get; set; }
        }

        [HttpPost("Read_Config_Title", Name = "Read_Config_Title")]
        public IActionResult Read_Config_Title([FromBody] Input_Param_Config_Title Config_Title_Data)
        {
            try
            {
                if (Config_Title_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Config_Title_Data.Org_Id;
                var Config_Title_Name = Config_Title_Data.Config_Title_Name;
                var Config_Title_Id = "";
                var Method_Name = "Get";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Config_Title.FromSqlInterpolated($"CALL dbo.USP_Config_Title ({Method_Name},{Org_Id},{Config_Title_Id},{Config_Title_Name},{1},{0},{null},{""},{null},{""});").ToList();
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

        [HttpPost("Read_All_Title", Name = "Read_All_Title")]
        public IActionResult Read_All_Title([FromBody] Input_Param_Config_Title Config_Title_Data)
        {
            try
            {
                if (Config_Title_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Config_Title_Data.Org_Id;
                var Config_Title_Name = Config_Title_Data.Config_Title_Name;
                var Config_Title_Id = "";
                var Method_Name = "Get_All";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Config_Title.FromSqlInterpolated($"CALL dbo.USP_Config_Title ({Method_Name},{Org_Id},{Config_Title_Id},{Config_Title_Name},{1},{0},{null},{""},{null},{""});").ToList();
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

        // API to Add or Remove Title
        public class Input_Param_Add_Remove_Title
        {
            public string Org_Id { get; set; }
            public string Title_Id { get; set; }
            public int Is_Active { get; set; }
            public string LastEdited_By { get; set; }
        }

        [HttpPost("Add_Remove_Title", Name = "Add_Remove_Title")]
        public IActionResult Add_Remove_Title([FromBody] Input_Param_Add_Remove_Title Title_Data)
        {
            try
            {
                if (Title_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Title_Data.Org_Id;
                var Title_Id = Title_Data.Title_Id;
                var Title_Name = "";
                int Is_Deleted = 0;
                var LastEdited_By = Title_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();
                int Is_Active = Title_Data.Is_Active;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = "";
                if (Title_Data.Is_Active == 1)
                {
                    Method_Name = "Create";
                }
                else
                {
                    Method_Name = "Delete";
                }


                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Config_Title ({Method_Name},{Org_Id},{Title_Id},{Title_Name},{Is_Active},{Is_Deleted},{LastEdited_On},{LastEdited_By},{LastEdited_On},{LastEdited_By});").ToList();
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

        // API to Read One Title
        public class Input_Param_Title_Read_One
        {
            public string Org_Id { get; set; }
            public string Title_Id { get; set; }
        }

        [HttpPost("Read_One_Title", Name = "Read_One_Title")]
        public IActionResult Read_One_Title([FromBody] Input_Param_Title_Read_One Title_Data)
        {
            try
            {
                if (Title_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Title_Data.Org_Id;
                var Title_Id = Title_Data.Title_Id;
                var Title_Name = "";
                var Method_Name = "Get_One";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Config_Title.FromSqlInterpolated($"CALL dbo.USP_Config_Title ({Method_Name},{Org_Id},{Title_Id},{Title_Name},{1},{0},{null},{""},{null},{""});").ToList();
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

        // API to Update Title
        public class Input_Param_Title_Update
        {
            public string Org_Id { get; set; }
            public string Title_Id { get; set; }
            public string Title_Name { get; set; }
            public int Is_Active { get; set; }
            public string LastEdited_By { get; set; }
        }

        [HttpPost("Update_Title", Name = "Update_Title")]
        public IActionResult Update_Title([FromBody] Input_Param_Title_Update Title_Data)
        {
            try
            {
                if (Title_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Title_Data.Org_Id;
                var Title_Id = Title_Data.Title_Id;
                var Title_Name = Title_Data.Title_Name.Trim();
                int Is_Active = Title_Data.Is_Active;
                var LastEdited_By = Title_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();


                int Is_Deleted = 0;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = "Update";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Config_Title ({Method_Name},{Org_Id},{Title_Id},{Title_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By});").ToList();
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

                string Err_msg_1 = ex.Message.Replace(@"""", @"\""");
                string Err_msg = Err_msg_1.Replace(@"'", @"\""");
                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }
        #endregion

        #region Config_Gender
        // Config_Gender related API
        // API to Read All Config_Gender
        public class Input_Param_Config_Gender
        {
            public string Org_Id { get; set; }
            public string Config_Gender_Name { get; set; }
        }

        [HttpPost("Read_Config_Gender", Name = "Read_Config_Gender")]
        public IActionResult Read_Config_Gender([FromBody] Input_Param_Config_Gender Config_Gender_Data)
        {
            try
            {
                if (Config_Gender_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Config_Gender_Data.Org_Id;
                var Config_Gender_Name = Config_Gender_Data.Config_Gender_Name;
                var Config_Gender_Id = "";
                var Method_Name = "Get";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Config_Gender.FromSqlInterpolated($"CALL dbo.USP_Config_Gender ({Method_Name},{Org_Id},{Config_Gender_Id},{Config_Gender_Name},{1},{0},{null},{""},{null},{""});").ToList();
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

        [HttpPost("Read_All_Gender", Name = "Read_All_Gender")]
        public IActionResult Read_All_Gender([FromBody] Input_Param_Config_Gender Config_Gender_Data)
        {
            try
            {
                if (Config_Gender_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Config_Gender_Data.Org_Id;
                var Config_Gender_Name = Config_Gender_Data.Config_Gender_Name;
                var Config_Gender_Id = "";
                var Method_Name = "Get_All";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Config_Gender.FromSqlInterpolated($"CALL dbo.USP_Config_Gender ({Method_Name},{Org_Id},{Config_Gender_Id},{Config_Gender_Name},{1},{0},{null},{""},{null},{""});").ToList();
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

        // API to Add or Remove Gender
        public class Input_Param_Add_Remove_Gender
        {
            public string Org_Id { get; set; }
            public string Gender_Id { get; set; }
            public int Is_Active { get; set; }
            public string LastEdited_By { get; set; }
        }

        [HttpPost("Add_Remove_Gender", Name = "Add_Remove_Gender")]
        public IActionResult Add_Remove_Gender([FromBody] Input_Param_Add_Remove_Gender Gender_Data)
        {
            try
            {
                if (Gender_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Gender_Data.Org_Id;
                var Gender_Id = Gender_Data.Gender_Id;
                var Gender_Name = "";
                int Is_Deleted = 0;
                var LastEdited_By = Gender_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();
                int Is_Active = Gender_Data.Is_Active;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = "";
                if (Gender_Data.Is_Active == 1)
                {
                    Method_Name = "Create";
                }
                else
                {
                    Method_Name = "Delete";
                }


                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Config_Gender ({Method_Name},{Org_Id},{Gender_Id},{Gender_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By});").ToList();
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

        // API to Read One Gender
        public class Input_Param_Gender_Read_One
        {
            public string Org_Id { get; set; }
            public string Gender_Id { get; set; }
        }

        [HttpPost("Read_One_Gender", Name = "Read_One_Gender")]
        public IActionResult Read_One_Gender([FromBody] Input_Param_Gender_Read_One Gender_Data)
        {
            try
            {
                if (Gender_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Gender_Data.Org_Id;
                var Gender_Id = Gender_Data.Gender_Id;
                var Gender_Name = "";
                var Method_Name = "Get_One";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Config_Gender.FromSqlInterpolated($"CALL dbo.USP_Config_Gender ({Method_Name},{Org_Id},{Gender_Id},{Gender_Name},{1},{0},{null},{""},{null},{""});").ToList();
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

        // API to Update Gender
        public class Input_Param_Gender_Update
        {
            public string Org_Id { get; set; }
            public string Gender_Id { get; set; }
            public string Gender_Name { get; set; }
            public int Is_Active { get; set; }
            public string LastEdited_By { get; set; }
        }

        [HttpPost("Update_Gender", Name = "Update_Gender")]
        public IActionResult Update_Gender([FromBody] Input_Param_Gender_Update Gender_Data)
        {
            try
            {
                if (Gender_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Gender_Data.Org_Id;
                var Gender_Id = Gender_Data.Gender_Id;
                var Gender_Name = Gender_Data.Gender_Name.Trim();
                int Is_Active = Gender_Data.Is_Active;
                var LastEdited_By = Gender_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();


                int Is_Deleted = 0;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = "Update";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Config_Gender ({Method_Name},{Org_Id},{Gender_Id},{Gender_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By});").ToList();
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

        #endregion

        #region Config_StaffType
        // StaffType related API
        // API to Read All StaffType
        public class Input_Param_StaffType_Read_All
        {
            public string Org_Id { get; set; }
            public string StaffType_Name { get; set; }
        }

        [HttpPost("Read_All_StaffType", Name = "Read_All_StaffType")]
        public IActionResult Read_All_StaffType([FromBody] Input_Param_StaffType_Read_All StaffType_Data)
        {
            try
            {
                if (StaffType_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = StaffType_Data.Org_Id;
                var StaffType_Name = StaffType_Data.StaffType_Name;
                var StaffType_Id = "";
                var Method_Name = "Get_All";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Config_StaffType.FromSqlInterpolated($"CALL dbo.USP_Config_StaffType ({Method_Name},{Org_Id},{StaffType_Id},{StaffType_Name},{1},{0},{null},{""},{null},{""});").ToList();
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

        [HttpPost("Read_Config_StaffType", Name = "Read_Config_StaffType")]
        public IActionResult Read_Config_StaffType([FromBody] Input_Param_StaffType_Read_All Config_StaffType_Data)
        {
            try
            {
                if (Config_StaffType_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Config_StaffType_Data.Org_Id;
                var Config_StaffType_Name = Config_StaffType_Data.StaffType_Name;
                var Config_StaffType_Id = "";
                var Method_Name = "Get_All";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Config_StaffType.FromSqlInterpolated($"CALL dbo.USP_Config_StaffType ({Method_Name},{Org_Id},{Config_StaffType_Id},{Config_StaffType_Name},{1},{0},{null},{""},{null},{""});").ToList();
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

        // API to Add or Remove StaffType
        public class Input_Param_Add_Remove_StaffType
        {
            public string Org_Id { get; set; }
            public string StaffType_Id { get; set; }
            public int Is_Active { get; set; }
            public string LastEdited_By { get; set; }
        }

        [HttpPost("Add_Remove_StaffType", Name = "Add_Remove_StaffType")]
        public IActionResult Add_Remove_StaffType([FromBody] Input_Param_Add_Remove_StaffType StaffType_Data)
        {
            try
            {
                if (StaffType_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = StaffType_Data.Org_Id;
                var StaffType_Id = StaffType_Data.StaffType_Id;
                var StaffType_Name = "";
                int Is_Deleted = 0;
                var LastEdited_By = StaffType_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();
                int Is_Active = StaffType_Data.Is_Active;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = "";
                if (StaffType_Data.Is_Active == 1)
                {
                    Method_Name = "Create";
                }
                else
                {
                    Method_Name = "Delete";
                }


                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Config_StaffType ({Method_Name},{Org_Id},{StaffType_Id},{StaffType_Name},{Is_Active},{Is_Deleted},{LastEdited_On},{LastEdited_By},{LastEdited_On},{LastEdited_By});").ToList();
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

        // API to Read One StaffType
        public class Input_Param_StaffType_Read_One
        {
            public string Org_Id { get; set; }
            public string StaffType_Id { get; set; }
        }

        [HttpPost("Read_One_StaffType", Name = "Read_One_StaffType")]
        public IActionResult Read_One_StaffType([FromBody] Input_Param_StaffType_Read_One StaffType_Data)
        {
            try
            {
                if (StaffType_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = StaffType_Data.Org_Id;
                var StaffType_Id = StaffType_Data.StaffType_Id;
                var StaffType_Name = "";
                var Method_Name = "Get_One";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Config_StaffType.FromSqlInterpolated($"CALL dbo.USP_Config_StaffType ({Method_Name},{Org_Id},{StaffType_Id},{StaffType_Name},{1},{0},{null},{""},{null},{""});").ToList();
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

        // API to Update StaffType
        public class Input_Param_StaffType_Update
        {
            public string Org_Id { get; set; }
            public string StaffType_Id { get; set; }
            public string StaffType_Name { get; set; }
            public int Is_Active { get; set; }
            public string LastEdited_By { get; set; }
        }

        [HttpPost("Update_StaffType", Name = "Update_StaffType")]
        public IActionResult Update_StaffType([FromBody] Input_Param_StaffType_Update StaffType_Data)
        {
            try
            {
                if (StaffType_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = StaffType_Data.Org_Id;
                var StaffType_Id = StaffType_Data.StaffType_Id;
                var StaffType_Name = StaffType_Data.StaffType_Name.Trim();
                int Is_Active = StaffType_Data.Is_Active;
                var LastEdited_By = StaffType_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();


                int Is_Deleted = 0;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = "Update";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Config_StaffType ({Method_Name},{Org_Id},{StaffType_Id},{StaffType_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By} ;").ToList();
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

        #endregion

        #region Config_AccessCode
        // Config_AccessCode related API
        // API to Read All Config_AccessCode
        public class Input_Param_Config_AccessCode
        {
            public string Org_Id { get; set; }
            public string App_Id { get; set; }
            public string Config_AccessCode_Name { get; set; }
            public string StaffType_Name { get; set; }
        }

        [HttpPost("Read_Config_AccessCode", Name = "Read_Config_AccessCode")]
        public IActionResult Read_Config_AccessCode([FromBody] Input_Param_Config_AccessCode Config_AccessCode_Data)
        {
            try
            {
                if (Config_AccessCode_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Config_AccessCode_Data.Org_Id;
                var Config_AccessCode_Name = Config_AccessCode_Data.Config_AccessCode_Name;
                var Config_AccessCode_Id = "";
                var Method_Name = "Get";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Config_AccessCode.FromSqlInterpolated($"CALL dbo.USP_Config_AccessCode ({Method_Name},{Org_Id},{Config_AccessCode_Id},{Config_AccessCode_Name},{1},{0},{null},{""},{null},{""});").ToList();
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

        [HttpPost("Read_All_AccessCode", Name = "Read_All_AccessCode")]
        public IActionResult Read_All_AccessCode([FromBody] Input_Param_Config_AccessCode Config_AccessCode_Data)
        {
            try
            {
                if (Config_AccessCode_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Config_AccessCode_Data.Org_Id;
                var App_Id = Config_AccessCode_Data.App_Id;
                var Config_AccessCode_Name = Config_AccessCode_Data.Config_AccessCode_Name;
                var StaffType_Name = Config_AccessCode_Data.StaffType_Name;
                var Config_AccessCode_Id = "";
                var Method_Name = "Get_All";

                using (var context = new MasterRepositoryContext())
                {
                    //var Return_Master_List = context.Config_AccessCode.FromSqlInterpolated($"CALL dbo.USP_Config_AccessCode {Method_Name},{Org_Id},{Config_AccessCode_Id},{Config_AccessCode_Name},{1},{0},{null},{""},{null},{""});").ToList();
                    var Return_Master_List = context.Config_GetAccessCode.FromSqlInterpolated($"CALL dbo.GetAccessCode ({StaffType_Name},'AP02';").ToList();
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

        // API to Add or Remove AccessCode
        public class Input_Param_Add_Remove_AccessCode
        {
            public string Org_Id { get; set; }
            public string AccessCode_Id { get; set; }
            public int Is_Active { get; set; }
            public string LastEdited_By { get; set; }
        }

        [HttpPost("Add_Remove_AccessCode", Name = "Add_Remove_AccessCode")]
        public IActionResult Add_Remove_AccessCode([FromBody] Input_Param_Add_Remove_AccessCode AccessCode_Data)
        {
            try
            {
                if (AccessCode_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = AccessCode_Data.Org_Id;
                var AccessCode_Id = AccessCode_Data.AccessCode_Id;
                var AccessCode_Name = "";
                int Is_Deleted = 0;
                var LastEdited_By = AccessCode_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();
                int Is_Active = AccessCode_Data.Is_Active;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = "";
                if (AccessCode_Data.Is_Active == 1)
                {
                    Method_Name = "Create";
                }
                else
                {
                    Method_Name = "Delete";
                }


                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Config_AccessCode ({Method_Name},{Org_Id},{AccessCode_Id},{AccessCode_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By});").ToList();
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

        // API to Read One AccessCode
        public class Input_Param_AccessCode_Read_One
        {
            public string Org_Id { get; set; }
            public string AccessCode_Id { get; set; }
        }

        [HttpPost("Read_One_AccessCode", Name = "Read_One_AccessCode")]
        public IActionResult Read_One_AccessCode([FromBody] Input_Param_AccessCode_Read_One AccessCode_Data)
        {
            try
            {
                if (AccessCode_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = AccessCode_Data.Org_Id;
                var AccessCode_Id = AccessCode_Data.AccessCode_Id;
                var AccessCode_Name = "";
                var Method_Name = "Get_One";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Config_AccessCode.FromSqlInterpolated($"CALL dbo.USP_Config_AccessCode ({Method_Name},{Org_Id},{AccessCode_Id},{AccessCode_Name},{1},{0},{null},{""},{null},{""});").ToList();
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

        // API to Update AccessCode
        public class Input_Param_AccessCode_Update
        {
            public string Org_Id { get; set; }
            public string AccessCode_Id { get; set; }
            public string AccessCode_Name { get; set; }
            public int Is_Active { get; set; }
            public string LastEdited_By { get; set; }
        }

        [HttpPost("Update_AccessCode", Name = "Update_AccessCode")]
        public IActionResult Update_AccessCode([FromBody] Input_Param_AccessCode_Update AccessCode_Data)
        {
            try
            {
                if (AccessCode_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = AccessCode_Data.Org_Id;
                var AccessCode_Id = AccessCode_Data.AccessCode_Id;
                var AccessCode_Name = AccessCode_Data.AccessCode_Name.Trim();
                int Is_Active = AccessCode_Data.Is_Active;
                var LastEdited_By = AccessCode_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();


                int Is_Deleted = 0;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = "Update";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Config_AccessCode ({Method_Name},{Org_Id},{AccessCode_Id},{AccessCode_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By});").ToList();
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

        #endregion
    }
}
