public static class UrlUtils
{
    public static string GetCharacterJsonUrl(string id)
    {
        return string.Format(Constant.ModelDataUrl.ModelJsonStrFormat, id);
    }

    public static string GetCharacterAtlasUrl(string id)
    {
        return string.Format(Constant.ModelDataUrl.ModelAtlasStrFormat, id);
    }

    public static string GetCharacterTextureUrl(string id)
    {
        return string.Format(Constant.ModelDataUrl.ModelTextureStrFormat, id);
    }
}