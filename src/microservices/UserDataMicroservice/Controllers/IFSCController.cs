using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Middleware;
using UserDataMicroservice.Repository;
using RestSharp;
//using RestSharp;
using Microsoft.AspNetCore.Hosting;


namespace UserDataMicroservice.Controllers
{
    [Route("v1/api/user/login/")]
    //:44300/api/user/login/OCR:1
    [ApiController]
    public class IFSCController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IEncryptor _encryptor;
        private object model;
        IWebHostEnvironment _enviroment;
        public string fileTypeData;
        private object client;
        private object request;

        public IFSCController(ILoggerManager logger, IUserRepository userRepository, IJwtBuilder jwtBuilder, IEncryptor encryptor, IWebHostEnvironment enviroment)
        {
            _logger = logger;
            _userRepository = userRepository;
            _jwtBuilder = jwtBuilder;
            _encryptor = encryptor;
            _enviroment = enviroment;
        }
        #region OCR
        public class Input_Param_User_ID
        {
            public string IFSC_code { get; set; }

        }
        [HttpPost("IFSCCheck", Name = "IFSCCheck")]
        public IActionResult IFSCCheck(Input_Param_User_ID ifscOBJ)
        {
            var client = new RestClient("https://ifsc.razorpay.com/"+ifscOBJ.IFSC_code+"");
            client.Timeout = -1;
            RestRequest restRequest = new RestRequest(Method.GET);
            RestRequest request = restRequest;
            //IRestResponse response = (IRestResponse)client.Execute(request);
            var response = client.Execute<List<ThirdPartySuggester>>(request);
            Console.WriteLine(response.Content);
            return Ok(response.Content);
        }

        //private IActionResult Ok(object content)
        //{
        //    //throw new NotImplementedException();
        //    return 0;
        //}
        #endregion
    }
}
