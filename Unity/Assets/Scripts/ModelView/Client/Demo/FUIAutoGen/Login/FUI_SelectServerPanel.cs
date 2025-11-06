/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.Login
{
    [EnableClass]
    public partial class FUI_SelectServerPanel: GComponent
    {
        public ET.Client.Login.FUI_LoginButton Btn_Select;
        public const string URL = "ui://9q0q76hci0ha2";

        public static FUI_SelectServerPanel CreateInstance()
        {
            return (FUI_SelectServerPanel)UIPackage.CreateObject("Login", "SelectServerPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);
            Btn_Select = (ET.Client.Login.FUI_LoginButton)GetChildAt(1);
        }
    }
}
