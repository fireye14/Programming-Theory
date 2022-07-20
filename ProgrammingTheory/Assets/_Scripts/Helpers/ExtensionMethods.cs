using System;
using Assets._Scripts.GameManagement.BaseTypes;

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
        public static void AddOrRemoveSystemManagerType<T, G>(this ISystemManager SM, bool adding)
            where T : class, ISystemManager
            where G : class, IGameManager
        {
            AddOrRemoveSystemManagerTypeEvent?.Invoke(typeof(T), adding);
        }

    }
}