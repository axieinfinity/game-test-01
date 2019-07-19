using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

class TextContent
{
    public string Id;
    public string Content;
}


public class CacheService : ICacheService,
    IInitializable
{
    private List<TextContent> _textContentList = new List<TextContent>();

    private string GetDataPath()
    {
        return Path.Combine(Application.persistentDataPath,
            "textdat");
    }

    public void SetCacheText(string id,
        string content)
    {
        _textContentList.Add(new TextContent
        {
            Id = id, Content = content
        });

        try
        {
            var streamWriter = new StreamWriter(GetDataPath(),
                false);
            streamWriter.Write(JsonConvert.SerializeObject(_textContentList,
                Formatting.Indented));
            streamWriter.Close();
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[CacheService] --> {e.Message}");
            File.Delete(GetDataPath());
        }
    }

    public string GetCacheText(string id)
    {
        var textContent = _textContentList?.FirstOrDefault(content => content.Id.Equals(id));
        return textContent?.Content;
    }

    public void Initialize()
    {
        if (!File.Exists(GetDataPath()))
        {
            return;
        }

        try
        {
            var streamReader = new StreamReader(GetDataPath());
            _textContentList = JsonConvert.DeserializeObject<List<TextContent>>(streamReader.ReadToEnd())
                .ToList();
            streamReader.Close();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[CacheService] -->{ex.Message}");
            File.Delete(GetDataPath());
        }
    }
}

public interface ICacheService
{
    string GetCacheText(string id);

    void SetCacheText(string id,
        string content);
}