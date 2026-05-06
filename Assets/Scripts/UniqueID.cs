using System;
using UnityEngine;

public class UniqueID : MonoBehaviour
{

    [SerializeField] string uID = Guid.NewGuid().ToString();

    public string Get_uID { get { return uID; } }

}
