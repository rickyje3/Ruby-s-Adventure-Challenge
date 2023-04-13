using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2d;

    // awake is called immediately when the object is created
    void Awake()
    {
        //makes sure the rigidbody functions on startup
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //magnitude is length of the vector, measures the distance of the projectile and if it goes over 100 units destroy it. 
        if (transform.position.magnitude > 100.0f)
        {
            Destroy(gameObject);
        }
    }

    //Moves the rigidbody, the higher the force the faster it goes
    public void Launch(Vector2 direction, float force)
    {
        //calls addforce on the rigidbody and links the force to a direction.
        rigidbody2d.AddForce(direction * force);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        //projectile will fix the robot who will stop moving and will not deal damage
        EnemyController e = other.collider.GetComponent<EnemyController>();
        if (e != null)
        {
            e.Fix();
        }

        HardEnemyController h = other.collider.GetComponent<HardEnemyController>();
        if (h != null)
        {
            h.Fix();
        }

        Destroy(gameObject);
    }
}
