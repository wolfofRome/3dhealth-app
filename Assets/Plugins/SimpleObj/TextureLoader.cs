using UnityEngine;
using System.IO;
using System;

//typedef struct tagBITMAPFILEHEADER {  14Byte  
//        WORD    bfType;               2Byte  
//        DWORD   bfSize;               4Byte  
//        WORD    bfReserved1;          2Byte  
//        WORD    bfReserved2;          2Byte  
//        DWORD   bfOffBits;            4Byte  
//} BITMAPFILEHEADER;
public struct BITMAPFILEHEADER
{
    public ushort bfType;           // ファイルタイプ
    public uint bfSize;             // ファイル全体のサイズ
    public ushort bfReserved1;      // 予約領域
    public ushort bfReserved2;      // 予約領域2
    public uint bfOffBits;          // ファイルの先頭から画像データまでのオフセット数
}

//typedef struct tagBITMAPINFOHEADER{   40Byte  
//        DWORD      biSize;            4Byte  
//        LONG       biWidth;           4Byte  
//        LONG       biHeight;          4Byte  
//        WORD       biPlanes;          2Byte  
//        WORD       biBitCount;        2Byte  
//        DWORD      biCompression;     4Byte  
//        DWORD      biSizeImage;       4Byte  
//        LONG       biXPelsPerMeter;   4Byte  
//        LONG       biYPelsPerMeter;   4Byte  
//        DWORD      biClrUsed;         4Byte  
//        DWORD      biClrImportant;    4Byte  
//} BITMAPINFOHEADER;  
public struct BITMAPINFOHEADER
{
    public uint biSize;             // BITMAPINFOHEADERのサイズ(40)
    public int biWidth;             // ビットマップの幅
    public int biHeight;            // ビットマップの高さ
    public ushort biPlanes;         // プレーン数(常に1)
    public ushort biBitCount;       // １ピクセルあたりのビット数(1, 4, 8, 16, 24, 32)
    public uint biCompression;      // 圧縮形式
    public uint biSizeImage;        // イメージのサイズ(バイト数)
    public int biXPelsPerMeter;     // ビットマップの水平解像度
    public int biYPelsPerMeter;     // ビットマップの垂直解像度
    public uint biClrUsed;          // カラーパレット数
    public uint biClrImportant;     // 重要なカラーパレットのインデックス
}

// 画像を読み込んでテクスチャに変換するクラス
public class TextureLoader
{

    public static Texture2D LoadImage(string filePath)
    {

        // 拡張子の確認
        string ext = Path.GetExtension(filePath);

        if (ext == ".bmp")
        {
            // BMPファイルの場合

            // ファイルを開く
            FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read);

            // データ
            byte[] readData = new byte[4];

            /* BITMAPFILEHEADERの読みこみ */
            BITMAPFILEHEADER bfh;

            // bfType  
            fs.Read(readData, 0, 2);
            bfh.bfType = BitConverter.ToUInt16(readData, 0);

            // bfSize  
            fs.Read(readData, 0, 4);
            bfh.bfSize = BitConverter.ToUInt32(readData, 0);
            // bfReserved1  
            fs.Read(readData, 0, 2);
            bfh.bfReserved1 = BitConverter.ToUInt16(readData, 0);
            // bfReserved2  
            fs.Read(readData, 0, 2);
            bfh.bfReserved2 = BitConverter.ToUInt16(readData, 0);
            // bfOffBits  
            fs.Read(readData, 0, 4);
            bfh.bfOffBits = BitConverter.ToUInt32(readData, 0);

            /* BITMAPINFOHEADERの読み込み */
            BITMAPINFOHEADER bih;

            // biSize  
            fs.Read(readData, 0, 4);
            bih.biSize = BitConverter.ToUInt32(readData, 0);
            // biWidth  
            fs.Read(readData, 0, 4);
            bih.biWidth = BitConverter.ToInt32(readData, 0);
            // biHeight  
            fs.Read(readData, 0, 4);
            bih.biHeight = BitConverter.ToInt32(readData, 0);
            // biPlanes  
            fs.Read(readData, 0, 2);
            bih.biPlanes = BitConverter.ToUInt16(readData, 0);
            //biBitCount  
            fs.Read(readData, 0, 2);
            bih.biBitCount = BitConverter.ToUInt16(readData, 0);
            // biCompression  
            fs.Read(readData, 0, 4);
            bih.biCompression = BitConverter.ToUInt32(readData, 0);
            // biSizeImage  
            fs.Read(readData, 0, 4);
            bih.biSizeImage = BitConverter.ToUInt32(readData, 0);
            // biXPelsPerMeter  
            fs.Read(readData, 0, 4);
            bih.biXPelsPerMeter = BitConverter.ToInt32(readData, 0);
            // biYPelsPerMeter  
            fs.Read(readData, 0, 4);
            bih.biYPelsPerMeter = BitConverter.ToInt32(readData, 0);
            // biClrUsed  
            fs.Read(readData, 0, 4);
            bih.biClrUsed = BitConverter.ToUInt32(readData, 0);
            // biClrImportant  
            fs.Read(readData, 0, 4);
            bih.biClrImportant = BitConverter.ToUInt32(readData, 0);

            /* 画像データの読みこみ */

            // 24bitでなければキャンセル
            if (bih.biBitCount != 24)
            {
                Debug.Log(bih.biBitCount + " bit is not supported");
                return null;
            }

            // テクスチャを作成
            Texture2D texture = new Texture2D(bih.biWidth, bih.biHeight);

            // １ラインあたりのデータ数を計算
            int line = bih.biWidth * bih.biBitCount / 8;
            if ((line % 4) != 0)
            {
                // データ数が4の倍数でなければ4の倍数に合わせる
                line = ((line / 4) + 1) * 4;
            }

            // データを左下から1行ずつ読み込む
            byte[] lineData = new byte[line];
            for (int y = 0; y < bih.biHeight; y++)
            {
                fs.Read(lineData, 0, line);

                // 1画素分ずつ取り出す
                for (int x = 0; x < bih.biWidth; x++)
                {
                    byte B = lineData[x * 3];
                    byte G = lineData[x * 3 + 1];
                    byte R = lineData[x * 3 + 2];

                    // カラーデータを作成
                    Color color = new Color32(R, G, B, 255);

                    // データをテクスチャにセット
                    texture.SetPixel(x, y, color);
                }
            }

            // データを適用
            texture.Apply();

            // 終了処理
            fs.Close();
            fs.Dispose();

            return texture;

        }
        else
        {
            throw new ArgumentException("filePath");
        }
    }
}