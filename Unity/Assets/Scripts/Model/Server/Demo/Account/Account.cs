using System;

namespace ET.Model.Server
{

    public enum AccountType
    {
        General = 0,
        BlackList = 1,
    }
    
    [ChildOf(typeof(Session))]
    public class Account : Entity, IAwake
    {
        public string AccountName; //用户名
        public string Password; //密码
        public long CreatTime; //创建时间
        public AccountType AccountType; //账号类型
    }
}

