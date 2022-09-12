using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LosingScreen : MonoBehaviour
{ 
    [SerializeField] private GameObject Screen;
    private void FixedUpdate()
    {
        Screen.SetActive(PlayerConfig.Instance.Health <= 0);
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }
}
