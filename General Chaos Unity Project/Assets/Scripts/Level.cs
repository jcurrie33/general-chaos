using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class Level : MonoBehaviour {

	public enum LevelMap {
		LavelMap_City = 0,
		LevelMap_City_Mirrored,
		LevelMap_Desert,
		LevelMap_Desert_Mirrored
	};

	public enum LevelSpawnLocation {
		LevelSpawnLocation_TopLeft = 0,
		LevelSpawnLocation_TopCenter,
		LevelSpawnLocation_TopRight,
		LevelSpawnLocation_MiddleLeft,
		LevelSpawnLocation_MiddleCenter,
		LevelSpawnLocation_MiddleRight,
		LevelSpawnLocation_BottomLeft,
		LevelSpawnLocation_BottomCenter,
		LevelSpawnLocation_BottomRight
	};

	public LevelMap levelMap;
	public LevelSpawnLocation playerSpawnLocation;
	public LevelSpawnLocation AISpawnLocation;
	public List<Unit> obstaclePlacementList;

	public GameObject obstacle1;
	public Vector3 obstacle1Position;
	public GameObject obstacle2;
	public Vector3 obstacle2Position;
	public GameObject obstacle3;
	public Vector3 obstacle3Position;
	public GameObject obstacle4;
	public Vector3 obstacle4Position;
	public GameObject obstacle5;
	public Vector3 obstacle5Position;
	public GameObject obstacle6;
	public Vector3 obstacle6Position;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
