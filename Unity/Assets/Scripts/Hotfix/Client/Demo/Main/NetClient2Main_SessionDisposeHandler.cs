namespace ET.Client
{
    [MessageHandler(SceneType.All)]
    public class NetClient2Main_SessionDisposeHandler: MessageHandler<Scene, NetClient2Main_SessionDispose>
    {
        protected override async ETTask Run(Scene entity, NetClient2Main_SessionDispose message)
        {
            if (message.Error != ErrorCode.ERR_Success)
            {
                Log.Error($"session dispose, error: {message.Error}");
            }
            await ETTask.CompletedTask;
        }
    }
}