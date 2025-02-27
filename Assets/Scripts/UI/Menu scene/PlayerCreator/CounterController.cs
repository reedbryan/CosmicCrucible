using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI; // For UI components
using System.Collections.Generic;


public class CounterController : MonoBehaviour
{
    PlayerCreator playerCreator;
    public Text valueText;            // Text element

    public int displayed_value;       // The value displayed by the interface
    public int value_weight;          // How much it costs/pays to increase/decrease this value
    [SerializeField]private int initial_value;
    public float current_value;       // The adjusted value (based on the base_values multiplier)
    
    void Awake(){
        // get PlayerCreator script
        playerCreator = transform.parent.transform.parent.GetComponent<PlayerCreator>();
        
        // initialie values
        initial_value = displayed_value;
        UpdateValue();
    }
    
    public void IncrementValue()
    {
        // check viability
        if (playerCreator.budget < value_weight) 
            return;
        
        // update values
        playerCreator.UpdateBudget(
            -(int)(value_weight * (1 + ((displayed_value - initial_value) / 10f)))
        ); // have weight scale with the difference from the base value
        displayed_value++;
        UpdateValue();
    }

    public void DecrementValue()
    {
        // check viability
        if (displayed_value <= 1) 
            return;
        
        displayed_value--;
        UpdateValue();

        // update values
        playerCreator.UpdateBudget(
            (int)(value_weight * (1 + ((displayed_value - initial_value) / 10f)))
        ); // have weight scale with the difference from the base value
    }

    void UpdateValue()
    {
        // update the displayed value
        valueText.text = displayed_value.ToString();

        // check that we don't need to invert the value
        int temp_value = displayed_value;
        string name = gameObject.name;
        if (name == "fire rate" ||
            name == "accuracy" ||
            name == "drag" ||
            name == "missle cooldown" ||
            name == "boost cooldown")
            {  
                temp_value = initial_value*2 - displayed_value; // 9 --> 11, 15 --> 5
            }
        
        // update the current value
        float scaler = temp_value / (float)initial_value; // normalize the scaler
        //Debug.Log("scaler: " + scaler);

        current_value = PlayerData.base_values[gameObject.name] * scaler;
    }
}
