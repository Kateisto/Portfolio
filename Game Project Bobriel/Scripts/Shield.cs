using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    private bool _powerUpUsed;
    Health _health;
    [SerializeField]
    private GameObject _shieldPrefab;
    private Transform _shieldBorder;
    private Transform _shieldGlow;
    private Transform _shieldFlicker;
    [SerializeField]
    private AudioClip _activateShield;
    [SerializeField]
    private AudioClip _shieldStop;

    //Luodaan delegaatti, joka ottaa vastaan stringin
    internal protected delegate void DeactivatePowerUp(string slotStatus);
    //Luodaan eventti
    internal protected static event DeactivatePowerUp SendStatus;


    void OnEnable()
    {
        _health = this.gameObject.GetComponent<Health>();
        _powerUpUsed = false;

        _shieldBorder = _shieldPrefab.transform.GetChild(0);
        _shieldGlow = _shieldPrefab.transform.GetChild(2);
        _shieldFlicker = _shieldPrefab.transform.GetChild(1);
    }

    void Update()
    {
        if (Input.GetButtonDown("ActivePowerup") && _powerUpUsed != true)
        {
            _powerUpUsed = true;

            //Lisätään kilven aktivointiääni
            AudioManager.instance.PlaySingle(_activateShield);

            //Käynnistetään event PowerUpManagerissa ja lähetetään tieto että powerup on käytetty
            SendStatus(null);

            StartCoroutine("DamageShield");
        }
    }

    //Pelaaja saa 6 sekunnin immuniteetin damagelle
    IEnumerator DamageShield()
    {
        _health.damageImmunity = true;

        _shieldBorder.gameObject.SetActive(true);
        _shieldGlow.gameObject.SetActive(true);

        yield return new WaitForSeconds(4);

        _shieldGlow.gameObject.SetActive(false);

        _shieldFlicker.gameObject.SetActive(true);

        yield return new WaitForSeconds(2);

        AudioManager.instance.PlaySingle(_shieldStop);
        _shieldFlicker.gameObject.SetActive(false);
        _shieldBorder.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        _health.damageImmunity = false;

        this.enabled = false;
    }

    //Lopetetaan kesken oleva Coroutine jos tämä komponentti tuhoutuu
    void OnDestroy()
    {
        StopCoroutine("DamageShield");
    }

    //Jos powerup vaihtuu niin lähetetään tieto että tämä powerup disabloidaan
    void OnDisable()
    {
        if (_powerUpUsed != true)
        {
            try
            {
                //Käynnistetään event
                SendStatus(null);
            }

            //Käsitellään poikkeus tilanteessa jolloin SendStatus eventillä ei ole kuuntelijaa
            catch (System.NullReferenceException)
            {
                return;
            }
        }
    }
}
