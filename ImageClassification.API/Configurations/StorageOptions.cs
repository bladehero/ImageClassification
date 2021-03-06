﻿using ImageClassification.API.Interfaces;

namespace ImageClassification.API.Configurations
{
    public class StorageOptions : IConfigurationOptions
    {
        string IConfigurationOptions.SectionPath => nameof(StorageOptions);

        public string StoragePath { get; set; }
    }
}
