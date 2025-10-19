using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    [SerializeField] private PlayerCharacter _player;
    /*private float h;
    private float v;

    void Update()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
        
        _player.SetInput(h, v);
    }*/

    public InputActionAsset InputActions;

    private InputAction _move;
    private Vector2 _moveAmt;

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        _move = InputSystem.actions.FindAction("Move");
    }

    private void Update()
    {
        _moveAmt = _move.ReadValue<Vector2>();
        _player.SetInput(_moveAmt.x, _moveAmt.y);

        SendMove();
    }

    private void SendMove()
    {
        _player.GetMoveInfo(out Vector3 position);
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            {"x", position.x }, 
            {"y", position.z }
        };
        MultiplayerManager.Instance.SendMessage("move", data);
    }
}
