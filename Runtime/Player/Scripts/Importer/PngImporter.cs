using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Module5.Player
{
    [System.Serializable]
    public class ImageImportSettings
    {
        public bool alphaIsTransparency = false;
        public FilterMode filterMode = FilterMode.Bilinear;
        public TextureWrapMode wrapMode = TextureWrapMode.Repeat;
    }

    public class PngImporter
    {
        const int FileFormatVersion = 1;

        public async Task<Texture2D> ImportAsync(string filePath, ImageImportSettings settings)
        {
            byte[] bytes = await File.ReadAllBytesAsync(filePath);
            Texture2D texture = new Texture2D(4, 4);
            texture.filterMode = settings.filterMode;
            texture.wrapMode = settings.wrapMode;
            bool success = texture.LoadImage(bytes, false);
            
            if (success)
            {
                if (!settings.alphaIsTransparency)
                {
                    texture.LoadImage(texture.EncodeToJPG(), false);
                }
                texture.name = Path.GetFileNameWithoutExtension(filePath);
                return texture;
            }
            else
            {
                return null;
            }
        }
    }
}
