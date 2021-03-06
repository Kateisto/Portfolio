﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PowerUpObject : MonoBehaviour
{
    private bool _triggerActive;
    private string _powerUpTag;
    [SerializeField]
    private byte _powerUpSlot;
    [SerializeField]
    private GameObject _powerUpText;
    [SerializeField]
    private AudioClip _pickupSound;

    //Luodaan delegaatti, joka ottaa vastaan stringin ja byten
    internal protected delegate void OnTriggerSendTag(string tag, byte slot);
    //Luodaan eventti
    internal protected static event OnTriggerSendTag SendTag;


    void OnEnable()
    {
        _triggerActive = true;
        _powerUpTag = this.gameObject.tag;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") && _triggerActive && SendTag != null)
        {
            //Käynnistetään event PowerUpManager luokassa
            SendTag(_powerUpTag, _powerUpSlot);            

            //Varmistetaan että disabloidaan tämä objekti vain jos powerup pystytään aktivoimaan tai on jo aktivoituna
            if (PowerUpManager.componentActivity != true)
            {
                AudioManager.instance.PlaySingle(_pickupSound);
                GameObject powertext = Instantiate(_powerUpText, transform.position, transform.rotation);
                Destroy(powertext, 2f);

                _triggerActive = false;
                //Disabloidaan tämä gameobject
                this.gameObject.SetActive(false);
            }
        }
    }
}
