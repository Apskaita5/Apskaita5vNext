using System;
using System.Collections.Generic;
using System.Text;

namespace Apskaita5.Common
{
    /// <summary>
    /// Use this exception to designate the situation when a requested business object is not found.
    /// </summary>
    [Serializable]
    public class BusinessObjectNotFoundException : Exception
    {

        private long _NotFoundId;


        /// <summary>
        /// Gets an id of the object that was requested but not found.
        /// </summary>
        public long NotFoundId => _NotFoundId;
        

        private BusinessObjectNotFoundException() : base() { }

        private BusinessObjectNotFoundException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Use this exception to designate the situation when a requested business object is not found. 
        /// </summary>
        /// <param name="message">a localized error message</param>
        /// <param name="objectId">an id of the object that was requested but not found</param>
        public BusinessObjectNotFoundException(string message, long objectId) : base(message)
        {
            _NotFoundId = objectId;
        }

    }
}
