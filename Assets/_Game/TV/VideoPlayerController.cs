using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Wraps objects that can play videos.
/// </summary>
public class VideoPlayerController : MonoBehaviour   
{
    [SerializeField]
    protected VideoPlayer _player;

    /// <summary>
    /// Changes the video being played.
    /// </summary>
    public void SwitchVideo(VideoClip clip)
    {
        _player.clip = clip;
    }
}
