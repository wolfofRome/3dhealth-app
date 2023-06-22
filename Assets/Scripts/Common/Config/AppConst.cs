using UnityEngine;

namespace Assets.Scripts.Common.Config
{
    public class AppConst : ScriptableObject
    {
        /// <summary>
        /// 採寸データ表示時のスケール(mm ⇒ cm)
        /// </summary>
        public const float MeasurementDataScale = 0.1f;

        /// <summary>
        /// OBJロード時のスケール
        /// </summary>
        public const float ObjLoadScale = 0.001f;

        /// <summary>
        /// 価格の表示フォーマット
        /// </summary>
        public const string PriceFormat = "#,0円";

        /// <summary>
        /// ポイントの表示フォーマット
        /// </summary>
        public const string PointFormat = "#,0pt";

        /// <summary>
        /// メッセージ中のポイント表示フォーマット
        /// </summary>
        public const string PointFormatInSentence = "#,0ポイント";

        /// <summary>
        /// 日付表示フォーマット
        /// </summary>
        public const string DateFormat = "yyyy/MM/dd";

        /// <summary>
        /// 日時表示フォーマット
        /// </summary>
        public const string DateTimeFormat = "yyyy/MM/dd HH:mm";

        /// <summary>
        /// アドバイスの消費ポイント
        /// </summary>
        public const int AdviceConsumptionPoint = 500;

        /// <summary>
        /// サーバーURL
        /// </summary>
#if DEVELOP
        public const string BaseURL = "https://stg-api.3dbodylab.jp/";
#else
        public const string BaseURL = "https://api.3dbodylab.jp/";
#endif
    }
}
