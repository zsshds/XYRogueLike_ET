namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_LoginGameGateHandler : MessageSessionHandler<C2G_LoginGameGate, G2C_LoginGameGate>
    {
        protected override async ETTask Run(Session session, C2G_LoginGameGate request, G2C_LoginGameGate response)
        {
            Scene root = session.Root();
            if (session.GetComponent<SessionLockingComponent>() != null)
            {
                response.Error = ErrorCode.ERR_RequestRepeatedly;
                return;
            }
            
            string account = session.Root().GetComponent<GateSessionKeyComponent>().Get(request.Key);
            if (account == null)
            {
                response.Error = ErrorCode.ERR_ConnectGateKetError;
                response.Message = "Gate key验证失败!";
                session.Disconnect().Coroutine();
                return;
            }
            long instanceId = session.InstanceId;
            
            root.GetComponent<GateSessionKeyComponent>().Remove(request.Key);
            session.RemoveComponent<SessionAcceptTimeoutComponent>();
            CoroutineLockComponent coroutineLockComponent = root.GetComponent<CoroutineLockComponent>();
            using (session.AddComponent<SessionLockingComponent>())
            {
                using (await coroutineLockComponent.Wait(CoroutineLockType.LoginGate, request.Account.GetLongHashCode()))
                {
                    //instanceId检查,如果有问题说明session可能已经被销毁
                    if (instanceId != session.InstanceId)
                    {
                        response.Error = ErrorCode.ERR_SessionIdError;
                        return;
                    }
                    //通知登录中心服，记录本次登录的zone
                    G2L_AddLoginRecord g2LAddLoginRecord = G2L_AddLoginRecord.Create();
                    g2LAddLoginRecord.Account = account;
                    g2LAddLoginRecord.serverId = root.Zone();
                    L2G_AddLoginRecord l2GAddLoginRecord = await root.GetComponent<MessageSender>().Call(StartSceneConfigCategory.Instance.LoginCenterConfig.ActorId, g2LAddLoginRecord) as L2G_AddLoginRecord ;
                    if (l2GAddLoginRecord.Error != ErrorCode.ERR_Success)
                    {
                        response.Error = l2GAddLoginRecord.Error;
                        session.Disconnect().Coroutine();
                        return;
                    }

                    PlayerComponent playerComponent = root.GetComponent<PlayerComponent>();
                    Player player = playerComponent.GetByAccount(account);
                    if (player == null)
                    {
                        player = playerComponent.AddChildWithId<Player, string>(request.RoleId, account);
                        player.UnitId = request.RoleId;
                        playerComponent.Add(player);
                        PlayerSessionComponent playerSessionComponent = player.AddComponent<PlayerSessionComponent>();
                        playerSessionComponent.AddComponent<MailBoxComponent, MailBoxType>(MailBoxType.GateSession);
                        await playerSessionComponent.AddLocation(LocationType.GateSession);
                        
                        player.AddComponent<MailBoxComponent, MailBoxType>(MailBoxType.UnOrderedMessage);
                        //将该player的位置告知location服务器
                        await player.AddLocation(LocationType.Player);
                        
                        session.AddComponent<SessionPlayerComponent>().Player = player;
                        playerSessionComponent.Session = session;
                        
                        player.playerState = PlayerState.Gate;
                    }
                    else
                    {
                        player.RemoveComponent<PlayerOfflineOutTimeComponent>();
                        
                        session.AddComponent<SessionPlayerComponent>().Player = player;
                        player.GetComponent<PlayerSessionComponent>().Session = session;
                    }

                    response.PlayerId = player.Id;
                }
            }
        }
    }
}

