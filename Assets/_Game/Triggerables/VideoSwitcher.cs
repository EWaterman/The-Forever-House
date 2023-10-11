using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Swaps the video of a video player to a different clip
/// </summary>
public class VideoSwitcher : Triggerable
{
    [SerializeField, Tooltip("The component that is playing the video.")]
    VideoPlayerController _player;

    [SerializeField, Tooltip("The clip to swap to.")]
    VideoClip _clip;

    public override void Trigger()
    {
        _player.SwitchVideo(_clip);
    }
}
