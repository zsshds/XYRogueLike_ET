namespace ET.Client
{
    [ComponentOf(typeof(Computer))]
    public class MonitorComponent : Entity, IAwake<int >
    {
        public int light;
    
    }
}