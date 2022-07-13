
namespace Assets.Scripts.GameManagement.BaseTypes
{
    public interface IGameManager
    {
        string CurrentSceneName { get; }

        IGameState CurrentGameState { get; }

    }
}