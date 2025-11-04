namespace ET.Server
{
    [ComponentOf(typeof(Player))]
    public class PlayerOfflineOutTimeComponent : Entity, IAwake, IDestroy
    {
        //作为定时器任务ID
        public long Timer;
    }
}

