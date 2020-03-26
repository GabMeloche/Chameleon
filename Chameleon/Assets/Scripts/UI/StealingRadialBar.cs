using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StealingRadialBar : MonoBehaviour
{
    [SerializeField]
    private float m_transparency = 1f;

    private GameObject m_player;
    private ColorChange m_colorChange;
    private Image m_bar;

    private Stealable m_objectBeingStolen;
    private Stealable[] m_stealables;

    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_colorChange = m_player.GetComponent<ColorChange>();
        m_bar = GetComponent<Image>();
        m_bar.fillAmount = 0f;
        m_stealables = FindObjectsOfType<Stealable>();

        Color tmp = m_bar.color;
        tmp.a = m_transparency;
        m_bar.color = tmp;
    }

    private void Update()
    {
        foreach (Stealable stealable in m_stealables)
        {
            if (stealable.m_timeSinceStartedStealing > 0f)
            {
                m_objectBeingStolen = stealable;
            }
        }

        if (m_objectBeingStolen)
        {
            m_bar.fillAmount = m_objectBeingStolen.m_timeSinceStartedStealing / m_objectBeingStolen.m_timeToSteal;
        }
        else
            m_bar.fillAmount = 0f;
    }


}
