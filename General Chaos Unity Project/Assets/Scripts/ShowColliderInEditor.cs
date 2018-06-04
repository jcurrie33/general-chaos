using UnityEngine;
using System.Collections;

public class ShowColliderInEditor : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

#if UNITY_EDITOR
	
	void OnDrawGizmos (){

		PolygonCollider2D collider = this.GetComponent<PolygonCollider2D> ();
		DebugDrawPolygon(collider.points, new Color(0.7f, 1, 0.7f));

	}
	
	//helper debug function
	void DebugDrawPolygon(Vector2[] points, Color color){
		for (int i = 0; i < points.Length; i++)
			Debug.DrawLine(points[i], points[(i + 1) % points.Length], color);
	}
	
#endif

}
