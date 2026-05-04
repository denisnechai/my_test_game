using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }
    public GameData Load()
    {
        string FullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;
        if (File.Exists(FullPath))
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FullPath));

                string dataToLoad = "";
                using (FileStream stream = new FileStream(FullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                Debug.Log("Loaded data from " + FullPath);
                


            }
            catch (Exception ex)
            {
                Debug.LogError("Error occured when trying to save data to file" + FullPath + "/n" + ex);
            }
        }
        return loadedData;
    }
    public void Save(GameData data) 
    {
        
        string FullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            using (FileStream stream = new FileStream(FullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error occured when trying to save data to file" + FullPath + "/n" + ex);
        }
          
    }
}
