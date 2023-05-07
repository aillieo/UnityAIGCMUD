namespace AillieoUtils.AIGC
{
    using System;
    using UnityEngine;

    public class AudioClipProperty
    {
        public AudioClipProperty(AudioClip value = null)
        {
            this.value = value;
        }

        public event Action onValueChanged;

        private AudioClip value;

        public AudioClip Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    this.onValueChanged?.Invoke();
                }
            }
        }

        public static explicit operator AudioClip(AudioClipProperty property)
        {
            if (property == null)
            {
                return null;
            }

            return property.value;
        }
    }
}
