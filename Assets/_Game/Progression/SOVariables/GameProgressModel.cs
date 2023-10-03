using UnityEngine;

/// <summary>
/// Holds the state of progression through the game.
/// </summary>
[CreateAssetMenu(menuName = "GameProgress/GameProgressModel")]
public class GameProgressModel : ScriptableObject
{
    [field:SerializeField, Tooltip("The loop number that we're currently on.")]
    public int LoopNumber { get; private set; } = 0;

    public void ResetState()
    {
        LoopNumber = 0;
    }

    public int IncrementNumLoops(int numLoopsDone=1)
    {
        LoopNumber += numLoopsDone;
        return LoopNumber;
    }
}
