using System;
using UnityEngine;

namespace REPO
{
    public class Render : MonoBehaviour
    {
        public static GUIStyle StringStyle { get; set; } = new GUIStyle(GUI.skin.label);

        public static UnityEngine.Color Color
        {
            get => GUI.color;
            set => GUI.color = value;
        }

        public static void DrawString(UnityEngine.Vector2 position, string label, bool centered = true)
        {
            GUIContent guicontent = new GUIContent(label);
            UnityEngine.Vector2 size = StringStyle.CalcSize(guicontent);
            UnityEngine.Vector2 pos = centered ? (position - size / 2f) : position;
            GUI.Label(new Rect(pos, size), guicontent);
        }

        public static void DrawLine(UnityEngine.Vector2 pointA, UnityEngine.Vector2 pointB, UnityEngine.Color color, float width)
        {
            if (lineTex == null)
            {
                lineTex = new Texture2D(1, 1);
            }

            UnityEngine.Matrix4x4 matrix = GUI.matrix;
            UnityEngine.Color oldColor = GUI.color;

            GUI.color = color;

            float angle = Vector3.Angle(pointB - pointA, Vector2.right);
            if (pointA.y > pointB.y)
            {
                angle = -angle;
            }

            GUIUtility.ScaleAroundPivot(new UnityEngine.Vector2((pointB - pointA).magnitude, width), new UnityEngine.Vector2(pointA.x, pointA.y + 0.5f));
            GUIUtility.RotateAroundPivot(angle, pointA);

            GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1f, 1f), lineTex);

            GUI.matrix = matrix;
            GUI.color = oldColor;
        }

        public static void DrawBox(float x, float y, float w, float h, UnityEngine.Color color, float thickness)
        {
            DrawLine(new UnityEngine.Vector2(x, y), new UnityEngine.Vector2(x + w, y), color, thickness);
            DrawLine(new UnityEngine.Vector2(x, y), new UnityEngine.Vector2(x, y + h), color, thickness);
            DrawLine(new UnityEngine.Vector2(x + w, y), new UnityEngine.Vector2(x + w, y + h), color, thickness);
            DrawLine(new UnityEngine.Vector2(x, y + h), new UnityEngine.Vector2(x + w, y + h), color, thickness);
        }

        public static Texture2D lineTex;
    }
}
