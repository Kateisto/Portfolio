using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJump : MonoBehaviour
{
    private RaycastHit2D _sideBox;
    private RaycastHit2D _bottomBox;
    private Vector2 _sideBoxCastSize = new Vector2(0.280f, 2.74f);
    private Vector2 _bottomBoxCastSize = new Vector2(0.61f, 0.06f);
    private LayerMask _checkCollisionLayers;
    PlayerController _playerController;
    private Vector3 _sideBoxOrigin;
    private Vector3 _bottomBoxOrigin = Vector3.down * 1.456f;
    private bool _wallJumpActive;
    private bool _wallJumped;
    private bool _canWallJumpAgain;
    private byte _jumpLimit;
    private byte _jumpCount;
    [SerializeField]
    private Transform _jumpParticlePos;
    [SerializeField]
    private GameObject _jumpParticle;
    [SerializeField]
    private AudioClip _jumpSound;

    //Luodaan delegaatti, joka ottaa vastaan stringin
    internal protected delegate void DeactivatePowerUp(string slotStatus);
    //Luodaan eventti
    internal protected static event DeactivatePowerUp SendStatus;


    void OnEnable()
    {
        _playerController = this.gameObject.GetComponent<PlayerController>();
        _checkCollisionLayers = LayerMask.GetMask("Ground");
        PowerUpManager.SendStackLevel += StackWallJump;
        _wallJumpActive = false;
        _wallJumped = false;
        _canWallJumpAgain = false;
        _jumpLimit = 1;
        _jumpCount = 0;
    }

    void Update()
    {
        _bottomBox = Physics2D.BoxCast(transform.position + _bottomBoxOrigin, _sideBoxCastSize, 0, Vector2.zero, 0, _checkCollisionLayers);
        _sideBoxOrigin = Vector2.down * 0.05f + _playerController.facingDirection * 0.31f;
        _sideBox = Physics2D.BoxCast(transform.position + _sideBoxOrigin, _sideBoxCastSize, 0, Vector2.zero, 0, _checkCollisionLayers);


        //Jos ei osuta maahan eikä ole tämän hypyn aikana vielä hypätty seinästä
        //Tai jos ei osuta maahan ja ollaan jo hypätty seinästä, mutta ei olla irtauduttu seinästä hypyn jälkeen
        if (_bottomBox != true && _canWallJumpAgain != true)
        {
            //Jos ollaan kontaktissa seinän kanssa eikä olla vielä hypätty seinästä
            if (_sideBox && _wallJumped != true)
            {
                Jump();
            }

            //Jos ollaan hypätty jo seinästä ja ei osuta seinään
            else if (_wallJumped && _sideBox != true)
            {
                //Sallitaan seinästä hyppääminen uudestaan
                _canWallJumpAgain = true;
            }
        }

        //Jos osutaan maahan
        if (_bottomBox)
        {
            _wallJumpActive = false;
            _canWallJumpAgain = false;
            _wallJumped = false;
            _jumpCount = 0;
        }

        //Lasketaan gravitya jos tullaan uudelleen seinää vasten kun on hypätty seinästä, välissä seinästä irti olleena, eikä _jumpCount ole limitissä
        //Tai jos ollaan seinää vasten ensimmäistä kertaa maasta hyppäämisen jälkeen, eikä olla menossa ylöspäin seinän kanssa kontaktissa ollessa(suorittamassa hyppyä)
        if (_bottomBox != true && _sideBox && _canWallJumpAgain && _jumpCount < _jumpLimit || _bottomBox != true && _sideBox && _wallJumpActive != true && _playerController.velocity.y < 0)
        {
            _playerController.gravityModifier = 2.5f;
            _playerController.anim.SetBool("WallJump", true);
        }

        else
        {
            _playerController.gravityModifier = 5f;
            _playerController.anim.SetBool("WallJump", false);
        }
    }

    void LateUpdate()
    {
        //Jos ei osuta maahan ja ollaan aiemman seinähypyn jälkeen välissä irtauduttu seinästä ja ollaan nyt uudelleen kontaktissa seinän kanssa
        if (_bottomBox != true && _canWallJumpAgain && _sideBox)
        {
            Jump();
        }
    }

    void Jump()
    {
        //Hypätään seinästä
        if (Input.GetButtonDown("Jump") && _jumpCount < _jumpLimit)
        {
            _playerController.velocity.y = _playerController.jumpForce;

            //Lisätään hyppy ääniefekti
            AudioManager.instance.JumpEffects(_jumpSound);
            //Instantoidaan hypyn partikkeliefekti
            Instantiate(_jumpParticle, _jumpParticlePos);

            _wallJumped = true;
            _canWallJumpAgain = false;
            _wallJumpActive = true;
            _jumpCount += 1;
        }
    }

    //Muokataan _jumpLimit muuttuja arvoa stack levelin mukaan
    void StackWallJump(byte stackLevel)
    {
        if (stackLevel == 1)
        {
            _jumpLimit = 2;
        }

        else if (stackLevel == 2)
        {
            _jumpLimit = 3;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(transform.position + _sideBoxOrigin, _sideBoxCastSize);
        Gizmos.DrawCube(transform.position + _bottomBoxOrigin, _bottomBoxCastSize);
    }

    void OnDisable()
    {
        PowerUpManager.SendStackLevel -= StackWallJump;
        _playerController.gravityModifier = 5f;

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
