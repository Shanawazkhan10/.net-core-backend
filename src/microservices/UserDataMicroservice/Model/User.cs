using Middleware;
using System;

namespace UserDataMicroservice.Model
{
    public class Common_Command_Output
    {
        public int Result_Id { get; set; }
        public string Result_Description { get; set; }
        public string Result_Extra_Key { get; set; }
    }

    public class User_Details
    {
        public string User_Id { get; set; }
        public string User_Name { get; set; }
        public string Org_Id { get; set; }
        public string Org_Name { get; set; }
        public int Is_Authorised { get; set; }
        public string Config_String { get; set; }
        public string Eva_Config_String { get; set; }
        public string User_Email { get; set; } 
        public string User_Mobile { get; set; }
        public string Gender_Id { get; set; }
    }

    public class User_Emailid
    {
        public string User_Email { get; set; }
        public string User_Id { get; set; }
        public string User_Name { get; set; }
    }
    public class User_Menu
    {
        public string Menu_Id { get; set; }
        public string Menu_Name { get; set; }
        public int Menu_Level { get; set; }
        public string Parent_Menu_Id { get; set; }
        public decimal Display_Order_Number { get; set; }
        public string Menu_Link { get; set; }
        public string Menu_Icon_Name { get; set; }
        public string Menu_Tooltip { get; set; }
        public int Display_Flag { get; set; }
        public int Add_Flag { get; set; }
        public int Modify_Flag { get; set; }
        public int Delete_Flag { get; set; }
    }

    // Config Application related Response models
    public class Application_List
    {
        public string Application_Id { get; set; }
        public string Application_Name { get; set; }
        public int Is_Active { get; set; }
        public int Is_Deleted { get; set; }
    }

    // Master Role related Response models
    public class Role_List
    {
        public string Application_Id { get; set; }
        public string Role_Id { get; set; }
        public string Role_Name { get; set; }
        public int Is_Active { get; set; }
        public int Is_Deleted { get; set; }
    }

    // Master Role_Menu related Response models
    public class Role_Menu_List
    {
        public string Menu_Id { get; set; }
        public string Menu_Name { get; set; }
        public int Display_Flag { get; set; }
        public int Add_Flag { get; set; }
        public int Modify_Flag { get; set; }
        public int Delete_Flag { get; set; }
    }

    // Master User related Response models
    public class User_List
    {
        public string Application_Id { get; set; }
        public string User_Id { get; set; }
        public string User_Name { get; set; }
        public int Is_Active { get; set; }
        public int Is_Deleted { get; set; }
        public string Created_On { get; set; }
        public string Created_By { get; set; }
        public string LastEdited_On { get; set; }
        public string LastEdited_By { get; set; }
        public string Role_Id { get; set; }
        public string User_Display_Name { get; set; }
        public string Role_Name { get; set; }
        public string StaffType_Id { get; set; }
        public string User_Extra_Details { get; set; }
        public string AccessCode_Id { get; set; }
        public string Allowed_IP_Address { get; set; }
        public int Allow_Multi_Login { get; set; }
        public string User_Email { get; set; }
        public string User_Mobile { get; set; }
        public string AccessCode_Name { get; set; }
        public string StaffType_Name { get; set; }
    }

    // Master - New_User_List
    public class New_User_List
    {
        public int Result_Id { get; set; }
        public string Result_Description { get; set; }
        public string Result_Extra_Key { get; set; }
    }

    // Master User related Response models
    public class User_Application_Role_List
    {
        public string User_Id { get; set; }
        public string Application_Id { get; set; }
        public string Application_Name { get; set; }
        public string Role_Id { get; set; }
        public string Role_Name { get; set; }
    }

    public class User_Search_Output
    {
        public string Menu_Id { get; set; }
        public string Menu_Name { get; set; }
        public string Menu_toolTip { get; set; }
        public int AccessFlag { get; set; }

    }

    public class User_Profile_page_Output
    {
        public string User_Id { get; set; }
        public string User_Name { get; set; }
        public string Full_Name { get; set; }
        public string User_Password { get; set; }
        public string User_Email { get; set; }
        public string User_Mobile { get; set; }
        public string Created_On { get; set; }
        public string AccessCode_Name { get; set; }
        public string Stafftype_Name { get; set; }
        public string Logged_In { get; set; }
    }

}
