using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Play : MonoBehaviour
{
    private GameObject m_firstMenu;
    private GameObject m_playMenu;

    private void Start()
    {
        m_firstMenu = GameObject.Find("FirstMenu");
        m_playMenu = GameObject.Find("PlayMenu");
        m_playMenu.SetActive(false);
    }
    // Start is called before the first frame update
    private void OnMouseUp()
    {
        m_firstMenu.SetActive(false);
        m_playMenu.SetActive(true);
    }
}
