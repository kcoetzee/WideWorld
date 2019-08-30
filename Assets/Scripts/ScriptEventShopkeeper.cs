using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScriptEventShopkeeper : MonoBehaviour
{
    // Start is called before the first frame update
    public int foodStock;
    public int waterStock;
    public int cannonballStock;
    public Text foodStockUI;
    public Text waterStockUI;
    public Text cannonballStockTextUI;

    public Slider foodSlider;
    public Slider waterSlider;
    public Slider cannonballSlider;

    public GameObject specialItem;
    public GameObject player;

    void Start()
    {
        foodSlider.maxValue = foodStock;
        waterSlider.maxValue = waterStock;
        cannonballSlider.maxValue = cannonballStock;
    }

    // Update is called once per frame
    public void ClickButtonBuySupplies()
    {
        Debug.Log("HOOPA");
        player.GetComponent<PlayerScript>().food += (int)foodSlider.value;
        foodStock -= (int)foodSlider.value;
        player.GetComponent<PlayerScript>().water += (int)waterSlider.value;
        waterStock -= (int)waterSlider.value;
        player.GetComponent<PlayerScript>().cannonballs += (int)cannonballSlider.value;
        cannonballStock -= (int)cannonballSlider.value;

        foodSlider.maxValue = foodStock;
        waterSlider.maxValue = waterStock;
        cannonballSlider.maxValue = cannonballStock;

        foodSlider.value = 0;
        waterSlider.value = 0;
        cannonballSlider.value = 0;
    }

    void Update()
    {
        foodStockUI.text = foodSlider.value.ToString();
        waterStockUI.text = waterSlider.value.ToString();
        cannonballStockTextUI.text = cannonballSlider.value.ToString();

        transform.Find("specialText").GetComponent<Text>().text = specialItem.GetComponent<SpecialItemScript>().itemName;
        transform.Find("specialPrice ").GetComponent<Text>().text = specialItem.GetComponent<SpecialItemScript>().price.ToString();
        transform.Find("specialDescription").GetComponent<Text>().text = specialItem.GetComponent<SpecialItemScript>().description;
        transform.Find("specialImage").GetComponent<RawImage>().texture = specialItem.GetComponent<SpecialItemScript>().image;
    }
}
