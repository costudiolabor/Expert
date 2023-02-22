using System;
using Unity.RenderStreaming;
using UnityEngine;

public class InputReceiverData : DataChannelBase {
    
    public event Action<byte[]> OnMessageEvent; 
    
    protected override void OnMessage(byte[] bytes) {
         OnMessageEvent?.Invoke(bytes);
    }

    public void UnSubscribe()
    {
        OnMessageEvent = null;
    }
}
