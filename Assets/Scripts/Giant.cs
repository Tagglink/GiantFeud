using UnityEngine;
using System.Collections;

public class Giant : MonoBehaviour {

    public Item currentWeapon;
    public Stats stats;
    public Stats[] buffs;

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
