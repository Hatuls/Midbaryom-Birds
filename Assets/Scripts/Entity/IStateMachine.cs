using System.Collections.Generic;

namespace Midbaryom.Core
{
    public interface IStateMachine: IUpdateable
    {
        IState CurrentState { get; }
        StateType CurrentStateType { get; }
        IReadOnlyDictionary<StateType, IState> StateDictionary { get; }

        void ChangeState(StateType stateType);
    }
}