namespace ET.Server
{
    public enum PlayerState
    {
        Disconnect = 99,
        Gate = 1,
        Game = 2,
    }
    
    [ChildOf(typeof(PlayerComponent))]
    public sealed class Player : Entity, IAwake<string>
    {
        public string Account { get; set; }
        
        public PlayerState playerState { get; set;}
        
        public long UnitId { get; set; }
    }
}