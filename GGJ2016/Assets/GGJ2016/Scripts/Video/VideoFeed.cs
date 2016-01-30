using UnityEngine;
using Sense.Injection;
using System.Linq;
using FlexiTweening;
using FlexiTweening.Extensions;
using Sense.Extensions;

[RequireComponent(typeof(CanvasGroup))]
public class VideoFeed : InjectableBehaviour
{
    private WebCamTexture _videoFeed;
    private WebCamDevice _videoFeedSource;

    [SerializeField] private Material _output;
    [SerializeField] private AnimationCurve _fadeEasing;

    private CanvasGroup _canvasGroup;
    private ITween fadeTween;
    

    protected override void OnPostInject()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        Debug.Log("Number of WebCam devices: " + WebCamTexture.devices.Count());
        WebCamTexture.devices.ForEach(dev => Debug.Log(dev.name));

        _videoFeedSource = WebCamTexture.devices.FirstOrDefault();
        if (_videoFeedSource.IsNotNull())
        {
            _videoFeed = new WebCamTexture(_videoFeedSource.name);
            _output.mainTexture = _videoFeed;
        }
    }

	public void Start()
    {
        if (_videoFeedSource.IsNull())
        {
            return;
        }
        _videoFeed.Play();
    }

    public void Pause()
    {
        if (_videoFeedSource.IsNull())
        {
            return;
        }
        _videoFeed.Pause();
    }

    public void Stop()
    {
        if (_videoFeedSource.IsNull())
        {
            return;
        }
        _videoFeed.Stop();
    }

    public void FadeTo(float alpha, float time)
    {
        fadeTween.SafelyAbort();
        fadeTween = _canvasGroup.TweenAlpha()
            .To(alpha, time)
            .Easing(_fadeEasing)
            .Start();
    }
}
