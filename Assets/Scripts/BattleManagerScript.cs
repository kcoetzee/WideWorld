using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManagerScript : MonoBehaviour
{
    //public GameObject playerShip;
    //public GameObject enemyShip;

    private SkyNet _skyNet;
    private SkyNet _skyNetEnemy;
    private Ship _playerShip;
    private Ship _enemyShip;

    public bool _battleOver;
    public bool _battleStart;

    public float _timer;
    //private Battle _currBattle;
    int turnIndex = 0;
    // Start is called before the first frame update
    public void Start()
    {
        
    }

    public void StartBattle(){
        _playerShip = new Ship("PLAYER", 20.0f, 5.0f, 1.0f);
        _playerShip.AddWeapon(new Weapon(WEAPONS_TYPE.CANNON, EFFECT_TYPE.NONE));
        _playerShip.AddWeapon(new Weapon(WEAPONS_TYPE.CANNON, EFFECT_TYPE.NONE));
        _playerShip._level = 1;

        _enemyShip = new Ship("ENEMY", 20.0f, 5.0f, 5.0f);
        _enemyShip.AddWeapon(new Weapon(WEAPONS_TYPE.CANNON, EFFECT_TYPE.NONE));
        _enemyShip.AddWeapon(new Weapon(WEAPONS_TYPE.CANNON, EFFECT_TYPE.NONE));

        _skyNet = new SkyNet(ref _playerShip, _playerShip._level, new Strategy(7, 2, 1, _playerShip));
        _skyNetEnemy = new SkyNet(ref _enemyShip, _enemyShip._level, new Strategy(7, 2, 1, _enemyShip));

        _playerShip._target = _enemyShip;
        _enemyShip._target = _playerShip;
        _battleOver = false;
        _battleStart = true;

        Debug.Log("Start");
        _timer = 0.0f;
        Turn();
    }

    public void Turn()
    {
        if (_timer >0.2f)
        {
            Debug.Log("Turn Start");
            int MaxTurnsPerRound = _playerShip._crew.Count > _enemyShip._crew.Count ? _playerShip._crew.Count - 1 : _enemyShip._crew.Count - 1;
            if (turnIndex <= MaxTurnsPerRound && !_battleOver)
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
                _timer = 0.0f;
            }
        }

    }

    // Update is called once per frame
    public void Update()
    {
        _timer+= Time.deltaTime;
        if (!_playerShip.isAlive || !_enemyShip.isAlive)
        {
            _battleOver = true;
            _battleStart = false;
        }
        else
        {
            int MaxTurnsPerRound = _playerShip._crew.Count > _enemyShip._crew.Count ? _playerShip._crew.Count - 1 : _enemyShip._crew.Count - 1;
            if (turnIndex > MaxTurnsPerRound)
            {
                turnIndex = 0;
                //next round    
            }
            Turn();
        }
    }
}
