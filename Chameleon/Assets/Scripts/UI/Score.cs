using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public int m_score { get; set; } = 0;
    public float m_timeSinceStarted { get; set; } = 0f;
    public float m_scoreMultiplier { get; set; } = 1f;

    private float m_newPoint = 0f;

    private TextMeshProUGUI m_text;

    void Start()
    {
        m_text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        m_timeSinceStarted += Time.deltaTime;
        m_newPoint += Time.deltaTime;

        if (m_newPoint >= 1f)
        {
            m_newPoint = 0f;
            m_score += (int)(m_scoreMultiplier);
        }

        m_text.text = m_score.ToString();
    }
}
