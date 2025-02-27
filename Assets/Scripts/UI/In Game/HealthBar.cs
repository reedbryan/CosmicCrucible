using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Can be added to any gameobject with and damageInkate script and will act as its health bar.

/// <summary>
/// The HealthBar class will only work with a host game object that has an accessable "DamageIntake" script.
/// This will need to be assigned manualy in the editor.
/// </summary>
public class HealthBar : MonoBehaviour
{
    /* - - - - New and possibly more efficiant - - - - - - -*/

    public GameObject healthBarCompletePrefab;
    GameObject anchor; // Empty transform that the health bar canvas is attached to, to allow for movement
    Slider slider;
    DamageIntake damageIntake;

    [SerializeField] float yOffset;

    private void Start()
    {
        damageIntake = GetComponent<DamageIntake>();

        // Get anchor
        anchor = Instantiate(healthBarCompletePrefab);
        anchor.transform.parent = transform; // Set anchor to parent

        // Get slider (go down the list of children :0 intil you get to the slider's Game Object called "Slider")
        GameObject canvasGameOb = anchor.transform.GetChild(0).gameObject; // canvas Game Object
        GameObject sliderGameOb = canvasGameOb.transform.GetChild(0).gameObject; // Slider Game Object
        slider = sliderGameOb.GetComponent<Slider>();
    }

    private void Update()
    {
        // Slider update
        slider.maxValue = damageIntake.maxHP;
        slider.value = damageIntake.HP;

        // Rotation update (so it stays at the top of the player)
        anchor.transform.position = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
        anchor.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0f);
    }

    /* - - - - Old but still worksx - - - - - - /

    public GameObject host; // Game Ob this script is tracking health for
    public GameObject anchor; // Empty transform that the health bar is attached to
    DamageIntake hostDamageIntake;
    Slider slider;

    private void Awake()
    {
        hostDamageIntake = host.GetComponent<DamageIntake>();
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        // Slider update
        slider.maxValue = hostDamageIntake.maxHP;
        slider.value = hostDamageIntake.HP;
        Debug.Log(hostDamageIntake.maxHP);
        Debug.Log(hostDamageIntake.HP);

        // Rotation update (so it stays at the top of the player)
        //anchor.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, host.transform.eulerAngles.z * -1f);
        anchor.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0f);
    }

    /* - - - - - - - - - - -*/
}
