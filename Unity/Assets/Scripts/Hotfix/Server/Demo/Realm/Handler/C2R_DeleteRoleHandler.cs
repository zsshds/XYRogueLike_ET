using System.Collections.Generic;

namespace ET.Server
{
    [MessageSessionHandler(SceneType.Realm)]
    [FriendOfAttribute(typeof(ET.RoleInfo))]
    public class C2R_DeleteRoleHandler : MessageSessionHandler<C2R_DeleteRole, R2C_DeleteRole>
    {
        protected override async ETTask Run(Session session, C2R_DeleteRole request, R2C_DeleteRole response)
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
            CoroutineLockComponent coroutineLockComponent = session.Root().GetComponent<CoroutineLockComponent>();
            using (session.AddComponent<SessionLockingComponent>())
            {
                using (await coroutineLockComponent.Wait(CoroutineLockType.CreateRole, request.Account.GetLongHashCode()))
                {
                    DBComponent dbComponent = session.Root().GetComponent<DBManagerComponent>().GetZoneDB(session.Zone());
                    List<RoleInfo> roleInfos = await dbComponent.Query<RoleInfo>(roleinfo =>
                            roleinfo.Id == request.RoleInfoId &&
                            roleinfo.ServerId == request.ServerId);
                    if (roleInfos != null && roleInfos.Count > 0)
                    {
                        response.Error = ErrorCode.ERR_RoleNotExist;
                        return;
                    }
                    //删除只是逻辑上的，实际上，我只会将角色状态改为删除
                    RoleInfo deleteRoleInfo = roleInfos[0];
                    session.AddChild(deleteRoleInfo);
                    deleteRoleInfo.State = (int)RoleIndoState.Delete;
                    await dbComponent.Save(deleteRoleInfo);
                    response.DeleteRoleInfoId = deleteRoleInfo.Id;
                    deleteRoleInfo?.Dispose();
                }
            }
        }
    }
}
