using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

//Code taken from EVE, tweaked for use with KWS
namespace Utils
{
    public class KWSMaterialManager
    {
        bool cached = false;

        //Apply the materialistic properties to the texture.
        List<KeyValuePair<String, object>> cache = new List<KeyValuePair<string, object>>();
        public void ApplyMaterialProperties(Material material, bool clampTextures = false)
        {
            if (!cached)
            {
                FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                foreach (FieldInfo field in fields)
                {
                    String name = field.Name;
                    //texture
                    if (field.FieldType == typeof(String))
                    {
                        String textureName = (String)field.GetValue(this);
                        bool isNormal = textureName.Contains("Bump") | textureName.Contains("Bmp") | textureName.Contains("Normal") | textureName.Contains("Nrm");
                        Texture2D texture = GameDatabase.Instance.GetTexture(textureName, isNormal); //set the 2D texture
                        if (clampTextures)
                        {
                            texture.wrapMode = TextureWrapMode.Clamp; //Clamp mode for texture wrap
                        }
                        try
                        {
                            Color32[] pixels = texture.GetPixels32(); //get the array of pixels from the texture
                            int width = texture.width; //the width of the texture
                            int height = texture.height; //the height of the texture
                            texture.Resize(width, height, TextureFormat.ARGB32, true); //resize to fit
                            texture.SetPixels32(pixels); //set the pixels as the ones in the texture
                            texture.Apply(true); //apply the texture
                        }
                        catch { }
                        cache.Add(new KeyValuePair<string, object>(name, texture));
                    }
                    else
                    {
                        cache.Add(new KeyValuePair<string, object>(name, field.GetValue(this)));
                    }
                }
                cached = true;
            }
            ApplyCache(material);
        }
        private void ApplyCache(Material material)
        {
            foreach (KeyValuePair<String, object> field in cache)
            {
                String name = field.Key;
                object obj = field.Value;
                if (obj == null || obj.GetType() == typeof(Texture2D))
                {
                    Texture2D value = (Texture2D)obj;
                    material.SetTexture(name, value);
                }
                //float
                else if (obj.GetType() == typeof(float))
                {
                    float value = (float)obj;
                    material.SetFloat(name, value);
                }
                //Color
                else if (obj.GetType() == typeof(Color))
                {
                    Color value = (Color)obj;
                    material.SetColor(name, value);
                }
                //Vector3
                else if (obj.GetType() == typeof(Vector3))
                {
                    Vector3 value = (Vector3)obj;
                    material.SetVector(name, value);
                }
            }
        }
    }
}
