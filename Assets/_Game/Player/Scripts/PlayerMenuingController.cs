using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles all menu/settings related inputs of the main player character.
/// Uses the new input system and the "Player Input" component.
/// </summary>
public class PlayerMenuingController : MonoBehaviour
{
    [Header("Broadcasting On")]
    [SerializeField] EmptyEventChannel _pauseEvent;

    void OnPause(InputValue value)
    {
        // Simply forward the event on to the pause manager
        _pauseEvent.RaiseEvent();
    }
}
