using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class Gamelogic : MonoBehaviour
{
    public float spinStrength;
    public GameObject spinner;
    public GameObject divider;
    public GameObject textParent;
    public GameObject ball;
    List<GameObject> balls = new List<GameObject>();
    public GameObject spinButtonBkg, spinButton;
    public GameObject quitButtonBkg, quitButton;
    public GameObject resetButtonBkg, resetButton;
    public GameObject togglesParent;
    public GameObject togglePrefab;
    public GameObject ballCountRoot;
    public UnityEngine.UI.Slider ballCountSlider;
    public UnityEngine.UI.Text ballCountText;
    public GameObject column, ground;
    public GameObject sunlight;
    public UnityEngine.UI.Dropdown dropdown;
    //public UnityEngine.UI.Dropdown SpeedDropdown;
    int divides = 2;
    Vector3 ballStartPos;
    List<string> thelist = new List<string>();
    List<GameObject> togObjs = new List<GameObject>();
    public List<AudioClip> clips;
    Vector3 camStartDir, camStartPos;
    float zoomAmount = 0, camLowerLimit, camUpperLimit;

    // Start is called before the first frame update
    void Start()
    {
        camStartDir = Camera.main.transform.forward;
        camStartPos = Camera.main.transform.position;
        camLowerLimit = (7f - camStartPos.y) / camStartDir.y;
        camUpperLimit = (25f - camStartPos.y) / camStartDir.y;

        SliderUpdate();

        string path = "Assets/lists";
        var files = Directory.GetFiles(path);
        List<string> flist = new List<string>();
        foreach (var f in files)
        {
            if (f.Substring(f.Length - 5, 5) != ".meta")
            {
                flist.Add(f.Substring(path.Length + 1, f.Length - (path.Length + 1)));
            }
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(flist);

        UpdateDropdown();

        ResetGame();
    }

    public void UpdateDropdown()
    {
        foreach(var o in togObjs)
        {
            Destroy(o);
        }
        thelist.Clear();
        togObjs.Clear();

        StreamReader sr = new StreamReader("Assets/lists/" + dropdown.options[dropdown.value].text);
        string line;
        // Read and display lines from the file until the end of
        // the file is reached.
        while ((line = sr.ReadLine()) != null)
        {
            //Debug.Log(line);
            thelist.Add(line);
            var go = GameObject.Instantiate(togglePrefab, togglesParent.transform);
            togObjs.Add(go);
            var rt = go.GetComponent<RectTransform>();
            rt.position = new Vector3(rt.position.x, rt.position.y - thelist.Count * rt.rect.height, rt.position.z);
            var ttxt = go.GetComponentInChildren<UnityEngine.UI.Text>().text = line;
            var tog = go.GetComponentInChildren<UnityEngine.UI.Toggle>();
            tog.onValueChanged.AddListener(UpdateSpinner);
        }
        sr.Close();

        StartCoroutine(DelayUpdateSpinner());
    }

    IEnumerator DelayUpdateSpinner()
    {
        yield return new WaitForEndOfFrame();
        UpdateSpinner();
    }

    private void UpdateSpinner(bool val = false)
    {
        List<string> findOns = new List<string>();
        foreach(var t in togglesParent.GetComponentsInChildren<UnityEngine.UI.Toggle>())
        {
            if(t.isOn == true)
            {
                findOns.Add(t.gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text);
            }
        }

        // shuffle the array
        {
            for (int i = findOns.Count - 1; i > 0; i--)
            {
                var j = UnityEngine.Random.Range(0, i);

                var temp = findOns[i];
                findOns[i] = findOns[j];
                findOns[j] = temp;
            }
        }

        // update the spinner
        {
            foreach(var c in spinner.GetComponentsInChildren<Transform>())
            {
                if(c.transform != spinner.transform)
                {
                    Destroy(c.gameObject);
                }
            }

            divides = findOns.Count;
            int dcount = divides - 1;
            float rotateAmount = 360.0f / (float)divides;

            var go4 = GameObject.Instantiate(divider, spinner.transform);

            for (int i = 1; i <= dcount; ++i)
            {
                var go = GameObject.Instantiate(divider, spinner.transform);
                go.transform.Rotate(0, rotateAmount * i, 0);

                var go2 = GameObject.Instantiate(textParent, spinner.transform);
                go2.transform.Rotate(0, rotateAmount * ((float)i + 0.5f), 0);
                go2.GetComponentInChildren<UnityEngine.UI.Text>().text = findOns[i - 1];

                // if the length of the string is greater than 21, then we want to make the font size smaller
                go2.GetComponentInChildren<UnityEngine.UI.Text>().fontSize = (findOns[i - 1].Length > 20) ? 50 : 74;
            }
            var go3 = GameObject.Instantiate(textParent, spinner.transform);
            go3.transform.Rotate(0, rotateAmount * ((float)divides + 0.5f), 0);
            go3.GetComponentInChildren<UnityEngine.UI.Text>().text = findOns[divides - 1];
        }
    }

    public void InitBalls()
    {
        foreach (var b in balls)
        {
            Destroy(b);
        }
        balls.Clear();
        for (int i = 0; i < (int)ballCountSlider.value; ++i)
        {
            balls.Add(GameObject.Instantiate(ball));
            balls[i].transform.position = new Vector3(balls[i].transform.position.x + i * 2, balls[i].transform.position.y, balls[i].transform.position.z);

            balls[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            balls[i].GetComponent<Renderer>().material.color = GetColor();
        }
    }

    private void InitSpin()
    {
        UpdateSpinner();

        InitBalls();

        spinButtonBkg.GetComponent<UnityEngine.UI.Image>().color = GetColor();
        quitButtonBkg.GetComponent<UnityEngine.UI.Image>().color = GetColor();
        resetButtonBkg.GetComponent<UnityEngine.UI.Image>().color = GetColor();
        spinner.GetComponent<Renderer>().material.color = GetColor();
        column.GetComponent<Renderer>().material.color = GetColor();
        ground.GetComponent<Renderer>().material.color = GetColor();
        ground.transform.position = new Vector3(ground.transform.position.x, UnityEngine.Random.Range(-4.0f, -60.0f), ground.transform.position.z);
        sunlight.transform.rotation = UnityEngine.Random.rotation;
        do {
            sunlight.transform.rotation = UnityEngine.Random.rotation;
        } while (sunlight.transform.forward.y > -0.5f);
    }

    private Color GetColor()
    {
        return new Color(UnityEngine.Random.Range(0.5f, 1.0f), UnityEngine.Random.Range(0.5f, 1.0f), UnityEngine.Random.Range(0.5f, 1.0f));
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void ResetGame()
    {
        spinButton.SetActive(true);
        spinButtonBkg.SetActive(true);
        resetButton.SetActive(false);
        resetButtonBkg.SetActive(false);
        ballCountRoot.SetActive(true);
        togglesParent.SetActive(true);
        dropdown.gameObject.SetActive(true);
        //SpeedDropdown.gameObject.SetActive(true);
        spinner.GetComponent<Rigidbody>().Sleep();
        InitSpin();
    }


    public void Spin()
    {
        StartCoroutine(SpinCR());
    }

    IEnumerator SpinCR()
    {
        AudioSource.PlayClipAtPoint(clips[0], Camera.main.transform.position, 1);

        dropdown.gameObject.SetActive(false);
        //SpeedDropdown.gameObject.SetActive(false);
        spinButton.SetActive(false);
        spinButtonBkg.SetActive(false);
        ballCountRoot.SetActive(false);
        togglesParent.SetActive(false);

        List<Vector3> startpos = new List<Vector3>();
        List<Vector3> targetpos = new List<Vector3>();
        List<float> tpmag = new List<float>();

        for (int i = 0; i < (int)ballCountSlider.value; ++i)
        {
            startpos.Add(ball.transform.position);
            targetpos.Add(Vector3.zero);
            tpmag.Add(float.PositiveInfinity);
        }

        for (int i = 0; i < (int)ballCountSlider.value; ++i)
        {
            do
            {
                targetpos[i] = new Vector3(UnityEngine.Random.Range(-9f, 9f), balls[i].transform.position.y, UnityEngine.Random.Range(-9f, 9f));
                tpmag[i] = Mathf.Sqrt(targetpos[i].x * targetpos[i].x + targetpos[i].z * targetpos[i].z);
            } while (tpmag[i] > 9.0f);
        }
        
        int frames = 50;
        for (int j = 0; j < frames; ++j)
        {
            for (int i = 0; i < (int)ballCountSlider.value; ++i)
            {
                balls[i].transform.position = startpos[i] + (targetpos[i] - startpos[i]) * ((float)j / (float)frames);
            }

            yield return new WaitForEndOfFrame();
        }
        for (int i = 0; i < (int)ballCountSlider.value; ++i)
        {
            balls[i].transform.position = targetpos[i];
        }


        bool ccw = false;
        if (UnityEngine.Random.Range(0, 2) == 1)
        {
            ccw = true;
        }

        float spinSpeedModifier = 2;
        //if(SpeedDropdown.value == 0)
        //{
        //    spinSpeedModifier = 0.5f;
        //}
        //else if (SpeedDropdown.value == 1)
        //{
        //    spinSpeedModifier = 0.75f;
        //}
        //else if (SpeedDropdown.value == 2)
        //{
        //    spinSpeedModifier = 1f;
        //}
        spinner.GetComponent<Rigidbody>().AddTorque((ccw ? Vector3.up : Vector3.down) * spinStrength * spinSpeedModifier);

        for (int i = 0; i < (int)ballCountSlider.value; ++i)
        {
            balls[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }

        resetButton.SetActive(true);
        resetButtonBkg.SetActive(true);
    }

    public void SliderUpdate()
    {
        ballCountText.text = ballCountSlider.value.ToString();
        InitBalls();
    }

    // Update is called once per frame
    void Update()
    {
        float toOrigin = Vector3.Distance(Camera.main.transform.position, Vector3.zero);
        float scrollspeed = toOrigin * 0.3f;
        zoomAmount += Input.mouseScrollDelta.y * scrollspeed;
        Camera.main.transform.position = camStartPos + camStartDir * zoomAmount;

        if (Camera.main.transform.position.y < 7)
        {
            Camera.main.transform.position = camStartPos + camStartDir * camLowerLimit;
            zoomAmount = camLowerLimit;
        }
        if (Camera.main.transform.position.y > 25)
        {
            Camera.main.transform.position = camStartPos + camStartDir * camUpperLimit;
            zoomAmount = camUpperLimit;
        }
    }
}
