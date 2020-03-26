using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MouseHover : MonoBehaviour
{
    [SerializeField]
    public Color m_colorWhenMouseHovering;

    private TextMeshPro m_textMesh;
    private Color m_originalColor;

    void Start()
    {
        m_textMesh = GetComponent<TextMeshPro>();
        m_originalColor = m_textMesh.color;
    }

    private void OnMouseEnter()
    {
        m_textMesh.color = m_colorWhenMouseHovering;
    }

    private void OnMouseExit()
    {
        m_textMesh.color = m_originalColor;
    }
}
