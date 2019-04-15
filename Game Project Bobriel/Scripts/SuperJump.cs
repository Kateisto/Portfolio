using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperJump : MonoBehaviour
{
    private float _addedJumpForce = 79.3f;
    PlayerController _playerController;
    private LayerMask _checkCollisionLayers;
    private bool _isAirborne;
    private bool _powerUpUsed;
    private RaycastHit2D _box;
    private Vector2 _boxCastSize = new Vector2(0.55f, 2.75f);
    private bool _firstRun;

    //Luodaan delegaatti, joka ottaa vastaan stringin
    internal protected delegate void DeactivatePowerUp(string slotStatus);
    //Luodaan eventti
    internal protected static event DeactivatePowerUp SendStatus;


    void OnEnable()
    {
        _playerController = this.gameObject.GetComponent<PlayerController>();
        _addedJumpForce *= 1f;

        _checkCollisionLayers = LayerMask.GetMask("Ground");
        _powerUpUsed = false;
        _firstRun = true;
    }

    void Update()
    {
        //Käytetään powerup
        if (Input.GetButtonDown("ActivePowerup") && _powerUpUsed != true)
        {
            _isAirborne = true;

            _powerUpUsed = true;

            _playerController.hasJumped = false;

            CheckSpeedBootsStage();

            //Lisätään pelaajan y velocityyn _addedJumpForce arvo
            _playerController.velocity.y = _addedJumpForce;

            //Käynnistetään event PowerUpManagerissa ja lähetetään tieto että powerup on käytetty
            SendStatus(null);
        }

        StartCoroutine(SetGravity());
    }

    IEnumerator SetGravity()
    {
        //Jos ollaan käytetty powerup eli ollaan ilmassa tai lähdössä ilmaan
        if (_isAirborne)
        {
            //Jos metodi ajetaan ensimmäisen kerran eli silloin kun ollaan lähdössä maasta ilmaan
            //niin odotellaan sadaosasekunnin verran, että päästään maasta irti ennen kuin aloitetaan BoxCast
            if (_firstRun)
            {
                yield return new WaitForSeconds(0.1f);
                _firstRun = false;
            }

            _box = Physics2D.BoxCast(transform.position + Vector3.down * 0.05f, _boxCastSize, 0, Vector2.zero, 0, _checkCollisionLayers);

            //Jos ei olla maassa niin muutetaan gravity vakioksi(5f)
            if (_box != true)
            {
                _playerController.gravityModifier = 5f;
            }

            //Kun osutaan maahan niin muutetaan gravity takaisin siihen mikä se oli ennen powerupin käyttöä
            else
            {
                _playerController.gravityModifier = SpeedBoots.currentGravity;
                _isAirborne = false;
                this.enabled = false;
            }
        }
    }

    //Tarkastellaan onko aktiivisena speedboots powerup ja millä stack tasolla se on
    //_addedJumpForcea säädellään sen mukaisesti
    void CheckSpeedBootsStage()
    {
        if (SpeedBoots.powerUpStage == 0)
        {
            _addedJumpForce *= 1f;
        }

        else if (SpeedBoots.powerUpStage == 1)
        {
            _addedJumpForce *= 1.01437957125f;
        }

        else if (SpeedBoots.powerUpStage == 2)
        {
            _addedJumpForce *= 1.02876166456f;
        }

        else if (SpeedBoots.powerUpStage == 3)
        {
            _addedJumpForce *= 1.04314249685f;
        }
    }

    //Lopetetaan kesken oleva Coroutine jos tämä komponentti tuhoutuu
    void OnDestroy()
    {
        StopCoroutine("SetGravity");
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
