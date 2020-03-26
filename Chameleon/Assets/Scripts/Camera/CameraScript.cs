using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_posOffset = Vector3.zero;
    [SerializeField]
    private float m_smoothSpeed = 0.125f;
    [SerializeField]
    private float m_cameraPreviewDistance = 50f;
    [SerializeField]
    private float m_maxMovementSpeed = 20f;
    [SerializeField]
    private float m_AlertAdditionalYOffset = 6f;

    private GameObject m_player;
    private Vector3 desiredPosition;
    private Vector3 smoothedPosition;
    private Vector3 m_zoomVelocity = Vector3.zero;
    private GuardManager m_guardManager;

    void Start()
    {
        m_guardManager = FindObjectOfType<GuardManager>();
    }

    private void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {

    }

    private void LateUpdate()
    {
        Vector3 posOffset = m_posOffset;

        if (m_guardManager.m_alertState == GuardManager.AlertState.Alert || m_guardManager.m_alertState == GuardManager.AlertState.Searching)
        {
            posOffset.y += m_AlertAdditionalYOffset;
        }
        desiredPosition = m_player.transform.position + posOffset + (m_player.GetComponent<ColorChange>().m_velocity * m_cameraPreviewDistance);
        smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref m_zoomVelocity, m_smoothSpeed, m_maxMovementSpeed);
        transform.position = smoothedPosition;
    }
}
