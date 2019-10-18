using UnityEngine;

public class MonoBehaviourWithInit : MonoBehaviour{

	private bool _isInitialized = false;

	public void InitIfNeeded(){
		if(_isInitialized){
			return;
		}
		Init();
		_isInitialized = true;
	}

	protected virtual void Init(){}

	protected virtual void Awake (){}

}
