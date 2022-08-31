using System;
using Assets._Scripts.GameManagement.BaseTypes;
using static Assets._Scripts.Helpers.Events;

namespace Assets._Scripts.Helpers
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Invoked when a SystemManager is instantiated or destroyed
        /// </summary>
        public static event Action<Type, bool> AddOrRemoveSystemManagerTypeEvent;

        /// <summary>
        /// Invoke event to add or remove SystemManager type T from its Game Manager's type list.
        /// </summary>
        /// <typeparam name="T">SystemManager type</typeparam>
        /// <typeparam name="G">GameManager type</typeparam>
        /// <param name="SM">The System Manager</param>
        /// <param name="adding">true to add; false to remove</param>
        public static void AddOrRemoveSystemManagerType<T, G>(this ISystemManager<G> SM, bool adding)
            where T : class, ISystemManager<G>
            where G : class, IGameManager
        {
            AddOrRemoveSystemManagerTypeEvent?.Invoke(typeof(T), adding);
        }


        /// <summary>
        /// Do something like this, make the GM event internal so it 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <param name="SM"></param>
        /// <param name="method"></param>
        public static void Test<T, G>(this SystemManagerBase<T, G> SM, GameStateChangedEventHandler method)
            where G : GameManagerBase<G>
            where T : SystemManagerBase<T, G>
        {
            SM.GM.GameStateChanged += method;
        }

    }
}