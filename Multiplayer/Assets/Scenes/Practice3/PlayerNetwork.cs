using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class PlayerNetwork : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> Nickname = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> HP = new(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> IsAlive = new(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] private CharacterController cc;
    [SerializeField] private CapsuleCollider collider;
    [SerializeField] private GameObject body;
    [SerializeField] private float spawnDelay = 3f;

    private PlayerSpawnPoint[] points;
    public override void OnNetworkSpawn()
    {
        points = FindObjectsByType<PlayerSpawnPoint>(FindObjectsSortMode.None);
        if (IsOwner)
        {
            SubmitNicknameServerRpc(ConnectionUI.PlayerNickname);
            transform.position = points[Random.Range(0, points.Length)].transform.position;

        }
        
        HP.OnValueChanged += OnHpChanged;
        IsAlive.OnValueChanged += OnIsAliveChanged;

        OnIsAliveChanged(true, IsAlive.Value);
    }

    public override void OnNetworkDespawn()
    {
        HP.OnValueChanged -= OnHpChanged;
        IsAlive.OnValueChanged -= OnIsAliveChanged;
    }
    
    IEnumerator RespawnCool(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        collider.enabled = true;
        body.SetActive(true);
        cc.enabled = true;
        HP.Value = 100;
        GetComponent<PlayerShooting>().Ammo.Value = 30;
        IsAlive.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SubmitNicknameServerRpc(string nickname)
    {
        Nickname.Value = string.IsNullOrWhiteSpace(nickname) ? $"Player_{OwnerClientId}" : nickname.Trim();
    }

    private void OnHpChanged(int prev, int next)
    {
        if (!IsServer) return;
        if (next <= 0 && IsAlive.Value)
        {
            IsAlive.Value = false;
        }
    }
    

    private void OnIsAliveChanged(bool prev, bool isAlive)
    {
        if (IsOwner && !IsAlive.Value)
        {
            
            body.SetActive(false);
            collider.enabled = false;
            transform.position = points[Random.Range(0, points.Length)].transform.position;
            cc.enabled = false;
            StartCoroutine(RespawnCool(spawnDelay));
        }
    }
}