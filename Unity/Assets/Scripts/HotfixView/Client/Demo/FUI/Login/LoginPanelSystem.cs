namespace ET.Client
{
    [EntitySystemOf(typeof(LoginPanel))]
    [FriendOf(typeof(LoginPanel))]
    public static partial class LoginPanelSystem
    {
        [EntitySystem]
        private static void Awake(this LoginPanel self)
        {
        }

        [EntitySystem]
        private static void Show(this LoginPanel self)
        {
        }
    }
}