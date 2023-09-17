namespace Script.Table
{
    public interface IBaseTableNode
    {
        public abstract void OnLoadComplete();
        public abstract void ClearTable();
    }
}