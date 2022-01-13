using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingController : MonoBehaviour
{
    float _timeLeft = 2.0f; // This loading scene will be displayed for 2 seconds. Then, go to menu scene
    
    // Start is called before the first frame update
    void Start()
    {
        Utility utility = new Utility();
        utility.CreateFile( utility.GetNameLogFile() );
        string pathSettingsScanner = utility.CreateFile( utility.GetNameSettingsScanner() );
        // if file is created for the first time, then write 1, meaning scanner line will be active
        if (utility.ReadFile(pathSettingsScanner) == "")
            utility.WriteFile(pathSettingsScanner, "true", "r");
        
        // file for scanner speed. If not exist , setting to default value of 2.0
        string pathSettingsSpeedScanner = utility.CreateFile( utility.GetNameSettingsSpeedScanner() );
        if (utility.ReadFile(pathSettingsSpeedScanner) == "")
            utility.WriteFile(pathSettingsSpeedScanner, "2", "r");
        
        string pathSettingsVibration = utility.CreateFile( utility.GetNameSettingsVibration() );
        // if file is created for the first time, then write 1, meaning scanner line will be active
        if (utility.ReadFile(pathSettingsVibration) == "")
            utility.WriteFile(pathSettingsVibration, "true", "r");
    }

    void Update()
    {
        _timeLeft -= Time.deltaTime;
        if ( _timeLeft < 0 )
        {
            GoToMenuScene();
        }
    }
    
    private void GoToMenuScene()
    {
        Loader.Load(Loader.Scene.MenuScene);
    }
}
