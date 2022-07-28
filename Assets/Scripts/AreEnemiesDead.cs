using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreEnemiesDead : MonoBehaviour
{
    List<GameObject> listOfOpponents = new List<GameObject>();

    void Start()
    {
        listOfOpponents.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        print(listOfOpponents.Count);
    }

    public void KilledOpponent(GameObject opponent)
    {
        if (listOfOpponents.Contains(opponent))
        {
            listOfOpponents.Remove(opponent);
        }

        print(listOfOpponents.Count);
    }

    public bool AreOpponentsDead()
    {
        if (listOfOpponents.Count <= 0)
        {
            // They are dead!
            return true;
        }
        else
        {
            // They're still alive dangit
            return false;
        }
    }
}
