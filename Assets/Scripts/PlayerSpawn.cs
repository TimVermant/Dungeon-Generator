using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private Transform _startPosition;
    [SerializeField] private GameObject _playerPrefab = null;
    // Start is called before the first frame update
    void Start()
    {
        if (_playerPrefab != null)
        {
            Instantiate(_playerPrefab, _startPosition);
        }

    }

}
