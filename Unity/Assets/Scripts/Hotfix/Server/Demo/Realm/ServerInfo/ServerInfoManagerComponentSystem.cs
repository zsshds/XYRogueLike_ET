namespace ET.Server
{
    [EntitySystemOf(typeof(ServerInfoManagerComponent))]
    [FriendOfAttribute(typeof(ET.Server.ServerInfoManagerComponent))]
    [FriendOfAttribute(typeof(ET.ServerInfo))]
    public static partial class ServerInfoManagerComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Server.ServerInfoManagerComponent self)
        {
            self.Load();
        }
        [EntitySystem]
        private static void Destroy(this ET.Server.ServerInfoManagerComponent self)
        {
            foreach (var serverInfoRef in self.ServerInfoList)
            {
                ServerInfo serverInfo = serverInfoRef;
                serverInfo?.Dispose();
            }
            self.ServerInfoList.Clear();
        }

        public static void Load(this ET.Server.ServerInfoManagerComponent self)
        {
            //先清理一下数据，准备重新加载
            foreach (var serverInfoRef in self.ServerInfoList)
            {
                ServerInfo serverInfo = serverInfoRef;
                serverInfo?.Dispose();
            }
            self.ServerInfoList.Clear();
            var serverInfoConfigs = StartZoneConfigCategory.Instance.GetAll();
            foreach (var info in serverInfoConfigs.Values)
            {
                if (info.ZoneType != 1)
                {
                    continue;
                }

                ServerInfo newServerInfo = self.AddChildWithId<ServerInfo>(info.Id);
                newServerInfo.ServerName = info.ZoneName;
                newServerInfo.Status = (int)ServerStatus.Normal;
                newServerInfo.DBName = info.DBName;
                self.ServerInfoList.Add(newServerInfo);
            }
        }
    }
}
