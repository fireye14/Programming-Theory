using System;
using System.Reflection;

namespace Assets.Scripts.GameManagement.BaseTypes
{
    /// <summary>
    /// This acts as a container for all possible game states. 
    /// In derived class, simply include another partial definition for this struct and add more public static readonly GameStates as below.
    /// </summary>
    public class GameStateBase : IEquatable<GameStateBase>, IGameState
    {

        // Reference these fields the same as an enum
        // Ex. CurrentGameState == GameState.Pregame


        public static readonly GameStateBase Pregame = new GameStateBase();
        public static readonly GameStateBase Running = new GameStateBase();
        public static readonly GameStateBase Paused = new GameStateBase();


        /// <summary>
        /// Holds the value to be assigned to the next added GameState
        /// </summary>
        private static ushort NextValue;

        /// <summary>
        /// Used for equality comparisons
        /// </summary>
        public ushort Value { get; private set; }

        /// <summary>
        /// Assign the Value and increment NextValue
        /// </summary>
        public GameStateBase()
        {
            Value = NextValue++;
        }


        /// <summary>
        /// Return name of variable. Example, GameState.Pregame -> "Pregame". Mostly for debugging purposes.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var fi = GetType().GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            foreach (var f in fi)
            {
                var fv = (GameStateBase)f.GetValue(null);
                if (fv == null || fv.Value != Value)
                    continue;

                return f.Name;
            }

            return Value.ToString();
        }


        #region Equality implementation

        public override bool Equals(object other)
        {
            if (!(other is GameStateBase))
            {
                return false;
            }

            if (this is null && other is null)
                return true;

            if (this is null || other is null)
                return false;

            var gs = (GameStateBase)other;
            return this == gs;
        }

        public bool Equals(GameStateBase other)
        {
            if (this is null && other is null)
                return true;

            if (this is null || other is null)
                return false;

            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return -1937169414 + Value.GetHashCode();
        }

        public static bool operator ==(GameStateBase A, GameStateBase B)
        {
            if (A is null && B is null)
                return true;

            if (A is null || B is null)
                return false;

            return A.Value == B.Value;
        }

        public static bool operator !=(GameStateBase A, GameStateBase B)
        {
            if (A is null && B is null)
                return false;

            if (A is null || B is null)
                return true;

            return A.Value != B.Value;
        }

        public static bool operator ==(IGameState A, GameStateBase B)
        {
            if (A is null && B is null)
                return true;

            if (A is null || B is null)
                return false;

            return A.Value == B.Value;
        }

        public static bool operator !=(IGameState A, GameStateBase B)
        {
            if (A is null && B is null)
                return false;

            if (A is null || B is null)
                return true;

            return A.Value != B.Value;
        }

        public static bool operator ==(GameStateBase A, IGameState B)
        {
            if (A is null && B is null)
                return true;

            if (A is null || B is null)
                return false;

            return A.Value == B.Value;
        }

        public static bool operator !=(GameStateBase A, IGameState B)
        {
            if (A is null && B is null)
                return false;

            if (A is null || B is null)
                return true;

            return A.Value != B.Value;
        }

        #endregion

    }
}