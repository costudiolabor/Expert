using System;
using UnityEngine;

[Serializable]
public class HandlerMessage : ViewOperator<HandlerMessageView>
{
    private HandlerMessageModel _handlerMessageModel;

    public void Initialize(HandlerMessageModel handlerMessageModel) {
        _handlerMessageModel = handlerMessageModel;
        base.CreateView();
        view.Open();
        SubscribeEvent();
        view.Initialize();
        _handlerMessageModel.Initialize();
    }

    private void SubscribeEvent() {
        _handlerMessageModel.SendEvent += view.Send;
        _handlerMessageModel.LocalIdEvent += view.SetLocalId;
        _handlerMessageModel.SenderStartEvent += view.SendMessageActive;
        _handlerMessageModel.InComingEvent += text => view.InComingCall(text);
        _handlerMessageModel.TakeAnswerEvent += text => view.TakeAnswer(text);
        _handlerMessageModel.TakePrivateMessageEvent += (localId, text) => view.TakePrivateMessage(localId, text);
        
        view.InputIdEvent += _handlerMessageModel.SetRemoteId;
        view.InputMessageEvent += _handlerMessageModel.SetTextOutMessage;
        view.SendMessageEvent += _handlerMessageModel.OnSendMessage;
        view.CallUpEvent += _handlerMessageModel.OnCallUp;
        view.TakeCallUpEvent += _handlerMessageModel.OnTakeCallUp;
        view.ResetCallUpEvent += _handlerMessageModel.OnResetCallUp;
    }
    
    public void Dispose(){
        _handlerMessageModel.SendEvent -= view.Send;
        _handlerMessageModel.LocalIdEvent -= view.SetLocalId;
        _handlerMessageModel.SenderStartEvent -= view.SendMessageActive;
        _handlerMessageModel.InComingEvent -= text => view.InComingCall(text);
        _handlerMessageModel.TakeAnswerEvent -= text => view.TakeAnswer(text);
        _handlerMessageModel.TakePrivateMessageEvent -= (localId, text) => view.TakePrivateMessage(localId, text);
        
        view.InputIdEvent -= _handlerMessageModel.SetRemoteId;
        view.InputMessageEvent -= _handlerMessageModel.SetTextOutMessage;
        view.SendMessageEvent -= _handlerMessageModel.OnSendMessage;
        view.CallUpEvent -= _handlerMessageModel.OnCallUp;
        view.TakeCallUpEvent -= _handlerMessageModel.OnTakeCallUp;
        view.ResetCallUpEvent -= _handlerMessageModel.OnResetCallUp;
    }
}
