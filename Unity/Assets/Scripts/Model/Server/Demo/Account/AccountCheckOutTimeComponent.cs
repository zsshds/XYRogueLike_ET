namespace ET.Server
{
    //会话管理，如果长时间不操作，会删除session，使用一次性定时器实现
    [ComponentOf(typeof(Session))]
    public class AccountCheckOutTimeComponent : Entity, IAwake<string>, IDestroy
    {
        public long Timer = 0;
        public string AccountName;
    }
}