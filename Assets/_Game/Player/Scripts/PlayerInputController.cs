using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages player input system at a high level, allowing us to switch between input methods.
/// </summary>
public class PlayerInputController : MonoBehaviour
{
    [SerializeField] PlayerInput _input;

    public void EnablePlayerInput()
    {
        _input.enabled = true;
    }

    public void DisablePlayerInput()
    {
        _input.enabled = false;
    }
}
