using System;
using System.Reflection;
using Assets._Scripts.Helpers;
using UnityEngine;

namespace Assets._Scripts.GameManagement.BaseTypes
{
    /// <summary>
    /// Manages a system in the game. All communication to/from the GameManager should be done to/from a SystemManager through UnityEvents.
    /// </summary>
    /// <typeparam name="T">SystemManager type</typeparam>
    /// <typeparam name="G">GameManager type</typeparam>
    public abstract class SystemManagerBase<T, G> : Singleton<T>, ISystemManager
        where T : SystemManagerBase<T, G>
        where G : GameManagerBase<G>
    {

        #region Properties and Backing Fields

        /// <summary>
        /// Should return GameManager's Instance
        /// </summary>
        private G _gm;

        protected G GM
        {
            get
            {
                if (_gm == null)
                {
                    _gm = (G)typeof(Singleton<G>).GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
                }

                return _gm;
            }
        }

        #endregion

        #region Events


        #endregion

        #region Abstract Methods

        /// <summary>
        /// Called in the Awake method, after successful initialization
        /// </summary>
        protected abstract void AwakeSystemManager();

        /// <summary>
        /// Called in the OnDestroy method, after successful destroy
        /// </summary>
        protected abstract void OnDestroySystemManager();

        #endregion

        #region Overrides

        /// <summary>
        /// If initialized successfully, add this SystemManager type to its GameManager's list if necessary. Then call AwakeSystemManager().
        /// </summary>
        protected sealed override void Awake()
        {
            try
            {
                base.Awake();

                if (GM == null)
                {
                    Destroy(gameObject);
                    throw new Exceptions.GameManagerNotInitializedException($"[{GetType().Name}] Cannot instantiate a SystemManager before its GameManager.");
                }

                // This is a valid SystemManager instance; add its type to the GameManager if necessary


                // The type would exist in the GameManager at this point if this SystemManager was attached to a prefab and instantiated by the GameManager as a globally-persistent system
                // The type would not exist if this SystemManager is being instantiated by a newly-loaded scene and is not a globally-persistent system
                //_addSystemManagerType?.Invoke(this, new EventArgs.SystemManagerTypeEventArgs(GetType()));

                GM.AddSystemManager(Instance);

                // Initialized successfully. Call the "real" Awake method defined by the derived class.
                AwakeSystemManager();

            }
            catch (Exceptions.DuplicateSingletonException e)
            {
                Debug.LogWarning(e.Message);
            }
            catch (Exceptions.GameManagerNotInitializedException e)
            {
                Debug.LogError(e.Message);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        /// <summary>
        /// Ensure this SystemManager type does not exist in the GameManager's list before being destroyed.
        /// </summary>
        protected sealed override void OnDestroy()
        {
            try
            {
                base.OnDestroy();

                GM?.RemoveSystemManager(this);
                OnDestroySystemManager();
            }
            catch (Exceptions.DuplicateSingletonException e)
            {
                Debug.LogWarning(e.Message);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

        }

        #endregion
    }
}
