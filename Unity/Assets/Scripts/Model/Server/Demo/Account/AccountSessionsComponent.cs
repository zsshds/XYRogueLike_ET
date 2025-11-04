using System.Collections.Generic;

namespace ET.Model.Server
{
    [ComponentOf(typeof(Scene))]
    public class AccountSessionsComponent : Entity, IAwake, IDestroy
    {
        //做这样的一个session字典是为了判断是否有其他的玩家使用同样的账号登录过了
        public Dictionary<string, EntityRef<Session>> AccountSessionDictionary = new Dictionary<string, EntityRef<Session>>();
    }   
}