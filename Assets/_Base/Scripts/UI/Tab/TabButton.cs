using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {

    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class TabButton : MonoBehaviour, IPointerClickHandler {
        [SerializeField] private bool goBackButton = false;
        [SerializeField, ShowIf("goBackButton", true)] private TabGroup group;
        [SerializeField, ShowIf("goBackButton", false)] private Tab tabToShow;

        private void Awake() {
            Button button = GetComponent<Button>();
            if (button != null) {
                button.onClick.AddListener(() => Action());
            }
        }

        public void OnPointerClick(PointerEventData eventData) {
            if(GetComponent<Button>() != null) {
                return;
            }
            Action();
        }

        public void Action() {
            if (goBackButton) {
                if (group == null) {
                    return;
                }

                group.GoBack();
                return;
            } else {

                if (tabToShow == null) {
                    return;
                }

                if (tabToShow.Group == null) {
                    tabToShow.ToggleActive();
                    return;
                }

                tabToShow.Group.ShowTab(tabToShow);
            }
        }
    }
}