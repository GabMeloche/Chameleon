using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Restart : MonoBehaviour
{
    void Start()
    {
    }

    private void OnMouseUp()
    {
        SceneManager.LoadScene("GameScene");
    }
}
