/// <summary>
/// An invokable class, that performs a simple action. called done when some external condition is met.
/// </summary>
public interface ITriggerable
{
    /// <summary>
    /// Triggers the effect.
    /// </summary>
    public void Trigger();
}