using System.IO;

namespace Matrix.Util.Extentions
{
    public static partial class Extention
    {
        /// <summary>
        /// 如果文件夹不存在，则创建
        /// </summary>
        /// <param name="direcotry">文件夹本地路径</param>
        public static void CreateDirectoryIfNotExists(this string direcotry)
        {
            if (direcotry.IsBlank())
            {
                return;
            }
            if (!Directory.Exists(direcotry))
            {
                Directory.CreateDirectory(direcotry);
            }
        }
    }
}
