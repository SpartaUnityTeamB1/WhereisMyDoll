using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private PlayerInput playerInput;

    private Vector3 mainCameraPos = new Vector3(0f, 6.5f, -30f);
    private Quaternion mainCameraRot = Quaternion.Euler(5f, 0f, 0f);
    private float mainCameraSize = 7f;
    private float zoomMainCamera = 3f;

    private Vector3 topViewCameraPos = new Vector3(0f, 25f, -6.5f);
    private Quaternion topViewCameraRot = Quaternion.Euler(78f, 0f, 0f);
    private float topViewCameraFov = 60f;
    private float zoomTopViewCamera = 10f;

    private CameraState curState = CameraState.Main;

    private Camera cam;
    private Ray ray;
    private RaycastHit hit;

    private List<Tween> moveTween = new List<Tween>();

    private void Start()
    {
        cam = Camera.main;

        playerInput = GameManager.Instance.Player;

        playerInput.OnChangeView += ChangeViewPoint;
        playerInput.OnMove += CameraMove;
        playerInput.OnZoom += ZoomCamera;
        playerInput.OnReleaseZoom += ReleaseZoomCamera;
    }

    private void ChangeViewPoint()
    {
        switch (curState)
        {
            case CameraState.Main:
                KillDoTween();
                cam.orthographic = false;
                cam.fieldOfView = topViewCameraFov;
                transform.position = topViewCameraPos;
                transform.rotation = topViewCameraRot;
                curState = CameraState.TopView;
                break;
            case CameraState.TopView:
                KillDoTween();
                cam.orthographic = true;
                cam.orthographicSize = mainCameraSize;
                transform.position = mainCameraPos;
                transform.rotation = mainCameraRot;
                curState = CameraState.Main;
                break;
        }
    }

    private void CameraMove()
    {
        switch (curState)
        {
            case CameraState.Main:
                MoveCamera();
                break;
            case CameraState.TopView:
                RotateCamera();
                break;
        }
    }

    private void ZoomCamera()
    {
        switch (curState)
        {
            case CameraState.Main:
                cam.DOOrthoSize(zoomMainCamera, 0.5f);
                break;
            case CameraState.TopView:
                cam.DOFieldOfView(zoomTopViewCamera, 0.5f);
                break;
        }
    }

    private void ReleaseZoomCamera()
    {
        switch (curState)
        {
            case CameraState.Main:
                KillDoTween();
                cam.DOOrthoSize(mainCameraSize, 0.5f);
                moveTween.Add(transform.DOMove(mainCameraPos, 1f));
                break;
            case CameraState.TopView:
                KillDoTween();
                cam.DOFieldOfView(topViewCameraFov, 0.5f);
                moveTween.Add(transform.DORotateQuaternion(topViewCameraRot, 1f));
                break;
        }
    }

    private void MoveCamera()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            Vector3 nextPos = ray.origin;
            nextPos.x = Mathf.Clamp(nextPos.x, -10f, 10f);
            nextPos.y = Mathf.Clamp(nextPos.y, 2f, 7f);
            nextPos.z = -30;
            moveTween.Add(transform.DOMove(nextPos, 3f));
        }
    }

    private void RotateCamera()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            Vector3 nextPos = hit.point;
            Quaternion targetRotation = Quaternion.LookRotation(nextPos - transform.position);
            moveTween.Add(transform.DORotateQuaternion(targetRotation, 3f));
        }
    }

    public void ResetCamera()
    {
        cam.orthographic = true;
        cam.orthographicSize = mainCameraSize;
        transform.position = mainCameraPos;
        transform.rotation = mainCameraRot;
        curState = CameraState.Main;
        playerInput.IsTop = false;
    }

    private void KillDoTween()
    {
        foreach (var move in moveTween)
        {
            move.Kill();
        }

        moveTween.Clear();
    }
}
