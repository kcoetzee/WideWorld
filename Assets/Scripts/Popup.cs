using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Popup : MonoBehaviour{
    float _totalTime = 0.0f;
    public float _lifetime;
    public GameObject _this;
    public Text _text;

    public void Setup(GameObject popup, float lifetime){
        _lifetime = lifetime;
        _this = popup;
        _text = transform.GetComponent<Text>();
    }

    public void SetString(string text){
         Text _text = transform.GetComponent<Text>();
        _text.text = text;
    }

    public void SetFloat(float val){
        Text _text = transform.GetComponent<Text>();
        _text.text = val.ToString();
    }

     void Update(){
         if  (_text)
         {
            if (_totalTime >= _lifetime){
                _text.text = "";
                _totalTime = 0.0f;
            }
            _totalTime += Time.deltaTime;
         }
    }

}