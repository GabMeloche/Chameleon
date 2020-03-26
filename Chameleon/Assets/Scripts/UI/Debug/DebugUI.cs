using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugUI : MonoBehaviour
{
    TextMeshProUGUI m_text;
    public int m_FPS { get; set; }

    void Start()
    {
        m_text = GetComponent<TextMeshProUGUI>();
    }

    private void LateUpdate()
    {
        m_FPS = (int)(1f / Time.deltaTime);
        GetComponent<TextMeshProUGUI>().text = "FPS: " + m_FPS;
    }
    private void FixedUpdate()
    {
        
    }
}
