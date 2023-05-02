using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Midbaryom.Core.Tutorial
{
    public class DelayTutorial : BaseTask
    {
        [SerializeField]
        private UnityEvent OnTutorialStarted,OnTutorialFinished;
        [SerializeField]
        private float _delay;
        public override void TaskStarted()
        {
            OnTutorialStarted?.Invoke();

            StartCoroutine(Delay());
            base.TaskStarted();
        }

        private IEnumerator Delay()
        {
            yield return new WaitForSeconds(_delay);
            TaskCompleted();
        }
    }
}