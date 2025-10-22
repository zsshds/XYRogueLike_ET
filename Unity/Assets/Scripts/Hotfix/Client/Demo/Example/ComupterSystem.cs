namespace ET.Client
{
    [EntitySystemOf(typeof(Computer))]
    public static partial class ComupterSystem
    {
        //这里是通过C#拓展方法去实现的，也就是说System会对对应实体增加拓展
        //在这里查看调用，会发现源代码自动生成器，生成了一个类。这个技术是.net自带的技术
        [EntitySystem]
        private static void Awake(this ET.Client.Computer self)
        {
            Log.Debug("computer awake");
        }
        [EntitySystem]
        private static void Update(this ET.Client.Computer self)
        {
            Log.Debug("computer update");
        }
        [EntitySystem]
        private static void Destroy(this ET.Client.Computer self)
        {
            Log.Debug("computer destroy");
        }

        public static void Open(this Computer self)
        {
            Log.Debug("computer open");
        }
    }
}