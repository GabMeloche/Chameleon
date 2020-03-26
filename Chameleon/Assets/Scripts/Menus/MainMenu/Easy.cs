using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Easy : MonoBehaviour
{
    private GameObject m_playMenu;
    void Start()
    {
        m_playMenu = GameObject.Find("PlayMenu");
    }

    private void OnMouseUp()
    {
        GameData.m_hardMode = false;
        SceneManager.LoadScene("GameScene");
    }
}
