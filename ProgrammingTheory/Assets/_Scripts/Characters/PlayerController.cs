using System.Collections;
using Assets._Scripts.GameManagement;
using UnityEngine;
using static Assets._Scripts.Helpers.Constants;

namespace Assets._Scripts.Characters
{
    public class PlayerController : MonoBehaviour, IDamageable
    {
        #region Fields

        /// <summary>
        /// Rigidbody component
        /// </summary>
        [SerializeField] private Rigidbody _rigidBody;

        /// <summary>
        /// Amount of time invincible to damage after being hit
        /// </summary>
        [SerializeField] private float _invincibleTime;

        /// <summary>
        /// Movement speed
        /// </summary>
        [SerializeField] private float _speed;

        /// <summary>
        /// Maximum health of the player
        /// </summary>
        [SerializeField] private int _maxHealth;

        /// <summary>
        /// Current health of the player
        /// </summary>
        [SerializeField] private int _currentHealth;

        /// <summary>
        /// Player's weapon
        /// </summary>
        [SerializeField] private Weapon _weapon;

        /// <summary>
        /// Animation to play while invincible
        /// </summary>
        [SerializeField] private Animation _invincibleAnimator;
        [SerializeField] private AnimationClip _invincibleAnimationClip;

        /// <summary>
        /// mesh renderer component
        /// </summary>
        [SerializeField] private MeshRenderer _meshRenderer;


        private bool _isInvincible;

        #endregion

        #region Properties and Backing Fields

        /// <summary>
        /// Singleton: <see cref="GameManager.Instance"/>
        /// </summary>
        public GameManager GM => GameManager.Instance;

        /// <summary>
        /// Singleton: <see cref="UIManager.Instance"/>
        /// </summary>
        public UIManager UI => UIManager.Instance;

        /// <summary>
        /// Current health of the player
        /// </summary>
        public int CurrentHealth => _currentHealth;

        /// <summary>
        /// Maximum health of the player
        /// </summary>
        public int MaxHealth => _maxHealth;

        #endregion

        #region Methods

        /// <summary>
        /// Initialize
        /// </summary>
        protected void Awake()
        {
            if (_rigidBody == null)
                _rigidBody = GetComponent<Rigidbody>();

            if (_invincibleTime == 0f)
                _invincibleTime = 2f;

            if (_speed == 0f)
                _speed = 1f;

            if (_maxHealth == 0)
                _maxHealth = 3;

            if (_currentHealth <= 0)
                _currentHealth = _maxHealth;

            if(UI != null)
                UI.UpdateHealth(_currentHealth);

            if(_weapon != null)
                _weapon.gameObject.SetActive(false);
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Update()
        {
            // Handle button input for movement and attacking

            var h = Input.GetAxisRaw("Horizontal");
            var v = Input.GetAxisRaw("Vertical");
            var dir = new Vector3(h, 0f, v).normalized;

            if (dir.magnitude >= 0.1f)
            {
                _rigidBody.MovePosition(_rigidBody.position + _speed * Time.deltaTime * dir);
            }

            if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump"))
            {
                Attack();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        void OnCollisionEnter(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                // Hit by enemy
                if (contact.otherCollider.CompareTag(Tags.Enemy))
                {
                    HitByEnemy();
                    _rigidBody.velocity = Vector3.zero;     // set velocity to zero to prevent sliding
                    break;
                }

            }
        }

        

        /// <summary>
        /// Handle being hit by enemy
        /// </summary>
        public void HitByEnemy()
        {
            // do nothing if invincible
            if (_isInvincible)
                return;

            StartCoroutine(BecomeInvincible());

            // subtract 1 health and trigger game over if necessary
            _currentHealth -= 1;
            UI.UpdateHealth(_currentHealth);
            if (_currentHealth <= 0 && GM.CurrentGameState == GameState.Running)
                GM.GameOver();
        }

        /// <summary>
        /// Perform weapon attack
        /// </summary>
        public void Attack()
        {
            if (_weapon == null || _isInvincible)
                return;

            _weapon.gameObject.SetActive(true);
            _weapon.Attack();
        }

        /// <summary>
        /// Set invincible flag to true, set to invincible layer, and play invincible animation for _invincibleTime seconds
        /// </summary>
        /// <returns></returns>
        protected IEnumerator BecomeInvincible()
        {
            _isInvincible = true;
            var oldLayer = gameObject.layer;
            gameObject.layer = Layers.Invincible;
            _invincibleAnimator.Stop();
            _invincibleAnimator.clip = _invincibleAnimationClip;
            _invincibleAnimator.Play();
            yield return new WaitForSeconds(_invincibleTime);
            gameObject.layer = oldLayer;
            _invincibleAnimator.Stop();
            _meshRenderer.enabled = true;       // "reset" animation
            _isInvincible = false;
        }

        #endregion

    }
}
