using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PelletFiller : MonoBehaviour
{

    public int HCells;
    public int VCells;

    public GameObject BottomLeft;
    public GameObject TopRight;

    public GameObject PelletPrefab;
    public Transform PelletHolder;

    public bool Active;

    private void Start()
    {
        FillField();
    }

    public void FillField()
    {
        if(Active)
            return;

        else
        {
            Active = true;
            HCells = (int)Vector3.Distance(new Vector3(TopRight.transform.position.x, 0, 0),
                                            new Vector3(BottomLeft.transform.position.x, 0, 0));

            VCells = (int)Vector3.Distance(new Vector3(0, 0, TopRight.transform.position.z),
                                            new Vector3(0, 0, BottomLeft.transform.position.z));

            for(int i = 0; i < HCells; i++)
            {
                for(int j = 0; j < VCells; j++)
                {
                    if (!Physics.CheckSphere(new Vector3(BottomLeft.transform.position.x + i, BottomLeft.transform.position.y, BottomLeft.transform.position.z + j), 0.4f))
                    {
                        GameObject pellet = Instantiate(PelletPrefab) as GameObject;
                        pellet.transform.position = new Vector3(BottomLeft.transform.position.x + i, BottomLeft.transform.position.y, BottomLeft.transform.position.z + j);
                        pellet.transform.parent = PelletHolder;

                    }
                }
            }
            Active = false;
        }
    }
}
