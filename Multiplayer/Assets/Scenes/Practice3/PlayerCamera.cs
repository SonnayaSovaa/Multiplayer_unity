using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField] private Vector3 _offset = new(0f, 8f, -6f);

    private Camera _cam;

    public override void OnStartNetwork()
    {
        _cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (_cam == null) return;
        _cam.transform.position = transform.position + _offset;
        _cam.transform.LookAt(transform.position);
    }
}