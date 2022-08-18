using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Coin : MonoBehaviour
{
    [SerializeField] private AudioClip collectingCoinAudio;
    private SoundManager soundManager;
    private CoinsCounter coinCounter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //soundManager.PlayEnviromentEffects(collectingCoinAudio);

            coinCounter.NumberOfCoins++;
            Destroy(gameObject);
        }
    }

    [Inject]
    public void construct(SoundManager sM, CoinsCounter cC)
    {
        soundManager = sM;
        coinCounter = cC;
    }

}
