using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PopupController
{
    public GameObject _attackPrefab;
    public GameObject _damagePrefab;
    public GameObject _dodgePrefab;
    public GameObject _defendPrefab;
    public GameObject _battleCanvas; 

    public GameObject _messagePrefab; 
    public PopupController(){
        _battleCanvas = GameObject.Find("PopupPrefab");
        _messagePrefab = GameObject.Find("_messagePopup");
        
    }

   public void CreatePopup(ABILITIES _ability = ABILITIES.NONE, int value = 0, string message = ""){
       GameObject _popupObject;
       if (_ability == ABILITIES.ATTACK){
            _popupObject = Object.Instantiate(_attackPrefab, Vector3.zero, Quaternion.identity);
            _popupObject.GetComponent<Popup>().Setup(_popupObject,1.0f);
       }
       if (_ability == ABILITIES.DEFEND){
            _popupObject = Object.Instantiate(_defendPrefab, Vector3.zero, Quaternion.identity);
       }
       if (_ability == ABILITIES.DODGE){
            _popupObject = Object.Instantiate(_dodgePrefab, Vector3.zero, Quaternion.identity);
       }
       if (_ability == ABILITIES.NONE){
            _messagePrefab.GetComponent<Popup>().Setup(_messagePrefab,1.0f);
       }
   }
}


