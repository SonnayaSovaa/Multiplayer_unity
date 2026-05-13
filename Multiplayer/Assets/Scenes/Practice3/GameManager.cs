using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private int _requiredPlayers = 2;

   
    public readonly SyncVar<GameState> CurrentState = new SyncVar<GameState>(0);
    
    public readonly SyncVar<int> ConnectedPlayers = new SyncVar<int>(0);
    public readonly SyncVar<float> MatchTimer = new SyncVar<float>(60);

    public enum GameState
    {
        WaitingForPlayers,
        InProgress,
        ShowingResults
    }
    
    private void Update()
    {
        if (!base.IsServerInitialized) return;
        if (CurrentState.Value != GameState.InProgress) return;

        MatchTimer.Value -= Time.deltaTime;

        if (MatchTimer.Value <= 0f)
        {
            EndMatch();
        }
    }

    public override void OnStartServer()
    {
        base.ServerManager.OnRemoteConnectionState += OnPlayerConnectionChanged;
        CurrentState.OnChange += OnGameStateChanged;
    }

    private void OnPlayerConnectionChanged(
        NetworkConnection conn,
        FishNet.Transporting.RemoteConnectionStateArgs args)
    {
        if (!base.IsServerInitialized) return;

        // Пересчитываем игроков.
        ConnectedPlayers.Value = base.ServerManager.Clients.Count;

        if (CurrentState.Value == GameState.WaitingForPlayers
            && ConnectedPlayers.Value >= _requiredPlayers)
        {
            StartMatch();
        }
    }

    private void StartMatch()
    {
        CurrentState.Value = GameState.InProgress;
        Debug.Log("[Server] Match started!");
    }

    private void OnGameStateChanged(
        GameState oldValue, GameState newValue, bool asServer)
    {
        // Клиенты реагируют на смену состояния — переключают UI.
        Debug.Log($"Game state changed: {oldValue} -> {newValue}");
    }
    
    private void EndMatch()
    {
        CurrentState.Value = GameState.ShowingResults;
        Debug.Log("[Server] Match ended! Showing results...");

        // Через 5 секунд возвращаемся в лобби.
        Invoke(nameof(ResetToLobby), 5f);
    }

    private void ResetToLobby()
    {
        // Сбросить очки всех игроков.
        foreach (var conn in base.ServerManager.Clients.Values)
        {
            foreach (var nob in conn.Objects)
            {
                PlayerNetwork pn = nob.GetComponent<PlayerNetwork>();
                if (pn != null)
                {
                    pn.HP.Value = 100;
                    pn.Score.Value = 0;
                }
            }
        }

        MatchTimer.Value = 60f;
        CurrentState.Value = GameState.WaitingForPlayers;
        Debug.Log("[Server] Lobby reset. Waiting for players...");
    }
}