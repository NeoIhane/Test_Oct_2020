using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;


public class MainScript : MonoBehaviour
{
    [SerializeField]
    PhotoStocks photoStocks;

    string apppath = "";
    private void Awake()
    {
        apppath = Application.dataPath;
    }
    void Start()
    {
        folderPath = string.Format("{0}/Photos", apppath);
#if UNITY_STANDALONE_WIN
        folderPath = string.Format("{0}/Photos", Directory.GetParent(apppath));
#endif
        photoStocks.SetPath(folderPath);
        photoStocks.ReloadPhotos();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            SaveImage();
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ChangePath();
        }
    }

    void SaveImage()
    {
        
#if UNITY_EDITOR
        string path = EditorUtility.SaveFilePanel("Save texture as PNG", folderPath, "test.png", "png");
        if (path.Length != 0)
        {
            StartCoroutine(photoStocks.TakeScreenShot(path));
        }
#elif UNITY_STANDALONE_WIN
        StartCoroutine(photoStocks.TakeScreenShot(folderPath + "/date.png"));
#endif
    }
    void ChangePath()
    {
#if UNITY_EDITOR
        string path = EditorUtility.OpenFolderPanel("Select Folder", folderPath, "");
        if (path.Length != 0)
        {
            this.folderPath = path;  
        }
#endif 
        photoStocks.ReloadPhotos();
    }

    [SerializeField]
    string folderPath;

   
}
