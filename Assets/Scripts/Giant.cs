using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Giant : MonoBehaviour {

    public Item currentWeapon;
    public Stats stats;
    public Stats[] buffs;

    public GameObject healthDisplay;
    public GameObject enemyGiant;

    private int timer;
    private int atkTime;

    void Attack()
    {
        int buffDamage = 0;
        if (buffs != null) {
            foreach (Stats s in buffs)
            {
                buffDamage += s.atk;
            }
        }
        int damageDealt = stats.atk + buffDamage;
        enemyGiant.GetComponent<Giant>().TakeDamage(damageDealt);
    }

    public void TakeDamage(int damageTaken)
    {
        stats.hp -= damageTaken - stats.def;
    }

    public void UseItem(Item item)
    {

    }

    void Start()
    {
        stats = new Stats();
        stats.atk = 10;
        stats.atkspd = 1f;
        stats.maxHP = 1000;
        stats.hp = 1000;
        stats.def = 0;
    }

    void Update()
    {
        
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
