using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coin : MonoBehaviour
{
    [SerializeField] private AudioClip collectingCoinAudio;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SoundManager.Instance.PlayEnviromentEffects(collectingCoinAudio);

            CoinsCounter.Instance.NumberOfCoins++;
            Destroy(gameObject);
        }
    }
}
