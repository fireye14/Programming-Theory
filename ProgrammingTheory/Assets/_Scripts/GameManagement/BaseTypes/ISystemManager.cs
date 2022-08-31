namespace Assets._Scripts.GameManagement.BaseTypes
{
    public interface ISystemManager<G>
        where G : IGameManager
    {
        G GM { get; }
    }
}