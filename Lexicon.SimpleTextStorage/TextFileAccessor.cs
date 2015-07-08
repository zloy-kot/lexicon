using System;
using System.IO;
using System.Text;
using Lexicon.Common;

namespace Lexicon.SimpleTextStorage
{
    public interface ITextFileAccessor
    {
        void Open(string filename);
        string ReadLine();
        void UpdateLine(string line);
        void RemoveLine();
        void SeekLines(int count);
        void AddLine(string line);
        void Close();
    }

    public class TextFileAccessor : ITextFileAccessor, IDisposable
    {
        internal const int DefaultBufferSize = 128;
        private bool _disposed = true;

        private FileStream _stream;
        private Encoding _encoding;

        public TextFileAccessor()
            : this(Encoding.UTF8)
        {
            //default encoding is Unicode
        }

        public TextFileAccessor(Encoding encoding)
        {
            _encoding = Ensure.IsNotNull(encoding);
        }

        internal long CurrentPosition
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("FileStream");
                return _stream.Position;
            }
        }

        public void Open(string filename)
        {
            Ensure.IsNotNullNorWhiteSpace(filename);
            if (!_disposed)
                Dispose();

            _stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            _encoding = Encoding.UTF8;
            _disposed = false;
        }

        public string ReadLine()
        {
            if (_disposed)
                throw new ObjectDisposedException("FileStream");

            string line;
            int cur, pos = 0, bytesRead = 0;
            byte[] buffer = new byte[DefaultBufferSize];
            
            do
            {
                //read next byte
                cur = _stream.ReadByte();
                if (IsEof(cur))
                {
                    //if it indicates end of the stream
                    //return read chars or null of none
                    line = GetString(buffer, bytesRead, true);
                    return line;
                }

                if (!IsEol(ref cur, false))
                {
                    //if it does not indicate end of the line
                    //put it into the buffer
                    if (pos >= buffer.Length)
                        ExtendBuffer(ref buffer);
                    buffer[pos++] = (byte)cur;
                    bytesRead++;
                }
            }
            while (!IsEol(ref cur));

            line = GetString(buffer, bytesRead, false);
            return line;
        }

        public void SeekLines(int count)
        {
            if (_disposed)
                throw new ObjectDisposedException("FileStream");

            if (count == 0) return;
            if (count > 0)
                SeekLinesForward(count);
            if(count < 0)
                SeekLinesBackward(count);
        }

        private void SeekLinesForward(int count)
        {
            int cur;
            for (int i = 0; i < count; i++)
            {
                do
                {
                    cur = _stream.ReadByte();
                    if (IsEof(cur))
                        return;
                } while (!IsEol(ref cur));
            }
        }

        private void SeekLinesBackward(int count)
        {
            if (_stream.Position == 0) return;
            //remember current position
            long initPos = _stream.Position;
            //list of positions of line endings
            long[] eolPos = new long[1];
            InitializeBuffer(eolPos, 0, -1);

            //start from the beginning
            _stream.Seek(0, SeekOrigin.Begin);
            int i = 0;
            do
            {
                //consume a line
                SeekLinesForward(1);
                //if the end of the line is before current position...
                if (_stream.Position < initPos)
                {
                    if (i >= eolPos.Length)
                        ExtendBuffer(ref eolPos, -1);
                    //it is a potential goal for seeking to
                    eolPos[i++] = _stream.Position;
                }
                else
                //list of line endings that precede current position is put together
                    break;
            } while (true);

            //calculate at what index in line endings list the sought index is
            int soughtPosIndex = i - Math.Abs(count);
            if (soughtPosIndex < 0 || eolPos[soughtPosIndex] < 0)
            //the case when requested number of steps 
            //is greater then total number of lines before current position
                _stream.Seek(0, SeekOrigin.Begin);
            else
                _stream.Seek(eolPos[soughtPosIndex], SeekOrigin.Begin);
        }

        public void AddLine(string line)
        {
            Ensure.IsNotNull(line);

            if (_disposed)
                throw new ObjectDisposedException("FileStream");
            
            //go to the end of the stream
            _stream.Seek(0, SeekOrigin.End);
            //end the line the the newline symbol
            if (!IsPrecededWithEol())
                line = Environment.NewLine + line;
            var buffer = _encoding.GetBytes(line);
            //lengthen the stream to room new line
            _stream.SetLength(_stream.Length + buffer.Length);

            _stream.Write(buffer, 0, buffer.Length);
        }

        private string GetString(byte[] buffer, int bytesRead, bool eof)
        {
            if (bytesRead == 0) 
                return eof ? null : String.Empty;

            byte[] resBytes = new byte[bytesRead];
            Buffer.BlockCopy(buffer, 0, resBytes, 0, bytesRead);
            return _encoding.GetString(resBytes);
        }

        private void ExtendBuffer<T>(ref T[] buffer, T? defaultValue = null) where T: struct
        {
            T[] tmp = new T[buffer.Length];
            buffer.CopyTo(tmp, 0);
            buffer = new T[buffer.Length * 2];
            if (defaultValue.HasValue)
                InitializeBuffer(buffer, tmp.Length, defaultValue.Value);
            tmp.CopyTo(buffer, 0);
        }

        private bool IsEol(ref int @byte, bool seekByteIfDosEol = true)
        {
            if (@byte == '\r')
            {
                if (seekByteIfDosEol)
                {
                    //read next char...
                    int cur = _stream.ReadByte();
                    if (IsEof(cur))
                        return true;
                    //...and seek one char back if it's not a DOS line ending
                    if (cur != '\n')
                        _stream.Seek(-1, SeekOrigin.Current);
                    else
                    //otherwise consume it
                        @byte = cur;
                }
                return true;
            }
            return (@byte == '\n');
        }

        private bool IsPrecededWithEol()
        {
            var initPos = _stream.Position;
            if (initPos == 0)
                return true;

            _stream.Seek(-1, SeekOrigin.Current);
            var @byte = _stream.ReadByte();
            return (@byte == '\r' || @byte == '\n');
        }

        private bool IsEof(int @byte)
        {
            return @byte == -1;
        }

        private void InitializeBuffer<T>(T[] buffer, int offset, T value)
        {
            for (int i = offset; i < buffer.Length; i++)
                buffer[i] = value;
        }

        public void UpdateLine(string line)
        {
            Ensure.IsNotNullNorEmpty(line);

            UpdateLineInternal(line);
        }

        public void RemoveLine()
        {
            UpdateLineInternal(String.Empty);
        }

        private void UpdateLineInternal(string line)
        {
            if (_disposed)
                throw new ObjectDisposedException("FileStream");

            //if the position is in the end
            if (_stream.Position == _stream.Length)
            {
                //and the line is not empty append the line
                if (!String.IsNullOrEmpty(line))
                    AddLine(line);
                return;
            }

            //remember start and end positions of the old line being updated
            long initPos = _stream.Position;
            SeekLinesForward(1);
            long endPos = _stream.Position;

            //the buffer for the chars that come after the end position of the old line
            long followBufferLen = _stream.Length - endPos;
            byte[] followBuffer = new byte[followBufferLen];
            byte[] lineBuffer;

            //an empty line means deletion
            if (String.IsNullOrEmpty(line))
                lineBuffer = new byte[0];
            else
            {
                //if some char come after the old line end the line with the newline symbol
                if (followBufferLen != 0)
                    line = line + Environment.NewLine;
                lineBuffer = _encoding.GetBytes(line);
            }

            //if some chars come after the old line
            if (followBufferLen != 0)
            //remember the chars
                _stream.Read(followBuffer, 0, followBuffer.Length);

            //calculate and set new length of the stream
            long newLen = initPos + lineBuffer.Length + followBuffer.Length;
            _stream.SetLength(newLen);

            //go to the beginning of the old line
            _stream.Seek(initPos, SeekOrigin.Begin);

            //overwrite it with new one
            if (lineBuffer.Length != 0)
                _stream.Write(lineBuffer, 0, lineBuffer.Length);
            //and if there was something after the old line
            if (followBufferLen != 0)
            //overwrite the remaining stream
                _stream.Write(followBuffer, 0, followBuffer.Length);

            //come back to the end position of the new line
            _stream.Seek(initPos + lineBuffer.Length, SeekOrigin.Begin);
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
                _stream.Flush();
                _stream.Dispose();
            }
            _disposed = true;
        }
    }
}