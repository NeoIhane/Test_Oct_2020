﻿using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using System;
using System.Collections.Generic;

public class MainScene : MonoBehaviour
{
    //AR
    [SerializeField] 
    ARSession aRSession;

    [SerializeField]
    ARTrackedImageManager trackingImageManager;
    
    Dictionary<string, GameObject> spawnedPrefab = new Dictionary<string, GameObject>();
    [SerializeField]
    TrackingImageObj trackImageObj;
    Dictionary<string, TrackingImageObj> trackImageObjs = new Dictionary<string, TrackingImageObj>();
    //Spheres
    [SerializeField]
    PoolManager ballManager;

    //Debug
    string debug = "";
    string debug_event = "";
    bool is_debug = false;

    IEnumerator Start()
    {
        if ((ARSession.state == ARSessionState.None) || (ARSession.state == ARSessionState.CheckingAvailability))
        {
            yield return ARSession.CheckAvailability();
        }
        if (ARSession.state == ARSessionState.Unsupported)
        {
            is_debug = true;
            debug_event = "ARSession.state == ARSessionState.Unsupported";
        }
        else
        {
            aRSession.enabled = true;
            for (int i = 0; i < trackingImageManager.referenceLibrary.count; i++)
            {
                TrackingImageObj track = Instantiate(trackImageObj);
                track.SetTexture(trackingImageManager.referenceLibrary[i].texture);
                track.SetTracking(false);
                track.onSpawn = () => { ballManager.RequestObject().Spawn(ballManager.RandomColor(), track.gameObject.transform); };
                //track.onDespawn = () => { };
                trackImageObjs.Add(trackingImageManager.referenceLibrary[i].name, track);
            }
        }

        ballManager.InitPool(3);
    }
    private void OnEnable()
    {
        trackingImageManager.trackedImagesChanged += TrackingImageManager_trackedImagesChanged;
    }
    private void OnDisable()
    {
        trackingImageManager.trackedImagesChanged -= TrackingImageManager_trackedImagesChanged;
    }
    float time = 2;
    float count = 0;
    void Update()
    {
        if (aRSession.enabled)
        {
            if (trackingImageManager.trackables.count > 1)
            {
                count += Time.deltaTime;
                if (count >= time)
                {
                    ballManager.Swap(0.3f);
                    count = 0;
                }
            }
            else
            {
                count = 0;
            }
        }
    }
    private void OnGUI()
    {
        if (is_debug)
        {
            if (GUI.Button(new Rect(0, 0, 100, 100), "Debug"))
            {
                is_debug = false;
            }
            if (GUI.Button(new Rect(100, 0, 100, 100), "Clear"))
            {
                debug_event = "";
            }
            GUI.Label(new Rect(0, 100, Screen.width / 2, Screen.height - 100), debug);
            GUI.Label(new Rect(Screen.width / 2, 100, Screen.width / 2, Screen.height - 100), debug_event);

        }
        else
        {
            if (GUI.Button(new Rect(0, 0, 100, 100), "Debug"))
            {
                is_debug = true;
            }
        }
    }

