using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamoBar : MonoBehaviour
{
    private GameObject m_player;
    private ColorChange m_colorChange;
    private Image m_bar;

    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_colorChange = m_player.GetComponent<ColorChange>();
        m_bar = GetComponent<Image>();
    }

    void Update()
    {
        float fillPercentage = 0f;

        if (m_player.GetComponent<ColorChange>().m_materialUnderMe != null)
            m_bar.color = m_player.GetComponent<ColorChange>().m_materialUnderMe.color;

        if (m_colorChange.m_cooldown)
            fillPercentage = 0f;

        else if (m_colorChange.m_materialTaken)
        {
            fillPercentage = 1f - (m_colorChange.m_timeUntilMaterialRemoved / m_colorChange.m_timeToRemoveMaterial);
            m_bar.color = m_player.GetComponent<Renderer>().material.color;
        }
        else
        {
            fillPercentage = m_colorChange.m_timeSinceStopped / m_colorChange.m_timeToTakeMaterial;

            if (m_player.GetComponent<ColorChange>().m_materialUnderMe != null)
                m_bar.color = m_player.GetComponent<ColorChange>().m_materialUnderMe.color;
        }

        m_bar.fillAmount = fillPercentage;
    }
}
