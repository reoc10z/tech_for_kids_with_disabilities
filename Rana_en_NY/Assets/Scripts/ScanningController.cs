using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScanningController : MonoBehaviour
{
    public float speed;
    public GameObject lineScanner;
    public Button BtnBack;
    public Text TouchTxt;

    private Utility _utility = new Utility();
    
    private Transform ScannerPosition;
    private float deltaScannerWidth;
    private Vector3 _direction = new Vector3(1,0,0);
    private float _screenWidth;
    private float _screenHeight;
    private bool _scannerOn;
    private bool _vibrationOn;
    
    public GameObject[] imgsSelector1 = new GameObject[2];
    public GameObject figureA;
    public AudioSource[] audiosSelector1 = new AudioSource[2];
    
    private GameObject imgA;
    private GameObject imgB;
    private AudioSource audioA;
    private AudioSource audioB;
    private string[] imgsTexts;

    private Vector2 imgA_limX;
    private Vector2 imgB_limX;
    private Vector2 imgA_limY;
    private Vector2 imgB_limY;

    private string _pathLog;
    private string _logMsg = "";
    
    private float _timeHighlihgting = 1.0f; // one second to highlight the selected figure
    private bool timmerON = false; // if activate timmer to highlight figure
    private int imgSelection = 0;
    
    private void Awake()
    {
        // screen will be always active
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
        ScannerPosition = lineScanner.transform;
    
        //loading imgs
        imgA = imgsSelector1[0];
        imgB = imgsSelector1[1];
                
        //imgs texts
        imgsTexts = new []
        {
            "A",
            "B"
        };
                
        //audios
        audioA = audiosSelector1[0];
        audioB = audiosSelector1[1];
        
        // get path to log file
        _pathLog = _utility.GetPathFile( _utility.GetNameLogFile() );
        
        // activate or deactivate scanner line
        _scannerOn = Boolean.Parse(   _utility.ReadFile(  _utility.GetPathFile( _utility.GetNameSettingsScanner() )) );
        //todo: make visible or not the scanner line
        if (_scannerOn)
        {
            // by default line scanner is active
            lineScanner.SetActive(true);
            //scanner line speed
            speed = float.Parse(_utility.ReadFile( _utility.GetPathFile( _utility.GetNameSettingsSpeedScanner() ) ));
        }
        else
        {
            lineScanner.SetActive(false);
            speed = 0.0f;
        }
        
        // activate or deactivate vibration
        _vibrationOn = Boolean.Parse(   _utility.ReadFile(  _utility.GetPathFile( _utility.GetNameSettingsVibration() )) );
    }

    // Start is called before the first frame update
    void Start()
    {
        _screenWidth = Screen.width;
        _screenHeight = Screen.height;
#if UNITY_EDITOR_LINUX
        float scaleFactor = 1.0f;
        ToLog("CARE MONDAAAA");
#else
        // in unity, I configured canvas to match the height. And 640 is the referenced height. 
        float scaleFactor = _screenHeight / _utility.GetScreenHeightReference();
#endif
        
        ToLog($"scale factor({scaleFactor})");
        ToLog($"screen ( {_screenWidth}, {_screenHeight} )");
        
        BtnBack.onClick.AddListener(GoBack);
        print(ScannerPosition.position);
        print("scanning...");

        // limits for line scanner
        deltaScannerWidth = ((RectTransform)ScannerPosition).rect.width * scaleFactor / 2;
        
        // limits for objects
        RectTransform rt;
        // lim sizes for object 1
        rt = ((RectTransform) imgA.transform);
        var position1 = imgA.transform.position;
        imgA_limX = new Vector2(position1.x - rt.rect.width*scaleFactor /2, position1.x + rt.rect.width*scaleFactor /2);
        imgA_limY = new Vector2(position1.y - rt.rect.height*scaleFactor/2, position1.y + rt.rect.height*scaleFactor/2);
        ToLog(string.Format("imgA - C( {4} , y ) - W( {0} , {1} ) - H( {2} , {3} )", imgA_limX.x, imgA_limX.y, imgA_limY.x, imgA_limY.y, position1.x));

        // lim sizes for object 2
        rt = ((RectTransform) imgB.transform);
        var position2 = imgB.transform.position;
        imgB_limX = new Vector2(position2.x - rt.rect.width*scaleFactor /2, position2.x + rt.rect.width*scaleFactor /2);
        imgB_limY = new Vector2(position2.y - rt.rect.height*scaleFactor/2, position2.y + rt.rect.height*scaleFactor/2);
        ToLog(string.Format("imgB - C( {4} , y ) - W( {0} , {1} ) - H( {2} , {3} )", imgB_limX.x, imgB_limX.y, imgB_limY.x, imgB_limY.y, position2.x));
    }
    
    private Texture2D LoadTexture(string FilePath)
    {
        // Load a PNG or JPG file from disk to a Texture2D
        // Returns null if load fails
        Texture2D Tex2D;
        byte[] FileData;
        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                return Tex2D;                 // If data = readable -> return texture
        }
        return null;                     // Return null if load failed
    }

    // Update is called once per frame
    void Update()
    {
        //timmerON: when any image is selected, that image will be highlighted for one second
        //during this time any other image cannot be selected
        if (timmerON)
        {
            _timeHighlihgting -= Time.deltaTime;
            if ( _timeHighlihgting < 0 )
            {
                timmerON = false;
                _timeHighlihgting = 1.0f; // one second
                ApplyAction(-imgSelection);
            }
        }
        else
        {
            // when line scanner is activated app reactions depends on its position.
            // else, app reaction depends extrictly on position of touch screen
            if (_scannerOn)
            {
                ScannerPosition.position += _direction * speed;
                if (ScannerPosition.position.x > _screenWidth )
                {
                    // Reset line position
                    print(ScannerPosition.position.x);
                    ScannerPosition.position = new Vector3(1, (int)(_screenHeight / 2), 0);
                    TouchTxt.text = "";
                }
            
                // any touch on screen or click on any part
                if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
                {
                    //Touch touch = Input.GetTouch(0);
                    //print("touch screen (x,y) _ " + (int)touch.position.x + " , " + (int)touch.position.y );

                    
                    // right side of the scanner line
                    imgSelection = ValidateTouch(new Vector2( ScannerPosition.position.x +deltaScannerWidth, ScannerPosition.position.y ));
                    if (imgSelection > 0)
                    {
                        ApplyAction(imgSelection);
                        if (_vibrationOn)
                            Handheld.Vibrate();
                    }
                    else
                    {
                        // left side of the scanner line
                        imgSelection = ValidateTouch(new Vector2( ScannerPosition.position.x -deltaScannerWidth, ScannerPosition.position.y ));
                        if (imgSelection > 0)
                        {
                            ApplyAction(imgSelection);
                            if (_vibrationOn)
                                Handheld.Vibrate();
                        }
                    }
                }
            }
            else
            {
                // any touch on screen or click on any part
                if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
                {
#if UNITY_EDITOR_LINUX
                    Vector3 screenTouch = Input.mousePosition;
                    imgSelection = ValidateTouch(new Vector2( screenTouch.x, screenTouch.y ));
                    if (imgSelection > 0)
                    {
                        ApplyAction(imgSelection);
                        if (_vibrationOn)
                            Handheld.Vibrate();
                    }
#else
                    Touch screenTouch = Input.GetTouch(0);
                    //print("touch screen (x,y) _ " + (int)screenTouch.position.x + " , " + (int)screenTouch.position.y );
                    //ToLog( $"touch screen ( { (int) screenTouch.position.x }, { (int) screenTouch.position.y } ) ");
                    imgSelection = ValidateTouch(new Vector2( screenTouch.position.x, screenTouch.position.y ));
                    if (imgSelection > 0)
                        {
                            ApplyAction(imgSelection);
                        if (_vibrationOn)
                            Handheld.Vibrate();
                        }
#endif
                    
                }
            }
            
        }
        
    }
    
    // ValidateTouch sees if the screen touch corresponds to any available element. 
    // It will return the corresponding id to the touched image, if there is no correspondence, a negative number will be returned
    private int ValidateTouch(Vector2 selectorPos)
    {
        TouchTxt.text = (int)selectorPos.x + " , " + (int)selectorPos.y;
        if (_scannerOn)
        {
            //when scanner is active , check only limits on the x-axis
            // test objects
            if (selectorPos.x > imgA_limX.x && selectorPos.x < imgA_limX.y)
                return 1;
            else if (selectorPos.x > imgB_limX.x && selectorPos.x < imgB_limX.y)
                return 2;
            else
            {
                return -1;
            }
        }
        else
        {
            //when scanner is NOT active , check limits on the x-axis AND the y-axis
            if (selectorPos.x > imgA_limX.x && selectorPos.x < imgA_limX.y && selectorPos.y > imgA_limY.x && selectorPos.y < imgA_limY.y)
                return 1;
            else if (selectorPos.x > imgB_limX.x && selectorPos.x < imgB_limX.y && selectorPos.y > imgB_limY.x && selectorPos.y < imgB_limY.y)
                return 2;
            else
            {
                return -1;
            }
        }
    }
    
    private void ApplyAction(int selection)
    {
        switch (selection)
        {
            case 1:
                TouchTxt.text = imgsTexts[0];
                audioA.Play();
                imgA.GetComponent<Image>().color = Color.yellow;
                timmerON = true;
                break;
            case 2:
                TouchTxt.text = imgsTexts[1];
                audioB.Play();
                imgB.GetComponent<Image>().color = Color.yellow;
                timmerON = true;
                break;
            case -1:
                imgA.GetComponent<Image>().color = Color.white;
                break;
            case -2:
                imgB.GetComponent<Image>().color = Color.white;
                break;
            default:
                break;
        }
    }

    private void ToLog(string msg)
    {
        _logMsg += msg + "\n";
        //todo: may be, the function write log should be called just after certain time, i.e. Stack all log messaged, and after certain time write them into the log file 
        WriteLog();
    }
    
    private void WriteLog()
    {
        _utility.WriteFile(_pathLog, _logMsg, "a");
        _logMsg = "";
    }

    private void GoBack()
    {
        Loader.Load(Loader.Scene.MenuScene);
    }
}
