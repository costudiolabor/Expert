using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.RenderStreaming;
using UnityEngine;

public class ListenerStreamModel : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private string connectionId;
    [SerializeField] private float timeOutConnection;
    [SerializeField] private int messageID;
    [SerializeField] private RenderStreaming renderStreaming;
    [SerializeField] private SingleConnection singleConnection;
    
    [SerializeField] private InputReceiverData inputReceiverData;
    [SerializeField] private InputSenderData inputSenderData;
    
#pragma warning restore 0649

    void Start() {
        if (renderStreaming.runOnAwake) return;
        renderStreaming.Run();

        inputReceiverData.OnMessageEvent += OnMessage;
        
        inputReceiverData.OnStartedChannel += id =>
        {
            Debug.Log("Start InputReceiverData " + id);
        };
        
        inputSenderData.OnStartedChannel += id =>
        {
            Debug.Log("Start InputSenderData " + id);
            SendMessage();
        };
        
        StartConnection();
       
    }

    private void OnMessage(byte[] bytes) {
        string message = System.Text.Encoding.UTF8.GetString(bytes);
        MessageData messageData = JsonUtility.FromJson<MessageData>(message);
        HandlerMessage(messageData);
    }

    private void HandlerMessage(MessageData messageData) {
        Debug.Log(" Сообщение от: " + messageData.id);
    }
    
    private async void  StartConnection() {
        await Task.Delay(TimeSpan.FromSeconds(timeOutConnection));
        Debug.Log("StartConnection");
        singleConnection.CreateConnection(connectionId);
        
       
    }  
    
    private void StopConnection() {
        singleConnection.DeleteConnection(connectionId);
    }

    private async void SendMessage()
    {
        MessageData messageData = new MessageData();
        messageData.id = messageID;
        var message = JsonUtility.ToJson(messageData);
             
        while (true) {
            await Task.Delay(TimeSpan.FromSeconds(1));
            inputSenderData.Send(message);
        }
    }
}