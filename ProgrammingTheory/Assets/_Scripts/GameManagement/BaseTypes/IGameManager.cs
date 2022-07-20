
namespace Assets._Scripts.GameManagement.BaseTypes
{
    public interface IGameManager
    {
        string CurrentSceneName { get; }

        IGameState CurrentGameState { get; }

    }
}