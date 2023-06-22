//#define SAVE_OBJ_ASSETS
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.Events;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Assets.Scripts.Network;
using Assets.Scripts.Common.Config;
using Assets.Scripts.Common.Data;
using Assets.Scripts.Database.DataRow;
using System.Linq;

namespace Assets.Scripts.Common
{
    public class ObjLoader : BaseLoader
    {
        [Serializable]
        public class ObjLoaderEvent : UnityEvent<ObjLoader, GameObject> { }
        public ObjLoaderEvent OnLoadCompleted;
        
        [SerializeField]
        private ObjLoadConfig _config = default;

        private bool _useMemoryCache = false;
        public bool useMemoryCache
        {
            get
            {
                return _useMemoryCache;
            }
            set
            {
                _useMemoryCache = value;
            }
        }

        private bool _useDiskCache = true;
        public bool useDiskCache
        {
            get
            {
                return _useDiskCache;
            }
            set
            {
                _useDiskCache = value;
            }
        }

        private MtlLoadResult _mtlLoadResult;
        private ObjLoadResult _objLoadResult;

        // メモリキャッシュはアプリ内で1つのみ保持するためstaticにしておく.
        private static MemoryCache _memoryCache = new MemoryCache();

        private class MemoryCache
        {
            private LoadParam _loadParam;
            public LoadParam loadParam
            {
                get
                {
                    return _loadParam;
                }
                set
                {
                    _loadParam = value;
                }
            }

            private MtlLoadResult _mtlLoadResult = null;
            public MtlLoadResult mtlLoadResult
            {
                get
                {
                    return _mtlLoadResult;
                }
                set
                {
                    _mtlLoadResult = value;
                }
            }

            private ObjLoadResult _objLoadResult = null;
            public ObjLoadResult objLoadResult
            {
                get
                {
                    return _objLoadResult;
                }
                set
                {
                    if (_objLoadResult != null && _objLoadResult.threeDObject != null)
                    {
                        Destroy(_objLoadResult.threeDObject);
                    }
                    DontDestroyOnLoad(value.threeDObject);
                    _objLoadResult = value;
                }
            }

            public void SetActive(bool value)
            {
                if (_objLoadResult != null && _objLoadResult.threeDObject != null)
                {
                    _objLoadResult.threeDObject.SetActive(value);
                }
            }
        }

        public Material[] Opaques {
            get {
                return _objLoadResult != null ? _objLoadResult.opaques : null;
            }
        }
        
        public Material[] Fades {
            get {
                return _objLoadResult != null ? _objLoadResult.fades : null;
            }
        }

        public string[] ObjInfo {
            get {
                return _objLoadResult != null ? _objLoadResult.objInfo : null;
            }
        }
        
        public GameObject Target3DModelObject {
            get {
                return _objLoadResult != null ? _objLoadResult.threeDObject : null;
            }
        }
        
        private abstract class BaseLoadResult
        {
            public string srcString { get; set; }
            public abstract bool isLoadCompleted { get; }
        }

        private class MtlLoadResult : BaseLoadResult
        {
            public Hashtable textures { get; set; }
            public Hashtable[] mtls { get; set; }

            public MtlLoadResult()
            {
                Clear();
            }

            public void Clear()
            {
                srcString = null;
                textures = null;
                mtls = null;
            }

            public override bool isLoadCompleted {
                get {
                    return (textures != null && textures.Count > 0) && (mtls != null && mtls.Length > 0);
                }
            }
        }

        private class ObjLoadResult : BaseLoadResult
        {
            public GameObject threeDObject { get; set; }
            public Material[] opaques { get; set; }
            public Material[] fades { get; set; }
            public string[] objInfo { get; set; }

            public ObjLoadResult()
            {
                Clear();
            }

            public void Clear()
            {
                srcString = null;
                objInfo = null;
                opaques = null;
                fades = null;
                if (threeDObject != null)
                {
                    Destroy(threeDObject);
                }
                threeDObject = null;
            }

            public override bool isLoadCompleted {
                get {
                    return (threeDObject != null);
                }
            }
        }

        void Start()
        {
        }

        void Update()
        {
        }

        private void OnDisable()
        {
            _memoryCache.SetActive(false);
        }

