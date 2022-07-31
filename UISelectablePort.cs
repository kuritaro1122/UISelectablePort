using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("UISelectablePort")]
[RequireComponent(typeof(Selectable))]
public class UISelectablePort : MonoBehaviour {
    [SerializeField] EventSystem eventSystem = null;
    [SerializeField] Way[] ways = new Way[0];
    private Selectable selfSelectable = null;
    private GameObject prebSelectable = null;
    public bool IsEnable {
        get {
            foreach (var w in this.ways) {
                if (w.IsEnable) return true;
            }
            return false;
        }
    }
    [SerializeField] Selectable error;

    // Start is called before the first frame update
    void Start() {
        if (this.eventSystem == null) this.eventSystem = EventSystem.current;
        this.selfSelectable = this.GetComponent<Selectable>();
    }

    // Update is called once per frame
    void Update() {
        UpdateCurrentSelectable();
        UpdatePrebSelectable();
    }
    private void UpdatePrebSelectable() {
        if (this.eventSystem.currentSelectedGameObject != this.gameObject) {
            this.prebSelectable = this.eventSystem.currentSelectedGameObject;
        }
    }
    private void UpdateCurrentSelectable() {
        this.gameObject.SetActive(this.IsEnable);
        this.selfSelectable.interactable = this.IsEnable;
        if (!this.IsEnable) return;
        if (this.eventSystem.currentSelectedGameObject == this.gameObject) {
            foreach (var w in this.ways) {
                GameObject move = w.MovePort(this.prebSelectable);
                if (move != null) {
                    this.eventSystem.SetSelectedGameObject(move);
                    break;
                } else this.eventSystem.SetSelectedGameObject(error.gameObject);
            }
        }
    }

    [System.Serializable]
    class Way {
        private static bool SelectableIsEnable(Selectable s) => s.gameObject.activeInHierarchy && s.interactable;
        [SerializeField] Selectable[] s0;
        [SerializeField] Selectable[] s1;
        private static bool SelectablesIsEnable(Selectable[] ss) {
            foreach (var s in ss) {
                if (SelectableIsEnable(s)) return true;
            }
            return false;
        }
        private static bool Contents(Selectable[] selectables, GameObject search) {
            foreach (var s in selectables) {
                if (s.gameObject == search) return true;
            }
            return false;
        }
        private static GameObject GetSelectable(Selectable[] selectables) {
            foreach (var s in selectables) {
                if (SelectableIsEnable(s)) return s.gameObject;
            }
            return null;
        }
        public bool IsEnable => SelectablesIsEnable(s0) && SelectablesIsEnable(s1);
        public GameObject MovePort(GameObject preb) {
            if (Contents(s0, preb)) {
                return GetSelectable(s1);
            } else if (Contents(s1, preb)) {
                return GetSelectable(s0);
            } else return null;
        }
    }
}
