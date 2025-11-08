/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.Login
{
    [EnableClass]
    public partial class FUI_SelectServerPanel: GComponent
    {
        public GList List_ServerBtn;
        public const string URL = "ui://9q0q76hci0ha2";

        public static FUI_SelectServerPanel CreateInstance()
        {
            return (FUI_SelectServerPanel)UIPackage.CreateObject("Login", "SelectServerPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);
            List_ServerBtn = (GList)GetChildAt(1);
        }
    }
}
