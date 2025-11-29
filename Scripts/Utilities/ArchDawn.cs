using System.Collections.Generic;
using System;
using UnityEngine;

namespace ArchDawn.Utilities
{
    public static class UtilsClass
    {
        public const int sortingOrderDefault = 5000;

        // Create a Sprite in the World, no parent
        public static GameObject CreateWorldSprite(string name, Sprite sprite, Vector3 position, Vector3 localScale, int sortingOrder, Color color)
        {
            return CreateWorldSprite(null, name, sprite, position, localScale, sortingOrder, color);
        }

        // Create a Sprite in the World
        public static GameObject CreateWorldSprite(Transform parent, string name, Sprite sprite, Vector3 localPosition, Vector3 localScale, int sortingOrder, Color color)
        {
            GameObject gameObject = new GameObject(name, typeof(SpriteRenderer));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            transform.localScale = localScale;
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = sortingOrder;
            spriteRenderer.color = color;
            return gameObject;
        }

        // Create Text in the World
        public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = sortingOrderDefault)
        {
            if (color == null) color = Color.white;
            return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
        }

        public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
        {
            GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            TextMesh textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
            return textMesh;
        }

        // Get Mouse Position in World with Z = 0f
        public static Vector3 GetMouseWorldPosition()
        {
            Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
            vec.z = 0f;
            return vec;
        }

        public static Vector3 GetMouseWorldPositionWithZ()
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
        {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }

