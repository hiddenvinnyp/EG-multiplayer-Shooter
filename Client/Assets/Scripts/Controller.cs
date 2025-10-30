using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    [SerializeField] private PlayerCharacter _player;
    [SerializeField] private PlayerGun _gun;
    [SerializeField] private float _mouseSensetivity = 2f;
    private MultiplayerManager _multiplayerManager;
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
    private InputAction _look;
    private InputAction _jump;
    private InputAction _attack;
    private InputAction _crouch;
    private Vector2 _moveAmt;
    private Vector2 _lookAmt;

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
        _look = InputSystem.actions.FindAction("Look");
        _jump = InputSystem.actions.FindAction("Jump");
        _attack = InputSystem.actions.FindAction("Attack");
        _crouch = InputSystem.actions.FindAction("Crouch");
    }
    private void Start()
    {
        _multiplayerManager = MultiplayerManager.Instance;
    }

    private void Update()
    {
        _moveAmt = _move.ReadValue<Vector2>();
        _lookAmt = _look.ReadValue<Vector2>();

        _player.SetInput(_moveAmt.x, _moveAmt.y, _lookAmt.x * _mouseSensetivity);
        _player.RotateX(-_lookAmt.y * _mouseSensetivity);

        if (_jump.WasPressedThisFrame()) _player.Jump();
        if (_crouch.IsPressed()) 
        { 
            _player.Crouch(true);
            SendCrouch();
        }

        if (_crouch.WasReleasedThisFrame())
        { 
            _player.Crouch(false); 
            SendCrouch();
        }
        if (_attack.WasPressedThisFrame() && _gun.TryShoot(out ShootInfo shootInfo)) SendShoot(ref shootInfo);

        SendMove();
    }

    private void SendShoot(ref ShootInfo shootInfo)
    {
        shootInfo.key = _multiplayerManager.GetSessionId();
        string json = JsonUtility.ToJson(shootInfo);
        _multiplayerManager.SendMessage("shoot", json);
    }

    private void SendMove()
    {
        _player.GetMoveInfo(out Vector3 position, out Vector3 velocity, out float rotateX, out float rotateY);
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            {"pX", position.x }, 
            {"pY", position.y },
            {"pZ", position.z },
            {"vX", velocity.x },
            {"vY", velocity.y },
            {"vZ", velocity.z },
            {"rX", rotateX },
            {"rY", rotateY }
        };
        _multiplayerManager.SendMessage("move", data);
    }

    private void SendCrouch()
    {
        _player.GetCrouchInfo(out bool isCrouch);
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            {"crouch", isCrouch }
        };
        _multiplayerManager.SendMessage("crouch", data);
    }
}

[System.Serializable]
public struct ShootInfo
{
    public string key;
    public float pX;
    public float pY;
    public float pZ;
    public float dX; 
    public float dY; 
    public float dZ;
}