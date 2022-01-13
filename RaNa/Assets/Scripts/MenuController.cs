using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class MenuController : MonoBehaviour
{
    public Button btnSelector1;
    public Button btnSelector2;
    public Button btnExit;

    // private string _fileSettingsImgs = "SettingsImgs.txt";
    // private string _pathSettingsImgs;
    
    private string _fileSettingsSelector = "SettingsSelector.txt";
    private string _pathSettingsSelector;
    
    // Start is called before the first frame update
    void Start()
    {
        // _pathSettingsImgs = CreateFile(_fileSettingsImgs);
        _pathSettingsSelector = CreateFile(_fileSettingsSelector); 
            
        btnSelector1.onClick.AddListener(GoToSelector1);
        btnSelector2.onClick.AddListener(GoToSelector2);
        btnExit.onClick.AddListener(ExitApp);
    }

    private void GoToSelector1()
    {
        // set selector
        WriteFile( _pathSettingsSelector , "1", "r");
        
        /*
        // set images
        string msg = "";
        string[] img_paths;
        
        img_paths = new string[]
        {
            "UserFiles/Imgs/A1.jpg",
            "UserFiles/Imgs/A2.jpg",
            "UserFiles/Imgs/A3.jpg",
        };

        foreach (var img_path in img_paths)
        {
            msg += img_path + "\n";
        }
        msg += "end";
        
        WriteFile( _pathSettingsImgs , msg, "r");
        */
        
        Loader.Load(Loader.Scene.ScanningScene);
    }
    
    private void GoToSelector2()
    {
        WriteFile( _pathSettingsSelector , "2", "r");
        
        /*
        // set images
        string msg = "";
        string[] img_paths;

        img_paths = new string[]
        {
            "UserFiles/Imgs/B1.jpg",
            "UserFiles/Imgs/B2.jpg",
            "UserFiles/Imgs/B3.jpg",
        };

        foreach (var img_path in img_paths)
        {
            msg += img_path + "\n";
        }
        msg += "end";
        
        WriteFile( _pathSettingsImgs , msg, "r");
        */
        
        Loader.Load(Loader.Scene.ScanningScene);
    }
    
    private void ExitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // if file already exists, it returns that path.
    private string CreateFile(string fileName)
    {
        string filePath = GetPathFile(fileName);
        
        if (!File.Exists(filePath)) {
            //Create File if it doesn't exist
            File.WriteAllText(filePath, "");
        }
        
        return filePath;
    }
    
    private string GetPathFile(string fileName)
    {
        string filePath;
        if ( SystemInfo.deviceModel == "PC")
        {
            filePath =  Application.dataPath + "/" + fileName;
        }
        else
        {
            filePath = Application.persistentDataPath + "/" + fileName;
        }

        return filePath;
    }
    
    // type = a, for appending
    // type = r, for replace
    private void WriteFile(string filePath, string msg, string type)
    {
        if (type == "a")
            File.AppendAllText(filePath, msg);
        else if (type=="r")
            File.WriteAllText(filePath, msg);
    }

}
