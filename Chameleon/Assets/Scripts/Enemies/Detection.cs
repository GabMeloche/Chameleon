using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Detection : MonoBehaviour
{
    public enum State
    {
        Idle,
        Alert,
        Curious,
        Searching
    }

    [SerializeField]
    private float m_idleDetectionDistance = 5f;
    [SerializeField]
    private float m_curiousDetectionDistance = 5f;
    [SerializeField]
    private float m_alertDetectionDistance = 7.5f;
    [SerializeField]
    private float m_lightRangePadding = 0f;
    [SerializeField]
    private float m_idleFOVAngle = 110f;
    [SerializeField]
    private float m_curiousFOVAngle = 110f;
    [SerializeField]
    private float m_alertFOVAngle = 130f;
    [SerializeField]
    private Vector3 m_eyePositionOffset = Vector3.zero;
    [SerializeField]
    private float m_timeToDetect = 2f;
    [SerializeField]
    private float m_lightIntensity = 14f;
    [SerializeField]
    private float m_lookAtPlayerSpeed = 0.05f;

    public bool m_playerFound { get; set; }
    public bool m_playerInSight { get; set; }
    public Vector3 m_lastSightingPosition { get; set; }
    public State m_state { get; set; } = State.Idle;
    public float m_timeSinceDetected { get; set; } = 0f;


    private float m_currentFOVAngle;
    private Vector3 m_eyePos;
    protected GameObject m_player;
    private ColorChange m_playerColorChange;
    private SphereCollider m_col;
    private GuardManager m_guardManager;
    private Light m_light;
    private Vector3 m_scale;

    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_playerColorChange = m_player.GetComponent<ColorChange>();
        m_guardManager = FindObjectOfType<GuardManager>();
        m_state = State.Idle;
        m_scale = transform.localScale;
    }

    private void Awake()
    {
        m_col = GetComponent<SphereCollider>();
        m_light = GetComponent<Light>();

        if (!m_light)
            m_light = GetComponentInChildren<Light>();

        m_col.radius = m_idleDetectionDistance / m_scale.z;
        m_light.range = m_idleDetectionDistance;

        m_eyePos = transform.position + m_eyePositionOffset;
    }

    void Update()
    {
        float FOVMultiplier = m_playerColorChange.m_detectionFOVMultiplier;
        float rangeMultiplier = m_playerColorChange.m_detectionRangeMultiplier;

        DecideState();

        switch (m_state)
        {
            default:
            case State.Idle:
                m_timeSinceDetected = 0f;
                m_light.color = Color.white;
                m_col.radius = m_idleDetectionDistance * rangeMultiplier / m_scale.z;
                m_light.range = (m_idleDetectionDistance * rangeMultiplier) + m_lightRangePadding;
                m_light.spotAngle = m_idleFOVAngle * FOVMultiplier;
                m_light.intensity = m_lightIntensity;
                m_currentFOVAngle = m_idleFOVAngle * FOVMultiplier;
                break;

            case State.Curious:
                FollowPlayer();
                float detectionTimePercentage = 1.2f - (m_timeSinceDetected / m_timeToDetect);
                m_light.color = Color.yellow;
                m_col.radius = (m_curiousDetectionDistance * rangeMultiplier / m_scale.z) + m_lightRangePadding;
                m_light.range = (m_curiousDetectionDistance * rangeMultiplier) + m_lightRangePadding;
                m_light.spotAngle = m_curiousFOVAngle * FOVMultiplier * detectionTimePercentage;
                m_light.intensity = m_lightIntensity / detectionTimePercentage;
                m_currentFOVAngle = m_curiousFOVAngle * FOVMultiplier;
                break;

            case State.Alert:
                m_light.color = Color.red;
                m_col.radius = m_alertDetectionDistance * rangeMultiplier / m_scale.z;
                m_light.range = (m_alertDetectionDistance * rangeMultiplier) + m_lightRangePadding;
                m_light.spotAngle = m_alertFOVAngle * FOVMultiplier;
                m_light.intensity = m_lightIntensity;
                m_currentFOVAngle = m_alertFOVAngle * FOVMultiplier;
                break;

            case State.Searching:
                m_light.color = Color.blue;
                m_col.radius = m_alertDetectionDistance * rangeMultiplier / m_scale.z;
                m_light.range = (m_alertDetectionDistance * rangeMultiplier) + m_lightRangePadding;
                m_light.spotAngle = m_alertFOVAngle * FOVMultiplier;
                m_light.intensity = m_lightIntensity;
                m_currentFOVAngle = m_alertFOVAngle * FOVMultiplier;
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        m_eyePos = transform.position + m_eyePositionOffset;

        if (other.gameObject == m_player)
        {
            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);

            if (angle < m_currentFOVAngle * 0.5f)
            {
                RaycastHit hit;
                if (Physics.Raycast(m_eyePos, direction.normalized, out hit, m_col.radius))
                {
                    Debug.DrawRay(m_eyePos, direction, Color.blue, 0, false);

                    m_playerInSight = true;

                    if (m_timeSinceDetected <= m_timeToDetect * m_playerColorChange.m_detectionSpeedMultiplier)
                    {
                        float distance = Vector3.Distance(transform.position, m_player.transform.position);
                        float distanceDetectionRatio = distance / m_idleDetectionDistance;
                        m_timeSinceDetected += Time.deltaTime / distanceDetectionRatio;
                    }
                    else
                    {
                        m_playerFound = true;
                        m_lastSightingPosition = other.transform.position;
                        m_timeSinceDetected = 0f;
                    }
                }
            }
            else
                m_playerInSight = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        m_playerInSight = false;
        if (m_timeSinceDetected >= m_timeToDetect)
            m_playerFound = false;
    }

    private void DecideState()
    {
        if (m_playerInSight &&
            !m_playerFound &&
            m_state != State.Searching &&
            m_state != State.Alert)
        {
            m_state = State.Curious;
        }
        else if (m_guardManager.m_alertState == GuardManager.AlertState.Alert)
        {
            m_state = State.Alert;
        }
        else if (m_guardManager.m_alertState == GuardManager.AlertState.Idle)
        {
            m_state = State.Idle;
        }
        else if (m_guardManager.m_alertState == GuardManager.AlertState.Searching)
        {
            m_state = State.Searching;
        }
        else
        {
            m_state = State.Idle;
        }
    }

    private void FollowPlayer()
    {
        if (m_playerInSight)
        {
            Quaternion rot = Quaternion.LookRotation(m_player.transform.position - transform.position, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, m_lookAtPlayerSpeed);
        }
    }

}
