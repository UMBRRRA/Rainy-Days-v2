using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraFollow : MonoBehaviour
{
    private Func<Vector3> GetCameraFollowPosFunc;
    public float cameraMoveSpeed = 0.7f;
    private MainCanvas canvas;

    public void Setup(Func<Vector3> GetCameraFollowPosFunc)
    {
        this.GetCameraFollowPosFunc = GetCameraFollowPosFunc;
    }

    public void SetGetCameraFollowPosFunc(Func<Vector3> GetCameraFollowPosFunc)
    {
        this.GetCameraFollowPosFunc = GetCameraFollowPosFunc;
    }

    private void Start()
    {
        StartCoroutine(FindCanvas());
    }

    public IEnumerator FindCanvas()
    {
        yield return new WaitUntil(() => (canvas = FindObjectOfType<MainCanvas>()) != null);
        canvas.ChangeLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetCameraFollowPosFunc != null)
        {
            Vector3 cameraFollowPos = GetCameraFollowPosFunc();
            cameraFollowPos.z = transform.position.z;

            Vector3 cameraMoveDir = (cameraFollowPos - transform.position).normalized;
            float distance = Vector3.Distance(cameraFollowPos, transform.position);

            if (distance > 0)
            {
                transform.position = transform.position + cameraMoveDir * distance * cameraMoveSpeed * Time.deltaTime;

            }

        }

    }
}
