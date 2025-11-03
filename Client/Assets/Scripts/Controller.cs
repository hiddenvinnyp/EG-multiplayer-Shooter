using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    [SerializeField] private float _restartDelay = 3f;
    [SerializeField] private PlayerCharacter _player;
    [SerializeField] private PlayerGun _gun;
    [field: SerializeField] public float _mouseSensetivity = 2f;
    private MultiplayerManager _multiplayerManager;
    private bool _hold = false;

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
        if (_hold) return;
        
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

    public void EquipWeapon(GameObject gunModel, int slot)
    {
        gunModel.SetActive(true);

        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            {"weapon", slot }
        };

        _multiplayerManager.SendMessage("weaponChange", data);
    }

    public void Restart(string jsonRestartInfo)
    {
        RestartInfo info = JsonUtility.FromJson<RestartInfo>(jsonRestartInfo);
        StartCoroutine(Hold());
        _player.transform.position = new Vector3(info.x, 0, info.z);
        _player.SetInput(0, 0, 0);

        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            {"pX", info.x },
            {"pY", 0 },
            {"pZ", info.z },
            {"vX", 0 },
            {"vY", 0 },
            {"vZ", 0 },
            {"rX", 0 },
            {"rY", 0 }
        };
        _multiplayerManager.SendMessage("move", data);
    }

    private IEnumerator Hold()
    {
        _hold = true;
        yield return new WaitForSecondsRealtime(_restartDelay);
        _hold = false;
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

[System.Serializable]
public struct RestartInfo
{
    public float x;
    public float z;
}