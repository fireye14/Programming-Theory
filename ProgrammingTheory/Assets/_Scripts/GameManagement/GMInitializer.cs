#if UNITY_EDITOR
using System.Linq;
using System.Reflection;
using Assets._Scripts.GameManagement.BaseTypes;
using UnityEditor;
using UnityEngine;
#endif

namespace Assets._Scripts.GameManagement
{
    /// <summary>
    /// Class that handles initializing the GM when game is played from Editor, starting at a different scene besides Boot scene.
    /// Should be placed once in every scene.
    /// Allows ability to select a default game state for the given scene.
    /// </summary>
    public class GMInitializer : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField]
        private GameObject GameManagerPrefab;

        [HideInInspector]
        [SerializeField]
        private int SelectedGameStateIndex;


        protected void Awake()
        {
            if (GameManagerPrefab == null || GameManagerPrefab.GetComponent<GameManager>() == null || GameManager.IsInitialized)
                return;
            
            GameManager.DefaultGameState = GameStates[SelectedGameStateIndex];
            Instantiate(GameManagerPrefab);
        }

        /// <summary>
        /// Uses reflection to return array of all game states that have been added
        /// </summary>
        private static IGameState[] _gameStates;
        private static IGameState[] GameStates
        {
            get
            {
                return _gameStates ??= typeof(GameState)
                    .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Select(f => (IGameState)f.GetValue(null)).ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [CustomEditor(typeof(GMInitializer)), CanEditMultipleObjects]
        public class GMInitializerEditor : Editor
        {
            /// <summary>
            /// 
            /// </summary>
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                var obj = (GMInitializer)target;

                obj.SelectedGameStateIndex = EditorGUILayout.Popup(new GUIContent("Default Game State"), obj.SelectedGameStateIndex,
                    GameStates.Select(s => s.Name).ToArray());
                var sp = serializedObject.FindProperty("SelectedGameStateIndex");

                // return if no change
                if (obj.SelectedGameStateIndex == sp.intValue)
                    return;

                EditorUtility.SetDirty(obj);
                sp.intValue = obj.SelectedGameStateIndex;
                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}
