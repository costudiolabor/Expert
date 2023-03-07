using UnityEngine;

public class Entry : MonoBehaviour
{
   [SerializeField] private int frameRate = 100; 
   
   [SerializeField] private Streamer streamer;
   [SerializeField] private HandlerMessage handlerMessage;

   private  void Awake() {
      Application.targetFrameRate = frameRate;
      
      streamer.Initialize();
      HandlerMessageModel handlerMessageModel = streamer.GetHandlerMessageModel();
      handlerMessage.Initialize(handlerMessageModel);
   }
}
