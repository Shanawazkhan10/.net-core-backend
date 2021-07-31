using Middleware;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterDataMicroservice.Model
{
    public class Common_Command_Output
    {
        public int Result_Id { get; set; }
        public string Result_Description { get; set; }
        public string Result_Extra_Key { get; set; }
    }

    // Master Country related Response models
    public class Country_List
    {
        public string Country_Id { get; set; }
        public string Country_Name { get; set; }
        public int Is_Active { get; set; }
        public int Is_Deleted { get; set; }
    }

    // Master State related Response models
    public class State_List
    {
        public string Country_Id { get; set; }
        public string State_Id { get; set; }
        public string State_Name { get; set; }
        public int Is_Active { get; set; }
        public int Is_Deleted { get; set; }
    }

    // Master City related Response models
    public class City_List
    {
        public string Country_Id { get; set; }
        public string State_Id { get; set; }
        public string City_Id { get; set; }
        public string City_Name { get; set; }
        public int Is_Active { get; set; }
        public int Is_Deleted { get; set; }
    }

    // Master Location related Response models
    public class Location_List
    {
        public string Country_Id { get; set; }
        public string State_Id { get; set; }
        public string City_Id { get; set; }
        public string Location_Id { get; set; }
        public string Location_Name { get; set; }
        public int Is_Active { get; set; }
        public int Is_Deleted { get; set; }

    }

    // Config related Response models
    public class Config_Title
    {
        public string Org_Id { get; set; }
        public string Title_Id { get; set; }
        public string Title_Name { get; set; }
        public string Gender_Id { get; set; }
        public int Is_Active { get; set; }
    }

    public class Config_Gender
    {
        public string Org_Id { get; set; }
        public string Gender_Id { get; set; }
        public string Gender_Name { get; set; }
        public int Is_Active { get; set; }
    }

    public class Config_AccessCode
    {
        public string Org_Id { get; set; }
        public string AccessCode_Id { get; set; }
        public string AccessCode_Name { get; set; }
        public int Is_Active { get; set; }
    }

    public class Config_GetAccessCode
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    // Config StaffType related Response models
    public class Config_StaffType
    {
        public string StaffType_Id { get; set; }
        public string StaffType_Name { get; set; }
        public int Is_Active { get; set; }
        public int Is_Deleted { get; set; }
        public string StaffDB_Name { get; set; }
        public string StaffView_Name { get; set; }
    }

    public class Menu_List
    {
        public string Menu_Id { get; set; }
        public string Menu_Name { get; set; }
        public int Is_Active { get; set; }
        public int Is_Deleted { get; set; }
        public string Parent_Menu_Id { get; set; }
        public string Parent_Menu_Name { get; set; }
        public string Menu_Icon_Name { get; set; }
        public string Menu_Tooltip { get; set; }
        public string Application_Id { get; set; }
        public decimal Display_Order_Number { get; set; }
        public string Functional_Owner { get; set; }
        public string Technical_Owner { get; set; }
        public string For_Devices { get; set; }
        public string Applicable_To { get; set; }

    }

    public class Notification_List
    {      
        public string Org_Id { get; set; }
        public string Notification_Id { get; set; }
        public string Notification_Type { get; set; }
        public string Notification_Text { get; set; }
        public string Staff_Type { get; set; }
        public DateTime Schedule_Start_Time { get; set; }
        public int Validity_Days { get; set; }
        public int Is_Active { get; set; }
        public int Is_Deleted { get; set; }
        public string Created_On { get; set; }
        public string Created_By { get; set; }
        public string LastEdited_On { get; set; }
        public string LastEdited_By { get; set; }
        public string Display_Schedule_Start_Time { get; set; }
    }


}
