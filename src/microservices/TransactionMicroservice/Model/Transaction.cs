using Middleware;
using System;

namespace TransactionMicroservice.Model
{
    public class Common_Command_Output
    {
        public int Result_Id { get; set; }
        public string Result_Description { get; set; }
        public string Result_Extra_Key { get; set; }
    }

   
}
