using System;
using System.Collections.Generic;
using System.Linq;

namespace Midbaryom.Core
{
    public class Stat : IStat
    {
        public event Action OnReset;
        public event Action<float> OnValueChanged;

        private readonly StatSO _statSO;
        private float _value;
        public Stat(StatSO stat)
        {
            _statSO = stat ;
            Value = StartValue;
        }
        public float StartValue => _statSO.StartValue;

        public float Value
        { 
            get => _value;
            set 
            {
                if (_value == value)
                    return;

                _value = value;
                _statSO.ClampValue(ref _value);
                OnValueChanged?.Invoke(Value);
            } 
        }

        public IEnumerable<TagSO> Tags
        {
            get
            {
                yield return _statSO;
            }
        }

        public void Reset()
        {
            OnReset?.Invoke();
            _value = _statSO.StartValue;
        }
    }

    
    public class StatHandler : IStatHandler
    {
        private readonly Dictionary<StatSO, IStat> _statDictionary;
        public StatHandler(params StatSO[] statSO)
        {
            int statCount = statSO.Length;
            _statDictionary = new Dictionary<StatSO, IStat>(statCount);
            for (int i = 0; i < statCount; i++)
            {
                StatSO stat = statSO[i];
                _statDictionary.Add(stat, new Stat(stat));
            }
        }

        public IStat this[StatSO stat]
        {
            get
            {
                if (_statDictionary.TryGetValue(stat, out IStat istat))
                    return istat;
                throw new Exception($"Stat Handler: stat was not found!\nName: {stat.name}");
            }
        }

        public IStat this[StatType statType] => _statDictionary.First(x=>x.Key.StatType== statType).Value;
    }

    public enum StatType
    {
        MovementSpeed = 0,
        Points = 1
    }
    public interface IStat : ITaggable
    {
        event Action OnReset;
        event Action<float> OnValueChanged;
        float StartValue { get; }
        float Value { get; set; }
        void Reset();
    }
    public interface IStatHandler
    {
        IStat this[StatSO stat] { get; }
        IStat this[StatType statType] { get; }
    }
}