using System.Collections;
using Assets._Scripts.Characters;
using Assets._Scripts.GameManagement;
using Assets._Scripts.GameManagement.BaseTypes;
using UnityEngine;

namespace Assets._Scripts.EnemySpawn
{
    public class EnemySpawnManager : SystemManagerBase<EnemySpawnManager, GameManager>
    {

        #region Fields

        /// <summary>
        /// number of seconds between spawns
        /// </summary>
        [SerializeField] private float _spawnCooldown;

        /// <summary>
        /// array of spawn locations
        /// </summary>
        [SerializeField] private SpawnPoint[] _spawnPoints;

        /// <summary>
        /// list of enemy types
        /// </summary>
        [SerializeField] private GameObject[] _enemyPrefabs;

        /// <summary>
        /// player object
        /// </summary>
        [SerializeField] private GameObject _player;

        #endregion

        #region Properties and Backing fields

        /// <summary>
        /// destroy on scene load
        /// </summary>
        public override bool IsPersisted => false;

        #endregion

        #region Overrides

        protected override void AwakeSystemManager()
        {
            StartCoroutine(SpawnEnemies());
        }

        protected override void OnDestroySystemManager()
        {
            StopCoroutine(SpawnEnemies());
        }

        #endregion

        #region Methods

        /// <summary>
        /// Begin spawning enemies
        /// </summary>
        /// <returns></returns>
        protected IEnumerator SpawnEnemies()
        {
            while(true)
            {
                yield return new WaitForSeconds(_spawnCooldown);
                SpawnEnemy();
            }
        }

        /// <summary>
        /// Pick a random enemy type and spawn location and spawn the enemy
        /// </summary>
        protected void SpawnEnemy()
        {
            if (_enemyPrefabs.Length == 0 || _spawnPoints.Length == 0)
                return;

            var point = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
            var enemyType = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];

            var enemy = Instantiate(enemyType, point.transform.position, Quaternion.identity, point.transform);

            enemy.transform.localPosition += Vector3.right * Random.Range(point.XMin, point.XMax);

            // zero out y position in vector so enemy doesn't aim up or down
            var vector = _player.transform.position - enemy.transform.position;
            enemy.transform.rotation = Quaternion.LookRotation(new Vector3(vector.x, 0f, vector.z));
        }

        #endregion

    }
}
