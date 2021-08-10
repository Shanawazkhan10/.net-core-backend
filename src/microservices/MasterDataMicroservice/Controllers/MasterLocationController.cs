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
    [Route("v1/api/master/location/")]
    [ApiController]
    
    public class MasterLocationController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IMasterRepository _userRepository;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IEncryptor _encryptor;

        public MasterLocationController(ILoggerManager logger, IMasterRepository userRepository, IJwtBuilder jwtBuilder, IEncryptor encryptor)
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

        #region Country
        // Country related API
        // API to Read All Country
        public class Input_Param_Country_Read_All
        {
            public string Org_Id { get; set; }
            public string Country_Name { get; set; }
        }

        [HttpPost("Read_All_Country", Name = "Read_All_Country")]
        public IActionResult Read_All_Country([FromBody] Input_Param_Country_Read_All Country_Data)
        {
            try
            {
                if (Country_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Country_Data.Org_Id;
                var Country_Name = Country_Data.Country_Name;
                var Country_Id = "";
                var Method_Name = "Get";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Country_List.FromSqlInterpolated($"CALL dbo.USP_Master_Country ({Method_Name},{Org_Id},{Country_Id},{Country_Name},{1},{0},{null},{""},{null},{""});").ToList();
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

        // API to Create Country
        public class Input_Param_Country_Create
        {
            public string Org_Id { get; set; }
            public string Country_Name { get; set; }
            public int Is_Active { get; set; } 
            public string Created_By { get; set; }
        }

        [HttpPost("Create_Country", Name = "Create_Country")]
        public IActionResult Create_Country([FromBody] Input_Param_Country_Create Country_Data)
        {
            try
            {
                if (Country_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Country_Data.Org_Id;
                var Country_Name = Country_Data.Country_Name.Trim();
                var Created_By = Country_Data.Created_By;
                var Created_On = Get_Current_DateTime();
                var Country_Id = "";
                int Is_Active = Country_Data.Is_Active;
                int Is_Deleted = 0;
                var Method_Name = "Create";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Master_Country ({Method_Name},{Org_Id},{Country_Id},{Country_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{null},{""} ;").ToList();
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

        // API to Read One Country
        public class Input_Param_Country_Read_One
        {
            public string Org_Id { get; set; }
            public string Country_Id { get; set; }
        }

        [HttpPost("Read_One_Country", Name = "Read_One_Country")]
        public IActionResult Read_One_Country([FromBody] Input_Param_Country_Read_One Country_Data)
        {
            try
            {
                if (Country_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Country_Data.Org_Id;
                var Country_Id = Country_Data.Country_Id;
                var Country_Name = "";
                var Method_Name = "Get_One";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Country_List.FromSqlInterpolated($"CALL dbo.USP_Master_Country ({Method_Name},{Org_Id},{Country_Id},{Country_Name},{1},{0},{null},{""},{null},{""});").ToList();
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

        // API to Update Country
        public class Input_Param_Country_Update
        {
            public string Org_Id { get; set; }
            public string Country_Id { get; set; }
            public string Country_Name { get; set; }
            public int Is_Active { get; set; }
            public string LastEdited_By { get; set; }
        } 

        [HttpPost("Update_Country", Name = "Update_Country")]
        public IActionResult Update_Country([FromBody] Input_Param_Country_Update Country_Data)
        {
            try
            {
                if (Country_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Country_Data.Org_Id;
                var Country_Id = Country_Data.Country_Id;
                var Country_Name = Country_Data.Country_Name.Trim();
                int Is_Active = Country_Data.Is_Active;
                var LastEdited_By = Country_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();


                int Is_Deleted = 0;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = "Update";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Master_Country ({Method_Name},{Org_Id},{Country_Id},{Country_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By});").ToList();
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

        // API to Delete Country
        public class Input_Param_Country_Delete
        {
            public string Org_Id { get; set; }
            public string Country_Id { get; set; }
            public int Is_Deleted { get; set; }
            public string LastEdited_By { get; set; }
        }

        [HttpPost("Delete_Country", Name = "Delete_Country")]
        public IActionResult Delete_Country([FromBody] Input_Param_Country_Delete Country_Data)
        {
            try
            {
                if (Country_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Country_Data.Org_Id;
                var Country_Id = Country_Data.Country_Id;
                var Country_Name = "";
                int Is_Active = 0;
                var LastEdited_By = Country_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();
                int Is_Deleted = Country_Data.Is_Deleted;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = "Delete";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Master_Country ({Method_Name},{Org_Id},{Country_Id},{Country_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By});").ToList();
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

        #endregion

        #region State
        // State related API
        // API to Read All State
        public class Input_Param_State_Read_All
        {
            public string Org_Id { get; set; }
            public string Country_Id { get; set; }
            public string State_Name { get; set; }
        }

        [HttpPost("Read_All_State", Name = "Read_All_State")]
        public IActionResult Read_All_State([FromBody] Input_Param_State_Read_All State_Data)
        {
            try
            {
                if (State_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = State_Data.Org_Id;
                var Country_Id = State_Data.Country_Id;
                var State_Name = State_Data.State_Name;
                var State_Id = "";
                var Method_Name = "Get";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.State_List.FromSqlInterpolated($"CALL dbo.USP_Master_State ({Method_Name},{Org_Id},{Country_Id},{State_Id},{State_Name},{1},{0},{null},{""},{null},{""});").ToList();
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

        // API to Create State
        public class Input_Param_State_Create
        {
            public string Org_Id { get; set; }
            public string Country_Id { get; set; }
            public string State_Name { get; set; }
            public int Is_Active { get; set; }
            public string Created_By { get; set; }
        }

        [HttpPost("Create_State", Name = "Create_State")]
        public IActionResult Create_State([FromBody] Input_Param_State_Create State_Data)
        {
            try
            {
                if (State_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = State_Data.Org_Id;
                var Country_Id = State_Data.Country_Id;
                var State_Name = State_Data.State_Name.Trim();
                var Created_By = State_Data.Created_By;
                var Created_On = Get_Current_DateTime();
                var State_Id = "";
                int Is_Active = State_Data.Is_Active;
                int Is_Deleted = 0;
                var Method_Name = "Create";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Master_State ({Method_Name},{Org_Id},{Country_Id},{State_Id},{State_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{null},{""});").ToList();
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

        // API to Read One State
        public class Input_Param_State_Read_One
        {
            public string Org_Id { get; set; }
            public string Country_Id { get; set; }
            public string State_Id { get; set; }
        }

        [HttpPost("Read_One_State", Name = "Read_One_State")]
        public IActionResult Read_One_State([FromBody] Input_Param_State_Read_One State_Data)
        {
            try
            {
                if (State_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = State_Data.Org_Id;
                var Country_Id = State_Data.Country_Id;
                var State_Id = State_Data.State_Id;
                var State_Name = "";
                var Method_Name = "Get_One";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.State_List.FromSqlInterpolated($"CALL dbo.USP_Master_State ({Method_Name},{Org_Id},{Country_Id},{State_Id},{State_Name},{1},{0},{null},{""},{null},{""});").ToList();
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

        // API to Update State
        public class Input_Param_State_Update
        {
            public string Org_Id { get; set; }
            public string Country_Id { get; set; }
            public string State_Id { get; set; }
            public string State_Name { get; set; }
            public int Is_Active { get; set; }
            public string LastEdited_By { get; set; }
        }

        [HttpPost("Update_State", Name = "Update_State")]
        public IActionResult Update_State([FromBody] Input_Param_State_Update State_Data)
        {
            try
            {
                if (State_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = State_Data.Org_Id;
                var Country_Id = State_Data.Country_Id;
                var State_Id = State_Data.State_Id;
                var State_Name = State_Data.State_Name.Trim();
                int Is_Active = State_Data.Is_Active;
                var LastEdited_By = State_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();


                int Is_Deleted = 0;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = "Update";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Master_State ({Method_Name},{Org_Id},{Country_Id},{State_Id},{State_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By} ;").ToList();
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

        // API to Delete State
        public class Input_Param_State_Delete
        {
            public string Org_Id { get; set; }
            public string Country_Id { get; set; }
            public string State_Id { get; set; }
            public int Is_Deleted { get; set; }
            public string LastEdited_By { get; set; }
        }

        [HttpPost("Delete_State", Name = "Delete_State")]
        public IActionResult Delete_State([FromBody] Input_Param_State_Delete State_Data)
        {
            try
            {
                if (State_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = State_Data.Org_Id;
                var Country_Id = State_Data.Country_Id;
                var State_Id = State_Data.State_Id;
                var State_Name = "";
                int Is_Active = 0;
                var LastEdited_By = State_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();
                int Is_Deleted = State_Data.Is_Deleted;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = "Delete";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Master_State ({Method_Name},{Org_Id},{Country_Id},{State_Id},{State_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By} ;").ToList();
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

        #endregion

        #region City
        // City related API
        // API to Read All City
        public class Input_Param_City_Read_All
        {
            public string Org_Id { get; set; }
            public string Country_Id { get; set; }
            public string State_Id { get; set; }
            public string City_Name { get; set; }
        }

        [HttpPost("Read_All_City", Name = "Read_All_City")]
        public IActionResult Read_All_City([FromBody] Input_Param_City_Read_All City_Data)
        {
            try
            {
                if (City_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = City_Data.Org_Id;
                var Country_Id = City_Data.Country_Id;
                var State_Id = City_Data.State_Id;
                var City_Name = City_Data.City_Name;
                var City_Id = "";
                var Method_Name = "Get";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.City_List.FromSqlInterpolated($"CALL dbo.USP_Master_City ({Method_Name},{Org_Id},{Country_Id},{State_Id},{City_Id},{City_Name},{1},{0},{""},{""},{""},{""});").ToList();
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

        // API to Create City
        public class Input_Param_City_Create
        {
            public string Org_Id { get; set; }
            public string Country_Id { get; set; }
            public string State_Id { get; set; }
            public string City_Name { get; set; }
            public int Is_Active { get; set; }
            public string Created_By { get; set; }
        }

        [HttpPost("Create_City", Name = "Create_City")]
        public IActionResult Create_City([FromBody] Input_Param_City_Create City_Data)
        {
            try
            {
                if (City_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = City_Data.Org_Id;
                var Country_Id = City_Data.Country_Id;
                var State_Id = City_Data.State_Id;
                var City_Name = City_Data.City_Name.Trim();
                var Created_By = City_Data.Created_By;
                var Created_On = Get_Current_DateTime();
                var City_Id = "";
                int Is_Active = City_Data.Is_Active;
                int Is_Deleted = 0;
                var Method_Name = "Create";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Master_City ({Method_Name},{Org_Id},{Country_Id},{State_Id},{City_Id},{City_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{null},{""});").ToList();
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

        // API to Read One City
        public class Input_Param_City_Read_One
        {
            public string Org_Id { get; set; }
            public string Country_Id { get; set; }
            public string State_Id { get; set; }
            public string City_Id { get; set; }
        }

        [HttpPost("Read_One_City", Name = "Read_One_City")]
        public IActionResult Read_One_City([FromBody] Input_Param_City_Read_One City_Data)
        {
            try
            {
                if (City_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = City_Data.Org_Id;
                var Country_Id = City_Data.Country_Id;
                var State_Id = City_Data.State_Id;
                var City_Id = City_Data.City_Id;
                var City_Name = "";
                var Method_Name = "Get_One";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.City_List.FromSqlInterpolated($"CALL dbo.USP_Master_City ({Method_Name},{Org_Id},{Country_Id},{State_Id},{City_Id},{City_Name},{1},{0},{null},{""},{null},{""});").ToList();
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

        // API to Update City
        public class Input_Param_City_Update
        {
            public string Org_Id { get; set; }
            public string Country_Id { get; set; }
            public string State_Id { get; set; }
            public string City_Id { get; set; }
            public string City_Name { get; set; }
            public int Is_Active { get; set; }
            public string LastEdited_By { get; set; }
        }

        [HttpPost("Update_City", Name = "Update_City")]
        public IActionResult Update_City([FromBody] Input_Param_City_Update City_Data)
        {
            try
            {
                if (City_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = City_Data.Org_Id;
                var Country_Id = City_Data.Country_Id;
                var State_Id = City_Data.State_Id;
                var City_Id = City_Data.City_Id;
                var City_Name = City_Data.City_Name.Trim();
                int Is_Active = City_Data.Is_Active;
                var LastEdited_By = City_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();

                int Is_Deleted = 0;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = "Update";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Master_City ({Method_Name},{Org_Id},{Country_Id},{State_Id},{City_Id},{City_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By});").ToList();
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

        // API to Delete City
        public class Input_Param_City_Delete
        {
            public string Org_Id { get; set; }
            public string Country_Id { get; set; }
            public string State_Id { get; set; }
            public string City_Id { get; set; }
            public int Is_Deleted { get; set; }
            public string LastEdited_By { get; set; }
        }

        [HttpPost("Delete_City", Name = "Delete_City")]
        public IActionResult Delete_City([FromBody] Input_Param_City_Delete City_Data)
        {
            try
            {
                if (City_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = City_Data.Org_Id;
                var Country_Id = City_Data.Country_Id;
                var State_Id = City_Data.State_Id;
                var City_Id = City_Data.City_Id;
                var City_Name = "";
                int Is_Active = 0;
                var LastEdited_By = City_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();
                int Is_Deleted = City_Data.Is_Deleted;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = "Delete";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Master_City ({Method_Name},{Org_Id},{Country_Id},{State_Id},{City_Id},{City_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By});").ToList();
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

        #endregion

        #region Location
        // Location related API
        // API to Read All Location
        public class Input_Param_Location_Read_All
        {
            public string Org_Id { get; set; }
            public string Country_Id { get; set; }
            public string State_Id { get; set; }
            public string City_Id { get; set; }
            public string Location_Name { get; set; }
        }

        [HttpPost("Read_All_Location", Name = "Read_All_Location")]
        public IActionResult Read_All_Location([FromBody] Input_Param_Location_Read_All Location_Data)
        {
            try
            {
                if (Location_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Location_Data.Org_Id;
                var Country_Id = Location_Data.Country_Id;
                var State_Id = Location_Data.State_Id;
                var City_Id = Location_Data.City_Id;
                var Location_Name = Location_Data.Location_Name;
                var Location_Id = "";
                var Method_Name = "Get";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Location_List.FromSqlInterpolated($"CALL dbo.USP_Master_Location ({Method_Name},{Org_Id},{Country_Id},{State_Id},{City_Id},{Location_Id},{Location_Name},{1},{0},{null},{""},{null},{""});").ToList();
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

        // API to Create Location
        public class Input_Param_Location_Create
        {
            public string Org_Id { get; set; }
            public string Country_Id { get; set; }
            public string State_Id { get; set; }
            public string City_Id { get; set; }
            public string Location_Name { get; set; }
            public int Is_Active { get; set; }
            public string Created_By { get; set; }
        }

        [HttpPost("Create_Location", Name = "Create_Location")]
        public IActionResult Create_Location([FromBody] Input_Param_Location_Create Location_Data)
        {
            try
            {
                if (Location_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Location_Data.Org_Id;
                var Country_Id = Location_Data.Country_Id;
                var State_Id = Location_Data.State_Id;
                var City_Id = Location_Data.City_Id;
                var Location_Name = Location_Data.Location_Name.Trim();
                var Created_By = Location_Data.Created_By;
                var Created_On = Get_Current_DateTime();
                var Location_Id = "";
                int Is_Active = Location_Data.Is_Active;
                int Is_Deleted = 0;
                var Method_Name = "Create";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Master_Location ({Method_Name},{Org_Id},{Country_Id},{State_Id},{City_Id},{Location_Id},{Location_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{null},{""});").ToList();
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

        // API to Read One Location
        public class Input_Param_Location_Read_One
        {
            public string Org_Id { get; set; }
            public string Country_Id { get; set; }
            public string State_Id { get; set; }
            public string City_Id { get; set; }
            public string Location_Id { get; set; }
        }

        [HttpPost("Read_One_Location", Name = "Read_One_Location")]
        public IActionResult Read_One_Location([FromBody] Input_Param_Location_Read_One Location_Data)
        {
            try
            {
                if (Location_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Location_Data.Org_Id;
                var Country_Id = Location_Data.Country_Id;
                var State_Id = Location_Data.State_Id;
                var City_Id = Location_Data.City_Id;
                var Location_Id = Location_Data.Location_Id;
                var Location_Name = "";
                var Method_Name = "Get_One";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Location_List.FromSqlInterpolated($"CALL dbo.USP_Master_Location ({Method_Name},{Org_Id},{Country_Id},{State_Id},{City_Id},{Location_Id},{Location_Name},{1},{0},{null},{""},{null},{""});").ToList();
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

        // API to Update Location
        public class Input_Param_Location_Update
        {
            public string Org_Id { get; set; }
            public string Country_Id { get; set; }
            public string State_Id { get; set; }
            public string City_Id { get; set; }
            public string Location_Id { get; set; }
            public string Location_Name { get; set; }
            public int Is_Active { get; set; }
            public string LastEdited_By { get; set; }
        }

        [HttpPost("Update_Location", Name = "Update_Location")]
        public IActionResult Update_Location([FromBody] Input_Param_Location_Update Location_Data)
        {
            try
            {
                if (Location_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Location_Data.Org_Id;
                var Country_Id = Location_Data.Country_Id;
                var State_Id = Location_Data.State_Id;
                var City_Id = Location_Data.City_Id;
                var Location_Id = Location_Data.Location_Id;
                var Location_Name = Location_Data.Location_Name.Trim();
                int Is_Active = Location_Data.Is_Active;
                var LastEdited_By = Location_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();

                int Is_Deleted = 0;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = "Update";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Master_Location ({Method_Name},{Org_Id},{Country_Id},{State_Id}, {City_Id},{Location_Id},{Location_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By});").ToList();
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

        // API to Delete Location
        public class Input_Param_Location_Delete
        {
            public string Org_Id { get; set; }
            public string Country_Id { get; set; }
            public string State_Id { get; set; }
            public string City_Id { get; set; }
            public string Location_Id { get; set; }
            public int Is_Deleted { get; set; }
            public string LastEdited_By { get; set; }
        }

        [HttpPost("Delete_Location", Name = "Delete_Location")]
        public IActionResult Delete_Location([FromBody] Input_Param_Location_Delete Location_Data)
        {
            try
            {
                if (Location_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Location_Data.Org_Id;
                var Country_Id = Location_Data.Country_Id;
                var State_Id = Location_Data.State_Id;
                var City_Id = Location_Data.City_Id;
                var Location_Id = Location_Data.Location_Id;
                var Location_Name = "";
                int Is_Active = 0;
                var LastEdited_By = Location_Data.LastEdited_By;
                var LastEdited_On = Get_Current_DateTime();
                int Is_Deleted = Location_Data.Is_Deleted;
                var Created_By = "";
                var Created_On = "";
                var Method_Name = "Delete";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL dbo.USP_Master_Location ({Method_Name},{Org_Id},{Country_Id},{State_Id},{City_Id},{Location_Id},{Location_Name},{Is_Active},{Is_Deleted},{Created_On},{Created_By},{LastEdited_On},{LastEdited_By});").ToList();
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

        public class Input_Param_Location_Read_All_For_User_City
        {
            public string Org_Id { get; set; }
            public string User_Id { get; set; }
        }

        [HttpPost("Read_All_Location_For_User_City", Name = "Read_All_Location_For_User_City")]
        public IActionResult Read_All_Location_For_User_City([FromBody] Input_Param_Location_Read_All_For_User_City Location_Data)
        {
            try
            {
                if (Location_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Location_Data.Org_Id;
                var User_Id = Location_Data.User_Id;
                var Method_Name = "Get_For_User_City";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Location_List.FromSqlInterpolated($"CALL dbo.USP_Master_Location ({Method_Name},{Org_Id},{User_Id},{""},{""},{""},{""},{1},{0},{null},{""},{null},{""});").ToList();
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

        #endregion

    }



}
