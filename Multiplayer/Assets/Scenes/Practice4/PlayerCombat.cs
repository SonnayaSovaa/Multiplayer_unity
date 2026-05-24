using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerCombat : NetworkBehaviour
{
    [SerializeField] private PlayerNetwork player;
    [SerializeField] private int damage = 10;
    private PlayerNetwork _target;
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServerInitialized)
            return;

        if (!IsSpawned)
            return;

        PlayerNetwork target = other.GetComponent<PlayerNetwork>();
        if (target == null)
            return;

        if (target.Owner.ClientId == Owner.ClientId)
            return;

        target.TakeDamage(damage);
        player.Score.Value +=damage;
    }

    private void OnTriggerExit(Collider other)
    {
        _target = null;
        int a = 0;
    }

    

}