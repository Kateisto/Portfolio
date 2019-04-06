using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    private short _health;
    private bool _damageImmunity;
    [SerializeField]
    private GameObject[] _heartIcons = new GameObject[3];
    Scene currentScene;


    void OnEnable()
    {
        _health = 3;

        _heartIcons[0].SetActive(true);
        _heartIcons[1].SetActive(true);
        _heartIcons[2].SetActive(true);

        //Aloitetaan eventtien kuuntelu kun tämä komponentti muuttuu aktiiviseksi
        ProjectileDamage.SendDamage += TakeDamage;
        Damage.SendDamage += TakeDamage;

        _damageImmunity = false;

        currentScene = SceneManager.GetActiveScene();
    }

    void TakeDamage(short damageReceived)
    {
        //Jos ei olla immuuneja damagelle niin vähennetään healthista vastaanotettu damage sekä disabloidaan UI:ssa olevia sydämiä sen mukaisesti
        if (_damageImmunity != true)
        {           
            _health -= damageReceived;

            if (_health == 2)
            {
                _heartIcons[2].SetActive(false);
            }

            else if (_health == 1)
            {
                _heartIcons[1].SetActive(false);
            }

            else if (_health == 0)
            {
                _heartIcons[0].SetActive(false);

                SceneManager.LoadScene(currentScene.buildIndex);
            }

            //Joka kerta damagen ottamisen jälkeen ollaan puoli sekuntia immuuneja damagelle
            StartCoroutine("ImmuneToDamage");
        }
    }

    IEnumerator ImmuneToDamage()
    {
        _damageImmunity = true;
        yield return new WaitForSeconds(0.5f);
        _damageImmunity = false;
    }

    void OnDisable()
    {
        //Lopetetaan eventtien kuuntelu jos tämä komponentti muuttuu inaktiiviseksi
        ProjectileDamage.SendDamage -= TakeDamage;
        Damage.SendDamage -= TakeDamage;
    }
}
