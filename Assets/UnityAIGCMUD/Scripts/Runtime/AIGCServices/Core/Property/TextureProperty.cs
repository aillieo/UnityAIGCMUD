namespace AillieoUtils.AIGC
{
    using System;
    using UnityEngine;

    public class TextureProperty
    {
        public TextureProperty(Texture2D value = null)
        {
            this.value = value;
        }

        public event Action onValueChanged;

        private Texture2D value;

        public Texture2D Value
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

        public static explicit operator Texture2D(TextureProperty property)
        {
            if (property == null)
            {
                return null;
            }

            return property.value;
        }
    }
}
