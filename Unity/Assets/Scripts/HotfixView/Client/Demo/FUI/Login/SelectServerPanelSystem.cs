using System.Collections.Generic;
using ET.Client.Login;

namespace ET.Client
{
    [EntitySystemOf(typeof(SelectServerPanel))]
    [FriendOf(typeof(SelectServerPanel))]
    [FriendOfAttribute(typeof(ET.Client.ServerInfoComponent))]
    [FriendOfAttribute(typeof(ET.ServerInfo))]
    public static partial class SelectServerPanelSystem
    {
        [EntitySystem]
        private static void Awake(this SelectServerPanel self)
        {
        }

        [EntitySystem]
        private static void Show(this SelectServerPanel self)
        {
            self.Refresh();
        }

        private static void Refresh(this SelectServerPanel self)
        {
            ServerInfoComponent serverInfoComponent = self.Root().GetComponent<ServerInfoComponent>();
            List<EntityRef<ServerInfo>> serverInfos = serverInfoComponent.ServerInfoList;
            //设置列表渲染器，最好还是写函数
            self.FUISelectServerPanel.List_ServerBtn.itemRenderer = (index, obj) =>
            {
                //这里渲染器是为了每个元素设置表现和行为
                FUI_ServerInfoButton serverInfoButton = obj as FUI_ServerInfoButton;
                ServerInfo serverInfo = serverInfos[index];
                serverInfoButton.Txt_Title.text = serverInfo.ServerName;
                //serverInfoButton.ServerInfo = serverInfo;
                //添加点击事件
                serverInfoButton.onClick.Add(() =>
                {
                    LoginHelper.EnterGame(self.Root(), serverInfo.ToMessage()).Coroutine();
                });
            };
            self.FUISelectServerPanel.List_ServerBtn.numItems = serverInfos.Count; 
        }
    }
}