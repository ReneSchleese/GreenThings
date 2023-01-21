using System;
using UnityEngine;

public class ForestSpirit : MonoBehaviour
{
    private void Update()
    {
        /*
         * if not following
         *      if player in range
         *          move closer to player
         *          start following player
         * else if following player
         *      if needs to catch up
         *          if is first spirit
         *              move to player position
         *          else
         *              follow last spirit
         * else if following spirit
         *      if player in range
         *          follow player
         *      if needs to catch up to spirit
         *          move to spirit position
         */
    }
}