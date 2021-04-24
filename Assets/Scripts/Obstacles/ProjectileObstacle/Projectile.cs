using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region inspector

    [SerializeField]
    float _speed = 10f;

    [SerializeField]
    float _lifetime = 2f;

    #endregion

    #region private members

    bool _isDestroying = false;

    #endregion

    #region private methods

    void Start()
    {
        StartCoroutine(DestroyCoroutine());
    }

    IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(_lifetime);
        if (!_isDestroying)
        {
            _isDestroying = true;
            Destroy(gameObject);
        }
    }

    void DestroySelf()
    {
        if (_isDestroying)
            return;
        
        _isDestroying = true;
        Destroy(gameObject);
    }

    void OnPlayerHit()
    {
        Debug.Log("Player was hit !");
        GameManager.singleton?.PlayerEvents.PlayerIsDead();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Player")
        {
            OnPlayerHit();
        }
        DestroySelf();
    }

    void FixedUpdate()
    {
        transform.position += transform.forward * _speed * Time.fixedDeltaTime;
    }

    #endregion
}