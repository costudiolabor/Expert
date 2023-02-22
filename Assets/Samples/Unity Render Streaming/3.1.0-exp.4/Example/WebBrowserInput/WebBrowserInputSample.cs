using Samples.Unity_Render_Streaming._3._1._0_exp._4.Example.Scripts;
using Unity.RenderStreaming;
using Unity.RenderStreaming.Samples;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.Unity_Render_Streaming._3._1._0_exp._4.Example.WebBrowserInput
{
    class WebBrowserInputSample : MonoBehaviour
    {
        [SerializeField] RenderStreaming renderStreaming;
        [SerializeField] Dropdown dropdownCamera;
        [SerializeField] Transform[] cameras;
        [SerializeField] CopyTransform copyTransform;

        RenderStreamingSettings settings;

        private void Awake()
        {
            settings = SampleManager.Instance.Settings;
        }

        // Start is called before the first frame update
        void Start()
        {
            dropdownCamera.onValueChanged.AddListener(OnChangeCamera);

            if (!renderStreaming.runOnAwake)
            {
                renderStreaming.Run(signaling: settings?.Signaling);
            }
        }

        void OnChangeCamera(int value)
        {
            copyTransform.SetOrigin(cameras[value]);
        }
    }
}