        /// <summary>
        /// 3Dモデルの読込処理.
        /// </summary>
        /// <param name="content">3Dモデル読込用のロードパラメータ</param>
        public void Load(LoadParam content)
        {
            // Clear previous model
            if (_objLoadResult != null && _objLoadResult.isLoadCompleted)
            {
                _objLoadResult.Clear();
            }

            // 3Dモデルが同じ場合はメモリキャッシュを使用.
            if (useMemoryCache && content.Equals(_memoryCache.loadParam))
            {
                _memoryCache.SetActive(true);
                _objLoadResult = _memoryCache.objLoadResult;
                _mtlLoadResult = _memoryCache.mtlLoadResult;
                OnLoadCompleted.Invoke(this, _objLoadResult.threeDObject);
            }
            else
            {
                StartCoroutine(DownloadAndImportFile(content, Quaternion.Euler(Vector3.zero)));
            }
        }
        
        /// <summary>
        /// 3Dモデルのダウンロード ＋ 読込処理.
        /// </summary>
        /// <param name="content">3Dモデル読込用のロードパラメータ</param>
        /// <param name="rotate">3Dモデル読込時の向き</param>
        /// <returns></returns>
        private IEnumerator DownloadAndImportFile(LoadParam content, Quaternion rotate)
        {
            Vector3 scale = new Vector3(AppConst.ObjLoadScale, AppConst.ObjLoadScale, AppConst.ObjLoadScale);
            int userID = DataManager.Instance.Profile.UserId;
            var dataSet = new Dictionary<FileType, ScanData>() {
                { FileType.Obj, new ScanData { Type = FileType.Obj }},
                { FileType.Mtl, new ScanData { Type = FileType.Mtl }},
                { FileType.Texture, new ScanData { Type = FileType.Texture }},
            };
            bool isLoaded = false;

            // OBJをキャッシュから読込.
            yield return LoadFromCache(userID, content, rotate, scale, ret => { isLoaded = ret; });

            if (!isLoaded)
            {
                // キャッシュから読み込めない場合はサーバーからダウンロード.
                byte[] zip = null;
                string[] filePathList = null;

                // スキャンデータ(ZIP)のダウンロード.
                yield return StartCoroutine(ApiHelper.Instance.GetFileAsBinary(content.Path, res => {
                    if (res.ErrorCode != null)
                    {
                        DebugUtil.LogError("GetFileAsBinary -> " + res.ErrorCode + " : " + res.ErrorMessage);
                    }
                    else
                    {
                        zip = res.Data;
                    }
                }));

                // スキャンデータ(ZIP)の解凍.
                yield return Unzip(zip, DataManager.Instance.CachePath.GetResourceDirPath(content.ResourceId), retval => filePathList = retval);

                foreach (string path in filePathList)
                {
                    string ext = Path.GetExtension(path).ToLower();
                    if (FileType.Mtl.GetExtension().Equals(ext))
                    {
                        dataSet[FileType.Mtl].Path = path;
                    }
                    else if (FileType.Obj.GetExtension().Equals(ext))
                    {
                        dataSet[FileType.Obj].Path = path;
                    }
                    else if (FileType.Texture.GetExtension().Equals(ext))
                    {
                        dataSet[FileType.Texture].Path = path;
                    }
                }

                // MTLファイル、テクスチャ画像のロード.
                yield return LoadMtlFile(dataSet[FileType.Mtl].Path, dataSet[FileType.Texture].Path, ret => { _mtlLoadResult = ret; });

                // OBJファイルのロード.
                yield return LoadObjFile(dataSet[FileType.Obj].Path, _mtlLoadResult, rotate, scale, ret => { _objLoadResult = ret; });

                // ディスクキャッシュ有効時は解凍後のファイルパスをDBに保存、無効時はファイルを削除.
                if (useDiskCache && _mtlLoadResult.isLoadCompleted && _objLoadResult.isLoadCompleted)
                {
                    foreach (var sd in dataSet.Values)
                    {
                        sd.UserId = userID;
                        sd.ResourceId = content.ResourceId;
                        sd.CreateTime = content.CreateTime;
                        sd.ModifiedTime = content.ModifiedTime;
                        DataManager.Instance.Cache.InsertOrReplace(sd);
                    }
                }
                else
                {
                    DeleteFiles(filePathList);
                }
            }

            // メモリキャッシュを更新.
            if (useMemoryCache)
            {
                _memoryCache.loadParam = content;
                _memoryCache.objLoadResult = _objLoadResult;
                _memoryCache.mtlLoadResult = _mtlLoadResult;
            }

            if (OnLoadCompleted != null)
            {
                OnLoadCompleted.Invoke(this, _objLoadResult.threeDObject);
            }
        }

