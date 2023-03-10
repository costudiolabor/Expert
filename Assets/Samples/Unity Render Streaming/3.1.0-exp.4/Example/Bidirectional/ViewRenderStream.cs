using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


    public class ViewRenderStream : MonoBehaviour
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button setUpButton;
        [SerializeField] private Button hangUpButton;
        [SerializeField] private RawImage localVideoImage;
        [SerializeField] private RawImage remoteVideoImage;
        [SerializeField] private GameObject panelMenu;
        

        public event Action
            StartButtonEvent,
            SetUpButton,
            HangUpButton;

        public void Init()
        {
            startButton.onClick.AddListener(delegate { StartButtonEvent?.Invoke(); });
            setUpButton.onClick.AddListener(delegate { SetUpButton?.Invoke(); });
            hangUpButton.onClick.AddListener(delegate { HangUpButton?.Invoke(); });

            StartButtonEvent += ClickStartButton;
            SetUpButton += ClickSetUpButton;
            HangUpButton += ClickHangUpButton;

            startButton.interactable = true;
            setUpButton.interactable = false;
            hangUpButton.interactable = false;
        }


        public void ActiveRemoteVideo(bool isActive)
        {
            remoteVideoImage.gameObject.SetActive(isActive);
        }
        
        public void SetRemoteVideoImage(Texture texture)
        {
            remoteVideoImage.texture = texture;
        }
        
        public void SetLocalVideoImage(Texture texture)
        {
            localVideoImage.texture = texture;
        }
        
        private void ClickStartButton()
        {
            startButton.interactable = false;
            setUpButton.interactable = true;
        }

        private void ClickSetUpButton()
        {
            setUpButton.interactable = false;
            hangUpButton.interactable = true;
            panelMenu.SetActive(false);
        }

        private void ClickHangUpButton()
        {
            remoteVideoImage.texture = null;
            setUpButton.interactable = true;
            hangUpButton.interactable = false;
            localVideoImage.texture = null;
        }
    }
