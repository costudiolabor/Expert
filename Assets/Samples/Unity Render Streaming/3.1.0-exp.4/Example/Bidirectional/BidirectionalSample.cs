using System.Linq;
using Samples.Unity_Render_Streaming._3._1._0_exp._4.Example.Receiver;
using Samples.Unity_Render_Streaming._3._1._0_exp._4.Example.Scripts;
using Unity.RenderStreaming;
using Unity.RenderStreaming.Samples;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.Unity_Render_Streaming._3._1._0_exp._4.Example.Bidirectional
{
    
    static class InputSenderExtension
    {
        public static void SetInputRange(this InputSender sender, RawImage image)
        {
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
    
    class BidirectionalSample : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private RenderStreaming renderStreaming;
        [SerializeField] private Button startButton;
        [SerializeField] private Button setUpButton;
        [SerializeField] private Button hangUpButton;
        [SerializeField] private RawImage localVideoImage;
        [SerializeField] private RawImage remoteVideoImage;
        [SerializeField] private AudioSource receiveAudioSource;
        [SerializeField] private VideoStreamSender videoStreamSender;
        [SerializeField] private VideoStreamReceiver receiveVideoViewer;
        [SerializeField] private AudioStreamSender audioStreamSender;
        [SerializeField] private AudioStreamReceiver receiveAudioViewer;
        [SerializeField] private SingleConnection singleConnection;
        [SerializeField] private InputSender inputSender;
        
#pragma warning restore 0649

        [SerializeField] private string connectionId;
        private RenderStreamingSettings settings;

        void Awake()
        {
            hangUpButton.gameObject.SetActive(false);
            startButton.onClick.AddListener(() =>
            {
                videoStreamSender.enabled = true;
                startButton.interactable = false;
                startButton.gameObject.SetActive(false);
                audioStreamSender.enabled = true;
                setUpButton.interactable = true;
            });
            setUpButton.onClick.AddListener(SetUp);
            hangUpButton.onClick.AddListener(HangUp);
            videoStreamSender.OnStartedStream += id => receiveVideoViewer.enabled = true;
            videoStreamSender.OnStartedStream += _ => localVideoImage.texture = videoStreamSender.sourceWebCamTexture;
            receiveVideoViewer.OnStartedStream += id =>
            {
                //inputSender.OnStartedChannel += OnStartedChannel;
                Debug.Log(" START receiveVideoViewer :" + id);
            }; 
            
            videoStreamSender.OnStartedStream += id => Debug.Log(" Start VideoStreamSender :" + id);
            
            
            

            settings = SampleManager.Instance.Settings;
            if (settings != null)
            {
                videoStreamSender.width = (uint)settings.StreamSize.x;
                videoStreamSender.height = (uint)settings.StreamSize.y;
            }

            //
            receiveVideoViewer.OnUpdateReceiveTexture += OnUpdateReceiveTexture;
            //

            Microphone.devices.Select(x => new Dropdown.OptionData(x)).ToList();
            receiveAudioViewer.targetAudioSource = receiveAudioSource;
            receiveAudioViewer.OnUpdateReceiveAudioSource += source =>
            {
                source.loop = true;
                source.Play();
            };
        }

        void Start()
        {
            if (renderStreaming.runOnAwake)
                return;
            renderStreaming.Run(signaling: settings?.Signaling);
            inputSender.OnStartedChannel += OnStartedChannel;
        }

        //
        void OnUpdateReceiveTexture(Texture texture)
        {
            remoteVideoImage.texture = texture;
            SetInputChange();
        }

        void OnStartedChannel(string connectionId)
        {
            SetInputChange();
        }

        void SetInputChange()
        {
            if (inputSender == null || !inputSender.IsConnected || remoteVideoImage.texture == null)
                return;
            inputSender.SetInputRange(remoteVideoImage);
            inputSender.EnableInputPositionCorrection(true);
        }

        //

        private void SetUp()
        {
            startButton.gameObject.SetActive(false);
            
            hangUpButton.gameObject.SetActive(true);
            setUpButton.gameObject.SetActive(false);

            if (settings != null)
            {
                receiveVideoViewer.SetCodec(settings.ReceiverVideoCodec);
                videoStreamSender.SetCodec(settings.SenderVideoCodec);
            }

            //
            //connectionId = "90851";
            //
            
            //inputSender.OnStartedChannel += OnStartedChannel;
            singleConnection.CreateConnection(connectionId);
        }

        private void HangUp()
        {
            singleConnection.DeleteConnection(connectionId);
            
            remoteVideoImage.texture = null;
            setUpButton.gameObject.SetActive(true);
            
            hangUpButton.gameObject.SetActive(false);
            localVideoImage.texture = null;
        }
    }
}