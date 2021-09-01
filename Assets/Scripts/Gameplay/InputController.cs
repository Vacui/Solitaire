using System.Linq;
using UnityEngine;
using Utils;

public class InputController : MonoBehaviour {

    private Vector3 clickPosition;
    private Group selectedGroup;
    private Vector3 direction;

    private void Update() {

        if (Solitaire.IsPaused) {
            return;
        }

        if (Input.GetMouseButtonDown(0)) {
            //Debug.Log("Click!");

            clickPosition = UtilsClass.GetMouseWorldPosition();

            if (selectedGroup != null) {
                selectedGroup.Deselect(clickPosition);
            }
            selectedGroup = null;

            RaycastHit2D[] hitInfoArray = Physics2D.RaycastAll(clickPosition, Vector2.zero, 130f);

            clickPosition.z = 0f;

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

            clickPosition = UtilsClass.GetMouseWorldPosition();

            selectedGroup.MoveTo(clickPosition);
        }

        if (Input.GetMouseButtonUp(0)) {
            if (selectedGroup == null) {
                return;
            }

            selectedGroup.Deselect(clickPosition);
            selectedGroup = null;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(clickPosition, 1);
    }

}