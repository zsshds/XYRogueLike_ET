namespace ET.Server
{
    [EntitySystemOf(typeof(LoginInfoRecordComponent))]
    [FriendOfAttribute(typeof(ET.Server.LoginInfoRecordComponent))]
    public static partial class LoginInfoRecordComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Server.LoginInfoRecordComponent self)
        {

        }
        [EntitySystem]
        private static void Destroy(this ET.Server.LoginInfoRecordComponent self)
        {
            self.AccountLoginInfoDictionary.Clear();
        }

        public static void Add(this LoginInfoRecordComponent self, long key, int value)
        {
            if (self.AccountLoginInfoDictionary.ContainsKey(key))
            {
                self.AccountLoginInfoDictionary[key] = value;
                return;
            }
            self.AccountLoginInfoDictionary.Add(key, value);
        }

        public static void Remove(this LoginInfoRecordComponent self, long key)
        {
            if (self.AccountLoginInfoDictionary.ContainsKey(key))
            {
                self.AccountLoginInfoDictionary.Remove(key);
            }
        }

        public static int Get(this LoginInfoRecordComponent self, long key)
        {
            if (self.AccountLoginInfoDictionary.ContainsKey(key))
            {
                return self.AccountLoginInfoDictionary[key];
            }

            return -1;
        }

        public static bool IsExist(this LoginInfoRecordComponent self, long key)
        {
            return self.AccountLoginInfoDictionary.ContainsKey(key);
        }
    }
}

