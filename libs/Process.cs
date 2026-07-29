using System;
using System.Collections;

namespace VTT.libs
{
    public class Process
    {
        /// <summary>
        /// chuyen chuoi nhap vao ra 1 chuoi HEX lien tiep
        /// </summary>
        /// <param name="_strA"> chuoi can chuyen</param>
        /// <returns></returns>
        public static string ProcessEncoding(string _strA)
        {
            string hex = null;
            foreach (char c in _strA)
            {
                int tmp = c;
                hex += String.Format("{0:x0}", (uint)System.Convert.ToUInt32(tmp.ToString()) + (ulong)System.Convert.ToChar(tmp));
            }
            _strA = hex;
            hex = null;
            foreach (char c in _strA)
            {
                int tmp = c;
                hex += String.Format("{0:x0}", (uint)System.Convert.ToUInt32(tmp.ToString()) + (ulong)System.Convert.ToChar(tmp));
            }
            return hex;
        }
    }
}
