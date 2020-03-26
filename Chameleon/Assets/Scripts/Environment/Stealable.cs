using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stealable : MonoBehaviour
{
    [SerializeField]
    public float m_timeToSteal = 4f;

    [SerializeField]
    public int m_extraScoreIfStolen = 30;

    public bool m_hasBeenStolen { get; set; } = false;

    public float m_timeSinceStartedStealing { get; set; }  = 0f;
    private GameObject m_player;
    
    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == m_player)
        {
            m_timeSinceStartedStealing += Time.deltaTime;

            if (m_timeSinceStartedStealing >= m_timeToSteal)
            {
                m_hasBeenStolen = true;
                FindObjectOfType<Score>().m_score += m_extraScoreIfStolen;
                m_timeSinceStartedStealing = 0f;
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        m_timeSinceStartedStealing = 0f;
    }
}
