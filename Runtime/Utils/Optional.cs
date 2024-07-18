using UnityEngine;

namespace Oneiromancer.EditorHelpers.Utils
{
    [System.Serializable]
    public struct Optional<T>
    {
        public T Value => _value;
        public bool Enabled => _enabled;
        
        [SerializeField] private T _value;
        [SerializeField] private bool _enabled;

        public Optional(T value)
        {
            _value = value;
            _enabled = true;
        }

        public bool IsEnabled(out T value)
        {
            value = _value;
            return _enabled;
        }

        public bool Validate() => !_enabled || _value != null;

        public static implicit operator T(Optional<T> item) => item.Value;
    }
}