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
    [Route("v1/api/master/notification/")]
    [ApiController]

    public class MasterNotificationController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IMasterRepository _userRepository;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IEncryptor _encryptor;

        public MasterNotificationController(ILoggerManager logger, IMasterRepository userRepository, IJwtBuilder jwtBuilder, IEncryptor encryptor)
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

        #region Notification
        //All Notification related API's

        // MAster Notification Model for all API's
        public class Input_Param_Notification_Master
        {
            public string Method_Name { get; set; }
            public string Notification_Period { get; set; }
            public string Org_Id { get; set; }
            public string Notification_Id { get; set; }
            public string Notification_Type { get; set; }
            public string Staff_Type { get; set; }
            public string Notification_Text { get; set; }
            public DateTime Schedule_Start_Time { get; set; }
            public int Validity_Days { get; set; }
            public int Is_Active { get; set; }
            public int Is_Deleted { get; set; }
            public string Created_On { get; set; }
            public string Created_By { get; set; }
            public string LastEdited_On { get; set; }
            public string LastEdited_By { get; set; }
        }


        // API to read all Notification

        [HttpPost("Read_All_Notification", Name = "Read_All_Notification")]
        public IActionResult Read_All_Notification([FromBody] Input_Param_Notification_Master Notification_Data)
        {
            try
            {
                if (Notification_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Notification_Data.Org_Id;
                var Notification_Id = "";
                var Notification_Period = Notification_Data.Notification_Period;
                var Method_Name = "Get_All";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Notification_List.FromSqlInterpolated($"CALL dbo.USP_Master_Notification ({Method_Name},{Org_Id},{""},{""},{""},{""},{""},{""},{""},{""},{""},{""},{""},{""},{Notification_Period});").ToList();
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


        // API to Create Notification

        [HttpPost("Create_Notification", Name = "Create_Notification")]
        public IActionResult Create_Notification([FromBody] Input_Param_Notification_Master Notification_Data)
        {
            try
            {
                if (Notification_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Notification_Data.Org_Id;
                var Notification_Type = Notification_Data.Notification_Type.Trim();
                var Notification_Text = Notification_Data.Notification_Text;
                var Staff_Type = Notification_Data.Staff_Type;
                var Schedule_Start_Time = Notification_Data.Schedule_Start_Time;
                var Validity_Days = Notification_Data.Validity_Days;
                int Is_Active = Notification_Data.Is_Active;
                var Created_By = Notification_Data.Created_By;
                var Created_On = Get_Current_DateTime();
                var Method_Name = "Create";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated
                        ($"CALL dbo.USP_Master_Notification  ({Method_Name},{Org_Id},{""},{Notification_Type},{Notification_Text},{Staff_Type},{Schedule_Start_Time},{Validity_Days},{Is_Active},{""},{Created_On},{Created_By},{""},{""},{""});").ToList();
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
                        else if (Return_Master_List[0].Result_Id == -2)
                        {
                            // Generate Output String 
                            var Return_Error = new Return_Master();
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ2\", \"Error_Msg\": \"Duplicate Notification ID exists\"}";
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
                _logger.LogError($"Error in Save Notification  : {ex.Message}");

                string Err_msg = Get_Normalised_Message(ex.Message);
                var Return_Error = new Return_Master();
                Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ4\", \"Error_Msg\": \"" + Err_msg + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        // API to Read One  Notification

        [HttpPost("Read_One_Notification", Name = "Read_One_Notification")]
        public IActionResult Read_One_Notification([FromBody] Input_Param_Notification_Master Notification_Data)
        {
            try
            {
                if (Notification_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }
                var Org_Id = Notification_Data.Org_Id;
                var Notification_Id = Notification_Data.Notification_Id;
                var Method_Name = "Get_One";
                
                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Notification_List.FromSqlInterpolated
                        ($"CALL dbo.USP_Master_Notification ({Method_Name},{Org_Id},{Notification_Id},{""},{""},{""},{""},{""},{""},{""},{""},{""},{""},{""},{""});").ToList();

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


        // API to Update Notification

        [HttpPost("Update_Notification", Name = "Update_Notification")]
        public IActionResult Update_Notification([FromBody] Input_Param_Notification_Master Notification_Data)
        {
            try
            {
                if (Notification_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Notification_Data.Org_Id;
                var Notification_Id = Notification_Data.Notification_Id;
                var Notification_Type = Notification_Data.Notification_Type;
                var Staff_Type = Notification_Data.Staff_Type;
                var Notification_Text = Notification_Data.Notification_Text;
                var Schedule_Start_Time = Notification_Data.Schedule_Start_Time;
                var Validity_Days = Notification_Data.Validity_Days;
                int Is_Active = Notification_Data.Is_Active;
                var LastEdited_On = Get_Current_DateTime();
                var LastEdited_By = Notification_Data.LastEdited_By;

                var Method_Name = "Update";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated
                    ($"CALL dbo.USP_Master_Notification ({Method_Name},{Org_Id},{Notification_Id},{Notification_Type},{Notification_Text},{Staff_Type},{Schedule_Start_Time},{Validity_Days},{Is_Active},{""},{""},{""},{LastEdited_On},{LastEdited_By},{""});").ToList();
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


        // API to Delete Notification

        [HttpPost("Delete_Notification", Name = "Delete_Notification")]
        public IActionResult Delete_Notification([FromBody] Input_Param_Notification_Master Notification_Data)
        {
            try
            {
                if (Notification_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Notification_Data.Org_Id;
                var Notification_Id = Notification_Data.Notification_Id;
                var Is_Deleted = Notification_Data.Is_Deleted;
                var LastEdited_By = Notification_Data.LastEdited_By;
                var Method_Name = "Delete";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated
                        ($"CALL dbo.USP_Master_Notification ({Method_Name},{Org_Id},{Notification_Id},{""},{""},{""},{""},{""},{""},{Is_Deleted},{""},{""},{""},{LastEdited_By},{""});").ToList();
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


    }
}
