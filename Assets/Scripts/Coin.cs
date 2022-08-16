using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Coin : MonoBehaviour
{
<<<<<<< HEAD
    private SoundManager soundManager;
    [SerializeField]
    private AudioClip collectingCoinAudio;
    void OnTriggerEnter2D(Collider2D collision)
=======
    [SerializeField] private AudioClip collectingCoinAudio;
    private void OnTriggerEnter2D(Collider2D collision)
>>>>>>> 13d2e926e68419e1974194969abaa700615ff344
    {
        if (collision.CompareTag("Player"))
        {
            soundManager.PlayEnviromentEffects(collectingCoinAudio);

            CoinsCounter.Instance.NumberOfCoins++;

            Destroy(gameObject);
        }
    }
    [Inject]
    public void construct(SoundManager sM)
    {
        soundManager = sM;
    }
}
