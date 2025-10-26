using UnityEngine;

public class EnemyCharacter : Character
{
    [SerializeField] private Transform _head;
    public Vector3 targetPosition { get; private set; } = Vector3.zero;
    private float _velocityMagnitude = 0f;



    private void Start()
    {
        targetPosition = transform.position;
        _colliderCenterStand = _collider.center;
        _colliderHeightStand = _collider.height;
    }

    private void Update()
    {
        if (_velocityMagnitude > 0.1f)
        {
            float maxDistance = _velocityMagnitude * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, maxDistance);
        }
        else
        {
            transform.position = targetPosition;
        }        
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetSpeed(float value) => speed = value;

    public void SetMovement(in Vector3 position, in Vector3 velocity, in float averageInterval)
    {
        targetPosition = position + velocity * averageInterval;        
        _velocityMagnitude = velocity.magnitude;
        
        this.velocity = velocity;
    }

    public void SetRotateX(float value)
    {
        _head.localEulerAngles = new Vector3(value, 0, 0);
    }

    public void SetRotateY(float value)
    {
        transform.localEulerAngles = new Vector3 (0, value, 0);
    }

    /*public void SetCrouch(bool isCrouch)
    {
        if (IsCrouch == isCrouch) return;
        if (isCrouch)
        {
            _collider.center = _colliderCenterCrouch;
            _collider.height = _colliderHeightCrouch;
        }
        else
        {
            _collider.center = _colliderCenterStand;
            _collider.height = _colliderHeightStand;
        }
    }*/
}
