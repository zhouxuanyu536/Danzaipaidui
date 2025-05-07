using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
public enum SpeedBlockType
{
    Up, Down
}
public enum SpeedBlockDirection
{
    Left, Right,Forward,Backward
}
[ExecuteInEditMode]
public class SpeedBlock : MonoBehaviour
{
    [SerializeField] private SpeedBlockType type;
    [SerializeField] private SpeedBlockDirection direction;

    private Material material;
    private void Start()
    {
        material = GetComponent<Renderer>().sharedMaterials.FirstOrDefault();
        GetTypeAndDirection(material.name);
    }
    private void OnEnable()
    {
        material = GetComponent<Renderer>().sharedMaterials.FirstOrDefault();
        GetTypeAndDirection(material.name);
    }
    private void OnValidate()
    {
        material = GetComponent<Renderer>().sharedMaterials.FirstOrDefault();
        if(material != null)
        {
            GetTypeAndDirection(material.name);
        }
    }
    private void GetTypeAndDirection(string materialName)
    {
        string pattern = @"Speed(.*?)Material(.*?)\(.*";
        Regex regex = new Regex(pattern, RegexOptions.Compiled);
        Match match = regex.Match(materialName);
        if (match.Success)
        {
            bool result = true;
            result &= Enum.TryParse(typeof(SpeedBlockType), match.Groups[1].Value, out object result1);
            result &= Enum.TryParse(typeof(SpeedBlockDirection), match.Groups[2].Value, out object result2);
            
            if(result)
            {
                type = (SpeedBlockType)result1;
                direction = (SpeedBlockDirection)result2;
            }
            else
            {
            }
        }
        else
        {
        }
    }
    public SpeedBlockType GetSpeedBlockType()
    {
        return type;
    }
    public SpeedBlockDirection GetSpeedBlockDirection()
    {
        return direction;
    }
    public void SetSpeedBlockType(SpeedBlockType type)
    {
        this.type = type;
    }
    public void SetSpeedBlockDirection(SpeedBlockDirection direction)
    {
        this.direction = direction;
    }
    public Vector3 GetOnSpeedBlockSpeed(PlayerController controller,float speed,float SpeedMultiplier)
    {
        Vector3 Direction = Vector3.zero;
        Vector3 OnSpeedBlockSpeed;
        if (GetSpeedBlockType() == SpeedBlockType.Up)
        {
            switch (GetSpeedBlockDirection())
            {
                case SpeedBlockDirection.Left:
                    OnSpeedBlockSpeed = new Vector3(-speed, 0, 0) * SpeedMultiplier;
                    break;
                case SpeedBlockDirection.Right:
                    OnSpeedBlockSpeed = new Vector3(speed, 0, 0) * SpeedMultiplier;
                    break;
                case SpeedBlockDirection.Forward:
                    OnSpeedBlockSpeed = new Vector3(0, 0, speed) * SpeedMultiplier;
                    break;
                case SpeedBlockDirection.Backward:
                    OnSpeedBlockSpeed = new Vector3(0, 0, -speed) * SpeedMultiplier;
                    break;
                default:
                    OnSpeedBlockSpeed = new Vector3(-speed, 0, 0) * SpeedMultiplier;
                    break;
            }
        }
        else
        {
            switch (GetSpeedBlockDirection())
            {
                case SpeedBlockDirection.Left:
                    OnSpeedBlockSpeed = new Vector3(speed, 0, 0) * SpeedMultiplier;
                    break;
                case SpeedBlockDirection.Right:
                    OnSpeedBlockSpeed = new Vector3(-speed, 0, 0) * SpeedMultiplier;
                    break;
                case SpeedBlockDirection.Forward:
                    OnSpeedBlockSpeed = new Vector3(0, 0, -speed) * SpeedMultiplier;
                    break;
                case SpeedBlockDirection.Backward:
                    OnSpeedBlockSpeed = new Vector3(0, 0, speed) * SpeedMultiplier;
                    break;
                default:
                    OnSpeedBlockSpeed = new Vector3(speed, 0, 0) * SpeedMultiplier;
                    break;
            }
        }
        return OnSpeedBlockSpeed;
    }
  
}
