using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public GameObject spinner;
    public Gamelogic logic;

    public List<AudioClip> clips;
    int sfxIndex;

    // Start is called before the first frame update
    void Start()
    {
        sfxIndex = UnityEngine.Random.Range(0, clips.Count);
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
            AudioSource.PlayClipAtPoint(clips[sfxIndex], this.transform.position);
        }
    }
}
