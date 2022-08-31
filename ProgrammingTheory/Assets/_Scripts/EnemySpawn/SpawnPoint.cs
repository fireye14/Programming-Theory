using UnityEngine;

namespace Assets._Scripts.EnemySpawn
{
    public class SpawnPoint : MonoBehaviour
    {

        /// <summary>
        /// max distance to the left of game object to spawn enemies
        /// </summary>
        public float XMin;

        /// <summary>
        /// max distance to the right of game object to spawn enemies
        /// </summary>
        public float XMax;
    }
}
