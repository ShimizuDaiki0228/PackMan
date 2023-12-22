using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(PelletFiller))]
[CanEditMultipleObjects]
public class PelletFillerEditor : Editor
{

    GameObject myPrefab;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PelletFiller filler = (PelletFiller)target;

        myPrefab = filler.PelletPrefab;

        if(GUILayout.Button("Fill Field"))
        {
            //filler.FillField();

            if (filler.Active)
                return;

            else
            {
                filler.Active = true;
                filler.HCells = (int)Vector3.Distance(new Vector3(filler.TopRight.transform.position.x, 0, 0),
                                                new Vector3(filler.BottomLeft.transform.position.x, 0, 0));

                filler.VCells = (int)Vector3.Distance(new Vector3(0, 0, filler.TopRight.transform.position.z),
                                                new Vector3(0, 0, filler.BottomLeft.transform.position.z));

                for (int i = 0; i < filler.HCells; i++)
                {
                    for (int j = 0; j < filler.VCells; j++)
                    {
                        if (!Physics.CheckSphere(new Vector3(filler.BottomLeft.transform.position.x + i, filler.BottomLeft.transform.position.y, filler.BottomLeft.transform.position.z + j), 0.4f, filler.Unwalkable))
                        {
                            //GameObject pellet = Instantiate(PelletPrefab,
                            //                                new Vector3(filler.BottomLeft.transform.position.x + i, filler.BottomLeft.transform.position.y, filler.BottomLeft.transform.position.z + j),
                            //                                Quaternion.identity,
                            //                                _pelletHolder);

                            GameObject pellet = PrefabUtility.InstantiatePrefab(myPrefab) as GameObject;
                            pellet.transform.position = new Vector3(filler.BottomLeft.transform.position.x + i, filler.BottomLeft.transform.position.y, filler.BottomLeft.transform.position.z + j);
                            pellet.transform.parent = filler.PelletHolder;
                        }
                    }
                }
                filler.Active = false;
            }
        }
    }
}
