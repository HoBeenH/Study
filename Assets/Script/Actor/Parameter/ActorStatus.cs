using Script.TableParser;

namespace Script.Actor.Parameter
{
    [System.Serializable]
    public class ActorStatus
    {
        public int CurrentHP => m_CurrentHP;
        private int m_CurrentHP;

        public int MaxHP => m_MaxHP;
        private int m_MaxHP;
        
        public float Speed => m_Speed;
        private float m_Speed;
        
        public int Damage => m_Damage;
        private int m_Damage;

        public void SetData(ActorTableData data)
        {
            m_MaxHP = data.HP;
            m_CurrentHP = data.HP;
            m_Speed = data.Speed;
            m_Damage = data.Damage;
        }
    }
}
