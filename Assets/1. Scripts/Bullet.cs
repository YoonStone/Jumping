using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int acotrNum;

    void Start()
    {
        GetComponent<Rigidbody2D>().AddForce(transform.right * 5, ForceMode2D.Impulse);
        Destroy(gameObject, 3f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
