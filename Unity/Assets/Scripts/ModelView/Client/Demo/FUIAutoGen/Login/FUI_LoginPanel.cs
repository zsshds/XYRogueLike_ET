/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.Login
{
    [EnableClass]
    public partial class FUI_LoginPanel: GComponent
    {
        public GTextInput TxtIn_Account;
        public GTextInput TxtIn_Pasword;
        public GGroup InputGroup;
        public ET.Client.Login.FUI_LoginButton Btn_Login;
        public const string URL = "ui://9q0q76hci0ha0";

        public static FUI_LoginPanel CreateInstance()
        {
            return (FUI_LoginPanel)UIPackage.CreateObject("Login", "LoginPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);
            TxtIn_Account = (GTextInput)GetChildAt(3);
            TxtIn_Pasword = (GTextInput)GetChildAt(6);
            InputGroup = (GGroup)GetChildAt(7);
            Btn_Login = (ET.Client.Login.FUI_LoginButton)GetChildAt(8);
        }
    }
}
