using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Laser : MonoBehaviourPun
{
    public Unit Target { get; set; }
    public int Damage { get; set; }
    public int TargetDefense { get; set; }
    public float Speed { get; set; }

    private void Awake()
    {
        Speed = 50f;
    }

    private void Start()
    {
        //Set laser colour according to owner
        SetMaterialColour();
    }

    private void Update()
    {
        if (photonView.Owner == PhotonNetwork.LocalPlayer)
        {
            if (Target)
            {
                transform.LookAt(Target.transform);
                transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, Speed * Time.deltaTime);

                if (transform.position == Target.transform.position)
                {
                    //Subtract attack roll from defense roll
                    int difference = TargetDefense - Damage;
                    //If attack outweighs the defense, then reduce health of attacked unit
                    if (difference < 0)
                    {
                        Target.SetAttacker(PhotonNetwork.LocalPlayer.ActorNumber);
                        Target.Health += difference;
                    }
                    PhotonNetwork.Destroy(gameObject);
                }
            }
            else
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    public void SetMaterialColour()
    {
        if (photonView.Owner.CustomProperties.ContainsKey("colour"))
        {
            int playerColour = (int)photonView.Owner.CustomProperties["colour"];

            //Set Material to Owner player's colour
            gameObject.GetComponent<Renderer>().material.color = GameManager.ColourList[playerColour];
        }
    }

}