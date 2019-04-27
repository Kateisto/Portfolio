using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    private bool _triggerActive;
    [SerializeField]
    private short _damageAmount = 1;

    //Luodaan delegaatti, joka ottaa vastaan shortin
    internal protected delegate void OnTriggerSendDamage(short damage);
    //Luodaan eventti
    internal protected static event OnTriggerSendDamage SendDamage;

    void OnEnable()
    {
        _triggerActive = true;
    }

    //Kun pelaaja osuu triggeriin, lähetetään eventtiä kuuntelevalle luokalle eventin sisältämä arvo(_damageAmount)
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") && _triggerActive)
        {
            _triggerActive = false;

            //Käynnistetään event Health skriptissä
            SendDamage(_damageAmount);
        }
    }
}
