using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLTFast;
using System.Threading.Tasks;
using System;
using System.Threading;
using GLTFast.Schema;

namespace Module5.Player
{
    public class GltfImportData : IModelData
    {
        public GltfImport gltfImport;
        public string Name { get; set; }
        public async Task<bool> Construct(Transform root, CancellationToken cancellationToken)
        {
            //ModelConstructResult result = new();
            if (gltfImport != null)
            {
                GameObjectInstantiator instantiator = new(gltfImport, root);
                if (await gltfImport.InstantiateMainSceneAsync(instantiator, cancellationToken))
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            return Name;
        }

        public void Dispose()
        {
            gltfImport.Dispose();
        }
    }

    public class GltfImporter
    {
        const int FileFormatVersion = 1;
        public async Task<GltfImportData> ImportAsync(Uri uri, ImportSettings importSettings = null)
        {
            if(importSettings == null)
            {
                importSettings = new()
                {
                    NodeNameMethod = NameImportMethod.OriginalUnique,
                    AnimationMethod = AnimationMethod.Legacy,
                    GenerateMipMaps = true,
                    AnisotropicFilterLevel = 1,
                };
            }

            // Load the glTF and pass along the settings
            //GLTFast.Logging.ConsoleLogger consoleLogger = new GLTFast.Logging.ConsoleLogger();
            GLTFast.Logging.CollectingLogger collectingLogger = new GLTFast.Logging.CollectingLogger();
            GltfImport gltfImport = new GltfImport(null, null, null, collectingLogger);
            var success = await gltfImport.Load(uri, importSettings);

            if (!success) 
            {
                collectingLogger.LogAll();
                Debug.LogError("Loading glTF failed! \r\n" + uri.AbsoluteUri);
            }
            GltfImportData gltfImportData = new GltfImportData();
            gltfImportData.Name = Path.GetFileName(uri.AbsolutePath);
            gltfImportData.gltfImport = gltfImport;
            return gltfImportData;
        }
    }
}
