namespace ET
{
    public enum RoleIndoState
    {
        Normal = 0,
        
        Freeze = 100,
        Delete = 999,
    }
    
    [ChildOf]
    public class RoleInfo : Entity, IAwake
    {
        public string Name;
        public int ServerId;
        public int State;
        public string Account;
        public long lastLoginTime;
        public long CreateTime;
    }
}
