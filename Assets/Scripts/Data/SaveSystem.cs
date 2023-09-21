using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Data
{
    public class SaveSystem : ISaveService
    {
        private string _path = Application.persistentDataPath + "/PlayerData.m3p";
        
        public void Save(PlayerData data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(_path, FileMode.Create);
            bf.Serialize(stream, data);
            stream.Close();
        }

        public PlayerData Load()
        {
            if (File.Exists(_path))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream stream = new FileStream(_path, FileMode.Open);
                PlayerData data = bf.Deserialize(stream) as PlayerData;
                stream.Close();
                return data;
            }
            else
            {
                Debug.LogError("Save file not found in " + _path);
                return null;
            }
        }

        public void DeleteData()
        {
            if (File.Exists(_path))
            {
                File.Delete(_path);
            }
            else
            {
                Debug.Log("File to delete not found in " + _path);
            }
        }
    }
}
