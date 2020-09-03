using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nullspace
{
    public class ResourceCacheEntity
    {
        public GameObject GameObject { get; internal set; }
        public int LevelIndex { get; internal set; }
        public int ManagerId { get; internal set; }
        public int InstanceId { get; set; }
        public bool IsTimerOn { get; set; }
        public ResourceCacheBehaviour Behaviour { get; set; }
        public Transform Transform { get { return GameObject.transform; } }

        public void SetParent(Transform parent, bool worldStay)
        {
            GameObject.transform.SetParent(parent, worldStay);
        }

        public void SetPos(Vector3 pos)
        {
            Transform.position = pos; 
        }
        public void SetScale(Vector3 scale)
        {
            Transform.localScale = scale;
        }
        public void SetRotate(Quaternion q)
        {
            Transform.rotation = q;
        }

        public void SetActive(bool active)
        {
            GameObject.SetActive(active);
        }

        public void Release()
        {
            if (Behaviour != null)
            {
                Behaviour.Release();
            }
        }

        public void Destroy()
        {
            InstanceId = -1;
            ManagerId = -1;
            LevelIndex = -1;
            if (Behaviour != null)
            {
                Behaviour.ReleaseEventAndTimers();
            }
            GameObject.Destroy(GameObject);
        }
    }
}
