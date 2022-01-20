using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIText : MonoBehaviour
{
    private TextMeshProUGUI Text;
    public Transform capsuleTransform;

    // Start is called before the first frame update
    void Start()
    {
        Text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        Text.SetText("Position: " + capsuleTransform.position.ToString()+ "\n"
                                                                           +"Rotation: " + capsuleTransform.rotation.ToString());
        
        
    }
}
