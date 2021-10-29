using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public static class SceneToData
{

    // Start is called before the first frame update
    [MenuItem("Assets/Create/SaveScene")]
    public static void SaveSceneData()
    {
        ScriptObjStreet thisStreet = ScriptableObject.CreateInstance<ScriptObjStreet>();

        GameObject ground = GameObject.Find("Ground");
        var groundTrans = ground.GetComponent<Transform>();
        thisStreet.Length = (int) groundTrans.localScale.x ;
        thisStreet.Width = (int) groundTrans.localScale.y;

        if (thisStreet.Width == 1){
            thisStreet.xOriented = true;
        } else {
            thisStreet.xOriented = false;
        }

        var groundMesh = ground.GetComponent<MeshRenderer>();
        thisStreet.Color = groundMesh.sharedMaterial;

        GameObject objPar = GameObject.Find("ObjectParent");
        thisStreet.objects = new streetObj[objPar.transform.childCount];
       
        for (int i = 0; i < objPar.transform.childCount; i++){
            streetObj obj = new streetObj();
            //Debug.Log(PrefabUtility.GetCorrespondingObjectFromSource(objPar.transform.GetChild(i).gameObject));
            obj.myPrefab = PrefabUtility.GetCorrespondingObjectFromSource(objPar.transform.GetChild(i).gameObject);
            obj.streetPos = objPar.transform.GetChild(i).GetComponent<Transform>().localPosition;
            thisStreet.objects[i] = obj;
        }

        AssetDatabase.DeleteAsset("Assets/Street5.asset");
        AssetDatabase.CreateAsset(thisStreet, "Assets/Street5.asset");
        AssetDatabase.SaveAssets();

    }
}
