using System;
using HelperMicroservice.Entities;
using HelperMicroservice.Repository;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Middleware;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using System.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Middleware.Hubs;
using System.Net.Mail;
using System.Net;
using RestSharp;
using System.Runtime.Serialization.Json;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Collections.Generic;

namespace Helper_Microservice.Controllers
{
    [Route("v1/api/Notify/")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IHelperRepository _repository;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IEncryptor _encryptor;
        private readonly IHubContext<ChatHub> _hubContext;

        public NotificationController(ILoggerManager logger, IHelperRepository repository, IHubContext<ChatHub> hubContext)
        {
            _logger = logger;
            _repository = repository;
            _hubContext = hubContext;
        }

        // Common Function
        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public string Get_Current_DateTime()    // Gives current datetime.  Should be modified to find IST
        {
            var Cur_DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");     //yyyy-mm-dd hh:mm:ss(24h)
            return Cur_DateTime;
        }

        public class Return_Master
        {
            public string Res_Output { get; set; }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("Send_Email_Notification", Name = "Send_Email_Notification")]

        public int Send_Email_Notification(string Org_Id, string User_Id, string Email_To, string Email_Cc, String Email_Bcc, string Email_Subject, string Email_Body, int Is_System_Notification)
        {
            try
            {
                using (MailMessage mm = new MailMessage())
                {
                    mm.To.Add("shanawazummarkhan@gmail.com");
                  //  if (Email_Cc != "") { mm.CC.Add(Email_Cc); }
                  //  if (Email_Bcc != "") { mm.Bcc.Add(Email_Bcc); }
                    mm.Subject = "Email_Subject";
                    mm.Body = "Email_Body";
                    mm.IsBodyHtml = true;
                    MailAddress address = new MailAddress("test_mail@gmail.com");
                    mm.From = address;

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential NetworkCred = new NetworkCredential("test_mail@gmail.com", "test@54321");
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = 587;
                    smtp.Send(mm); // Email Success

                    return 1;
                }
            }
            catch (Exception ex)
            {
                return 1;
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public ArrayList Send_SMS_Notification(string Org_Id, string User_Id, string SMS_To, string SMS_Text, string Action_Type, int Is_System_Notification)
        {
            ArrayList Response_Array = new ArrayList();
            try
            {
                
                // Anil R SMS API
                var uri = "http://49.50.67.32/smsapi/httpapi.jsp?username=we3test&password=testpass&from=MOSL&to=" + SMS_To + "&text=" + SMS_Text + "";
                var translationWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(uri);
                var response = translationWebRequest.GetResponse();
                System.IO.Stream stream = response.GetResponseStream();
                Encoding encode = Encoding.GetEncoding("utf-8");
                System.IO.StreamReader translatedStream = new System.IO.StreamReader(stream, encode);
                var resp = translatedStream.ReadToEnd();
                Response_Array.Add(new
                {
                    response = resp
                });
                return Response_Array;
            }
            catch (Exception ex)
            {
                Response_Array.Add(new
                {
                    response = "Error Sending SMS",
                });
                return Response_Array;
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public int Send_System_Notification(string Org_Id, string User_Id, string Notification_Type, string User_Name, string Notification_Message, int Is_System_Notification)
        {
            try
            {
                using (var context = new HelperRepositoryContext())
                {
                    var Method_Name = "Create";
                    var Sent_On = Get_Current_DateTime();

                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL USP_Notification ({Method_Name},{Org_Id},{User_Id},{User_Name},{""},{Notification_Message},{Sent_On},{""},{""},{0},{""},{0},{""},{0},{Is_System_Notification},{Notification_Type});").ToList();
                    if (Return_Master_List.Count > 0)
                    {
                        // Send message using SignalR
                        _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Org_Id, User_Id, "Update_Notification");  // Inform User that he/she has new notification

                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }

            }
            catch (Exception ex)
            {
                return -1;
            }

        }

        // API to Read All Notification
        public class Input_Param_Notification
        {
            public string Org_Id { get; set; }
            public string User_Id { get; set; }
            public string Notification_Type { get; set; }
            public string Notification_Id { get; set; }
            public string Method_Name { get; set; }
        }

        [HttpPost("Read_Notification", Name = "Read_Notification")]
        public IActionResult Read_Notification([FromBody] Input_Param_Notification Notification_Data)
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
                var User_Id = Notification_Data.User_Id;
                var Method_Name = Notification_Data.Method_Name;
                var Notification_Type = Notification_Data.Notification_Type;
                var Notification_Id = Notification_Data.Notification_Id;

                using (var context = new HelperRepositoryContext())
                {
                    var Return_Master_List = context.Notification_List.FromSqlInterpolated($"CALL USP_User_Notification ({Method_Name},{Org_Id},{User_Id},{""},{Notification_Id},{""},{""},{""},{""},{0},{""},{0},{""},{0},{0},{Notification_Type});").ToList();
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

        // API to Update Notification
        public class Input_Param_Notification_Update
        {
            public string Org_Id { get; set; }
            public string User_Id { get; set; }
            public string Notification_Id { get; set; }
            public int Is_Read { get; set; }
            public string Method_Name { get; set; }
        }

        [HttpPost("Update_Notification_Status", Name = "Update_Notification_Status")]
        public IActionResult Update_Notification_Status([FromBody] Input_Param_Notification_Update Notification_Data)
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
                int Is_Read = Notification_Data.Is_Read;
                var Read_On = Get_Current_DateTime();
                var User_Id = Notification_Data.User_Id;
                var Method_Name = Notification_Data.Method_Name;

                using (var context = new HelperRepositoryContext())
                {
                    var Return_Master_List = context.Common_Command_Output.FromSqlInterpolated($"CALL USP_User_Notification ({Method_Name},{Org_Id},{User_Id},{""},{Notification_Id},{""},{""},{""},{""},{1},{""},{Is_Read},{Read_On},{0},{1},{""});").ToList();

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

        // API to send System Notification or SMS or EMail from System
        public class Input_Param_System_Messages
        {
            public string Org_Id { get; set; }
            public string User_Id { get; set; }
            public string Notification_Type { get; set; }  // Possible Values (Email, SMS, Notification, Popup, Marquee)
            public string Message_To { get; set; }  // User Name
            public string Message_CC { get; set; }
            public string Message_Bcc { get; set; }
            public string Message_Subject { get; set; }
            public string Message_Body { get; set; }
            public string Method_Name { get; set; }
            public int Is_System_Notification { get; set; }
        }

        [HttpPost("Send_Notification", Name = "Send_Notification")]
        public IActionResult Send_Notification([FromBody] Input_Param_System_Messages Message_Data)
        {
            try
            {
                if (Message_Data == null)
                {
                    var Err_Msg_Str = "User Credentials is null";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = Message_Data.Org_Id;
                var User_Id = Message_Data.User_Id;

                var Notification_Type = Message_Data.Notification_Type;
                var Message_To = Message_Data.Message_To;
                var Message_CC = Message_Data.Message_CC;
                var Message_Bcc = Message_Data.Message_Bcc;
                var Message_Subject = Message_Data.Message_Subject;
                var Method_Name = Message_Data.Method_Name;
                var Message_Body = Message_Data.Message_Body;
                var Is_System_Notification = Message_Data.Is_System_Notification;

                switch (Notification_Type)
                {
                    case "EMail":
                        Send_Email_Notification(Org_Id, User_Id, Message_To, Message_CC, Message_Bcc, Message_Subject, Message_Body, Is_System_Notification);
                        break;
                    case "SMS":
                        Send_SMS_Notification(Org_Id, User_Id, Message_To, Message_Subject, "", Is_System_Notification);
                        break;
                    case "Notification":
                        Send_System_Notification(Org_Id, User_Id, Notification_Type, Message_To, Message_Subject, Is_System_Notification);
                        break;
                    default:
                        break;
                }

                var Return_Message = "{\"Response\": \"OK\"}";
                return Ok(Return_Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in User OTP  : {ex.Message}");

                var Return_Error = "{\"Response\": \"Error\", \"Error_No\": \"EL02\", \"Error_Msg\": \"" + ex.Message + "\"}";
                return StatusCode(500, Return_Error);
            }
        }

        public class Input_Param_Display_Notification_Data
        {
            public string User_Id { get; set; }

            public string Application_Id { get; set; }
        }

        // API to Read Data for Dashboard Display

        [HttpPost("Display_Notification_Data", Name = "Display_Notification_Data")]
        public IActionResult Display_Notification_Data([FromBody] Input_Param_Display_Notification_Data NotificationDisplayData)
        {
            try
            {
                if (NotificationDisplayData == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    _logger.LogError(Err_Msg_Str);
                    return BadRequest(Err_Msg_Str);
                }
                var Application_Id = NotificationDisplayData.Application_Id;
                var User_Id = NotificationDisplayData.User_Id;
               
                using (var context = new HelperRepositoryContext())
                    {
                    var Return_Notification_Data = context.Notification_Display_Data.FromSqlInterpolated($"CALL dbo.Kwik_NOTIFICATION ({User_Id},{Application_Id});").ToList();
                    if (Return_Notification_Data.Count > 0)
                    {
                        var Return_Master_String = JsonConvert.SerializeObject(Return_Notification_Data);

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
        public class dataTOpass
        {

            public string pan_no { get; set; }

            public string full_name { get; set; }
            public string date_of_birth { get; set; }
        }

        [HttpPost("PanAPITest", Name = "PanAPITest")]
        public IActionResult PanAPITest(dataTOpass panData)
        {
            ArrayList Response_Array = new ArrayList();
            var client = new RestClient("https://ext.digio.in:444/v3/client/kyc/pan/verify");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Basic QUlZM0gxWFM1QVBUMkVNRkU1NFVXWjU2SVE4RlBLRlA6R083NUZXMllBWjZLUU0zRjFaU0dRVlVRQ1pQWEQ2T0Y=");
            request.AddHeader("Content-Type", "application/json");
           
            dataTOpass cust = new dataTOpass() 
            { pan_no = panData.pan_no, full_name = panData.full_name, date_of_birth = panData.date_of_birth };

            string result = JsonConvert.SerializeObject(cust);

            request.AddParameter("application/json", result, ParameterType.RequestBody);
            IRestResponse response =  client.Execute(request);
       
            Console.WriteLine(response.Content);
 
            return Ok(response.Content);
        }
        private const string Secret = "db3OIsj+BXE9NZDy0t8W3TcNekrF+2d/1sFnWG4HnV8TZY30iTOdtVWJG8abWvB1GlOgJuQZdcF2Luqm/hccMw==";

        public class SmsVerify
        {
            public string smsContact { get; set; }

            public string OTP { get; set; }
        }
        // message call API
        [HttpPost("smsAPI", Name = "smsAPI")]
        public IActionResult smsAPI(SmsVerify smsData)
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            var otpBKC = _rdm.Next(_min, _max);
            var client = new RestClient("http://mobile1.ssexpertsystem.com/vendorsms/pushsms.aspx?user=mangalk&password=mangal09a&msisdn="+smsData.smsContact+"&sid=MKFSLP&msg=Dear Sir/Madam,Greetings from Mangal Keshav! "+ otpBKC + " is your one time password (OTP) for Mobile verification to complete the Mangal Keshav eKYC process.Regards,Mangal Keshav Team&fl=0&gwid=2");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var body = @"{
" + "\n" +
            @"    
" + "\n" +
            @"}";
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            //Console.WriteLine(response.Content);
            var jsonotpBKC = Newtonsoft.Json.JsonConvert.SerializeObject(otpBKC);
            return Ok(new { response.Content, jsonotpBKC });
            //return Ok(otpBKC);
        }

        public class VerifyDetails
        {
            public string flag { get; set; }
            public string smsContact { get; set; }

        }
        // message call API
        [HttpPost("VerifyNumber", Name = "VerifyNumber")]
        public void VerifyNumber(VerifyDetails VerifyOBJ)
         {
            var contact = "abc";
            //return Ok(contact);
        }

        public class JWTWebTokenData
        {
            public string smsContact { get; set; }

            public string OTP { get; set; }

        }
        // message call API
        [HttpPost("JWTWebToken", Name = "JWTWebToken")]
        public IActionResult JWTWebToken(JWTWebTokenData jwtOBJ)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, jwtOBJ.smsContact)
        }),

                //Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(symmetricKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var JWTtoken = tokenHandler.WriteToken(stoken);

            return Ok(JWTtoken);

        }

        public class EmailData
        {
            public string emailSend { get; set; }

            public string user_token { get; set; }
            //public string date_of_birth { get; set; }
        }

        [HttpPost("EmailAPITest", Name = "EmailAPITest")]
        public void EmailAPITest(EmailData emailOBJ)
        {
            var token = Convert.ToString(emailOBJ.user_token);
                using (MailMessage mm = new MailMessage())
                {
                    mm.To.Add(emailOBJ.emailSend);
                    //  if (Email_Cc != "") { mm.CC.Add(Email_Cc); }
                    //  if (Email_Bcc != "") { mm.Bcc.Add(Email_Bcc); }
                    mm.Subject = "Sending Email using SMTP";
                //"<a href='http://localhost:3000/ConfirmPage/"+ emailOBJ.user_token + @"'>abc</a>"
                String body = "<a href='http://localhost:3000/ConfirmPage/"+token+"'>Verify</a>";
                mm.Body = body;
                mm.IsBodyHtml = true;
                    MailAddress address = new MailAddress("shanawazumarkhan@gmail.com");
                    mm.From = address;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential NetworkCred = new NetworkCredential("shanawazumarkhan@gmail.com", "kjshanawaz");
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = 587;
                    smtp.Send(mm); // Email Success

                  
                }
        }
        public class BankData
        {
            public string beneficiary_account_no { get; set; }

            public string beneficiary_ifsc { get; set; }
            //public string date_of_birth { get; set; }
        }

        [HttpPost("BankVerify", Name = "BankVerify")]
        public IActionResult BankVerify(BankData BankOBJ)
        {
            ArrayList Response_Array = new ArrayList();
            var client = new RestClient("https://ext.digio.in:444/client/verify/bank_account");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Basic QUlZM0gxWFM1QVBUMkVNRkU1NFVXWjU2SVE4RlBLRlA6R083NUZXMllBWjZLUU0zRjFaU0dRVlVRQ1pQWEQ2T0Y=");
            request.AddHeader("Content-Type", "application/json");

            BankData cust = new BankData()
            { beneficiary_account_no = BankOBJ.beneficiary_account_no, beneficiary_ifsc = BankOBJ.beneficiary_ifsc };

            string result = JsonConvert.SerializeObject(cust);

            request.AddParameter("application/json", result, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            Console.WriteLine(response.Content);

            return Ok(response.Content);

        }
        //Razorpay
        //public class RazorPay
        //{
        //    public string beneficiary_account_no { get; set; }

        //    public string beneficiary_ifsc { get; set; }
        //    //public string date_of_birth { get; set; }
        //}

        [HttpPost("RazorPayIntegrated", Name = "RazorPayIntegrated")]
        public IActionResult RazorPayIntegrated()
        {
            Razorpay.Api.RazorpayClient client = new Razorpay.Api.RazorpayClient("rzp_test_dojmbldJSpz91g", "jb9h6XQ1wukVmCyTerDAnSc1");
            Dictionary<string, object> options = new Dictionary<string, object>(); 
            options.Add("amount", 50000); // amount in the smallest currency unit                                                                                                                                                              options.Add("receipt", "order_rcptid_11");
            options.Add("currency", "INR");
            Razorpay.Api.Order order = client.Order.Create(options);

            return Ok(options);

        }
    }

    }
