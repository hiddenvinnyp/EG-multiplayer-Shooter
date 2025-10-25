using System;
using UnityEngine;

public class PlayerCharacter : Character
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _cameraPoint;
    [SerializeField] private float _minHeadAngle = -90f;
    [SerializeField] private float _maxHeadAngle = 90f;
    [SerializeField] private float _jumpForce = 50f;
    [SerializeField] private CheckFly _checkFly;
    [SerializeField] private float _jumpDelay = .2f;
    private float _inputH;
    private float _inputV;
    private float _rotateY;
    private float _currentRotateX;
    private float _jumpTime;

    public bool IsCrouch { get; private set; } = false;

    public void SetInput(float h, float v, float rotateY)
    {
        _inputH = h;
        _inputV = v;
        _rotateY += rotateY;
    }

    public void GetMoveInfo(out Vector3 position, out Vector3 velocity, out float rotateX, out float rotateY)
    {
        position = transform.position;
        velocity = _rigidbody.linearVelocity;

        rotateX = _head.localEulerAngles.x;
        rotateY = transform.eulerAngles.y;
    }

    public void RotateX(float value)
    {
        _currentRotateX = Mathf.Clamp(_currentRotateX + value, _minHeadAngle, _maxHeadAngle);
        _head.localEulerAngles = new Vector3(_currentRotateX, 0, 0);
    }

    public void Jump()
    {
        if (_checkFly.IsFly) return;
        if (Time.time - _jumpTime < _jumpDelay) return;
        _jumpTime = Time.time;
        _rigidbody.AddForce(0, _jumpForce, 0, ForceMode.VelocityChange);
        //_rigidbody.AddForceAtPosition(new Vector3(0, _jumpForce, 0), Vector3.up, ForceMode.Impulse);
    }
    public void Crouch(bool isCrouch)
    {
        if (IsCrouch == isCrouch) return;

        IsCrouch = isCrouch;

    }

    private void Start()
    {
        Transform camera = Camera.main.transform;
        camera.parent = _cameraPoint;        
        camera.localPosition = Vector3.zero;
        camera.localRotation = Quaternion.identity;
    }

    void FixedUpdate()
    {
        Move();
        RotateY();
    }

    private void Move()
    {
        Vector3 velocity = (transform.forward * _inputV + transform.right * _inputH).normalized * speed;
        velocity.y = _rigidbody.linearVelocity.y;
        base.velocity = velocity;
        _rigidbody.linearVelocity = base.velocity;
    }

    private void RotateY()
    {
        _rigidbody.angularVelocity = new Vector3(0, _rotateY, 0);
        _rotateY = 0;
    }

    /*private void OnCollisionStay(Collision collision)
    {
        var contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (contactPoints[i].normal.y > .45f) _isFly = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        _isFly = true;
    }*/
}
