using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetManager : MonoBehaviour
{
    public ScriptObjStreet myStreet;
    public renderedStreet onStreet;
    public renderedStreet wrapStreet;
    public renderedStreet interStreet;
    public renderedStreet interStreetWrap;

    // Start is called before the first frame update
    void Start()
    {
        onStreet = new renderedStreet(myStreet, new Vector3(0, 0, 0), false);
        wrapStreet = null;
        Debug.Log(onStreet.myIntersections);
    }

    // Update is called once per frame
    void Update()
    {
        onStreet.parent.name = "onStreet";
        if (interStreet != null){
            interStreet.parent.name = "interStreet";
        }

        var playerTrans = this.GetComponent<Transform>();
        var relPos = playerTrans.position - onStreet.truePos;  

        // WRAP THE CURRENT STREET
        this.wraparoundLogic(25.0f, ref this.onStreet, ref this.wrapStreet);
        if (myStreet.xOriented) {
            if (Mathf.Abs(relPos.x) > myStreet.Length * 5.0f){
                var temp = wrapStreet;
                wrapStreet = onStreet;
                onStreet = temp;
            }
        } else {
            if (Mathf.Abs(relPos.z) > myStreet.Width * 5.0f){
                var temp = wrapStreet;
                wrapStreet = onStreet;
                onStreet = temp;
            }
        }

        var otherStreetMaybe = onStreet.onIntersection(relPos);
        if(interStreet != null && otherStreetMaybe == null){
            Debug.Log("Moved off intersection");
            int id = onStreet.getOtherIntersectionId(interStreet.streetInfo);
            interStreet.downSize(id);
            interStreetWrap.destroyStreet();
            this.interStreetWrap = null;
        }
        
        interStreet = otherStreetMaybe;
        if (interStreet != null){
            if (!interStreet.fullyRendered){
                int id = onStreet.getOtherIntersectionId(interStreet.streetInfo);
                interStreet.fullRender(id);
            } else {
                this.wraparoundLogic(25.0f, ref this.interStreet, ref this.interStreetWrap);
            }
            
            if (myStreet.xOriented){
                if (Mathf.Abs(relPos.z) > myStreet.Width * 5){
                    myStreet = interStreet.streetInfo;
                    var pos = interStreet.truePos;
                    onStreet.destroyStreet();
                    if (wrapStreet != null){
                        wrapStreet.destroyStreet();
                        wrapStreet = null;
                    }
                    interStreet.destroyStreet();
                    interStreet = null;
                    if (interStreetWrap != null){
                        interStreetWrap.destroyStreet();
                        interStreetWrap = null;
                    }
                    onStreet = new renderedStreet(myStreet, pos, false);
                }
            } else {
                if (Mathf.Abs(relPos.x) > myStreet.Length * 5){
                    myStreet = interStreet.streetInfo;
                    var pos = interStreet.truePos;
                    onStreet.destroyStreet();
                    if (wrapStreet != null){
                        wrapStreet.destroyStreet();
                        wrapStreet = null;
                    }
                    interStreet.destroyStreet();
                    interStreet = null;
                    if (interStreetWrap != null){
                        interStreetWrap.destroyStreet();
                        interStreetWrap = null;
                    }
                    onStreet = new renderedStreet(myStreet, pos, false);
                }
            }
        }

    }

    // BIG NOTE: WRAPAROUND LOGIC DOES NOT WORK IF THE STREET IS <= RENDER DISTANCE SIZE
    void wraparoundLogic(float distance, ref renderedStreet street, ref renderedStreet wrapStreet){
        var myInfo = street.streetInfo;
        var playerTrans = this.GetComponent<Transform>();
        var relPos = playerTrans.position - street.truePos; 
        if (street.atEdge(25.0f, relPos))
        {
            if (wrapStreet is null){
                Vector3 wrapPos;
                float direction;
                if (myInfo.xOriented){
                    direction = Mathf.Sign(relPos.x);
                    wrapPos = street.truePos + new Vector3(myInfo.Length * 10.0f * direction, 0.0f,0.0f);
                    wrapStreet = new renderedStreet(myInfo, wrapPos, false);
                    wrapStreet.parent.name = "Wraparound";
                } else {
                    direction = Mathf.Sign(relPos.z);
                    wrapPos = street.truePos + new Vector3(0.0f, 0.0f,myInfo.Width * 10.0f * direction);
                    wrapStreet = new renderedStreet(myInfo, wrapPos, false);
                    wrapStreet.parent.name = "Wraparound";
                }
                
            }
        } 
        else if (!(wrapStreet is null))
        {
            wrapStreet.destroyStreet();
            wrapStreet = null;
        }
        
    }
}

public class renderedStreet {

    public ScriptObjStreet streetInfo;
    public Vector3 truePos;
    public GameObject parent;
    public bool fullyRendered;

    public renderedStreet[] myIntersections;

