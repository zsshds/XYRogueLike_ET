using ET.Model.Server;

namespace ET.Server
{
    [EntitySystemOf(typeof(AccountSessionsComponent))]
    [FriendOfAttribute(typeof(ET.Model.Server.AccountSessionsComponent))]
    public static partial class AccountSessionsComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Model.Server.AccountSessionsComponent self)
        {

        }
        [EntitySystem]
        private static void Destroy(this ET.Model.Server.AccountSessionsComponent self)
        {
            self.AccountSessionDictionary.Clear();
        }

        public static Session Get(this AccountSessionsComponent self, string accountName)
        {
            if (!self.AccountSessionDictionary.TryGetValue(accountName, out EntityRef<Session> session))
            {
                return null;
            }

            return session;
        }

        public static void Add(this AccountSessionsComponent self, string accountName, EntityRef<Session> session)
        {
            if (self.AccountSessionDictionary.ContainsKey(accountName))
            {
                self.AccountSessionDictionary[accountName] = session;
            }
            else
            {
                self.AccountSessionDictionary.Add(accountName, session);
            }
        }

        public static void Remove(this AccountSessionsComponent self, string accountName)
        {
            if (self.AccountSessionDictionary.ContainsKey(accountName))
            {
                self.AccountSessionDictionary.Remove(accountName);
            }
        }
    }
}