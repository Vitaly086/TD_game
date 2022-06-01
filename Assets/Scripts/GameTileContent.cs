using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTileContent : MonoBehaviour
{
   public GameTileContentType Type => _type;
   [SerializeField] private GameTileContentType _type; 
   
   public GameTailContentFactory  TailContentFactory { get; set; }

   public void Recycle()
   {
      TailContentFactory.Reclaim(this);
   }
}

public enum GameTileContentType
{
   Empty,
   Destination,
   Wall
}
