using UnityEngine;

namespace Oneiromancer.EditorHelpers.Utils
{
    [System.Serializable]
    public class UnityLayer
    {
        public int Layer => _layerIndex;
        public LayerMask Mask => 1 << _layerIndex;
        
        [SerializeField] private int _layerIndex;
        
        public void Set(int layer)
        {
            if (layer <= 0 || layer >= 32) return;
            _layerIndex = layer;
        }
    }
}