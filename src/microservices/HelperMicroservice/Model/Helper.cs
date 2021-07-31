using Middleware;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelperMicroservice.Model
{
    public class Common_Command_Output
    {
        public int Result_Id { get; set; }
        public string Result_Description { get; set; }
        public string Result_Extra_Key { get; set; }
    }

    public class Notification_List
    {
        public string Org_Id { get; set; }
        public string Notification_Id { get; set; }
        public string User_Name { get; set; }
        public string Notification_Message { get; set; }
        public string Sent_On { get; set; }
        public int Is_Delivered { get; set; }
        public string Delivered_To { get; set; }
        public int Is_Read { get; set; }
        public string Read_On { get; set; }
        public string Notification_Link { get; set; }
        public string Notification_Image { get; set; }
        public string Notification_Type { get; set; }
        public int Is_System_Notification { get; set; }
        public DateTime Schedule_Start_Time { get; set; }
        public int Validity_Days { get; set; }
    }

        public class Notification_Display_Data
    {
        public int ID { get; set; }
        public string OutPut { get; set; }
    }
}
