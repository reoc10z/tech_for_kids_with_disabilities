#define USER_ISI
// #define  USER_EMMA

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScanningController : MonoBehaviour
{
    public float speed = 2f;
    public Transform LinePosition;
    public Button BtnBack;
    public Text TouchTxt;
    
    private Vector3 _direction = new Vector3(1,0,0);
    private float _screenWidth;
    private float _screenHeight;
    
#if USER_ISI
    public GameObject[] imgsSelector1 = new GameObject[3];
    public GameObject[] imgsSelector2 = new GameObject[3];
    public AudioSource[] audiosSelector1 = new AudioSource[3];
    public AudioSource[] audiosSelector2 = new AudioSource[3];
#elif USER_EMMA
    public GameObject[] imgs = new GameObject[2];
#endif
    private GameObject imgA;
    private GameObject imgB;
    private GameObject imgC;
    private AudioSource audioA;
    private AudioSource audioB;
    private AudioSource audioC;
    private string[] imgsTexts;

    private Vector2 imgA_limX;
    private Vector2 imgB_limX;
    private Vector2 imgC_limX;
    private Vector2 imgA_limY;
    private Vector2 imgB_limY;
    private Vector2 imgC_limY;
    
    //private string _fileSettingsImgs = "SettingsImgs.txt";
    private string _fileSettingsSelector = "SettingsSelector.txt";
    
    private void Awake()
    {
        int selector = Int32.Parse( ReadFile(GetPathFile(_fileSettingsSelector)) );
        
        switch (selector)
        {
            case 1:
                //imgs on for selector 1
                imgA = imgsSelector1[0];
                imgB = imgsSelector1[1];
                imgC = imgsSelector1[2];
                
                //imgs off for selector 2
                foreach (var img in imgsSelector2)
                    img.SetActive( false ) ;
                
                //imgs texts
                imgsTexts = new []
                {
                    "A",
                    "B",
                    "C"
                };
                
                //audios
                audioA = audiosSelector1[0];
                audioB = audiosSelector1[1];
                audioC = audiosSelector1[2];
                break;
            
            case 2:
                //imgs
                imgA = imgsSelector2[0];
                imgB = imgsSelector2[1];
                imgC = imgsSelector2[2];
                
                //imgs off for selector 1
                foreach (var img in imgsSelector1)
                    img.SetActive( false ) ;
                
                //imgs texts
                imgsTexts = new []
                {
                    "D",
                    "E",
                    "F"
                };
                
                //audios
                audioA = audiosSelector2[0];
                audioB = audiosSelector2[1];
                audioC = audiosSelector2[2];
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _screenWidth = Screen.width;
        _screenHeight = Screen.height;

        BtnBack.onClick.AddListener(GoBack);
        print(LinePosition.position);
        print("scanning...");

        // limits for objects
        RectTransform rt;
        // lim sizes for object 1
        print(imgA.transform.position.x);
        rt = ((RectTransform) imgA.transform);
        imgA_limX = new Vector2(imgA.transform.position.x - rt.rect.width / 2,
            imgA.transform.position.x + rt.rect.width / 2);
        imgA_limY = new Vector2(imgA.transform.position.y - rt.rect.height / 2,
            imgA.transform.position.y + rt.rect.height / 2);

        // lim sizes for object 2
        rt = ((RectTransform) imgB.transform);
        imgB_limX = new Vector2(imgB.transform.position.x - rt.rect.width / 2,
            imgB.transform.position.x + rt.rect.width / 2);
        imgB_limY = new Vector2(imgB.transform.position.y - rt.rect.height / 2,
            imgB.transform.position.y + rt.rect.height / 2);

        // lim sizes for object 3
        rt = ((RectTransform) imgC.transform);
        imgC_limX = new Vector2(imgC.transform.position.x - rt.rect.width / 2,
            imgC.transform.position.x + rt.rect.width / 2);
        imgC_limY = new Vector2(imgC.transform.position.y - rt.rect.height / 2,
            imgC.transform.position.y + rt.rect.height / 2);
        
        // load scene images according to settings file
        /*
        string file_txt = ReadFile(GetPathFile(_fileSettingsImgs));
        string [] imgPaths = file_txt.Split('\n');
        for (int k = 0; k< imgPaths.Length; k++)
        {
            if (imgPaths[k] == "end")
                break;
            string path = GetPathFile( imgPaths[k] );
            Sprite img = LoadNewSprite( path );
            imgObjs[k].GetComponent<Image>().sprite = img;
        }
        */

    }
    
    private Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f, SpriteMeshType spriteType = SpriteMeshType.Tight)
    {      
        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
        Texture2D SpriteTexture = LoadTexture(FilePath);
        Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit, 0, spriteType);
 
        return NewSprite;
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

        LinePosition.position += _direction * speed;
        if (LinePosition.position.x > _screenWidth )
        {
            // Reset line position
            print(LinePosition.position.x);
            LinePosition.position = new Vector3(1, (int)(_screenHeight / 2), 0);
            TouchTxt.text = "";
        }
        
        // any touch on screen or click on any part
        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            //Touch touch = Input.GetTouch(0);
            //print("touch screen (x,y) _ " + (int)touch.position.x + " , " + (int)touch.position.y );

            int selection = ValidateTouch(new Vector2( LinePosition.position.x, LinePosition.position.y ));
            if (selection > 0)
                ApplyAction(selection);
        }
        
    }

    // ValidateTouch sees if the screen touch corresponds to any available element. 
    // It will return the corresponding id, if there is no correspondence, a negative number will be returned
    private int ValidateTouch(Vector2 selectorPos)
    {
        TouchTxt.text = (int)selectorPos.x + " , " + (int)selectorPos.y;
        // test objects
        if (selectorPos.x > imgA_limX.x && selectorPos.x < imgA_limX.y)
            return 1;
        else if (selectorPos.x > imgB_limX.x && selectorPos.x < imgB_limX.y)
            return 2;
        else if (selectorPos.x > imgC_limX.x && selectorPos.x < imgC_limX.y)
            return 3;
        else
        {
            return -1;
        }
    }
    
    private void ApplyAction(int selection)
    {
        switch (selection)
        {
            case 1:
                TouchTxt.text = imgsTexts[0];
                audioA.Play();
                break;
            case 2:
                TouchTxt.text = imgsTexts[1];
                audioB.Play();
                break;
            case 3:
                TouchTxt.text = imgsTexts[2];
                audioC.Play();
                break;
            default:
                break;
        }
    }
    
    private void GoBack()
    {
        Loader.Load(Loader.Scene.MenuScene);
    }
    
    private string ReadFile(string filePath)
    {
        // read filepath and return all text as one string
        return System.IO.File.ReadAllText(filePath);
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
}
