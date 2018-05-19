using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollisionScript : MonoBehaviour {

    
    int fireballDamage = 50;
    public float maxSecondsAlive = 5f;
    float secondsLifeRemaining;

    void Start()
    {
        secondsLifeRemaining = maxSecondsAlive;
    }

    void Update()
    {
        secondsLifeRemaining -= Time.deltaTime;
        if(secondsLifeRemaining <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    

	void OnCollisionEnter(Collision col)
    {
        //if the game object attached to the collision variable is any type of enemy
        // we get that objects stats and then damage it
        EnemyStats enemyStats = col.gameObject.GetComponent<EnemyStats>();
        if (enemyStats != null)
        {
            Debug.Log("Fireball hit and damaged " + col.gameObject.name);
            enemyStats.TakeDamage(fireballDamage);
         
        }
        Destroy(this.gameObject);
    }
}
