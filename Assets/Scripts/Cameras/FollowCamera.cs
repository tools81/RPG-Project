using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Cameras
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] Transform target;

        void Update()
        {
            transform.position = target.position;
        }
    }
}