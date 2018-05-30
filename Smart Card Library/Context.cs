using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartCard.Library
{
    public enum Scopes
    {
        User     = 0,
        Terminal = 1,
        System   = 2
    }
    
    
    /// <summary>
    /// The Resource Manager Context or Scope.
    /// </summary>
    public class Context
    {
        private IntPtr m_hContext;

        public Context()
        {
            m_hContext = IntPtr.Zero;
        }
        
        /// <summary>
        /// Establishes the Resource Manager Context within which operations
        /// are performed.
        /// </summary>
        /// <param name="scope">The scope to use in the established context.</param>
        /// <exception cref="System.Exception">Thrown if context could not be established.</exception>
        public void Establish(Scopes scope)
        {
            int result = Interop.SCardEstablishContext((uint)scope, IntPtr.Zero, IntPtr.Zero, out m_hContext);
            if (result != 0)
            {
                throw new Exception(string.Format("Failed to establish Resource Manager Context. Error Code: {0}.", result));
            }
        }

        /// <summary>
        /// Establishes the Resource Manager Context within which operations
        /// are performed.
        /// </summary>
        /// <exception cref="System.Exception">Thrown if context could not be established.</exception>
        public void Establish()
        {
            Establish(Scopes.System);
        }

        /// <summary>
        /// Releases the Resource Manager Context.
        /// </summary>
        /// <exception cref="System.Exception">Thrown if context could not be established.</exception>
        public void Release()
        {
            int result = Interop.SCardReleaseContext(m_hContext);
            if (result != 0)
            {
                throw new Exception(string.Format("Failed to release Resource Manager Context. Error Code: {0}", result));
            }
        }

        /// <summary>
        /// Provides a list of installed smart card readers.
        /// </summary>
        /// <returns>Returns a list of <typeparamref name="SmartCard.Library.Reader"/> objects.</returns>
        public List<Reader> ListReaders()
        {
            uint pcchReaders = 0;
            int result = Interop.SCardListReaders(m_hContext, null, null, ref pcchReaders);

            byte[] mszReaders = new byte[pcchReaders];
            result = Interop.SCardListReaders(m_hContext, null, mszReaders, ref pcchReaders);

            ASCIIEncoding asciiEncoder = new ASCIIEncoding();
            string readersString = asciiEncoder.GetString(mszReaders);
            int length = (int)pcchReaders;
            int nullindex = -1;
            List<Reader> readers = new List<Reader>();
            if (length > 0)
            {
                while (readersString[0] != (char)0)
                {
                    nullindex = readersString.IndexOf((char)0);
                    string reader = readersString.Substring(0, nullindex);
                    readers.Add(new Reader(reader));
                    length = length - (reader.Length + 1);
                    readersString = readersString.Substring(nullindex + 1, length);
                }
            }

            return readers;
        }
    }
}
