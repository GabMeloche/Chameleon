using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameplayManager : MonoBehaviour
{
    private GameObject m_player;
    private GameObject m_caughtMenu;
    private GameObject m_pauseMenu;

    private Button m_continue;
    private Button m_pauseReturn;

    private bool m_paused = false;

    void Start()
    {
        Cursor.visible = false;
    }
    private void OnEnable()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_continue = GameObject.Find("Continue").GetComponent<Button>();
        m_pauseReturn = GameObject.Find("Restart").GetComponent<Button>();
        m_caughtMenu = GameObject.Find("Endgame");
        m_pauseMenu = GameObject.Find("PauseMenu");
        m_caughtMenu.SetActive(false);
        m_pauseMenu.SetActive(false);
        m_player.GetComponent<Player>().m_playerCaught.AddListener(OnPlayerDeath);
    }
    void Update()
    {
        //Debug.Log(Input.GetAxis("Vertical"));
        if (m_paused)
        {
            if (Input.GetAxis("Vertical") > 0f)
            {
                m_continue.Select();
            }
            else if (Input.GetAxis("Vertical") < 0f)
            {
                m_pauseReturn.Select();
            }

            //m_pauseReturn.FindSelectableOnLeft().Select();
        }
        if (Input.GetButtonUp("Cancel"))
        {
            m_paused = !m_paused;
            m_pauseMenu.SetActive(m_paused);
            Cursor.visible = !Cursor.visible;
        }

        if (m_paused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }

    private void OnPlayerDeath()
    {
        Cursor.visible = true;
        m_caughtMenu.SetActive(true);
        m_paused = true;
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void UnPause()
    {
        m_paused = false;
        Time.timeScale = 1f;
        m_pauseMenu.SetActive(false);
    }

}
