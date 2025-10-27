using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace REPO
{
    public class Hacks : MonoBehaviour
    {
        private ValuableObject[] cachedValuables;
        private Enemy[] cachedEnemies;
        private float lastCacheTime;
        private float cacheInterval = 1f;

        private Camera mainCam;

        public bool enemyEsp = true;
        public bool enemyDistanceEsp = true;
        public bool enemyHealthEsp = true;

        public Transform localPlayer;

        void Start()
        {
            DontDestroyOnLoad(this.gameObject); // Nezničit po změně scény

            mainCam = Camera.main;
            if (mainCam == null)
            {
                Debug.LogError("Main camera not found!");
            }
        }

        void Update()
        {
            // Obnova kamery po změně scény
            if (mainCam == null || !mainCam.isActiveAndEnabled)
            {
                mainCam = Camera.main;
                if (mainCam != null)
                    Debug.Log("Main camera reacquired after scene load.");
            }

            // Obnova localPlayer pokud chybí (můžeš upravit podle své hry)
            if (localPlayer == null)
            {
                GameObject playerObj = GameObject.FindWithTag("Player"); // Ujisti se, že hráč má tag "Player"
                if (playerObj != null)
                    localPlayer = playerObj.transform;
            }

            // Cache každou sekundu
            if (Time.time - lastCacheTime > cacheInterval)
            {
                cachedValuables = Object.FindObjectsOfType<ValuableObject>();
                cachedEnemies = Object.FindObjectsOfType<Enemy>();
                lastCacheTime = Time.time;
            }
        }

        void OnGUI()
        {
            if (mainCam == null)
                return;

            // ESP na předměty
            if (cachedValuables != null)
            {
                foreach (var valuableObject in cachedValuables)
                {
                    if (valuableObject == null || valuableObject.transform == null)
                        continue;

                    Vector3 worldPos = valuableObject.transform.position;
                    Vector3 screenPos = WorldToScreen(worldPos);

                    if (screenPos.z > 0)
                    {
                        Vector2 guiPos = new Vector2(screenPos.x, screenPos.y);

                        if (IsOnScreen(guiPos))
                        {
                            DrawName(screenPos, valuableObject.name, Color.green);
                        }
                    }
                }
            }

            // ESP na nepřátele
            if (enemyEsp && cachedEnemies != null)
            {
                Vector3 offset = new Vector3(0, 1.5f, 0);

                foreach (Enemy enemy in cachedEnemies)
                {
                    if (enemy == null)
                        continue;

                    Vector3 enemyPos = (enemy.CenterTransform != null)
                        ? enemy.CenterTransform.position + offset
                        : enemy.transform.position + offset;

                    Vector3 screenPos = WorldToScreen(enemyPos);
                    if (screenPos.z <= 0)
                        continue;

                    EnemyParent enemyParent = enemy.GetComponentInParent<EnemyParent>();
                    if (enemyParent == null)
                        continue;

                    EnemyHealth enemyHealth = enemyParent.GetComponentInChildren<EnemyHealth>();
                    if (enemyHealth == null)
                        continue;

                    int health = enemyHealth.health;
                    if (health <= 0)
                        continue;

                    Vector2 guiPos = new Vector2(screenPos.x, screenPos.y);
                    if (!IsOnScreen(guiPos))
                        continue;

                    string text = enemyParent.enemyName;

                    if (enemyDistanceEsp && localPlayer != null)
                    {
                        float dist = Vector3.Distance(localPlayer.position, enemyPos);
                        text += $" [{dist:F1}M]";
                    }

                    DrawName(screenPos, text, Color.red);

                    if (enemyHealthEsp)
                    {
                        DrawHealth(screenPos, $"[{health}HP]", Color.red);
                    }
                }
            }
            GUIStyle smallText = new GUIStyle();
            smallText.fontSize = 10;
            smallText.normal.textColor = Color.white;
            smallText.alignment = TextAnchor.UpperRight;

            string credit = "Cheat made by ItzKubass on GitHub";

            Vector2 size = smallText.CalcSize(new GUIContent(credit));
            Rect position = new Rect(Screen.width - size.x - 5, 5, size.x, size.y);
            GUI.Label(position, credit, smallText);
        }

        Vector3 WorldToScreen(Vector3 worldPosition)
        {
            Matrix4x4 matrix = mainCam.projectionMatrix * mainCam.worldToCameraMatrix;
            Vector4 pos = matrix * new Vector4(worldPosition.x, worldPosition.y, worldPosition.z, 1f);

            if (pos.w <= 0f)
                return new Vector3(-1f, -1f, -1f);

            Vector3 ndc = new Vector3(pos.x / pos.w, pos.y / pos.w, pos.z / pos.w);

            float x = (ndc.x + 1f) * 0.5f * Screen.width;
            float y = (1f - ndc.y) * 0.5f * Screen.height;

            return new Vector3(x, y, ndc.z);
        }

        bool IsOnScreen(Vector2 pos)
        {
            return pos.x >= 0 && pos.x <= Screen.width && pos.y >= 0 && pos.y <= Screen.height;
        }

        void DrawName(Vector3 screenPos, string name, Color color)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = color;
            style.fontSize = 12;
            style.alignment = TextAnchor.MiddleCenter;

            Vector2 size = style.CalcSize(new GUIContent(name));
            Vector2 pos = new Vector2(screenPos.x - size.x / 2f, screenPos.y - size.y - 1f);

            GUI.Label(new Rect(pos.x, pos.y, size.x, size.y), name, style);
        }

        void DrawHealth(Vector3 screenPos, string healthText, Color color)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = color;
            style.fontSize = 10;
            style.alignment = TextAnchor.MiddleCenter;

            Vector2 size = style.CalcSize(new GUIContent(healthText));
            Vector2 pos = new Vector2(screenPos.x - size.x / 2f, screenPos.y + 2f);

            GUI.Label(new Rect(pos.x, pos.y, size.x, size.y), healthText, style);
        }
    }
}
