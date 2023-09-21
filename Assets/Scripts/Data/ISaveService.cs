namespace Data
{
    public interface ISaveService
    {
        public void Save(PlayerData data);
        public PlayerData Load();
        public void DeleteData();
    }
}
