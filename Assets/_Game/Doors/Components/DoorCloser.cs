/// <summary>
/// Simple component for triggering a door to close when the player enters within range.
/// </summary>
public class DoorCloser : DoorAction
{
    protected override void DoAction()
    {
        _controller.CloseDoor();
    }
}
