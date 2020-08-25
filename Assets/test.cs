using Extensions.Properties;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {
	[LimitedRange]
  public LimitedRange limitedRange;
	[ReadOnly]
	public string read = "Read Only";
	[EnumFlags]
  public En enums = En.First;
  [LimitedVector2]
  public LimitedVector2 vector2;
	[LimitedVector3]
	public LimitedVector3 vector3;
	[AssetPath]
	public string asset;
	public bool on;
	[Visibility("on", true, false)]
	public int count;


	public enum En {
		First,
		Second,
		Third
	}


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
