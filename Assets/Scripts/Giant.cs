using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Giant : MonoBehaviour {

    public Item currentWeapon;
    public Stats stats;
    public Stats[] buffs;

    public GameObject healthDisplay;
    
    private int timer;
    private int atkTime;

    void Attack()
    {
        Debug.Log("Giant attacked at: " + Time.time);
    }

    public void TakeDamage()
    {

    }

    public void UseItem(Item item)
    {

    }

    void Start()
    {
        stats = new Stats();
        stats.atkspd = 0.5f;
        stats.maxHP = 1000;
        stats.hp = 1000;
    }

    void Update()
    {
        healthDisplay.GetComponent<Text>().text = stats.hp + " / " + stats.maxHP;
    }

    void FixedUpdate()
    {
        timer++;
        atkTime = Mathf.RoundToInt(50 / stats.atkspd);
        if (timer % atkTime == 0)
        {
            Attack();
        }
    }
}
