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
        {
            Screen.SetActive(true);
        }
        else
        {
            Screen.SetActive(false);
        }
    }
    public void ReloadScene()
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int projectileLayer = LayerMask.NameToLayer("Projectiles");

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        Physics2D.IgnoreLayerCollision(playerLayer, projectileLayer, false);
    }
}
