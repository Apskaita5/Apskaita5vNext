using System;
using System.Collections.Generic;

namespace Apskaita5.DAL.Common
{
    /// <summary>
    /// A data proxy class used to serialize LightDataTable.
    /// </summary>
    [Serializable]
    public sealed class LightDataTableProxy
    {

        public List<LightDataColumnProxy> Columns { get; set; }

        public List<LightDataRowProxy> Rows { get; set; }

        public string TableName { get; set; }

        public List<string> DateTimeFormats { get; set; }

    }
}
