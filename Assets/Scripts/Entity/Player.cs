using Midbaryom.Camera;
using Midbaryom.Inputs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Midbaryom.Core
{
    public class Player : MonoBehaviour, IPlayer
    {
        [SerializeField]
        private Entity _entity;
        [SerializeField]
        private Transform _cameraTransform;
        [SerializeField]
        private UnityEngine.Camera _camera;
        [SerializeField]
        private AimAssists _aimAssists;

        public IEntity Entity => _entity;
        public IEnumerable<TagSO> Tags => Entity.Tags;
        public PlayerController PlayerController { get; set; }
        public CameraManager CameraManager { get; set; }
        public IEnumerable<IUpdateable> UpdateCollection
        {
            get
            {
                yield return PlayerController;
                yield return CameraManager;
            }
        }



        private void Start()
        {
            CameraManager = new CameraManager(this, _camera, _cameraTransform);
            PlayerController = new PlayerController(Entity);
        }




        private void Update()
        {
            foreach (IUpdateable updateable in UpdateCollection)
                updateable.Tick();
        }
    }

    public interface IPlayer : ITaggable
    {
        IEntity Entity { get; }

        public PlayerController PlayerController { get; }
        CameraManager CameraManager { get; }
    }
}