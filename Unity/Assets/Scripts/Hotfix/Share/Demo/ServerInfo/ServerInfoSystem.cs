namespace ET
{
    [EntitySystemOf(typeof(ServerInfo))]
    [FriendOfAttribute(typeof(ET.ServerInfo))]
    public static partial class ServerInfoSystem
    {
        [EntitySystem]
        private static void Awake(this ET.ServerInfo self)
        {

        }

        public static void FromMessage(this ServerInfo self, ServerInfosProto serverInfosProto)
        {
            self.Status = serverInfosProto.Status;
            self.ServerName = serverInfosProto.ServerName;
            self.DBName = serverInfosProto.DBName;
        }

        public static ServerInfosProto ToMessage(this ServerInfo self)
        {
            ServerInfosProto serverInfosProto = ServerInfosProto.Create();
            serverInfosProto.Id = (int)self.Id;
            serverInfosProto.ServerName = self.ServerName;
            serverInfosProto.DBName = self.DBName;
            serverInfosProto.Status = self.Status;
            return serverInfosProto;
        }
    }
}