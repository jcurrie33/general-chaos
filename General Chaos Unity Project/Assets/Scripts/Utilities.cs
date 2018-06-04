using UnityEngine;
using System.Collections;


public class Utilities : MonoBehaviour {

	public static bool DEBUG_IS_LOGGING_ENABLED = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static void DebugLog (object message, bool shouldForceLog) {
		if (DEBUG_IS_LOGGING_ENABLED || shouldForceLog) {
			Debug.Log (message);
		}
	}

	public static void DebugLog (object message) {
		Utilities.DebugLog (message, false);
	}

	// Show the game object by enabling its renderer
	public static void Show (GameObject gameObject) {
		Utilities.DebugLog ("Show ()");
		// First, try to access its generic Renderer
		Renderer renderer = gameObject.GetComponent<Renderer> ();
		if (renderer) {
			renderer.enabled = true;
		} else {
			// If that fails, try to access its SpriteRenderer
			SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
			if (spriteRenderer) {
				spriteRenderer.enabled = true;
			} else {
				// If that fails, try to access its CanvasRenderer
				CanvasRenderer canvasRenderer = gameObject.GetComponent<CanvasRenderer> ();
				if (canvasRenderer) {
					canvasRenderer.SetAlpha (1.0f);
				}
			}
		}

		// Show all children of this gameobject
		foreach (Transform child in gameObject.transform) {
			Show (child.gameObject);
		}
	}

	// Hide the game object by disabling its renderer
	public static void Hide (GameObject gameObject) {
		Utilities.DebugLog ("Hide ()");
		// First, try to access its generic Renderer
		Renderer renderer = gameObject.GetComponent<Renderer> ();
		if (renderer) {
			renderer.enabled = false;
		} else {
			// If that fails, try to access its SpriteRenderer
			SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
			if (spriteRenderer) {
				spriteRenderer.enabled = false;
			} else {
				// If that fails, try to access its CanvasRenderer
				CanvasRenderer canvasRenderer = gameObject.GetComponent<CanvasRenderer> ();
				if (canvasRenderer) {
					canvasRenderer.SetAlpha (0);
				}
			}
		}

		// Hide all children of this gameobject
		foreach (Transform child in gameObject.transform) {
			Hide (child.gameObject);
		}
	}
}
