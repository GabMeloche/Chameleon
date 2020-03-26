using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Back : MonoBehaviour
{
    private GameObject m_firstMenu;
    private GameObject m_currentMenu;

    void Start()
    {
        m_firstMenu = GameObject.Find("FirstMenu");
        m_currentMenu = transform.parent.gameObject;
    }

    private void OnMouseUp()
    {
        m_firstMenu.SetActive(true);
        m_currentMenu.SetActive(false);
    }
}
