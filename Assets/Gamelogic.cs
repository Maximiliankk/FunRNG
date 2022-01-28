using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Gamelogic : MonoBehaviour
{
    public float spinStrength;
    public GameObject spinner;
    public GameObject divider;
    public GameObject textParent;
    public GameObject ball;
    bool happensOnce = true;
    bool spin = false;
    public int divides = 1;

    // Start is called before the first frame update
    void Start()
    {
        StreamReader sr = new StreamReader("Assets/mylist.txt");

        string line;
        // Read and display lines from the file until the end of
        // the file is reached.
        List<string> thelist = new List<string>();
        while ((line = sr.ReadLine()) != null)
        {
            Debug.Log(line);
            thelist.Add(line);
        }

        // shuffle the array
        for (int i = thelist.Count - 1; i > 0; i--)
        {
            var j = UnityEngine.Random.Range(0, i);

            var temp = thelist[i];
            thelist[i] = thelist[j];
            thelist[j] = temp;
        }

        divides = thelist.Count;
        int dcount = divides - 1;
        float rotateAmount = 360.0f / (float)divides;
        for (int i = 1; i <= dcount; ++i)
        {
            var go = GameObject.Instantiate(divider, spinner.transform);
            go.transform.Rotate(0, rotateAmount * i, 0);
            
            var go2 = GameObject.Instantiate(textParent, spinner.transform);
            go2.transform.Rotate(0, rotateAmount * ((float)i + 0.5f), 0);
            go2.GetComponentInChildren<UnityEngine.UI.Text>().text = thelist[i-1];
        }
        var go3 = GameObject.Instantiate(textParent, spinner.transform);
        go3.transform.Rotate(0, rotateAmount * ((float)divides + 0.5f), 0);
        go3.GetComponentInChildren<UnityEngine.UI.Text>().text = thelist[divides-1];

        // set ball x and z to zero
        ball.transform.position = new Vector3(0, ball.transform.position.y, 0);

        textParent.SetActive(false);

        ball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    private void FixedUpdate()
    {
        if(spin && happensOnce)
        {
            bool ccw = false;
            if (UnityEngine.Random.Range(0, 2) == 1)
            {
                ccw = true;
            }

            spinner.GetComponent<Rigidbody>().AddTorque((ccw ? Vector3.up : Vector3.down) * spinStrength);
            happensOnce = !happensOnce;
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void Spin()
    {
        ball.transform.position = new Vector3(UnityEngine.Random.Range(-2f,2f), ball.transform.position.y, UnityEngine.Random.Range(-2f, 2f));

        ball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        spin = true;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
