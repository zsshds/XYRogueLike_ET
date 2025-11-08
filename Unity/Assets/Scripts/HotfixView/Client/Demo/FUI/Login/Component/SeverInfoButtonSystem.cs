// using ET.Client.Login;
// namespace ET.Client
// {
//
//     [EntitySystemOf(typeof(SeverInfoButton))]
//     [FriendOfAttribute(typeof(ET.Client.SeverInfoButton))]
//     [FriendOfAttribute(typeof(ET.ServerInfo))]
//     public static partial class SeverInfoButtonSystem
//     {
//         [EntitySystem]
//         private static void Awake(this ET.Client.SeverInfoButton self)
//         {
//
//         }
//
//         public static void Refresh(this SeverInfoButton self, ServerInfo serverInfo)
//         {
//             self.ServerInfo = serverInfo;
//             self.FUIServerInfoButton.title = serverInfo.ServerName;
//             self.FUIServerInfoButton.AddListner(self.OnBtnClick);
//         }
//
//         private static void OnBtnClick(this SeverInfoButton self)
//         {
//             
//         }
//     }
// }
//
