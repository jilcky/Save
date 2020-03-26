using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(UnityEngine.Video.VideoPlayer))]
public class VideoPlayerEvents : MonoBehaviour
{
    [System.Serializable] public class mEvent : UnityEvent { }

    [System.Serializable]
    public class mDescribedEvent
    {
        public UnityEvent mEvent;
        public void Invoke()
        {
            mEvent.Invoke();
        }
    }

    [System.Serializable]
    public class C_videoEvents
    {
        [Tooltip("Invoked when the VideoPlayer clock is synced back to its VideoTimeReference.")]
        public mDescribedEvent OnClockResyncOccured;
        [Tooltip("Errors such as HTTP connection problems are reported through this callback.")]
        public mDescribedEvent OnErrorReceived;
        [Tooltip("Invoked when a new frame is ready.")]
        //public mDescribedEvent OnFrameReady;
        //[Tooltip("Invoked when the VideoPlayer reaches the end of the content to play.")]
        public mDescribedEvent OnEndReached;
        [Tooltip("Invoked when the VideoPlayer preparation is complete.")]
        public mDescribedEvent OnPrepareCompleted;
        [Tooltip("Invoke after a seek operation completes.")]
        public mDescribedEvent OnSeekCompleted;
        [Tooltip("Invoked immediately after Play is called.")]
        public mDescribedEvent OnStarted;
    }

    public C_videoEvents events;
    public Text ErrorMessage;

    UnityEngine.Video.VideoPlayer videoPlayer;

    private void Awake()
    {
        videoPlayer = gameObject.GetComponent<UnityEngine.Video.VideoPlayer>();
        videoPlayer.clockResyncOccurred += ClockResyncOccurred;
        videoPlayer.errorReceived += ErrorReceived;
        //videoPlayer.frameReady += FrameReady;
        videoPlayer.loopPointReached += EndReached;
        videoPlayer.prepareCompleted += PrepareCompleted;
        videoPlayer.seekCompleted += SeekCompleted;
        videoPlayer.started += Started;

    
    }


    //Event to Unity.Event

    void ClockResyncOccurred(UnityEngine.Video.VideoPlayer vp, double seconds)
    {
        events.OnClockResyncOccured.Invoke();
    }
    void ErrorReceived(UnityEngine.Video.VideoPlayer vp, string message)
    {
        Debug.LogError("VideoPlayer error: "+message);
        if (ErrorMessage != null)
        {
            ErrorMessage.text = message;
        }
        events.OnErrorReceived.Invoke();
    }
    //This is likely to tax the CPU so this callback must be explicitly enabled through the VideoPlayer.frameReadyEventsEnabled property.
    //void FrameReady(UnityEngine.Video.VideoPlayer vp)
    //{
    //    events.OnFrameReady.Invoke();
    //}
    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        events.OnEndReached.Invoke();
    }
    void PrepareCompleted(UnityEngine.Video.VideoPlayer vp)
    {
        events.OnPrepareCompleted.Invoke();
    }
    void SeekCompleted(UnityEngine.Video.VideoPlayer vp)
    {
        events.OnSeekCompleted.Invoke();
    }
    void Started(UnityEngine.Video.VideoPlayer vp)
    {
        events.OnStarted.Invoke();
    }

}
