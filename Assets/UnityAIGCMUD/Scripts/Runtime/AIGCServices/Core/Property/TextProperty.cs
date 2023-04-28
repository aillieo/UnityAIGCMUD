namespace AillieoUtils.AIGC
{
    using System;

    public class TextProperty
    {
        public TextProperty(string value = null)
        {
            this.value = value;
        }

        public event Action onValueChanged;

        private string value;

        public string Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (string.CompareOrdinal(value, this.value) != 0)
                {
                    this.value = value;
                    onValueChanged?.Invoke();
                }
            }
        }

        public static explicit operator string(TextProperty property)
        {
            if (property == null)
            {
                return null;
            }

            return property.value;
        }
    }
}
