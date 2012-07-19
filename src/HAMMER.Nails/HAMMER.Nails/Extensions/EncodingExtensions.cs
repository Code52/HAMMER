using System.Text;

namespace HAMMER.Nails.Extensions
{
    public static class EncodingExtensions
    {
        public static string GetString(this Encoding e, byte[] bytes)
        {
            return e.GetString(bytes, 0, bytes.Length);
        }
    }
}