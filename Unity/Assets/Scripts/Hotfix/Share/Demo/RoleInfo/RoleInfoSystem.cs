namespace ET
{
    [EntitySystemOf(typeof(RoleInfo))]
    [FriendOfAttribute(typeof(ET.RoleInfo))]
    public static partial class RoleInfoSystem
    {
        [EntitySystem]
        private static void Awake(this ET.RoleInfo self)
        {

        }

        public static void FromMessage(this RoleInfo self, RoleInfoProto roleInfoProto)
        {
            self.Name = roleInfoProto.Name;
            self.State = roleInfoProto.State;
            self.Account = roleInfoProto.Account;
            self.ServerId = roleInfoProto.ServerId;
            self.CreateTime = roleInfoProto.CreateTime;
            self.lastLoginTime = roleInfoProto.LastLoginTime;
        }

        public static RoleInfoProto ToMessage(this RoleInfo self)
        {
            RoleInfoProto roleInfoProto = RoleInfoProto.Create();
            roleInfoProto.Name = self.Name;
            roleInfoProto.Account = self.Account;
            roleInfoProto.ServerId = self.ServerId;
            roleInfoProto.Id = self.Id;
            roleInfoProto.CreateTime = self.CreateTime;
            roleInfoProto.LastLoginTime = self.lastLoginTime;
            roleInfoProto.State = self.State;
            
            return roleInfoProto;
        }
    }
}

