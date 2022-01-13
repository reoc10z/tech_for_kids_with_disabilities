using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ConfigController : MonoBehaviour
{
    public Button BtnBack;
    public Toggle ToggleScanner;
    public InputField InputSpeed;
    public Toggle ToggleVibration;
    public Button BtnReset;
    
    private Utility _utility = new Utility();

    private void Awake()
    {
        // display current values to configurations
        
        bool scannerOn = Boolean.Parse(  _utility.ReadFile( _utility.GetPathFile( _utility.GetNameSettingsScanner() ) ) );
        ToggleScanner.isOn = scannerOn;
        
        int speed = Int32.Parse(  _utility.ReadFile( _utility.GetPathFile( _utility.GetNameSettingsSpeedScanner() ) ) );
        InputSpeed.text = "" + speed;
        
        bool vibrationOn = Boolean.Parse(  _utility.ReadFile( _utility.GetPathFile( _utility.GetNameSettingsVibration() ) ) );
        ToggleVibration.isOn = vibrationOn;
        
    }

    void Start()
    {
        BtnBack.onClick.AddListener(GoBack);
        
        //Add listener for when the state of the Toggle changes, to take action
        ToggleScanner.onValueChanged.AddListener(delegate
        {
            ToggleScannerChanged(ToggleScanner);
        });
        
        //Adds a listener to the main input field and invokes a method when the value changes.
        InputSpeed.onValueChanged.AddListener (delegate { InputSpeedChanged(); });
        
        ToggleVibration.onValueChanged.AddListener(delegate
        {
            ToggleVibrationChanged(ToggleVibration);
        });
        
        BtnReset.onClick.AddListener(ResetSettings);
    }
    
    private void GoBack()
    {
        Loader.Load(Loader.Scene.MenuScene);
    }
    
    private void ToggleScannerChanged(Toggle change)
    {
        _utility.WriteFile( _utility.GetPathFile( _utility.GetNameSettingsScanner() ) , change.isOn ? "true" : "false", "r");
        print(" " + ToggleScanner.isOn);
    }
    
    private void InputSpeedChanged()
    {
        _utility.WriteFile(_utility.GetPathFile( _utility.GetNameSettingsSpeedScanner() ), InputSpeed.text, "r");
        print(" " + InputSpeed.text);
    }
    
    private void ToggleVibrationChanged(Toggle change)
    {
        _utility.WriteFile( _utility.GetPathFile( _utility.GetNameSettingsVibration() ) , change.isOn ? "true" : "false", "r");
        print(" " + ToggleVibration.isOn);
    }

    private void ResetSettings()
    {
        // deleting files for settings. Logfile is not removed
        _utility.DeleteFile(_utility.GetPathFile(_utility.GetNameSettingsVibration()));
        _utility.DeleteFile(_utility.GetPathFile(_utility.GetNameSettingsScanner() ));
        _utility.DeleteFile(_utility.GetPathFile(_utility.GetNameSettingsSpeedScanner() ));
        // go to loading scene to create settings files with the default values
        GoToLoaderScene();
    }

    private void GoToLoaderScene()
    {
        Loader.Load(Loader.Scene.LoadingScene);
    }
}
