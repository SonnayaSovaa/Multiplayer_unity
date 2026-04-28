using Unity.Collections;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using System.Collections;

public class PlayerNetwork : NetworkBehaviour
{

    public readonly SyncVar<int> HP;
    public readonly SyncVar<string> Nickname;
    public readonly SyncVar<bool> IsAlive;
    
    
    [SerializeField] private CharacterController cc;
    [SerializeField] private CapsuleCollider collider;
    [SerializeField] private GameObject body;
    [SerializeField] private float spawnDelay = 3f;
    [SerializeField] private PlayerView playerView;

    private PlayerSpawnPoint[] points;

    public override void OnStartNetwork()
    {
        HP.OnChange += OnHpChanged;
        Nickname.OnChange += OnNicknameChanged;
        IsAlive.OnChange += OnIsAliveChanged;

        points = FindObjectsByType<PlayerSpawnPoint>(FindObjectsSortMode.None);

        transform.position = points[Random.Range(0, points.Length)].transform.position;

        OnIsAliveChanged(true, IsAlive.Value, IsServerInitialized);
    }

    private void OnNicknameChanged(string prev, string next, bool asServer)
    {
        playerView.OnNicknameChanged(next);
    }
    
    public override void OnStopNetwork()
    {
        HP.OnChange -= OnHpChanged;
        IsAlive.OnChange -= OnIsAliveChanged;
        Nickname.OnChange -= OnNicknameChanged;
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

    private void OnHpChanged(int prev, int next, bool asServer)
    {
        if (!IsServerInitialized) return;
        if (next <= 0 && IsAlive.Value)
        {
            IsAlive.Value = false;
        }
    }
    

    private void OnIsAliveChanged(bool prev, bool next, bool asServer)
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