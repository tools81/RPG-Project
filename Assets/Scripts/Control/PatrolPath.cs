using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        private void OnDrawGizmos() 
        {
            const float waypointGizmoRadius = 0.3f;
            var childCount = GetChildCount();
            Gizmos.color = Color.white;

            for(int i = 0; i < childCount; i++)
            {
                var position = GetWaypointPosition(i);
                var nextPosition = GetWaypointPosition(GetNextIndex(i, childCount));

                Gizmos.DrawSphere(position, waypointGizmoRadius);
                Gizmos.DrawLine(position, nextPosition);
            }
        }

        public int GetNextIndex(int i, int childCount)
        {
            if (i % childCount == childCount - 1)
            {
                return 0;
            }
            else
            {
                return i + 1;
            }
        }

        public Vector3 GetWaypointPosition(int i)
        {
            return transform.GetChild(i).position;
        }

        public int GetChildCount()
        {
            return transform.childCount;
        }
    }
}
