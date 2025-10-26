using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private const string Grounded = "Grounded";
    private const string Speed = "Speed";
    private const string IsCrouch = "IsCrouch";

    [SerializeField] private Animator _animator;
    [SerializeField] private Animator _animatorBody;
    [SerializeField] private CheckFly _checkFly;
    [SerializeField] private Character _character;

    private void Update()
    {
        Vector3 localVelocity = _character.transform.InverseTransformVector(_character.velocity);
        float speed = localVelocity.magnitude / _character.speed;
        float sign = Mathf.Sign(localVelocity.z);

        _animator.SetFloat(Speed, speed * sign);
        _animator.SetBool(Grounded, _checkFly.IsFly == false);
        _animatorBody.SetBool(IsCrouch, _character.IsCrouch);
    }
}
