using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PowerUpManager : MonoBehaviour
{
    private string _incTag;
    private byte _incSlot;
    private string _activeInSlot_1;
    private string _activeInSlot_2;
    private GameObject _thisObject;
    [SerializeField]
    private GameObject _speedBootsIcon;
    [SerializeField]
    private GameObject _superJumpIcon;
    [SerializeField]
    private GameObject _shieldIcon;
    private byte _stackLevel;
    internal static bool componentActivity;
    private GameObject _currentActiveIcon;
    private GameObject _currentPassiveIcon;

    //Luodaan delegaatti, joka ottaa vastaan byten
    internal protected delegate void DeactivatePowerUp(byte stackLevel);
    //Luodaan eventti
    internal protected static event DeactivatePowerUp SendStackLevel;


    void OnEnable()
    {
        //Aloitetaan PowerUpObejct luokkien eventin kuuntelu kun tämä komponentti muuttuu aktiiviseksi
        PowerUpObject.SendTag += PickedPowerUp;
        //Muuttuja joka on luotu vain koodin lukemisen helpottamiseksi
        _thisObject = this.gameObject;
        _thisObject.GetComponent<SuperJump>().enabled = false;
        _thisObject.GetComponent<SpeedBoots>().enabled = false;
        _thisObject.GetComponent<Shield>().enabled = false;
        _speedBootsIcon.SetActive(false);
        _superJumpIcon.SetActive(false);
        _shieldIcon.SetActive(false);
        _stackLevel = 0;
    }

    void PickedPowerUp(string tag, byte slot)
    {
        //PowerUpObject komponenteilta vastaanotettava tagi
        _incTag = tag;
        //PowerUpObject komponenteilta vastaanotettava slot numero, joka määrittää kuuluuko powerup passiiviseen vai aktiiviseen slottiin
        _incSlot = slot;

        //Jos poimitun powerupin slot on 1(aktiivisten poweruppien slot) ja slotissa ei ole aktiivisena poweruppia
        //Tai jos aktiivisena on powerup, mutta se ei ole sama kuin juuri poimittu
        if (_incSlot == 1 && _activeInSlot_1 == null || _incSlot == 1 && _activeInSlot_1 != _incTag)
        {
            //Jos poimitaan uusi powerup niin disabloidaan vanha powerup ja powerup ikoni
            if (_activeInSlot_1 != null)
            {
                (_thisObject.GetComponent(_activeInSlot_1) as MonoBehaviour).enabled = false;
                _currentActiveIcon.SetActive(false);
            }

            //Aktivoidaan powerup jos se ei ole vielä aktiivinen
            if ((_thisObject.GetComponent(_incTag) as MonoBehaviour).enabled != true)
            {
                componentActivity = false;

                (_thisObject.GetComponent(_incTag) as MonoBehaviour).enabled = true;
                _activeInSlot_1 = _incTag;

                EnableIconAndSubscribeEvent();
            }

            //Jos komponentti on jo aktiivisena, muutetaan componentActivity trueksi(muuttujaa tulkitaan PowerUpObject scriptissä)
            else if ((_thisObject.GetComponent(_incTag) as MonoBehaviour).enabled == true)
            {
                componentActivity = true;
            }
        }

        //Jos vastaanotettu slot numero on 2(passiivisten poweruppien slot)
        else if (_incSlot == 2)
        {
            //Jos aktiivisena on joku powerup, mutta se ei ole sama kuin poimittu powerup
            if (_activeInSlot_2 != null && _activeInSlot_2 != _incTag)
            {
                //Jos poimitaan uusi powerup niin disabloidaan vanha powerup ja powerup ikoni
                (_thisObject.GetComponent(_activeInSlot_2) as MonoBehaviour).enabled = false;
                _currentPassiveIcon.SetActive(false);

                //Nollataan stackaus kun powerup vaihdetaan toiseen
                _stackLevel = 0;
            }

            //Jos aktiivinen powerup ei ole sama kuin poimittu powerup tai aktiivisena ei ole mitään poweruppia
            if (_activeInSlot_2 != _incTag || _activeInSlot_2 == null)
            {
                //Aktivoidaan powerup
                (_thisObject.GetComponent(_incTag) as MonoBehaviour).enabled = true;
                _activeInSlot_2 = _incTag;

                EnableIconAndSubscribeEvent();
            }

            //Jos poimittu powerup on sama kuin aktiivisena oleva niin kutsutaan StackPassivePowerUp(); metodia, joka suorittaa stackauksen
            else if (_activeInSlot_2 == _incTag)
            {
                StackPassivePowerUp();
            }
        }
    }

    //Stackataan passiivisia poweruppeja eli lähetetään tieto powerup luokalle stackin tasosta
    void StackPassivePowerUp()
    {
        if (_stackLevel > 2)
        {
            _stackLevel = 2;
        }

        else
        {
            _stackLevel += 1;

            SendStackLevel(_stackLevel);
        }
    }

    //Aktivoidaan poimitun powerupin ikoni näkyväksi UI slotissa ja aloitetaan kyseisen powerup luokan eventin kuuntelu
    void EnableIconAndSubscribeEvent()
    {
        if (_incTag == "SuperJump")
        {
            SuperJump.SendStatus += SuperJumpStatus;
            _superJumpIcon.SetActive(true);
            _currentActiveIcon = _superJumpIcon;
        }

        else if (_incTag == "Shield")
        {
            Shield.SendStatus += ShieldStatus;
            _shieldIcon.SetActive(true);
            _currentActiveIcon = _shieldIcon;
        }

        else if (_incTag == "SpeedBoots")
        {
            SpeedBoots.SendStatus += SpeedBootsStatus;
            _speedBootsIcon.SetActive(true);
            _currentPassiveIcon = _speedBootsIcon;
        }
    }

    void SuperJumpStatus(string slotStatus)
    {
        _activeInSlot_1 = slotStatus;

        //Lopetetaan SuperJump luokan SendStatus eventin kuuntelu
        SuperJump.SendStatus -= SuperJumpStatus;
        //Disabloidaan UI ikoni
        _superJumpIcon.SetActive(false);
        _currentActiveIcon = null;
    }

    void SpeedBootsStatus(string slotStatus)
    {
        _activeInSlot_2 = slotStatus;

        //Lopetetaan SpeedBoots luokan SendStatus eventin kuuntelu
        SpeedBoots.SendStatus -= SpeedBootsStatus;
        //Disabloidaan UI ikoni
        _speedBootsIcon.SetActive(false);
        _currentPassiveIcon = null;
        //Nollataan stackaus
        _stackLevel = 0;
    }

    void ShieldStatus(string slotStatus)
    {
        _activeInSlot_1 = slotStatus;

        //Lopetetaan Shield luokan SendStatus eventin kuuntelu
        Shield.SendStatus -= ShieldStatus;
        //Disabloidaan UI ikoni
        _shieldIcon.SetActive(false);
        _currentActiveIcon = null;
    }

    void OnDisable()
    {
        //Lopetetaan eventtien kuuntelu jos tämä komponentti muuttuu inaktiiviseksi
        PowerUpObject.SendTag -= PickedPowerUp;
        SuperJump.SendStatus -= SuperJumpStatus;
        SpeedBoots.SendStatus -= SpeedBootsStatus;
        Shield.SendStatus -= ShieldStatus;
    }
}
