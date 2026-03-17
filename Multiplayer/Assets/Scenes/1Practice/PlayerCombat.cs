using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerCombat : NetworkBehaviour
{
    [SerializeField] private PlayerNetwork playerNetwork;
    [SerializeField] private int damage = 10;
    private PlayerNetwork _target;
    private ActionMaps _control;

    
    private void Awake()
    {
        _control = new ActionMaps();
        _control.Enable();
        _control.Player.Attack.started += ctx => TryAttack();

    }
    private void OnCollisionEnter(Collision other)
    {
        playerNetwork = other.gameObject.GetComponent<PlayerNetwork>();
    }

    private void OnCollisionExit(Collision other)
    {
        playerNetwork = null;
    }

    public void TryAttack()
    {
        
        Debug.Log(_target);

        // Атаку инициирует только локальный владелец объекта.
        if (!IsOwner || _target == null)
            return;
        DealDamageServerRpc(_target.NetworkObjectId, damage);
    }

    [ServerRpc]
    private void DealDamageServerRpc(ulong targetObjectId, int inputDamage)
    {
        Debug.Log(targetObjectId);
        
        // Сервер проверяет, существует ли цель среди заспавненных сетевых объектов.
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(targetObjectId, out NetworkObject targetObject))
            return;

        PlayerNetwork targetPlayer = targetObject.GetComponent<PlayerNetwork>();
        // Запрещаем урон самому себе и удары по некорректной цели.
        if (targetPlayer == null || targetPlayer == playerNetwork)
            return;

        // Итоговое значение HP ограничиваем снизу нулем.
        int nextHp = Mathf.Max(0, targetPlayer.HP.Value - inputDamage);
        targetPlayer.HP.Value = nextHp;
    }
    
    private void OnDisable()
    {
        _control.Disable();
    }
}