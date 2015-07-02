using System;
using System.IO;
using System.Text;
using Lexicon.Common;

namespace Lexicon.SimpleTextStorage
{
    public interface ITextFileModifier
    {
        void Open(string filename);
        string ReadLine();
        void Close();
    }

    public class TextFileModifier : ITextFileModifier, IDisposable
    {
        private StreamReader _currentReader;
        private bool _disposed = true;

        public void Open(string filename)
        {
            Ensure.IsNotNullNorWhiteSpace(filename);
            if (!_disposed)
                Dispose();

            var stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Read);
            _currentReader = new StreamReader(stream, Encoding.UTF8);
            _disposed = false;
        }

        public string ReadLine()
        {
            if (_disposed)
                throw new ObjectDisposedException("StreamReader");
            return _currentReader.ReadLine();
        }

        public void Close()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _currentReader.Close();
                _currentReader.Dispose();
            }
            _disposed = true;
        }
    }
}