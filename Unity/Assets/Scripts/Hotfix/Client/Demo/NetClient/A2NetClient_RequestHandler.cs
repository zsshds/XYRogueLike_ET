namespace ET.Client
{
    [MessageHandler(SceneType.NetClient)]
    public class A2NetClient_RequestHandler: MessageHandler<Scene, A2NetClient_Request, A2NetClient_Response>
    {
        protected override async ETTask Run(Scene root, A2NetClient_Request request, A2NetClient_Response response)
        {
            int rpcId = request.RpcId;
            SessionComponent sessionComponent = root.GetComponent<SessionComponent>();
            Session session = sessionComponent.Session;
            IResponse res = await session.Call(request.MessageObject);
            res.RpcId = rpcId;
            response.MessageObject = res;
        }
    }
}