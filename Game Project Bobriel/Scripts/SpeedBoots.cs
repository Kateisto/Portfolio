using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoots : MonoBehaviour
{
    private float _addedSpeed;
    PlayerController _playerController;
    private float[] _gravity = new float[3];
    private float[] _movingSpeed = new float[3];
    internal static float currentGravity = 5f;
    internal static byte powerUpStage = 0;
    [SerializeField]
    private float _defaultJumpForce = 32f;
    [SerializeField]
    private float _defaultMovingSpeed = 10f;
    [SerializeField]
    private Animator _anim;

    //Luodaan delegaatti, joka ottaa vastaan stringin
    internal protected delegate void DeactivatePowerUp(string slotStatus);
    //Luodaan eventti
    internal protected static event DeactivatePowerUp SendStatus;


    void OnEnable()
    {
        _playerController = this.gameObject.GetComponent<PlayerController>();
        //Aloitetaan PowerUpManager luokan SendStackLevel eventin kuuntelu
        PowerUpManager.SendStackLevel += StackSpeed;

        _gravity[0] = (_playerController.gravityModifier = 6f);
        _playerController.jumpForce = 35.1073f;
        _movingSpeed[0] = (_playerController.movingSpeed = 13.333f);
        _anim.SetFloat("runningSpeed", 1.333f);
        currentGravity = _gravity[0];
        powerUpStage = 1;
    }

    //Muokataan pelaajan nopeutta ja painovoimaa stackauksen mukaan
    void StackSpeed(byte stackLevel)
    {
        if (stackLevel == 1)
        {
            _gravity[1] = (_playerController.gravityModifier = _gravity[0] + 1f);
            _playerController.jumpForce = 37.9695f;
            _movingSpeed[1] = (_playerController.movingSpeed = _movingSpeed[0] + 3.333f);
            _anim.SetFloat("runningSpeed", 1.666f);
            currentGravity = _gravity[1];
            powerUpStage = 2;
        }

        else if (stackLevel == 2)
        {
            _gravity[2] = (_playerController.gravityModifier = _gravity[1] + 1f);
            _playerController.jumpForce = 40.6461f;
            _movingSpeed[2] = (_playerController.movingSpeed = _movingSpeed[1] + 3.333f);
            _anim.SetFloat("runningSpeed", 1.999f);
            currentGravity = _gravity[2];
            powerUpStage = 3;
        }
    }

    void OnDisable()
    {
        //Asetetaan gravity default arvoon
        currentGravity = 5f;
        //Nollataan tämän powerupin taso
        powerUpStage = 0;

        //Asetetaan liikkumisnopeus takaisin default arvoon
        _playerController.movingSpeed = _defaultMovingSpeed;

        //Asetetaan hyppyvoima takaisin default arvoon
        _playerController.jumpForce = _defaultJumpForce;

        _anim.SetFloat("runningSpeed", 1f);

        //Lopetetaan PowerUpManager luokan SendStackLevel eventin kuuntelu
        PowerUpManager.SendStackLevel -= StackSpeed;

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
