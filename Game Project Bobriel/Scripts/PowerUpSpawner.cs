using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _powerUpPrefabs;
    private int _powerUpNumber;

    void Start()
    {
        _powerUpNumber = Random.Range(1, 3);

        switch (_powerUpNumber)
        {
            case 1:
                Instantiate(_powerUpPrefabs[0], transform.position, Quaternion.identity);
                break;

            case 2:
                Instantiate(_powerUpPrefabs[1], transform.position, Quaternion.identity);
                break;
        }

        Destroy(this.gameObject);
    }
}
