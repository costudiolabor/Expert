using System;
using System.Threading.Tasks;
using Unity.RenderStreaming;
using UnityEngine;

public class HandlerMessageModel : MonoBehaviour
{
    [SerializeField] private StreamerModel renderStreamMain;
    [SerializeField] private SingleConnection connection;
    [SerializeField] private InputSenderData inputSenderData;
    [SerializeField] private InputReceiverData inputReceiverData;
    
    [SerializeField] private string localId;
    [SerializeField] private string remoteId;
    [SerializeField] private string textOutMessage;
    [SerializeField] private string tempRemoteId;
    
    [SerializeField] private bool isStartSender;

    public event Action SendEvent;
    public event Action<string> LocalIdEvent;
    public event Action<bool> SenderStartEvent;
    public event Action<string> InComingEvent; 
    public event Action<string> TakeAnswerEvent; 
    public event Action<string, string> TakePrivateMessageEvent;

    public void SetRemoteId(string id) {
        remoteId = id;
    }

    public void SetTextOutMessage(string text) {
        textOutMessage = text;
    }

    public void Initialize() {
        localId = UnityEngine.Random.Range(100, 999).ToString();
        LocalIdEvent?.Invoke(localId);
        AsyncStart();
    }
    
    private async void AsyncStart() {
        await Task.Delay(TimeSpan.FromSeconds(2.0f));
        Debug.Log("START HandlerMessage");
        inputReceiverData.OnMessageEvent += InComingMessage;
        CreateLocalConnection();
        
        inputSenderData.OnStartedChannel += id => {
            isStartSender = true;
             SenderStartEvent?.Invoke(true); 
        };

        inputSenderData.OnStoppedChannel += id => {
            isStartSender = false;
        };
    }
    
    private void InComingMessage(byte[] bytes) {
        Debug.Log("Входящие данные");
        string message = System.Text.Encoding.UTF8.GetString(bytes);
        ParseMessage(message);
    }
    
    private void ParseMessage(string message) {
        MessageData messageData = JsonUtility.FromJson<MessageData>(message);
        switch (messageData.typeMessage)
        {
            case TypeMessage.Call:
                InComingCall(messageData);
                break;
            case TypeMessage.AnswerCall:
                TakeAnswer(messageData);
                break;
            case TypeMessage.PrivateMessage:
                TakePrivateMessage(messageData);
                break;
        }
    }
    
    private void InComingCall(MessageData messageData) {
        string text = $"Входящий вызов\n от {messageData.id}";
        tempRemoteId = messageData.roomId;
        InComingEvent?.Invoke(text);
    }

    private void TakeAnswer(MessageData messageData) {
        tempRemoteId = messageData.roomId;
        string text = $"Соединение\n  ID: {tempRemoteId}";
        TakeAnswerEvent?.Invoke(text);
        StartMainStream();
    }

    private void TakePrivateMessage(MessageData messageData) {
        string text = $"Сообщение от {messageData.id}\n{messageData.textMessage}";
        TakePrivateMessageEvent?.Invoke(localId, text);
    }
    
    private void StartMainStream() {
        renderStreamMain.SetConnectId(tempRemoteId);
        renderStreamMain.CallUp();
    }
    
    private void CreateLocalConnection() {
        connection.CreateConnection(localId);
    }
    
    public void OnSendMessage() {
        if (isStartSender == false)
        {
            DeleteLocalConnection();
            CreateRemoteConnection();
        }
        SendPrivateMessage();
    }
    
    private void DeleteLocalConnection() {
        connection.DeleteConnection(localId);
    }

    private void CreateRemoteConnection() {
        connection.CreateConnection(remoteId);
    }
    
    private void SendPrivateMessage() { 
        MessageData messageData = new MessageData();
        messageData.id = localId;
        messageData.typeMessage = TypeMessage.PrivateMessage;
        messageData.textMessage = textOutMessage;
        string message = JsonUtility.ToJson(messageData);
        Send(message);
    }
    
    private async void Send(string message) {
        while (!isStartSender)
        {
            await Task.Yield();
        }
        Debug.Log("Данные пошли");
        inputSenderData.Send(message);
        SendEvent?.Invoke();
    }
    
    public void OnCallUp() {
        if (isStartSender == false) {
            DeleteLocalConnection();
            CreateRemoteConnection();
        }
        SendCall();
    }
    
    private void SendCall() {
        MessageData messageData = new MessageData();
        messageData.id = localId;
        messageData.typeMessage = TypeMessage.Call;
        messageData.roomId = UnityEngine.Random.Range(10000, 99999).ToString();
        string message = JsonUtility.ToJson(messageData);
        Send(message);
    }
    
    public void OnTakeCallUp() {
        SendAnswerCall();
        StartMainStream();
    }
    
    private void SendAnswerCall() {
        MessageData messageData = new MessageData();
        messageData.id = localId;
        messageData.typeMessage = TypeMessage.AnswerCall;
        messageData.roomId = tempRemoteId;
        string message = JsonUtility.ToJson(messageData);
        Send(message);
    }
    
    public void OnResetCallUp() {
        StopMainStream();
    }
    
    private void StopMainStream() {
        renderStreamMain.HangUp();
    }
    
}
