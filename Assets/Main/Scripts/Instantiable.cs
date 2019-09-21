using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gameplay
{
    public abstract class Instantiable<T> : MonoBehaviour
    {
        public T data { get; private set; }
        private bool instantiated;
        private List<Instantiable<T>> instances;

        public abstract void OnCreate(T data);

        public void Reinstantiate(IEnumerable<T> dataCollection)
        {
            if (this.instantiated)
            {
                Debug.LogError("Can not instantiate on non-base object");
                return;
            }

            ClearInstances();

            //Recreate new instances
            foreach (var data in dataCollection)
            {
                var instance = Instantiate(this, this.transform.parent);
                instance.gameObject.SetActive(true);
                instance.transform.localPosition = transform.localPosition;
                instance.transform.localScale = transform.localScale;
                instance.instantiated = true;
                instance.data = data;
                instance.OnCreate(data);
                instances.Add(instance);
            }
        }

        public void ClearInstances()
        {
            if (instances == null) instances = new List<Instantiable<T>>();
            foreach (var i in instances) Destroy(i.gameObject);
            instances.Clear();
        }

        public IEnumerable<Instantiable<T>> Instances
        {
            get
            {
                if (instances == null) yield break;
                foreach (var instance in instances) yield return instance;
            }
        }

    }
}