using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class UA11YNavAgentMenuExtension : MonoBehaviour
{
    [MenuItem("GameObject/KAP/UA11YNavAgent/UA11YNavAgentManager", false, 10)]
    static void CreateUA11YNavAgentManager(MenuCommand menuCommand)
    {
        GameObject managerGameObject = Resources.Load<GameObject>("Prefabs/NavAgent/UA11YNavAgentManager");
        managerGameObject = Instantiate<GameObject>(managerGameObject);
        managerGameObject.name = "UA11YNavAgentManager";

        GameObjectUtility.SetParentAndAlign(managerGameObject, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(managerGameObject, "Create " + managerGameObject.name);
        Selection.activeObject = managerGameObject;
    }
}

#endif