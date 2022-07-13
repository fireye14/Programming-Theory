using System;
using Assets.Scripts.GameManagement.BaseTypes;
using UnityEngine;
using UnityEngine.Events;
using static Assets.Scripts.Helpers.Enums;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts.Helpers
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

        public delegate void LoadOperationCompletedEventHandler(object sender, EventArgs.LoadOperationCompletedEventArgs e);

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

        public class LoadOperationCompletedEventArgs : System.EventArgs
        {
            public LoadOperationCompletedEventArgs(string sceneName, LoadOperationType opType)
            {
                SceneName = sceneName;
                OpType = opType;
            }

            /// <summary>
            /// Name of scene to load
            /// </summary>
            public string SceneName { get; }

            /// <summary>
            /// Type of load operation
            /// </summary>
            public LoadOperationType OpType { get; }
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