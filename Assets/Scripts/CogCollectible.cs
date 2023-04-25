using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CogCollectible : MonoBehaviour
{
    public AudioClip collectedClip;
    RubyController controller;

    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();
        if (GameObject.FindWithTag("Cog"))
        {
            if (controller != null)
            {
                controller.cogs += 4;
                controller.SetCogText();
                Destroy(gameObject);
                controller.PlaySound(collectedClip);
            }
        }
    }
}
