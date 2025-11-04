using System.Threading.Tasks;

namespace ET.Client
{
    [EntitySystemOf(typeof(ClientSenderComponent))]
    [FriendOf(typeof(ClientSenderComponent))]
    public static partial class ClientSenderComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ClientSenderComponent self)
        {

        }
        
        [EntitySystem]
        private static void Destroy(this ClientSenderComponent self)
        {
            self.RemoveFiberAsync().Coroutine();
        }

        private static async ETTask RemoveFiberAsync(this ClientSenderComponent self)
        {
            if (self.fiberId == 0)
            {
                return;
            }

            int fiberId = self.fiberId;
            self.fiberId = 0;
            await FiberManager.Instance.Remove(fiberId);
        }

        public static async ETTask DisposeAsync(this ClientSenderComponent self)
        {
            await self.RemoveFiberAsync();
            self.Dispose();
        }

        public static async ETTask<NetClient2Main_Login> LoginAsync(this ClientSenderComponent self, string account, string password)
        {
            //创建一个新的纤程，这个纤程就是客户端第二个纤程，NetClient。返回有一个fiberID标识纤程
            self.fiberId = await FiberManager.Instance.Create(SchedulerType.ThreadPool, 0, SceneType.NetClient, "");
            
            //进行纤程间消息传递，主纤程 2 NetClient
            
            //这里创建一个ActorId指向，具体进程，具体纤程，具体实体。p1当前纤程所在进程ID，p2，目标纤程ID。这个构造函数不需要传instanceId。已经默认指向，NetClient下root（instanceId = 1）实体了
            self.netClientActorId = new ActorId(self.Fiber().Process, self.fiberId);
            //更具proto创建一条网络消息
            Main2NetClient_Login main2NetClientLogin = Main2NetClient_Login.Create();
            main2NetClientLogin.OwnerFiberId = self.Fiber().Id;
            main2NetClientLogin.Account = account;
            main2NetClientLogin.Password = password;
            //ProcessInnerSender是一个消息队列
            NetClient2Main_Login response = await self.Root().GetComponent<ProcessInnerSender>().Call(self.netClientActorId, main2NetClientLogin) as NetClient2Main_Login;
            return response;
        }

        public static void Send(this ClientSenderComponent self, IMessage message)
        {
            A2NetClient_Message a2NetClientMessage = A2NetClient_Message.Create();
            a2NetClientMessage.MessageObject = message;
            self.Root().GetComponent<ProcessInnerSender>().Send(self.netClientActorId, a2NetClientMessage);
        }

        public static async ETTask<IResponse> Call(this ClientSenderComponent self, IRequest request, bool needException = true)
        {
            A2NetClient_Request a2NetClientRequest = A2NetClient_Request.Create();
            a2NetClientRequest.MessageObject = request;
            using A2NetClient_Response a2NetClientResponse = await self.Root().GetComponent<ProcessInnerSender>().Call(self.netClientActorId, a2NetClientRequest) as A2NetClient_Response;
            IResponse response = a2NetClientResponse.MessageObject;
                        
            if (response.Error == ErrorCore.ERR_MessageTimeout)
            {
                throw new RpcException(response.Error, $"Rpc error: request, 注意Actor消息超时，请注意查看是否死锁或者没有reply: {request}, response: {response}");
            }

            if (needException && ErrorCore.IsRpcNeedThrowException(response.Error))
            {
                throw new RpcException(response.Error, $"Rpc error: {request}, response: {response}");
            }
            return response;
        }

        public static async ETTask<NetClient2Main_LoginGame> LoginGameAsync(this ClientSenderComponent self, string account, long key, long roleId,
        string address)
        {
            Main2NetClient_LoginGame main2NetClientLoginGame = Main2NetClient_LoginGame.Create();
            main2NetClientLoginGame.Account = account;
            main2NetClientLoginGame.GateAddress = address;
            main2NetClientLoginGame.RoleId = roleId;
            main2NetClientLoginGame.RealmKey = key;
            NetClient2Main_LoginGame response = await self.Root().GetComponent<ProcessInnerSender>().Call(self.netClientActorId, main2NetClientLoginGame) as NetClient2Main_LoginGame;
            return response;
        }

    }
}