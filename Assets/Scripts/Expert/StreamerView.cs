using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class StreamerView : AnimatedView
{
    [SerializeField] private Button callUpButton;
    [SerializeField] private Button hangUpButton;
    [SerializeField] private RawImage localVideoImage;
    [SerializeField] private RawImage remoteVideoImage;
    [SerializeField] private RectTransform panelNotice;
    [SerializeField] private TMP_Text textNotice;
    [SerializeField] private TMP_InputField inputID;
    [SerializeField] private TMP_Text localID;
   
    private Coroutine _refCallExpert;
    public event Action CallUpEvent, HangUpEvent ;
    public event Action<string> EndEditEvent;

    public  Texture localVideoTexture {
        set => localVideoImage.texture = value;
    }
    
    public  Texture remoteVideoTexture {
        set => remoteVideoImage.texture = value;
    }
    
    public RawImage GetRemoteVideoImage() {
        return remoteVideoImage;
    }
    
    public RawImage GetLocalVideoImage() {
        return localVideoImage;
    }
    
    private void Awake() {
        callUpButton.onClick.AddListener(() => CallUpEvent?.Invoke());
        hangUpButton.onClick.AddListener(() => HangUpEvent?.Invoke());
        inputID.onValueChanged.AddListener (delegate {ValueChangeCheck ();});
        inputID.onEndEdit.AddListener(delegate { EndEdit();});
        callUpButton.gameObject.SetActive(true);
        hangUpButton.gameObject.SetActive(false); 
        CallUpEvent += OnCallUp;
        HangUpEvent += OnHangUp;
    }

    public void SetTextLocalID(string id) {
        localID.text = id;
    }

    public void SetTextRemoteID(string id)
    {
        inputID.text = id;
    }
    
    private void EndEdit() {
    }
    
    private void ValueChangeCheck() {
        EndEditEvent?.Invoke(inputID.text);
    }
    
    public void SetNotice(string text) {
      panelNotice.gameObject.SetActive(true);
      textNotice.text = text;
    }

    public void StartInputReceiver(string id) {
    }
    
    public void StoppedInputReceiver(string id) {
        OnHangUp();
    }

    private void OnCallUp() {
        callUpButton.gameObject.SetActive(false);
        hangUpButton.gameObject.SetActive(true);
        panelNotice.gameObject.SetActive(false);
    }
    
    private void OnHangUp() {
        callUpButton.gameObject.SetActive(true);
        hangUpButton.gameObject.SetActive(false); 
        remoteVideoImage.texture = null;
        localVideoImage.texture = null;
        SetNotice("Абонент не отвечает");
    }
 
}

