namespace ET.Server
{
    [MessageHandler(SceneType.LoginCenter)]
    public class G2L_RemoveLoginRecordHandler : MessageHandler<Scene, G2L_RemoveLoginRecord, L2G_RemoveLoginRecord>
    {
        protected override async ETTask Run(Scene scene, G2L_RemoveLoginRecord request, L2G_RemoveLoginRecord response)
        {
            int zone = scene.GetComponent<LoginInfoRecordComponent>().Get(request.Account.GetLongHashCode());
            if (request.serverId == zone)
            {
                scene.GetComponent<LoginInfoRecordComponent>().Remove(request.Account.GetLongHashCode());
            }
            
            await ETTask.CompletedTask;
        }
    }
}

