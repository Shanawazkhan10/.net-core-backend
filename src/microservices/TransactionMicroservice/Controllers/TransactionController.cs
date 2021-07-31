using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Middleware;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TransactionMicroservice.Repository;
using TransactionMicroservice.Entities;
using TransactionMicroservice.Model;

namespace TransactionMicroservice.Controllers
{
    [Route("v1/api/transaction/")]
    [ApiController]

    public class TransactionController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly ITransactionRepository _reportRepository;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IEncryptor _encryptor;

        public TransactionController(ILoggerManager logger, ITransactionRepository reportRepository, IJwtBuilder jwtBuilder, IEncryptor encryptor)
        {
            _logger = logger;
            _reportRepository = reportRepository;
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

    }
}
