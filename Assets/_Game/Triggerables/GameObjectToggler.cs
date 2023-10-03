using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Toggles objects on and off depending on the boolean result of a given event.
/// </summary>
public class GameObjectToggler : Triggerable
{
    [SerializeField, Tooltip("The objects to toggle ON when triggered.")]
    List<GameObject> _objectsTurnOn;

    [SerializeField, Tooltip("The objects to toggle OFF when triggered.")]
    List<GameObject> _objectsTurnOff;

    public override void Trigger()
    {
        foreach (GameObject obj in _objectsTurnOn)
        {
            obj.SetActive(true);
        }

        foreach (GameObject obj in _objectsTurnOff)
        {
            obj.SetActive(false);
        }
    }
}
