using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Midbaryom.Core.Config
{

    [CreateAssetMenu(menuName = "ScriptableObjects/Config/New Game Config")]
    public class SpawnConfigSO : ScriptableObject
    {
        public float SpawnRadius;
        public float ReturnRadius;
        public int MaxMobsCount;
        public SpawnerInfo[] SpawnerInfos;

        public EntityTagSO ConductMob(IReadOnlyList<IEntity> activeList)
        {
            EntityTagSO spawnerInfo = null;
            int maxValue = MaxWeight();
            int previousValue = 0;
            int randomize = UnityEngine.Random.Range(0, maxValue);

            for (int i = 0; i < SpawnerInfos.Length; i++)
            {
                SpawnerInfo current = SpawnerInfos[i];
                int spawnWeight = current.SpawnWeight;
                if (spawnWeight > 0 && randomize < previousValue + spawnWeight)
                {
                    var tag = current.Tag;
                    if (activeList.Where(x => x.ContainTag(tag)).Count() < current.MaxOfSameAnimal)
                    {
                        spawnerInfo = tag;
                        break;
                    }
                }

                previousValue += spawnWeight;
            }

            if (spawnerInfo == null)
                spawnerInfo = ConductMob(activeList);

            return spawnerInfo;

            int MaxWeight()
            {
                int maxWeight = 0;
                for (int i = 0; i < SpawnerInfos.Length; i++)
                    maxWeight += SpawnerInfos[i].SpawnWeight;
                return maxWeight;
            }
        }
    }
    [Serializable]
    public class SpawnerInfo
    {
        public EntityTagSO Tag;
        public int SpawnWeight;
        public int MaxOfSameAnimal;
    }
}