    #region Tracking
    private void TrackingImageManager_trackedImagesChanged(ARTrackedImagesChangedEventArgs obj)
    {
        foreach(ARTrackedImage trackedImage in obj.added)
        {
            debug_event += String.Format("<color=blue>[ADD]<color> Image name:{0} State:{1}\nID:{2}",
                        trackedImage.referenceImage.name,
                        trackedImage.trackingState,
                        trackedImage.trackableId);

            TrackingImageObj trackobj;
            if (trackImageObjs.TryGetValue(trackedImage.referenceImage.name, out trackobj))
            {
                trackobj.SetTracking(true);
                //ballManager.RequestObject().Spawn(ballManager.RandomColor(), trackobj.gameObject.transform);
            }
        }
        foreach (ARTrackedImage trackedImage in obj.updated)
        {
            UpdateImage(trackedImage);
        }
        foreach (ARTrackedImage trackedImage in obj.removed)
        {
            debug_event += String.Format("<color=red>[Removed]</color> Image name:{0} State:{1}\nID:{2}",
                        trackedImage.referenceImage.name,
                        trackedImage.trackingState,
                        trackedImage.trackableId);

            TrackingImageObj trackobj;
            if (trackImageObjs.TryGetValue(trackedImage.referenceImage.name, out trackobj))
            {
                trackobj.SetTracking(false);
            }
        }
    }
    void UpdateImage(ARTrackedImage trackedImage)
    {
        TrackingImageObj trackobj;
        if (trackImageObjs.TryGetValue(trackedImage.referenceImage.name, out trackobj))
        {
            if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
            {
                trackobj.SetTracking(true);
                trackobj.transform.position = trackedImage.transform.position;
                trackobj.transform.rotation = trackedImage.transform.rotation;
                trackobj.SetColor(new Color(1,1,1,0.5f));
            }
            else if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
            {
                trackobj.SetTracking(false);
                trackobj.transform.position = trackedImage.transform.position;
                trackobj.transform.rotation = trackedImage.transform.rotation;
                trackobj.SetColor(new Color(1, 0, 0, 0.5f));
            }
            else if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.None)
            {
                trackobj.SetTracking(false);
            }
        }
        debug = String.Format("Image name:{0} State:{1}\nID:{2}",
                       trackedImage.referenceImage.name,
                       trackedImage.trackingState,
                       trackedImage.trackableId);

    }
    #endregion
    #region ball sphere pool
    [Serializable]
    public class PoolManager
    {
        [SerializeField]
        GameObject _objectContainer;
        [SerializeField]
        Ball _objectPrefab;
        List<Ball> _objectPool { get; set; }
        Color[] colors = new Color[] { Color.red, Color.green, Color.blue, Color.black, Color.cyan, Color.gray, Color.magenta, Color.yellow };
        
        public void InitPool(int total)
        {
            _objectPool = new List<Ball>();
            _objectPool = GenerateObjects(total);
        }
        public List<Ball> GenerateObjects(int amountOfObjects)
        {
            for (int i = 0; i < amountOfObjects; i++)
            {
                Ball obj = CreateObject();
                _objectPool.Add(obj);
                SetActive(obj, false);
            }
            return _objectPool;
        }
        public Ball RequestObject()
        {
            foreach (var obj in _objectPool)
                if (!IsActiveHierachy(obj))
                {
                    SetActive(obj, true);
                    return obj;
                }
            return CreateObject();
        }
        Ball CreateObject()
        {
            Ball obj= Instantiate(_objectPrefab, _objectContainer.transform) as Ball;
            return obj;
        }
        bool IsActiveHierachy(Ball obj)
        {
            return obj.gameObject.activeInHierarchy;
        }

        void SetActive(Ball obj, bool isActive)
        {
            obj.SetActive(isActive);
        }
        public void SetDeactiveAll()
        {
            foreach (var obj in _objectPool)
                SetActive(obj, false);
        }
        public Color RandomColor()
        {
            List<Color> pickedColor = new List<Color>();
            foreach (Ball ball in _objectPool)
            {
                //if (ball.transform.gameObject.activeSelf)
                pickedColor.Add(ball.GetColor());
            }

            List<Color> randomColor = new List<Color>();
            for (int i = 0; i < colors.Length; i++)
            {
                bool isUse = false;
                foreach (Color color in pickedColor)
                {
                    if (colors[i] == color)
                        isUse = true;
                }
                if (!isUse)
                    randomColor.Add(colors[i]);
            }
            if (randomColor.Count >= 1)
            {
                int rand = UnityEngine.Random.Range(0, randomColor.Count - 1);
                return randomColor[rand];

            }
            else
            {
                float r = UnityEngine.Random.Range(0.0f, 1.0f);
                float g = UnityEngine.Random.Range(0.0f, 1.0f);
                float b = UnityEngine.Random.Range(0.0f, 1.0f);
                return new Color(r, g, b, 1f);
            }
        }
        public void Swap(float time)
        {
            List<Ball> activeBall = new List<Ball>();
            for (int i = 0; i < _objectPool.Count; i++)
            {
                if(_objectPool[i].isActiveAndEnabled)
                {
                    activeBall.Add(_objectPool[i]);
                }
            }
            if (activeBall.Count > 1)
            {
                Transform tmp = activeBall[0].GetTarget();
                for (int i = 0; i < activeBall.Count - 1; i++)
                {
                    activeBall[i].ChangeToNewTarget(activeBall[i + 1].GetTarget(), time);
                }
                activeBall[activeBall.Count - 1].ChangeToNewTarget(tmp, time);
            }
        }
    }
    #endregion
}


