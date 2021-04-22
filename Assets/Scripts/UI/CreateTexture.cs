using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
 public class CreateTexture : MonoBehaviour
 {
     public TextureSize textureSize = TextureSize.bit256;
     public enum TextureSize
     {
         bit16 = 16,
         bit32 = 32,
         bit64 = 64,
         bit128 = 128,
         bit256 = 256,
         bit512 = 512,
     }
     public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
     public FilterMode filterMode = FilterMode.Bilinear;
 
     public void GenerateTexture(int size , FilterMode filter, TextureWrapMode wrap)
     {
         Texture2D texture = new Texture2D(size, size);
         GetComponent<Renderer>().material.mainTexture = texture;
 
         Vector2 center = new Vector2(size / 2f, size / 2f);
 
         for (int y = 0; y < texture.height; y++)
         {
             for (int x = 0; x < texture.width; x++)
             {
                 float DistanceFromCenter = Vector2.Distance(center, new Vector2(x, y));
                 float currentAlpha = 1;
 
                 if((1 - (DistanceFromCenter / size)) >= 0)
                 {
                     currentAlpha = (1 - (DistanceFromCenter / size));
                 }
                 else
                 {
                     currentAlpha = 0;
                 }
 
                 Color color = new Color(currentAlpha, currentAlpha, currentAlpha, currentAlpha);
                 texture.SetPixel(x, y, color);
             }
         }
         texture.Apply();
         texture.filterMode = filterMode;
 
         SpriteRenderer s = GetComponent<SpriteRenderer>();
         s.sprite = Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(.5f,.5f));
         s.sprite.name = "New Sprite";    
     }
 }