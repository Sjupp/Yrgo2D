using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardMovement : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 0f;
    [SerializeField]
    private float _accelerationSharpness = 1f;
    [SerializeField]
    private float _decelerationSharpness = 1f;
    private Rigidbody2D _rb = null;
    
    Vector3 _movementVector = Vector3.zero;
    [SerializeField]
    Vector3 _targetVector = Vector3.zero;

    //private bool _acceleratedOnce = false;
    //private bool _startedTimer = false;
    //private float _timer = 0f;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        _targetVector.x = Input.GetAxis("Horizontal");
        _targetVector.y = Input.GetAxis("Vertical");
        
        //_targetVector.Normalize();

        if (_targetVector.sqrMagnitude > 0)
        {
            //if(!_acceleratedOnce)
            //    _acceleratedOnce = true;

            _movementVector = Vector3.Lerp(_movementVector, _targetVector * _moveSpeed, _accelerationSharpness * Time.deltaTime);
        }
        else
        {
            //if (_acceleratedOnce && !_startedTimer)
            //{
            //    _startedTimer = true;
            //    _timer = Time.time;
            //    Debug.Log("started breaking");
            //}
            _movementVector = Vector3.Lerp(_movementVector, _targetVector, _decelerationSharpness * Time.deltaTime);
        }

        //if (_startedTimer && _movementVector.magnitude == 0)
        //{
        //    _acceleratedOnce = false;
        //    _startedTimer = false;
        //    Debug.Log("break time: " + (Time.time - _timer));
        //}
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _movementVector;
        //_rb.MovePosition(transform.position + _movementVector * _moveSpeed * Time.deltaTime);
    }
}
