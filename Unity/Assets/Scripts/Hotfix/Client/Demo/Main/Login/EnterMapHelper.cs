using System;


namespace ET.Client
{
    [FriendOfAttribute(typeof(ET.RoleInfo))]
    [FriendOfAttribute(typeof(ET.Client.AccountComponent))]
    public static partial class EnterMapHelper
    {
        public static async ETTask EnterMapAsync(Scene root)
        {
            try
            {
                //这里session链接的是gate网关服务器，现在要创建的是在map服务器中的unit映射对象实体
                G2C_EnterMap g2CEnterMap = await root.GetComponent<ClientSenderComponent>().Call(C2G_EnterMap.Create()) as G2C_EnterMap;

                // 等待场景切换完成
                await root.GetComponent<ObjectWait>().Wait<Wait_SceneChangeFinish>();

                EventSystem.Instance.Publish(root, new EnterMapFinish());
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static async ETTask EnterMap(Scene root)
        {
            try
            {
                string account = root.GetComponent<AccountComponent>().Account;
                string Token = root.GetComponent<AccountComponent>().Token;
                ClientSenderComponent clientSenderComponent = root.GetComponent<ClientSenderComponent>();
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

                ServerInfosProto serverInfosProto = r2CGetServerInfos.ServerInfosList[0];
                Log.Info($"请求服务器列表成功，区服名称：{serverInfosProto.ServerName} 区服ID：{serverInfosProto.Id}");
                
                //获取区服角色列表
                C2R_GetRoles c2RGetRoles = C2R_GetRoles.Create();
                c2RGetRoles.Token = Token;
                c2RGetRoles.Account = account;
                c2RGetRoles.ServerId = serverInfosProto.Id;
                R2C_GetRoles r2CGetRoles= await clientSenderComponent.Call(c2RGetRoles) as R2C_GetRoles;
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
                    c2RCreateRole.ServerId = serverInfosProto.Id;
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
                c2RGetRealmKey.ServerId = serverInfosProto.Id;
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
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static async ETTask Match(Fiber fiber)
        {
            try
            {
                G2C_Match g2CEnterMap = await fiber.Root.GetComponent<ClientSenderComponent>().Call(C2G_Match.Create()) as G2C_Match;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}