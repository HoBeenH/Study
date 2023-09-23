namespace Script.Map.Parameters
{
    [System.Serializable]
    public class SpawnParameter
    {
        public int SlotID { get; private set; }
        public int SpawnGroupID { get; private set; }

        public void Init(int slotID, int groupID)
        {
            this.SlotID = slotID;
            this.SpawnGroupID = groupID;
        }
    }
}