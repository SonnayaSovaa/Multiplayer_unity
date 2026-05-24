using System;
using FishNet;
using FishNet.Managing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionUI : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;

    [SerializeField] private TMP_InputField nicknameInputField;
    [SerializeField] GameObject buttonContainer;

    public static event Action<string> OnNameChanged;

    public static string PlayerNickname { get; private set; } = "Player";

    private void Start()
    {
        if (!networkManager) networkManager = InstanceFinder.NetworkManager;
        nicknameInputField.onValueChanged.AddListener(ChangeName);
    }

    public void StartClient()
    {
        SaveNickname();
        if (networkManager) networkManager.ClientManager.StartConnection();
        DeactivateButtons();
    }
    

    private void DeactivateButtons()
    {
        buttonContainer.SetActive(false);
    }

    private void SaveNickname()
    {
        nicknameInputField.DeactivateInputField(true);

        string rawValue = nicknameInputField.text != null ? nicknameInputField.text : string.Empty;
        if (string.IsNullOrWhiteSpace(rawValue)) PlayerNickname = "Player";
        else PlayerNickname = rawValue.Trim();
    }

    private void ChangeName(string playerName)
    {
        PlayerNickname = string.IsNullOrEmpty(playerName) ? "Player" : playerName.Trim();
        OnNameChanged?.Invoke(playerName);
    }
}