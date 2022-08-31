using System;
using System.Collections;
using System.Collections.Generic;
using Assets._Scripts.GameManagement;
using UnityEngine;
using static Assets._Scripts.Helpers.Constants;

namespace Assets._Scripts.Characters
{
    public class EnemyController : MonoBehaviour, IDamageable
    {

        #region Fields

        /// <summary>
        /// Maximum health of the enemy
        /// </summary>
        [SerializeField] private int _maxHealth;

        /// <summary>
        /// Current health of the enemy
        /// </summary>
        [SerializeField] private int _currentHealth;

        /// <summary>
        /// Movement speed
        /// </summary>
        [SerializeField] private float _speed;

        /// <summary>
        /// How many points awarded to the player upon defeat
        /// </summary>
        [SerializeField] private int _pointsAwarded;

        /// <summary>
        /// Rigidbody component
        /// </summary>
        [SerializeField] private Rigidbody _rigidBody;

        /// <summary>
        /// Amount of time invincible to damage after being hit
        /// </summary>
        [SerializeField] private float _invincibleTime;

        /// <summary>
        /// currently invincible?
        /// </summary>
        private bool _isInvincible;

        /// <summary>
        /// able to move forward?
        /// </summary>
        private bool _canMoveForward;

        #endregion


        #region Properties and Backing Fields

        /// <summary>
        /// Maximum health of the enemy
        /// </summary>
        public int MaxHealth => _maxHealth;

        /// <summary>
        /// Current health of the enemy
        /// </summary>
        public int CurrentHealth => _currentHealth;

        /// <summary>
        /// <see cref="GameManager.Instance"/>
        /// </summary>
        protected GameManager GM = GameManager.Instance;

        #endregion

        #region Methods

        /// <summary>
        /// Handle collisions
        /// </summary>
        /// <param name="collision">information about the collision</param>
        void OnCollisionEnter(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                // death zone is outside of game boundary; clean up enemies that hit it
                if (contact.otherCollider.CompareTag(Tags.DeathZone))
                {
                    Destroy(gameObject);
                    return;
                }

                // take damage if struck by player weapon
                else if (contact.otherCollider.CompareTag(Tags.Weapon))
                {
                    TakeDamage(contact, collision.relativeVelocity);
                    break;
                }

            }
        }

        /// <summary>
        /// Initialize
        /// </summary>
        protected void Awake()
        {
            if(MaxHealth == 0)
                _maxHealth = 1;
            _currentHealth = MaxHealth;

            if (_invincibleTime == 0f)
                _invincibleTime = 0.68f;

            if (_speed <= 0)
                _speed = 1f;

            if (_pointsAwarded <= 0)
                _pointsAwarded = 10;

            _canMoveForward = true;

            if (_rigidBody == null)
                _rigidBody = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Handle physics operations
        /// </summary>
        protected void FixedUpdate()
        {
            // keep moving forward if able to
            if (_canMoveForward)
            {
                _rigidBody.MovePosition(_rigidBody.position + (_rigidBody.rotation * Vector3.forward * _speed * Time.fixedDeltaTime));
            }
        }

        /// <summary>
        /// Take damage. Called when hit by player weapon.
        /// </summary>
        /// <param name="point">point of contact</param>
        /// <param name="velocity">velocity of collision</param>
        protected void TakeDamage(ContactPoint point, Vector3 velocity)
        {
            // do nothing if invincible
            if (_isInvincible)
                return;

            // subtract 1 from current health and handle accordingly
            _currentHealth -= 1;
            if (CurrentHealth <= 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(BecomeInvincible());
                StartCoroutine(ApplyKnockback(transform.position - point.otherCollider.transform.position, velocity));
            }
        }

        /// <summary>
        /// Handle enemy death
        /// </summary>
        protected void Die()
        {
            GM.AddScore(_pointsAwarded);
            Destroy(gameObject);
        }

        /// <summary>
        /// Stop moving forward and apply "knockback" after being hit
        /// </summary>
        /// <param name="direction">direction to knock back in</param>
        /// <param name="velocity">velocity of knockback</param>
        /// <returns></returns>
        protected IEnumerator ApplyKnockback(Vector3 direction, Vector3 velocity)
        {
            _canMoveForward = false;
            direction.y = 0f;
            velocity.y = 0f;
            var force = direction.normalized * 10f + velocity;

            var time = 0.0f;
            while (time < 0.07f)
            {
                _rigidBody.MovePosition(_rigidBody.position + (force * Time.fixedDeltaTime));
                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _canMoveForward = true;
        }

        /// <summary>
        /// Set invincible flag to true for _invincibleTime seconds
        /// </summary>
        /// <returns></returns>
        protected IEnumerator BecomeInvincible()
        {
            _isInvincible = true;
            yield return new WaitForSeconds(_invincibleTime);
            _isInvincible = false;
        }

        #endregion
    }
}
