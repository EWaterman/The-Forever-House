using UnityEngine;

/// <summary>
/// Tracks how many times we've looped through the scene and invokes logic on certain loops.
/// </summary>
public class LoopingManager : MonoBehaviour
{
    [SerializeField, Tooltip("The scriptable object holding the game progress.")]
    GameProgressModel _gameProgress;

    [Header("Listening To")]
    [SerializeField, Tooltip("The event that triggers when the player completes a loop.")]
    EmptyEventChannel _playerLoopedEventChannel;

    [SerializeField, Tooltip("Maps loop numbers to a group of triggerables to invoke when we hit that loop.")]
    SerializableDict<int, TriggerableGroup> _loopNumToTriggerGroup;

    void OnEnable()
    {
        _playerLoopedEventChannel.Listeners += OnPlayerLooped;
    }

    void OnDisable()
    {
        _playerLoopedEventChannel.Listeners -= OnPlayerLooped;
    }

    void Awake()
    {
        _gameProgress.ResetState();
    }

    void OnPlayerLooped()
    {
        int numLoops = _gameProgress.IncrementNumLoops();
        if (_loopNumToTriggerGroup.ContainsKey(numLoops))
        {
            _loopNumToTriggerGroup[numLoops].TriggerAll();
        }
    }
}
