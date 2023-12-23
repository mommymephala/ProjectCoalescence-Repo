using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "New ItemData", menuName = "Inventory/ItemData", order = 1)]

public class ItemData : ScriptableObject
{
    public ItemData itemData;
    public string typeName;

    public MeshRenderer MeshRenderer;
   //Mesh ile ilgili şeyleri buraya yazmak lazım
    // Start is called before the first frame update
    void Start()
    
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