        public static Vector3 GetDirToMouse(Vector3 fromPosition)
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            return (mouseWorldPosition - fromPosition).normalized;
        }

        // Create a Text Popup in the World, no parent 
        public static void CreateWorldTextPopup(string text, Vector3 localPosition, float popupTime = 1f)
        {
            CreateWorldTextPopup(null, text, localPosition, 40, Color.white, localPosition + new Vector3(0, 20), popupTime);
        }

        // Create a Text Popup in the World, no parent
        public static void CreateWorldTextPopup(string text, Vector3 localPosition, Color color, float popupTime = 1f)
        {
            CreateWorldTextPopup(null, text, localPosition, 40, color, localPosition + new Vector3(0, 20), popupTime);
        }

        // Create a Text Popup in the World
        public static void CreateWorldTextPopup(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, Vector3 finalPopupPosition, float popupTime)
        {
            text = Localizer.Instance.GetLocalizedText(text);
            TextMesh textMesh = CreateWorldText(parent, text, localPosition, fontSize, color, TextAnchor.LowerLeft, TextAlignment.Left, sortingOrderDefault);
            Transform transform = textMesh.transform;
            Vector3 moveAmount = (finalPopupPosition - localPosition) / popupTime;
            FunctionUpdater.Create(delegate () {
                transform.position += moveAmount * Time.unscaledDeltaTime;
                popupTime -= Time.unscaledDeltaTime;
                if (popupTime <= 0f)
                {
                    UnityEngine.Object.Destroy(transform.gameObject);
                    return true;
                }
                else
                {
                    return false;
                }
            }, "WorldTextPopup");
        }

        // Returns 00-FF, value 0->255
        public static string Dec_to_Hex(int value)
        {
            return value.ToString("X2");
        }

        // Returns 0-255
        public static int Hex_to_Dec(string hex)
        {
            return Convert.ToInt32(hex, 16);
        }

        // Returns a hex string based on a number between 0->1
        public static string Dec01_to_Hex(float value)
        {
            return Dec_to_Hex((int)Mathf.Round(value * 255f));
        }

        // Returns a float between 0->1
        public static float Hex_to_Dec01(string hex)
        {
            return Hex_to_Dec(hex) / 255f;
        }

        // Get Hex Color FF00FF
        public static string GetStringFromColor(Color color)
        {
            string red = Dec01_to_Hex(color.r);
            string green = Dec01_to_Hex(color.g);
            string blue = Dec01_to_Hex(color.b);
            return red + green + blue;
        }

        // Get Hex Color FF00FFAA
        public static string GetStringFromColorWithAlpha(Color color)
        {
            string alpha = Dec01_to_Hex(color.a);
            return GetStringFromColor(color) + alpha;
        }

        // Sets out values to Hex String 'FF'
        public static void GetStringFromColor(Color color, out string red, out string green, out string blue, out string alpha)
        {
            red = Dec01_to_Hex(color.r);
            green = Dec01_to_Hex(color.g);
            blue = Dec01_to_Hex(color.b);
            alpha = Dec01_to_Hex(color.a);
        }

        // Get Hex Color FF00FF
        public static string GetStringFromColor(float r, float g, float b)
        {
            string red = Dec01_to_Hex(r);
            string green = Dec01_to_Hex(g);
            string blue = Dec01_to_Hex(b);
            return red + green + blue;
        }

        // Get Hex Color FF00FFAA
        public static string GetStringFromColor(float r, float g, float b, float a)
        {
            string alpha = Dec01_to_Hex(a);
            return GetStringFromColor(r, g, b) + alpha;
        }

        // Get Color from Hex string FF00FFAA
        public static Color GetColorFromString(string color)
        {
            float red = Hex_to_Dec01(color.Substring(0, 2));
            float green = Hex_to_Dec01(color.Substring(2, 2));
            float blue = Hex_to_Dec01(color.Substring(4, 2));
            float alpha = 1f;
            if (color.Length >= 8)
            {
                // Color string contains alpha
                alpha = Hex_to_Dec01(color.Substring(6, 2));
            }
            return new Color(red, green, blue, alpha);
        }
    }

    /*
 * Calls function on every Update until it returns true
 * */
    public class FunctionUpdater
    {

        /*
         * Class to hook Actions into MonoBehaviour
         * */
        private class MonoBehaviourHook : MonoBehaviour
        {

            public Action OnUpdate;

            private void Update()
            {
                if (OnUpdate != null) OnUpdate();
            }

        }

        private static List<FunctionUpdater> updaterList; // Holds a reference to all active updaters
        private static GameObject initGameObject; // Global game object used for initializing class, is destroyed on scene change

        private static void InitIfNeeded()
        {
            if (initGameObject == null)
            {
                initGameObject = new GameObject("FunctionUpdater_Global");
                updaterList = new List<FunctionUpdater>();
            }
        }




        public static FunctionUpdater Create(Action updateFunc)
        {
            return Create(() => { updateFunc(); return false; }, "", true, false);
        }

        public static FunctionUpdater Create(Action updateFunc, string functionName)
        {
            return Create(() => { updateFunc(); return false; }, functionName, true, false);
        }

        public static FunctionUpdater Create(Func<bool> updateFunc)
        {
            return Create(updateFunc, "", true, false);
        }

        public static FunctionUpdater Create(Func<bool> updateFunc, string functionName)
        {
            return Create(updateFunc, functionName, true, false);
        }

        public static FunctionUpdater Create(Func<bool> updateFunc, string functionName, bool active)
        {
            return Create(updateFunc, functionName, active, false);
        }

        public static FunctionUpdater Create(Func<bool> updateFunc, string functionName, bool active, bool stopAllWithSameName)
        {
            InitIfNeeded();

            if (stopAllWithSameName)
            {
                StopAllUpdatersWithName(functionName);
            }

            GameObject gameObject = new GameObject("FunctionUpdater Object " + functionName, typeof(MonoBehaviourHook));
            FunctionUpdater functionUpdater = new FunctionUpdater(gameObject, updateFunc, functionName, active);
            gameObject.GetComponent<MonoBehaviourHook>().OnUpdate = functionUpdater.Update;

            updaterList.Add(functionUpdater);
            return functionUpdater;
        }

        private static void RemoveUpdater(FunctionUpdater funcUpdater)
        {
            InitIfNeeded();
            updaterList.Remove(funcUpdater);
        }

        public static void DestroyUpdater(FunctionUpdater funcUpdater)
        {
            InitIfNeeded();
            if (funcUpdater != null)
            {
                funcUpdater.DestroySelf();
            }
        }

        public static void StopUpdaterWithName(string functionName)
        {
            InitIfNeeded();
            for (int i = 0; i < updaterList.Count; i++)
            {
                if (updaterList[i].functionName == functionName)
                {
                    updaterList[i].DestroySelf();
                    return;
                }
            }
        }

        public static void StopAllUpdatersWithName(string functionName)
        {
            InitIfNeeded();
            for (int i = 0; i < updaterList.Count; i++)
            {
                if (updaterList[i].functionName == functionName)
                {
                    updaterList[i].DestroySelf();
                    i--;
                }
            }
        }





        private GameObject gameObject;
        private string functionName;
        private bool active;
        private Func<bool> updateFunc; // Destroy Updater if return true;

        public FunctionUpdater(GameObject gameObject, Func<bool> updateFunc, string functionName, bool active)
        {
            this.gameObject = gameObject;
            this.updateFunc = updateFunc;
            this.functionName = functionName;
            this.active = active;
        }

        public void Pause()
        {
            active = false;
        }

        public void Resume()
        {
            active = true;
        }

        private void Update()
        {
            if (!active) return;
            if (updateFunc())
            {
                DestroySelf();
            }
        }

        public void DestroySelf()
        {
            RemoveUpdater(this);
            if (gameObject != null)
            {
                UnityEngine.Object.Destroy(gameObject);
            }
        }

    }

    public static class MeshUtils
    {

        private static readonly Vector3 Vector3zero = Vector3.zero;
        private static readonly Vector3 Vector3one = Vector3.one;
        private static readonly Vector3 Vector3yDown = new Vector3(0, -1);


        private static Quaternion[] cachedQuaternionEulerArr;
        private static void CacheQuaternionEuler()
        {
            if (cachedQuaternionEulerArr != null) return;
            cachedQuaternionEulerArr = new Quaternion[360];
            for (int i = 0; i < 360; i++)
            {
                cachedQuaternionEulerArr[i] = Quaternion.Euler(0, 0, i);
            }
        }
        private static Quaternion GetQuaternionEuler(float rotFloat)
        {
            int rot = Mathf.RoundToInt(rotFloat);
            rot = rot % 360;
            if (rot < 0) rot += 360;
            //if (rot >= 360) rot -= 360;
            if (cachedQuaternionEulerArr == null) CacheQuaternionEuler();
            return cachedQuaternionEulerArr[rot];
        }


        public static Mesh CreateEmptyMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[0];
            mesh.uv = new Vector2[0];
            mesh.triangles = new int[0];
            return mesh;
        }

        public static void CreateEmptyMeshArrays(int quadCount, out Vector3[] vertices, out Vector2[] uvs, out int[] triangles)
        {
            vertices = new Vector3[4 * quadCount];
            uvs = new Vector2[4 * quadCount];
            triangles = new int[6 * quadCount];
        }

        public static Mesh CreateMesh(Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11)
        {
            return AddToMesh(null, pos, rot, baseSize, uv00, uv11);
        }

        public static Mesh AddToMesh(Mesh mesh, Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11)
        {
            if (mesh == null)
            {
                mesh = CreateEmptyMesh();
            }
            Vector3[] vertices = new Vector3[4 + mesh.vertices.Length];
            Vector2[] uvs = new Vector2[4 + mesh.uv.Length];
            int[] triangles = new int[6 + mesh.triangles.Length];

            mesh.vertices.CopyTo(vertices, 0);
            mesh.uv.CopyTo(uvs, 0);
            mesh.triangles.CopyTo(triangles, 0);

            int index = vertices.Length / 4 - 1;
            //Relocate vertices
            int vIndex = index * 4;
            int vIndex0 = vIndex;
            int vIndex1 = vIndex + 1;
            int vIndex2 = vIndex + 2;
            int vIndex3 = vIndex + 3;

            baseSize *= .5f;

            bool skewed = baseSize.x != baseSize.y;
            if (skewed)
            {
                vertices[vIndex0] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, baseSize.y);
                vertices[vIndex1] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, -baseSize.y);
                vertices[vIndex2] = pos + GetQuaternionEuler(rot) * new Vector3(baseSize.x, -baseSize.y);
                vertices[vIndex3] = pos + GetQuaternionEuler(rot) * baseSize;
            }
            else
            {
                vertices[vIndex0] = pos + GetQuaternionEuler(rot - 270) * baseSize;
                vertices[vIndex1] = pos + GetQuaternionEuler(rot - 180) * baseSize;
                vertices[vIndex2] = pos + GetQuaternionEuler(rot - 90) * baseSize;
                vertices[vIndex3] = pos + GetQuaternionEuler(rot - 0) * baseSize;
            }

            //Relocate UVs
            uvs[vIndex0] = new Vector2(uv00.x, uv11.y);
            uvs[vIndex1] = new Vector2(uv00.x, uv00.y);
            uvs[vIndex2] = new Vector2(uv11.x, uv00.y);
            uvs[vIndex3] = new Vector2(uv11.x, uv11.y);

            //Create triangles
            int tIndex = index * 6;

            triangles[tIndex + 0] = vIndex0;
            triangles[tIndex + 1] = vIndex3;
            triangles[tIndex + 2] = vIndex1;

            triangles[tIndex + 3] = vIndex1;
            triangles[tIndex + 4] = vIndex3;
            triangles[tIndex + 5] = vIndex2;

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            //mesh.bounds = bounds;

            return mesh;
        }

        public static void AddToMeshArrays(Vector3[] vertices, Vector2[] uvs, int[] triangles, int index, Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11)
        {
            //Relocate vertices
            int vIndex = index * 4;
            int vIndex0 = vIndex;
            int vIndex1 = vIndex + 1;
            int vIndex2 = vIndex + 2;
            int vIndex3 = vIndex + 3;

            baseSize *= .5f;

            bool skewed = baseSize.x != baseSize.y;
            if (skewed)
            {
                vertices[vIndex0] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, baseSize.y);
                vertices[vIndex1] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, -baseSize.y);
                vertices[vIndex2] = pos + GetQuaternionEuler(rot) * new Vector3(baseSize.x, -baseSize.y);
                vertices[vIndex3] = pos + GetQuaternionEuler(rot) * baseSize;
            }
            else
            {
                vertices[vIndex0] = pos + GetQuaternionEuler(rot - 270) * baseSize;
                vertices[vIndex1] = pos + GetQuaternionEuler(rot - 180) * baseSize;
                vertices[vIndex2] = pos + GetQuaternionEuler(rot - 90) * baseSize;
                vertices[vIndex3] = pos + GetQuaternionEuler(rot - 0) * baseSize;
            }

            //Relocate UVs
            uvs[vIndex0] = new Vector2(uv00.x, uv11.y);
            uvs[vIndex1] = new Vector2(uv00.x, uv00.y);
            uvs[vIndex2] = new Vector2(uv11.x, uv00.y);
            uvs[vIndex3] = new Vector2(uv11.x, uv11.y);

            //Create triangles
            int tIndex = index * 6;

            triangles[tIndex + 0] = vIndex0;
            triangles[tIndex + 1] = vIndex3;
            triangles[tIndex + 2] = vIndex1;

            triangles[tIndex + 3] = vIndex1;
            triangles[tIndex + 4] = vIndex3;
            triangles[tIndex + 5] = vIndex2;
        }
    }




}