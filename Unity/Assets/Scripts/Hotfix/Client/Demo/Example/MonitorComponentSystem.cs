namespace ET.Client
{
    [EntitySystemOf(typeof(MonitorComponent))]
    [FriendOfAttribute(typeof(ET.Client.MonitorComponent))] //访问组件的属性，需要加这个友好标签
    public static partial class MonitorComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.MonitorComponent self, int args2)
        {
            Log.Debug("Monitor awake");
            self.light = args2;
        }

        public static void ChangeLight(this MonitorComponent self, int light)
        {
            self.light = light;
            Log.Debug("monitor light = " + self.light);
        }
    }
}

