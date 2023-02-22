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
    //[SerializeField] private SingleConnection singleConnectionDial;
    [SerializeField] private VideoStreamSender videoStreamSender;
    [SerializeField] private VideoStreamReceiver receiveVideoViewer;
    [SerializeField] private AudioStreamReceiver receiveAudioViewer;
    [SerializeField] private AudioStreamSender audioStreamSender;
    [SerializeField] private AudioSource receiveAudioSource;
    [SerializeField] private InputSender inputSender;
    
    [SerializeField] private float timeOutConnection;
    [SerializeField] private float timerMessage;
    [SerializeField] private InputReceiverData inputReceiverData;
    [SerializeField] private InputSenderData inputSenderData;
    
#pragma warning restore 0649

    [SerializeField] private string localId;
    [SerializeField] private string remoteId;
    
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
            receiveVideoViewer.enabled = true;
        };
        
        receiveVideoViewer.OnUpdateReceiveTexture += texture => {
            OnUpdateReceiveTextureEvent?.Invoke(texture);
            OnUpdateReceiveTexture(texture);
        };
        receiveAudioViewer.targetAudioSource = receiveAudioSource;
        receiveAudioViewer.OnUpdateReceiveAudioSource += source => {
            source.loop = true;
            source.Play();
        };
    }

    void Start()
    {
        if (renderStreaming.runOnAwake) return;
        renderStreaming.Run();
        inputSender.OnStartedChannel += OnStartedChannel;

        inputReceiverData.OnStartedChannel += id => { Debug.Log("START InputReceiverData " + id); };
        
        //StartListening(localId);
    }
    
    public void SetConnectionId(string id) {
        remoteId = id;
        Debug.Log("ID: " + id);
    }

    public string GetLocalID() {
        return localId;
    }

    public void SetRemoteVideoImage(RawImage remoteVideoImage) {
        _remoteVideoImage = remoteVideoImage;
    }
    
    public void SetLocalVideoImage(RawImage localVideoImage) {
        _localVideoImage = localVideoImage;
    }

    // private async void  StartListening(string id) {
    //     await Task.Delay(TimeSpan.FromSeconds(timeOutConnection));
    //       Debug.Log("START LISTENER");
    //     singleConnectionDial.CreateConnection(id);
    //    
    //     inputReceiverData.OnMessageEvent += OnMessage;
    //     
    //     CallUpEvent += () => { StartDialing(remoteId); };
    //     HangUpEvent += () => { StopDialing(localId); };
    // }  
    
    // private  void  StopListening(string id) {
    //     Debug.Log("STOP LISTENER");
    //     singleConnectionDial.DeleteConnection(id);
    //     inputReceiverData.OnMessageEvent -= OnMessage;
    //     
    //     CallUpEvent = null;
    //     HangUpEvent = null;
    // }  
    
    private void OnMessage(byte[] bytes) {
        string message = System.Text.Encoding.UTF8.GetString(bytes);
        MessageData messageData = JsonUtility.FromJson<MessageData>(message);
        HandlerMessage(messageData);
    }

    private void HandlerMessage(MessageData messageData) {
        string textMessage = " Сообщение от: " + messageData.name + 
                             "\n ID: " + messageData.id + "\n remoteID: " +
                             messageData.remoteId + "\n Type Message: " + messageData.typeMessage;
        OnUpdateMessageEvent?.Invoke(textMessage);
        Debug.Log(textMessage);
        if (messageData.typeMessage == TypeMessage.Call) {
           //StartConnection(messageData.remoteId.ToString());
        }
    }

    // private void StartConnection(string id) {
    //     StopDialing(localId);
    //     CallUpEvent = null;
    //     HangUpEvent = null;
    //     HangUpEvent += () => {
    //         StopRemoteConnection(id);
    //     };
    //     SetConnectionId(id);
    //     SetConnectionIdEvent?.Invoke(id);
    //     RemoteConnection(id);
    // }

    // private void StartDialing(string id) {
    //     StopListening(localId);
    //     StartListening(id);
    //     inputSenderData.OnStartedChannel += id => {
    //         Debug.Log("Start InputSenderData " + id);
    //         ConnectionRequest();
    //     };
    // }

    // private void StopDialing(string id) {
    //     StopListening(remoteId);
    //     StartListening(id);
    //     inputSenderData.OnStartedChannel = null;
    // }


    private void RemoteConnection(string id) {
        videoStreamSender.enabled = true;
        audioStreamSender.enabled = true;
        singleConnection.CreateConnection(remoteId);
        receiveVideoViewer.OnStartedStream += OnStartedStream ;
        receiveVideoViewer.OnStoppedStream += OnStoppedStream;
    }

    private void StopRemoteConnection(string id) {
        singleConnection.DeleteConnection(remoteId);
        receiveVideoViewer.OnStartedStream -= OnStartedStream ;
        receiveVideoViewer.OnStoppedStream -= OnStoppedStream;
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
    }
   
    public void HangUp() {
        HangUpEvent?.Invoke();
    }

    private void OnStartedStream(string id) {
        OnStartInputReceiverEvent?.Invoke(id);
    }
    
    private void OnStoppedStream(string id) {
        OnStoppedInputReceiverEvent?.Invoke(id);
    }

    private async void ConnectionRequest() {
        var messageData = new MessageData();
        messageData.name = "Абонент № 1";
        messageData.id = int.Parse(localId);
        messageData.remoteId = UnityEngine.Random.Range(10000, 9000);
        messageData.typeMessage = TypeMessage.Call;
        await SendMessage(messageData);
    }
    
    private async Task SendMessage(MessageData messageData) {
        var message = JsonUtility.ToJson(messageData);
        await Task.Yield(); 
        inputSenderData.Send(message);
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
