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
        Debug.Log("Collis");
        
        _target = other.gameObject.GetComponent<PlayerNetwork>();
    }

    private void OnTriggerExit(Collider other)
    {
        
        _target = null;
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.middleButton.wasPressedThisFrame)
            TryAttack();
    }

    public void TryAttack()
    {
        
        Debug.Log(_target);

        // Атаку инициирует только локальный владелец объекта.
        if (!IsOwner || _target == null || !player.IsAlive.Value)
            return;
        DealDamage(_target.ObjectId, damage);
    }

    [ServerRpc]
    private void DealDamage(int targetObjectId, int inputDamage)
    {
        Debug.Log(targetObjectId);
        
        // Сервер проверяет, существует ли цель среди заспавненных сетевых объектов.
        
        // Запрещаем урон самому себе и удары по некорректной цели.
        if (_target == null || _target == player)
            return;

        // Итоговое значение HP ограничиваем снизу нулем.
        int nextHp = Mathf.Max(0, _target.HP.Value - inputDamage);
        _target.HP.Value = nextHp;

        // Jump();
    }

}