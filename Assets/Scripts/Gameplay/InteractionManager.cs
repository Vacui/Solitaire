using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour {

    private static Stack<InteractionGroup> interactionGroups;
    private static InteractionGroup openIntegrationGroup = null;

    private static int interactions;
    public static int Interactions {
        get { return interactions; }
        set {
            interactions = value;
            NewInteraction?.Invoke(value);
        }
    }
    public static Action<int> NewInteraction;

    public static bool IsUndoing;

    private void Awake() {
        Solitaire.ResetEvent += () => {
            interactionGroups = new Stack<InteractionGroup>();
            Interactions = 0;
        };
    }

    public static void OpenInteraction() {
        if (openIntegrationGroup != null) {
            CloseInteraction();
        }

        openIntegrationGroup = new InteractionGroup();
    }

    public static void AddInteraction(Interaction interaction) {
        if (openIntegrationGroup == null) {
            Debug.LogWarning("No open interaction");
            return;
        }

        openIntegrationGroup.AddInteraction(interaction);
    }

    public static void CloseInteraction() {
        if (openIntegrationGroup == null) {
            Debug.LogWarning("No open interaction");
            return;
        }

        if (!openIntegrationGroup.IsEmpty) {
            interactionGroups.Push(openIntegrationGroup);
            Interactions++;
        }
               
        openIntegrationGroup = null;

        Instances.Solitaire.CheckWin();
    }

    public void UndoInteraction() {

        if (IsUndoing) {
            Debug.LogWarning("Can't undoing while undoing");
            return;
        }

        if (interactionGroups == null || interactionGroups.Count <= 0) {
            return;
        }

        IsUndoing = true;
        Interactions--;
        InteractionGroup interactionGroup = interactionGroups.Pop();
        interactionGroup.UndoInteractions();
        IsUndoing = false;

    }

}