using System;
using System.Collections.Generic;

namespace Midbaryom.Core
{
    /// <summary>
    /// Handles the destroy behaviour
    /// </summary>
    public interface IDestroyHandler
    {
        event Action<IEntity> OnDestroy;
        IReadOnlyList<IBehaviour> DestroyBehaviours { get; }
        void Destroy();
        void AddBehaviour(IBehaviour behaviour);
        void RemoveBehaviour(IBehaviour behaviour);
    }

    public class DestroyBehaviour : IDestroyHandler
    {
        public event Action<IEntity> OnDestroy;

        private readonly IEntity _entity;
        private List<IBehaviour> _behaviours;

        public IReadOnlyList<IBehaviour> DestroyBehaviours => _behaviours;

        public DestroyBehaviour(IEntity entity)
        {
            _behaviours = new List<IBehaviour>();
            _entity = entity;
        }

        public void AddBehaviour(IBehaviour behaviour)
            => _behaviours.Add(behaviour);
        public void RemoveBehaviour(IBehaviour behaviour)
            => _behaviours.Add(behaviour);



        public void Destroy()
        {
            for (int i = 0; i < DestroyBehaviours.Count; i++)
                DestroyBehaviours[i].ApplyBehaviour();

            OnDestroy?.Invoke(_entity);
        }
    }


}