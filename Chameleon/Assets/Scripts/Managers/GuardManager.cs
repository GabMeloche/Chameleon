using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardManager : MonoBehaviour
{
    public enum AlertState
    {
        Idle,
        Alert,
        Searching
    }

    [SerializeField]
    public float m_timeToStopSearching = 5f;

    public Vector3 m_lastSeenPosition { get; set; }
    public AlertState m_alertState { get; set; } = AlertState.Idle;

    private Detection[] m_detections;
    private Guard[] m_guards;
    private float m_timeSinceFound = 0f;
    private float m_timeSinceSearching = 0f;



    void Start()
    {
        m_detections = FindObjectsOfType<Detection>();
        m_guards = FindObjectsOfType<Guard>();
    }

    void Update()
    {
        bool playerInSight = false;
        bool playerFound = false;

        switch (m_alertState)
        {
            case AlertState.Alert:
                m_timeSinceFound += Time.deltaTime;
                m_timeSinceSearching = 0f;

                foreach (Detection detection in m_detections)
                {
                    if (detection.m_playerInSight)
                    {
                        playerInSight = true;
                        m_lastSeenPosition = detection.m_lastSightingPosition;
                    }
                    else
                        detection.m_lastSightingPosition = m_lastSeenPosition;
                }

                if (!playerInSight)
                {
                    m_alertState = AlertState.Searching;
                }
                break;

            case AlertState.Idle:
                foreach (Detection detection in m_detections)
                {
                    if (detection.m_playerFound && detection.m_playerInSight)
                    {
                        playerFound = true;
                    }
                }

                if (playerFound)
                    m_alertState = AlertState.Alert;

                break;

            case AlertState.Searching:
                m_timeSinceSearching += Time.deltaTime;
                m_timeSinceFound += Time.deltaTime;

                if (m_timeSinceSearching >= m_timeToStopSearching && !GameData.m_hardMode)
                {
                    m_timeSinceSearching = 0f;
                    m_alertState = AlertState.Idle;

                    foreach (Guard guard in m_guards)
                    {
                        if (guard.m_guardRole == Guard.GuardRole.Patrol)
                        {
                            guard.GoBackToPatrol();
                        }
                    }
                    break;
                }
                foreach (Detection detection in m_detections)
                {
                    if (detection.m_playerInSight)
                    {
                        playerInSight = true;
                    }
                }

                if (playerInSight)
                {
                    m_alertState = AlertState.Alert;
                }


                break;

            default:
                break;
        }
    }

    private void AlertLoop()
    {
        bool playerInSight = false;

        m_timeSinceSearching = 0f;

        foreach (Detection detection in m_detections)
        {
            if (detection.m_playerInSight)
            {
                playerInSight = true;
                m_lastSeenPosition = detection.m_lastSightingPosition;
            }
            else
                detection.m_lastSightingPosition = m_lastSeenPosition;
        }

        if (!playerInSight)
        {
            m_alertState = AlertState.Searching;
        }
    }

    private void DecideState()
    {

    }
}
