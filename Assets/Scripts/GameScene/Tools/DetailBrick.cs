using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DetailBrick : MonoBehaviour
{
    [SerializeField] private bool isElastic;

    public bool GetIsElastic()
    {
        return isElastic;
    }

}
