using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MenuController : MonoBehaviour
{
    public Button btnStartScanner;
    public Button btnConfig;
    public Button btnExit;
    
    // Start is called before the first frame update
    void Start()
    {
        btnStartScanner.onClick.AddListener(GoToScanningScene);
        btnConfig.onClick.AddListener(GoToConfiguration);
        btnExit.onClick.AddListener(ExitApp);
    }

    private void GoToScanningScene()
    {
        Loader.Load(Loader.Scene.ScanningScene);
    }
    
    private void GoToConfiguration()
    {
        Loader.Load(Loader.Scene.ConfigScene);
    }
    
    private void ExitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
