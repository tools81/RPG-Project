using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using RPG.Control;
using UnityEngine;

namespace RPG.Cameras
{
    public class PlayerCameraSwitch : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera switchVirtualCameraTo = null;

        GameObject player;

        private void Awake() 
        {
            player = GameObject.FindWithTag("Player");
        }

        //Requires a collider
        private void OnTriggerEnter(Collider other) 
        {
            if (other.gameObject.tag == "Player")
            {
                SwitchCamera();
            }
        }

        public void SwitchCamera()
        {
            switchVirtualCameraTo.Priority = 100;
            Debug.Log(switchVirtualCameraTo.name);

            foreach (var camera in player.GetComponent<PlayerController>().GetVirtualCameras())
            {
                if (camera != switchVirtualCameraTo)
                {
                    camera.Priority = 0;
                }
            }
        }
    }
}
