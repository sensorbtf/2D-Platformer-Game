using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LosingScreen : MonoBehaviour
{ 
    [SerializeField] private GameObject Screen;
    private void FixedUpdate()
    {
        if (Player.Instance.Health <= 0)
            Screen.SetActive(true);
        else
            Screen.SetActive(false);
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }
}
