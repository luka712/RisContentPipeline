using Eto.Drawing;

namespace RisContentPipeline.GUI.Extensions
{
    internal static class BitmapExtensions
    {
        /// <summary>
        /// Gets the byte array representation of the bitmap.
        /// </summary>
        /// <param name="bitmap">The <see cref="Bitmap"/>.</param>
        /// <returns>The byte array.</returns>
        public static byte[] GetBytes(this Bitmap bitmap)
        {
            using (var bd = bitmap.Lock())
            {
                int size = bitmap.Height * bd.ScanWidth;
                byte[] bytes = new byte[size];

                System.Runtime.InteropServices.Marshal.Copy(
                    bd.Data, bytes, 0, size);

                return bytes;
            }
        }

        /// <summary>
        /// Executes the specified action with the bitmap data pointer and size.
        /// </summary>
        /// <param name="bitmap">The <see cref="Bitmap"/>.</param>
        /// <param name="action">The action to execute.</param>
        public static void WithBitmapData(this Bitmap bitmap, Action<IntPtr, uint> action)
        {
            using (var bd = bitmap.Lock())
            {
                uint size = (uint) (bitmap.Height * bd.ScanWidth);
                action(bd.Data, size);
            }
        }
    }
}
