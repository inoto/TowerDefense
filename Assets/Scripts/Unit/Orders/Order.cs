using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class Order : MonoBehaviour
    {
        public bool IsActive;

        protected Transform _transform;
        protected Unit _unit;

        protected virtual void Awake()
        {
            _transform = transform;
            _unit = GetComponentInParent<Unit>();
        }

        public virtual void Pause()
        {
            IsActive = false;
        }

        public virtual void Activate()
        {
            IsActive = true;
        }
    }
}
