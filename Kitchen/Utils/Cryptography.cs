//==========================================================================================
//
//		Cryptography
//		Copyright (c) 2007
//
//    Author: Kojomin Dmitry
//
//==========================================================================================

#region ----------------------------- Added namespaces -------------------------------------

using System;
using System.Collections;

#endregion

namespace Cryptography
{
    public sealed class Cryptography:MarshalByRefObject
    {
        #region ------------------------- Cryptography -------------------------------

        /// <summary>
        /// Cryptografic dimension
        /// </summary>
        public int Dim
        {
            get;
            set;
        }

        /// <summary>
        /// Crypts current string.
        /// </summary>
        public string Crypt(string line,string ckey)
        {
            string result = string.Empty;

            try
            {
                int l = 0, s = 0;
                int s0 = int.Parse(ckey.Split('|')[1]);
                int c = int.Parse(ckey.Split('|')[2]);
                int a = int.Parse(ckey.Split('|')[0]);

                Hashtable hs = new Hashtable();
                bool val = true;

                while (val)
                {
                    if (l == 0)
                    {
                        hs.Add(l, s0);
                        s = (int)(hs[l]);
                    }
                    else
                    {
                        hs.Add(l, (a * (int)(hs[l - 1]) + c) % Dim);
                        s = (int)(hs[l]);

                        if (s == s0)
                            val = false;
                    }

                    l++;
                }

                hs.Remove(l - 1);
                int[] m_Gamma = new int[l - 1];

                foreach (object key in hs.Keys)
                    m_Gamma[(int)key] = (int)(hs[key]);

                char[] liter = new char[line.Length];
                int v = 0;

                foreach (char lit in line)
                {
                    liter[v] = lit;
                    v++;
                }

                for (int i = 0; i < line.Length; i++)
                    result += (char)(((int)liter[i]) ^ m_Gamma[i]);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Ошибка в методе шифрования.");
            }

            return result;
        }

        /// <summary>
        /// Decrypts current string.
        /// </summary>
        public string Decrypt(string line, string ckey)
        {
            string result = string.Empty;

            try
            {
                int l = 0, s = 0;
                int s0 = int.Parse(ckey.Split('|')[1]);
                int c = int.Parse(ckey.Split('|')[2]);
                int a = int.Parse(ckey.Split('|')[0]);

                Hashtable hs = new Hashtable();
                bool val = true;

                while (val)
                {
                    if (l == 0)
                    {
                        hs.Add(l, s0);
                        s = (int)(hs[l]);
                    }
                    else
                    {
                        hs.Add(l, (a * (int)(hs[l - 1]) + c) % Dim);
                        s = (int)(hs[l]);
                        
                        if (s == s0)
                            val = false;
                    }

                    l++;
                }

                hs.Remove(l - 1);
                int[] m_Gamma = new int[l - 1];

                foreach (object key in hs.Keys)
                    m_Gamma[(int)key] = (int)(hs[key]);

                char[] liter = new char[line.Length];
                int v = 0;

                foreach (char lit in line)
                {
                    liter[v] = lit;
                    v++;
                }

                for (int i = 0; i < line.Length; i++)
                    result += (char)(((int)liter[i]) ^ m_Gamma[i]);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Ошибка в методе дешифрования.");
            }

            return result;
        }

        /// <summary>
        /// Crypts current string.
        /// </summary>
        public byte[] CryptToBinary(string line, string ckey)
        {
            byte[] result = new byte[line.Length];

            try
            {
                int l = 0, s = 0;
                int s0 = int.Parse(ckey.Split('|')[1]);
                int c = int.Parse(ckey.Split('|')[2]);
                int a = int.Parse(ckey.Split('|')[0]);

                Hashtable hs = new Hashtable();
                bool val = true;

                while (val)
                {
                    if (l == 0)
                    {
                        hs.Add(l, s0);
                        s = (int)(hs[l]);
                    }
                    else
                    {
                        hs.Add(l, (a * (int)(hs[l - 1]) + c) % Dim);
                        s = (int)(hs[l]);

                        if (s == s0)
                            val = false;
                    }

                    l++;
                }

                hs.Remove(l - 1);
                int[] m_Gamma = new int[l - 1];

                foreach (object key in hs.Keys)
                    m_Gamma[(int)key] = (int)(hs[key]);

                char[] liter = new char[line.Length];
                int v = 0;

                foreach (char lit in line)
                {
                    liter[v] = lit;
                    v++;
                }

                for (int i = 0; i < line.Length; i++)
                    result[i] = (byte)(((int)liter[i]) ^ m_Gamma[i]);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Ошибка в методе шифрования.");
            }

            return result;
        }

        /// <summary>
        /// Decrypts current string.
        /// </summary>
        public string DecryptFromBinary(byte[] line, string ckey)
        {
            string result = string.Empty;

            try
            {
                int l = 0, s = 0;
                int s0 = int.Parse(ckey.Split('|')[1]);
                int c = int.Parse(ckey.Split('|')[2]);
                int a = int.Parse(ckey.Split('|')[0]);

                Hashtable hs = new Hashtable();
                bool val = true;

                while (val)
                {
                    if (l == 0)
                    {
                        hs.Add(l, s0);
                        s = (int)(hs[l]);
                    }
                    else
                    {
                        hs.Add(l, (a * (int)(hs[l - 1]) + c) % Dim);
                        s = (int)(hs[l]);

                        if (s == s0)
                            val = false;
                    }

                    l++;
                }

                hs.Remove(l - 1);
                int[] m_Gamma = new int[l - 1];

                foreach (object key in hs.Keys)
                    m_Gamma[(int)key] = (int)(hs[key]);

                for (int i = 0; i < line.Length; i++)
                    result += (char)(line[i] ^ m_Gamma[i]);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Ошибка в методе дешифрования.");
            }

            return result;
        }
        
        /// <summary>
        /// Selects key for Crypting/Decrypting method
        /// </summary>
        public string GetKey(string line)
        {
            string result = string.Empty;

            try
            {
                byte tmp = 0;
                int a = 0;
                int s0 = 0;
                int c = 0;

                for (int i = 0; i < line.Length; i++)
                {
                    tmp = (byte)line[i];

                    if (tmp % 2 == 0)
                        break;
                }

                a = tmp;

                while (a % 4 != 0)
                    a++;

                a += 1;
                s0 = tmp * 3 + 13;
                c = tmp + 7;
                result = a.ToString() + "|" + s0.ToString() + "|" + c.ToString();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Ошибка в методе выбора ключа.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return string.Empty;
            }

            return "53|169|59";//result;
        }

        #endregion ---------------------------------------------------------------
    }
}
