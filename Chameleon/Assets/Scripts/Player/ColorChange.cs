using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    [SerializeField]
    public float m_timeToTakeMaterial = 4.0f;
    [SerializeField]
    public float m_timeToRemoveMaterial = 4f;
    [SerializeField]
    public float m_camoCooldown = 2f;

    public Material m_materialUnderMe { get; set; }
    public float m_timeUntilMaterialTaken { get; set; } = 0f;
    public float m_timeUntilMaterialRemoved { get; set; } = 0f;
    public float m_timeSinceCooldown { get; set; } = 0f;

    public bool m_materialTaken { get; set; } = false;
    public bool m_materialRemoved { get; set; }  = false;
    public bool m_cooldown { get; set; } = false;

    public float m_detectionSpeedMultiplier { get; set; }  = 1f;
    public float m_detectionFOVMultiplier { get; set; }  = 1f;
    public float m_detectionRangeMultiplier { get; set; }  = 1f;

    public Vector3 m_velocity { get; set; }

    public float m_textureBlendAmount { get; set; }  = 0f;
    public float m_timeSinceStopped { get; set; } = 0.0f;
    private float m_velocityMagnitude;
    private Vector3 m_lastPos;
    void Start()
    {
        m_lastPos = transform.position;

    }

    void Update()
    {
        RaycastHit hit;
        m_textureBlendAmount = GetComponent<Renderer>().material.GetFloat("_Blend");

        if (m_velocityMagnitude <= 0.0001f)
        {
            m_timeSinceStopped += Time.deltaTime;
        }
        else
        {
            m_timeSinceStopped = 0.0f;

        }

        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            if (GetComponent<Renderer>().material.GetTexture("_MainTex2") == hit.collider.gameObject.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex") &&
                GetComponent<Renderer>().material.GetColor("_Color2") == hit.collider.gameObject.GetComponent<Renderer>().sharedMaterial.GetColor("_Color") &&
                GetComponent<Renderer>().material.GetFloat("_Blend") >= 0.98f)
            {
                m_detectionFOVMultiplier = 0.5f;
                m_detectionSpeedMultiplier = 2f;
                m_detectionRangeMultiplier = 0.8f;
            }
            else
            {
                m_detectionFOVMultiplier = 1f;
                m_detectionSpeedMultiplier = 1f;
                m_detectionRangeMultiplier = 1f;
            }

            if (!m_cooldown)
            {
                if (m_timeSinceStopped >= m_timeToTakeMaterial && !m_materialTaken)
                {
                    m_materialTaken = true;
                }
                else if (!m_materialTaken)
                {
                    GetComponent<Renderer>().material.SetFloat("_Blend", m_timeSinceStopped / m_timeToTakeMaterial);
                    GetComponent<Renderer>().material.SetTexture("_MainTex2", hit.collider.gameObject.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex"));
                    GetComponent<Renderer>().material.SetColor("_Color2", hit.collider.gameObject.GetComponent<Renderer>().sharedMaterial.GetColor("_Color"));
                }

                if (m_materialTaken)
                {
                    m_timeUntilMaterialRemoved += Time.deltaTime;

                    if (m_timeUntilMaterialRemoved >= m_timeToRemoveMaterial)
                    {
                        m_materialTaken = false;
                        m_materialRemoved = true;
                        m_cooldown = true;
                        m_timeUntilMaterialRemoved = 0f;
                        m_timeSinceStopped = 0f;
                    }
                }
            }
            if (m_cooldown)
            {
                m_timeSinceCooldown += Time.deltaTime;

                if (1f - (m_timeSinceCooldown / m_camoCooldown * 10f) >= 0f)
                    GetComponent<Renderer>().material.SetFloat("_Blend", 1f - (m_timeSinceCooldown / m_camoCooldown * 10f));

                if (m_timeSinceCooldown >= m_camoCooldown)
                {
                    m_materialRemoved = false;
                    m_cooldown = false;
                    m_timeSinceCooldown = 0f;
                    m_timeSinceStopped = 0f;
                }
            }
        }
    }

    private void LateUpdate()
    {
        m_velocity = transform.position - m_lastPos;

        m_velocityMagnitude = m_velocity.magnitude;
        m_lastPos = transform.position;
    }
}
