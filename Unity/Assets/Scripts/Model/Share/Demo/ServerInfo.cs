namespace ET
{
    //定义在share目录下就不用写两遍了，这里真的能感受到框架的便捷
    public enum ServerStatus
    {
        Normal = 0,
        Stop = 1,
    }
    
    [ChildOf]
    public class ServerInfo : Entity, IAwake
    {
        public int Status;
        public string ServerName;
        public string DBName;
    }
}