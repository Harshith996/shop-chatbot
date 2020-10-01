using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showWebpage : MonoBehaviour
{
    // Start is called before the first frame update

    public string url = "http://www.google.com";
    IEnumerator Start()
    {
        using (WWW www = new WWW(url))
        {
            yield return www;
            Renderer renderer = GetComponent<Renderer>();
            renderer.material.mainTexture = www.texture;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
