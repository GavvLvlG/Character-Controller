using UnityEngine;

public class DamageEnemies : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null){
            damageable.TakeDamage(5);
            Debug.Log(damageable.GetHealth());
        }
    }
}
