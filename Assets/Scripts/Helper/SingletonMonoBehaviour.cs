using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviourWithInit where T : MonoBehaviourWithInit {

	private static T _instance;
	public static T Instance {
		get {
			if (_instance == null) {

				_instance = (T)FindObjectOfType (typeof(T));

				if (_instance == null) {
					Debug.LogError (typeof(T) + " is nothing");
				}
				
				else {
					_instance.InitIfNeeded ();
				}
			}
			return _instance;
		}
	}

	protected sealed override void Awake(){
		if(this == Instance){
			return;
		}

		Debug.LogError (typeof(T) + " is duplicated");
	}

}
