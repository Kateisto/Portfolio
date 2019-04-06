using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _rockPrefab;
    [SerializeField]
    private int _pooledAmount;
    [SerializeField]
    private float _poolSpeed;
    [SerializeField]
    private float _poolRepeatRate;
    private float _spawnRadiusX;
    private Collider2D _trigger;

    List<GameObject> items;


    void Start()
    {
        items = new List<GameObject>();
        _trigger = GetComponent<Collider2D>();        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            for (int i = 0; i < _pooledAmount; i++)
            {
                GameObject obj = (GameObject)Instantiate(_rockPrefab);
                obj.SetActive(false);
                items.Add(obj);
            }

            InvokeRepeating("DropRock", _poolSpeed, _poolRepeatRate);

            _trigger.enabled = false;
        }
    }

    void DropRock()
    {
        _spawnRadiusX = Random.Range(-3f, 3f);

        for (int i = 0; i < items.Count; i++)
        {
            if (!items[i].activeInHierarchy)
            {
                items[i].transform.position = new Vector2(_spawnRadiusX, 1086f);
                items[i].SetActive(true);
                break;
            }
        }
    }
}
