using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Detection))]
public class SecurityCamera : MonoBehaviour
{
    [SerializeField]
    private float m_timeForFullRotation = 20f;
    [SerializeField]
    private float m_maxRotationAngle = 140f;
    [SerializeField]
    private float m_timeStandingStill = 2f;
    [SerializeField]
    private float m_lookAtPlayerSpeed = 0.05f;

    private bool m_returnToIdle = false;
    private float m_timeSinceBeginningOfCycle = 0f;
    private float m_degreesPerSecond;
    private float m_timeForHalfRotation;
    private Quaternion m_idleRotation;
    private GameObject m_player;
    private Detection m_detection;

    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_detection = GetComponent<Detection>();
        m_degreesPerSecond = (m_maxRotationAngle) / ((m_timeForFullRotation / 2f) - (m_timeStandingStill));
        m_timeForHalfRotation = (m_timeForFullRotation / 2f) - (m_timeStandingStill);
    }

    private void Awake()
    {
        transform.Rotate(new Vector3(0f, -m_maxRotationAngle / 2f, 0f));
    }

    void Update()
    {
        switch (m_detection.m_state)
        {
            case Detection.State.Idle:
            case Detection.State.Searching:
                if (m_returnToIdle)
                {
                    m_returnToIdle = false;
                    transform.rotation = m_idleRotation;
                }
                CameraIdleScan();
                break;

            case Detection.State.Alert:
            case Detection.State.Curious:
                m_returnToIdle = true;
                break;

            default:
                break;
        }

    }

    private void CameraIdleScan()
    {
        m_timeSinceBeginningOfCycle += Time.deltaTime;

        if (m_timeSinceBeginningOfCycle <= m_timeForHalfRotation)
        {
            transform.Rotate(new Vector3(0f, m_degreesPerSecond * Time.deltaTime, 0f));
        }
        else if (m_timeSinceBeginningOfCycle <= m_timeForHalfRotation + m_timeStandingStill)
        {
            return;
        }
        else if ((m_timeSinceBeginningOfCycle <= (m_timeForHalfRotation * 2f) + m_timeStandingStill))
        {
            transform.Rotate(new Vector3(0f, -m_degreesPerSecond * Time.deltaTime, 0f));
        }
        else if (m_timeSinceBeginningOfCycle <= m_timeForFullRotation)
        {
            return;
        }
        else
        {
            m_timeSinceBeginningOfCycle = 0f;
        }

        m_idleRotation = transform.rotation;
    }

    private void FollowPlayer()
    {
        Quaternion rot = Quaternion.LookRotation(m_player.transform.position - transform.position, transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, m_lookAtPlayerSpeed);
    }
}
