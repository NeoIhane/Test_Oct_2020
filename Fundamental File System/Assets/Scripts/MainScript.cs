using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System;

public class MainScript : MonoBehaviour
{
    [SerializeField]
    PhotoStocks photoStocks;
    [SerializeField]
    Camera renderCamera;

    [SerializeField]
    string folderPath;
    [SerializeField]
    string fileName = "Photo.png";

    private void Awake()
    {
        folderPath = PlayerPrefs.GetString("folderPath", Application.dataPath);
        if (!Directory.Exists(folderPath))
        {
            folderPath = Application.dataPath;
            PlayerPrefs.SetString("folderPath", folderPath);
        }
        photoStocks.InitPool(3);
        photoStocks.SetPath(folderPath);
        photoStocks.ReloadPhotos();
    }
    void Start()
    {

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
        StartCoroutine(photoStocks.TakeScreenShot(folderPath + "/" + fileName, renderCamera));
    }
    void ChangePath()
    {
#if UNITY_EDITOR
        //string path = EditorUtility.OpenFolderPanel("Select Folder", folderPath, "");
        //if (path.Length != 0)
        //{
        //    folderPath = path;
        //    PlayerPrefs.SetString("folderPath", folderPath);
        //    photoStocks.SetPath(folderPath);
        //}
#endif 
        selectPath = folderPath;
        GetFolderData();
        isShowFileDialogMockup = true;
        //photoStocks.ReloadPhotos();
    }
    bool isShowFileDialogMockup = false;
    Rect FileDialogMockupRect = new Rect(0, 0, Screen.width, (int)(Screen.height * 0.7f));
    DirectoryInfo selectDirectory;
    DirectoryInfo parentDirectory;
    DirectoryInfo[] sameLevelDirectory;
    DirectoryInfo[] subDirectory;
    string selectPath;
    void GetFolderData()
    {
        parentDirectory = Directory.GetParent(selectPath);
        if (parentDirectory != null)
        {
            sameLevelDirectory = parentDirectory.GetDirectories();
        }else
        {
            DriveInfo[] driveInfos = DriveInfo.GetDrives();
            sameLevelDirectory = new DirectoryInfo[driveInfos.Length];
            for (int i = 0; i < sameLevelDirectory.Length; i++)
            {
                sameLevelDirectory[i] = driveInfos[i].RootDirectory;
            }
        }
        string[] s = selectPath.Split('/');

        foreach (DirectoryInfo dir in sameLevelDirectory)
        {
            if (dir.Name == s[s.Length - 1] || dir.FullName == selectPath)
            {
                selectDirectory = dir;
            }
        }
        subDirectory = selectDirectory.GetDirectories();
    }
    private void OnGUI()
    {
        if(isShowFileDialogMockup)
        {
            FileDialogMockupRect = GUILayout.Window(0, FileDialogMockupRect, FileDialogMockup, "Select Folder");
        }
    }
    Vector2 parentfolderScrollPos;
    Vector2 samefolderScrollPos;
    Vector2 childfolderScrollPos;
    void FileDialogMockup(int windowID)
    {
        GUILayout.Label(selectPath);
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical("box");
        GUILayout.Label("ParentFolder");
        parentfolderScrollPos = GUILayout.BeginScrollView(parentfolderScrollPos);
        if (parentDirectory != null)
        {
            if (GUILayout.Button(parentDirectory.Name))
            {
                selectPath = parentDirectory.FullName;
                GetFolderData();
            }
        }else
            GUILayout.Label("Computer");
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.BeginVertical("box");
        GUILayout.Label("Select Folder");
        samefolderScrollPos = GUILayout.BeginScrollView(samefolderScrollPos);
        for (int i = 0; i < sameLevelDirectory.Length; i++)
        {
            if(selectDirectory==sameLevelDirectory[i])
            {
                GUILayout.Label(sameLevelDirectory[i].Name,"box");
            }
            else if (GUILayout.Button(sameLevelDirectory[i].Name))
            {
                selectPath = sameLevelDirectory[i].FullName;
                GetFolderData();
            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.BeginVertical("box");
        GUILayout.Label("SubFolder");
        childfolderScrollPos = GUILayout.BeginScrollView(childfolderScrollPos);
        for (int i = 0; i < subDirectory.Length; i++)
        {
            if (GUILayout.Button(subDirectory[i].Name))
            {
                selectPath = subDirectory[i].FullName;
                GetFolderData();
            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Cancel"))
        {
            isShowFileDialogMockup = false;
        }
        if (GUILayout.Button("Select"))
        {
            folderPath = selectPath;
            isShowFileDialogMockup = false;

            PlayerPrefs.SetString("folderPath", folderPath);
            photoStocks.SetPath(folderPath);
            photoStocks.ReloadPhotos();
        }
        GUILayout.EndHorizontal();
    }
}
