using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEnhanced.SerializeInterfaces;

/// <summary>
/// A simple wrapper on a list of triggerables, allowing them all to be invoked at the same time (sequentially).
/// </summary>
public class TriggerableGroup : MonoBehaviour
{
    [SerializeField, RequireInterface(typeof(ITriggerable)), Tooltip("A list of triggerables to be invoked.")]
    List<Object> _triggerables;

    public void TriggerAll()
    {
        foreach (ITriggerable t in _triggerables.Cast<ITriggerable>())
        {
            t.Trigger();
        }
    }
}
