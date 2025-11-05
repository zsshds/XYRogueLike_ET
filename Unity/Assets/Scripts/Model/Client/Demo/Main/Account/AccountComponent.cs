namespace ET.Client
{
    [ComponentOf(typeof(Scene))]
    public class AccountComponent : Entity, IAwake, IDestroy
    {
        //客户端Account组件，存储服务器下发的player，unit映射以及token等必要信息，这个组件有一个子组件是RoleInfo
        public string Token;
        public string Account;
        public string RealmAddress;
        public long RealmKey;
        public long RoleId;
    }
}

