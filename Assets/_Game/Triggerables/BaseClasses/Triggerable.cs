using UnityEngine;

/// <summary>
/// An invokable class, that performs a simple action. called done when some external condition is met.
///  Unity hates interfaces so use this abstract class when we need to fetch components.
/// </summary>
public abstract class Triggerable : MonoBehaviour, ITriggerable
{
    /// <summary>
    /// Triggers the effect.
    /// </summary>
    public abstract void Trigger();
}