using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles all movement related inputs of the main player character.
/// Uses the new input system and the "Player Input" component.
/// </summary>
public class PlayerMovementController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] float _walkSpeed;
    [SerializeField] float _gravity = -9.8f;

    [Header("Required Components")]
    [SerializeField] CharacterController _characterController;
    [SerializeField] Camera _camera;

    Transform _localRefTransform;
    Vector3 _velocity = Vector3.zero;
    Vector3 _movementInputDirection = Vector2.zero;

    void Awake()
    {
        _localRefTransform = _camera.transform;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // TODO: make it so you don't bounce down slopes https://forum.unity.com/threads/player-bouncing-down-slopes.828093/

        ApplyVerticalForcesToVelocity();
        ApplyHorizontalForcesToVelocity();
        MoveCharacter();
    }

    void OnMove(InputValue value)
    {
        // OnMove only gets triggered when input is changed. For keyboard, this means
        // that if we continue to hold down a key while rotating the camera, OnMove only
        // gets called once. As such we can't convert the movement to local coordinates
        // here, we need to do it in the update loop.
        _movementInputDirection = value.Get<Vector2>();
    }

    void ApplyVerticalForcesToVelocity()
    {
        if (_characterController.isGrounded)
        {
            // Reset the y velocity to some constant so we don't continue to build up
            // downward speed while grounded.
            _velocity.y = 0f;
        }
        else
        {
            // += so that we constantly accelerate as we fall. Apply delta time here
            // because our fall speed depends on how long we've fallen.
            _velocity.y += _gravity * Time.deltaTime;
        }
    }

    /// <summary>
    /// Convert the input movement vector to be relative to the direction the player
    /// is facing and apply it to the velocity.
    /// 
    /// Important to note:
    /// For a Vector2, x is left / right movement, y is forward / back.
    /// For a Vector3, x is still left / right, but z is the forward / back.
    /// </summary>
    void ApplyHorizontalForcesToVelocity()
    {
        //TODO: remove the _localRefTransform once we're actually rotating the player (not the camera), then just use our own transform.
        Vector3 localMovement =
            _localRefTransform.right * _movementInputDirection.x +
            _localRefTransform.forward * _movementInputDirection.y;

        localMovement *= _walkSpeed;

        _velocity.x = localMovement.x;
        _velocity.z = localMovement.z;
    }

    /// <summary>
    /// Actually perform the player movement. Should only be invoked from the Update()
    /// method and only once per frame so that we can ensure smooth movement.
    /// </summary>
    void MoveCharacter()
    {
        _characterController.Move(_velocity * Time.deltaTime);
    }
}