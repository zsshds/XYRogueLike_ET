using ET.Client.Login;

namespace ET.Client
{
    [ComponentOf(typeof(FUIEntity))]
    [FUIPanel(PanelId.SelectServerPanel, "Login", "SelectServerPanel")]
    public class SelectServerPanel: Entity, IAwake, IShow
    {
        private FUI_SelectServerPanel _fuiSelectServerPanel;

        public FUI_SelectServerPanel FUISelectServerPanel
        {
            get => _fuiSelectServerPanel ??= (FUI_SelectServerPanel)this.GetParent<FUIEntity>().GComponent;
        }
    }
}
