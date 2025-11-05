namespace ET.Client
{
    [EntitySystemOf(typeof(AccountComponent))]
    [FriendOfAttribute(typeof(ET.Client.AccountComponent))]
    public static partial class AccountComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.AccountComponent self)
        {

        }

        public static void SetLoginMapInfo(this AccountComponent self, string Token, string Account)
        {
            self.Token = Token;
            self.Account = Account;
        }

        public static void SetRealmInfo(this AccountComponent self, string realmAddress, long realmKey)
        {
            //其他的信息会记录在RoleInfo中
            self.RealmAddress = realmAddress;
            self.RealmKey = realmKey;
        }
        
        [EntitySystem]
        private static void Destroy(this ET.Client.AccountComponent self)
        {
            
        }
    }
}

