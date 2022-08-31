using System;
using Assets._Scripts.GameManagement.BaseTypes;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
#endif

namespace Assets._Scripts.Helpers
{
    public static class Events
    {
        #region UnityEvents

#if UNITY_EDITOR

        [Serializable]
        public class GameStateChangedEvent : UnityEvent<IGameState, IGameState> { }

        [Serializable]
        public class LoadOperationCompletedEvent : UnityEvent<string, bool> { }

        public class AddOrRemoveSystemManagerTypeEvent : UnityEvent<Type, bool> { }

        [Serializable]
        public class FadeCompleteEvent : UnityEvent<bool> { }

        [Serializable]
        public class EventVector3 : UnityEvent<Vector3> { }

#endif

        #endregion

        public delegate void GameStateChangedEventHandler(object sender, EventArgs.GameStateChangedEventArgs e);

        public delegate void ScoreChangedEventHandler(object sender, EventArgs.ScoreChangedEventArgs e);

        public delegate void LoadOperationEventHandler(object sender, EventArgs.LoadOperationEventArgs e);

        public delegate void AsyncOperationEventHandler(object sender, EventArgs.AsyncOperationEventArgs e);

        public delegate void SystemManagerTypeEventHandler(object sender, EventArgs.SystemManagerTypeEventArgs e);

    }

    public static class EventArgs
    {
        public class GameStateChangedEventArgs : System.EventArgs
        {
            public GameStateChangedEventArgs(IGameState prevState, IGameState newState)
            {
                PrevState = prevState;
                NewState = newState;
            }

            public IGameState PrevState { get; }
            public IGameState NewState { get; }
        }

        public class ScoreChangedEventArgs : System.EventArgs
        {
            public ScoreChangedEventArgs(int newScore)
            {
                NewScore = newScore;
            }

            public int NewScore { get; }
        }

        public class LoadOperationEventArgs : System.EventArgs
        {
            public LoadOperationEventArgs(LoadOperationParams p)
            {
                OperationParams = p;
            }

            /// <summary>
            /// Name of scene to load
            /// </summary>
            public LoadOperationParams OperationParams { get; }
        }

        public class AsyncOperationEventArgs : System.EventArgs
        {
            public AsyncOperationEventArgs(AsyncOperation ao)
            {
                AsyncOperation = ao;
            }

            /// <summary>
            /// Name of scene to load
            /// </summary>
            public AsyncOperation AsyncOperation { get; }
        }

        public class SystemManagerTypeEventArgs : System.EventArgs
        {
            public SystemManagerTypeEventArgs(Type type)
            {
                Type = type;
            }

            public Type Type { get; }
        }

    }
}