using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CSVLoader {
    public static string Load(string relativePath) {
        string fullPath = Path.Combine(Application.streamingAssetsPath, relativePath);
     
        if (!File.Exists(fullPath)) {
            Debug.LogError(fullPath + " CSV file not found");
            return null;
        }

        return File.ReadAllText(fullPath);
    }
}
