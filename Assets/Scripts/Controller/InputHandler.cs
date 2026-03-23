using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [Header("Settings")]

    [SerializeField] private float fingerOffset =1.5f; // De ngon tay khong che mat block

    private bool isDragging;

    private Camera mainCamera;

    private TouchControls touchControls;
    private Vector2 lastDragWorldPos;
    private bool hasLastDragWorldPos;

    private const float DragUpdateMinDeltaSqr = 0.0001f;

    void Awake()
    {
        touchControls = new TouchControls();

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindAnyObjectByType<Camera>();
        }

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
        hasLastDragWorldPos = false;

        EventBus.Instance.Publish(new OnDragStart
        {
            pos = GetWorldPosition()
        });

        


    }

    public void EndTouch(InputAction.CallbackContext context)
    {
        isDragging  = false;
        hasLastDragWorldPos = false;
        EventBus.Instance.Publish(new OnDragEnd
        {
            pos = GetWorldPositionWithOffset()
        });

       

    }

    void Update()
    {
        if (isDragging)
        {
            Vector2 currentPos = GetWorldPositionWithOffset();
            if (!hasLastDragWorldPos || (currentPos - lastDragWorldPos).sqrMagnitude >= DragUpdateMinDeltaSqr)
            {
                lastDragWorldPos = currentPos;
                hasLastDragWorldPos = true;
                EventBus.Instance.Publish(new OnDragUpdate
                {
                    pos = currentPos
                });
            }

        }
    }

    

    public  Vector2 GetWorldPosition(){
        if (mainCamera == null)
        {
            return Vector2.zero;
        }

        Vector2 screenPosition = touchControls.Gameplay.TouchPosition.ReadValue<Vector2>();
        
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -mainCamera.transform.position.z));
        return new Vector2(worldPosition.x, worldPosition.y);
    }

    public  Vector2 GetWorldPositionWithOffset(){
        Vector2 worldPosition = GetWorldPosition();
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


