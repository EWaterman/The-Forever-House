/// <summary>
/// Simple component for triggering a door to open when the player enters within range.
/// </summary>
public class DoorOpener : DoorAction
{
    protected override void DoAction()
    {
        _controller.OpenDoor();
    }
}
