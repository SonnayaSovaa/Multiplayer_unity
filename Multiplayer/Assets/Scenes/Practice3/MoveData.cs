using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using UnityEngine;

public struct MoveData : IReplicateData
{
    public float Horizontal;
    public float Vertical;

    private uint _tick;
    public void Dispose() { }
    public uint GetTick() => _tick;
    public void SetTick(uint value) => _tick = value;
}

public struct ReconcileData : IReconcileData
{
    public Vector3 Position;

    private uint _tick;
    public void Dispose() { }
    public uint GetTick() => _tick;
    public void SetTick(uint value) => _tick = value;
}

public class PlayerMovementPredicted : NetworkBehaviour
{
    [SerializeField] private float _speed = 5f;

    public override void OnStartNetwork()
    {
        base.TimeManager.OnTick += OnTick;
    }

    public override void OnStopNetwork()
    {
        base.TimeManager.OnTick -= OnTick;
    }

    private void OnTick()
    {
        // Владелец собирает ввод и отправляет на сервер.
        if (base.IsOwner)
        {
            MoveData md = new MoveData
            {
                Horizontal = Input.GetAxisRaw("Horizontal"),
                Vertical = Input.GetAxisRaw("Vertical")
            };
            Replicate(md);
        }
        else
        {
            Replicate(default);
        }

        // Сервер периодически шлёт «истинное» состояние для коррекции.
        if (base.IsServerInitialized)
        {
            ReconcileData rd = new ReconcileData
            {
                Position = transform.position
            };
            Reconcile(rd);
        }
    }

    public override void CreateReconcile()
    {
        ReconcileData rd = new ReconcileData
        {
            Position = transform.position
        };
        Reconcile(rd);
    }

    [Replicate]
    private void Replicate(
        MoveData md,
        ReplicateState state = ReplicateState.Invalid,
        Channel channel = Channel.Unreliable)
    {
        Vector3 move = new Vector3(md.Horizontal, 0f, md.Vertical).normalized;
        transform.position += move * _speed * (float)base.TimeManager.TickDelta;
    }

    [Reconcile]
    private void Reconcile(
        ReconcileData rd,
        Channel channel = Channel.Unreliable)
    {
        transform.position = rd.Position;
    }
}