using System.Collections;
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
    ARTrackedImageManager manager;

    //Spheres
    [SerializeField]
    PoolManager ballManager;

    //Debug
    string debug = "";
    bool is_problem = false;

    IEnumerator Start()
    {
        if ((ARSession.state == ARSessionState.None) || (ARSession.state == ARSessionState.CheckingAvailability))
        {
            yield return ARSession.CheckAvailability();
        }
        if (ARSession.state == ARSessionState.Unsupported)
        {
            is_problem = true;
            debug = "ARSession.state == ARSessionState.Unsupported";
        }
        else
        {
            aRSession.enabled = true;
            
        }

        ballManager.InitPool(3);
    }

    void Update()
    {
        
    }
    private void OnGUI()
    {
        if (is_problem)
        {
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), debug);
        }
    }
    #region ball sphere pool
    [Serializable]
    public class PoolManager
    {
        [SerializeField]
        GameObject _objectContainer;
        [SerializeField]
        Ball _objectPrefab;
        List<Ball> _objectPool { get; set; }
        
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
    }
    #endregion
}


