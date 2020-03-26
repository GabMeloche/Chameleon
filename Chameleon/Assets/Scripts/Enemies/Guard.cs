using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Detection))]
public class Guard : MonoBehaviour
{
    public enum GuardRole
    {
        Patrol,
        Guard
    }

    public enum PatrolType
    {
        Circle,
        BackTrack
    }

    [SerializeField]
    public float m_idleMovementSpeed = 3f;
    [SerializeField]
    public float m_alertMovementSpeed = 6.5f;
    [SerializeField]
    public float m_searchingMovementSpeed = 5f;
    [SerializeField]
    public float m_maximumDistanceToSearch = 200f;
    [SerializeField]
    public Quaternion m_standingSpotRotation;
    [SerializeField]
    public GuardRole m_guardRole = GuardRole.Guard;
    [SerializeField]
    public PatrolType m_patrolType = PatrolType.Circle;
    [SerializeField]
    public float m_patrolSpeed = 3f;
    [SerializeField]
    public Vector3[] m_patrolPoints;

    private float m_currentMovementSpeed;
    private Vector3 m_standingSpotPosition;
    private NavMeshAgent m_agent;
    private Detection m_detection;
    private GameObject m_player;

    private Vector3 m_nextPointToInspect;
    private int m_patrolPointIndex = 0;
    private GuardManager m_guardManager;
    private bool m_nextDestinationIsSet = false;
    private bool m_reachedLastSeenPoint = false;
    private bool m_patrolBackTrack = false;

    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_detection = GetComponent<Detection>();
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_guardManager = FindObjectOfType<GuardManager>();
        m_agent.speed = m_patrolSpeed;
    }

    private void Awake()
    {
        m_standingSpotPosition = transform.position;
        m_standingSpotRotation = transform.rotation;

        if (m_guardRole == GuardRole.Patrol)
        {
            Vector3[] tmpVec = m_patrolPoints;
            m_patrolPoints = new Vector3[tmpVec.Length + 1];

            for (int i = 0; i < tmpVec.Length; ++i)
            {
                m_patrolPoints[i + 1] = tmpVec[i];
            }
            m_patrolPoints[0] = m_standingSpotPosition;
            m_nextDestinationIsSet = true;

        }
    }


    void Update()
    {
        m_agent.isStopped = false;

        switch (m_detection.m_state)
        {
            default:
            case Detection.State.Idle:
                m_currentMovementSpeed = m_idleMovementSpeed;
                GuardRoleBehaviour();
                break;

            case Detection.State.Alert:
                m_currentMovementSpeed = m_alertMovementSpeed;
                m_agent.SetDestination(m_player.transform.position);
                break;

            case Detection.State.Searching:
                m_currentMovementSpeed = m_searchingMovementSpeed;

                if (!m_reachedLastSeenPoint)
                    m_agent.SetDestination(m_guardManager.m_lastSeenPosition);

                if (Vector3.Distance(transform.position, m_agent.destination) >= 2f)
                    break;
                else
                    m_reachedLastSeenPoint = true;

                if (!m_nextDestinationIsSet)
                {
                    m_nextPointToInspect = ChooseNextSearchingPoint();
                    m_nextDestinationIsSet = true;
                    m_agent.SetDestination(m_nextPointToInspect);
                    break;
                }

                while (Vector3.Distance(transform.position, m_nextPointToInspect) >= 1f)
                {
                    break;
                }

                m_nextPointToInspect = ChooseNextSearchingPoint();
                m_agent.SetDestination(m_nextPointToInspect);
                m_nextDestinationIsSet = false;
                break;

            case Detection.State.Curious:
                m_agent.isStopped = true;
                break;
        }

        if (m_agent.speed != m_currentMovementSpeed)
            m_agent.speed = m_currentMovementSpeed;
    }

    private Vector3 ChooseNextSearchingPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * m_maximumDistanceToSearch;
        randomDirection += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, m_maximumDistanceToSearch, 1);
        return hit.position;
    }

    private void GuardRoleBehaviour()
    {

        switch (m_guardRole)
        {
            case GuardRole.Guard:
                if (m_standingSpotPosition != transform.position)
                {
                    m_agent.SetDestination(m_standingSpotPosition);
                }
                if (Vector3.Distance(m_standingSpotPosition, transform.position) <= 1f)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, m_standingSpotRotation, 0.1f);
                }
                break;

            case GuardRole.Patrol:
                if (m_patrolType == PatrolType.BackTrack)
                {
                    if (m_patrolPointIndex == m_patrolPoints.Length - 1)
                        m_patrolBackTrack = true;
                    else if (m_patrolPointIndex == 0)
                        m_patrolBackTrack = false;

                    if (Vector3.Distance(m_patrolPoints[m_patrolPointIndex], transform.position) <= 1f)
                    {
                        if (m_patrolBackTrack)
                            --m_patrolPointIndex;
                        else
                            ++m_patrolPointIndex;

                        m_nextDestinationIsSet = true;
                    }
                }

                if (m_patrolType == PatrolType.Circle && Vector3.Distance(m_patrolPoints[m_patrolPointIndex], transform.position) <= 1f)
                {
                    ++m_patrolPointIndex;

                    if (m_patrolPointIndex >= m_patrolPoints.Length)
                        m_patrolPointIndex = 0;

                    m_nextDestinationIsSet = true;
                }

                if (m_nextDestinationIsSet)
                {
                    m_agent.SetDestination(m_patrolPoints[m_patrolPointIndex]);
                    m_nextDestinationIsSet = false;
                }
                break;

            default:
                break;
        }
    }

    public void GoBackToPatrol()
    {
        if (m_guardRole == GuardRole.Patrol)
        {
            m_agent.SetDestination(m_patrolPoints[m_patrolPointIndex]);
        }
    }
}
