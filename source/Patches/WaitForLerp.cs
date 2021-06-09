using System;
using System.Collections;
using UnityEngine;

namespace TownOfUs
{
    public class WaitForLerp : IEnumerator
    {
        public WaitForLerp(float seconds, System.Action<float> act)
        {
            duration = seconds;
            this.act = act;
        }

        public object Current
        {
            get
            {
                return null;
            }
        }

        public bool MoveNext()
        {
            timer = Mathf.Min(timer + Time.deltaTime, duration);
            act(timer / duration);
            return timer < duration;
        }

        public void Reset()
        {
            timer = 0f;
        }

        private float duration;

        private float timer;

        private System.Action<float> act;
    }
}
