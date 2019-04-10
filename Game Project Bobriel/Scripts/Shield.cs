using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    private bool _powerUpUsed;
    Health _health;

    //Luodaan delegaatti, joka ottaa vastaan stringin
    internal protected delegate void DeactivatePowerUp(string slotStatus);
    //Luodaan eventti
    internal protected static event DeactivatePowerUp SendStatus;

    void OnEnable()
    {
        _health = this.gameObject.GetComponent<Health>();
        _powerUpUsed = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("ActivePowerup") && _powerUpUsed != true)
        {
            _powerUpUsed = true;

            //Käynnistetään event PowerUpManagerissa ja lähetetään tieto että powerup on käytetty
            SendStatus(null);

            StartCoroutine(DamageShield());
        }
    }

    //Pelaaja saa 6 sekunnin immuniteetin damagelle
    IEnumerator DamageShield()
    {
        _health.damageImmunity = true;

        Debug.Log("Shield activated");

        yield return new WaitForSeconds(4);

        Debug.Log("Blinky Blinky");

        yield return new WaitForSeconds(2);

        _health.damageImmunity = false;

        this.enabled = false;

        Debug.Log("Shield disabled");
    }

    //Lopetetaan kesken oleva Coroutine jos tämä komponentti tuhoutuu
    void OnDestroy()
    {
        StopCoroutine("DamageShield");
    }
}
