using Assets._Scripts.GameManagement.BaseTypes;

namespace Assets._Scripts.GameManagement
{
    public class GameState : GameStateBase
    {
        public GameState(string name) : base(name)
        {
        }

        public static readonly GameState Pregame = new GameState(nameof(Pregame));
        public static readonly GameState TitleScreen = new GameState(nameof(TitleScreen));
        public static readonly GameState Running = new GameState(nameof(Running));
        public static readonly GameState Paused = new GameState(nameof(Paused));
    }
}