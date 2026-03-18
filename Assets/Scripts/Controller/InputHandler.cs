using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [Header("Settings")]

    [SerializeField] private float fingerOffset =1.5f; // De ngon tay khong che mat block

    private bool isDragging;

    private Camera mainCamera;

    private TouchControls touchControls;

    void Awake()
    {
        touchControls = new TouchControls();

        mainCamera = Camera.main;

    }

    public void OnEnable()
    {
        touchControls.Enable();

        // Dang ky su kien bat dau va ket thuc cham
        
        touchControls.Gameplay.TouchPress.started += StartTounch;

        touchControls.Gameplay.TouchPress.canceled += EndTouch;
    }

    public void OnDisable()
    {
        touchControls.Disable();

        // Huy su kien bat dau va ket thuc cham
        
        touchControls.Gameplay.TouchPress.started -= StartTounch;

        touchControls.Gameplay.TouchPress.canceled -= EndTouch;
    }

    private void StartTounch(InputAction.CallbackContext context)
    {
        isDragging = true;

        EventBus.Instance.Publish(new OnDragStart
        {
            pos = GetWorldPositionWithOffset()
        });

        Debug.Log(GridCoordinateConverter.WorldToGrid(GetWorldPositionWithOffset(), GameManager.Instance.GridConfig ));


    }

    public void EndTouch(InputAction.CallbackContext context)
    {
        isDragging  = false;
        EventBus.Instance.Publish(new OnDragEnd
        {
            pos = GetWorldPositionWithOffset()
        });

    }

    void Update()
    {
        if (isDragging)
        {
            EventBus.Instance.Publish(new OnDragUpdate
        {
            pos = GetWorldPositionWithOffset()
        });

        }
    }

    

    public  Vector2 GetWorldPositionWithOffset(){
        Vector2 screenPosition = touchControls.Gameplay.TouchPosition.ReadValue<Vector2>();
        
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -mainCamera.transform.position.z));
        return new Vector2(worldPosition.x, worldPosition.y + fingerOffset);

    }
}

public struct OnDragStart: IEvent
{
    public Vector2 pos;
}

public struct OnDragUpdate: IEvent
{
    public Vector2 pos;
}

public struct OnDragEnd: IEvent
{
    public Vector2 pos;
}


