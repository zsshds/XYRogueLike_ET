namespace ET.Client
{
    [FriendOfAttribute(typeof(ET.ServerInfo))]
    [FriendOfAttribute(typeof(ET.Client.AccountComponent))]
    public static class LoginHelper
    {
        public static async ETTask LoginAndGetServerInfo(Scene root, string account, string password)
        {
            //先移除
            root.RemoveComponent<ClientSenderComponent>();
            //再添加，这个组件是帮助游戏客户端Main fiber 和游戏服务器端创建链接的组件
            //这样先移除后添加是为了保障，每次登录clientSenderComponent都是一个全新的组件，并且跟之前的登录链接没有任何关系
            ClientSenderComponent clientSenderComponent = root.AddComponent<ClientSenderComponent>();
            //唯一ID，clientSenderComponent通访问RouterManager路由管理服务，获取router服务器节点列表，选择一个router
            //选择router后通过Realm网关负载均衡服务器，选择一个Gate网关服务，在Gate网关服务器中创建player映射实体，返回playerId给客户端
            NetClient2Main_Login response = await clientSenderComponent.LoginAsync(account, password);
            if (response.Error != ErrorCode.ERR_Success)
            {
                Log.Error($"response Erroe : {response.Error}");
                return;
            }
            Log.Info($"========= {account} 已经登录成功 =========");
            string Token = response.ToKen;

            //创建account组件
            AccountComponent accountComponent = root.AddComponent<AccountComponent>();
            accountComponent.SetLoginMapInfo(Token, account);

            //登录后，获取服务器列表
            C2R_GetServerInfos c2RGetServerInfos = C2R_GetServerInfos.Create();
            c2RGetServerInfos.Account = account;
            c2RGetServerInfos.Token = Token;
            //这里直接使用Call函数的原因是，session已经被创建并保留下来了
            R2C_GetServerInfos r2CGetServerInfos = await clientSenderComponent.Call(c2RGetServerInfos) as R2C_GetServerInfos;
            if (r2CGetServerInfos.Error != ErrorCode.ERR_Success)
            {
                Log.Error("请求服务器列表失败");
                return;
            }

            ServerInfoComponent serverInfoComponent = root.AddComponent<ServerInfoComponent>();
            foreach (var serverInfoProto in r2CGetServerInfos.ServerInfosList)
            {
                ServerInfo serverInfo = serverInfoComponent.AddChildWithId<ServerInfo>(serverInfoProto.Id);
                serverInfo.FromMessage(serverInfoProto);
                serverInfoComponent.AddServerInfo(serverInfo);
                Log.Info($"区服名称：{serverInfoProto.ServerName} 区服ID：{serverInfoProto.Id}, 状态：{serverInfoProto.Status}, 数据库名称：{serverInfoProto.DBName}");
            }



            // //获取区服角色列表
            // C2R_GetRoles c2RGetRoles = C2R_GetRoles.Create();
            // c2RGetRoles.Token = Token;
            // c2RGetRoles.Account = account;
            // c2RGetRoles.ServerId = serverInfosProto.Id;
            // R2C_GetRoles r2CGetRoles= await clientSenderComponent.Call(c2RGetRoles) as R2C_GetRoles;
            // if (r2CGetRoles.Error != ErrorCode.ERR_Success)
            // {
            //     Log.Error("请求角色信息失败！");
            //     return;
            // }
            //
            // RoleInfoProto roleInfoProto = default;
            // if (r2CGetRoles.RoleInfo.Count <= 0)
            // {
            //     //无角色，创建角色
            //     C2R_CreateRole c2RCreateRole = C2R_CreateRole.Create();
            //     c2RCreateRole.Account = account;
            //     c2RCreateRole.Token = Token;
            //     c2RCreateRole.ServerId = serverInfosProto.Id;
            //     c2RCreateRole.Name = account;
            //     R2C_CreatRole r2CCreatRole = await clientSenderComponent.Call(c2RCreateRole) as R2C_CreatRole;
            //     if (r2CCreatRole.Error != ErrorCode.ERR_Success)
            //     {
            //         Log.Error("创建角色失败！");
            //         return;
            //     }
            //
            //     roleInfoProto = r2CCreatRole.roleInfo;
            // }
            // else
            // {
            //     roleInfoProto = r2CGetRoles.RoleInfo[0];
            // }
            //
            // //请求获取RealmKey
            // C2R_GetRealmKey c2RGetRealmKey = C2R_GetRealmKey.Create();
            // c2RGetRealmKey.ServerId = serverInfosProto.Id;
            // c2RGetRealmKey.Account = account;
            // c2RGetRealmKey.Token = Token;
            // R2C_GetRealmKey r2CGetRealmKey = await clientSenderComponent.Call(c2RGetRealmKey) as R2C_GetRealmKey;
            // if (r2CGetRealmKey.Error != ErrorCode.ERR_Success)
            // {
            //     Log.Error("获取RealmKey失败！");
            //     return;
            // }
            //
            // //创建account组件
            // AccountComponent accountComponent = root.AddComponent<AccountComponent>();
            // accountComponent.SetRealmInfo(r2CGetRealmKey.Address, r2CGetRealmKey.Key);
            // //添加roleInfo组件
            // RoleInfo roleInfo = accountComponent.AddChildWithId<RoleInfo>(0);
            // roleInfo.FromMessage(roleInfoProto);

            // NetClient2Main_LoginGame netClient2MainLoginGame =
            //         await clientSenderComponent.LoginGameAsync(account, r2CGetRealmKey.Key, roleInfoProto.Id, r2CGetRealmKey.Address);
            // if (netClient2MainLoginGame.Error != ErrorCode.ERR_Success)
            // {
            //     Log.Error("登录游戏失败！");
            //     return;
            // }
            // Log.Info("登录游戏成功");
            // //记录ID,这里的PlayerComponent是游戏客户端的，和服务器端不是一种
            // root.GetComponent<PlayerComponent>().MyId = response.PlayerId;
            //抛出登录结束事件
            //记录ID,这里的PlayerComponent是游戏客户端的，和服务器端不是一种
            root.GetComponent<PlayerComponent>().MyId = response.PlayerId;
            await EventSystem.Instance.PublishAsync(root, new LoginFinish());
        }

        public static async ETTask EnterGame(Scene root, ServerInfosProto erverInfosProto)
        {
            ClientSenderComponent clientSenderComponent = root.GetComponent<ClientSenderComponent>();
            string Token = root.GetComponent<AccountComponent>().Token;
            string account = root.GetComponent<AccountComponent>().Account;
            //获取区服角色列表
            C2R_GetRoles c2RGetRoles = C2R_GetRoles.Create();
            c2RGetRoles.Token = Token;
            c2RGetRoles.Account = account;
            c2RGetRoles.ServerId = erverInfosProto.Id;
            R2C_GetRoles r2CGetRoles = await clientSenderComponent.Call(c2RGetRoles) as R2C_GetRoles;
            if (r2CGetRoles.Error != ErrorCode.ERR_Success)
            {
                Log.Error("请求角色信息失败！");
                return;
            }

            RoleInfoProto roleInfoProto = default;
            if (r2CGetRoles.RoleInfo.Count <= 0)
            {
                //无角色，创建角色
                C2R_CreateRole c2RCreateRole = C2R_CreateRole.Create();
                c2RCreateRole.Account = account;
                c2RCreateRole.Token = Token;
                c2RCreateRole.ServerId = erverInfosProto.Id;
                c2RCreateRole.Name = account;
                R2C_CreatRole r2CCreatRole = await clientSenderComponent.Call(c2RCreateRole) as R2C_CreatRole;
                if (r2CCreatRole.Error != ErrorCode.ERR_Success)
                {
                    Log.Error("创建角色失败！");
                    return;
                }

                roleInfoProto = r2CCreatRole.roleInfo;
            }
            else
            {
                roleInfoProto = r2CGetRoles.RoleInfo[0];
            }

            //请求获取RealmKey
            C2R_GetRealmKey c2RGetRealmKey = C2R_GetRealmKey.Create();
            c2RGetRealmKey.ServerId = erverInfosProto.Id;
            c2RGetRealmKey.Account = account;
            c2RGetRealmKey.Token = Token;
            R2C_GetRealmKey r2CGetRealmKey = await clientSenderComponent.Call(c2RGetRealmKey) as R2C_GetRealmKey;
            if (r2CGetRealmKey.Error != ErrorCode.ERR_Success)
            {
                Log.Error("获取RealmKey失败！");
                return;
            }
            NetClient2Main_LoginGame netClient2MainLoginGame =
                    await clientSenderComponent.LoginGameAsync(account, r2CGetRealmKey.Key, roleInfoProto.Id, r2CGetRealmKey.Address);
            if (netClient2MainLoginGame.Error != ErrorCode.ERR_Success)
            {
                Log.Error("登录游戏失败！");
                return;
            }
            Log.Info("登录游戏成功");
            // 等待场景切换完成
            await root.GetComponent<ObjectWait>().Wait<Wait_SceneChangeFinish>();
            EventSystem.Instance.Publish(root, new EnterMapFinish());
        }
    }
}