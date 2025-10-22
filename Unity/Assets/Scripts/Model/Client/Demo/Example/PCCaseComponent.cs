namespace ET.Client
{
    [ComponentOf(typeof(Computer))] //标明该组件只会挂在在Computer实体上
    public class PCCaseComponent : Entity, IAwake
    {
    
    }
}

