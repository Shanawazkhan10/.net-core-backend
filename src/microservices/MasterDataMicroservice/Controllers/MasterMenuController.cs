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
    [Route("v1/api/master/menu/")]
    [ApiController]
    public class MasterMenuController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IMasterRepository _userRepository;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IEncryptor _encryptor;

        public MasterMenuController(ILoggerManager logger, IMasterRepository userRepository, IJwtBuilder jwtBuilder, IEncryptor encryptor)
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

        #region Menu
        //All Menu related API's

        // MAster Menu Model for all API's
        public class Input_Param_Menu_Master
        {
            public string Method_Name { get; set; }
            public string User_Id { get; set; }
            public string Org_Id { get; set; }
            public string Application_Id { get; set; }
            public string Menu_Name { get; set; }
            public string Menu_Id { get; set; }
            public string Parent_Menu_Id { get; set; }
            public string Display_Order_Number { get; set; }
            public string Menu_Icon_Name { get; set; }
            public string Menu_Tooltip { get; set; }
            public string Functional_Owner { get; set; }
            public string Technical_Owner { get; set; }
            public string For_Devices { get; set; }
            public string Applicable_To { get; set; }
            public int Is_Active { get; set; }
            public int Is_Deleted { get; set; }
        }
           

        // API to read all Menu

        [HttpPost("Read_All_Menu", Name = "Read_All_Menu")]
        public IActionResult Read_All_Menu([FromBody] Input_Param_Menu_Master Menu_Data)
        {
            try
            {
                if (Menu_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var User_Id = Menu_Data.User_Id;
                var Org_Id = Menu_Data.Org_Id;
                var Application_Id = Menu_Data.Application_Id;
                decimal Display_Order_Number = 0;
                var Method_Name = "Get";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Menu_List.FromSqlInterpolated($"CALL dbo.USP_Master_Menu  ({Method_Name},{Org_Id},{User_Id},{Application_Id},{Display_Order_Number},{""},{""},{""});").ToList();
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

        //API to read all Parent Menu 

        public class Input_Param_Parent_Menu_Read_All
        {
            public string Org_Id { get; set; }
            public string Application_Id { get; set; }
            public string Parent_Menu_Id { get; set; }

        }

        [HttpPost("Read_All_Parent_Menu", Name = "Read_All_Parent_Menu")]
        public IActionResult Read_All_Parent_Menu([FromBody] Input_Param_Parent_Menu_Read_All Role_Data)
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
                var Parent_Menu_Id = "";
                var Method_Name = "GetParent";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Menu_List.FromSqlInterpolated
                        ($"CALL dbo.USP_Master_Menu  ({Method_Name},{Org_Id},{""},{Application_Id},{0.00},{""},{""},{Parent_Menu_Id});").ToList();
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

        //API to read All Applicable To 

        public class Input_Param_Applicable_To_Read_All
        {
            public string Org_Id { get; set; }
            public string Applicable_To { get; set; }
            public string Applicable_To_Id { get; set; }
        } 

        [HttpPost("Read_All_Applicable_To", Name = "Read_All_Applicable_To")]
        public IActionResult Read_All_Applicable_To([FromBody] Input_Param_Applicable_To_Read_All Menu_Data)
        {
            try
            {
                if (Menu_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Menu_Data.Org_Id;
                var Applicable_To = "";
                var Method_Name = "GetApplicableTo";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Config_StaffType.FromSqlInterpolated
                        ($"CALL dbo.USP_Master_Menu  ({Method_Name},{Org_Id},{""},{""},{0.00},{""},{""},{""},{""},{""},{""},{""},{""},{Applicable_To});").ToList();
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

        // API to Create Menu

        [HttpPost("Create_Menu", Name = "Create_Menu")]
        public IActionResult Create_Menu([FromBody] Input_Param_Menu_Master Menu_Data)
        {
            try
            {
                if (Menu_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }
                var User_Id = Menu_Data.User_Id;
                var Org_Id = Menu_Data.Org_Id;
                var Menu_Id = Menu_Data.Menu_Id;
                var Menu_Name = Menu_Data.Menu_Name.Trim();
                Decimal Display_Order_Number = Convert.ToDecimal(Menu_Data.Display_Order_Number.ToString());
                var Application_Id = Menu_Data.Application_Id;
                var Parent_Menu_Id = Menu_Data.Parent_Menu_Id;
                var Menu_Icon_Name = Menu_Data.Menu_Icon_Name;
                var Menu_Tool_Tip = Menu_Data.Menu_Tooltip;
                var Functional_Owner = Menu_Data.Functional_Owner;
                var Technical_Owner = Menu_Data.Technical_Owner;
                var For_Devices = Menu_Data.For_Devices;
                var Applicable_To = Menu_Data.Applicable_To;
                int Is_Active = Menu_Data.Is_Active;
                var Created_On = Get_Current_DateTime();
                var Method_Name = "Create";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated
                        ($"CALL dbo.USP_Master_Menu  ({Method_Name},{Org_Id},{User_Id},{Application_Id},{Display_Order_Number},{Menu_Name},{Menu_Id},{Parent_Menu_Id},{Menu_Icon_Name},{Menu_Tool_Tip},{Functional_Owner},{Technical_Owner},{For_Devices},{Applicable_To});").ToList();
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
                            Return_Error.Res_Output = "{\"Response\": \"Error\", \"Error_No\": \"EMQ2\", \"Error_Msg\": \"Duplicate Menu ID exists\"}";
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

        // API to Read One  Menu

        [HttpPost("Read_One_Menu", Name = "Read_One_Menu")]
        public IActionResult Read_One_Menu([FromBody] Input_Param_Menu_Master Menu_Data)
        {
            try
            {
                if (Menu_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }
                var Menu_Id = Menu_Data.Menu_Id;
                var Org_Id = Menu_Data.Org_Id;
                var Application_Id = "";
                var Parent_Menu_Id = "";
                var Method_Name = "Get_One";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Menu_List.FromSqlInterpolated
                        ($"CALL dbo.USP_Master_Menu ({ Method_Name},{ Org_Id},{""},{ Application_Id},{0.00},{""},{Menu_Id},{ Parent_Menu_Id}; ").ToList();

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


        // API to Update Menu

        [HttpPost("Update_Menu", Name = "Update_Menu")]
        public IActionResult Update_Menu([FromBody] Input_Param_Menu_Master Menu_Data)
        {
            try
            {
                if (Menu_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Menu_Id = Menu_Data.Menu_Id;
                var User_Id = Menu_Data.User_Id;
                var Org_Id = Menu_Data.Org_Id;
                var Application_Id = Menu_Data.Application_Id;
                var Menu_Name = Menu_Data.Menu_Name.Trim();
                var Display_Order_Number = Menu_Data.Display_Order_Number;
                var Parent_Menu_Id = Menu_Data.Parent_Menu_Id;
                var Menu_Icon_Name = Menu_Data.Menu_Icon_Name;
                var Menu_Tool_Tip = Menu_Data.Menu_Tooltip;
                var Functional_Owner = Menu_Data.Functional_Owner;
                var Technical_Owner = Menu_Data.Technical_Owner;
                var For_Devices = Menu_Data.For_Devices;
                var Applicable_To = Menu_Data.Applicable_To;
                int Is_Active = Menu_Data.Is_Active;
                var LastEdited_On = Get_Current_DateTime();

                var Method_Name = "Update";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated
                    ($"CALL dbo.USP_Master_Menu  ({Method_Name},{Org_Id},{User_Id},{Application_Id},{Display_Order_Number},{Menu_Name},{Menu_Id},{Parent_Menu_Id},{Menu_Icon_Name},{Menu_Tool_Tip},{Functional_Owner},{Technical_Owner},{For_Devices},{Applicable_To},{Is_Active});").ToList();                    
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


        // API to Delete Menu

        [HttpPost("Delete_Menu", Name = "Delete_Menu")]
        public IActionResult Delete_Menu([FromBody] Input_Param_Menu_Master Menu_Data)
        {
            try
            {
                if (Menu_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Menu_Id = Menu_Data.Menu_Id;
                var User_Id = Menu_Data.User_Id;
                var Org_Id = Menu_Data.Org_Id;
                var Application_Id = Menu_Data.Application_Id;
                var Menu_Name = "";
                int Is_Active = Menu_Data.Is_Active;
                var LastEdited_On = Get_Current_DateTime();
                int Is_Deleted = Menu_Data.Is_Deleted;
                var Method_Name = "Delete";

                using (var context = new MasterRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated
                        ($"CALL dbo.USP_Master_Menu  ({Method_Name},{Org_Id},{User_Id},{Application_Id},{0.00},{Menu_Name},{Menu_Id},{""},{""},{""});").ToList();
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
