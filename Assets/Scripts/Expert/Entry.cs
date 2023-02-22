using UnityEngine;

public class Entry : MonoBehaviour
{
   [SerializeField] private int frameRate = 100; 
   
   [SerializeField] private Streamer streamer;
   private  void Awake() {
      Application.targetFrameRate = frameRate;
      streamer.Initialize();
      
   }
}
