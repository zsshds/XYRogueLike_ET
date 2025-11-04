using System.Threading.Tasks;

namespace ET.Server
{
    public static class DisconnectHelper
    {
        public static async ETTask Disconnect(this Session self)
        {
            if (self == null || self.IsDisposed)
            {
                return;
            }

            long instanceId = self.InstanceId;
            TimerComponent timerComponent = self.Root().GetComponent<TimerComponent>();
            //等待一秒后删除会话 
            await timerComponent.WaitAsync(1000);
            //这里判断一下，在这一秒中是不是session用重新使用
            if (self.InstanceId != instanceId)
            {
                return;
            }
            self.Dispose();
        }

        public static async ETTask KickPlayer(Player player)
        {
            if (player == null || player.IsDisposed)
            {
                return;
            }
            long instanceId = player.InstanceId;
            CoroutineLockComponent coroutineLockComponent = player.Root().GetComponent<CoroutineLockComponent>();
            using (await coroutineLockComponent.Wait(CoroutineLockType.LoginGate, player.Account.GetLongHashCode()))
            {
                if (player.IsDisposed || instanceId != player.InstanceId)
                {
                    return;
                }

                await KickPlayerNoLock(player);
            }
        }

        private static async Task KickPlayerNoLock(Player player)
        {
            if (player == null || player.IsDisposed)
            {
                return;
            }

            switch (player.playerState)
            {
                case PlayerState.Disconnect:
                    break;
                case PlayerState.Gate: 
                    break;
                case PlayerState.Game:
                    //通知游戏逻辑服下线Unit角色逻辑，并将数据存入数据库
                    M2G_RequestExitGame m2GRequestExitGame = await player.Root().GetComponent<MessageLocationSenderComponent>().Get(LocationType.Unit).Call(player.UnitId, G2M_RequestExitGame.Create()) as M2G_RequestExitGame;
                    
                    //通知移除账号角色登录信息
                    G2L_RemoveLoginRecord g2LRemoveLoginRecord = G2L_RemoveLoginRecord.Create();
                    g2LRemoveLoginRecord.Account = player.Account;
                    g2LRemoveLoginRecord.serverId = player.Zone();
                    L2G_RemoveLoginRecord l2GRemoveLoginRecord = await player.Root().GetComponent<MessageSender>()
                            .Call(StartSceneConfigCategory.Instance.LoginCenterConfig.ActorId, g2LRemoveLoginRecord) as L2G_RemoveLoginRecord;
                    break;
            }
            TimerComponent timerComponent = player.Root().GetComponent<TimerComponent>();
            player.playerState = PlayerState.Disconnect;
            //移除Location服务器上的定位形象
            await player.GetComponent<PlayerSessionComponent>().RemoveLocation(LocationType.GateSession);
            await player.RemoveLocation(LocationType.Player);
            //移除角色数据
            player.Root().GetComponent<PlayerComponent>().Remove(player);
            player?.Dispose();
            
            await timerComponent.WaitAsync(300);
        }
    }
}

