using UnityEngine;
using System.IO;

public class FileManager : MonoBehaviour
{
    private void Start()
    {
        string directoryPath = Application.dataPath + "/JSON"; // Example directory path
        string fileName = "stallsinfo.json"; // Example file name
        string jsonFilePath = Path.Combine(directoryPath, fileName);

        if (File.Exists(jsonFilePath))
        {
            // File exists, proceed with reading the file
            Debug.Log("File exists at: " + jsonFilePath);
            ReadFile(jsonFilePath);
        }
        else
        {
            // File does not exist, handle this case
            Debug.LogError("File not found at path: " + jsonFilePath);
        }
    }

    private void ReadFile(string filePath)
    {
        // Code to read the file goes here
    }
}
