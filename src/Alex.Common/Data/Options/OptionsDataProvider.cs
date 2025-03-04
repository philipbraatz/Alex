﻿using System.IO;
using Alex.Common.Services;

namespace Alex.Common.Data.Options
{
    public class OptionsDataProvider : IDataProvider<AlexOptions>
    {
        private const string StorageKey = "Settings";

        public AlexOptions Data { get; private set; }

        private readonly IStorageSystem _storage;

        public OptionsDataProvider(IStorageSystem storage)
        {
            _storage = storage;
            Data = new AlexOptions();

            Load();
        }

        public void Load()
        {
            if (!_storage.TryReadJson<AlexOptions>(StorageKey, out var options))
            {
                // no options file?
                options = new AlexOptions();
                if (!_storage.TryWriteJson<AlexOptions>(StorageKey, options))
                {
                    // uhmmm...
                    throw new IOException("Unable to write default AlexOptions!");
                }
            }
        }

        public void Save(AlexOptions data)
        {
            Data = data;

            if (!_storage.TryWriteJson<AlexOptions>(StorageKey, data))
            {
                // uhmmm...
                throw new IOException("Unable to write AlexOptions!");
            }
        }
    }
}
