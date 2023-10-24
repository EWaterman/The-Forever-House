using UnityEngine;

/// <summary>
/// Simple component for triggering a door to do something when the player enters within range.
/// </summary>
[RequireComponent(typeof(Collider))]
public abstract class DoorAction : MonoBehaviour
{
    [SerializeField] protected DoorController _controller;

    void OnTriggerEnter(Collider other)
    {
        if (other.IsPlayer())
        {
            DoAction();
        }
    }

    protected abstract void DoAction();
}
