using UnityEngine;

namespace Midbaryom.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Stats", fileName = "New Stat Data SO")]
    public class StatSO : TagSO
    {
        public StatType StatType;
        public float StartValue;

        [SerializeField]
        private bool _hasMinValue;
        [SerializeField]
        private float _minValue;

        [SerializeField]
        private  bool _hasMaxValue;
        [SerializeField]
        private float _maxValue;

        public void ClampValue(ref float value)
        {
            if (_hasMinValue)
                value = Mathf.Max(_minValue, value);
            if (_hasMaxValue)
                value = Mathf.Min(_maxValue, value);
        }
    }
}