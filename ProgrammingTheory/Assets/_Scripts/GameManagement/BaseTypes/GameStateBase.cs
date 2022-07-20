using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Assets._Scripts.GameManagement.BaseTypes
{
    /// <summary>
    /// This acts as a container for all possible game states. 
    /// In derived class, simply include another partial definition for this struct and add more public static readonly GameStates as below.
    /// </summary>
    public class GameStateBase : IEquatable<GameStateBase>, IGameState
    {

        // Reference these fields the same as an enum
        // Ex. CurrentGameState == GameState.Default

        #region GameState List

        public static readonly GameStateBase Default = new GameStateBase(nameof(Default));

        #endregion

        /// <summary>
        /// Holds the value to be assigned to the next added GameState
        /// </summary>
        private static ushort NextValue;

        /// <summary>
        /// Used for equality comparisons
        /// </summary>
        public ushort Value { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; }

        public GameStateBase() : this(string.Empty)
        {
        }

        /// <summary>
        /// Assign the Value and increment NextValue
        /// </summary>
        public GameStateBase(string name)
        {
            Value = NextValue++;
            Name = name; 
        }

        /// <summary>
        /// Return name of variable. Example, GameState.Pregame -> "Pregame". Mostly for debugging purposes.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(Name) ? Value.ToString() : Name;
        }


        #region Equality implementation

        public override bool Equals(object other)
        {
            if (!(other is GameStateBase gs))
            {
                return false;
            }

            return this == gs;
        }

        public bool Equals(GameStateBase other)
        {
            if (other is null)
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