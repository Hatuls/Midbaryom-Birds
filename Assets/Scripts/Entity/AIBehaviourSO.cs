using Midbaryom.Core;
using UnityEngine;

namespace Midbaryom.AI
{
    [CreateAssetMenu(menuName ="ScriptableObjects/AI/Config")]
    public class AIBehaviourSO :ScriptableObject
    {
        public float TargetDestinationRadius;

        public AIBrainConfig[] AIBrainConfigs;

        public AIBrainConfig GetConfig(StateType stateType)
        {
            for (int i = 0; i < AIBrainConfigs.Length; i++)
            {
                if (AIBrainConfigs[i].stateType == stateType)
                    return AIBrainConfigs[i];
            }
            throw new System.Exception("AI BRAIN CONFIG WAS NOT FOUND!");
        }
    }


    [System.Serializable]
    public class AIBrainConfig
    {
        public StateType stateType;

        public Vector2 MinMaxDuration;

        public float RandomDuration => UnityEngine.Random.Range(MinMaxDuration.x, MinMaxDuration.y);
    }
}