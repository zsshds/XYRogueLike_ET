/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.Login
{
    [EnableClass]
    public partial class FUI_ServerInfoButton: GButton
    {
        public GTextField Txt_Title;
        public const string URL = "ui://9q0q76hcnubc3";

        public static FUI_ServerInfoButton CreateInstance()
        {
            return (FUI_ServerInfoButton)UIPackage.CreateObject("Login", "ServerInfoButton");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);
            Txt_Title = (GTextField)GetChildAt(3);
        }
    }
}
