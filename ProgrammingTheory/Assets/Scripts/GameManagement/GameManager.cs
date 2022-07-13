using Assets.Scripts.GameManagement.BaseTypes;
using static Assets.Scripts.Helpers.Enums;

namespace Assets.Scripts.GameManagement
{
    public class GameManager : GameManagerBase<GameManager>
    {
        protected override void AwakeGameManager()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnGameStateChanged(IGameState prevState, IGameState newState)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnLoadOperationCompleted(string sceneName, LoadOperationType opType)
        {
            throw new System.NotImplementedException();
        }
    }
}
