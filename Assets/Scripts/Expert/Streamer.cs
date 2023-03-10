using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class Streamer : ViewOperator<StreamerView>
{
  [SerializeField] private StreamerModel prefabStreamerModel;
  private StreamerModel _streamerModel;
  
  public HandlerMessageModel GetHandlerMessageModel() {
      HandlerMessageModel handlerMessageModel = _streamerModel.GetComponent<HandlerMessageModel>();
      return handlerMessageModel;
  }

  public void Initialize() {
      base.CreateView();
      _streamerModel = Object.Instantiate(prefabStreamerModel);
      _streamerModel.SetRemoteVideoImage(view.GetRemoteVideoImage());
      _streamerModel.SetLocalVideoImage(view.GetLocalVideoImage());
      view.Open();
      //view.SetTextLocalID(_streamerModel.GetLocalID());
      SubscribeEvent();
  }
  
  public void CallUp() {
      _streamerModel.CallUp();
  }
  
  private void HangUp() {
      _streamerModel.HangUp();
  }
  
  private void SubscribeEvent() {
       view.CallUpEvent += CallUp;
       view.HangUpEvent += HangUp;
       view.EndEditEvent += _streamerModel.SetConnectId;
      _streamerModel.OnUpdateReceiveTextureEvent += texture => view.remoteVideoTexture = texture;
      _streamerModel.OnUpdateLocalTextureEvent += texture => view.localVideoTexture = texture;
      _streamerModel.OnStartInputReceiverEvent += view.StartInputReceiver;
      _streamerModel.OnStoppedInputReceiverEvent += view.StoppedInputReceiver;
      _streamerModel.OnUpdateMessageEvent += view.SetNotice;
      _streamerModel.SetConnectionIdEvent += view.SetTextRemoteID;
  }
  
  public void Disable() {
      view.CallUpEvent -= CallUp;
      view.HangUpEvent -= HangUp;
      view.EndEditEvent -= _streamerModel.SetConnectId;
      _streamerModel.OnStartInputReceiverEvent -= view.StartInputReceiver;
      _streamerModel.OnStoppedInputReceiverEvent -= view.StoppedInputReceiver;
      _streamerModel.OnUpdateMessageEvent -= view.SetNotice;
      _streamerModel.SetConnectionIdEvent -= view.SetTextRemoteID;

  }
  
  
  
  
  
}
