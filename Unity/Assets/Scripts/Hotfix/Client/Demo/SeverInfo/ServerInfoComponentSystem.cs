namespace ET.Client
{
    [EntitySystemOf(typeof(ServerInfoComponent))]
    [FriendOfAttribute(typeof(ET.Client.ServerInfoComponent))]
    public static partial class ServerInfoComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.ServerInfoComponent self)
        {
            self.ServerInfoList.Clear();
        }

        [EntitySystem]
        private static void Destroy(this ET.Client.ServerInfoComponent self)
        {
            foreach (var serverInfoRef in self.ServerInfoList)
            {
                ServerInfo serverInfo = serverInfoRef;
                serverInfo?.Dispose();
            }
            self.ServerInfoList.Clear();
        }

        public static void AddServerInfo(this ServerInfoComponent self, ServerInfo serverInfo)
        {
            self.ServerInfoList.Add(serverInfo);
        }
    }
}

