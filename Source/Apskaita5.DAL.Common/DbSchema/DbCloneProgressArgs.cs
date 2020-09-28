using System;

namespace Apskaita5.DAL.Common.DbSchema
{
    public sealed class DbCloneProgressArgs
    {

        public DbCloneProgressArgs(Stage currentStage, string currentTable, int rowProgress)
        {
            CurrentStage = currentStage;
            CurrentTable = currentTable ?? throw new ArgumentNullException(nameof(currentTable));
            RowProgress = rowProgress;
        }


        public enum Stage
        {
            FetchingSchema,
            CreatingSchema,
            FetchingRowCount,
            CopyingData,
            Canceled,
            Completed
        }


        public Stage CurrentStage { get; private set; }

        public string CurrentTable { get; private set; }

        public int RowProgress { get; private set; }   

    }

}
