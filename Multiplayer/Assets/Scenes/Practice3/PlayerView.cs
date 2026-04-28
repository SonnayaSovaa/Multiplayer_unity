using TMPro;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using Unity.Collections;

public class PlayerView : NetworkBehaviour
{
    [SerializeField] private PlayerNetwork _playerNetwork;
    [SerializeField] private TMP_Text _nicknameText;
    [SerializeField] private TMP_Text _hpText;

    public override void OnStartNetwork()
    {
        // Подписываемся на изменения только после сетевого спавна объекта.

        // Сразу рисуем текущее состояние, чтобы UI не ждал первого сетевого события.
        //OnNicknameChanged(_playerNetwork.Nickname.Value);
        //OnHpChanged(0, _playerNetwork.HP.Value);
    }

    public void OnNicknameChanged(string newValue)
    {
        _nicknameText.text = newValue;
    }

    private void OnHpChanged(int oldValue, int newValue)
    {
        _hpText.text = $"HP: {newValue}";
    }
}