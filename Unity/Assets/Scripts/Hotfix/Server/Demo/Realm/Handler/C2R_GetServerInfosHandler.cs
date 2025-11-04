using ET.Server;

namespace ET.Server
{

    [MessageSessionHandler(SceneType.Realm)]
    [FriendOfAttribute(typeof(ET.Server.ServerInfoManagerComponent))]
    public class C2R_GetServerInfosHandler : MessageSessionHandler<C2R_GetServerInfos, R2C_GetServerInfos>
    {
        protected override async ETTask Run(Session session, C2R_GetServerInfos request, R2C_GetServerInfos response)
        {
            int a = session.Root().GetComponent<ServerInfoManagerComponent>().ServerInfoList.Count;
            Log.Info("当前可选服务器数量" + a);
            //判断合法性
            string Token = session.Root().GetComponent<TokenComponent>().Get(request.Account);
            if (Token == null || Token != request.Token)
            {
                response.Error = ErrorCode.ERR_TokenError;
                session?.Disconnect().Coroutine();
                return;
            }

            if (session.Root().GetComponent<ServerInfoManagerComponent>().ServerInfoList.Count > 0)
            {
                //获取服务器列表
                foreach (var serverInfoRef in session.Root().GetComponent<ServerInfoManagerComponent>().ServerInfoList)
                {
                    ServerInfo serverInfo = serverInfoRef;
                    response.ServerInfosList.Add(serverInfo.ToMessage());
                }
            }
            

            await ETTask.CompletedTask;
        }
    }
}