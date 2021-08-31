using System.Linq;
using UnityEngine;
using Utils;

public class InputController : MonoBehaviour {

    public InputController Instance;

    private Vector3 touchPosition;
    private Group selectedGroup;
    private Vector3 direction;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }

        Instance = this;
    }

    private void Update() {

        if (Solitarie.IsPaused) {
            return;
        }

        if (Input.GetMouseButtonDown(0)) {
            //Debug.Log("Click!");

            touchPosition = UtilsClass.GetMouseWorldPosition();

            if (selectedGroup != null) {
                selectedGroup.Deselect(touchPosition);
            }
            selectedGroup = null;

            RaycastHit2D[] hitInfoArray = Physics2D.RaycastAll(touchPosition, Vector2.zero, 130f);

            touchPosition.z = 0f;

            if (hitInfoArray.Length > 0 && hitInfoArray.First().collider != null) {

                //Debug.Log("Hit");
                Card selectedCard = hitInfoArray.First().transform.GetComponent<Card>();
                if (selectedCard != null) {
                    Debug.Log(string.Format("Click on card {0}", selectedCard.ToString()));
                    selectedGroup = selectedCard.Select();
                    return;
                }

                Waste waste = hitInfoArray.First().transform.GetComponent<Waste>();
                if(waste != null) {
                    InteractionManager.OpenInteraction();
                    waste.DrawCard(true);
                    InteractionManager.CloseInteraction();
                }
            }
            
        }

        if (Input.GetMouseButton(0)) {
            if(selectedGroup == null) {
                return;
            }

            touchPosition = UtilsClass.GetMouseWorldPosition();

            selectedGroup.MoveTo(touchPosition);
        }

        if (Input.GetMouseButtonUp(0)) {
            if (selectedGroup == null) {
                return;
            }

            selectedGroup.Deselect(touchPosition);
            selectedGroup = null;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(touchPosition, 1);
    }

}