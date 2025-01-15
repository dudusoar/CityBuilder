using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class InputManager: MonoBehaviour
{
    // 从玩家中获取输入事件，并通过委托广播给其他脚本
    public Action<Vector3Int> OnMouseClick, OnMouseHold;
    public Action OnMouseUp;
    private Vector2 cameraMovementVector;

    [SerializeField]
    Camera mainCamera;

    public LayerMask groundMask;
    
    public Vector2 CameraMovementVector
    {
        get { return cameraMovementVector; }
    }

    private void Update()
    {
        CheckClickDownEvent();
        CheckClickUpEvent();
        CheckClickHoldEvent();
        CheckArrowInput();
    }

    private Vector3Int? RaycastGround()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
        {
            Vector3Int positionInt = Vector3Int.RoundToInt(hit.point);
            return positionInt;
        }

        return null;
    }
    
    #region check input event

    private void CheckClickDownEvent()
    {
        if(Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            var position = RaycastGround();
            if (position != null)
            {
                OnMouseClick?.Invoke(position.Value);
            }
        }
    }

    private void CheckClickUpEvent()
    {
        if(Input.GetMouseButtonUp(0) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            OnMouseUp?.Invoke();
        }
    }

    public void CheckClickHoldEvent()
    {
        // hold鼠标键但不会误触UI界面
        if(Input.GetMouseButton(0) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            var position = RaycastGround();
            if (position != null)
            {
                OnMouseHold?.Invoke(position.Value);
            }
        }
    }

    public void CheckArrowInput()
    {
        cameraMovementVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    #endregion
}
