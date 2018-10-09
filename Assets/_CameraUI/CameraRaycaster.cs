using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using RPG.Characters;
using System;

namespace RPG.CameraUI
{
    public class CameraRaycaster : MonoBehaviour
    {
        float maxRaycastDepth = 100f; // Hard coded value
        int topPriorityLayerLastFrame = -1; // So get ? from start with Default layer 
        [SerializeField] Texture2D walkCursor = null;
        [SerializeField] Texture2D enemyCursor = null;
        [SerializeField] Texture2D lootableCursor = null;
        [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);

        const int WALKABLE_LAYER = 8;
        const int LOOTABLE_LAYER = 11;

        //New delegates
        public delegate void OnMouseOverEnemy(Enemy enemy);
        public event OnMouseOverEnemy onMouseOverEnemy;
        public delegate void OnMouseOverLootable(GameObject lootable);
        public event OnMouseOverLootable onMouseOverLootable;

        Rect screenRect;

        public delegate void OnMouseOverWalkable(Vector3 destination);
        public event OnMouseOverWalkable onMouseOverWalkable;

        //OnMouseOverInteractable(??) //TODO Set Up Interactables


        void Update()
        {
            screenRect = new Rect(0, 0, Screen.width, Screen.height);
            // Check if pointer is over an interactable UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                //Implement UI interaction
            }
            else
            {
                PerformRaycasts();
            }
        }

        void PerformRaycasts()
        {
            if(screenRect.Contains(Input.mousePosition))
            {
                //Specify Layer Priorities, order matters
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (RaycastForEnemy(ray))
                {
                    return;
                }
                //if (RaycastForLootable(ray)) //TODO Fix when Loot works
                //{
                //    return;
                //}
                if (RaycastForWalkable(ray))
                {
                    return;
                }
            }
        }
        bool RaycastForEnemy(Ray ray)
        {
            RaycastHit hitInfo;
            Physics.Raycast(ray, out hitInfo, maxRaycastDepth);
            var gameObjectHit = hitInfo.collider.gameObject;
            var enemyHit = gameObjectHit.GetComponent<Enemy>();
            if (enemyHit)
            {
                Cursor.SetCursor(enemyCursor, cursorHotspot, CursorMode.Auto);
                onMouseOverEnemy(enemyHit);
                return true;
            }
            return false;
        }

        //private bool RaycastForLootable(Ray ray) //TODO Fix when Loot works
        //{
        //    RaycastHit hitInfo;
        //    Physics.Raycast(ray, out hitInfo, maxRaycastDepth);
        //    var gameObjectHit = hitInfo.collider.gameObject;
        //    var lootableHit = gameObjectHit.GetComponent<Enemy>();
        //    if (enemyHit)
        //    {
        //        Cursor.SetCursor(enemyCursor, cursorHotspot, CursorMode.Auto);
        //        onMouseOverEnemy(enemyHit);
        //        return true;
        //    }
        //}

        private bool RaycastForWalkable(Ray ray)
        {
            RaycastHit hitInfo;
            LayerMask walkableLayer = 1 << WALKABLE_LAYER;
            bool walkableHit = Physics.Raycast(ray, out hitInfo, maxRaycastDepth, walkableLayer);
            if (walkableHit)
            {
                Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
                onMouseOverWalkable(hitInfo.point);
                return true;
            }
            return false;
        }
    }
}