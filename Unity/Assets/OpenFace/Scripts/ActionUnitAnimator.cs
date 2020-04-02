using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq; // JSON reader https://assetstore.unity.com/packages/tools/input-management/json-net-for-unity-11347
using System;
using UnityEngine;

public class ActionUnitAnimator : MonoBehaviour {

    // Manuel Bastioni / MakeHuman model
    SkinnedMeshRenderer skinnedMeshRenderer;
    //   Mesh skinnedMesh;
    //   Dictionary<string, int> blendDict = new Dictionary<string, int> ();

    [SerializeField]
    public ActionUnit[] actionUnits;

    void Awake () {
        // get MB / MH model
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer> ();
        //     skinnedMesh = GetComponent<SkinnedMeshRenderer> ().sharedMesh;
    }

    // Use this for initialization
    void Start () {
        /*
        // create dict of all blendshapes this skinnedMesh has
        blendShapeCount = skinnedMesh.blendShapeCount;
        for (int i = 0; i < blendShapeCount; i++) {
            string expression = skinnedMesh.GetBlendShapeName (i);
            //Debug.Log(expression);
            blendDict.Add (expression, i);
        }
        */
    }

    // We use a JSON message to set the head rotation and facial expressions;
    public IEnumerator RequestBlendshapes (JObject blendJson) {
        foreach (KeyValuePair<string, JToken> pair in blendJson) {
            foreach (ActionUnit aU in actionUnits) {
                if (pair.Key == "AU" + aU.actionUnit.ToString ("00")) {
                    skinnedMeshRenderer.SetBlendShapeWeight (aU.blendShapeIndex, float.Parse (pair.Value.ToString ()) * 100f);
                }
            }
        }
        yield return null;
    }
}

[System.Serializable]
public class ActionUnit : ICloneable {

    [SerializeField]
    public int actionUnit = 1;
    [SerializeField]
    public int blendShapeIndex = 0;

    public object Clone () {
        return MemberwiseClone ();
    }
}