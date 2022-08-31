using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    #region Fields

    [SerializeField] private Animation _weaponAnimator;
    [SerializeField] private AnimationClip _weaponSwingAnim;

    /// <summary>
    /// able to attack? 
    /// </summary>
    private bool _canAttack;

    /// <summary>
    /// amount of seconds to wait before able to attack again
    /// </summary>
    private float _attackCooldown;

    #endregion

    #region Methods

    /// <summary>
    /// Initialize
    /// </summary>
    protected void Awake()
    {
        _canAttack = true;
        _attackCooldown = _weaponSwingAnim.length;
        _weaponSwingAnim.AddEvent(new AnimationEvent { functionName = nameof(OnWeaponSwingComplete), time = _weaponSwingAnim.length });
    }

    /// <summary>
    /// Perform attack
    /// </summary>
    public void Attack()
    {
        if (!_canAttack)
            return;

        _weaponAnimator.Stop();
        if (_weaponSwingAnim == null)
            return;

        _weaponAnimator.clip = _weaponSwingAnim;
        _weaponAnimator.Play();
        StartCoroutine(AttackCoolDown());
    }

    /// <summary>
    /// Disable once attack done
    /// </summary>
    public void OnWeaponSwingComplete()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// unable to attack for _attackCooldown seconds
    /// </summary>
    /// <returns></returns>
    protected IEnumerator AttackCoolDown()
    {
        _canAttack = false;
        yield return new WaitForSeconds(_attackCooldown);
        _canAttack = true;
    }

    #endregion

}
