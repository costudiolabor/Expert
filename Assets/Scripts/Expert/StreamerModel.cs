using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.RenderStreaming;
using System.Threading.Tasks;
using Unity.RenderStreaming.InputSystem;
using Random = System.Random;

public class StreamerModel : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private RenderStreaming renderStreaming;
    [SerializeField] private SingleConnection singleConnection;
    [SerializeField] private VideoStreamSender videoStreamSender;
    [SerializeField] private VideoStreamReceiver videoStreamReceiver;
    [SerializeField] private AudioStreamReceiver audioStreamReceiver;
    [SerializeField] private AudioStreamSender audioStreamSender;
    [SerializeField] private AudioSource receiveAudioSource;
    [SerializeField] private InputSender inputSender;
#pragma warning restore 0649

    
    [SerializeField] private string connectionId;
   // [SerializeField] private string localId;
   // [SerializeField] private string remoteId;
    
    private InputManager _inputManager;
    private Vector2Int _screenSize;
    private RawImage _remoteVideoImage;
    private RawImage _localVideoImage;

    public event Action<Texture> OnUpdateReceiveTextureEvent, OnUpdateLocalTextureEvent;
    public event Action<string>  OnStoppedInputReceiverEvent, OnStartInputReceiverEvent;
    public event Action<string> OnUpdateMessageEvent;
    public event Action<string> SetConnectionIdEvent;
    public event Action CallUpEvent, HangUpEvent;

    public void Awake()
    {
        videoStreamSender.OnStartedStream += id => {
            _localVideoImage.texture = videoStreamSender.sourceWebCamTexture;
            videoStreamReceiver.enabled = true;
        };
        
        videoStreamReceiver.OnUpdateReceiveTexture += texture => { OnUpdateReceiveTextureEvent?.Invoke(texture); OnUpdateReceiveTexture(texture); };
        audioStreamReceiver.targetAudioSource = receiveAudioSource;
        audioStreamReceiver.OnUpdateReceiveAudioSource += source => { source.loop = true; source.Play(); };
    }

    void Start()
    {
        if (renderStreaming.runOnAwake) return;
        renderStreaming.Run();
        inputSender.OnStartedChannel += OnStartedChannel;
    }
    
    public void SetConnectId(string id) {
        connectionId = id;
        Debug.Log("ID: " + id);
    }

    // public string GetLocalID() {
    //     return localId;
    // }

    public void SetRemoteVideoImage(RawImage remoteVideoImage) {
        _remoteVideoImage = remoteVideoImage;
    }
    
    public void SetLocalVideoImage(RawImage localVideoImage) {
        _localVideoImage = localVideoImage;
    }
    
    void OnUpdateReceiveTexture(Texture texture) {
        SetInputChange();
    }
    
    void OnStartedChannel(string connectionId) {
        SetInputChange();
    }
    
    void SetInputChange() {
        if (inputSender == null || !inputSender.IsConnected ||
            _remoteVideoImage.texture == null) return;
        inputSender.SetInputRange(_remoteVideoImage);
        inputSender.EnableInputPositionCorrection(true);
    }
    
    public void CallUp() {
        CallUpEvent?.Invoke();
        videoStreamSender.enabled = true;
        audioStreamSender.enabled = true;
        singleConnection.CreateConnection(connectionId);
    }
   
    public void HangUp() {
        HangUpEvent?.Invoke();
        singleConnection.DeleteConnection(connectionId);
    }

    private void OnStartedStream(string id) {
        OnStartInputReceiverEvent?.Invoke(id);
    }
    
    private void OnStoppedStream(string id) {
        OnStoppedInputReceiverEvent?.Invoke(id);
    }
}

static class InputSenderExtension {
    public static void SetInputRange(this InputSender sender, RawImage image) {
        // correct pointer position
        Vector3[] corners = new Vector3[4];
        image.rectTransform.GetWorldCorners(corners);
        Camera camera = image.canvas.worldCamera;
        var corner0 = RectTransformUtility.WorldToScreenPoint(camera, corners[0]);
        var corner2 = RectTransformUtility.WorldToScreenPoint(camera, corners[2]);
        var region = new Rect(
            corner0.x,
            corner0.y,
            corner2.x - corner0.x,
            corner2.y - corner0.y
        );
        var size = new Vector2Int(image.texture.width, image.texture.height);
        sender.SetInputRange(region, size);
    }
}
