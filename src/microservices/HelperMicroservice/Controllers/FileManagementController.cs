using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Middleware;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http;


namespace Helper_Microservice.Controllers
{
    [Route("v1/api/FileManagement/")]
    [ApiController]

    public class FileManagementController : Controller
    {
        private IHostingEnvironment hostingEnv;

        public FileManagementController(IHostingEnvironment env)
        {
            this.hostingEnv = env;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public IActionResult Index([FromForm] FileInputModel Files)
        {
            return View();
        }

        public class Return_Master
        {
            public string Res_Output { get; set; }
        }

        public class FileInputModel
        {
            public IFormFile File { get; set; }
            public string Org_Id { get; set; }
            public string Method_Name { get; set; }
            public string Sys_File_Name { get; set; }
        }

        [HttpPost("Upload_File", Name = "Upload_File")]
        public string Upload_File([FromForm] FileInputModel Files)
        {
            string result = string.Empty;
            var Org_Id = Files.Org_Id;
            
            var Method_Name = Files.Method_Name;
            var Sys_File_Name = Files.Sys_File_Name;
            try
            {
                long size = 0;
                var file = Request.Form.Files;

                var filename = ContentDispositionHeaderValue
                                .Parse(file[0].ContentDisposition)
                                .FileName
                                .Trim('"');

                if (Sys_File_Name != "")
                {
                    FileInfo fi = new FileInfo(filename);
                    string extn = fi.Extension;
                    filename = Sys_File_Name + extn;
                }

                string FilePath;
                string Relative_FilePath;


                string Document_Path = hostingEnv.WebRootPath + "/user_documents/" + Method_Name;


                if (!Directory.Exists(Document_Path))
                {
                    Directory.CreateDirectory(Document_Path);
                }

                Guid loGuid = Guid.NewGuid();
                filename = loGuid + "_" + filename;

                FilePath = hostingEnv.WebRootPath + "/user_documents/" + Method_Name + $@"/{ filename}";
                Relative_FilePath = "/user_documents/" + Method_Name  + $@"/{ filename}";

                size += file[0].Length;

                using (FileStream fs = System.IO.File.Create(FilePath))
                {
                    file[0].CopyTo(fs);
                    fs.Flush();
                }

                result = Relative_FilePath;


            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;

        }

        // API to Save Content In a File
        public class Input_Param_Save_File_Content
        {
            public string Org_Id { get; set; }
            public string Method_Name { get; set; }
            public string Shop_Name { get; set; }
            public string File_Content { get; set; }
            public string Content_File_Name { get; set; }
        }

        [HttpPost("Save_File", Name = "Save_File")]
        public IActionResult Save_File([FromBody] Input_Param_Save_File_Content File_Data)
        {
            try
            {
                if (File_Data == null)
                {
                    var Err_Msg_Str = "Empty input parameter";
                    return BadRequest(Err_Msg_Str);
                }

                var Org_Id = File_Data.Org_Id;
                var Method_Name = File_Data.Method_Name;
                var Shop_Name = File_Data.Shop_Name;
                var File_Content = File_Data.File_Content;
                var Content_File_Name = File_Data.Content_File_Name;
                var FilePath = "";

                // Generate File
                if (Method_Name == "Save_Product")
                {
                    FilePath = hostingEnv.WebRootPath + "/" + Shop_Name + "/product" + $@"/{ Content_File_Name}";
                }
                else if (Method_Name == "Save_Collection")
                {
                    FilePath = hostingEnv.WebRootPath + "/" + Shop_Name + "/collection" + $@"/{ Content_File_Name}";
                }
                else if (Method_Name == "Save_Group")
                {
                    FilePath = hostingEnv.WebRootPath + "/" + Shop_Name + "/group" + $@"/{ Content_File_Name}";
                }

                // This method automatically opens the file, writes to it, and closes file  
                System.IO.File.WriteAllText(FilePath, File_Content);


                // Generate Output String 
                var Output_API_String = FilePath;
                var Return_Master = new Return_Master();
                Return_Master.Res_Output = Output_API_String;
                return Ok(Return_Master);


            }
            catch (Exception ex)
            {

                var Return_Error = new Return_Master();
                Return_Error.Res_Output = ex.Message;
                return StatusCode(500, Return_Error);
            }
        }


    }
}