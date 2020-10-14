using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.Networking;

public class PhotoStocks : MonoBehaviour
{
    string path = "";
    [SerializeField]
    Text pathText;
    [SerializeField]
    RenderTexture renderTexture;

    private void Start()
    {
        //InitPool(3);
    }
    public void SetPath(string path)
    {
        this.path = path;
        pathText.text = "Path: " + path;
    }
    public void ReloadPhotos()
    {
        if (Directory.Exists(path))
        {
            SetDeactiveAll();
            debug += "\nReloadPhotos " + path;
            string[] filesNames = Directory.GetFiles(path,"*.png");
            for (int i = 0; i < filesNames.Length; i++)
            {
                //Debug.Log(filesNames[i]);
                StartCoroutine(GetTexture(filesNames[i]));
                debug += "\nfilesNames[i] " + filesNames[i];
            }
        }else debug+= "\nReloadPhotos !Directory.Exists(path) " + path;
    }
    string debug = "Esc = Clear";
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            isDebugLog = !isDebugLog;
        }
    }
    [SerializeField]
    bool isDebugLog = false;
    void OnGUI()
    {
        if(isDebugLog)
        {
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), debug);
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                debug = "Esc = Clear";
            }
        }
    }
    IEnumerator GetTexture(string filePath)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(filePath))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded asset bundle
                var texture = DownloadHandlerTexture.GetContent(uwr);
                RequestPhoto().Set(texture, filePath.Substring(path.Length));
            }
        }
    }
    IEnumerator Upload(Texture2D texture)
    {
        using (UnityWebRequest www = UnityWebRequest.Put(path, texture.EncodeToPNG()))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Upload complete!");
            }
        }
    }

    public IEnumerator TakeScreenShot(string filePath, Camera renderCamera)
    {
        yield return new WaitForEndOfFrame();
        debug += "\nTakeScreenShot " + filePath;

        renderCamera.targetTexture = renderTexture;
        RenderTexture.active = renderCamera.targetTexture;
        renderCamera.Render();

        Texture2D texture = new Texture2D(renderCamera.targetTexture.width, renderCamera.targetTexture.height, TextureFormat.ARGB32, false);
        Rect rect = new Rect(0, 0, renderCamera.targetTexture.width, renderCamera.targetTexture.height);
        texture.ReadPixels(rect, 0, 0);
        texture.Apply();

        try
        {
            var pngData = texture.EncodeToPNG();
            File.WriteAllBytes(filePath, pngData);

        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.ToString());
            debug += "\nError: " + ex.ToString();
        }

        renderCamera.targetTexture = null;

        ReloadPhotos();
    }
    public static Texture2D Resize(Texture2D source, int newWidth, int newHeight)
    {
        source.filterMode = FilterMode.Point;
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Point;
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        Texture2D nTex = new Texture2D(newWidth, newHeight);
        nTex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        nTex.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return nTex;
    }
    #region Photo Pool
    [SerializeField]
    GameObject _photoContainer;
    [SerializeField]
    Photo _photoPrefab;
    List<Photo> _photoPool { get; set; }

    public void InitPool(int total)
    {
        _photoPool = new List<Photo>();
        _photoPool = GeneratePhotos(total);
    }
    public List<Photo> GeneratePhotos(int amountOfObjects)
    {
        for (int i = 0; i < amountOfObjects; i++)
        {
            Photo obj = CreateObject();
            _photoPool.Add(obj);
            SetActive(obj, false);
        }
        return _photoPool;
    }
    public Photo RequestPhoto()
    {
        foreach (var obj in _photoPool)
            if (!IsActiveHierachy(obj))
            {
                SetActive(obj, true);
                return obj;
            }
        return CreateObject();
    }
    protected Photo CreateObject()
    {
        Photo obj = Instantiate(_photoPrefab, _photoContainer.transform) as Photo;
        return obj;
    }
    protected bool IsActiveHierachy(Photo obj)
    {
       return obj.gameObject.activeInHierarchy;
    }

    protected void SetActive(Photo obj, bool isActive)
    {
        obj.gameObject.SetActive(isActive);
    }
    public void SetDeactiveAll()
    {
        foreach (var obj in _photoPool)
            SetActive(obj, false);
    }
    #endregion
}
