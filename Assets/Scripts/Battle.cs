using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AI_TYPE
{
    AGGRESSIVE = 0,
    DEFENSIVE = 1,
    RANDOM = 2
}

public class Strategy
{
    public int _aggro;
    public int _defensive;
    public int _chaos;
    public Ship _ship;
    public List<Move> _stratList;


    public Strategy(int aggro, int def, int chaos, Ship ship)
    {
        if (aggro + def + chaos != 10)
        {
            Debug.Log("ITS GONNA BREAK!!");
        }
        _aggro = aggro;
        _defensive = def;
        _chaos = chaos;
        _ship = ship;
        _stratList = new List<Move>();
    }

    public List<Move> GenerateStrategyList()
    {
        for (int i = 0; i < _aggro; i++)
        {
            _stratList.Add(new Move(ABILITIES.ATTACK, Random.Range(0, _ship._weapons.Count)));
        }
        for (int i = 0; i < _defensive; i++)
        {
            if (Random.Range(0, 1) == 1)
            {
                _stratList.Add(new Move(ABILITIES.DEFEND, 0));
            }
            else
            {
                _stratList.Add(new Move(ABILITIES.DODGE, 0));
            }
        }
        for (int i = 0; i < _chaos; i++)
        {
            ABILITIES ablity = (ABILITIES)Random.Range(0, 3);

            if (ablity == ABILITIES.ATTACK)
            {
                _stratList.Add(new Move(ABILITIES.ATTACK, Random.Range(0, _ship._weapons.Count)));
            }
            else
            {
                _stratList.Add(new Move(ablity, 0));
            }
        }
        return _stratList;
    }

}


public class SkyNet
{
    public Ship _playerShip;
    public int _level;
    public float _levelModifier;
    public List<Move> _currentSet;
    public List<Move> _lastSet;
    public Strategy _strategy;
    public SkyNet(ref Ship ship, int level, Strategy strat)
    {
        _playerShip = ship;
        _level = level;
        _strategy = strat;
        ApplyLevelModifier();
        _lastSet = new List<Move>();
        _currentSet = new List<Move>();


        GenerateCrewList();
        GenerateMoveList();
    }


    public void GenerateMoveList()
    {
        int index = 0;
        List<Move> stratList = _strategy.GenerateStrategyList();
        foreach (Crew _man in _playerShip._crew)
        {
            //gen list of moves one per crew memeber
            int moveIndex = Random.Range(0, stratList.Count);
            _currentSet.Insert(index, stratList[moveIndex]);
            stratList.RemoveAt(moveIndex);
            index++;
        }
        _playerShip._turnMoves = _currentSet;
    }

    public void GenerateCrewList()
    {
        int crewCount = (int)(2.0f * _levelModifier);
        _playerShip._crew = new List<Crew>();
        for (int i = 0; i < crewCount; i++)
        {
            SPECIES species = (SPECIES)Random.Range(0, 3);
            _playerShip._crew.Add(new Crew(species));
        }
    }

    public void update(Battle _currentBattle)
    {
        GenerateMoveList();
        // if (_playerShip._health < _currentBattle._playerShip._health)
        // {
        //     GenerateMoveList();
        // }
    }

    public void ApplyLevelModifier()
    {
        if (_level < 5)
        {
            _levelModifier = 1.5f;
        }
        if (5 < _level && _level <= 10)
        {
            _levelModifier = 2.0f;
        }
        if (_level > 10)
        {
            _levelModifier = 2.5f;
        }
    }

}

public class Battle : ScriptableObject
{

    public Ship _enemy;

    public bool _nextTurn = false;
    public SkyNet _skyNet;
    public Ship _playerShip;

    bool _battleActive = false;
    bool _nextRound = false;
    int turnIndex = 0;

    public void StartBattle(Ship playerShip, Ship enemy)
    {
        _playerShip = playerShip;
        _enemy = enemy;

        // prepare skynet lists
        
        _skyNet = new SkyNet(ref _enemy, _playerShip._level, new Strategy(7,2,1, _enemy));

        //set targets 
        _enemy._target = _playerShip;
        _playerShip._target = _enemy;
        _battleActive = true;
    }


    public void StartNextRound()
    {

    }

    // Start is called before the first frame update
    public void Awake()
    {

    }

    // Update is called once per frame
    public bool Update()
    {

        if (Input.GetKeyDown(KeyCode.Space) && _battleActive)
        {
            _nextRound = true;
        }

        if (_nextRound)
        {
            int MaxTurnsPerRound = _playerShip._crew.Count > _enemy._crew.Count ? _playerShip._crew.Count - 1 : _enemy._crew.Count - 1;
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
                if (turnIndex <= _enemy._turnMoves.Count)
                {
                    enemyMove = _enemy._turnMoves[turnIndex];
                }
                else
                {
                    enemyMove = new Move(ABILITIES.NONE, 0);
                }

                _playerShip.ExecuteTurn(turnIndex);
                _enemy.ExecuteTurn(turnIndex);

                _playerShip.update();
                _enemy.update();
                _skyNet.update(this);
                turnIndex++;
            }
            else
            {
                turnIndex = 0;
                _nextRound = false;
            }

        }
        if (!_playerShip.isAlive || !_enemy.isAlive)
        {
            _battleActive = false;
        }

        return _battleActive;

    }

}

public class _battlePopupTimer : MonoBehaviour
{
    float timer = 0.0f;


    public void UpdateTimer()
    {
        timer += Time.deltaTime;
    }

    public void ResetTimer()
    {
        timer = 0.0f;
    }

    public bool CheckTimer(float time)
    {
        if (timer <= time)
        {
            return true;
        }
        return false;
    }
}