    public GameObject[] myObjects;

    GameObject ground;

    public renderedStreet(ScriptObjStreet street, Vector3 pos, bool intersection, int interIndex = -1){
        streetInfo = street;
        truePos = pos;
        parent = new GameObject();
        parent.name = "Street";
        var parentTrans = parent.GetComponent<Transform>();
        parentTrans.position = pos;

        ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.transform.parent = parent.transform;
        var groundRenderer = ground.GetComponent<MeshRenderer>();
        groundRenderer.material = street.Color;

        if (intersection){
            this.renderIntersection(interIndex);
        } else {
            this.fullRender(interIndex);
        }        
    }

    void renderIntersection(int i){
        parent.name = "Small intersection";
        var groundTransform = ground.GetComponent<Transform>();
        groundTransform.localPosition = streetInfo.intersections[i].position;
        if (streetInfo.xOriented){
            groundTransform.localScale = new Vector3(2.0f, 1.0f, streetInfo.Width);
        } else {
            groundTransform.localScale = new Vector3(streetInfo.Length, 1.0f, 2.0f);
        }

        myIntersections = new renderedStreet[0];

        fullyRendered = false;
    }

    public void fullRender(int id){
        var groundTransform = ground.GetComponent<Transform>();
        groundTransform.localPosition = new Vector3(0.0f,0.0f,0.0f);
        groundTransform.localScale = new Vector3(streetInfo.Length, 1, streetInfo.Width);

        fullyRendered = true;

        myIntersections = new renderedStreet[streetInfo.intersections.Length];
        
        for(int j = 0; j < streetInfo.intersections.Length; j++){
            var inter = streetInfo.intersections[j];
            Vector3 pos = truePos + inter.position - inter.otherPosition;
            var otherStreet = inter.other;
            int index = this.getOtherIntersectionId(otherStreet);
            
            if (j != id){
                var renderedInter = new renderedStreet(otherStreet, pos, true, index);
                Debug.Log(streetInfo);
                Debug.Log(j);
                Debug.Log(myIntersections.Length);
                myIntersections[j] = renderedInter;
            } else {
                myIntersections[j] = null;
            }
        }

        myObjects = new GameObject[streetInfo.objects.Length];
        for (int i = 0; i < streetInfo.objects.Length; i++){
            streetObj info = streetInfo.objects[i];
            myObjects[i] = GameObject.Instantiate(info.myPrefab);
            myObjects[i].transform.parent = parent.transform;

            var objTransform = myObjects[i].GetComponent<Transform>();
            objTransform.localPosition = info.streetPos;
        }
    }

    public bool atEdge(float distance, Vector3 playerRelPos){
        bool answer = false;
        if (streetInfo.xOriented){
            answer = Mathf.Abs(playerRelPos.x) + distance > streetInfo.Length * 5.0f;
        } else {
            answer = Mathf.Abs(playerRelPos.z) + distance > streetInfo.Width * 5.0f;
        }
        return answer;
    }

    public void destroyStreet(){
        GameObject.Destroy(ground);
        GameObject.Destroy(parent);
        for (int i = 0; i < myIntersections.Length; i++){
            var inter = myIntersections[i];
            if (inter != null){
                inter.destroyStreet();
            }
        }
    }

    public void downSize(int j){
        parent.name = "Small Intersection";
        for (int i = 0; i < myIntersections.Length; i++){
            var inter = myIntersections[i];
            if (inter != null){
                inter.destroyStreet();
            }
        }
        var groundTransform = ground.GetComponent<Transform>();
        groundTransform.localPosition = streetInfo.intersections[j].position;
        if (streetInfo.xOriented){
            groundTransform.localScale = new Vector3(2.0f, 1.0f, streetInfo.Width);
        } else {
            groundTransform.localScale = new Vector3(streetInfo.Length, 1.0f, 2.0f);
        }

        myIntersections = new renderedStreet[0];

        fullyRendered = false;
    }

    public renderedStreet onIntersection(Vector3 playerRelPos){
        int index = -1;
        for (int i = 0; i < streetInfo.intersections.Length; i++){
            Intersection inter = streetInfo.intersections[i];
            if (streetInfo.xOriented){
                if (Mathf.Abs(inter.position.x - playerRelPos.x) <= 5.0f){
                    index = i;
                }
            } else {
                if (Mathf.Abs(inter.position.z - playerRelPos.z) <= 5.0f){
                    index = i;
                }
            }
        }
        if (index != -1){
            return myIntersections[index];
        } else {
            return null;
        }
        
    }

    public int getOtherIntersectionId(ScriptObjStreet otherStreet){
        int index = -1;
        for (int i = 0; i < otherStreet.intersections.Length; i++){
            var maybeThis = otherStreet.intersections[i].other;
            if (maybeThis == streetInfo){
                index = i;
                break;
            }
        }
        return index;
    }
}