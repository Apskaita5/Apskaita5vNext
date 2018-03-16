using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeveloperUtils
{
    interface IStandardActionForm
    {

        bool CanSave { get; }

        bool CanOpen { get; }

        bool CanCreate { get; }

        bool CanPaste { get; }

        string DefaultExtension { get; }

        string DefaultExtensionDescription { get; }

        string CurrentFilePath { get; }


        void Save(string filePath);

        void Open(string filePath);

        void Create();

        void Paste(string source);

    }
}
