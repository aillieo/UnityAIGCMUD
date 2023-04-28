namespace AillieoUtils.AIGC
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class ArrayProperty<T> : IEnumerable<T>
    {
        public ArrayProperty(int size = 0)
        {
            this.value = new T[size];
        }

        public ArrayProperty(params T[] value)
        {
            this.value = value;
        }

        public event Action onLengthChanged;

        public event Action<int> onValueChanged;

        private T[] value;

        public int Length
        {
            get
            {
                return this.value.Length;
            }

            set
            {
                if (this.value.Length != value)
                {
                    Array.Resize(ref this.value, value);
                    onLengthChanged?.Invoke();
                }
            }
        }

        public T this[int index]
        {
            get
            {
                return this.value[index];
            }

            set
            {
                this.value[index] = value;
                onValueChanged?.Invoke(index);
            }
        }

        public ArrayProperty<T> Clear()
        {
            this.Length = 0;
            return this;
        }

        public ArrayProperty<T> Add(T item)
        {
            this.Length++;
            this[Length - 1] = item;
            return this;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)this.value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return value.GetEnumerator();
        }
    }
}
