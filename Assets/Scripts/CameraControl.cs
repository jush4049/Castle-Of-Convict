using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControl : MonoBehaviour
{
    private CinemachineFreeLook freeLookComp;

    private float xSpeed = 1000;
    private float ySpeed = 8;

    // 카메라 시점 변경 설정값
    private float lookAtDistance = 4;
    private float lookAtHeight = 1.9f;
    private float bottomRigHeight = 0.1f;
    private float midRigDegree = 15;
    private float topRigDegree = 75;
    private float maxZoom = 1.0f;
    private float minZoom = 0.5f;
    private bool enableZoom = true;
    private AxisState zAxis = new AxisState(0, 1, false, true, 50f, 0.1f, 0.1f, "Mouse ScrollWheel", false);

    void Start()
    {
        freeLookComp = FindObjectOfType<CinemachineFreeLook>();
        freeLookComp.LookAt = this.transform;
        freeLookComp.Follow = this.transform;
        Zoom(1);
    }

    void LateUpdate()
    {
        if (!Dialogue.instance.dialogueRunning && !GameManager.isMiniGame)
        {
            freeLookComp.m_XAxis.m_MaxSpeed = xSpeed;
            freeLookComp.m_YAxis.m_MaxSpeed = ySpeed;
        }
        else if (Dialogue.instance.dialogueRunning || GameManager.isMiniGame)
        {
            if (Input.GetMouseButton(1))
            {
                freeLookComp.m_XAxis.m_MaxSpeed = xSpeed;
                freeLookComp.m_YAxis.m_MaxSpeed = ySpeed;
            }
            else
            {
                freeLookComp.m_XAxis.m_MaxSpeed = 0;
                freeLookComp.m_YAxis.m_MaxSpeed = 0;
            }
        }

        // 마우스 스크롤시 줌인/줌아웃
        if (enableZoom)
        {
            zAxis.Update(Time.deltaTime);
            float zoom = Mathf.Lerp(minZoom, maxZoom, zAxis.Value);
            Zoom(zoom);
        }
    }

    public void StartZoom()
    {
        if (!enableZoom)
        {
            enableZoom = true;
        }
    }

    public void StopZoom()
    {
        if (enableZoom)
        {
            enableZoom = false;
        }
    }

    // Rig값 설정
    private void Zoom(float zoom)
    {
        freeLookComp.m_Orbits[1].m_Height = lookAtHeight + (lookAtDistance * Mathf.Sin(midRigDegree * Mathf.Deg2Rad) * zoom); ;
        freeLookComp.m_Orbits[1].m_Radius = lookAtDistance * Mathf.Cos(midRigDegree * Mathf.Deg2Rad) * zoom;
        freeLookComp.m_Orbits[0].m_Height = lookAtHeight + (lookAtDistance * Mathf.Sin(topRigDegree * Mathf.Deg2Rad) * zoom);
        freeLookComp.m_Orbits[0].m_Radius = lookAtDistance * Mathf.Cos(topRigDegree * Mathf.Deg2Rad) * zoom;
        freeLookComp.m_Orbits[2].m_Height = bottomRigHeight;
        freeLookComp.m_Orbits[2].m_Radius = Mathf.Sqrt(Mathf.Pow(lookAtDistance * zoom, 2) - Mathf.Pow(lookAtHeight - bottomRigHeight, 2));
    }
}
