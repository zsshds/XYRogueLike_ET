using System.Collections.Generic;

namespace ET.Server
{
    [MessageSessionHandler(SceneType.Realm)]
    [FriendOfAttribute(typeof(ET.RoleInfo))]
    public class C2R_CreateRoleHandler : MessageSessionHandler<C2R_CreateRole, R2C_CreatRole>
    {
        protected override async ETTask Run(Session session, C2R_CreateRole request, R2C_CreatRole response)
        {
            if (session.GetComponent<SessionLockingComponent>() != null)
            {
                response.Error = ErrorCode.ERR_RequestRepeatedly;
                session.Disconnect().Coroutine();
                return;
            }

            string Token = session.Root().GetComponent<TokenComponent>().Get(request.Account);
            if (Token == null || Token != request.Token)
            {
                response.Error = ErrorCode.ERR_TokenError;
                session.Disconnect().Coroutine();
                return;
            }

            if (string.IsNullOrEmpty(request.Name))
            {
                response.Error = ErrorCode.ERR_RoleNameNull;
                session.Disconnect().Coroutine();
                return;
            }

            CoroutineLockComponent coroutineLockComponent = session.Root().GetComponent<CoroutineLockComponent>();
            using (session.AddComponent<SessionLockingComponent>())
            {
                using (await coroutineLockComponent.Wait(CoroutineLockType.CreateRole, request.Account.GetLongHashCode()))
                {
                    DBComponent dbComponent = session.Root().GetComponent<DBManagerComponent>().GetZoneDB(session.Zone());
                    List<RoleInfo> roleInfos = await dbComponent.Query<RoleInfo>(roleinfo =>
                           roleinfo.Name == request.Name &&
                           roleinfo.ServerId == request.ServerId);
                    if (roleInfos != null && roleInfos.Count > 0)
                    {
                        response.Error = ErrorCode.ERR_RoleNameSame;
                        return;
                    }
                    //创建角色
                    RoleInfo newRoleInfo = session.AddChild<RoleInfo>();
                    newRoleInfo.Name = request.Name;
                    newRoleInfo.Account = request.Account;
                    newRoleInfo.ServerId = request.ServerId;
                    newRoleInfo.State = (int)RoleIndoState.Normal;
                    newRoleInfo.CreateTime = TimeInfo.Instance.ServerNow();
                    newRoleInfo.lastLoginTime = 0;
                    await dbComponent.Save<RoleInfo>(newRoleInfo);
                    //返回数据
                    response.roleInfo = newRoleInfo.ToMessage();
                    newRoleInfo?.Dispose();
                }
            }
        }
    }
}