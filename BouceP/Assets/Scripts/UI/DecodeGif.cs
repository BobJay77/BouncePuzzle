using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.GifAssets.PowerGif.Examples.Scripts
{
	public class DecodeGif : MonoBehaviour
	{
		
		public List<AnimatedImage> AnimatedImage;

		public void OnEnable()
		{
			foreach (var image in AnimatedImage)
			{
                var path = "Assets/Gifs/" + image.gifPath + ".gif";

                if (path == "") return;

                var bytes = File.ReadAllBytes(path);
                var gif = Gif.Decode(bytes);

                image.Play(gif);
            }
			
		}
	}
}