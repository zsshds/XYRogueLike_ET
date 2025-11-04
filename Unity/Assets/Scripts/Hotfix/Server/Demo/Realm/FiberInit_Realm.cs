using System.Net;
using ET.Model.Server;

namespace ET.Server
{
    [Invoke((long)SceneType.Realm)]
    public class FiberInit_Realm: AInvokeHandler<FiberInit, ETTask>
    {
        public override async ETTask Handle(FiberInit fiberInit)
        {
            Scene root = fiberInit.Fiber.Root;
            root.AddComponent<MailBoxComponent, MailBoxType>(MailBoxType.UnOrderedMessage);
            root.AddComponent<TimerComponent>();
            root.AddComponent<CoroutineLockComponent>();
            root.AddComponent<ProcessInnerSender>();
            root.AddComponent<MessageSender>();
            StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.Get(root.Fiber.Id);
            root.AddComponent<NetComponent, IPEndPoint, NetworkProtocol>(startSceneConfig.InnerIPPort, NetworkProtocol.UDP);
            //添加数据库管理组件
            root.AddComponent<DBManagerComponent>();
            //添加一个用户和session的映射组件
            root.AddComponent<AccountSessionsComponent>();
            //添加token管理组件
            root.AddComponent<TokenComponent>();
            //添加区服信息管理组件
            root.AddComponent<ServerInfoManagerComponent>();
            
            await ETTask.CompletedTask;
        }
    }
}