using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private float _speed = 15f;
    [SerializeField] private int _damage = 25;
    private bool _hasHit = false;
    private void Update()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer || _hasHit) return; 

        var target = other.GetComponent<PlayerNetwork>();

        if (target != null)
        {
            if (target.OwnerClientId == OwnerClientId) return;

            _hasHit = true; 

            target.HP.Value = Mathf.Max(0, target.HP.Value - _damage);

            if (NetworkObject.IsSpawned)
            {
                NetworkObject.Despawn(true);
            }
        }
    }
}