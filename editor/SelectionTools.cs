using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using PrefabEvolution;

public class SelectionTools : EditorWindow {

	[MenuItem("GameObject/Match To (target,(s),destination")]
	public static void MatchTo(){
		var targets = (from s in Selection.gameObjects
		 select s).Skip (1).ToArray ();
		var destination = (from s in Selection.gameObjects
		                   select s).FirstOrDefault ();
		if ((targets.Count() == 0) || (destination == null)) {
			return;
		}

		foreach (var t in targets) {
			t.transform.position = destination.transform.position;
			t.transform.rotation = destination.transform.rotation;
		}

	}

	[MenuItem("Selection/Select MeshFilter Children (GameObject)")]
	public static void SelectChildMeshes(){
		var originalSelection = Selection.gameObjects;
		List<GameObject> newSelection = new List<GameObject> ();
		foreach (var go in originalSelection) {
			var meshFilters = go.GetComponentsInChildren<MeshFilter> ();
			foreach (var mf in meshFilters) {
				if (!newSelection.Contains (mf.gameObject)) {
					newSelection.Add (mf.gameObject);
				}
			}
		}
		Selection.objects = newSelection.ToArray ();
	}

	[MenuItem("Selection/Select Evolve Prefab Children")]
	public static void SelectChildPrefabs(){
		var originalSelection = Selection.gameObjects;
		List<GameObject> newSelection = new List<GameObject> ();
		foreach (var go in originalSelection) {
			var prefabs = go.GetComponentsInChildren<PrefabEvolution.EvolvePrefab> ();
			foreach (var pf in prefabs) {
				if (!newSelection.Contains (pf.gameObject)) {
					newSelection.Add (pf.gameObject);
				}
			}
		}
		Selection.objects = newSelection.ToArray ();
	}

	[MenuItem("Selection/Filter With Colliders (GameObject)")]
	public static void FilterColliders(){
		var originalSelection = Selection.gameObjects;
		List<GameObject> newSelection = new List<GameObject> ();
		foreach (var o in originalSelection) {
			if (o.GetComponent<Collider> () == null) {
				newSelection.Add (o);
			}
		}
		Selection.objects = newSelection.ToArray ();
	}

	[MenuItem("Selection/SelectChildren")]
	public static void SelectChildren(){
		var originalSelection = Selection.gameObjects;
		List<GameObject> newSelection = new List<GameObject> ();
		foreach (var o in originalSelection) {
			for (int c = 0; c < o.transform.childCount; c++) {
				newSelection.Add(o.transform.GetChild (c).gameObject);
			}
		}
		Selection.objects = newSelection.ToArray ();
	}

	[MenuItem("Selection/SelectParents")]
	public static void SelectParents(){
		var originalSelection = Selection.gameObjects;
		List<GameObject> newSelection = new List<GameObject> ();
		foreach (var o in originalSelection) {
			var p = o.transform.parent;
			if (!newSelection.Contains (p.gameObject)) {
				newSelection.Add (p.gameObject);
			}
		}
		Selection.objects = newSelection.ToArray ();
	}

	[MenuItem("GameObject/Create Matched Parent(s)")]
	public static void CreateMatchedParents(){
		var originalSelection = Selection.gameObjects;
		foreach (var o in originalSelection) {
			var newParent = new GameObject (o.name);
			newParent.transform.position = o.transform.position;
			newParent.transform.rotation = o.transform.rotation;
			newParent.transform.localScale = o.transform.localScale;
			o.transform.parent = newParent.transform;
		}
	}

	[MenuItem("GameObject/Create Parent(s)")]
	public static void CreateParents(){
		List<GameObject> newSelection = new List<GameObject>();
		var originalSelection = Selection.gameObjects;
		foreach (var o in originalSelection) {
			var oldParent = o.transform.parent;
			var newParent = new GameObject (o.name);
			newParent.transform.parent = oldParent;
			o.transform.parent = newParent.transform;
			newSelection.Add(newParent);
		}
		Selection.objects = newSelection.ToArray();
	}


	public static void PostpendNames(string s){
		var originalSelection = Selection.gameObjects;
		foreach (var o in originalSelection) {
			o.name = o.name + s;
		}
	}

}
