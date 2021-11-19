using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyColourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Working!");
        myRenderer = plane.GetComponent<MeshRenderer>();
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    Debug.Log("Working!");
    //}
    public Material materialBlue;
    public Material materialYellow;
    public Material materialBlack;
    public Material materialPurple;
    
    public GameObject plane;
    private MeshRenderer myRenderer;

    public void OnClick()
    {
        Debug.Log("Click!");
        
        myRenderer.enabled = !myRenderer.enabled;
        Debug.Log(myRenderer.enabled);
    }

    public void ColourBlack()
    {
        ChangeColour(ref materialBlack);
    }

    public void ColourYellow()
    {
        ChangeColour(ref materialYellow);
    }

    public void ColourPurple()
    {
        ChangeColour(ref materialPurple);
    }
    public void ColourBlue()
    {
        ChangeColour(ref materialBlue);
    }

    public void ChangeColour(ref Material colour)
    {
        myRenderer.material = colour;
    }

}
