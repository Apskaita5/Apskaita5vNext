using System;

namespace Apskaita5.DAL.Common
{
    /// <summary>
    /// A data proxy class used to serialize LightDataRow.
    /// </summary>
    [Serializable]
    public sealed class LightDataRowProxy
    {

        public Object[] Values { get; set; }

    }
}
