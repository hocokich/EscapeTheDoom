using System.IO;
using UnityEngine;

public static class SaveController
{
    public static void Save(string levelName, int score)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/saves/"))
        {
            //if it doesn't, create it
            Directory.CreateDirectory(Application.persistentDataPath + "/saves/");
        }

        string path = Application.persistentDataPath + "/saves/" + levelName + ".sav";
        string data = JsonUtility.ToJson(GameManager.instance);
        File.WriteAllText(path, data);
    }

    public static GameManager Load(string levelName)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/saves/"))
        {
            //if it doesn't, create it
            Directory.CreateDirectory(Application.persistentDataPath + "/saves/");
        }

        string path = Application.persistentDataPath + "/saves/" + levelName + ".sav";

        if (File.Exists(path))
        {
            return JsonUtility.FromJson<GameManager>(File.ReadAllText(path));
        }

        return null;

    }
}
