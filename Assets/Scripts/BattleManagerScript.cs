using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManagerScript : MonoBehaviour
{
    //public GameObject playerShip;
    //public GameObject enemyShip;
    
    private SkyNet _skyNet;
    private Ship _playerShip;
    private Ship _enemyShip;

    //private Battle _currBattle;
    int turnIndex = 0;
    // Start is called before the first frame update
    public void Start()
    {
        _playerShip = new Ship("PLAYER", 20.0f, 5.0f, 1.0f);
        _playerShip.AddWeapon(new Weapon(WEAPONS_TYPE.CANNON, EFFECT_TYPE.NONE));
        _playerShip.AddWeapon(new Weapon(WEAPONS_TYPE.CANNON, EFFECT_TYPE.NONE));
        _playerShip._level = 1;

        _enemyShip = new Ship("ENEMY", 20.0f, 5.0f, 5.0f);
        _enemyShip.AddWeapon(new Weapon(WEAPONS_TYPE.CANNON, EFFECT_TYPE.NONE));
        _enemyShip.AddWeapon(new Weapon(WEAPONS_TYPE.CANNON, EFFECT_TYPE.NONE));

        Debug.Log("Start");
        Turn();

        //_skyNet = new SkyNet(ref _playerShip, _playerShip._level);
        //_currBattle =  ScriptableObject.CreateInstance<Battle>();
        //_currBattle.StartBattle(_playerShip);
    }

    public void Turn()
    {
        Debug.Log("Turn Start");
        int MaxTurnsPerRound = _playerShip._crew.Count > _enemyShip._crew.Count ? _playerShip._crew.Count - 1 : _enemyShip._crew.Count - 1;
        Debug.Log(MaxTurnsPerRound);
        if (turnIndex <= MaxTurnsPerRound)
        {
            //ship turn first (player)
            Move shipMove;
            if (turnIndex <= _playerShip._turnMoves.Count)
            {
                shipMove = _playerShip._turnMoves[turnIndex];
            }
            else
            {
                shipMove = new Move(ABILITIES.NONE, 0);
            }
            //enemy turn second (

            Move enemyMove;
            if (turnIndex <= _enemyShip._turnMoves.Count)
            {
                enemyMove = _enemyShip._turnMoves[turnIndex];
            }
            else
            {
                enemyMove = new Move(ABILITIES.NONE, 0);
            }

            _playerShip.ExecuteTurn(turnIndex);
            _enemyShip.ExecuteTurn(turnIndex);

            _playerShip.update();
            _enemyShip.update();
            //_skyNet.update(this);
            turnIndex++;
        }
    }

    // Update is called once per frame
    public void Update()
    {
        
    }
}
