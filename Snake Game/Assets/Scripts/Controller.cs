using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Controller : MonoBehaviour
{
    [SerializeField]
    Transform touchObject;
    [SerializeField]
    EventTrigger trigger;
    [SerializeField]
    Vector2 touchPos = new Vector2();
    [SerializeField]
    Vector2 direction = new Vector2();
    [SerializeField]
    int id = -1;
    float angle;

    public Action onTouchUP;
    public Action onTouchDown;
    public Action onTouchRight;
    public Action onTouchLeft;
    public bool EnableControll = true;
    private void Start()
    {
        trigger = GetComponent<EventTrigger>();

        EventTrigger.Entry entry_begindrag = new EventTrigger.Entry();
        entry_begindrag.eventID = EventTriggerType.BeginDrag;
        entry_begindrag.callback.AddListener((data) => { OnBeginDrag((PointerEventData)data); });
        trigger.triggers.Add(entry_begindrag);

        EventTrigger.Entry entry_drag = new EventTrigger.Entry();
        entry_drag.eventID = EventTriggerType.Drag;
        entry_drag.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
        trigger.triggers.Add(entry_drag);

        EventTrigger.Entry entry_enddrag = new EventTrigger.Entry();
        entry_enddrag.eventID = EventTriggerType.EndDrag;
        entry_enddrag.callback.AddListener((data) => { OnEndDrag((PointerEventData)data); });
        trigger.triggers.Add(entry_enddrag);

        touchObject.gameObject.SetActive(false);
    }
    public void OnDrag(PointerEventData data)
    {
        if (id != data.pointerId) return;
        if (!touchObject.gameObject.activeSelf) touchObject.gameObject.SetActive(true);

        Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(data.position.x, data.position.y, Camera.main.nearClipPlane));
        touchObject.position = new Vector3(point.x, point.y, 0);
    }
    public void OnBeginDrag(PointerEventData data)
    {
        if (id == -1)
            id = data.pointerId;
        if (id != data.pointerId) return;

        touchPos = data.position;
        direction = Vector3.zero;

        touchObject.gameObject.SetActive(EnableControll);
    }
    public void OnEndDrag(PointerEventData data)
    {
        if (id != data.pointerId) return;
        direction = (data.position - touchPos).normalized;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (EnableControll)
        {
            if (angle > -45 && angle < 45)
            {
                if (onTouchRight != null) onTouchRight();
            }
            else if (angle > 45 && angle < 135)
            {
                if (onTouchUP != null) onTouchUP();
            }
            else if (angle < -45 && angle > -135)
            {
                if (onTouchDown != null) onTouchDown();
            }
            else
            {
                if (onTouchLeft != null) onTouchLeft();
            }
        }
        id = -1;
        touchObject.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (EnableControll)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (onTouchRight != null) onTouchRight();
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (onTouchLeft != null) onTouchLeft();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (onTouchDown != null) onTouchDown();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (onTouchUP != null) onTouchUP();
            }
        }
    }

    private void OnGUI()
    {
        //GUI.Label(new Rect(100, Screen.height - 300, 300, 300), touchPos.ToString() + " " + direction.ToString() + " " + touchObject.position.ToString() + " " + angle);
    }
}
