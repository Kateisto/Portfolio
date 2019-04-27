using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    private short _health;
    internal bool damageImmunity;
    [SerializeField]
    private GameObject[] _heartIcons = new GameObject[3];
    Scene currentScene;
    private Animator _animator;

    protected internal short GetHealth { get { return _health; } }

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        _health = 3;

        for (int i = 0; i < _heartIcons.Length; i++)
        {
            _heartIcons[i].SetActive(true);
        }

        //Aloitetaan eventtien kuuntelu kun tämä komponentti muuttuu aktiiviseksi
        Fireball.SendDamage += TakeDamage;
        Damage.SendDamage += TakeDamage;

        damageImmunity = false;

        currentScene = SceneManager.GetActiveScene();
    }

    void TakeDamage(short damageReceived)
    {
        //Jos ei olla immuuneja damagelle niin vähennetään healthista vastaanotettu damage sekä disabloidaan UI:ssa olevia sydämiä sen mukaisesti
        if (damageImmunity != true)
        {

            _health -= damageReceived;

            if (_health > 0)
            {
                _animator.SetTrigger("damage");
            }

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
                StartCoroutine(Die());
            }

            //Joka kerta damagen ottamisen jälkeen ollaan puoli sekuntia immuuneja damagelle
            StartCoroutine(ImmuneToDamage());
        }
    }

    IEnumerator ImmuneToDamage()
    {
        if (damageImmunity != true)
        {
            damageImmunity = true;
            yield return new WaitForSeconds(0.5f);
            damageImmunity = false;
        }        
    }

    internal void SaveCurrentHealthStatus()
    {
        SaveManagerOld.SaveCurrentHealth(this);
    }

    internal void LoadCurrentHealthStatus()
    {
        SaveHealth saveHealth = SaveManagerOld.LoadHealth();

        _health = saveHealth.health;
    }

    IEnumerator Die()
    {
        _animator.SetTrigger("death");
        PlayerController playerController = GetComponent<PlayerController>();
        playerController.enabled = false;
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(1f);
        DeathScreen.Death();
        Time.timeScale = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("InstantDeath"))
        {
            StartCoroutine(Die());
        }
    }

    void OnDisable()
    {
        //Lopetetaan eventtien kuuntelu jos tämä komponentti muuttuu inaktiiviseksi
        Fireball.SendDamage -= TakeDamage;
        Damage.SendDamage -= TakeDamage;
    }

    void OnDestroy()
    {
        StopCoroutine(ImmuneToDamage());
        StopCoroutine(Die());
    }
}

