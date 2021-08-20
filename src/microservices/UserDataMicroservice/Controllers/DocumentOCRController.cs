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
using RestSharp;
//using RestSharp;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;


namespace UserDataMicroservice.Controllers
{
    [Route("v1/api/user/login/")]
    //:44300/api/user/login/OCR:1
    [ApiController]
    public class DocumentOCRController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IEncryptor _encryptor;
        private object webHostEnvironment;
        private object model;
        IWebHostEnvironment _enviroment;
        public string fileTypeData;

        public DocumentOCRController(ILoggerManager logger, IUserRepository userRepository, IJwtBuilder jwtBuilder, IEncryptor encryptor,IWebHostEnvironment enviroment)
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
            public IFormFile front_part { get; set; }

        }
        [HttpPost("OCR", Name = "OCR")]
        public IActionResult OCR([FromForm] Input_Param_User_ID panOBJ)
        {
            if (panOBJ.front_part.Length > 0)
            {
                if (!Directory.Exists(_enviroment.WebRootPath + "\\ImgOCR\\"))
                    Directory.CreateDirectory(_enviroment.WebRootPath + "\\ImgOCR\\");
            }
            using (FileStream fileStream = System.IO.File.Create(_enviroment.WebRootPath + "\\ImgOCR\\" + panOBJ.front_part.FileName))
            {
                panOBJ.front_part.CopyTo(fileStream);
                fileStream.Flush();
                string fileTypeData = "\\ImgOCR\\" + panOBJ.front_part.FileName;
            }
            string path = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\imgOCR\"+panOBJ.front_part.FileName+""}";
            var client = new RestClient("https://ext.digio.in:444/v3/client/kyc/analyze/file/idcard");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Basic QUlZM0gxWFM1QVBUMkVNRkU1NFVXWjU2SVE4RlBLRlA6R083NUZXMllBWjZLUU0zRjFaU0dRVlVRQ1pQWEQ2T0Y=");
            request.AddFile("front_part", path);
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

    internal interface IRestResponse
    {
        object Content { get; set; }
    }
}
