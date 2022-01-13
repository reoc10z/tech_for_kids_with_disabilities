using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;


// TODO: lo de copiar y pegar las funciones de uitiliy a todos los scripts no arregló el problema
// borrar las funciones en los scripts y seguir buscando el error de por qué no funciona el botón atrás en config o en scanner scenes
    
public class Utility
{
    // type = a, for appending
    // type = r, for replace
    public void WriteFile(string filePath, string msg, string type)
    {
        if (type == "a")
            File.AppendAllText(filePath, msg);
        else if (type=="r")
            File.WriteAllText(filePath, msg);
    }
    
    public string GetPathFile(string fileName)
    {
        if ( SystemInfo.deviceModel == "PC")
        {
            return Application.dataPath + "/" + fileName;
        }
        else
        {
            return Application.persistentDataPath + "/" + fileName;
        }
    }
    
    
    // if file already exists, it returns that path.
    public string CreateFile(string fileName)
    {
        string filePath = GetPathFile(fileName);
        
        if (!File.Exists(filePath)) {
            //Create File if it doesn't exist
            File.WriteAllText(filePath, "");
        }
        return filePath;
    }

    public string ReadFile(string filePath)
    {
        // read filepath and return all text as one string
        return System.IO.File.ReadAllText(filePath);
    }

    public void DeleteFile(string filePath)
    {
        File.Delete( filePath );
    }
    
    //constant variables
    private const string FileLog = "Log.txt";
    private const string FileSettingsScanner = "SettingsScanner.txt";
    private const string FileSettingsSpeedScanner = "SettingsSpeedScanner.txt";
    private const float ScreenHeightReference = 640.0f;
    private const string FileSettingsVibration = "SettingsVibration.txt";

    public float GetScreenHeightReference()
    {
        return ScreenHeightReference;
    }

    public string GetNameLogFile()
    {
        return FileLog;
    }
    
    public string GetNameSettingsScanner()
    {
        return FileSettingsScanner;
    }
    
    public string GetNameSettingsSpeedScanner()
    {
        return FileSettingsSpeedScanner;
    }
    
    public string GetNameSettingsVibration()
    {
        return FileSettingsVibration;
    }
    
    
    
}
