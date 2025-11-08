namespace ET.Client
{
    [EntitySystemOf(typeof(LoginPanel))]
    [FriendOf(typeof(LoginPanel))]
    public static partial class LoginPanelSystem
    {
        [EntitySystem]
        private static void Awake(this LoginPanel self)
        {
            self.FUILoginPanel.Btn_Login.onClick.Add(self.OnLoginClick);
        }

        [EntitySystem]
        private static void Show(this LoginPanel self)
        {
        }
        
        private static void OnLoginClick(this LoginPanel self)
        {
            LoginHelper.LoginAndGetServerInfo(self.Root(), 
                self.FUILoginPanel.TxtIn_Account.text, 
                self.FUILoginPanel.TxtIn_Pasword.text).Coroutine();
        }
    }
}