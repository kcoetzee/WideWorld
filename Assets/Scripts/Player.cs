using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    Ship _playerShip;
    // Start is called before the first frame update
    void Start()
    {
        _playerShip = new Ship("PLAYER", 20.0f, 5.0f, 5.0f);
        _playerShip.AddWeapon(new Weapon(WEAPONS_TYPE.CANNON, EFFECT_TYPE.NONE));
        _playerShip.AddWeapon(new Weapon(WEAPONS_TYPE.CANNON, EFFECT_TYPE.NONE));
        _playerShip._level = 1;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
