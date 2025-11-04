using ET.Model.Server;

namespace ET.Server
{
    [Invoke(TimerInvokeType.AccountSessionCheckOutTimer)]
    public class AccountCheckOutTimer : ATimer<AccountCheckOutTimeComponent>
    {
        protected override void Run(AccountCheckOutTimeComponent t)
        {
            t.DeleteSession();
        }
    }


    [EntitySystemOf(typeof(AccountCheckOutTimeComponent))]
    [FriendOfAttribute(typeof(ET.Server.AccountCheckOutTimeComponent))]
    public static partial class AccountCheckOutTimeComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Server.AccountCheckOutTimeComponent self, string accountName)
        {
            self.AccountName = accountName;
            self.Root().GetComponent<TimerComponent>().Remove(ref self.Timer);
            self.Timer = self.Root().GetComponent<TimerComponent>()
                    .NewOnceTimer(TimeInfo.Instance.ServerNow() + 600000, TimerInvokeType.AccountSessionCheckOutTimer, self);
        }

        [EntitySystem]
        private static void Destroy(this ET.Server.AccountCheckOutTimeComponent self)
        {
            self.Root().GetComponent<TimerComponent>().Remove(ref self.Timer);
        }
        
        public static void DeleteSession(this ET.Server.AccountCheckOutTimeComponent self)
        {
            Session session = self.GetParent<Session>();
            Session originSession = session.Root().GetComponent<AccountSessionsComponent>().Get(self.AccountName);
            if (session != null && session.InstanceId == originSession.InstanceId)
            {
                session.Root().GetComponent<AccountSessionsComponent>().Remove(self.AccountName);
            }
            A2C_Disconnect a2CDisconnect = A2C_Disconnect.Create();
            a2CDisconnect.Error = 1;
            session?.Send(a2CDisconnect);
            session?.Disconnect().Coroutine();
        }
    }
}
