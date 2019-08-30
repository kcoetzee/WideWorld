using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ABILITIES
{
    ATTACK = 0,
    DEFEND = 1,
    DODGE = 2,
    BILGE = 3,
    NONE = 4
}


public enum SPECIES
{
    HUMAN = 0,
    SQUID = 1,
    SHARK = 2
}

public enum WEAPONS_TYPE
{
    CANNON,
    RAILGUN
}

public enum EFFECT_TYPE
{
    NONE,
    DISABLE
}

public class Crew
{
    public SPECIES _species;

    public Crew(SPECIES species)
    {
        _species = species;
    }

}



public class Weapon
{
    public float _damage;

    public bool _disabled;

    public int _cooldown;

    public WEAPONS_TYPE _type;

    public EFFECT_TYPE _effect;


    public Weapon(WEAPONS_TYPE type, EFFECT_TYPE effect)
    {
        if (type == WEAPONS_TYPE.CANNON){
            _damage = 10.0f;
            _cooldown = 2;
        }
        if (type == WEAPONS_TYPE.RAILGUN){
            _damage = 15f;
            _cooldown = 4;
        }
        _type = type;
        _effect = effect;
    }



    public void update()
    {
        if (_cooldown > 0)
        {
            _cooldown--;
        }
    }

}

public struct Move
{
    public ABILITIES _ablity;
    public bool _cooldown;
    public int _weaponIndex;

    public Move(ABILITIES move, int WeaponIndex)
    {
        _ablity = move;
        _weaponIndex = WeaponIndex;
        _cooldown = false;
    }
};


public class Ship
{
    public GameObject _shipCenter;
    public bool isAlive;
    public float _health;
    public float _dodge;
    public float _resistance;
    public int _level;
    public bool _dodgeEnabled;
    public string _name;

    public float _oldResistance;
    public List<Move> _turnMoves;

    public List<Crew> _crew;
    public List<Weapon> _weapons;

    public Ship _target;

    public PopupController _popupController;

    public Ship(string name ,float health, float dodge, float resistance, int numCrew)
    {
        _health = health;
        _dodge = dodge;
        _resistance = resistance;
        _crew = new List<Crew>();

        _crew.

        _turnMoves = new List<Move>();
        _name = name;
        _weapons = new List<Weapon>();
        _popupController = new PopupController();
        isAlive = true;

    }

    public void AddWeapon(Weapon weapon)
    {
        _weapons.Add(weapon);
    }

    public void Damage(float Damage, EFFECT_TYPE effect)
    {
        if (!_dodgeEnabled)
        {
            if ((Damage - _resistance) > 0.0f){
                Debug.Log(_name + " TAKES " + (Damage - _resistance).ToString() + " DAMAGE");
                _popupController.CreatePopup(ABILITIES.NONE,0, _name + " TAKES " + (Damage - _resistance).ToString() + " DAMAGE");
                
                _health -= (Damage - _resistance);
            } 
            else{
                Debug.Log(_name + " BLOCKS " + Damage.ToString() + " DAMAGE");
            }
        }
        else
        {
            Debug.Log(_name + " DODGES " + Damage.ToString() + " DAMAGE");
        }
    }

    public void Attack(int WeaponIndex)
    {
        _target.Damage(_weapons[WeaponIndex]._damage, _weapons[WeaponIndex]._effect);
    }

    public void update()
    {
        foreach (Weapon gat in _weapons)
        {
            gat.update();
        }
        if (_health <= 0)
        {
            isAlive = false;
            Debug.Log("DEAD AF SUN!");
        }
        _dodgeEnabled = false;
    }

    public void ExecuteTurn(int turn)
    {
        Debug.Log("Executing turn");
        Move cMove = _turnMoves[turn];

        if (cMove._ablity == ABILITIES.ATTACK)
        {
            Debug.Log(_name + ": ATTACKS");
            Attack(cMove._weaponIndex);
        }
        else if (cMove._ablity == ABILITIES.DEFEND)
        {
            if (_oldResistance != 0.0f){
                _resistance = _oldResistance;
            }
            Debug.Log(_name + ": DEFENDS");
       
            _oldResistance = _resistance;
            _resistance *= 2.0f;
        }
        else if (cMove._ablity == ABILITIES.DODGE)
        {
            Debug.Log(_name + ": DODGES");
            _dodgeEnabled = true;
        }
        else if (cMove._ablity == ABILITIES.BILGE)
        {
            _health += 2;
        }
      

    }
}
