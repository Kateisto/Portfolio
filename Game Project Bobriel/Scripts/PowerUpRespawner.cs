using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PowerUpRespawner : MonoBehaviour
{
    [SerializeField]
    private float _respawnTime = 5;


    // Kun pelaaja osuu triggeriin niin odotellaan _respawnTime:ssa määritellyn ajan verran
    // Sen jälkeen aktivoidaan tämä objekti(PowerUp) uudestaan
    IEnumerator OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            yield return new WaitForSeconds(_respawnTime);

            this.gameObject.SetActive(true);
        }
    }

    //Lopetetaan kesken oleva Coroutine jos tämä komponentti tuhoutuu
    void OnDestroy()
    {
        StopCoroutine("OnTriggerEnter2D");
    }
}
