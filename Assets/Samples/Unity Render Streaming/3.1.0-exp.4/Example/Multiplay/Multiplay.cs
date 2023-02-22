using System.Collections.Generic;
using System.Linq;
using Samples.Unity_Render_Streaming._3._1._0_exp._4.Example.Scripts;
using Unity.RenderStreaming;
using Unity.RenderStreaming.Samples;
using UnityEngine;

namespace Samples.Unity_Render_Streaming._3._1._0_exp._4.Example.Multiplay
{
    class Multiplay : SignalingHandlerBase,
        IOfferHandler, IAddChannelHandler, IDisconnectHandler, IDeletedConnectionHandler
    {
        [SerializeField] GameObject prefab;

        [SerializeField] private List<string> connectionIds = new List<string>();
        [SerializeField] private List<Component> streams = new List<Component>();
        [SerializeField] private Dictionary<string, GameObject> dictObj = new Dictionary<string, GameObject>();

        private RenderStreamingSettings settings;

        void Awake()
        {
            settings = SampleManager.Instance.Settings;
        }

        public override IEnumerable<Component> Streams => streams;

        public void OnDeletedConnection(SignalingEventData eventData)
        {
            Disconnect(eventData.connectionId);
        }

        public void OnDisconnect(SignalingEventData eventData)
        {
            Disconnect(eventData.connectionId);
        }

        private void Disconnect(string connectionId)
        {
            if (!connectionIds.Contains(connectionId))
                return;
            connectionIds.Remove(connectionId);

            var obj = dictObj[connectionId];
            var sender = obj.GetComponentInChildren<StreamSenderBase>();
            var inputChannel = obj.GetComponentInChildren<InputReceiver>();
            var multiplayChannel = obj.GetComponentInChildren<MultiplayChannel>();

            dictObj.Remove(connectionId);
            Object.Destroy(obj);

            RemoveSender(connectionId, sender);
            RemoveChannel(connectionId, inputChannel);
            RemoveChannel(connectionId, multiplayChannel);

            streams.Remove(sender);
            streams.Remove(inputChannel);
            streams.Remove(multiplayChannel);

            if (ExistConnection(connectionId))
                DeleteConnection(connectionId);
        }

        public void OnOffer(SignalingEventData data)
        {
            if (connectionIds.Contains(data.connectionId))
            {
                Debug.Log($"Already answered this connectionId : {data.connectionId}");
                return;
            }
            connectionIds.Add(data.connectionId);

            var initialPosition = new Vector3(0, 3, 0);
            var newObj = Instantiate(prefab, initialPosition, Quaternion.identity);
            dictObj.Add(data.connectionId, newObj);

            var sender = newObj.GetComponentInChildren<StreamSenderBase>();

            if (sender is VideoStreamSender videoStreamSender && settings != null)
            {
                videoStreamSender.width = (uint)settings.StreamSize.x;
                videoStreamSender.height = (uint)settings.StreamSize.y;
                videoStreamSender.SetCodec(settings.SenderVideoCodec);
            }

            var inputChannel = newObj.GetComponentInChildren<InputReceiver>();
            var multiplayChannel = newObj.GetComponentInChildren<MultiplayChannel>();
            var playerController = newObj.GetComponentInChildren<PlayerController>();

            if (multiplayChannel.OnChangeLabel == null)
                multiplayChannel.OnChangeLabel = new ChangeLabelEvent();
            multiplayChannel.OnChangeLabel.AddListener(playerController.SetLabel);

            streams.Add(sender);
            streams.Add(inputChannel);
            streams.Add(multiplayChannel);

            AddSender(data.connectionId, sender);
            AddChannel(data.connectionId, inputChannel);
            AddChannel(data.connectionId, multiplayChannel);

            SendAnswer(data.connectionId);
        }

        /// todo(kazuki)::
        public void OnAddChannel(SignalingEventData data)
        {
            var obj = dictObj[data.connectionId];
            var channels = obj.GetComponentsInChildren<IDataChannel>();
            var channel = channels.FirstOrDefault(_ => !_.IsLocal && !_.IsConnected);
            channel?.SetChannel(data);
        }
    }
}
