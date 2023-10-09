using Script.Base.Pool;
using Script.Parameter.Enum;
using Script.TableParser;

namespace Script.Actor.Base
{
    public abstract class ActorBase : PoolMonoObj
    {
        protected EActorState m_ActorState = EActorState.Empty;
        
        protected ActorTableData m_TableData = null;

        protected abstract void InitActor();
        
        protected abstract void OnStateEnter();

        protected abstract void OnStateUpdate(float deltaTime);

        protected abstract void OnStateExit();

        public abstract bool IsDie();
        
        public void Init(ActorTableData data)
        {
            if (data == null)
                return;

            m_TableData = data;
            InitActor();
        }

        public bool IsState(EActorState state) => m_ActorState == state;

        public void SetState(EActorState state)
        {
            if (m_ActorState == state)
                return;
            
            OnStateExit();
            m_ActorState = state;
            OnStateEnter();
        }

        public void OnUpdate(float deltaTime)
        {
            if (IsDie())
                return;

            OnStateUpdate(deltaTime);
        }
        
        public void ClearModel()
        {
            m_TableData = null;
        }
    }
}
