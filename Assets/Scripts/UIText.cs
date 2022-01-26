using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIText : MonoBehaviour
{
    private TextMeshProUGUI Text;
    public Transform capsuleTransform;
    public Vector3 forceVector = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        Text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        Text.SetText("Position: " + capsuleTransform.position.ToString()+ "\n"
                                                                           +"Rotation: " + capsuleTransform.rotation.ToString()
                                                                           + "\n"
                                                                           + forceVector.x.ToString() + "\n"
                                                                           + forceVector.y.ToString() + "\n"
                                                                           + forceVector.z.ToString());
        
        
    }
}
