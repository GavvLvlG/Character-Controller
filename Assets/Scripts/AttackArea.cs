using UnityEngine;

public class AttackArea : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy"){
            GetComponentInParent<PlayerMovement>().enemyList.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Enemy"){
            GetComponentInParent<PlayerMovement>().enemyList.Remove(other.gameObject);
        }
    }
}
