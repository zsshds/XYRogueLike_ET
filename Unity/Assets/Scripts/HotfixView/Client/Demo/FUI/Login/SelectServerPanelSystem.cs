namespace ET.Client
{
    [EntitySystemOf(typeof(SelectServerPanel))]
    [FriendOf(typeof(SelectServerPanel))]
    public static partial class SelectServerPanelSystem
    {
        [EntitySystem]
        private static void Awake(this SelectServerPanel self)
        {
        }

        [EntitySystem]
        private static void Show(this SelectServerPanel self)
        {
        }
    }
}