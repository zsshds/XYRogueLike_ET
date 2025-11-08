// using ET.Client.Login;
//
// namespace ET.Client
// {
//     [ComponentOf(typeof(FUIEntity))]
//     //[ChildOf(typeof(FUIEntity))]
//     public class SeverInfoButton: Entity, IAwake, IShow
//     {
//         private FUI_ServerInfoButton _fuiSeverInfoButton;
//         public EntityRef<ServerInfo> ServerInfo;
//
//         public FUI_ServerInfoButton FUIServerInfoButton
//         {
//             get => _fuiSeverInfoButton ??= (FUI_ServerInfoButton)this.GetParent<FUIEntity>().GComponent;
//         }
//     }
// }