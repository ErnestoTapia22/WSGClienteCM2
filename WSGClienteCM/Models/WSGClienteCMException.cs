using System;

namespace WSGClienteCM.Models
{
    public class WSGClienteCMException:Exception
    {
        private int _errorId;
        public int ErrorId => _errorId;

        public WSGClienteCMException() { }

        public WSGClienteCMException(string message) : base(message) { }

        public WSGClienteCMException(string message, Exception innerException) : base(message, innerException) { }

        public WSGClienteCMException(string message, int errorId) : base(message) => this._errorId = errorId;
    }
}
