using System;
using UnityEngine;

namespace TowerDefense
{
    public class WizardMagic : MonoBehaviour
    {
        public event Action<WizardMagic> DurationEndedEvent;

        public float Duration = 10f;
        public float Rate = 1f;
        public Sprite Icon;
        public Color IconColor = Color.white;

        protected float timer = 0f;
        protected float timerDuration = 0f;

        protected void RaiseDurationEndedEvent()
        {
            DurationEndedEvent?.Invoke(this);
        }

        void OnEnable()
        {
            Reset();
        }

        void Reset()
        {
            timerDuration = 0f;
            timer = 0f;
        }

        void Update()
        {
            timer += Time.deltaTime;
            timerDuration += Time.deltaTime;
            if (timer >= Rate)
            {
                Tick();
                timer = 0f;
            }

            if (timerDuration >= Duration)
            {
                RaiseDurationEndedEvent();
                gameObject.SetActive(false);
            }
        }

        protected virtual void Tick()
        {

        }
    }
}