        /// <summary>
        /// ファイルの削除.
        /// </summary>
        /// <param name="filePathList"></param>
        private void DeleteFiles(string[] filePathList)
        {
            foreach (string path in filePathList)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        /// <summary>
        /// ディスクキャッシュからロード.
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="content">3Dモデル読込用のロードパラメータ</param>
        /// <param name="rotate">3Dモデル読込時の向き</param>
        /// <param name="scale">3Dモデル読込時の拡縮率</param>
        /// <param name="callback">コールバック処理</param>
        /// <returns></returns>
        private IEnumerator LoadFromCache(int userId, LoadParam content, Quaternion rotate, Vector3 scale, Action<bool> callback)
        {
            var dataSet = new Dictionary<FileType, ScanData>() {
                { FileType.Obj, new ScanData { Type = FileType.Obj }},
                { FileType.Mtl, new ScanData { Type = FileType.Mtl }},
                { FileType.Texture, new ScanData { Type = FileType.Texture }},
            };

            var db = DataManager.Instance.Cache;

            // キャッシュDBに登録済、且つファイルが存在するかチェック.
            foreach (ScanData sd in dataSet.Values)
            {
                ScanData cacheSd = db.GetScanData(userId, content.ResourceId, sd.Type);
                if (cacheSd != null && cacheSd.ModifiedTime >= content.ModifiedTime && File.Exists(cacheSd.Path))
                {
                    sd.Path = cacheSd.Path;
                }
            }

            var cacheEmptyNum = dataSet.Values
                .Where(v => v.Path == null)
                .ToArray().Count();

            if (cacheEmptyNum == 0)
            {
                yield return LoadMtlFile(dataSet[FileType.Mtl].Path, dataSet[FileType.Texture].Path, ret => { _mtlLoadResult = ret; });
                yield return LoadObjFile(dataSet[FileType.Obj].Path, _mtlLoadResult, rotate, scale, ret => { _objLoadResult = ret; });
            }

            callback((_mtlLoadResult != null && _mtlLoadResult.isLoadCompleted) &&
                     (_objLoadResult != null && _objLoadResult.isLoadCompleted));
        }

        /// <summary>
        /// MTLファイルのロード.
        /// </summary>
        /// <param name="mtlPath">MTLファイルのパス</param>
        /// <param name="texturePath">テクスチャファイルのパス</param>
        /// <param name="callback">コールバック処理</param>
        /// <returns></returns>
        private IEnumerator LoadMtlFile(string mtlPath, string texturePath, Action<MtlLoadResult> callback)
        {
            var ret = new MtlLoadResult();

            ret.srcString = File.ReadAllText(mtlPath);
            if (ret.srcString != null && ret.srcString.Length > 0)
            {
                ret.mtls = ObjImporter.ImportMaterialSpecs(ret.srcString);

                for (int i = 0; i < ret.mtls.Length; i++)
                {
                    if (ret.mtls[i].ContainsKey("mainTexName"))
                    {
                        Texture2D texture = TextureLoader.LoadImage(texturePath);
                        if (texture != null)
                        {
                            if (ret.textures == null) ret.textures = new Hashtable();
                            ret.textures[ret.mtls[i]["mainTexName"]] = texture;
                        }
                        yield return 0;
                    }
                }
            }
            callback(ret);
        }

        /// <summary>
        /// OBJファイルのロード.
        /// </summary>
        /// <param name="objPath">OBJファイルのパス</param>
        /// <param name="mtl">MTLのロード結果</param>
        /// <param name="rotate">3Dモデル読込時の角度</param>
        /// <param name="scale">3Dモデル読込時の拡縮率</param>
        /// <param name="callback">コールバック処理</param>
        /// <returns></returns>
        private IEnumerator LoadObjFile(string objPath, MtlLoadResult mtl, Quaternion rotate, Vector3 scale, Action<ObjLoadResult> callback)
        {
            var ret = new ObjLoadResult();

            ret.srcString = File.ReadAllText(objPath);
            if (ret.srcString != null && ret.srcString.Length > 0)
            {
                yield return StartCoroutine(ObjImporter.ImportInBackground(ret.srcString, mtl.srcString, mtl.textures, rotate, scale, Vector2.zero, retval => ret.threeDObject = retval, false, false, true));
                ret.objInfo = ret.srcString.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                
                if (ret.threeDObject != null)
                {
                    // Set Standard(Fade) Materials
                    if (mtl.srcString != null && mtl.srcString.Length > 0)
                    {
                        Renderer[] renderers = ret.threeDObject.GetComponentsInChildren<Renderer>();
                        foreach (Renderer r in renderers)
                        {
                            List<Material> nextMaterials = new List<Material>();

                            int count = r.materials.Length;
                            for (int i = 0; i < count; i++)
                            {
                                Material nextMat = new Material(_config.opaqueMaterial);
                                nextMat.mainTexture = r.materials[i].mainTexture;
                                nextMaterials.Add(nextMat);
#if SAVE_OBJ_ASSETS
                                AssetDatabase.CreateAsset(nextMat.mainTexture, "Assets/" + nextMat.mainTexture.name + i + "_texture.asset");
                                AssetDatabase.CreateAsset(nextMat, "Assets/" + nextMat.name + ".asset");
#endif
                            }

                            ret.opaques = nextMaterials.ToArray();
                            r.materials = ret.opaques;

                            nextMaterials = new List<Material>();

                            count = r.materials.Length;
                            for (int i = 0; i < count; i++)
                            {
                                Material nextMat = new Material(_config.fadeMaterial);
                                nextMat.mainTexture = r.materials[i].mainTexture;

                                nextMaterials.Add(nextMat);
                            }

                            ret.fades = nextMaterials.ToArray();
                        }
                    }

#if SAVE_OBJ_ASSETS
                    var mf = _target3DModelObject.GetComponent<MeshFilter>();
                    AssetDatabase.CreateAsset(mf.mesh, "Assets/" + mf.mesh.name + ".asset");
                    AssetDatabase.SaveAssets();
#endif
                }
            }
            callback(ret);
        }

        /// <summary>
        /// ZIP解凍処理.
        /// </summary>
        /// <param name="src">バイト配列</param>
        /// <param name="unzip_dir">解凍先ディレクトリ</param>
        /// <param name="callback">コールバック処理</param>
        /// <returns></returns>
        private IEnumerator Unzip(byte[] src, string unzip_dir, System.Action<string[]> callback)
        {
            List<string> path_list = new List<string>();
            MemoryStream ms = new MemoryStream(src);
            ms.Seek(0, SeekOrigin.Begin);
            
            ZipInputStream zipInputStream = new ZipInputStream(ms);
            ZipEntry zipEntry = zipInputStream.GetNextEntry();
            while (zipEntry != null)
            {
                String entryFileName = zipEntry.Name;
                // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                // Optionally match entrynames against a selection list here to skip as desired.
                // The unpacked length is available in the zipEntry.Size property.

                byte[] buffer = new byte[4096];     // 4K is optimum

                // Manipulate the output filename here as desired.
                string fullZipToPath = Path.Combine(unzip_dir, entryFileName);
                string directoryName = Path.GetDirectoryName(fullZipToPath);
                if (directoryName.Length > 0)
                {
                    Directory.CreateDirectory(directoryName);
                }

                // Skip directory entry
                string fileName = Path.GetFileName(fullZipToPath);
                if (fileName.Length == 0)
                {
                    zipEntry = zipInputStream.GetNextEntry();
                    continue;
                }

                // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                // of the file, but does not waste memory.
                // The "using" will close the stream even if an exception occurs.
                using (FileStream streamWriter = File.Create(fullZipToPath))
                {
                    StreamUtils.Copy(zipInputStream, streamWriter, buffer);
                }

                zipEntry = zipInputStream.GetNextEntry();

                path_list.Add(fullZipToPath);

                yield return 0;
            }

            callback(path_list.ToArray());
        }
    }
}