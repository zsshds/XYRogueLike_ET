using System.Collections.Generic;

namespace ET.Client
{
    [ComponentOf(typeof(Scene))]
    public class ServerInfoComponent : Entity, IAwake, IDestroy
    {
        public List<EntityRef<ServerInfo>> ServerInfoList = new List<EntityRef<ServerInfo>>();
    }
}

