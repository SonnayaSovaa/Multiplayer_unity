using UnityEngine;
using FishNet;

public class ServerAutoStart : MonoBehaviour
{
    private void Start()
    {
        if (Application.isBatchMode)
        {
            Debug.Log("[Server] Headless mode detected. Starting server...");
            InstanceFinder.ServerManager.StartConnection();
        }
    }
}
