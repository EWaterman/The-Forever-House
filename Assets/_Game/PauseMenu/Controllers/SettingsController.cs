using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Controls all the settings that can be changed via the pause menu.
/// </summary>
public class SettingsController : MonoBehaviour
{
    [SerializeField] AudioMixer _mixer;

    /// <summary>
    /// Changes the master volume of the game.
    /// </summary>
    public void ChangeVolume(float volume)
    {
        _mixer.SetFloat("MasterVolume", volume);
    }
}
