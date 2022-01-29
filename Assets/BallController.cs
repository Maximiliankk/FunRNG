using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public GameObject spinner;
    public Gamelogic logic;

    public List<AudioClip> clips;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if(collision.collider.gameObject == spinner)
        {
            // play a sound effect
            AudioSource.PlayClipAtPoint(clips[UnityEngine.Random.Range(0, clips.Count)], this.transform.position);
        }
    }
}
