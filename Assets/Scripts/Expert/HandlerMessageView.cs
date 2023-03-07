using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandlerMessageView : AnimatedView
{
    [SerializeField] private TMP_Text textLocalId;

    [SerializeField] private TMP_Text textInMessage;
    [SerializeField] private TMP_InputField inputMessage;
    [SerializeField] private Button buttonSendMessage;
    [SerializeField] private GameObject panelButtonCloseCall;
    [SerializeField] private Button buttonOpenCall;
    [SerializeField] private Button buttonCloseCall;
    [SerializeField] private GameObject panelCallUp;
    [SerializeField] private TMP_InputField inputId;
    [SerializeField] private Button buttonCallUp;
    [SerializeField] private GameObject panelInComingCall;
    [SerializeField] private TMP_Text infoComingCall;
    [SerializeField] private Button buttonTakeCall;
    [SerializeField] private Button buttonResetCall;
    
    public event Action<string> InputIdEvent;
    public event Action<string> InputMessageEvent;
    public event Action SendMessageEvent;
    public event Action CallUpEvent; 
    public event Action TakeCallUpEvent; 
    public event Action ResetCallUpEvent; 

    public void Initialize() {
        inputId.onValueChanged.AddListener(input => { InputIdEvent?.Invoke(input); });
        inputMessage.onValueChanged.AddListener(input => { InputMessageEvent?.Invoke(input); });
        buttonSendMessage.onClick.AddListener(OnSendMessage);
        buttonCallUp.onClick.AddListener(OnCallUp);
        buttonTakeCall.onClick.AddListener(OnTakeCallUp);
        buttonResetCall.onClick.AddListener(OnResetCallUp);
        buttonOpenCall.onClick.AddListener(OnOpenCall);
        buttonCloseCall.onClick.AddListener(OnCloseCall);
        buttonTakeCall.interactable = false;
        buttonResetCall.interactable = false;
        buttonOpenCall.gameObject.SetActive(true);
        buttonCloseCall.gameObject.SetActive(false);
        panelInComingCall.SetActive(false);
    }
    
    public void SetLocalId(string id) {
        textLocalId.text = id;
    }

    public void SendMessageActive(bool isActive) {
        buttonSendMessage.interactable = isActive;
        buttonCallUp.interactable = isActive;
    }
    
    private void OnSendMessage() {
        if (string.IsNullOrEmpty(inputId.text)) return;
        if (string.IsNullOrEmpty(inputMessage.text)) return;
        Debug.Log("Пробуем отправить");
        SendMessageActive(false);
        SendMessageEvent?.Invoke();
    }
    
      private void OnCallUp() {
            if (string.IsNullOrEmpty(inputId.text)) return;
            Debug.Log("Пробуем дозвонится");
            panelCallUp.SetActive(false);
            SendMessageActive(false);
            CallUpEvent?.Invoke();
      }
      
      private void OnTakeCallUp() {
          buttonTakeCall.interactable = false;
          string text = $"Соединение\n  ID: {inputId.text}";
          infoComingCall.text = text;
          panelInComingCall.SetActive(false);
          buttonOpenCall.gameObject.SetActive(false);
          buttonCloseCall.gameObject.SetActive(true);
          TakeCallUpEvent?.Invoke();
      }

      private void OnResetCallUp() {
          string text = $"Вызов отклонен";
          infoComingCall.text = text;
          buttonTakeCall.interactable = false;
          buttonResetCall.interactable = false;
          panelInComingCall.SetActive(false);
          ResetCallUpEvent?.Invoke();
      }

      private void OnOpenCall() {
          panelCallUp.SetActive(true);
          buttonCallUp.interactable = true;
          buttonOpenCall.gameObject.SetActive(false);
          buttonCloseCall.gameObject.SetActive(true);
      }
      
      private void OnCloseCall() {
          panelCallUp.SetActive(false);
          buttonOpenCall.gameObject.SetActive(true);
          buttonCloseCall.gameObject.SetActive(false);
          OnResetCallUp();
      }

      public void InComingCall(string text) {
          infoComingCall.text = text;
          panelInComingCall.SetActive(true);
          buttonTakeCall.interactable = true;
          buttonResetCall.interactable = true;
      }
      
      public void TakeAnswer(string text) {
          infoComingCall.text = text;
          buttonResetCall.interactable = true;
      }
      
      public void TakePrivateMessage(string localId, string text) {
          inputId.text = localId;
          textInMessage.text = text;
      }
      
      public void Send() {
          SendMessageActive(true);
      }
      
}
