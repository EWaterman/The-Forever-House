using DG.Tweening;
using UnityEngine;

/// <summary>
/// Controls simple doors that open when the player enters within range of them.
/// </summary>
public class DoorController : MonoBehaviour
{
    [SerializeField] Transform _doorObject;
    [SerializeField] float _rotationAmount = 120;
    [SerializeField] float _rotationDuration = 1;

    bool _isOpen = false;

    public void OpenDoor()
    {
        if (_isOpen)
            return;

        _isOpen = true;
        _doorObject.DORotate(new Vector3(0, _rotationAmount, 0), _rotationDuration);
    }

    public void CloseDoor()
    {
        if (!_isOpen)
            return;

        _isOpen = false;
        _doorObject.DORotate(Vector3.zero, _rotationDuration);
    }
}
