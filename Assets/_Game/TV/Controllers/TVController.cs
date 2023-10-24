using UnityEngine.Video;
using UnityEngine;

/// <summary>
/// Controls all public facing functionality for TV objects.
/// https://stackoverflow.com/questions/42747285/get-current-frame-texture-from-videoplayer
/// </summary>
public class TVController : VideoPlayerController
{
    [Header("Dependencies")]
    [SerializeField, Tooltip("The light component that will emit the TV light.")]
    Light _lightSource;

    [SerializeField, Tooltip("The mesh that references the screen material. Used to create a bloom effect using the actual colour of the video frame.")]
    Renderer _screenMesh;

    [Header("Config")]
    [SerializeField, Tooltip("The number of pixels to sample when calculating the average colour of the emission. Making this too high can tank framerate.")]
    int _numPixelsToSample = 50;

    [SerializeField, Range(1, 60), Tooltip("The number of frames to wait between re-computing the light source colour (the texture emission effect needs to happen per frame no matter what to avoid desync with the actual video). Making this too low can tank framerate.")]
    int _refreshRate = 1;

    [SerializeField, Range(0, 1020), Tooltip("The absolute colour difference (max 255 per RGBA channel) from the previous light colour before we allow the colour to change. This prevents excessive colour jitter on high variable videos.")]
    float _thresholdForLightChange = 0;

    [SerializeField, Range(0, 1), Tooltip("The weight that the average colour of each video frame has on the colour of the TV light. 0 means completely use the default colour plugged into the light. 1 means completely use the colour of the video.")]
    float _videoColourLightWeight = 1;

    Color32 _defaultColour;
    Color32 _currentColour;
    Texture2D _videoFrame;
    bool _isInitialized = false;

    void Awake()
    {
        // Textures can only be initialized in Awake or Start so set it to some simple value and we'll resize later.
        _videoFrame = new(1, 1);

        _defaultColour = _lightSource.color;
        _currentColour = _defaultColour;
    }

    void OnEnable()
    {
        _player.sendFrameReadyEvents = true;
        _player.frameReady += OnNewFrame;
    }

    void OnDisable()
    {
        _player.sendFrameReadyEvents = false;
        _player.frameReady -= OnNewFrame;
    }

    void OnNewFrame(VideoPlayer source, long frameIdx)
    {
        RenderTexture videoFrameTexture = source.texture as RenderTexture;
        if (!_isInitialized)
        {
            // We only do this once because we assume that all frames of the video have the same dimensions.
            _videoFrame.Reinitialize(videoFrameTexture.width, videoFrameTexture.height);
            _isInitialized = true;
        }

        ReadPixelsFromVideoFrame(videoFrameTexture);
        ApplyTextureToScreenEmission();

        if (frameIdx % _refreshRate != 0)
            return;

        ApplyColourToLightSource();
    }

    void ReadPixelsFromVideoFrame(RenderTexture videoFrameTexture)
    {
        RenderTexture.active = videoFrameTexture;
        _videoFrame.ReadPixels(new Rect(0, 0, videoFrameTexture.width, videoFrameTexture.height), 0, 0);
        _videoFrame.Apply();
        RenderTexture.active = null;
    }

    void ApplyTextureToScreenEmission()
    {
        // index 1 here because it's the second sub-mesh.
        _screenMesh.materials[1].SetTexture("_EmissionMap", _videoFrame);
    }

    void ApplyColourToLightSource()
    {
        Color32[] texColors = _videoFrame.GetPixels32();
        int numPixels = texColors.Length;
        int actualNumSamplesToSample = Mathf.Min(_numPixelsToSample, numPixels);

        float r = 0;
        float g = 0;
        float b = 0;
        float a = 0;

        for (int i = 0; i < actualNumSamplesToSample; i++)
        {
            int pixelToSample = Random.Range(0, numPixels);

            r += texColors[pixelToSample].r;
            g += texColors[pixelToSample].g;
            b += texColors[pixelToSample].b;
            a += texColors[pixelToSample].a;
        }
        r /= actualNumSamplesToSample;
        g /= actualNumSamplesToSample;
        b /= actualNumSamplesToSample;
        a /= actualNumSamplesToSample;

        float defaultColourEmissionWeight = 1 - _videoColourLightWeight;
        r = (r * _videoColourLightWeight) + (_defaultColour.r * defaultColourEmissionWeight);
        g = (g * _videoColourLightWeight) + (_defaultColour.g * defaultColourEmissionWeight);
        b = (b * _videoColourLightWeight) + (_defaultColour.b * defaultColourEmissionWeight);
        a = (a * _videoColourLightWeight) + (_defaultColour.a * defaultColourEmissionWeight);

        Color32 newColour = new ((byte) r, (byte) g, (byte) b, (byte) a);

        float colourDiff = Mathf.Abs(_currentColour.r - r)
            + Mathf.Abs(_currentColour.g - g)
            + Mathf.Abs(_currentColour.b - b)
            + Mathf.Abs(_currentColour.a - a);

        if (colourDiff > _thresholdForLightChange)
        {
            _currentColour = newColour;
        }
        
        _lightSource.color = _currentColour;
    }
}
